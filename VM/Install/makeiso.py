"""Write a Joliet data ISO from a directory tree.

stage_setupcd.py uses this to build setup.iso, the CD the unattended install
runs its own setup from. It is a plain directory-to-ISO writer with no live-CD
assumptions left in it; the old build_payload_iso entry point lived here too and
is kept only as a CLI.

ISO9660 names are generated as F00001.DAT and friends. They are never seen:
Windows reads the Joliet tree, which carries the real names. Generating them
this way sidesteps 8.3 collisions across the ~200 files MySQL ships.

Usage:
    python makeiso.py SRC_DIR OUT.iso
"""

import os
import sys

try:
    import pycdlib
except ImportError:
    raise SystemExit("pycdlib is required:  pip install pycdlib")

SKIP_SUFFIXES = (".pdb", ".cs", ".pyc")


def build(src, out):
    iso = pycdlib.PyCdlib()
    iso.new(joliet=3, interchange_level=3, vol_ident="CONSIGNMENT")

    counter = [0]

    def iso_name(is_dir):
        counter[0] += 1
        if is_dir:
            return "/DIR%05d" % counter[0]
        return "/F%05d.DAT;1" % counter[0]

    total = 0
    for root, dirs, files in os.walk(src):
        dirs.sort()
        files.sort()
        rel = os.path.relpath(root, src).replace("\\", "/")
        joliet_dir = "/" if rel == "." else "/" + rel

        if rel != ".":
            iso.add_directory(iso_name(True), joliet_path=joliet_dir)

        for name in files:
            if name.lower().endswith(SKIP_SUFFIXES):
                continue
            local = os.path.join(root, name)
            joliet = (joliet_dir.rstrip("/") + "/" + name)
            iso.add_file(local, iso_name(False), joliet_path=joliet)
            total += os.path.getsize(local)

    iso.write(out)
    iso.close()
    print("%s: %d files, %.1f MB of content, image is %.1f MB"
          % (out, counter[0], total / 1048576.0, os.path.getsize(out) / 1048576.0))


def main():
    if len(sys.argv) != 3:
        raise SystemExit(__doc__)
    build(sys.argv[1], sys.argv[2])


if __name__ == "__main__":
    main()
