// Install Windows XP SP3 and the program, unattended, under v86.
//
// This is the whole build step: a blank disk goes in, xpsetup.iso drives a fully
// unattended setup, \SETUP\INSTALL.CMD finishes the job, and a bootable image
// comes out. Nothing here needs a person watching it.
//
// It runs headless in Node deliberately. The browser demo is the same emulator
// over the same wasm, so an image built here has never met a different machine:
// no foreign IDE controller, no foreign HAL, none of the driver mismatch that
// comes from installing under one emulator and playing back in another.
//
// The guest reports progress on COM1. That is the only channel out of a Windows
// guest that costs nothing and cannot be buffered away by a write-back cache,
// and INSTALL-COMPLETE on it is what ends the run.
//
//     node install.mjs [out.img] [hours] [--from image.img]
//
// --from carries on from an existing image rather than a blank one, which turns
// a failed run into a short one instead of another few hours.

import fs from "node:fs";
import { transfer } from "./bigfile.mjs";
import { V86 } from "./node_modules/v86/build/libv86.mjs";

const ISO = "../Install/XPDrives/xpsetup.iso";
const OUT = process.argv[2] || "../Install/XPDrives/consign-v86.img";
const DEADLINE_H = Number(process.argv[3] || 8);
// --from seeds the disk from an existing image instead of a blank one, to carry
// on from a run that died rather than start the hours again.
const FROM = (() => {
    const i = process.argv.indexOf("--from");
    return i < 0 ? null : process.argv[i + 1];
})();

// 4 GB, not 3. At 3 GB the .NET 4 installer stop-blocks: it wants 842 MB free and
// GUI setup's 768 MB pagefile leaves about 705 MB, so setup fails in one second
// with 5100 and the run carries on without .NET. The disk is streamed block by
// block in the browser and empty blocks are never fetched, so the extra gigabyte
// costs the demo nothing. bigfile.mjs already chunks every transfer, and Node's
// 2 GB per-call fs limit is the only size limit in the path.
const DISK_SIZE = 4 * 1024 * 1024 * 1024;
// XP's own minimum is 64 MB, but v86's docs ask for 512 MB for the NT family and
// the .NET installer is the heaviest thing this guest ever runs.
const MEMORY = 512 * 1024 * 1024;
// Windows dismounts its volumes and flushes on the way down. Snapshotting before
// that finishes is how you get a file whose metadata committed and whose data
// did not, so wait for the disk to go quiet rather than for the shutdown command.
const QUIET_MS = 30000;
// A guest that has stopped touching its disk for this long, this far into a run,
// is not installing any more: it is sitting at a prompt, or wedged, or it
// finished and never managed to say so. Ending the run beats waiting out the
// deadline, and the image is kept as .partial to be looked at.
const STALL_MS = 6 * 60000;
const STALL_AFTER_MS = 15 * 60000;
// XP wedges at the end of GUI mode setup, every clean run, about 45 minutes in:
// setupact.log records "GUI mode Setup has finished", and then the restart into
// first logon never happens. The machine stays up and idle, so GuiRunOnce never
// fires and \SETUP\INSTALL.CMD never runs.
//
// Writes are no use for spotting it. The guest keeps trickling a few dozen
// sectors every half minute, which held last_activity fresh and let one run idle
// for eight hours inside a six minute stall window. Reads are unambiguous: the
// counter freezes at the exact instruction the guest stops running new code, and
// a healthy install never stops reading for minutes at a time, not through text
// setup, not through .NET, not through NGEN.
//
// Booting the half-installed image cold is known to carry straight on into
// GuiRunOnce, so do that, in memory: reboot_internal resets the CPU and the BIOS
// and leaves the disk buffers alone. Once only. A second freeze is a different
// problem and should be looked at rather than papered over.
const READ_STALL_MS = 5 * 60000;
const RESTART_AFTER_MS = 25 * 60000;

const COLS = 80, ROWS = 50;
const screen = new Uint8Array(COLS * ROWS).fill(32);
let bpp = 0, mode = "text";
let reads = 0, writes = 0, written_sectors = 0, last_write = Date.now();
// Serial counts as being alive too. NGEN precompiles for minutes at a time
// almost entirely in the CPU, and a stall detector watching only the disk would
// throw away a healthy run near the end of it.
let last_activity = Date.now();
let serial_line = "";
let complete = false, failed = false;
let last_read = Date.now(), restarted = false;
const started = Date.now();

const disk = new ArrayBuffer(DISK_SIZE);
if(FROM) transfer(FROM, Buffer.from(disk), "read");

// The install media, held in memory rather than read from the file as it goes.
//
// v86's async disks break when the guest reboots and then boots from the hard
// disk: the CD comes back empty and reads never complete (copy/v86#1349). XP
// setup reboots twice, and the third boot is the one that runs \SETUP\INSTALL.CMD
// off this disc, so the async version of this line hangs the install every time,
// silently, thirty minutes in. 728 MB of resident memory is a cheap way out on a
// machine that is already holding a 4 GB disk.
const iso = fs.readFileSync(ISO);
const iso_buffer = iso.buffer.slice(iso.byteOffset, iso.byteOffset + iso.byteLength);

// The ISO is read into memory once, above, so a rebuild during a run no longer
// corrupts it. It does silently give the run different media from the one the
// build thinks it produced, so build_install_iso.py still refuses while this
// file is here.
const LOCK = "../.install-running";
fs.writeFileSync(LOCK, `${process.pid}\n${new Date().toISOString()}\n`);
for(const sig of ["exit", "SIGINT", "SIGTERM"]) {
    process.on(sig, () => { try { fs.unlinkSync(LOCK); } catch {} });
}

function stamp() {
    const s = Math.round((Date.now() - started) / 1000);
    return `${String((s / 3600) | 0).padStart(2, "0")}:${
        String(((s / 60) | 0) % 60).padStart(2, "0")}:${String(s % 60).padStart(2, "0")}`;
}
function log(...a) { console.log(stamp(), ...a); }

const emulator = new V86({
    wasm_path: "./node_modules/v86/build/v86.wasm",
    bios: { url: "./bios/seabios.bin" },
    vga_bios: { url: "./bios/vgabios.bin" },
    cdrom: { buffer: iso_buffer },
    // The live disk. v86 wraps this ArrayBuffer directly and writes through it,
    // so when the run ends this *is* the finished image.
    hda: { buffer: disk },
    memory_size: MEMORY,
    vga_memory_size: 8 * 1024 * 1024,
    boot_order: 0x123,          // CD, then hard disk, then floppy
    // Must match WINNT.SIF's ComputerType = "Standard PC". With the ACPI HAL,
    // XP faults during startup under v86.
    acpi: false,
    fastboot: true,
    autostart: true,
});

emulator.add_listener("serial0-output-byte", byte => {
    const c = String.fromCharCode(byte);
    if(c === "\n" || c === "\r") {
        const line = serial_line.trim();
        serial_line = "";
        if(!line) return;
        last_activity = Date.now();
        log("guest:", line);
        // A step that failed still lets the script run to the end, so the image
        // is written as .partial and the run exits non-zero rather than passing
        // off a half-installed disk as finished. .NET 4 failing this way, and
        // INSTALL-COMPLETE arriving anyway, is what produced an image with no
        // framework, no Crystal runtime and nothing to run the program with.
        if(line.includes("INSTALL-FAILED")) failed = true;
        if(line.includes("INSTALL-COMPLETE")) {
            complete = true;
            log("install reported complete; waiting for the disk to go quiet");
        }
    }
    else if(serial_line.length < 500) serial_line += c;
});

emulator.add_listener("screen-put-char", ([row, col, chr]) => {
    if(row < ROWS && col < COLS) screen[row * COLS + col] = chr;
});
emulator.add_listener("screen-set-size", ([w, h, depth]) => {
    const next = depth ? "graphical" : "text";
    if(next !== mode) log(`screen now ${next} (${w}x${h}, bpp ${depth})`);
    mode = next; bpp = depth;
});
emulator.add_listener("ide-read-end", () => { reads++; last_read = Date.now(); });
emulator.add_listener("ide-write-end", ([, , sectors]) => {
    writes++; written_sectors += sectors;
    last_write = last_activity = Date.now();
});

function text_screen() {
    const out = [];
    for(let r = 0; r < ROWS; r++) {
        const line = Buffer.from(screen.subarray(r * COLS, (r + 1) * COLS))
            .toString("latin1").replace(/\s+$/, "");
        if(line.trim()) out.push("  |" + line);
    }
    return out.join("\n");
}

function status() {
    log(`${mode}  instr=${(emulator.get_instruction_counter() / 1e9).toFixed(1)}G` +
        `  ide r=${reads} w=${writes} (${(written_sectors / 2048).toFixed(0)} MB written)` +
        `  quiet=${((Date.now() - last_write) / 1000).toFixed(0)}s`);
    if(mode === "text") {
        const s = text_screen();
        if(s) console.log(s);
    }
}

async function finish(ok) {
    clearInterval(ticker);
    status();
    await emulator.destroy();

    // A disk that never got a partition table is a run that failed early, and
    // writing it out under the real name would hide that.
    const view = new Uint8Array(disk);
    const boot_signature = view[510] === 0x55 && view[511] === 0xAA;
    const path = ok && boot_signature ? OUT : OUT + ".partial";
    if(!boot_signature) log("WARNING: no MBR signature on the disk");

    log(`writing ${(DISK_SIZE / 1048576).toFixed(0)} MB to ${path}`);
    transfer(path, Buffer.from(disk), "write");
    const written_bytes = fs.statSync(path).size;
    if(written_bytes !== DISK_SIZE) {
        log(`WARNING: wrote ${written_bytes} bytes, expected ${DISK_SIZE}`);
    }
    log("done");
    process.exit(ok && boot_signature ? 0 : 1);
}

const ticker = setInterval(() => {
    status();
    if(complete && Date.now() - last_write > QUIET_MS) {
        log(`disk quiet for ${QUIET_MS / 1000}s after shutdown; image is settled`);
        if(failed) log("a step reported INSTALL-FAILED; keeping the image as .partial");
        finish(!failed);
    }
    else if(!complete && !restarted && Date.now() - started > RESTART_AFTER_MS &&
            Date.now() - last_read > READ_STALL_MS) {
        restarted = true;
        log(`no disk reads for ${READ_STALL_MS / 60000} minutes: the guest has ` +
            `stopped running new code. This is the hang at the end of GUI setup. ` +
            `Restarting the machine, which is the reboot it owed us.`);
        emulator.restart();
        last_read = last_write = last_activity = Date.now();
    }
    else if(Date.now() - started > STALL_AFTER_MS &&
            Date.now() - last_activity > STALL_MS) {
        log(`silent on disk and on COM1 for ${STALL_MS / 60000} minutes, with no ` +
            `INSTALL-COMPLETE. Either the guest is stuck, or it finished and the ` +
            `serial port never reached the host. Keeping the image to look at.`);
        finish(false);
    }
    else if(Date.now() - started > DEADLINE_H * 3600 * 1000) {
        log(`deadline of ${DEADLINE_H}h reached without INSTALL-COMPLETE`);
        finish(false);
    }
}, 30000);

log(`installing to ${OUT}: ${(DISK_SIZE / 1073741824).toFixed(0)} GB disk, ` +
    `${(MEMORY / 1048576).toFixed(0)} MB RAM, acpi off, deadline ${DEADLINE_H}h`);
