@echo off
rem Startup script for the demo. LiveXP's Run key points here; see
rem set_run_value.py for how that is wired up.
rem
rem LiveXP has no xcopy, only cmd's built-ins, so nothing here recurses.
rem MySQL and the runtime live on the CD; only what has to be written goes to
rem the RAM disk, which is 63 MB and shared with everything else.

set SRC=%~d0
set DEST=B:\demo

rem LiveXP normally launches this from the Run key we took over. It is not a
rem screensaver, it runs part of the post-boot script chain, and the shell does
rem not finish starting without it.
if defined PEsys32 start "" "%PEsys32%\screensaver.exe"

echo.
echo Consignment Manager demo
echo Source: %SRC%   Target: %DEST%
echo.

rem Halfix sets the RTC year but not the CMOS century byte, so XP reads 2026 as
rem 1926. MySQL refuses to start on a pre-1970 clock ("doesn't support dates
rem later then 2038", which is what it says for out of range in either
rem direction), so correct the century before anything else runs.
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (
  set MM=%%a
  set DD=%%b
  set YY=%%c
)
if %YY% LSS 1970 set /a YY=%YY%+100
date %MM%-%DD%-%YY%
echo Clock set to %MM%-%DD%-%YY%.

echo [1/5] Registering .NET Framework 4.0...
call %SRC%\netreg.bat

echo [2/5] Copying to the RAM disk...
md %DEST%\data\mysql 2>nul
md %DEST%\app 2>nul
md %DEST%\export 2>nul
copy %SRC%\MYSQL\data\mysql\*.* %DEST%\data\mysql\ >nul

rem APP already holds the runtime assemblies merged in by stage_app.py. Copying
rem the build output and the runtime separately overflowed the 63 MB RAM disk
rem and took the RAMDisk driver down with it.
copy %SRC%\APP\*.* %DEST%\app\ >nul
copy %SRC%\settings.ini %DEST%\app\ >nul

echo [3/5] Checking that managed code runs...
%DEST%\app\hello.exe
if errorlevel 1 echo .NET smoke test FAILED.
%DEST%\app\wintest.exe

echo [4/5] Starting MySQL...
start "MySQL" /min %SRC%\MYSQL\bin\mysqld.exe --basedir=%SRC%/MYSQL --datadir=%DEST%/data --skip-networking --enable-named-pipe --skip-innodb

rem No sleep on XP, so poll the server instead of guessing at a delay.
for /L %%i in (1,1,60) do @%SRC%\MYSQL\bin\mysql.exe --protocol=pipe -u root -e "SELECT 1" >nul 2>&1 && goto ready
echo MySQL did not start. Run mysqld.exe again with --console to see why.
goto :eof

:ready
echo Loading the database...
%SRC%\MYSQL\bin\mysql.exe --protocol=pipe -u root < %SRC%\SQL\consignment_db_structure.sql
%SRC%\MYSQL\bin\mysql.exe --protocol=pipe -u root < %SRC%\SQL\demo_database.sql

echo [5/5] Starting Consignment Manager...
cd /d %DEST%\app
start "" "%DEST%\app\Multi Express Consignment.exe"
