' Windows shell replacement for the demo image.
'
' Winlogon runs this instead of explorer.exe. No Explorer means no taskbar, no
' Start menu, no desktop icons and no notification area, so none of the tray
' balloons XP otherwise throws at a new profile can appear at all. It also saves
' the memory and the disk reads Explorer would cost, which matters because the
' savestate is a snapshot of RAM.
'
' Two lines of VBScript rather than pointing Winlogon straight at STARTDEMO.CMD,
' because a batch file as the shell leaves a console window on screen for the
' whole session. Run with the window style 0 it is never drawn.
'
' The True on the first Run is what makes closing the program shut the machine
' down: it blocks until STARTDEMO.CMD returns, and that only happens once the
' program it waits on has exited.

Set sh = CreateObject("WScript.Shell")
sh.Run "C:\STARTDEMO.CMD", 0, True
sh.Run "shutdown.exe -s -t 0 -f", 0, False
