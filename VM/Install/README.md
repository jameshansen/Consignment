# Install

Everything that turns a blank 4 GB file into a bootable Windows XP image with the
program installed. Driven by [`../build.py`](../build.py); the scripts here also
run on their own.

## Layout

| | |
|:--- |:--- |
| `Deps/` | every dependency in one place: the XP media, MySQL, the SQL, .NET 4, Crystal Reports, the barcode font |
| `XPSetupFiles/` | the answer file and the scripts that run inside the guest |
| `XPDrives/` | build output: the ISO, the disk image, and the staging tree |
| `stage_setupcd.py` | collect the program and its dependencies into `XPDrives/setupcd/SETUP` |
| `build_install_iso.py` | copy the XP media, append the answer file and that payload |
| `test_install_iso.py` | read the finished ISO back the way the guest will |
| `iso_add.py` | add files and directories to an ISO without moving anything on it |
| `makeiso.py` | write a plain data ISO from a directory tree |
| `ntfs_file.py` | read and write a file inside the raw image, from the host |
| `set_resolution.py` | rewrite the screen resolution registry values in a finished image |
| `build_patch_iso.py`, `Patch/` | a small CD for fixing an image without rebuilding it |

`Deps/MYSQL` must be exactly there, not nested under `SQL`.

## The answer file

`XPSetupFiles/WINNT.SIF` makes setup fully unattended. `build_install_iso.py`
substitutes the product key into it, so no key is ever stored in the repository.

Two settings are load-bearing.

**`ComputerType = "Standard PC", Retail`** forces the non-ACPI HAL. v86 faults
during XP startup with the ACPI one, and the HAL is the single thing that cannot be
corrected after the install. `Retail` is not optional spelling: an unrecognised
second token makes setup ignore the whole line and autodetect an ACPI HAL. The
description also has to match a `[Computer]` entry in the media's `TXTSETUP.SIF`,
where it appears as

```
e_isa_up    = "Standard PC",files.none
```

**`SFCQuota = 0`** stops Windows File Protection filling `i386\dllcache` with about
350 MB of duplicated system files. Nothing in this image is ever repaired.

`[GuiRunOnce]` probes the drives for `\SETUP\INSTALL.CMD` and hands the rest of the
install over to it.

## The guest scripts

| | |
|:--- |:--- |
| `INSTALL.CMD` | first boot only. Installs everything, configures Windows, shuts down |
| `STARTDEMO.CMD` | the demo session. Runs as the Windows shell on every boot |
| `SHELL.VBS` | replaces `explorer.exe`, runs `STARTDEMO.CMD` hidden, shuts down after |
| `WIN2K.REG` | window metrics and colour scheme, reapplied on every boot |
| `my.ini`, `settings.ini` | MySQL server and client config, both on the named pipe |

Both scripts report on COM1. `INSTALL.CMD` uses a `:say` helper that writes to the
screen and to the port. The space in `echo %* >COM1` is load-bearing, because `cmd`
reads a digit immediately before `>` as a file handle, so a message ending in one
would quietly redirect stderr and send nothing.

`INSTALL.CMD` ends with `INSTALL-COMPLETE`, which is what tells the host the image
is finished. `STARTDEMO.CMD` writes `DEMO-READY` once MySQL answers a query, which
is where the snapshot gets taken.

Everything in `INSTALL.CMD` is synchronous. XP ships no `sleep`, and with no network
stack there is no `ping` to abuse as one, so installers run under `start /wait` and
MySQL is polled rather than guessed at. The transcript lands in `C:\SETUPLOG.TXT`,
which is worth reading back out of a finished image:

```bash
python ntfs_file.py read XPDrives/consign-v86.img SETUPLOG.TXT log.txt
```

`dotnetfx exit 0`, `crystal exit 0` and `Tables_in_consignment_db` in there mean the
run went the way it should.

## The ISO

Rebuilding a Microsoft install disc with an ordinary ISO tool is a good way to end
up with media that stops at "Please insert the disk labeled Boot Disk into Drive
A:". Rather than work out which of the thousand things a rebuild changes is the one
that matters, `iso_add.py` never relocates existing data. New content is appended
past the end of the image, the volume size is grown to match, and the El Torito boot
image and boot catalogue keep the sectors they already have.

`build_install_iso.py` uses it to add two things to a copy of the XP media: the
answer file at `/I386/WINNT.SIF`, and the whole payload tree at `/SETUP`. One disc,
because v86 exposes one CD drive.

`test_install_iso.py` reads the result back before anything commits hours to it. It
checks that the boot record and catalogue still describe a bootable image, that the
answer file has the `Standard PC` line and a substituted key, that the payload
scripts are byte identical to `XPSetupFiles/`, and that `mysqld.exe` is still a PE
image three directories down.

This XP media carries **no Joliet tree**, so names in the primary tree are uppercased
and keep their spaces and second extension, as in
`MULTI EXPRESS CONSIGNMENT.EXE.CONFIG`. That is outside ISO9660 and Windows reads it
anyway; Windows being case insensitive is what makes the uppercasing harmless.

## Editing a built image from the host

`ntfs_file.py` resolves a file through its MFT record rather than searching the image
for its text, which matters more than it sounds.

NTFS records a **ValidDataLength** per `$DATA` attribute and returns zeros for
anything past it, whatever the clusters hold. This image once arrived with
`C:\STARTDEMO.CMD` at real size 5118 and ValidDataLength 0, the signature of a copy
whose metadata committed and whose data did not. The guest opened the shell script,
read 5118 zero bytes, ran nothing, and shut down. Patching those clusters by hand
changed nothing, because nothing past ValidDataLength is ever read. Check `validlen`
first whenever the guest behaves as though a file were empty.

```bash
python ntfs_file.py info  XPDrives/consign-v86.img STARTDEMO.CMD
python ntfs_file.py write XPDrives/consign-v86.img STARTDEMO.CMD new.cmd
```

Only what this build needs is implemented: one non-resident, uncompressed `$DATA`
attribute per file, which is what every script on `C:` is. Files small enough to live
inside the MFT record are rejected rather than half-handled.

`set_resolution.py` rewrites `DefaultSettings.XResolution` and `YResolution` in a
finished image. A DWORD small enough to fit is stored inline in its registry cell, so
the four bytes can be changed where they lie without understanding the rest of the
hive. The image carries several copies, one per control set plus the hardware
profile's, and all of them get set so none disagrees with another. The guest must be
shut down, and the change takes effect on its next boot.

This is a repair tool rather than a build step. The resolution is set during setup
instead; see [screen resolution](../README.md#screen-resolution).
