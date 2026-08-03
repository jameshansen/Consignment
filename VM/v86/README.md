# v86

The emulator and the three scripts that drive it. Everything here runs in Node, on
the same compiled emulator the browser runs, which is the point of the whole design.

```bash
npm install          # fetches v86, prebuilt; no Rust or emscripten toolchain needed
```

The BIOS files are not in the npm package and are fetched separately into `bios/`
from the [v86 repository](https://github.com/copy/v86/tree/master/bios).

| | |
|:--- |:--- |
| `install.mjs` | run XP setup unattended against a blank disk and write the finished image |
| `capture.mjs` | boot a finished image, wait for the program, save the state |
| `boot.mjs` | boot anything and narrate what it does. A bench, not part of the build |
| `bigfile.mjs` | chunked reads and writes for images over 2 GB |
| `test_bigfile.mjs` | round-trips 2.25 GB and checks both sides of a chunk seam |

## install.mjs

```bash
node install.mjs [out.img] [hours] [--from image.img]
```

Holds the 4 GB disk as an `ArrayBuffer` that v86 writes through, so when the run
ends that buffer *is* the finished image. The install media is held in memory too,
which is not an optimisation but a workaround: v86's async disks break once the
guest reboots and boots from the hard disk, and XP setup reboots twice
([copy/v86#1349](https://github.com/copy/v86/issues/1349)).

The run ends when the guest writes `INSTALL-COMPLETE` to COM1 and the disk then goes
quiet for 30 seconds. Waiting for the disk rather than for the shutdown command
matters, because Windows dismounts and flushes on the way down, and snapshotting
before that finishes produces files whose metadata committed and whose data did not.

Two guards worth knowing about. A run that goes silent on both disk and serial for
six minutes is ended early and its image kept as `.partial`, rather than sitting out
the deadline. And a `.install-running` lock file stops `build_install_iso.py`
rebuilding the media underneath a run.

`--from` seeds the disk from an existing image, which turns a failed run into a short
one instead of another two hours.

## capture.mjs

```bash
node capture.mjs [image] [out.bin.zst]
```

Boots the image and snapshots it the moment `STARTDEMO.CMD` writes `DEMO-READY`,
which is after MySQL answers and before anything reads the date.

The disk is mounted here exactly as the page mounts it, read only and async. That is
not a detail. A savestate records the disk backend's own state, so capturing over an
in-memory disk and resuming into a range-request one hands the browser an object of
the wrong shape.

Before saving, it checks that no block written into the savestate covers
`C:\DATE.TXT`. v86 puts only *written* blocks into a savestate and never caches
reads, so every other block is fetched from the server on resume, which is what lets
the server hand each visitor today's date. A written block would travel inside the
state and freeze every visitor on the capture date.

The state is compressed with zstd because v86 decompresses `.zst` state images
itself, in the browser, with no library on the page. 125 MB of machine becomes about
28 MB.

## boot.mjs

```bash
node boot.mjs <image> [--cd <iso>] [--cts] [--mem 512] [--minutes 10]
```

Boots and narrates: serial output, screen mode changes, disk activity, and the text
screen when there is one. This is the tool for looking at a half built or wedged
image without a two hour install in front of it.

`--cts` raises the modem status lines, which v86 otherwise leaves low. It turns out
not to be needed, since `echo text >COM1` from `cmd.exe` works without it, but a
serial write that blocks for ever is a plausible enough failure to keep the switch
around.
