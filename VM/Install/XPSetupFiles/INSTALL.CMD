@echo off
rem First boot only. WINNT.SIF's [GuiRunOnce] finds this on the setup CD and
rem runs it once, with a full desktop up, which is the only place an MSI or a
rem bootstrapper will reliably work.
rem
rem Everything is synchronous: XP ships no sleep, and with no network stack
rem there is no ping to abuse as one either. Installers run under "start /wait"
rem and MySQL is polled rather than guessed at.
rem
rem The transcript lands in C:\SETUPLOG.TXT. Install/ntfs_file.py reads it back out
rem of the finished image from the host, which is how a run gets checked without
rem booting anything.
rem
rem Every step also announces itself on COM1, which is how install.mjs follows a
rem run it cannot see. The serial port is the one channel out of this guest that
rem needs no driver, no disk flush and no network: v86 hands the byte to the host
rem in the same instruction. INSTALL-COMPLETE on that port is what tells the host
rem the image is finished and can be written out.

set SRC=%~d0\SETUP
set LOG=C:\SETUPLOG.TXT

echo Consignment demo image, first boot > %LOG%
date /t >> %LOG%

call :say [1/9] Copying program, MySQL and SQL to C:
md C:\APP 2>nul
md C:\SQL 2>nul
xcopy "%SRC%\APP\*.*" C:\APP\ /E /I /Y >> %LOG% 2>&1
xcopy "%SRC%\MYSQL\*.*" C:\MYSQL\ /E /I /Y >> %LOG% 2>&1
copy "%SRC%\SQL\*.sql" C:\SQL\ >> %LOG% 2>&1
copy "%SRC%\my.ini" C:\MYSQL\ >> %LOG% 2>&1
copy "%SRC%\settings.ini" C:\APP\ >> %LOG% 2>&1
copy "%SRC%\STARTDEMO.CMD" C:\ >> %LOG% 2>&1
copy "%SRC%\SHELL.VBS" C:\ >> %LOG% 2>&1
copy "%SRC%\SLEEP.VBS" C:\ >> %LOG% 2>&1
copy "%SRC%\WIN2K.REG" C:\ >> %LOG% 2>&1
rem Nothing may read this before the savestate is captured, or its contents end
rem up in the guest's file cache and the served rewrite is never seen. Copying
rem it is fine; reading it is not. See dateblock.py.
copy "%SRC%\DATE.TXT" C:\ >> %LOG% 2>&1
md C:\APP\export 2>nul

call :say [2/9] Installing .NET Framework 4.0, this takes a while
echo --- dotnetfx --- >> %LOG%
start /wait "" "%SRC%\dotNetFx40_Full_x86_x64.exe" /q /norestart
rem Captured before anything else runs. Every command overwrites ERRORLEVEL, and
rem the code is wanted twice: in the log and on the wire.
set RC=%ERRORLEVEL%
echo dotnetfx exit %RC% >> %LOG%
rem Brackets, because :say ends the line with a redirect and cmd reads a digit
rem immediately before ">" as a file handle. A bare code would send nothing.
if not "%RC%"=="0" call :say INSTALL-FAILED dotnetfx [%RC%]

call :say [3/9] Installing the Crystal Reports runtime
echo --- crystal --- >> %LOG%
start /wait "" msiexec /i "%SRC%\CRRuntime_32bit_13_0.msi" /qn /norestart
set RC=%ERRORLEVEL%
echo crystal exit %RC% >> %LOG%
if not "%RC%"=="0" call :say INSTALL-FAILED crystal [%RC%]

call :say [4/9] Installing the 3 of 9 barcode font
copy "%SRC%\3of9.ttf" %SystemRoot%\Fonts\ >> %LOG% 2>&1
reg add "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Fonts" /v "3 of 9 Barcode (TrueType)" /t REG_SZ /d "3of9.ttf" /f >> %LOG% 2>&1

rem Crystal Reports resolves page metrics through the default printer and throws
rem if there is not one. Nothing is ever printed, so a Generic / Text Only driver
rem pointed at FILE: is enough and pulls in no hardware.
call :say [5/9] Adding a default printer for Crystal Reports
rundll32 printui.dll,PrintUIEntry /if /b "Demo Printer" /f "%SystemRoot%\inf\ntprint.inf" /r "FILE:" /m "Generic / Text Only" /Z >> %LOG% 2>&1
rundll32 printui.dll,PrintUIEntry /y /n "Demo Printer" >> %LOG% 2>&1

call :say [6/9] Registering MySQL as a service
echo --- mysql --- >> %LOG%
C:\MYSQL\bin\mysqld.exe --install MySQL --defaults-file=C:\MYSQL\my.ini >> %LOG% 2>&1
net start MySQL >> %LOG% 2>&1

rem No sleep, so poll the server rather than guess at a delay.
for /L %%i in (1,1,120) do @C:\MYSQL\bin\mysql.exe --protocol=pipe -u root -e "SELECT 1" >nul 2>&1 && goto ready
echo MySQL DID NOT START >> %LOG%
call :say INSTALL-FAILED mysql did not start
goto skipdb

:ready
call :say [7/9] Loading the schema and the demo data
echo --- schema --- >> %LOG%
C:\MYSQL\bin\mysql.exe --protocol=pipe -u root < C:\SQL\consignment_db_structure.sql >> %LOG% 2>&1
C:\MYSQL\bin\mysql.exe --protocol=pipe -u root < C:\SQL\demo_database.sql >> %LOG% 2>&1
C:\MYSQL\bin\mysql.exe --protocol=pipe -u root -e "SHOW TABLES FROM consignment_db" >> %LOG% 2>&1

:skipdb
rem Demand start, not automatic. A savestate thaws with the guest clock frozen at
rem capture time, and STARTDEMO.CMD corrects it from C:\DATE.TXT on the way up.
rem MySQL has to come up after that correction, not before it, so it is started
rem by hand once the clock is right.
sc config MySQL start= demand >> %LOG% 2>&1

rem Precompiling matters more here than anywhere else. The guest runs at roughly
rem 50-70 MIPS, so JIT time the browser would otherwise pay on every cold start
rem is worth the native images it costs.
call :say [8/9] Precompiling managed code
set NGEN=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ngen.exe
rem No ngen.exe means no framework, whatever the installer claimed. The program
rem cannot start on an image in that state, so say so rather than precompiling
rem nothing and reporting success.
if not exist "%NGEN%" call :say INSTALL-FAILED ngen missing
"%NGEN%" install "C:\APP\Multi Express Consignment.exe" >> %LOG% 2>&1
"%NGEN%" executequeueditems >> %LOG% 2>&1

call :say [9/9] Configuring autologon and startup
rem Blank-password autologon has to be set here; WINNT.SIF's AutoLogonCount only
rem covers this one boot.
rem
rem AutoLogonCount must be deleted, not left sitting at zero. Winlogon decrements
rem it on each automatic logon and, once it reaches zero, ignores AutoAdminLogon
rem and clears it again. Setting AutoAdminLogon while that counter is still
rem present is why the first build came up at the logon dialog. With no counter
rem the setting has no expiry.
set WL=HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon
reg delete "%WL%" /v AutoLogonCount /f >> %LOG% 2>&1
reg add "%WL%" /v AutoAdminLogon /t REG_SZ /d 1 /f >> %LOG% 2>&1
reg add "%WL%" /v DefaultUserName /t REG_SZ /d Administrator /f >> %LOG% 2>&1
reg add "%WL%" /v DefaultDomainName /t REG_SZ /d CONSIGNMENT /f >> %LOG% 2>&1
rem An empty REG_SZ, which is what a blank password needs. Omitting /d creates
rem the value empty; passing /d "" is parsed inconsistently.
reg add "%WL%" /v DefaultPassword /t REG_SZ /f >> %LOG% 2>&1
reg add "%WL%" /v LegalNoticeCaption /t REG_SZ /d "" /f >> %LOG% 2>&1
reg add "%WL%" /v LegalNoticeText /t REG_SZ /d "" /f >> %LOG% 2>&1
rem Replace the shell rather than adding a Run entry. With no explorer.exe there
rem is no taskbar, no Start menu, no desktop and no notification area, so the
rem tray balloons cannot appear at all, and closing the program ends the session
rem because SHELL.VBS waits on it. See SHELL.VBS and STARTDEMO.CMD.
reg add "%WL%" /v Shell /t REG_SZ /d "wscript.exe //B C:\SHELL.VBS" /f >> %LOG% 2>&1

rem Nothing here is ever serviced, recovered or networked. Disabling Themes is
rem also what selects the Classic look, so it is doing double duty. The spooler
rem deliberately stays on: Crystal Reports resolves page metrics through the
rem default printer and throws without one.
rem
rem LanmanServer and LanmanWorkstation are deliberately left alone. Nothing here
rem uses SMB, but MySQL is reached over a named pipe and that is not worth
rem risking to save two services.
for %%s in (wuauserv srservice SharedAccess wscsvc ERSvc helpsvc CiSvc WZCSVC uploadmgr) do @sc config %%s start= disabled >> %LOG% 2>&1
for %%s in (Themes TapiSrv RasMan RemoteAccess Messenger Alerter SSDPSRV upnphost) do @sc config %%s start= disabled >> %LOG% 2>&1
for %%s in (Browser Netman Nla TrkWks W32Time WmiApSrv AudioSrv seclogon Schedule) do @sc config %%s start= disabled >> %LOG% 2>&1
for %%s in (dmserver ShellHWDetection FastUserSwitchingCompatibility) do @sc config %%s start= disabled >> %LOG% 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore" /v DisableSR /t REG_DWORD /d 1 /f >> %LOG% 2>&1
reg add "HKCU\Control Panel\Desktop" /v ScreenSaveActive /t REG_SZ /d 0 /f >> %LOG% 2>&1

rem The demo is a kiosk, and every one of these puts a balloon or a window over
rem the program on the first boot: the tour, the low resolution notice, and the
rem "you have unused icons" cleanup.
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Applets\Tour" /v RunCount /t REG_DWORD /d 0 /f >> %LOG% 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v EnableBalloonTips /t REG_DWORD /d 0 /f >> %LOG% 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\CleanupWiz" /v NoRun /t REG_DWORD /d 1 /f >> %LOG% 2>&1
reg add "HKLM\SYSTEM\CurrentControlSet\Control\CrashControl" /v CrashDumpEnabled /t REG_DWORD /d 0 /f >> %LOG% 2>&1

rem Do not restart on a bugcheck. A headless install run has nobody watching the
rem screen, and a machine that reboots itself erases the one piece of evidence
rem worth having. Off, the stop code stays up until something reads it.
reg add "HKLM\SYSTEM\CurrentControlSet\Control\CrashControl" /v AutoReboot /t REG_DWORD /d 0 /f >> %LOG% 2>&1

rem Boot straight to the shell instead of sitting on the "welcome" list.
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" /v LogonType /t REG_DWORD /d 0 /f >> %LOG% 2>&1

rem Not needed for this build, and kept anyway. XP only boot-starts the storage
rem driver it found during setup, so an image that moves to a machine with a
rem different IDE controller stops at 0x7B INACCESSIBLE_BOOT_DEVICE. This image is
rem installed and played back under the same emulator, so that cannot happen here;
rem the old mergeide trick costs nothing and keeps the image portable if anyone
rem ever wants to boot it under something else.
for %%d in (atapi intelide pciide viaide aliide cmdide toside) do @reg add "HKLM\SYSTEM\CurrentControlSet\Services\%%d" /v Start /t REG_DWORD /d 0 /f >> %LOG% 2>&1

rem A pagefile in an image whose writes are thrown away on reload is mostly
rem waste: every page written costs browser memory in the block cache. Small and
rem fixed, rather than absent, because the guest has well under 512 MB.
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v PagingFiles /t REG_MULTI_SZ /d "C:\pagefile.sys 64 64" /f >> %LOG% 2>&1

rem Classic look. Disabling the Themes service above is what actually does it;
rem these two stop the theme being reapplied per user.
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ThemeManager" /v ThemeActive /t REG_SZ /d 0 /f >> %LOG% 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ThemeManager" /v DllName /t REG_SZ /f >> %LOG% 2>&1

rem XP's logo screensaver kicks in after ten idle minutes and covers the demo.
rem Nothing here should ever blank or lock.
reg add "HKCU\Control Panel\Desktop" /v ScreenSaveActive /t REG_SZ /d 0 /f >> %LOG% 2>&1
reg add "HKCU\Control Panel\Desktop" /v ScreenSaveTimeOut /t REG_SZ /d 0 /f >> %LOG% 2>&1
reg add "HKCU\Control Panel\Desktop" /v SCRNSAVE.EXE /t REG_SZ /f >> %LOG% 2>&1

rem The window metrics and the colour scheme used to be written here and did not
rem survive: USER32 writes the live values back over those keys at logoff, and
rem this script ends in a shutdown. They now live in WIN2K.REG and are imported
rem and applied by STARTDEMO.CMD on every boot instead. See WIN2K.REG.

rem The screen resolution is not set here. WINNT.SIF's [Display] section does it,
rem applied by the VBE display driver that setup installed from $OEM$, so it is
rem already right by the time this script runs. Writing DefaultSettings under
rem VgaSave, which is what this used to do, targets a driver that is no longer the
rem one in use. Install/set_resolution.py can still change a finished image.

echo Marking Windows activated
rem The retail media wants product activation, but the emulator has no network
rem and XP's activation servers are long retired, so it can never complete and
rem loops at logon ("activate now?" -> No -> logoff -> autologon -> repeat). This
rem OOBETimer value is the standard preservation mark that tells WPA the install
rem is already activated, which stops the loop.
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\WPAEvents" /v OOBETimer /t REG_BINARY /d FFD571D68B6A8D6FD53393FD /f >> %LOG% 2>&1

echo Listing IDE controllers in the critical device database
rem The other half of the same insurance. The PnP manager needs to bind a disk
rem controller before the boot volume is available, and it will only do that for
rem devices listed here. Both the legacy channels and the PIIX PCI controllers are
rem listed, so the image boots whichever way its host presents the disk.
set CDDB=HKLM\SYSTEM\CurrentControlSet\Control\CriticalDeviceDatabase
set IDECLASS={4D36E96A-E325-11CE-BFC1-08002BE10318}

for %%d in (primary_ide_channel secondary_ide_channel *pnp0600 *azt0502 gendisk) do @reg add "%CDDB%\%%d" /v Service /t REG_SZ /d atapi /f >> %LOG% 2>&1
for %%d in (primary_ide_channel secondary_ide_channel *pnp0600 *azt0502 gendisk) do @reg add "%CDDB%\%%d" /v ClassGUID /t REG_SZ /d "%IDECLASS%" /f >> %LOG% 2>&1

for %%d in (pci#ven_8086^&dev_1230 pci#ven_8086^&dev_7010 pci#ven_8086^&dev_7111) do @reg add "%CDDB%\%%d" /v Service /t REG_SZ /d intelide /f >> %LOG% 2>&1
for %%d in (pci#ven_8086^&dev_1230 pci#ven_8086^&dev_7010 pci#ven_8086^&dev_7111) do @reg add "%CDDB%\%%d" /v ClassGUID /t REG_SZ /d "%IDECLASS%" /f >> %LOG% 2>&1

echo Stripping
rem Bulk that nothing in the demo reads. Modest next to the whole image, but it
rem is also memory the file cache will never waste and blocks never fetched.
rem
rem Driver Cache\i386\driver.cab deliberately stays. It is around 70 MB, and it is
rem the only driver source the guest has if Windows ever re-detects hardware. The
rem browser fetches disk blocks on demand, so an unread 70 MB costs nothing.
for %%d in ("%SystemRoot%\Help" "%SystemRoot%\Web\Wallpaper" "%SystemRoot%\Media" "%SystemRoot%\srchasst" "%SystemRoot%\ime" "%SystemRoot%\system32\oobe" "%SystemRoot%\Offline Web Pages") do @if exist %%d rd /s /q %%d >> %LOG% 2>&1
for %%d in ("%ALLUSERSPROFILE%\Documents\My Music" "%ALLUSERSPROFILE%\Documents\My Pictures" "%ALLUSERSPROFILE%\Documents\My Videos") do @if exist %%d rd /s /q %%d >> %LOG% 2>&1
if exist "%SystemRoot%\system32\dllcache" rd /s /q "%SystemRoot%\system32\dllcache" >> %LOG% 2>&1

echo done >> %LOG%
date /t >> %LOG%

rem install.mjs waits for this line on COM1, then for the machine to power off,
rem and only writes the image out if it saw both.
call :say INSTALL-COMPLETE

shutdown -s -t 5 -f
goto :eof

rem Screen and serial port. 2>nul so a machine without COM1 still installs; the
rem host just gets a silent run.
rem
rem The space before the redirect is load bearing. cmd reads a digit immediately
rem before ">" as a file handle, so a message ending in one would quietly
rem redirect stderr to the port and send nothing. None of the messages above end
rem in a digit today, which is exactly the kind of thing that stops being true.
rem install.mjs trims the trailing space back off.
:say
echo %*
echo %* >COM1 2>nul
goto :eof
