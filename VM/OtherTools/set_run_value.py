"""Point LiveXP's one startup entry at the demo, in place inside VirtualXP.iso.

The entry lives in I386\\SYSTEM32\\CONFIG\\DEFAULT:

    Software\\Microsoft\\Windows\\CurrentVersion\\Run
        ScreenSaver = %PEsys32%\\screensaver.exe

A registry string is stored in its own cell, and the cell is not resized when
the string shrinks, so a shorter replacement can be written straight over it and
the length field in the 'vk' record updated. Nothing moves, which means this
works directly on the ISO the same way set_resolution.py does: no rebuilt image,
no mounted hive, no admin rights.

    "%PEsys32%\\screensaver.exe"  25 chars
    "cmd /k D:\\setup.bat"         19 chars, fits

Replacing screensaver.exe itself was the obvious alternative and it does not
work: the shell never finishes starting. Leaving the file alone and changing
only what the Run key points at avoids that, and setup.bat launches
screensaver.exe itself so LiveXP still gets whatever it does.

Usage:
    python set_run_value.py VirtualXP.iso                     # show current value
    python set_run_value.py VirtualXP.iso "cmd /k D:\\setup.bat"
"""

import struct
import sys

VALUE_NAME = b"ScreenSaver"


def find_vk(buf):
    """Locate the ScreenSaver vk record, identified by its current data."""
    hits = []
    pos = 0
    while True:
        pos = buf.find(b"vk", pos)
        if pos < 0:
            return hits
        name_len = struct.unpack_from("<H", buf, pos + 2)[0]
        if name_len == len(VALUE_NAME) and bytes(buf[pos + 20:pos + 20 + name_len]) == VALUE_NAME:
            data_size = struct.unpack_from("<I", buf, pos + 4)[0]
            data_off = struct.unpack_from("<I", buf, pos + 8)[0]
            if not (data_size & 0x80000000):
                hits.append((pos, data_size, data_off))
        pos += 2


def hive_bases(buf):
    """Offsets of every hive in the file. Several are stored back to back in the
    ISO, and cell offsets are relative to the start of whichever one they are
    in, so the position of the containing hive has to be found rather than
    assumed."""
    out, pos = [], 0
    while True:
        pos = buf.find(b"regf", pos)
        if pos < 0:
            return out
        out.append(pos)
        pos += 4


def run(path, new_value=None):
    with open(path, "rb") as f:
        buf = bytearray(f.read())

    bases = hive_bases(buf) or [0]

    candidates = []
    for pos, size, off in find_vk(buf):
        # cell data sits after the hbin-relative offset plus a 4 byte size header,
        # measured from the start of the hive this record belongs to
        base = max([b for b in bases if b <= pos], default=0)
        start = base + 4096 + off + 4
        if start + size > len(buf):
            continue
        try:
            current = bytes(buf[start:start + size]).decode("utf-16-le").rstrip("\0")
        except UnicodeDecodeError:
            continue
        if "screensaver" in current.lower() or "setup.bat" in current.lower():
            candidates.append((pos, size, start, current))

    seen, uniq = set(), []
    for c in candidates:
        if c[2] not in seen:
            seen.add(c[2])
            uniq.append(c)

    if len(uniq) != 1:
        raise SystemExit("expected exactly one ScreenSaver value, found %d" % len(uniq))

    vk_pos, size, start, current = uniq[0]
    print("  current: %r (%d bytes)" % (current, size))

    if new_value is None:
        return

    encoded = new_value.encode("utf-16-le") + b"\0\0"
    if len(encoded) > size:
        raise SystemExit("error: %r needs %d bytes, only %d available"
                         % (new_value, len(encoded), size))

    buf[start:start + len(encoded)] = encoded
    buf[start + len(encoded):start + size] = b"\0" * (size - len(encoded))
    struct.pack_into("<I", buf, vk_pos + 4, len(encoded))

    with open(path, "wb") as f:
        f.write(buf)
    print("  new:     %r (%d bytes)" % (new_value, len(encoded)))


def main():
    if len(sys.argv) not in (2, 3):
        raise SystemExit(__doc__)
    print("patching %s" % sys.argv[1])
    run(sys.argv[1], sys.argv[2] if len(sys.argv) == 3 else None)


if __name__ == "__main__":
    main()
