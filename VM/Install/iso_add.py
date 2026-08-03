"""Add files and directories to VirtualXP.iso without moving anything.

Rebuilding this image with a normal ISO tool leaves SETUPLDR.BIN stuck at
"Please insert the disk labeled Boot Disk into Drive A:". Rather than work out
which of the thousand things a rebuild changes is the one that matters, this
never relocates existing data: the El Torito boot image, the boot catalog and
every existing file extent keep the sector they already have.

New content is appended past the end of the image and the volume space size is
grown to match. Directory records are inserted into the slack that already
exists at the end of a directory's allocated extent; if a directory outgrows
that, only that directory is relocated. Both name trees are maintained, and the
path tables are rebuilt into fresh sectors, since adding directories renumbers
them.

Records are inserted in sorted order, which matters: ISO9660 requires it and
SETUPLDR relies on it. Putting a name out of order makes everything sorting
between it and its neighbour unreachable at boot.

Usage:
    python iso_add.py VirtualXP.iso local.dll=/I386/SYSTEM32/MSCOREE.DLL ...
    python iso_add.py VirtualXP.iso localdir/=/I386/MICROSOFT.NET

A trailing slash on the source adds the directory tree recursively. ISO9660
names are uppercased; the Joliet tree keeps the name as given.
"""

import os
import struct
import sys

SECTOR = 2048
DIR_FLAG = 0x02


# --------------------------------------------------------------- descriptors

class Volume(object):
    """One volume descriptor: the primary tree, or the Joliet one."""

    def __init__(self, buf, vd_sector, joliet):
        self.vd = vd_sector * SECTOR
        self.joliet = joliet
        self.buf = buf

    def _u32(self, off):
        return struct.unpack_from("<I", self.buf, self.vd + off)[0]

    def set_both(self, off, value, size=4):
        """Volume descriptor fields are stored little then big endian."""
        if size == 4:
            struct.pack_into("<I", self.buf, self.vd + off, value)
            struct.pack_into(">I", self.buf, self.vd + off + 4, value)
        else:
            struct.pack_into("<H", self.buf, self.vd + off, value)
            struct.pack_into(">H", self.buf, self.vd + off + 2, value)

    @property
    def space_size(self):
        return self._u32(80)

    @property
    def root_offset(self):
        return self.vd + 156

    def encode(self, name):
        return name.encode("utf-16-be") if self.joliet else name.upper().encode("ascii")

    def decode(self, raw):
        return raw.decode("utf-16-be") if self.joliet else raw.decode("ascii")

    def pad_char(self):
        return b"\x00\x20" if self.joliet else b"\x20"


def find_volumes(buf):
    """Primary descriptor is at sector 16; scan on for a Joliet supplementary."""
    primary = joliet = None
    s = 16
    while True:
        off = s * SECTOR
        vtype = buf[off]
        if buf[off + 1:off + 6] != b"CD001":
            break
        if vtype == 1:
            primary = Volume(buf, s, False)
        elif vtype == 2:
            # escape sequences at offset 88 identify Joliet UCS-2 levels
            if buf[off + 88:off + 91] in (b"%/@", b"%/C", b"%/E"):
                joliet = Volume(buf, s, True)
        elif vtype == 255:
            break
        s += 1
    if primary is None:
        raise SystemExit("no primary volume descriptor")
    return primary, joliet


# ------------------------------------------------------------------- records

class Record(object):
    __slots__ = ("name", "lba", "length", "flags", "stamp", "volseq")

    def __init__(self, name, lba, length, flags, stamp, volseq=1):
        self.name = name          # raw identifier bytes
        self.lba = lba
        self.length = length
        self.flags = flags
        self.stamp = stamp        # 7 byte recording timestamp, copied verbatim
        self.volseq = volseq

    @property
    def is_dir(self):
        return bool(self.flags & DIR_FLAG)

    def encode(self):
        n = len(self.name)
        size = 33 + n + (1 if n % 2 == 0 else 0)
        rec = bytearray(size)
        rec[0] = size
        struct.pack_into("<I", rec, 2, self.lba)
        struct.pack_into(">I", rec, 6, self.lba)
        struct.pack_into("<I", rec, 10, self.length)
        struct.pack_into(">I", rec, 14, self.length)
        rec[18:25] = self.stamp
        rec[25] = self.flags
        struct.pack_into("<H", rec, 28, self.volseq)
        struct.pack_into(">H", rec, 30, self.volseq)
        rec[32] = n
        rec[33:33 + n] = self.name
        return bytes(rec)


def parse_records(buf, lba, length):
    out = []
    base = lba * SECTOR
    off = 0
    while off < length:
        rlen = buf[base + off]
        if rlen == 0:
            nxt = (off // SECTOR + 1) * SECTOR
            if nxt >= length:
                break
            off = nxt
            continue
        r = base + off
        nlen = buf[r + 32]
        out.append(Record(bytes(buf[r + 33:r + 33 + nlen]),
                          struct.unpack_from("<I", buf, r + 2)[0],
                          struct.unpack_from("<I", buf, r + 10)[0],
                          buf[r + 25],
                          bytes(buf[r + 18:r + 25]),
                          struct.unpack_from("<H", buf, r + 28)[0]))
        off += rlen
    return out


def sort_key(vol):
    pad = vol.pad_char()
    unit = len(pad)

    def key(rec):
        # '.' and '..' are identifiers 0x00 and 0x01 and always lead
        if rec.name in (b"\x00", b"\x01"):
            return (0, rec.name)
        return (1, rec.name)

    def cmp_name(a, b):
        # pad the shorter identifier so comparison is positional
        n = max(len(a), len(b))
        a = a + pad * ((n - len(a)) // unit)
        b = b + pad * ((n - len(b)) // unit)
        return (a > b) - (a < b)

    return key, cmp_name


def layout_exact(records, vol):
    """Serialise records, never letting one straddle a sector boundary.

    Returns the sector padded blob and the exact byte length up to the end of
    the last record, which are not the same number and both matter. mkisofs
    records a directory's length rounded up to a sector; Microsoft's mastering
    tool records the exact count. Extending a directory in place needs the exact
    figure, and writing it needs the padded one.
    """
    import functools
    key, cmp_name = sort_key(vol)
    ordered = sorted(records, key=lambda r: key(r)[0])
    lead = [r for r in ordered if key(r)[0] == 0]
    rest = [r for r in ordered if key(r)[0] != 0]
    rest.sort(key=functools.cmp_to_key(lambda a, b: cmp_name(a.name, b.name)))

    out = bytearray()
    for rec in lead + rest:
        blob = rec.encode()
        room = SECTOR - (len(out) % SECTOR)
        if len(blob) > room:
            out.extend(b"\0" * room)
        out.extend(blob)
    exact = len(out)
    if len(out) % SECTOR:
        out.extend(b"\0" * (SECTOR - len(out) % SECTOR))
    return bytes(out), exact


def layout(records, vol):
    return layout_exact(records, vol)[0]


# --------------------------------------------------------------------- image

class DirNode(object):
    __slots__ = ("path", "lba", "length", "records", "parent", "dirty")

    def __init__(self, path, lba, length, records, parent):
        self.path = path
        self.lba = lba
        self.length = length
        self.records = records
        self.parent = parent
        self.dirty = False


class Image(object):
    def __init__(self, path):
        with open(path, "rb") as f:
            self.buf = bytearray(f.read())
        self.path = path
        self.primary, self.joliet = find_volumes(self.buf)
        for v in self.volumes():
            v.buf = self.buf
        self.next_lba = max(self.primary.space_size,
                            (len(self.buf) + SECTOR - 1) // SECTOR)
        self.stamp = bytes(self.buf[self.primary.root_offset + 18:
                                    self.primary.root_offset + 25])
        self.trees = {}
        for v in self.volumes():
            lba = struct.unpack_from("<I", self.buf, v.root_offset + 2)[0]
            length = struct.unpack_from("<I", self.buf, v.root_offset + 10)[0]
            self.trees[id(v)] = {
                "/": DirNode("/", lba, length,
                             parse_records(self.buf, lba, length), None)}

    def volumes(self):
        return [v for v in (self.primary, self.joliet) if v is not None]

    def allocate(self, nbytes):
        lba = self.next_lba
        sectors = max(1, (nbytes + SECTOR - 1) // SECTOR)
        need = (lba + sectors) * SECTOR
        if len(self.buf) < need:
            self.buf.extend(b"\0" * (need - len(self.buf)))
        self.next_lba += sectors
        return lba

    def write_at(self, lba, data):
        end = lba * SECTOR + len(data)
        if len(self.buf) < end:
            self.buf.extend(b"\0" * (end - len(self.buf)))
        self.buf[lba * SECTOR:end] = data

    # ------------------------------------------------------------ traversal

    def getdir(self, vol, path):
        path = "/" + path.strip("/")
        tree = self.trees[id(vol)]
        if path in tree:
            return tree[path]
        parts = path.strip("/").split("/")
        parent = self.getdir(vol, "/" + "/".join(parts[:-1]))
        want = vol.encode(parts[-1]).upper()
        for rec in parent.records:
            if rec.is_dir and rec.name.upper() == want:
                node = DirNode(path, rec.lba, rec.length,
                               parse_records(self.buf, rec.lba, rec.length), parent)
                tree[path] = node
                return node
        raise KeyError(path)

    # -------------------------------------------------------------- mutation

    def add_record(self, vol, dirpath, rec):
        node = self.getdir(vol, dirpath)
        want = rec.name.upper()
        node.records = [r for r in node.records if r.name.upper() != want]
        node.records.append(rec)
        node.dirty = True

    def add_file(self, local, isopath):
        with open(local, "rb") as f:
            data = f.read()
        lba = self.allocate(len(data))
        self.write_at(lba, data + b"\0" * ((-len(data)) % SECTOR))
        parent, name = isopath.rsplit("/", 1)
        for vol in self.volumes():
            self.add_record(vol, parent or "/",
                            Record(vol.encode(name), lba, len(data), 0, self.stamp))
        return len(data)

    def add_dir(self, isopath):
        parent, name = isopath.rsplit("/", 1)
        parent = parent or "/"
        for vol in self.volumes():
            pnode = self.getdir(vol, parent)
            lba = self.allocate(SECTOR)
            dot = Record(b"\x00", lba, SECTOR, DIR_FLAG, self.stamp)
            dotdot = Record(b"\x01", pnode.lba, pnode.length, DIR_FLAG, self.stamp)
            self.write_at(lba, layout([dot, dotdot], vol))
            self.add_record(vol, parent,
                            Record(vol.encode(name), lba, SECTOR, DIR_FLAG, self.stamp))
            self.trees[id(vol)]["/" + isopath.strip("/")] = DirNode(
                "/" + isopath.strip("/"), lba, SECTOR, [dot, dotdot], pnode)

    def exists(self, path):
        try:
            for vol in self.volumes():
                self.getdir(vol, path)
            return True
        except KeyError:
            return False

    # ---------------------------------------------------------------- flush

    def set_extent(self, vol, node):
        """Publish node.lba and node.length to everything that records them."""
        if node.parent is None:
            struct.pack_into("<I", self.buf, vol.root_offset + 2, node.lba)
            struct.pack_into(">I", self.buf, vol.root_offset + 6, node.lba)
            struct.pack_into("<I", self.buf, vol.root_offset + 10, node.length)
            struct.pack_into(">I", self.buf, vol.root_offset + 14, node.length)
        else:
            want = vol.encode(node.path.strip("/").split("/")[-1]).upper()
            for r in node.parent.records:
                if r.is_dir and r.name.upper() == want:
                    r.lba, r.length = node.lba, node.length
            node.parent.dirty = True

    def flush_dirs(self):
        """Write modified directory extents, relocating only if one outgrows.

        Writing a node can dirty its parent, and parents are always visited
        first, so one pass is not enough. Repeat until nothing is left dirty.
        """
        for vol in self.volumes():
            for _ in range(64):
                if not self._flush_pass(vol):
                    break
            else:
                raise SystemExit("directory flush did not settle")

    def _flush_pass(self, vol):
        """One pass over the tree. True if anything still needed writing."""
        did = False
        for node in list(self.trees[id(vol)].values()):
            if not node.dirty:
                continue
            did = True
            data, exact = layout_exact(node.records, vol)

            # The recorded length may be smaller than the extent actually
            # allocated, so compare against the sector the extent ends on
            # rather than the figure in the record. Growing into that slack
            # keeps the lba, which is what makes it safe: the '..' entries in
            # any subdirectory still point at the right place.
            capacity = ((node.length + SECTOR - 1) // SECTOR) * SECTOR
            if len(data) <= capacity:
                for r in node.records:
                    if r.name == b"\x00":
                        r.length = exact
                data, _ = layout_exact(node.records, vol)
                self.write_at(node.lba, data + b"\0" * (capacity - len(data)))
                if exact != node.length:
                    node.length = exact
                    self.set_extent(vol, node)
                node.dirty = False
                continue

            if any(r.is_dir and r.name not in (b"\x00", b"\x01")
                   for r in node.records):
                raise SystemExit(
                    "%s must grow but has subdirectories whose '..' would need "
                    "rewriting; not handled" % node.path)

            new_lba = self.allocate(len(data))
            node.lba, node.length = new_lba, len(data)
            for r in node.records:
                if r.name == b"\x00":
                    r.lba, r.length = new_lba, len(data)
            self.write_at(new_lba, layout(node.records, vol))
            self.set_extent(vol, node)
            node.dirty = False
        return did

    # ----------------------------------------------------------- path tables

    def walk_dirs(self, vol):
        """Every directory breadth first, which is the order the tables want."""
        import functools
        _, cmp_name = sort_key(vol)
        out = [("/", self.getdir(vol, "/"), 1)]
        i = 0
        while i < len(out):
            path, node, _ = out[i]
            kids = [r for r in node.records
                    if r.is_dir and r.name not in (b"\x00", b"\x01")]
            kids.sort(key=functools.cmp_to_key(lambda a, b: cmp_name(a.name, b.name)))
            for rec in kids:
                child = path.rstrip("/") + "/" + vol.decode(rec.name)
                out.append((child, self.getdir(vol, child), i + 1))
            i += 1
        return out

    def rebuild_path_tables(self):
        """Adding a directory renumbers the tables, so write fresh ones."""
        for vol in self.volumes():
            entries = self.walk_dirs(vol)
            for endian in ("<", ">"):
                blob = bytearray()
                for idx, (path, node, parent_no) in enumerate(entries):
                    name = b"\x00" if idx == 0 else \
                        vol.encode(path.strip("/").split("/")[-1])
                    blob.append(len(name))
                    blob.append(0)
                    blob.extend(struct.pack(endian + "I", node.lba))
                    blob.extend(struct.pack(endian + "H", parent_no))
                    blob.extend(name)
                    if len(name) % 2:
                        blob.append(0)
                lba = self.allocate(len(blob))
                self.write_at(lba, bytes(blob) + b"\0" * ((-len(blob)) % SECTOR))
                if endian == "<":
                    struct.pack_into("<I", self.buf, vol.vd + 132, len(blob))
                    struct.pack_into(">I", self.buf, vol.vd + 136, len(blob))
                    struct.pack_into("<I", self.buf, vol.vd + 140, lba)
                    struct.pack_into("<I", self.buf, vol.vd + 144, 0)
                else:
                    struct.pack_into(">I", self.buf, vol.vd + 148, lba)
                    struct.pack_into(">I", self.buf, vol.vd + 152, 0)

    def save(self, out):
        for vol in self.volumes():
            vol.set_both(80, self.next_lba)
        with open(out, "wb") as f:
            f.write(self.buf)


# ------------------------------------------------------------------------ cli

def main():
    if len(sys.argv) < 3:
        raise SystemExit(__doc__)

    img = Image(sys.argv[1])
    start = img.next_lba
    files = 0

    def ensure_dir(path):
        path = "/" + path.strip("/")
        if path == "/" or img.exists(path):
            return
        ensure_dir("/" + "/".join(path.strip("/").split("/")[:-1]))
        img.add_dir(path)
        print("  mkdir %s" % path)

    for arg in sys.argv[2:]:
        src, dst = arg.rsplit("=", 1)
        dst = "/" + dst.strip("/")
        if os.path.isdir(src):
            for root, dirs, names in os.walk(src):
                dirs.sort()
                rel = os.path.relpath(root, src).replace("\\", "/")
                target = dst if rel == "." else dst + "/" + rel
                ensure_dir(target)
                for n in sorted(names):
                    img.add_file(os.path.join(root, n), target + "/" + n)
                    files += 1
            print("  added tree %s -> %s" % (src, dst))
        else:
            ensure_dir(dst.rsplit("/", 1)[0])
            size = img.add_file(src, dst)
            print("  add   %s (%d bytes)" % (dst, size))
            files += 1

    img.flush_dirs()
    img.rebuild_path_tables()
    img.save(sys.argv[1])
    print("  %d files added; image %d -> %d sectors (%.1f MB)"
          % (files, start, img.next_lba, len(img.buf) / 1048576.0))


if __name__ == "__main__":
    main()
