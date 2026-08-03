"""Assemble the application directory for the payload CD.

Two things have to happen here rather than at runtime.

The Release build carries its own copy of System.dll, System.Windows.Forms.dll
and the rest, because every framework reference in the csproj is marked Private.
Those copies come from the reference assembly folder: they hold metadata only,
and loading one throws

    BadImageFormatException: Cannot load a reference assembly for execution

They are dropped here and replaced with the real runtime assemblies, which is
also the cheaper option: the reference set is 9 MB of dead weight on a 63 MB RAM
disk, and copying both and letting one win overflowed it.

Usage:
    python stage_app.py <bin/Release> <DOTNET/app> <payload>/APP
"""

import os
import shutil
import sys

DROP_SUFFIXES = (".pdb", ".cs")


def is_reference_only(path):
    with open(path, "rb") as f:
        return b"ReferenceAssemblyAttribute" in f.read()


def stage(release, runtime, out):
    if os.path.isdir(out):
        shutil.rmtree(out)
    os.makedirs(out)

    dropped = kept = 0
    for name in sorted(os.listdir(release)):
        src = os.path.join(release, name)
        if not os.path.isfile(src) or name.lower().endswith(DROP_SUFFIXES):
            continue
        if name.lower().endswith(".dll") and is_reference_only(src):
            dropped += 1
            continue
        shutil.copy2(src, os.path.join(out, name))
        kept += 1

    added = 0
    for name in sorted(os.listdir(runtime)):
        src = os.path.join(runtime, name)
        if os.path.isfile(src):
            shutil.copy2(src, os.path.join(out, name))
            added += 1

    total = sum(os.path.getsize(os.path.join(out, f)) for f in os.listdir(out))
    print("  kept %d from the build, dropped %d reference assemblies, "
          "added %d runtime files" % (kept, dropped, added))
    print("  %s: %.1f MB" % (out, total / 1048576.0))


def main():
    if len(sys.argv) != 4:
        raise SystemExit(__doc__)
    stage(sys.argv[1], sys.argv[2], sys.argv[3])


if __name__ == "__main__":
    main()
