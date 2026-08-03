' A sleep, because Windows XP does not ship one.
'
' No timeout.exe, no choice.exe, and ping needs a TCP/IP stack this image
' deliberately does not have. A busy loop would work but its duration depends on
' how fast the emulator happens to be running. WScript.Sleep is a real timed
' wait against the system tick, which the savestate preserves and resumes.
'
' STARTDEMO.CMD calls this to hold the machine still while the savestate is
' captured. Whatever is left of the wait elapses after the resume, and then the
' session carries on by itself, which is why the demo needs nothing typed at it.
'
'   cscript //nologo C:\SLEEP.VBS 45

Dim secs
secs = 30
If WScript.Arguments.Count > 0 Then secs = CInt(WScript.Arguments(0))
WScript.Sleep secs * 1000
