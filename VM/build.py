"""Build the browser demo, start to finish, from a blank disk.

Every step is a command. Nothing here needs a person to watch a screen and press
a key, and nothing needs an AI to drive it: the guest reports what it is doing on
COM1, and the emulator that installs the image is the same one that later runs it
in the browser.

    python build.py                 stage, iso, verify, install, capture
    python build.py install         one step, and everything after it
    python build.py install only    one step, on its own
    python build.py serve           run the demo locally

The product key is read from XPKEY, or from Install/xpkey.txt. Volume licence
media is used when it is present in Install/Deps, so the result never has to
activate.

Steps
    stage     collect the program and its dependencies into Install/XPDrives/setupcd
    iso       append the answer file and that payload to the XP media
    verify    read the finished ISO back the way the guest will
    install   run XP setup under v86, unattended, to a fresh 4 GB image (hours)
    capture   boot the image, wait for the program, save the state
    serve     serve Viewer/ with range support and a live date
"""

import json
import os
import shutil
import subprocess
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
INSTALL = os.path.join(HERE, "Install")
V86 = os.path.join(HERE, "v86")
VIEWER = os.path.join(HERE, "Viewer")

IMAGE = os.path.join(INSTALL, "XPDrives", "consign-v86.img")


def key():
    k = os.environ.get("XPKEY", "").strip()
    if k:
        return k
    path = os.path.join(INSTALL, "xpkey.txt")
    if os.path.exists(path):
        return open(path).read().strip()
    raise SystemExit(
        "no product key: set XPKEY, or put one in Install/xpkey.txt")


def run(cwd, *cmd):
    print("\n=== %s" % " ".join(str(c) for c in cmd))
    r = subprocess.run([str(c) for c in cmd], cwd=cwd)
    if r.returncode:
        raise SystemExit("failed (%d): %s" % (r.returncode, " ".join(cmd)))


def stage():
    run(INSTALL, sys.executable, "stage_setupcd.py")


def iso():
    run(INSTALL, sys.executable, "build_install_iso.py", key())


def verify():
    run(INSTALL, sys.executable, "test_install_iso.py")


def install():
    # v86 writes the image only when the guest reports INSTALL-COMPLETE, so a
    # failed run leaves consign-v86.img.partial and this stops here.
    #
    # Three hours, not the eight install.mjs defaults to. A healthy run takes
    # under one, and the failure this guards against is a guest wedged with the
    # disk still ticking over, which no deadline can tell from a slow install.
    # Eight hours of that is a night; three is a coffee. Resume the .partial with
    # install.mjs --from rather than starting again.
    run(V86, "node", "install.mjs", os.path.relpath(IMAGE, V86), 3)


def stage_viewer():
    """Copy the emulator into Viewer/ from the package npm installed.

    The page and the capture have to be the same build. A savestate is tied to the
    emulator that wrote it, so a Viewer/ carrying last month's v86.wasm restores
    into something subtly different and bugchecks. Copying on every run means the
    two cannot drift, and it keeps Viewer/ a complete static directory that can be
    deployed on its own.
    """
    build = os.path.join(V86, "node_modules", "v86", "build")
    if not os.path.isdir(build):
        raise SystemExit("no v86 package; run 'npm install' in %s" % V86)
    os.makedirs(os.path.join(VIEWER, "bios"), exist_ok=True)
    for src, dest in (
            (os.path.join(build, "libv86.mjs"), os.path.join(VIEWER, "libv86.mjs")),
            (os.path.join(build, "v86.wasm"), os.path.join(VIEWER, "v86.wasm")),
            (os.path.join(V86, "bios", "seabios.bin"),
             os.path.join(VIEWER, "bios", "seabios.bin")),
            (os.path.join(V86, "bios", "vgabios.bin"),
             os.path.join(VIEWER, "bios", "vgabios.bin"))):
        if not os.path.exists(src):
            raise SystemExit("missing %s" % src)
        shutil.copyfile(src, dest)
    print("  staged the emulator into %s" % os.path.relpath(VIEWER, HERE))


def date_patch_sidecar(image):
    """Write down where C:\\DATE.TXT really is, resolved through the MFT.

    serve.py finds the date block by scanning the image for its magic string and
    taking the first hit. That is one guess too many: this image carries two
    copies of the block, and the first one on disk is not the file. Patching it
    leaves the real DATE.TXT untouched, so every visitor reads the capture date,
    and nothing anywhere reports a problem.

    NTFS already knows the answer. Resolve the file through its MFT record and
    hand serve.py the offset in the sidecar it caches anyway, so neither it nor
    capture.mjs ever scans.
    """
    sys.path[:0] = [INSTALL, VIEWER]
    import mmap
    import dateblock
    import ntfs_file

    with open(image, "rb") as f:
        mm = mmap.mmap(f.fileno(), 0, access=mmap.ACCESS_READ)
        spans = ntfs_file.extents(mm, "DATE.TXT")[5]
        at = spans[0][0]
        # The block moves whenever the image is rebuilt, so trust the MFT rather
        # than the last known offset, but do check the magic is actually there.
        # A wrong offset here is silent, and silent is the whole problem.
        if mm[at:at + len(dateblock.MAGIC)] != dateblock.MAGIC:
            raise SystemExit(
                "DATE.TXT resolves to byte %d, which does not hold the date "
                "block. The image or ntfs_file.py has changed." % at)

    st = os.stat(image)
    json.dump({"size": st.st_size, "mtime": int(st.st_mtime),
               "offset": at + dateblock.FIELD_OFFSET},
              open(image + ".datepatch", "w"))
    print("  date field at byte %d, from the MFT" % (at + dateblock.FIELD_OFFSET))


def capture():
    stage_viewer()
    # The disk the page serves is the one that was just installed. Copying
    # rather than pointing at it keeps Viewer/ self-contained and keeps the
    # build's output away from its input.
    dest = os.path.join(VIEWER, "xp.img")
    print("\n=== copying %s -> %s" % (os.path.basename(IMAGE), dest))
    # Unlink first. copyfile opens the destination for writing, and if something
    # left a hard link here (handy for testing against an image without copying
    # four gigabytes) that write goes straight through into the original.
    if os.path.exists(dest):
        os.remove(dest)
    shutil.copyfile(IMAGE, dest)
    # Before capture.mjs, which asks serve.py where the date field is and would
    # otherwise cache the scan's answer.
    date_patch_sidecar(dest)
    run(V86, "node", "capture.mjs", os.path.relpath(dest, V86))


def serve():
    stage_viewer()
    run(VIEWER, sys.executable, "serve.py")


STEPS = [("stage", stage), ("iso", iso), ("verify", verify),
         ("install", install), ("capture", capture)]


def main():
    names = [n for n, _ in STEPS]
    arg = sys.argv[1] if len(sys.argv) > 1 else None
    only = len(sys.argv) > 2 and sys.argv[2] == "only"

    if arg == "serve":
        serve()
        return
    if arg is None:
        todo = STEPS
    elif arg in names:
        i = names.index(arg)
        todo = STEPS[i:i + 1] if only else STEPS[i:]
    else:
        raise SystemExit(__doc__)

    for name, fn in todo:
        print("\n########## %s" % name)
        fn()
    print("\nall done. python build.py serve, then open"
          " http://localhost:8899/demo.html")


if __name__ == "__main__":
    main()
