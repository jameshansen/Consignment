"""Lay out a Global Assembly Cache for the framework assemblies.

From .NET 2.0 onward mscorlib lives in the GAC rather than the framework
directory, and the loader deliberately reports the framework directory as its
location for compatibility. Framework assemblies sitting only in an application
directory therefore fail as soon as anything executes in them:

    FileLoadException: The given assembly name or codebase,
    '...\\v4.0.30319\\mscorlib.dll', was invalid

The GAC is nothing but nested directories, one assembly each:

    GAC_MSIL\\<name>\\v4.0_<version>__<token>\\<name>.dll

and nothing writes to it at load time, so a prebuilt read-only copy on the boot
CD is enough. iso_add.py puts the result at \\I386\\Microsoft.NET\\assembly,
which is where the CLR looks, since %windir% is \\I386 in this image.

Strong name tokens are read from the assemblies rather than hardcoded; a wrong
token gives a directory the loader silently ignores.

Usage:
    python stage_gac.py <dir with runtime assemblies> <mscorlib.dll> <out>/GAC_MSIL
"""

import os
import shutil
import subprocess
import sys

# the two strong name keys Microsoft signs the framework with
FRAMEWORK_TOKENS = ("b77a5c561934e089", "b03f5f7f11d50a3a")


def identity(paths):
    """Ask .NET for each assembly's name, version and public key token."""
    script = (
        "$ErrorActionPreference='SilentlyContinue';"
        "foreach ($p in $input) {"
        "  try {"
        "    $n=[System.Reflection.AssemblyName]::GetAssemblyName($p);"
        "    $t=($n.GetPublicKeyToken()|ForEach-Object{$_.ToString('x2')}) -join '';"
        "    if ($t) { '{0}|{1}|{2}|{3}' -f $p,$n.Name,$n.Version,$t }"
        "  } catch {}"
        "}"
    )
    proc = subprocess.run(["powershell", "-NoProfile", "-Command", script],
                          input="\n".join(paths), capture_output=True, text=True)
    out = {}
    for line in proc.stdout.splitlines():
        parts = line.strip().split("|")
        if len(parts) == 4:
            out[parts[0]] = (parts[1], parts[2], parts[3])
    return out


def stage(srcdir, mscorlib, out):
    if os.path.isdir(out):
        shutil.rmtree(out)
    os.makedirs(out)

    candidates = [os.path.join(srcdir, f) for f in sorted(os.listdir(srcdir))
                  if f.lower().endswith(".dll")]
    if mscorlib:
        candidates.insert(0, mscorlib)

    found = identity(candidates)
    if not found:
        raise SystemExit("could not read any assembly identities; is PowerShell available?")

    total = 0
    kept = 0
    for path in candidates:
        info = found.get(path)
        if not info:
            continue                      # unsigned, or not a managed assembly
        name, version, token = info
        # Only the framework itself belongs here. The program's own assemblies,
        # Crystal Reports and Connector/Net included, stay beside the executable.
        if version != "4.0.0.0" or token not in FRAMEWORK_TOKENS:
            continue
        kept += 1
        folder = os.path.join(out, name, "v4.0_%s__%s" % (version, token))
        os.makedirs(folder, exist_ok=True)
        shutil.copy2(path, os.path.join(folder, name + ".dll"))
        total += os.path.getsize(path)
        print("  %-34s %s  %s" % (name, version, token))

    print("  %d framework assemblies, %.1f MB" % (kept, total / 1048576.0))


def main():
    if len(sys.argv) != 4:
        raise SystemExit(__doc__)
    stage(sys.argv[1], sys.argv[2], sys.argv[3])


if __name__ == "__main__":
    main()
