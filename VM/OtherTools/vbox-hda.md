# Attempt 2: real XP install in VirtualBox, ported to Halfix (abandoned)

A real XP SP3 install answers the GAC problem outright: it has a writable `%windir%`,
a real registry, a working Windows Installer and COM registration, so .NET 4, MySQL
and the Crystal Reports runtime all install the ordinary way. That part worked. The
install was done **headless in VirtualBox** and the resulting disk image handed to
Halfix.

**Booting the result under Halfix does not work**, and that is where this approach
stopped. The disk image was built on one emulator and run on another, and the two do
not present the same disk controller.

The VirtualBox driver script for this path is archived alongside this file as
`OtherTools/build_vm.ps1`.
The install pipeline it drove (`build_install_iso.py`, `iso_add.py`,
`stage_setupcd.py`, `unattend/`) was **kept and reused** for the current approach and
now lives in [`../Install/`](../Install/).

## The pipeline

    stage_setupcd.py     assemble the setup CD from the app, MySQL, SQL,
                         dotNetFx40_Full_x86_x64.exe, CRRuntime_32bit_13_0.msi,
                         the 3 of 9 font and unattend/
    build_install_iso.py copy the XP media, inject I386\WINNT.SIF with iso_add.py
    build_vm.ps1         create the VM, run the install headless, verify, halt
    clonemedium          VHD to raw
    find_date_block.py   locate C:\DATE.TXT in the raw image
    imgsplit.py          split into gzipped 256 KB blocks
    savestate_server.py  serve it, rewrite the date block, catch the savestate

One command drove the install:

```powershell
.\build_vm.ps1 -ProductKey "XXXXX-XXXXX-XXXXX-XXXXX-XXXXX"
```

## What was verified, under VirtualBox

- Setup runs unattended from `WINNT.SIF` with no prompt at any stage.
- .NET 4, the Crystal Reports runtime, MySQL 5.1, the schema and the demo data all
  install, and the program is NGEN compiled at the end.
- The program runs and reports `MySQL Connected`.
- There is no taskbar, Start menu or notification area, because `Winlogon\Shell` is
  `SHELL.VBS` rather than `explorer.exe`.
- Closing the program shuts the machine down.

None of this was ever seen under Halfix, because the guest does not get that far.

## Where it stops: atapi.sys faults under Halfix

Booting the image under Halfix reaches the kernel and then stops:

    *** STOP: 0x0000007E (0xC0000005, 0xFAAD3876, 0xFAF731A4, 0xFAF72EA0)
    *** atapi.sys - Address FAAD3876 base at FAACD000, DateStamp 4802539d

`0x7E` is `SYSTEM_THREAD_EXCEPTION_NOT_HANDLED`, `0xC0000005` is an access violation.
The storage driver binds to the controller and then faults driving it.

**The root cause is that the image is built on one emulator and run on another.**
Setup installs drivers for the hardware it sees, and VirtualBox and Halfix do not
present the same disk controller. This was not anticipated when the approach was
chosen, and it is the single reason it failed. **It is also the whole reason for the
current approach: install under the emulator that runs it, so there is nothing to
port.**

## What was tried against the fault, and did not move it

Three things were changed independently and the fault address was identical every
time:

| change | result |
| :--- | :--- |
| PIIX4 to PIIX3 controller | same stop, same address |
| DMA to PIO mode 4 | same stop, same address |
| `apic`/`acpi` off | worse: resets in NTLDR before the kernel |

- **DMA is not it.** Forcing PIO via `MasterDeviceTimingModeAllowed` /
  `SlaveDeviceTimingModeAllowed` = `0x10` produced byte-for-byte the same stop. If the
  fault were in a transfer, changing the transfer mode would have moved it. A real
  install has six subkeys under the class GUID (`0000`-`0005`), not the two channels
  you would expect, so loop wide.
- **`0x7B` was fixed by a workaround.** The first symptom was `INACCESSIBLE_BOOT_DEVICE`
  (`0xC0000034`): nothing claimed the disk. `INSTALL.CMD` now starts `atapi`,
  `intelide` and `pciide` at boot and populates the critical device database for both
  legacy and PIIX3 controllers. That got from `0x7B` to `0x7E`: progress, not a fix.
- **Empty CD drives make it worse.** Halfix's own notes warn the ATAPI emulation is
  buggy and to avoid a CD-ROM present. Adding empty CD drives to match the install-time
  layout hung the guest at `Booting from Hard Disk...`.
- **`build_vm.ps1` created the VM with PIIX4 while Halfix emulates PIIX3.** Corrected to
  PIIX3. VirtualBox's PIIX4 is an 82371AB (`8086:7111`); Halfix's is an 82371SB
  (`8086:7010`), confirmed in its BIOS log.
- **`apic` and `acpi` have to be on, and the reason is not understood.** With both off
  the guest resets inside NTLDR; with both on it reaches the kernel and bugchecks. This
  contradicts `build_vm.ps1` installing with `--acpi off --ioapic off` to get the
  Standard PC HAL. The accident was load-bearing.
- **Set `CrashControl\AutoReboot` to 0 before debugging.** XP restarts on bugcheck, and
  under Halfix a restart ends the run before bootvid draws the stop code, so a bugcheck
  and a triple fault look identical otherwise. `INSTALL.CMD` sets it.

What is left is `atapi.sys+0x6876` dereferencing something it should not, in
initialisation, independent of transfer configuration. The honest next step for anyone
reviving this path is to disassemble that offset, not guess at another setting.

## The appearance: getting the Windows 2000 look

LiveXP had the Windows 2000 scheme for free. A real XP install has to be told, and XP
Classic is not the 2000 look:

| | XP Classic | Windows 2000 |
| :--- | :--- | :--- |
| caption band | 25 px | 20 px |
| `ButtonFace` | 236 233 216 | 212 208 200 |
| active title gradient | 0 84 227 to 61 149 255 | 10 36 106 to 166 202 240 |

`WIN2K.REG` carries the scheme. It is applied by `STARTDEMO.CMD` on **every boot**, not
once during setup, and that ordering is the whole trick: **USER32 writes the live
colours and metrics back over `HKCU` at logoff**, so anything `INSTALL.CMD` sets is
destroyed by the shutdown that ends it.
`rundll32 user32.dll,UpdatePerUserSystemParameters` is what makes the imported values
live. The fonts are deliberately not carried: both versions use Tahoma 8 pt, so the
`LOGFONT` blobs are already right. A resource patcher such as ineXPerience is the wrong
tool: it swaps icons and bitmaps, not metrics or colours.

## Getting a shell in the guest

Replacing the shell leaves no Start menu and no Run dialog, but **Task Manager still
opens on Ctrl+Shift+Esc, and `File > New Task` is a full Run dialog.** That is enough to
change a built image without rebuilding it: attach a CD, run a script off it, reboot.
Driving it from `VBoxManage`: `keyboardputstring` does not emit `"`, so put anything
needing quotes in a batch file on the CD; and the window with focus is not always the
one on top, so `taskkill /f /im taskmgr.exe` from the console is the easy way out.

## Other findings from this path

- **The guest runs at 640x480.** XP's inbox VGA driver does not use the VBE modes the
  Bochs VGA BIOS advertises, so it cannot match the 1024x768 canvas without a VESA
  driver such as VBEMP. Never installed.
- **Volume licence keys do not work with retail media.** MSDN ships XP SP3 as separate
  retail and volume images and a key only validates against its own channel.
  `build_install_iso.py` prefers an ISO with `_vl_` in the name.
- **Injected scancodes cannot drive XP's product-key entry.** The five boxes rely on an
  auto-advance that does not fire for synthetic input, so the key has to come from the
  answer file.
- **`build_vm.ps1` cannot tell "never booted" from "finished".** With ACPI off the guest
  cannot power itself off, so the script watches for a still screen and an idle CPU,
  which a failed boot also produces. It should check the VHD has grown.

## Why it was abandoned

The hard-disk route solved the software problem and replaced it with a portability
problem between two emulators, which was a worse trade. The fix is to remove the port
entirely by installing under the emulator that runs it, which is the current
approach; see the
[README](../README.md). The install pipeline here carries over unchanged.
