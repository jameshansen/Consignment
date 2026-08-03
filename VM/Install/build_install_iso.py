"""Build the unattended XP SP3 install ISO: one disc, boot media and payload.

SETUPLDR.BIN reads I386\\WINNT.SIF off the boot CD, so the answer file has to go
inside the image. Rebuilding a Microsoft install ISO with an ordinary ISO tool is
a good way to end up with media that will not boot, so this appends to it with
iso_add.py instead: nothing already on the disc moves, and the El Torito boot
image and boot catalog keep the sectors they have.

\\SETUP goes on the same disc rather than a second one, because v86 exposes a
single cdrom. The payload rides along with the boot media and [GuiRunOnce] finds
it on the drive the machine already booted from.

Run stage_setupcd.py first: it assembles XPDrives/setupcd/SETUP, which is what
gets appended here.

The product key is substituted in here rather than stored in XPSetupFiles, so the
answer file in the repository never carries one.

Usage:
    python build_install_iso.py XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
    XPKEY=... python build_install_iso.py
"""

import os
import re
import shutil
import sys

HERE = os.path.dirname(os.path.abspath(__file__))

sys.path.insert(0, HERE)
import iso_add

KEY_RE = re.compile(r"^[A-Z0-9]{5}(-[A-Z0-9]{5}){4}$")


def find_media():
    """Pick the XP source ISO, preferring the volume licence one.

    A key only validates against media from its own channel. A volume key on the
    retail MSDN image gets "The CD Key you entered is not valid" from setup, so
    when both images are present the VL one wins: its keys never need
    activation, which matters for an image that is rebuilt rather than
    maintained.
    """
    import glob
    deps = os.path.join(HERE, "Deps")
    found = sorted(glob.glob(os.path.join(
        deps, "en_windows_xp_professional_with_service_pack_3_x86*.iso")))
    if not found:
        raise SystemExit("no XP SP3 source ISO in %s" % deps)
    for path in found:
        if "_vl_" in os.path.basename(path).lower():
            return path
    return found[0]


def main():
    key = (sys.argv[1] if len(sys.argv) > 1 else os.environ.get("XPKEY", "")).strip().upper()
    if not KEY_RE.match(key):
        raise SystemExit("need a product key as XXXXX-XXXXX-XXXXX-XXXXX-XXXXX")

    # An install reads this ISO from start to finish. Replacing it underneath one
    # does not fail loudly; the guest simply stops being able to read its media,
    # and the run is a write-off hours later.
    lock = os.path.join(HERE, "..", ".install-running")
    if os.path.exists(lock):
        raise SystemExit(
            "an install is running (%s); it is reading xpsetup.iso.\n"
            "Wait for it, or stop it and delete that file." % os.path.abspath(lock))

    src = find_media()

    out_dir = os.path.join(HERE, "XPDrives")
    if not os.path.isdir(out_dir):
        os.makedirs(out_dir)
    out = os.path.join(out_dir, "xpsetup.iso")
    sif = os.path.join(out_dir, "WINNT.SIF")
    payload = os.path.join(out_dir, "setupcd", "SETUP")

    if not os.path.isdir(payload):
        raise SystemExit("no %s; run stage_setupcd.py first" % payload)

    template = open(os.path.join(HERE, "XPSetupFiles", "WINNT.SIF")).read()
    if "@PRODUCTKEY@" not in template:
        raise SystemExit("XPSetupFiles/WINNT.SIF has no @PRODUCTKEY@ placeholder")
    # CRLF: this is read by a real mode loader, not by Windows.
    with open(sif, "w", newline="\r\n") as f:
        f.write(template.replace("@PRODUCTKEY@", key))

    print("  copying %s" % os.path.basename(src))
    shutil.copyfile(src, out)

    img = iso_add.Image(out)
    img.add_file(sif, "/I386/WINNT.SIF")

    # Two trees go on. \SETUP is the payload GuiRunOnce runs after Windows is up.
    # \$OEM$ is read by setup itself: everything under $OEM$\$1 is copied to the
    # system drive before hardware detection, which is how the display driver gets
    # installed during setup rather than after it.
    files = 0
    for src_root, iso_root in ((payload, "/SETUP"),
                               (os.path.join(out_dir, "setupcd", "$OEM$"), "/$OEM$")):
        if not os.path.isdir(src_root):
            continue
        # Directories are created top down, because a record can only be added to
        # a directory that already exists on the disc.
        for root, dirs, names in os.walk(src_root):
            dirs.sort()
            rel = os.path.relpath(root, src_root).replace("\\", "/")
            target = iso_root if rel == "." else iso_root + "/" + rel
            if not img.exists(target):
                img.add_dir(target)
            for name in sorted(names):
                img.add_file(os.path.join(root, name), target + "/" + name)
                files += 1

    img.flush_dirs()
    img.rebuild_path_tables()
    img.save(out)
    os.remove(sif)

    print("  %s  (%.1f MB)" % (out, os.path.getsize(out) / 1048576.0))
    print("  answer file injected, key ending %s" % key[-5:])
    print("  \\SETUP payload: %d files" % files)


if __name__ == "__main__":
    main()
