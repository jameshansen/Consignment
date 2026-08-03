"""Set the screen resolution the installed image boots into, after the fact.

`WINNT.SIF` has a `[Display]` section asking for 1024x768, and XP's standard VGA
driver ignores it and comes up at 640x480 anyway. The resolution that actually
gets used lives in registry DWORDs:

    ...\\Services\\VgaSave\\Device0\\DefaultSettings.XResolution
    ...\\Services\\VgaSave\\Device0\\DefaultSettings.YResolution

`INSTALL.CMD` writes them during the install, and they still do not take, because
the values setup wrote are re-applied over them on the way out. So they are set
here instead, in the finished image, where nothing comes along afterwards to
overwrite them.

A DWORD small enough to fit is stored *inline* in its registry cell rather than
in a separate data block, so the four bytes can be rewritten where they lie, with
no change of length and no need to understand the rest of the hive. The SYSTEM
hive is inside the NTFS image and each cell sits within one cluster, so scanning
the raw disk for the cell and patching in place works without mounting anything.

The image carries several `Device0` blocks (one per control set, plus the
hardware profile's copy), and every inline copy is set, so no block disagrees
with another.

The guest must be shut down when this runs, and the change takes effect on its
next boot, so this belongs after `install` and before `capture`.

    python set_resolution.py XPDrives/consign-v86.img            # report
    python set_resolution.py XPDrives/consign-v86.img 1024 768   # set
"""

import os
import struct
import sys

KEYS = ("DefaultSettings.XResolution", "DefaultSettings.YResolution")
CHUNK = 1 << 24
# Longest key above is 27 bytes, and a cell's name starts 20 bytes in, so no
# record of interest spans more than this. Overlapping reads by it means one
# straddling a chunk boundary is still seen whole.
OVERLAP = 64


def find_inline_dwords(path, name):
    """Offsets of every inline-DWORD value cell with this name, and its value.

    A registry value cell is "vk", a 2-byte name length, a 4-byte data size whose
    top bit means "the data is these four bytes, not a pointer", then the data
    itself, and the name at offset 20.
    """
    raw = name.encode("ascii")
    hits = []
    with open(path, "rb") as f:
        carry = b""
        base = 0
        while True:
            buf = f.read(CHUNK)
            if not buf:
                break
            window = carry + buf
            pos = 0
            while True:
                pos = window.find(b"vk", pos)
                if pos < 0 or pos + 20 + len(raw) > len(window):
                    break
                name_len = struct.unpack_from("<H", window, pos + 2)[0]
                if name_len == len(raw) and window[pos + 20:pos + 20 + name_len] == raw:
                    data_size = struct.unpack_from("<I", window, pos + 4)[0]
                    if data_size & 0x80000000:
                        value = struct.unpack_from("<I", window, pos + 8)[0]
                        hits.append((base + pos + 8, value))
                pos += 2
            carry = window[-OVERLAP:]
            base += len(window) - len(carry)
    return hits


def main():
    if len(sys.argv) not in (2, 4):
        raise SystemExit(__doc__)
    path = sys.argv[1]
    if not os.path.exists(path):
        raise SystemExit("no such image: %s" % path)
    setting = len(sys.argv) == 4
    values = (int(sys.argv[2]), int(sys.argv[3])) if setting else (None, None)

    if setting and (values[0] < 640 or values[1] < 480):
        raise SystemExit("refusing to set a resolution below 640x480")

    patched = 0
    for name, want in zip(KEYS, values):
        hits = find_inline_dwords(path, name)
        if not hits:
            raise SystemExit(
                "no inline %s found; the image is not what this expects "
                "and has been left alone" % name)
        for offset, old in hits:
            if want is None:
                print("  %-28s @ %#x: %d" % (name, offset, old))
                continue
            if old != want:
                with open(path, "r+b") as f:
                    f.seek(offset)
                    f.write(struct.pack("<I", want))
                patched += 1
            print("  %-28s @ %#x: %d -> %d" % (name, offset, old, want))

    if setting:
        print("set %d value(s) across %d copies; takes effect on the next boot"
              % (patched, len(hits)))


if __name__ == "__main__":
    main()
