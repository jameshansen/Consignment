<#
Runs the unattended XP SP3 install in VirtualBox and leaves a disk image ready
for imgsplit.py.

VirtualBox is a build tool here, nothing more. The install has to come out
looking like the machine Halfix emulates rather than the machine it was built
on, which is what the -acpi off and PIIX3/PIIX4 settings below are for. The HAL
is the part that cannot be fixed afterwards: XP picks it during text mode setup
and an ACPI HAL will not boot against an emulator running with acpi=0.
WINNT.SIF pins it to "Standard PC" as well, so the two agree.

The disc is created as VHD rather than VDI for one reason: Windows can mount a
VHD directly, so if the install goes wrong the filesystem can be inspected from
the host with Mount-DiskImage. That needs an elevated shell; the verification
here uses screenshots instead so the ordinary case needs no privileges.

    powershell -File build_vm.ps1 -ProductKey XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
#>

param(
    [Parameter(Mandatory = $true)][string]$ProductKey,
    [string]$VMName = "ConsignmentXP",
    [int]$DiskMB = 3072,
    [int]$TimeoutMinutes = 90,
    [switch]$SkipMedia
)

$ErrorActionPreference = "Stop"
$VBM = "C:\Program Files\Oracle\VirtualBox\VBoxManage.exe"
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$repo = Resolve-Path (Join-Path $here "..\..")
$build = Join-Path $repo "xpbuild"
$vhd = Join-Path $build "$VMName.vhd"

# Never redirect a native command's stderr in Windows PowerShell 5.1. Doing so
# wraps each line in an ErrorRecord, and with ErrorActionPreference = Stop that
# turns VBoxManage writing a harmless "no such machine" into a fatal error.
# Every VBoxManage call here is judged by its exit code instead.
function VBox {
    & $VBM @args
    if ($LASTEXITCODE -ne 0) { throw "VBoxManage $($args -join ' ') failed with $LASTEXITCODE" }
}

function VBoxSoft {
    $old = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try { & $VBM @args | Out-Null } catch { }
    $global:LASTEXITCODE = 0
    $ErrorActionPreference = $old
}

function Test-VMExists {
    $old = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try { $vms = & $VBM list vms } catch { $vms = @() }
    $ErrorActionPreference = $old
    $global:LASTEXITCODE = 0
    return (($vms -join "`n") -match ('"' + [regex]::Escape($VMName) + '"'))
}

function Get-VMState {
    if (-not (Test-VMExists)) { return "missing" }
    $old = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try { $info = & $VBM showvminfo $VMName --machinereadable } catch { $info = $null }
    $ErrorActionPreference = $old
    $global:LASTEXITCODE = 0
    $line = $info | Select-String '^VMState='
    if (-not $line) { return "unknown" }
    return $line.ToString().Split('"')[1]
}

# --------------------------------------------------------------- build media

$xpiso = Join-Path $build "xpsetup.iso"
$setupiso = Join-Path $build "setup.iso"

if ($SkipMedia -and (Test-Path $xpiso) -and (Test-Path $setupiso)) {
    Write-Host "==> Reusing the existing xpsetup.iso and setup.iso"
}
else {
    Write-Host "==> Building the setup CD"
    & python (Join-Path $here "stage_setupcd.py")
    if ($LASTEXITCODE -ne 0) { throw "stage_setupcd.py failed" }

    Write-Host "==> Building the unattended install ISO"
    & python (Join-Path $here "build_install_iso.py") $ProductKey
    if ($LASTEXITCODE -ne 0) { throw "build_install_iso.py failed" }
}

# ------------------------------------------------------------------- the VM

if ((Get-VMState) -ne "missing") {
    Write-Host "==> Removing the previous $VMName"
    VBoxSoft controlvm $VMName poweroff
    Start-Sleep -Seconds 3
    VBoxSoft unregistervm $VMName --delete
}
if (Test-Path $vhd) { Remove-Item $vhd -Force }

Write-Host "==> Creating $VMName"
VBox createvm --name $VMName --ostype WindowsXP --basefolder $build --register

# acpi/ioapic off so text mode setup installs the Standard PC HAL, matching
# VirtualXP.htm's acpi=false/apic=false. No NIC, because Halfix emulates none
# and MySQL is reached over a named pipe instead.
VBox modifyvm $VMName --memory 512 --vram 16 --acpi off --ioapic off --pae off `
    --chipset piix3 --graphicscontroller vboxvga --nic1 none --usb off `
    --boot1 dvd --boot2 disk --boot3 none --boot4 none --audio-driver none

VBox createmedium disk --filename $vhd --size $DiskMB --format VHD
# PIIX3, not PIIX4. Halfix emulates an 82371SB, which enumerates as PCI device
# 8086:7010; VirtualBox's PIIX4 default is an 82371AB, 8086:7111. Setup installs
# the driver for whatever it sees, and this image is booted somewhere else.
VBox storagectl $VMName --name IDE --add ide --controller PIIX3 --hostiocache on
VBox storageattach $VMName --storagectl IDE --port 0 --device 0 --type hdd --medium $vhd
VBox storageattach $VMName --storagectl IDE --port 1 --device 0 --type dvddrive --medium $xpiso
VBox storageattach $VMName --storagectl IDE --port 1 --device 1 --type dvddrive --medium $setupiso

# ------------------------------------------------------------------ install

Write-Host "==> Starting the install (headless)"
VBox startvm $VMName --type headless

# The XP boot sector asks to "press any key to boot from CD" and falls through
# to the hard disk after about five seconds. Headless there is nobody to press
# it, so send Enter for the first half minute. Text mode setup is unattended
# from there on and prompts for nothing, so nothing else can be hit by these.
Write-Host "    answering the boot prompt"
for ($i = 0; $i -lt 30; $i++) {
    VBoxSoft controlvm $VMName keyboardputscancode 1c 9c
    Start-Sleep -Seconds 1
}

# ACPI is off, so the guest cannot power itself off: INSTALL.CMD's "shutdown -s"
# leaves it parked on "It is now safe to turn off your computer" and the VM stays
# in the running state for ever. Waiting for poweroff would always time out.
#
# First logon also throws up two modal dialogs that OemSkipWelcome does not
# cover, "Display Settings" and "Monitor Settings", and the first of them opens
# without focus, so keystrokes sent straight at it go nowhere. VirtualBox has no
# mouse injection, only keyboard, which makes Alt+Tab the only way to reach it.
#
# Both situations look the same from out here: nothing drawn and no CPU burned.
# So watch for that, try Alt+Tab then Enter a few times to clear a dialog, and
# if nudging changes nothing, take it as halted.
Write-Host "==> Installing. Watching for the halt at the end."
$watch = Join-Path $build "watch.png"
$deadline = (Get-Date).AddMinutes($TimeoutMinutes)
$prevHash = ""; $prevCpu = 0.0; $idle = 0; $nudges = 0; $halted = $false

function Get-ScreenHash {
    VBoxSoft controlvm $VMName screenshotpng $watch
    if (Test-Path $watch) { return (Get-FileHash $watch -Algorithm MD5).Hash }
    return ""
}
function Get-GuestCpu {
    $p = Get-Process VBoxHeadless -ErrorAction SilentlyContinue |
         Sort-Object CPU -Descending | Select-Object -First 1
    if ($p) { return [double]$p.CPU } else { return 0.0 }
}

while ((Get-Date) -lt $deadline) {
    if ((Get-VMState) -in @("poweroff", "aborted")) { $halted = $true; break }
    Start-Sleep -Seconds 30
    $h = Get-ScreenHash
    $cpu = Get-GuestCpu
    $busy = ($h -ne $prevHash) -or (($cpu - $prevCpu) -gt 3.0)
    $prevHash = $h; $prevCpu = $cpu
    if ($busy) { $idle = 0 } else { $idle++ }

    if ($idle -ge 6) {
        # Three minutes with a still screen and an idle CPU.
        if ($nudges -lt 4) {
            Write-Host "    screen idle, trying Alt+Tab then Enter"
            VBoxSoft controlvm $VMName keyboardputscancode 38 0f 8f b8
            Start-Sleep -Milliseconds 600
            VBoxSoft controlvm $VMName keyboardputscancode 1c 9c
            $nudges++; $idle = 0
        }
        else {
            Write-Host "    guest has halted"
            $halted = $true
            break
        }
    }
    else {
        $left = [int]($deadline - (Get-Date)).TotalMinutes
        Write-Host ("    working  ({0} min left, idle {1})" -f $left, $idle)
    }
}
if (-not $halted) {
    VBoxSoft controlvm $VMName screenshotpng (Join-Path $build "timeout.png")
    throw "install did not finish inside $TimeoutMinutes minutes; see xpbuild\timeout.png"
}

# Safe even mid-halt: the guest flushed its disks before parking on that screen.
if ((Get-VMState) -eq "running") { VBoxSoft controlvm $VMName poweroff; Start-Sleep -Seconds 5 }

Write-Host "==> Install finished, ejecting the discs"
VBox storageattach $VMName --storagectl IDE --port 1 --device 0 --type dvddrive --medium none
VBox storageattach $VMName --storagectl IDE --port 1 --device 1 --type dvddrive --medium none
VBox modifyvm $VMName --boot1 disk --boot2 none

# -------------------------------------------------------------- verification

Write-Host "==> Booting once more to check the demo comes up"
VBox startvm $VMName --type headless
for ($i = 1; $i -le 10; $i++) {
    Start-Sleep -Seconds 60
    $shot = Join-Path $build ("boot{0:d2}.png" -f $i)
    VBoxSoft controlvm $VMName screenshotpng $shot
    Write-Host "    $shot"
}
Write-Host "    leaving the VM running; look at the screenshots, then:"
Write-Host "      VBoxManage controlvm $VMName acpipowerbutton   (or poweroff)"
Write-Host ""
Write-Host "Once it is shut down cleanly, convert and chunk it:"
Write-Host "      VBoxManage clonemedium disk `"$vhd`" `"$build\xp.img`" --format RAW"
Write-Host "      python imgsplit.py `"$build\xp.img`" `"$build\xp`""
