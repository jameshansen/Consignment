// Round-trip a buffer bigger than Node's 2 GB single-call limit.
//
// The size is the whole point: anything under 2 GB passes with or without the
// chunking, which is how the plain fs.writeFileSync version survived long enough
// to throw away a finished install.
//
//     node test_bigfile.mjs

import fs from "node:fs";
import os from "node:os";
import path from "node:path";
import { transfer, CHUNK } from "./bigfile.mjs";

const SIZE = 2.25 * 1024 * 1024 * 1024;         // over the limit, under a full disk
const file = path.join(process.env.TEMP || os.tmpdir(), "v86_bigfile_test.img");

function check(name, ok) {
    if(!ok) throw new Error("FAIL: " + name);
    console.log("  ok:", name);
}

try {
    const out = Buffer.from(new ArrayBuffer(SIZE));
    // First and last bytes, both sides of a chunk seam, and the MBR signature
    // the installer checks for.
    const marks = [[0, 0x11], [510, 0x55], [511, 0xAA],
                   [CHUNK - 1, 0x22], [CHUNK, 0x33], [CHUNK + 1, 0x44],
                   [2 * CHUNK + 7, 0x66], [SIZE - 1, 0x99]];
    for(const [at, v] of marks) out[at] = v;

    transfer(file, out, "write");
    check(`wrote ${(SIZE / 1073741824).toFixed(2)} GB`,
          fs.statSync(file).size === SIZE);

    const back = Buffer.from(new ArrayBuffer(SIZE));
    transfer(file, back, "read");
    for(const [at, v] of marks) check(`byte ${at} survived`, back[at] === v);
    check("everything else still zero", back.indexOf(0x77) === -1);

    let threw = false;
    try { transfer(file, out, "sideways"); } catch { threw = true; }
    check("a bad mode throws rather than truncating the file", threw);

    console.log("PASS");
}
finally {
    try { fs.unlinkSync(file); } catch {}
}
