# Browser demo

Run **Consignment Manager**, a .NET 4.0 WinForms program that needs Windows XP,
MySQL and the Crystal Reports runtime, as a link someone can click. No installer,
no download, no virtual machine on their side. A locked-down Windows XP SP3 guest
boots in the page under [v86](https://github.com/copy/v86) with the program
already running and the database connected.

The whole thing is built by one command from a blank disk. Nothing here needs a
person to watch a screen and press a key.

```bash
python build.py
```

---

## The idea

Install the operating system and the program **under the same emulator that will
later run them**, headless, and snapshot the result.

```
build.py install    v86 in Node runs XP setup unattended against a blank
                    4 GB image, and \SETUP\INSTALL.CMD finishes the job
   |
   v
build.py capture    boot that image, wait for the program to say it is ready,
                    save the machine's state
   |
   v
build.py serve      the page restores that state and reads the disk over
                    HTTP range requests
```

The reason to install under v86 rather than under something convenient is that
**an image built somewhere else has met different hardware**. Two earlier attempts
at this project died on exactly that. One installed under VirtualBox and played
back elsewhere, and spent its life on storage driver mismatches. One installed
under a native build of an emulator and played back in that emulator's WebAssembly
build, which turned out to differ in the ways that mattered, and never booted.

v86 has one implementation and runs headless in Node, so the install and the demo
are the same code over the same state. That is not a nice property, it is the
whole design.

---

## What you need

| | |
|:--- |:--- |
| Node 22 or newer | runs the emulator; also needs `zstdCompressSync`, added in 22.15 |
| Python 3.9 or newer | the build scripts and the server |
| Windows XP SP3 install media | an `.iso`, in `Install/Deps` |
| An XP product key | in `XPKEY`, or `Install/xpkey.txt` |
| About 16 GB of disk | the image is 4 GB and several exist at once |
| About 7 GB of free RAM | the installer holds the 4 GB disk and the 728 MB ISO in memory |

Everything else, including the emulator, is fetched by `npm install` in `v86/`.

### Use volume licence media

Put a volume licence XP ISO in `Install/Deps` if you possibly can. `build_install_iso.py`
prefers any file whose name contains `_vl_`.

Retail media forces Windows Product Activation, and this demo cannot survive it.
The guest has no network, Microsoft's XP activation servers are long retired, and
the clock gets moved to the visitor's real date on every resume, which expires the
activation grace period. The result is a logon loop that no amount of `OOBETimer`
poking reliably escapes. A volume licence key never activates, so the problem does
not exist.

A key only validates against media from its own channel, so a volume key on retail
media gets "The CD Key you entered is not valid" from setup. Match them.

---

## Build it

```bash
cd VM
python build.py
```

That runs every step in order. Budget two to three hours, nearly all of it the
install. Steps also run on their own, and `only` runs one without the rest:

| | | typical |
|:--- |:--- |:--- |
| `python build.py stage` | collect the program and its dependencies into `Install/XPDrives/setupcd` | seconds |
| `python build.py iso` | append the answer file and that payload to the XP media | 2 min |
| `python build.py verify` | read the finished ISO back the way the guest will | seconds |
| `python build.py install` | run XP setup under v86, unattended, to a fresh image | 2 h |
| `python build.py capture` | boot the image, wait for the program, save the state | 5 min |
| `python build.py serve` | serve `Viewer/` on <http://localhost:8899/demo.html> | |

If a run dies partway, `install.mjs` keeps what it had as `*.img.partial`, and

```bash
node install.mjs ../Install/XPDrives/consign-v86.img 8 --from ../Install/XPDrives/consign-v86.img.partial
```

carries on from it instead of starting the hours again.

### Watching a run

The guest reports on its serial port, and the host prints every line:

```
00:03:26 guest: [1/9] Copying program, MySQL and SQL to C:
00:03:31 guest: [2/9] Installing .NET Framework 4.0, this takes a while
01:28:57 guest: INSTALL-COMPLETE
```

`v86/boot.mjs` boots any image with an optional CD and narrates the same way,
which is the tool to reach for when something is wrong:

```bash
node boot.mjs ../Install/XPDrives/consign-v86.img --mem 256 --minutes 10
```

---

## Putting it on a server

The demo is a static directory plus one small dynamic bit for the date. There is
no database behind it, no application server, and nothing to keep running per
visitor: the emulator is entirely in the browser.

Copy these to the host:

```
demo.html  machine.json  libv86.mjs  v86.wasm  bios/  state.bin.zst
xp.img  xp.img.datepatch
```

That is everything in `Viewer/` after a build. `xp.img` is 4 GB but around a
quarter of it is zeros, so send it with something that understands that:

```bash
rsync -avz --sparse Viewer/ demo@host:/srv/consignment/
```

`xp.img.datepatch` has to travel with the image. It is where the date field sits,
written at build time out of the MFT, and both servers below read it. Send a new
image without it and every visitor gets the capture date.

### Two requirements, and one choice

**`Range: bytes=...` must work**, answered with a `206` and a `Content-Range`
carrying the total size. v86 aborts the load if a ranged request comes back `200`.

**The image must not be compressed on the wire.** v86 asks for exact byte ranges
and expects exactly those bytes back.

Then the choice, which is only about the date. A savestate thaws with the guest
clock stopped at capture time, so something has to rewrite the date inside the
disk as it is served. Three ways, in order of how much you have to run:

| | Date | Needs |
|---|---|---|
| `serve.py` behind nginx | today's | a Python process |
| `disk.php` on Apache or nginx | today's | PHP, which the host may already run |
| plain static file | frozen at capture | nothing |

The first two are below. The third is a real option if the site is on a static
host: everything still works, the guest just always thinks it is the day the
snapshot was taken, and the honest thing is to say so on the page.

### nginx in front of serve.py

This serves the small files directly and passes only the disk through Python:

```nginx
server {
    listen 443 ssl;
    server_name demo.example.com;
    root /srv/consignment;

    # Older nginx mime.types has neither .mjs nor .wasm, and a module script
    # served as anything but JavaScript is refused outright. These two lines set
    # the type only when the map has no answer, which is safe either way. Do not
    # reach for a "types { ... }" block here: inside a server it replaces the
    # inherited map rather than adding to it, and takes text/html with it.
    location ~ \.mjs$  { default_type text/javascript; }
    location ~ \.wasm$ { default_type application/wasm; }

    location / {
        gzip on;
        gzip_types text/html text/javascript application/json;
    }

    # The savestate is static and immutable. It is the one real download.
    location = /state.bin.zst {
        gzip off;                       # already zstd
        add_header Cache-Control "public, max-age=31536000, immutable";
    }

    # The disk goes through serve.py, which rewrites the date on the way past.
    # Buffering off, because these are ranged reads of a 4 GB file.
    location = /xp.img {
        proxy_pass http://127.0.0.1:8899;
        proxy_http_version 1.1;
        proxy_buffering off;
        gzip off;
    }
}
```

Drop the last block and put `xp.img` in `root` if you would rather serve it
statically and live with the capture date. nginx does ranges on static files
without being asked.

### serve.py as a service

```ini
[Unit]
Description=Consignment browser demo
After=network.target

[Service]
WorkingDirectory=/srv/consignment
ExecStart=/usr/bin/python3 serve.py 8899 . xp.img
Restart=on-failure
User=www-data

[Install]
WantedBy=multi-user.target
```

`serve.py` binds `127.0.0.1` only, which is what you want behind a proxy.

It needs `xp.img.datepatch` to know where the date field is, and `build.py` writes
that during `capture`, resolved through the MFT. Ship it with the image and leave
it alone. `serve.py --scan` will regenerate one by searching the image for the
block's magic string, but **the image carries two copies of that block and the
first one on disk is not the file**: a scanned sidecar patches a copy nothing
reads, and every visitor sees the capture date while the server reports success.
Scan only if the sidecar is lost, and check the offset against the MFT after:

```bash
python3 ../Install/ntfs_file.py info xp.img DATE.TXT
```

It is `http.server` underneath, threaded but not a production web server. Behind
nginx, serving one file to a handful of concurrent visitors, that is fine.

### PHP, if the host already runs it

`disk.php` is the same 19-byte splice with no Python behind it, for a host that
already speaks PHP. Drop it next to the image and there is no process to keep
alive, no port, and no unit file.

Point the page at it instead of the raw image, which `demo.html` already supports:

```
demo.html?img=disk.php
```

Or hardcode `const IMAGE = "disk.php"` in a copy of the page, which is what the
deployed release does. Then close the image itself, so nobody can bypass the patch
or pull four gigabytes in one request:

```apache
<FilesMatch "^xp\.img(\.datepatch)?$">
    Require all denied
</FilesMatch>
AddType text/javascript .mjs
AddType application/wasm .wasm
```

Two things it does that are worth keeping if you rewrite it in something else.
It sets its own timezone, because PHP falls back to UTC when `date.timezone` is
unset and the guest then lands in tomorrow for anyone visiting after late
afternoon. And it marks only the one block holding the date `no-store`, letting
the browser cache the rest of the disk for a year, because those blocks are
immutable and they are the bulk of the traffic. Marking the whole image
`no-store` costs every returning visitor the whole download again; caching the
date block freezes the clock, which is the bug the file exists to prevent.

### What a visitor actually costs you

The savestate, about 32 MB, plus whatever disk blocks XP reads after the resume,
which is a few megabytes. The 4 GB image is never downloaded, so it is a file that
sits on the disk rather than bandwidth you pay for.

No cross-origin isolation headers are needed. The page does not use
`SharedArrayBuffer`, so `COOP`/`COEP` do not come into it.

---

## How the pieces talk

**The guest reports on COM1.** `INSTALL.CMD` announces each step and ends with
`INSTALL-COMPLETE`. `STARTDEMO.CMD` writes `DEMO-READY` once MySQL answers a query.
`install.mjs` and `capture.mjs` watch that port and act on those two lines.

A serial port is the only channel out of a Windows guest that needs no driver, no
network and no disk flush. Earlier versions of this project tried to signal
through a file on disk, and lost the message to NTFS's write-back cache; then
through an I/O port, which needed the 16-bit subsystem; then through the real time
clock, which needed a patch to the emulator. `echo text >COM1` needs none of that
and the host sees the byte in the same instruction.

**The disk is never downloaded.** v86 asks for the blocks it touches with HTTP
range requests, so a 4 GB image costs the visitor whatever XP actually reads.
`serve.py` answers those requests.

**The date is rewritten in flight.** A savestate thaws with the guest clock frozen
at capture time, and a resumed Windows kernel never re-reads the hardware clock,
so the date has to be pushed into the guest by something running inside it.
`C:\DATE.TXT` is a 4 KB file that `STARTDEMO.CMD` reads immediately after the
resume; `serve.py` rewrites those bytes as the block goes past. Nothing on disk
changes, and two visitors an hour apart each get their own date from the same
file.

The snapshot is deliberately taken *before* that read. `capture.mjs` also checks
that no block written into the savestate covers `DATE.TXT`, because a written
block travels inside the state and would freeze every visitor on the capture date.

**One machine definition.** `Viewer/machine.json` is read by both `capture.mjs`
and the page. A savestate records the sizes the machine had, so a page that
disagrees with the capture restores into the wrong shape and bugchecks. Neither
side keeps its own copy.

---

## Doing this with your own program

Most of this repository is not about Consignment Manager. The parts you would
change are small.

1. **`Install/stage_setupcd.py`** collects your program and its dependencies into
   one tree. Point `APP_SRC` at your build output and put your installers in
   `Install/Deps`.
2. **`Install/XPSetupFiles/INSTALL.CMD`** runs once inside the guest with a full
   desktop up, which is the only place an MSI or a bootstrapper works properly.
   Replace the middle of it. Keep `call :say` for progress, and keep the
   `INSTALL-COMPLETE` line at the end, because that is what ends the build.
3. **`Install/XPSetupFiles/STARTDEMO.CMD`** is the demo session. It is the Windows
   shell, so when it returns the session is over. Keep the `DEMO-READY` line and
   put it where your program is up but has not yet read anything time-dependent.
4. **`Install/XPSetupFiles/WINNT.SIF`** is the answer file. The one line you must
   not change is `ComputerType`, see below.

Things worth keeping whatever you build:

- **Replace the shell instead of adding a Run entry.** `SHELL.VBS` runs in place of
  `explorer.exe`, so there is no taskbar, no Start menu, no desktop, and no tray
  balloons to appear over your program. Closing the program ends the session.
- **NGEN your managed code.** The guest runs at a fraction of native speed, and
  precompiling buys back JIT time on every single cold start.
- **Give Crystal Reports a printer.** It resolves page metrics through the default
  printer and throws if there is not one. A Generic / Text Only driver pointed at
  `FILE:` is enough and pulls in no hardware.

---

## Things that cost us a day each

**The HAL is the one thing that cannot be fixed after the install.** v86 faults
during XP startup with the ACPI HAL, so `WINNT.SIF` must carry
`ComputerType = "Standard PC", Retail` and the machine must run with `acpi: false`.
The second token is `Retail`. This repository said `Retain` for months, which is
not a recognised value, so setup ignored the line and autodetected an ACPI HAL.
The description string also has to match a `[Computer]` entry in the media's
`TXTSETUP.SIF`, where `"Standard PC"` is the key `e_isa_up`.

**Do not give the installer an async CD.** v86's async disks break once the guest
reboots and boots from the hard disk. The drive comes back empty and reads never
complete ([copy/v86#1349](https://github.com/copy/v86/issues/1349)). XP setup
reboots twice, and the third boot is the one that runs `\SETUP\INSTALL.CMD` off
that disc, so an async CD hangs the build about half an hour in with no error at
all. `install.mjs` holds the ISO in memory instead. The symptom is worth
recognising: IDE **reads** freeze while writes carry on for a minute, then the CPU
halts. A black screen with an hourglass is `GuiRunOnce` running, not a crash.

**Node refuses any single `fs` read or write over 2 GB.** A 4 GB image needs
chunked transfers both ways, which is what `v86/bigfile.mjs` is for. The one-line
version works on every test fixture and throws at the end of a two hour run.

**XP's standard VGA driver is 640x480 and nothing else.** See below.

**`SO_REUSEADDR` on Windows lets two processes bind the same port** and splits
connections between them unpredictably, so a stale server from an earlier session
answers half the requests and the new one looks like it is hanging. `serve.py`
sets `allow_reuse_address = False` so this fails loudly.

**Python's `mimetypes` calls `.mjs` `text/plain`**, and browsers refuse a module
script that is not served as JavaScript. The page stops at its first line with
nothing in the console.

---

## Screen resolution

Windows XP ships no display driver that can do more than 640x480 on the hardware
v86 emulates. `WINNT.SIF`'s `[Display]` section and the registry's
`DefaultSettings.XResolution` are both honoured, and both are ignored by a driver
that has no higher mode to offer.

v86's own [Windows NT guest notes](https://github.com/copy/v86/blob/master/docs/windows-nt.md)
point at the **VBE Miniport driver** from
[Bearwindows](https://bearwindows.zcm.com.au/vbemp.htm), which drives any mode the
video BIOS exposes. Put `vbempk.zip` in `Install/Deps` and the build does the rest:
`stage_setupcd.py` lifts the XP driver out of it into a `$OEM$` tree, and
`WINNT.SIF` turns on `OemPreinstall` and points `OemPnPDriversPath` at it, so setup
installs the driver before it detects the display adapter.

Doing it during setup is what makes `[Display]` mean anything, and it is also what
avoids the "do you want to keep these settings?" dialog. That dialog only ever
appears for a resolution changed after the fact; a mode applied at boot from the
driver's own defaults is never confirmed.

> **Licence.** VBEMP is free **for noncommercial use only**. Consignment Manager is
> GPL, so that is fine here. If you are demoing something commercial it is not, and
> the author offers commercial terms by email. The alternative that needs no
> third-party driver is to lay your program's forms out to fit 640x480.

---

## Folders

- **[Install/](Install/)** — building the disk image. The answer file and the guest
  scripts (`XPSetupFiles/`), the ISO tooling, and `Deps/`, which holds every
  dependency in one place.
- **[Viewer/](Viewer/)** — the demo. `demo.html`, the v86 build, and the two
  servers: `serve.py` for local work, `disk.php` for a host that already runs PHP.
- **[v86/](v86/)** — the emulator and the scripts that drive it.
- **[OtherTools/](OtherTools/)** — notes from two approaches abandoned before this
  one. Historical, not part of the build.

---

## Credits

This is mostly other people's work.

| | | |
|:--- |:--- |:--- |
| [v86](https://github.com/copy/v86) | the x86 emulator and x86-to-wasm JIT that makes all of this possible | BSD 2-Clause |
| [SeaBIOS](https://www.seabios.org/) | the PC BIOS the guest boots, shipped with v86 | LGPL v3 |
| [SeaVGABIOS](https://www.seabios.org/SeaVGABIOS) | the video BIOS, shipped with v86 | LGPL v3 |
| [VBE Miniport](https://bearwindows.zcm.com.au/vbemp.htm) | the display driver that gets XP past 640x480 | free for noncommercial use |
| [pycdlib](https://github.com/clalancette/pycdlib) | used by `makeiso.py` to write data ISOs | LGPL v2.1 |
| [MySQL](https://www.mysql.com/) and Connector/Net | the database and its .NET driver | GPL v2 with FOSS exception |
| [SAP Crystal Reports](https://www.crystalreports.com/) runtime | report rendering | SAP licence |
| Microsoft Windows XP | the guest operating system | your own licence and media |

`iso_add.py` exists because rebuilding a Microsoft install disc with an ordinary
ISO tool produces media that will not boot. It appends to the disc without moving
anything already on it, which is a narrower and much more reliable trick than
regenerating the image.

Nothing in this repository redistributes Windows, the Crystal Reports runtime, or
any product key. You supply your own.
