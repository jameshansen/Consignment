"""Read and write a file inside the raw NTFS image, from the host.

This resolves the file properly through its MFT record rather than searching the
image for its text, which matters for two reasons.

First, correctness: a file's bytes only count as data if the MFT agrees. NTFS
records a **ValidDataLength** per $DATA attribute and returns zeros for anything
past it, no matter what the clusters hold. The demo image arrived with
C:\\STARTDEMO.CMD at real size 5118 and ValidDataLength 0, the signature of a copy
whose metadata was committed and whose data never was (an emulator killed
mid-write will do it). The guest read 5118 zero bytes, ran nothing, and the shell
fell straight through to shutdown; patching the clusters by hand changed nothing,
because nothing past VDL is ever read.

Second, it means the size may change: writing through the MFT updates the data
size, so a replacement script no longer has to be padded to the byte length of the
one it replaces.

Only what this build needs is implemented: one non-resident, uncompressed $DATA
attribute per file, which is what every script on C: is. Resident files (under
about 700 bytes) are rejected rather than half-handled.

    python ntfs_file.py info  <image> <NAME.EXT>
    python ntfs_file.py read  <image> <NAME.EXT> [out]
    python ntfs_file.py write <image> <NAME.EXT> <local-file>
"""

import mmap
import os
import re
import struct
import sys

MFT_RECORD_MAX = 1024
ATTR_DATA = 0x80


def partition_start(mm):
    """Byte offset of the first NTFS partition, via the MBR table."""
    if mm[0x1FE:0x200] != b"\x55\xAA":
        return 0                       # already a bare filesystem
    for i in range(4):
        e = 0x1BE + i * 16
        if mm[e + 4] in (0x07, 0x17):  # NTFS/exFAT type
            return struct.unpack_from("<I", mm, e + 8)[0] * 512
    return 0


def geometry(mm, base):
    bps = struct.unpack_from("<H", mm, base + 0x0B)[0]
    spc = mm[base + 0x0D]
    return bps, spc, bps * spc


def find_record(mm, name):
    """MFT record offset for a file, found via its $FILE_NAME in UTF-16."""
    enc = name.upper().encode("utf-16-le")
    for m in re.finditer(re.escape(enc), mm):
        start = mm.rfind(b"FILE0", max(0, m.start() - MFT_RECORD_MAX), m.start())
        if start < 0:
            continue
        if struct.unpack_from("<H", mm, start + 0x16)[0] & 1:   # in use
            return start
    return None


def data_attr(mm, rec):
    """(attr_offset, resident, real_size, valid_len, runlist_offset)."""
    off = struct.unpack_from("<H", mm, rec + 0x14)[0]
    while True:
        a = rec + off
        atype = struct.unpack_from("<I", mm, a)[0]
        if atype == 0xFFFFFFFF:
            raise SystemExit("no $DATA attribute")
        alen = struct.unpack_from("<I", mm, a + 4)[0]
        if alen == 0:
            raise SystemExit("corrupt attribute list")
        if atype == ATTR_DATA:
            if not mm[a + 8]:
                raise SystemExit("resident $DATA; this tool only handles "
                                 "non-resident files")
            real = struct.unpack_from("<Q", mm, a + 0x30)[0]
            valid = struct.unpack_from("<Q", mm, a + 0x38)[0]
            runoff = struct.unpack_from("<H", mm, a + 0x20)[0]
            return a, real, valid, a + runoff
        off += alen


def runs(mm, pos):
    """Decode a data runlist into [(lcn, cluster_count), ...]."""
    out = []
    lcn = 0
    while True:
        head = mm[pos]
        if head == 0:
            return out
        lenlen, offlen = head & 0x0F, head >> 4
        pos += 1
        count = int.from_bytes(mm[pos:pos + lenlen], "little")
        pos += lenlen
        delta = int.from_bytes(mm[pos:pos + offlen], "little", signed=True)
        pos += offlen
        lcn += delta
        out.append((lcn, count))


def extents(mm, name):
    base = partition_start(mm)
    _, _, csize = geometry(mm, base)
    rec = find_record(mm, name)
    if rec is None:
        raise SystemExit("%s not found in the MFT" % name)
    a, real, valid, runoff = data_attr(mm, rec)
    spans = [(base + lcn * csize, n * csize) for lcn, n in runs(mm, runoff)]
    return rec, a, real, valid, csize, spans


def info(image, name):
    with open(image, "rb") as f:
        mm = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)
        rec, a, real, valid, csize, spans = extents(mm, name)
        print("%s: mft@%d size=%d validlen=%d%s" %
              (name, rec, real, valid, "   <-- READS AS ZEROS" if valid < real else ""))
        print("  cluster=%d  extents=%s" % (csize, spans))
        mm.close()


def read(image, name, out=None):
    with open(image, "rb") as f:
        mm = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)
        _, _, real, valid, _, spans = extents(mm, name)
        body = b""
        for off, ln in spans:
            body += mm[off:off + ln]
        body = body[:real]
        if valid < real:
            print("(validlen=%d, so the guest actually sees %d zero bytes)"
                  % (valid, real))
        mm.close()
    if out:
        open(out, "wb").write(body)
        print("wrote %s (%d bytes)" % (out, len(body)))
    else:
        sys.stdout.write(body.decode("cp1252", "replace"))


def write(image, name, local):
    body = open(local, "rb").read()
    with open(image, "r+b") as f:
        mm = mmap.mmap(f.fileno(), 0)
        rec, a, real, valid, csize, spans = extents(mm, name)
        room = sum(n for _, n in spans)
        if len(body) > room:
            raise SystemExit("%d bytes does not fit in %d allocated"
                             % (len(body), room))
        pos = 0
        for off, ln in spans:
            chunk = body[pos:pos + ln]
            if not chunk:
                break
            mm[off:off + len(chunk)] = chunk
            pos += len(chunk)
        # Data size and ValidDataLength both move to the new length, so the guest
        # sees exactly these bytes and not a zero-filled hole.
        struct.pack_into("<Q", mm, a + 0x30, len(body))
        struct.pack_into("<Q", mm, a + 0x38, len(body))
        mm.flush()
        mm.close()
    print("%s: wrote %d bytes from %s, size and validlen set to %d"
          % (name, len(body), local, len(body)))


def main():
    if len(sys.argv) < 4:
        raise SystemExit(__doc__)
    cmd, image, name = sys.argv[1], sys.argv[2], sys.argv[3]
    if cmd == "info":
        info(image, name)
    elif cmd == "read":
        read(image, name, sys.argv[4] if len(sys.argv) > 4 else None)
    elif cmd == "write":
        write(image, name, sys.argv[4])
    else:
        raise SystemExit(__doc__)


if __name__ == "__main__":
    main()
