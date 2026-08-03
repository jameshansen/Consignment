@echo off
rem The demo session, start to finish. SHELL.VBS runs this hidden in place of
rem explorer.exe, so this script *is* the Windows shell: when it returns, the
rem session is over and SHELL.VBS shuts the machine down.
rem
rem The savestate is taken from here. Once MySQL answers, this writes DEMO-READY
rem to COM1 and capture.mjs snapshots the machine on that byte, in the same
rem instruction the guest wrote it. No disk marker, no write-back cache, no clock
rem trick: the emulator is on the other end of the port.
rem
rem The snapshot lands deliberately *before* the DATE.TXT read below, so a state
rem frozen months ago still picks up the visitor's real date when it resumes.

rem A fixed post-1970 date rather than parsing "date /t" and adding a century. The
rem value does not matter; DATE.TXT below replaces it with the visitor's real one.
rem Parsing the locale's format was one bad token away from handing "date" an
rem invalid argument, which makes it PROMPT, and on this hidden console that
rem blocks the shell for ever, silently. "<nul" is the backstop.
date 01-01-2020 <nul

rem Applied here rather than during setup because USER32 writes the live values
rem back over these keys at logoff, destroying anything INSTALL.CMD wrote.
regedit /s C:\WIN2K.REG
rundll32.exe user32.dll,UpdatePerUserSystemParameters

net start MySQL >nul 2>&1

rem Poll rather than guess; there is no sleep on XP.
for /L %%i in (1,1,120) do @C:\MYSQL\bin\mysql.exe --protocol=pipe -u root -e "SELECT 1" >nul 2>&1 && goto ready

rem With no Explorer there is nowhere for an error to show, so put the shell back
rem long enough to be told what went wrong.
echo MySQL did not start. Run C:\MYSQL\bin\mysqld.exe --console to see why.
start "" explorer.exe
goto :eof

:ready
rem The capture point. Everything slow has happened; nothing date-dependent has.
echo DEMO-READY>COM1 2>nul

rem C:\DATE.TXT is a 4 KB file whose second line savestate_server.py rewrites, in
rem the disk block, as it serves it, so this reads the real date at the moment the
rem visitor loaded the page. It is 4 KB so NTFS gives it its own cluster rather
rem than keeping it resident in the MFT record.
for /f "skip=1 tokens=1,2" %%a in (C:\DATE.TXT) do (
  set STAMP=%%a
  set CLOCK=%%b
  goto gotdate
)
:gotdate
if defined STAMP date %STAMP% <nul
if defined CLOCK time %CLOCK% <nul

rem /wait ties the program's lifetime to the session: closing it returns from
rem here, this script ends, and SHELL.VBS shuts down. The program starts
rem maximized on its own (Program.cs sets WindowState).
cd /d C:\APP
start "" /wait "C:\APP\Multi Express Consignment.exe"
