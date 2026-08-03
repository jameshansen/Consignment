@echo off
rem Drops updated files onto the installed C:. Run from the Safe Mode command
rem prompt (the patch CD is the only optical drive, so it is D:), then reboot.
rem
rem Only needed when the image is being changed by hand. The build itself patches
rem STARTDEMO.CMD straight into the raw image from the host with image_file.py,
rem which needs no boot at all; this CD is the fallback for anything that route
rem cannot do, such as adding a file that is not already on C:.
copy /y "%~dp0STARTDEMO.CMD" C:\STARTDEMO.CMD
echo.
echo Updated C:\STARTDEMO.CMD. Reboot normally, then recapture.
