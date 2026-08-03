// Read and write disk images larger than 2 GB.
//
// Node's fs refuses any single read or write longer than 2147483647 bytes with
// ERR_OUT_OF_RANGE, and every disk image here is 4 GB. The obvious
// fs.readFileSync / fs.writeFileSync one-liners work fine on every test fixture
// and then throw at exactly the two moments a run cannot afford it: seeding the
// disk at the start, and saving it at the end. Both go through here instead.

import fs from "node:fs";

export const CHUNK = 1 << 28;           // 256 MB

/** Copy a whole Buffer to or from a file, in slices Node will accept. */
export function transfer(path, buf, mode) {
    if(mode !== "read" && mode !== "write") {
        throw new Error(`transfer mode must be "read" or "write", got ${mode}`);
    }
    const fd = fs.openSync(path, mode === "read" ? "r" : "w");
    try {
        for(let at = 0; at < buf.length; at += CHUNK) {
            const n = Math.min(CHUNK, buf.length - at);
            if(mode === "read") fs.readSync(fd, buf, at, n, at);
            else fs.writeSync(fd, buf, at, n, at);
        }
    }
    finally { fs.closeSync(fd); }
}
