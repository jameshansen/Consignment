"""Set the screen resolution the installed image boots into.

WINNT.SIF asks for 1024x768 but XP's standard VGA driver comes up at 640x480
anyway, so the resolution is fixed after the fact, in the image, the same way
set_resolution.py fixes it in the LiveXP ISO: the values live in registry DWORDs

    ...\\Services\\{VgaSave,vga,...}\\Device0\\DefaultSettings.XResolution
    ...\\Services\\...\\Device0\\DefaultSettings.YResolution

and a small DWORD is stored inline in its registry cell, so it can be rewritten
in place without changing any length. The SYSTEM hive lives inside the NTFS
image; each cell sits in one cluster, so scanning the raw disk for the cell and
patching the four bytes works without mounting anything.

The image carries several Device0 blocks (different control sets and the
hardware profile), so every inline copy is set, keeping each block internally
consistent. The guest must be shut down first; the change takes effect on the
next boot.

    python set_hdd_resolution.py ../Install/XPDrives/consign.img          # report
    python set_hdd_resolution.py ../Install/XPDrives/consign.img 1024 768 # set

Take a copy first; this rewrites the file given to it.
"""

import struct
import sys

KEYS = ("DefaultSettings.XResolution", "DefaultSettings.YResolution")


def patch(path, width=None, height=None):
    with open(path, "rb") as f:
        buf = bytearray(f.read())

    total = 0
    for name, value in zip(KEYS, (width, height)):
        raw = name.encode("ascii")
        found = pos = 0
        while True:
            pos = buf.find(b"vk", pos)
            if pos < 0:
                break
            name_len = struct.unpack_from("<H", buf, pos + 2)[0]
            if name_len == len(raw) and bytes(buf[pos + 20:pos + 20 + name_len]) == raw:
                data_size = struct.unpack_from("<I", buf, pos + 4)[0]
                if data_size & 0x80000000:      # high bit set: stored inline
                    old = struct.unpack_from("<I", buf, pos + 8)[0]
                    if value is None:
                        print("  %s @ %#x: %d" % (name, pos, old))
                    else:
                        struct.pack_into("<I", buf, pos + 8, value)
                        print("  %s @ %#x: %d -> %d" % (name, pos, old, value))
                    found += 1
            pos += 2
        if found == 0:
            raise SystemExit("error: no inline %s found, image left unchanged" % name)
        total += found

    if width is not None:
        with open(path, "wb") as f:
            f.write(buf)
        print("patched %d values" % total)


def main():
    if len(sys.argv) == 2:
        patch(sys.argv[1])
        return
    if len(sys.argv) != 4:
        raise SystemExit(__doc__)
    path, width, height = sys.argv[1], int(sys.argv[2]), int(sys.argv[3])
    print("patching %s" % path)
    patch(path, width, height)
    print("done - reboot the guest for it to take effect")


if __name__ == "__main__":
    main()
