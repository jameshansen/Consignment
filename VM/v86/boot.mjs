// Boot an image and report what it does. A bench, not part of the build.
//
// install.mjs and capture.mjs each boot a machine for one purpose and end when
// they get what they came for. This one just boots and narrates, so a half built
// or wedged image can be looked at without a 35 minute install in front of it.
//
//     node boot.mjs <image> [--cd <iso>] [--cts] [--mem 512] [--minutes 10]
//
// --cts asserts the modem status lines the emulated UART otherwise leaves low.
// v86 starts the port with modem_status = 0, so CTS, DSR and DCD all read as
// deasserted, and a guest that opens COM1 with output handshaking on will block
// on its first write for ever.

import fs from "node:fs";
import { V86 } from "./node_modules/v86/build/libv86.mjs";

const args = process.argv.slice(2);
const IMAGE = args[0];
const flag = (name, fallback) => {
    const i = args.indexOf("--" + name);
    return i < 0 ? fallback : (args[i + 1] ?? true);
};
if(!IMAGE || IMAGE.startsWith("--")) {
    console.error("usage: node boot.mjs <image> [--cd <iso>] [--cts] " +
                  "[--mem 512] [--minutes 10]");
    process.exit(2);
}
const CD = flag("cd", null);
const CTS = args.includes("--cts");
const MEM = Number(flag("mem", 512));
const MINUTES = Number(flag("minutes", 10));

let reads = 0, writes = 0, last_write = Date.now();
let serial_line = "";
let mode = "text";
const started = Date.now();

const COLS = 80, ROWS = 50;
const screen = new Uint8Array(COLS * ROWS).fill(32);

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
    ...(CD ? { cdrom: { url: CD, async: true, size: fs.statSync(CD).size } } : {}),
    memory_size: MEM * 1024 * 1024,
    vga_memory_size: 8 * 1024 * 1024,
    boot_order: CD ? 0x123 : 0x132,
    acpi: false,
    fastboot: true,
    autostart: true,
});

emulator.add_listener("serial0-output-byte", byte => {
    const c = String.fromCharCode(byte);
    if(c !== "\n" && c !== "\r") {
        if(serial_line.length < 500) serial_line += c;
        return;
    }
    const line = serial_line.trim();
    serial_line = "";
    if(line) log("guest:", line);
});
emulator.add_listener("screen-put-char", ([row, col, chr]) => {
    if(row < ROWS && col < COLS) screen[row * COLS + col] = chr;
});
emulator.add_listener("screen-set-size", ([w, h, depth]) => {
    const next = depth ? "graphical" : "text";
    if(next !== mode) log(`screen now ${next} (${w}x${h}, bpp ${depth})`);
    mode = next;
});
emulator.add_listener("ide-read-end", () => reads++);
emulator.add_listener("ide-write-end", () => { writes++; last_write = Date.now(); });

emulator.add_listener("emulator-started", () => {
    if(!CTS) return;
    // Raise CTS, DSR and DCD. Nothing in v86 drives these, and every one of them
    // is a line a Windows serial write can wait on.
    for(const line of ["clear-to-send", "data-set-ready", "carrier-detect"]) {
        emulator.bus.send(`serial0-${line}-input`, true);
    }
    log("asserted CTS, DSR and DCD on COM1");
});

setInterval(() => {
    log(`${mode}  ide r=${reads} w=${writes}  quiet=${
        ((Date.now() - last_write) / 1000).toFixed(0)}s`);
    if(mode === "text") {
        for(let r = 0; r < ROWS; r++) {
            const line = Buffer.from(screen.subarray(r * COLS, (r + 1) * COLS))
                .toString("latin1").replace(/\s+$/, "");
            if(line.trim()) console.log("  |" + line);
        }
    }
    if(Date.now() - started > MINUTES * 60000) {
        log("time up");
        process.exit(0);
    }
}, 15000);

emulator.add_listener("screen-set-size", ([w, h, depth]) => {
    if(depth) log(`GRAPHICAL MODE ${w}x${h} bpp${depth}`);
});

log(`booting ${IMAGE}${CD ? " with " + CD : ""}: ${MEM} MB, cts ${CTS}`);
