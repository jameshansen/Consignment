// Boot the finished image, wait for the program to be ready, save the state.
//
// The point of the savestate is that a visitor does not sit through a two minute
// XP boot and a MySQL start. STARTDEMO.CMD writes DEMO-READY to COM1 once MySQL
// answers and before it reads C:\DATE.TXT, and the snapshot is taken on that
// byte: everything slow has happened, nothing date-dependent has.
//
// The disk is mounted here exactly as the page mounts it, read only and async.
// That is not a detail. A savestate records the disk backend's own state, so
// capturing over an in-memory disk and resuming into a range-request one hands
// the browser an object of the wrong shape. Same backend both sides, or neither.
//
//     node capture.mjs [image] [out.bin.zst]

import fs from "node:fs";
import zlib from "node:zlib";
import path from "node:path";
import { execFileSync } from "node:child_process";
import { V86 } from "./node_modules/v86/build/libv86.mjs";

const IMAGE = process.argv[2] || "../Install/XPDrives/consign-v86.img";
const OUT = process.argv[3] || "../Viewer/state.bin.zst";
const MACHINE = JSON.parse(fs.readFileSync("../Viewer/machine.json", "utf8"));
// Generous. This boot is light next to the install, but STARTDEMO.CMD polls
// MySQL up to 120 times before giving up and a cold service start on a browser
// emulator is not quick. Failing early here would mean rebooting to find out.
const DEADLINE_MIN = 30;

let serial_line = "";
let captured = false;
const started = Date.now();

const COLS = 80, ROWS = 50;
const screen = new Uint8Array(COLS * ROWS).fill(32);
let mode = "text";

function stamp() {
    const s = Math.round((Date.now() - started) / 1000);
    return `${String((s / 60) | 0).padStart(2, "0")}:${String(s % 60).padStart(2, "0")}`;
}
function log(...a) { console.log(stamp(), ...a); }

const emulator = new V86({
    wasm_path: "./node_modules/v86/build/v86.wasm",
    bios: { url: "./bios/seabios.bin" },
    vga_bios: { url: "./bios/vgabios.bin" },
    hda: { url: IMAGE, async: true, size: fs.statSync(IMAGE).size },
    memory_size: MACHINE.memory_size,
    vga_memory_size: MACHINE.vga_memory_size,
    acpi: MACHINE.acpi,
    boot_order: MACHINE.boot_order,
    fastboot: true,
    autostart: true,
});

emulator.add_listener("screen-put-char", ([row, col, chr]) => {
    if(row < ROWS && col < COLS) screen[row * COLS + col] = chr;
});
emulator.add_listener("screen-set-size", ([w, h, depth]) => {
    const next = depth ? "graphical" : "text";
    if(next !== mode) log(`screen now ${next} (${w}x${h}, bpp ${depth})`);
    mode = next;
});

emulator.add_listener("serial0-output-byte", byte => {
    const c = String.fromCharCode(byte);
    if(c !== "\n" && c !== "\r") {
        if(serial_line.length < 500) serial_line += c;
        return;
    }
    const line = serial_line.trim();
    serial_line = "";
    if(!line) return;
    log("guest:", line);
    if(line.includes("DEMO-READY") && !captured) {
        captured = true;
        capture();
    }
});

// v86 stores an async disk in 256-byte blocks and puts only the ones the guest
// *wrote* into the savestate; reads are never cached, so every other block comes
// off the server on resume. That is what lets serve.py hand the guest today's
// date. It stops being true for C:\DATE.TXT the moment the guest writes to the
// blocks holding it, because a written block travels inside the state and the
// server never gets asked. Nothing should write there, so check rather than hope.
function check_date_block_is_clean() {
    let field_at;
    try {
        field_at = Number(execFileSync(
            "python", ["serve.py", "--scan", path.resolve(IMAGE)],
            { cwd: "../Viewer", encoding: "utf8" }).trim());
    }
    catch {
        log("WARNING: could not locate DATE.TXT; skipping the staleness check");
        return;
    }

    // Reaching into the disk backend: this is a build-time assertion, not
    // shipped behaviour, and there is no public way to ask. ide.primary is the
    // channel and .master the interface on it; ide.master is not a thing, and
    // getting that wrong makes this quietly check nothing, so it throws rather
    // than warns when the shape is not what it expects.
    const disk = emulator.v86?.cpu?.devices?.ide?.primary?.master?.buffer;
    const written = disk?.block_cache_is_write;
    if(!(written instanceof Set)) {
        throw new Error(
            "cannot reach the disk backend's write set at " +
            "v86.cpu.devices.ide.primary.master.buffer.block_cache_is_write. " +
            "v86's internals have moved; fix this check rather than skip it.");
    }

    const first = Math.floor(field_at / 256);
    const last = Math.floor((field_at + 4096 - 1) / 256);
    const dirty = [];
    for(let b = first; b <= last; b++) if(written.has(b)) dirty.push(b);
    if(dirty.length) {
        throw new Error(
            `the guest wrote to the disk blocks holding C:\\DATE.TXT ` +
            `(${dirty.join(", ")}). Those blocks travel inside the savestate, so ` +
            `every visitor would see the capture date. Something read or ` +
            `rewrote that file before the snapshot.`);
    }
    log(`date block clean: ${last - first + 1} blocks at ${field_at}, none written`);
}

async function capture() {
    log("capturing");
    // stop() first: save_state on a running machine can land mid-instruction.
    await emulator.stop();
    check_date_block_is_clean();
    const state = await emulator.save_state();
    await emulator.destroy();

    const raw = Buffer.from(state);
    // zstd because v86 decompresses .zst state images itself, in the browser,
    // with no library on the page. This is the visitor's download.
    const packed = zlib.zstdCompressSync(raw, {
        params: { [zlib.constants.ZSTD_c_compressionLevel]: 19 },
    });
    fs.writeFileSync(OUT, packed);
    log(`state ${(raw.length / 1048576).toFixed(0)} MB -> ${
        (packed.length / 1048576).toFixed(0)} MB  ${OUT}`);
    process.exit(0);
}

const ticker = setInterval(() => {
    log(`${mode}  instr=${(emulator.get_instruction_counter() / 1e9).toFixed(1)}G`);
    if(mode === "text") {
        for(let r = 0; r < ROWS; r++) {
            const line = Buffer.from(screen.subarray(r * COLS, (r + 1) * COLS))
                .toString("latin1").replace(/\s+$/, "");
            if(line.trim()) console.log("  |" + line);
        }
    }
    if(Date.now() - started > DEADLINE_MIN * 60000) {
        clearInterval(ticker);
        log(`no DEMO-READY within ${DEADLINE_MIN} minutes`);
        process.exit(1);
    }
}, 15000);

log(`capturing from ${IMAGE}: ${(MACHINE.memory_size / 1048576).toFixed(0)} MB RAM, ` +
    `acpi ${MACHINE.acpi}`);
