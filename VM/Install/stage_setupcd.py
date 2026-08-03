"""Assemble the payload the unattended install runs its own setup from.

This writes a directory tree rather than an ISO. build_install_iso.py appends it
to the Windows media, so the machine has one disc to read.

WINNT.SIF's [GuiRunOnce] probes the drives for \\SETUP\\INSTALL.CMD on it and hands
the rest of the install over. Everything the guest needs is here because the guest
has no network: the .NET 4 bootstrapper, the Crystal Reports runtime, MySQL, the
program and the SQL. The display driver goes in the $OEM$ tree beside it, because
setup has to install that one itself.

The sources are scattered across the repository, so this collects them rather
than expecting a prepared directory.

The one non-obvious part is what is left out of APP. The Release build has the
.NET 4 framework assemblies sitting next to the exe, copied local by the project.
On the LiveXP image that was deliberate, since there was no GAC to load them
from. Here there is one, and a real install puts them in it. Shipping a second
copy beside the exe is what the old image was doing and what this whole image
exists to stop doing, so they are filtered out.

Usage:
    python stage_setupcd.py                 # writes Install/XPDrives/setupcd/
    python stage_setupcd.py OUTDIR
"""

import os
import re
import shutil
import sys

HERE = os.path.dirname(os.path.abspath(__file__))

sys.path.insert(0, HERE)
# dateblock lives in ../Viewer, because the server owns that layout. This build
# step reuses it so the placeholder it writes matches what the server rewrites.
sys.path.insert(0, os.path.join(HERE, "..", "Viewer"))
import dateblock

# The program comes from the build output; every external dependency is
# consolidated in Deps/ so the install has one place to look.
APP_SRC = os.path.join(HERE, "..", "..", "Multi Express Consignment",
                       "bin", "Release")
DEPS = os.path.join(HERE, "Deps")
MYSQL_SRC = os.path.join(DEPS, "MYSQL")
SQL_SRC = os.path.join(DEPS, "SQL")
DOTNET = os.path.join(DEPS, "dotNetFx40_Full_x86_x64.exe")
CRYSTAL = os.path.join(DEPS, "CRRuntime_32bit_13_0.msi")
FONT = os.path.join(DEPS, "3of9.ttf")

# The VBE Miniport display driver, which is the only way this guest gets above
# 640x480. XP's own VGA driver has no higher mode to offer, whatever the answer
# file and the registry ask for. These two files go into $OEM$ so setup installs
# the driver itself, which is what makes WINNT.SIF's [Display] section take
# effect on the first boot rather than needing a change afterwards.
VBEMP_ZIP = os.path.join(DEPS, "vbempk.zip")
VBEMP_FILES = ("VBE30/XP/PNP/vbemp.sys", "VBE30/XP/PNP/vbemppnp.inf")

# Anything a real .NET 4 install puts in the GAC, plus the two unmanaged files
# that only existed to prop the runtime up on the CD. MySql.Data.dll and the
# CrystalDecisions assemblies deliberately do not match any of these.
FRAMEWORK = re.compile(
    r"^(System(\..*)?\.dll|Microsoft\.[^.]+\.dll|mscor.*\.dll"
    r"|Accessibility\.dll|msvcr\d+.*\.dll|gdiplus\.dll)$", re.I)

DROP_SUFFIXES = (".pdb", ".cs")


def copy_app(dest):
    kept = dropped = 0
    os.makedirs(dest)
    for name in sorted(os.listdir(APP_SRC)):
        src = os.path.join(APP_SRC, name)
        if not os.path.isfile(src):
            continue
        if name.lower().endswith(DROP_SUFFIXES) or FRAMEWORK.match(name):
            dropped += 1
            continue
        shutil.copy2(src, os.path.join(dest, name))
        kept += 1
    return kept, dropped


def stage_oem(stage_root):
    """Lay out the $OEM$ tree setup copies to the system drive.

    $OEM$\\$1\\ becomes C:\\, so these land in C:\\drivers\\video, which is what
    WINNT.SIF's OemPnPDriversPath points at. Setup adds that to the driver search
    path before it detects hardware, so the display adapter is bound to this
    driver during setup rather than after it.
    """
    dest = os.path.join(stage_root, "$OEM$", "$1", "drivers", "video")
    if os.path.isdir(os.path.join(stage_root, "$OEM$")):
        shutil.rmtree(os.path.join(stage_root, "$OEM$"))
    os.makedirs(dest)

    import zipfile
    with zipfile.ZipFile(VBEMP_ZIP) as z:
        have = set(z.namelist())
        for member in VBEMP_FILES:
            if member not in have:
                raise SystemExit(
                    "%s has no %s; the VBEMP package layout has changed"
                    % (os.path.basename(VBEMP_ZIP), member))
            with z.open(member) as src:
                name = member.rsplit("/", 1)[-1]
                with open(os.path.join(dest, name), "wb") as out:
                    shutil.copyfileobj(src, out)
    print("  oem     %d display driver files -> $OEM$\\$1\\drivers\\video"
          % len(VBEMP_FILES))


def main():
    stage = sys.argv[1] if len(sys.argv) > 1 else \
        os.path.join(HERE, "XPDrives", "setupcd")

    for path in (DOTNET, CRYSTAL, FONT, APP_SRC, MYSQL_SRC, SQL_SRC, VBEMP_ZIP):
        if not os.path.exists(path):
            raise SystemExit("missing: %s" % path)

    if os.path.isdir(stage):
        shutil.rmtree(stage)
    root = os.path.join(stage, "SETUP")
    os.makedirs(root)

    stage_oem(stage)

    kept, dropped = copy_app(os.path.join(root, "APP"))
    print("  APP     %d files kept, %d framework/debug files dropped"
          % (kept, dropped))

    for name, src in (("MYSQL", MYSQL_SRC), ("SQL", SQL_SRC)):
        shutil.copytree(src, os.path.join(root, name))
        print("  %-8sDeps/%s" % (name, name))

    for src in (DOTNET, CRYSTAL, FONT):
        shutil.copy2(src, root)
        print("  extra   %s" % os.path.basename(src))

    for name in ("INSTALL.CMD", "STARTDEMO.CMD", "SHELL.VBS", "SLEEP.VBS",
                 "WIN2K.REG", "my.ini", "settings.ini"):
        shutil.copy2(os.path.join(HERE, "XPSetupFiles", name), root)
        print("  setup   %s" % name)

    # The placeholder the server later rewrites in place. Generated rather than
    # checked in because its size and layout have to match dateblock.py exactly.
    with open(os.path.join(root, "DATE.TXT"), "wb") as f:
        f.write(dateblock.build())
    print("  unatt   DATE.TXT (%d bytes)" % dateblock.SIZE)

    print("  staged  %s" % stage)


if __name__ == "__main__":
    main()
