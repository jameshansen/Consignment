# OtherTools

**Historical. Nothing here is part of the build.**

Notes and scripts from two approaches that were tried and abandoned before the
current one. They are kept because the failures are documented, and the same walls
are easy to walk into twice.

| | |
|:--- |:--- |
| `livecd.md` | a LiveXP / PE build. Abandoned: no room for a real .NET install, and registry edits could not change a string's length in place |
| `vbox-hda.md` | install under VirtualBox, play back elsewhere. Abandoned: the image faults in `atapi.sys` because the two present different disk controllers |
| `build_vm.ps1`, `stage_app.py`, `stage_gac.py`, `extract_netfx.py` | the VirtualBox-era build |
| `set_resolution.py`, `set_hdd_resolution.py` | earlier resolution patchers. The working one is now `../Install/set_resolution.py` |
| `payload/` | the LiveXP-era payload |

The lesson both of them paid for is the one the current build is designed around:
**install the operating system under the emulator that will run it.** An image that
has met different hardware brings the wrong drivers with it, and no amount of
registry surgery afterwards is as reliable as not creating the problem.

These files refer to emulators and layouts that no longer exist in this repository.
Read them as a record of what was tried, not as instructions.
