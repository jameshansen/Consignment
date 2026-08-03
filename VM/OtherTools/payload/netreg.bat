@echo off
rem Registers the .NET Framework 4.0 that ships on this CD.
rem
rem LiveXP's registry lives in RAM, rebuilt from the CD on every boot, so these
rem keys have to be written at startup. That is fine: reg.exe is one of the 168
rem files the image does ship.
rem
rem mscoree.dll reads InstallRoot to find the runtime. The shim itself is not
rem here but next to each executable, where the OS loader looks first, which
rem avoids needing to write to the read-only system32 on the boot CD.

set NETROOT=%~d0\DOTNET\Framework

echo Registering .NET Framework 4.0 at %NETROOT%\

rem The trailing backslash has to be doubled. reg.exe goes through the usual
rem command line parser, so "...\Framework\" ends in an escaped quote: the
rem argument runs on and the value lands as  D:\DOTNET\Framework" /f  which the
rem shim cannot resolve, and the only symptom is "you first must install .NET".
reg add "HKLM\SOFTWARE\Microsoft\.NETFramework" /v InstallRoot /t REG_SZ /d "%NETROOT%\\" /f >nul

rem The shim decides which builds exist from this policy range. An empty value
rem is not enough: it reports "you first must install .NET Framework v4.0.30319"
rem even with the runtime sitting right there. A real install writes 30319-30319.
reg add "HKLM\SOFTWARE\Microsoft\.NETFramework\policy\v4.0" /v 30319 /t REG_SZ /d "30319-30319" /f >nul
reg add "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Install /t REG_DWORD /d 1 /f >nul
reg add "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Version /t REG_SZ /d "4.0.30319" /f >nul
reg add "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" /v Install /t REG_DWORD /d 1 /f >nul
reg add "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" /v Version /t REG_SZ /d "4.0.30319" /f >nul
echo Done.
