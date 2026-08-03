# Viewer

The demo itself. A static page, a disk image, a savestate, and a small server that
knows two tricks.

```bash
python ../build.py serve      # or: python serve.py 8899 . xp.img
```

then open <http://localhost:8899/demo.html>.

## Layout

| | |
|:--- |:--- |
| `demo.html` | the page |
| `serve.py` | static server with HTTP range support and the date rewrite |
| `disk.php` | the same date rewrite for a host that runs PHP instead |
| `machine.json` | the machine definition, read by both the page and `capture.mjs` |
| `dateblock.py` | the layout of `C:\DATE.TXT`, shared by the build and the servers |
| `libv86.mjs`, `v86.wasm` | the emulator, copied from `../v86/node_modules/v86/build` |
| `bios/` | SeaBIOS and SeaVGABIOS, copied from the v86 repository |
| `xp.img` | the disk, copied here by `build.py capture` |
| `xp.img.datepatch` | where the date field sits, written by `build.py capture` from the MFT |
| `state.bin.zst` | the savestate |
| `test_serve.py`, `test_dateblock.py` | the checks |

`xp.img`, `xp.img.datepatch` and `state.bin.zst` are build output and are not in
the repository.

`serve.py` and `disk.php` do the same job and read the same sidecar; pick whichever
suits the host. `serve.py` is what `build.py serve` runs, so it is the one to use
locally. See "Putting it on a server" in [../README.md](../README.md).

## What the page does

By default it restores `state.bin.zst`, which is the machine as it stood the moment
MySQL answered. That takes seconds. `?cold` boots the disk from scratch instead,
which takes minutes and is how you tell an image problem from a savestate problem.

| | |
|:--- |:--- |
| `demo.html` | resume the snapshot |
| `demo.html?cold` | cold boot, ignore the snapshot |
| `demo.html?img=other.img` | point at a different image in this directory |
| `demo.html?img=disk.php` | read the disk through PHP rather than `serve.py` |

The emulator is left on `window.emulator`. A guest that comes up black gives you
nothing to go on from outside the module scope, and that one line is the difference
between guessing and reading `v86.cpu` state.

## The server

**Range requests.** v86 reads the disk with ordinary HTTP range requests and aborts
the load if it gets a 200 to one, and Python's own handler answers every request
with the whole file. `Range: bytes=0-0` is also how v86 asks for the size, so the
total in `Content-Range` has to be right.

Two things about this that are easy to lose an hour to:

- `HTTPServer` sets `SO_REUSEADDR`, and on Windows that lets a **second** process
  bind a port that is already in use. Connections then go to whichever one the stack
  feels like, so a stale server from an earlier session answers half the requests and
  the new one looks like it is hanging. `serve.py` sets `allow_reuse_address = False`
  so this fails loudly instead.
- Python's `mimetypes` does not know `.mjs` and calls it `text/plain`. Browsers
  refuse a module script that is not served as JavaScript, and the page then stops at
  its first line with an empty console.

**The date.** A savestate thaws with the guest clock frozen at capture time, and a
resumed Windows kernel never re-reads the hardware clock, so nothing the emulator
does to its RTC will help. The date has to be handed to something running inside the
guest.

`C:\DATE.TXT` is a 4 KB file whose second line `STARTDEMO.CMD` reads immediately
after the resume and sets the clock from. `serve.py` rewrites those bytes as the
block goes past. Nothing on disk changes, the image stays read only, and two
visitors an hour apart each get their own date from the same file.

Where that file landed is NTFS's business, so it is found by scanning the image once
for a magic string and remembered in a `.datepatch` sidecar keyed on the image's size
and mtime. A rebuilt image rescans, a served one does not.

Three things have to agree about where the date field sits, so the layout lives in
`dateblock.py` and nowhere else. The 4 KB size is deliberate: at that size NTFS gives
the file its own cluster instead of keeping it resident in the MFT record, where
unrelated boot metadata would drag it into the guest's file cache long before it is
read.

## machine.json

A savestate records the sizes the machine had. A page that disagrees with the capture
restores into the wrong shape and bugchecks, so neither side keeps its own copy of
this.

```json
{ "memory_size": 268435456, "vga_memory_size": 8388608,
  "acpi": false, "boot_order": 306 }
```

`acpi` must stay `false`, because the image was installed with the Standard PC HAL.
256 MB is not the 512 MB the install runs with: the disk does not care what it was
installed with, and this number is what every visitor downloads, since the savestate
carries a flat copy of RAM.

## Serving it somewhere real

Everything in this directory after a build is the deployable artifact. There is no
application server behind it and nothing runs per visitor, because the emulator is
entirely in the browser.

Two requirements, whatever you host it on:

- **`Range: bytes=...` must be supported**, answered with a 206 and a `Content-Range`
  carrying the total size. v86 aborts the load on a 200.
- **The disk image must not be compressed on the wire.** v86 asks for exact byte
  ranges and expects exactly those bytes back.

Serving it without `serve.py` loses the date rewrite, and every visitor sees the
capture date instead of their own.

The nginx and systemd configuration, and what a visitor actually costs, are in
[putting it on a server](../README.md#putting-it-on-a-server).
