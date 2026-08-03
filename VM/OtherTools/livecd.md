# Attempt 1: LiveXP live-CD demo (abandoned)

The first attempt ran the program off a **VirtualXP / LiveXP live CD** booted in the
browser by [Halfix](https://github.com/nepx/halfix) compiled to WebAssembly. The
demo files dropped into a [VirtualXP](https://github.com/lrusso/VirtualXP) checkout
and booted a read-only Windows XP PE. It got a long way and then hit a wall it could
not cross: **a live CD has no writable `%windir%`, so there is no GAC, and the .NET
class libraries will not execute without one.**

Everything here is kept for reference. The scripts that belonged only to this path
live alongside this file in `OtherTools/`.

## How it was built

`VirtualXP.iso` was bootable and best left alone, so everything the demo added rode
on a second data CD attached as `cdb`:

```
payload/
  APP/       the Release build, minus *.pdb and *.cs
  MYSQL/     a MySQL 5.1 noinstall tree: bin, share, data
  SQL/       consignment_db_structure.sql and demo_database.sql
  setup.bat  my.ini  settings.ini  autorun.inf
```

MySQL 5.1 is the last version that runs on XP. Deleting `ib_logfile*`, `ibdata1` and
the unused `share/` language packs took the payload from 50 MB to 29 MB, since the
schema is entirely MyISAM.

Build steps:

```bash
python set_resolution.py VirtualXP.iso 1024 768   # patches SETUPREG.HIV in place
python build_payload_iso.py payload/ payload.iso
```

`set_resolution.py` rewrites two registry DWORDs inline inside the ISO, so nothing is
rebuilt or remounted and no admin rights are needed. Run with just a path it reports
the current values.

## What worked

- XP booted at 1024x768 with both discs attached, reaching the desktop in ~13 minutes
  at roughly 50-70 MIPS.
- `setup.bat` ran automatically at logon off the payload CD (`CONSIGNMENT (D:)`) and
  drove the sequence unattended.
- **MySQL 5.1.73 ran in the guest**, reported `ready for connections` over a named
  pipe, and the schema and demo data loaded.
- **The .NET Framework 4.0 CLR ran**: `hello.exe` reported `CLR version : 4.0.30319.1`
  on `Microsoft Windows NT 5.1.2600.0`.

## .NET 4.0 on an XP SP2 live CD

Getting the CLR to run took several non-obvious steps, all now in `netreg.bat`,
`extract_netfx.py` and `replace_system_file.py`:

- **Use the real 4.0 redistributable.** The `v4.0.30319` directory on any modern
  Windows box is actually .NET 4.8 (4.5 replaced 4.0 in place and dropped XP). Those
  binaries cannot run in the guest. `extract_netfx.py` pulls the genuine 4.0.30319.1
  x86 files out of `dotNetFx40_Full_x86_x64.exe`, where `netfx_Core.mzz` is an
  ordinary MS cabinet. The result is 34 MB against 190 MB installed, since none of it
  is NGEN native images.
- **SP3 is a support policy, not an API dependency.** Every function `mscoree.dll`,
  `mscoreei.dll`, `clr.dll` and `clrjit.dll` import is exported by the SP2 binaries in
  the image. The only missing DLL is `msvcr100_clr0400.dll`, which ships with the
  framework.
- **mscoree.dll has to be in the system directory.** XP's loader resolves it from
  `%SystemRoot%\system32` for a managed image; anywhere else gives 0xc0000135 at
  process start. `replace_system_file.py` renames a spare file in place, since the
  boot CD is read-only. `mscorees.dll` is worth adding the same way, or the shim
  reports only "a fatal error occurred".
- **Watch the trailing backslash in `reg add`.** `reg add ... /d "D:\DOTNET\Framework\"
  /f` stores `D:\DOTNET\Framework" /f`, because the parser reads `\"` as an escaped
  quote. Double it.
- **The policy range matters.** `policy\v4.0` needs `30319` set to `30319-30319`, the
  way a real install writes it. An empty value produces "you first must install .NET
  Framework v4.0.30319".

MySQL was the easy dependency: `mysqld.exe` 5.1 imports only ADVAPI32, KERNEL32,
USER32 and WS2_32, links the CRT statically, and runs with `skip-networking` over a
named pipe (`my.ini`), with `Protocol=pipe` passed to Connector/Net (`settings.ini`).

GDI+ is not in the image at all and WinForms needs it. A redistributable `gdiplus.dll`
built for XP is inside `CRRuntime_32bit_13_0.msi`, and it is loaded by ordinary
P/Invoke, so the application directory is enough for it.

## Where it stopped: no GAC

The CLR runs, but the class libraries do not. `System.Windows.Forms` and
`System.Drawing` load with the right identity, then the first attempt to *execute*
anything in them fails:

    FileLoadException: The given assembly name or codebase,
    'B:\fx\v4.0.30319\mscorlib.dll', was invalid.

Creating a `Bitmap`, calling `MessageBox.Show` and constructing a `Form` all fail the
same way, while `hello.exe`, which only touches mscorlib, runs fine. Ruled out, each
tested in the guest: the runtime being on the CD, `machine.config`, and `DEVPATH` with
`developmentMode developerInstallation="true"`.

What is left is that these assemblies load from the application directory instead of
the GAC, which the framework does not support. There is no GAC to install them into:
it lives in `%windir%\assembly`, and LiveXP's `%windir%` is `E:\I386` on the read-only
boot CD. This cannot be worked around by swapping a file; it needs a **writable**
Windows directory, which means rebuilding LiveXP in WinBuilder so the Windows
directory sits on the RAM disk.

Crystal Reports would not work regardless, since it needs an MSI install and COM
registration, so the reports suite was missing from any demo built this way.

## The autostart hook that broke the boot

LiveXP's only startup entry is:

    HKCU\Software\Microsoft\Windows\CurrentVersion\Run
        ScreenSaver = %PEsys32%\screensaver.exe

Registry values cannot be edited in place once a string changes length, so the plan
was to replace `screensaver.exe` with a small launcher. **It breaks the boot**: with
that file replaced the shell never finishes starting. Its strings include "Script
Execution", so LiveXP appears to run its post-boot script chain through it. The
launcher builds fine; what was needed is a different hook or a rebuilt ISO with an
edited hive.

## Why it was abandoned

The GAC problem is structural to a read-only PE. The better attack, if this path is
ever revived, is to give the PE a writable `%windir%`, either via LiveXP's RunFromRAM
and Boot SDI, or by moving to a WinPE 2.0+ image where `X:\Windows` is a writable RAM
disk by design and the tooling (`wimlib`, `DISM`) is maintained. Instead the project
moved to a real hard-disk install (see [vbox-hda.md](vbox-hda.md), also abandoned),
and then to installing directly under the emulator that runs it (see the current
[README](../README.md)).
