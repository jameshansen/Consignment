"""Build XPDrives/patch.iso: the CD you boot in Safe Mode to drop updated files
onto the installed C: without a full reinstall.

It stages the canonical files into a temp dir and hands them to makeiso.py, so
STARTDEMO.CMD has one source of truth (XPSetupFiles/STARTDEMO.CMD) instead of a
second copy drifting inside the ISO.

Workflow (see README, "Patching the installed image"):
    python build_patch_iso.py
    # then boot patch.conf, F8 -> Safe Mode with Command Prompt, run D:UPDATE.BAT,
    # reboot, and recapture with after-install.conf.

Add more files by dropping them in Patch/ and copying them in UPDATE.BAT.

Usage:
    python build_patch_iso.py [out.iso]
"""

import os
import shutil
import sys
import tempfile

import makeiso

HERE = os.path.dirname(os.path.abspath(__file__))
# Files that come from their canonical location, not a copy kept in Patch/.
CANONICAL = [os.path.join(HERE, "XPSetupFiles", "STARTDEMO.CMD")]
# Everything hand-authored for the patch (UPDATE.BAT) lives here.
PATCH_DIR = os.path.join(HERE, "Patch")


def build(out):
    stage = tempfile.mkdtemp(prefix="patchiso_")
    try:
        for src in CANONICAL:
            shutil.copy2(src, stage)
        for name in os.listdir(PATCH_DIR):
            shutil.copy2(os.path.join(PATCH_DIR, name), stage)
        makeiso.build(stage, out)
    finally:
        shutil.rmtree(stage, ignore_errors=True)


def main():
    out = sys.argv[1] if len(sys.argv) > 1 else os.path.join(
        HERE, "XPDrives", "patch.iso")
    build(out)


if __name__ == "__main__":
    main()
