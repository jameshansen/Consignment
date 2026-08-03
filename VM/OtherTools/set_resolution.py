"""Set the screen resolution VirtualXP boots into.

The LiveXP image inside VirtualXP.iso ships at 800x600. The resolution lives in
two registry DWORDs in I386\\SYSTEM32\\SETUPREG.HIV:

    ...\\Services\\vmx_svga\\Device0\\DefaultSettings.XResolution
    ...\\Services\\vmx_svga\\Device0\\DefaultSettings.YResolution

Small DWORDs are stored inline in the registry cell rather than in a separate
data block, so changing them never changes any length. That means the ISO can be
patched in place -- ISO9660 stores file contents verbatim, so the four bytes sit
at a fixed offset and nothing downstream needs recomputing. No ISO rebuild, no
mounting, no admin rights.

Usage:
    python set_resolution.py VirtualXP.iso            # report current resolution
    python set_resolution.py VirtualXP.iso 1024 768   # set it

Take a copy of the ISO first; this rewrites the file given to it.
"""

import struct
import sys

KEYS = ("DefaultSettings.XResolution", "DefaultSettings.YResolution")


def patch(path, width=None, height=None):
    """Set the resolution, or report it when width and height are omitted."""
    with open(path, "rb") as f:
        buf = bytearray(f.read())

    patched = 0
    for name, value in zip(KEYS, (width, height)):
        raw = name.encode("ascii")
        pos = 0
        while True:
            pos = buf.find(b"vk", pos)
            if pos < 0:
                break
            # registry vk cell: 'vk', name length, data size, data/offset, type
            name_len = struct.unpack_from("<H", buf, pos + 2)[0]
            if name_len == len(raw) and bytes(buf[pos + 20:pos + 20 + name_len]) == raw:
                data_size = struct.unpack_from("<I", buf, pos + 4)[0]
                if data_size & 0x80000000:      # high bit set means stored inline
                    old = struct.unpack_from("<I", buf, pos + 8)[0]
                    if value is None:
                        print("  %s: %d" % (name, old))
                    else:
                        struct.pack_into("<I", buf, pos + 8, value)
                        print("  %s: %d -> %d" % (name, old, value))
                    patched += 1
                else:
                    print("  %s: not stored inline, skipped" % name)
            pos += 2

    if patched != len(KEYS):
        raise SystemExit("error: found %d of %d values, image left unchanged"
                         % (patched, len(KEYS)))

    if width is not None:
        with open(path, "wb") as f:
            f.write(buf)


def main():
    if len(sys.argv) == 2:
        patch(sys.argv[1])
        return
    if len(sys.argv) != 4:
        raise SystemExit(__doc__)
    path, width, height = sys.argv[1], int(sys.argv[2]), int(sys.argv[3])
    print("patching %s" % path)
    patch(path, width, height)
    print("done - clear the browser's service worker cache before reloading")


if __name__ == "__main__":
    main()
