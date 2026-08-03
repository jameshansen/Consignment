"""Serve the demo, and put today's date into the disk on the way past.

v86 reads the disk image with ordinary HTTP range requests, so this is a static
file server with two things added.

**Ranges.** Python's own handler answers every request with the whole file, and
v86 aborts the load when it sees a 200 to a ranged request, so ranges are
implemented here. `Range: bytes=0-0` is also how v86 asks the size, which is why
the total in `Content-Range` has to be right.

**The date.** A savestate thaws with the guest clock frozen at capture time, and
this guest has no network and no other clock source. `C:\\DATE.TXT` is a 4 KB file
that `STARTDEMO.CMD` reads just after the resume and sets the clock from; the
bytes of it are rewritten here, in flight, so what the guest reads is the moment
the visitor opened the page. Nothing on disk changes: the image is read only and
every visitor gets their own date off the same file. See dateblock.py.

Where that file landed is NTFS's business, so it is found by scanning the image
once for the magic and remembered in a sidecar next to it.

    python serve.py                    serve . on 8899
    python serve.py 8899 . xp.img
    python serve.py --scan xp.img      print the date field offset, cache it
"""

import datetime
import json
import os
import re
import sys
from http.server import HTTPServer, SimpleHTTPRequestHandler
from socketserver import ThreadingMixIn

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import dateblock

IMAGE = "xp.img"        # the one file the date is patched into
PATCH_AT = None         # absolute byte offset of the date field, or None
RANGE_RE = re.compile(r"^bytes=(\d+)-(\d*)$")


def find_date_field(path):
    """Absolute offset of the date field inside the image, via a sidecar cache.

    Scanning four gigabytes takes a while and the answer only changes when the
    image does, so it is written down. The image's size and mtime are the key:
    a rebuilt image rescans, a served one does not.
    """
    sidecar = path + ".datepatch"
    st = os.stat(path)
    key = {"size": st.st_size, "mtime": int(st.st_mtime)}
    if os.path.exists(sidecar):
        try:
            saved = json.load(open(sidecar))
            if all(saved.get(k) == v for k, v in key.items()):
                return saved["offset"]
        except (ValueError, KeyError):
            pass

    print("  scanning %s for the date block..." % os.path.basename(path), file=sys.stderr)
    hits = []
    chunk = 1 << 24
    carry = b""
    pos = 0
    with open(path, "rb") as f:
        while True:
            buf = f.read(chunk)
            if not buf:
                break
            window = carry + buf
            base = pos - len(carry)
            start = 0
            while True:
                i = window.find(dateblock.MAGIC, start)
                if i < 0:
                    break
                hits.append(base + i)
                start = i + 1
            carry = window[-len(dateblock.MAGIC):]
            pos += len(buf)

    if not hits:
        print("  no date block found; the guest will keep the capture date", file=sys.stderr)
        return None
    if len(hits) > 1:
        # The copy on the setup CD is not in this image, so a second hit is a
        # stale duplicate on C:. Patching one of two is a silent wrong date.
        print("  warning: %d copies of the date block found, using the first" % len(hits), file=sys.stderr)

    offset = hits[0] + dateblock.FIELD_OFFSET
    key["offset"] = offset
    json.dump(key, open(sidecar, "w"))
    print("  date field at byte %d" % offset, file=sys.stderr)
    return offset


def apply_date(data, start, when=None):
    """Rewrite whatever part of the date field this slice happens to hold.

    `when` exists so a test can pin the clock. Left alone it means now, which is
    the whole point: the guest reads this the moment the visitor resumes.
    """
    if PATCH_AT is None:
        return data
    lo = max(start, PATCH_AT)
    hi = min(start + len(data), PATCH_AT + dateblock.FIELD_LEN)
    if lo >= hi:
        return data
    field = dateblock.render(when)
    out = bytearray(data)
    out[lo - start:hi - start] = field[lo - PATCH_AT:hi - PATCH_AT]
    return bytes(out)


class Handler(SimpleHTTPRequestHandler):
    protocol_version = "HTTP/1.1"

    # Python's mimetypes does not know .mjs and calls it text/plain, and a
    # browser refuses to run a module script that is not served as JavaScript.
    # The page then stops at its first line with nothing in the console, which
    # looks like the emulator failing rather than the server mislabelling a file.
    extensions_map = {
        **SimpleHTTPRequestHandler.extensions_map,
        ".mjs": "text/javascript",
    }

    def do_GET(self):
        rng = self.headers.get("Range")
        if not rng:
            SimpleHTTPRequestHandler.do_GET(self)
            return

        path = self.translate_path(self.path)
        if not os.path.isfile(path):
            self.send_error(404)
            return
        size = os.path.getsize(path)

        m = RANGE_RE.match(rng.strip())
        if not m:
            self.send_response(416)
            self.send_header("Content-Range", "bytes */%d" % size)
            self.send_header("Content-Length", "0")
            self.end_headers()
            return

        start = int(m.group(1))
        end = int(m.group(2)) if m.group(2) else size - 1
        end = min(end, size - 1)
        if start > end:
            self.send_response(416)
            self.send_header("Content-Range", "bytes */%d" % size)
            self.send_header("Content-Length", "0")
            self.end_headers()
            return

        with open(path, "rb") as f:
            f.seek(start)
            data = f.read(end - start + 1)

        if os.path.basename(path) == IMAGE:
            data = apply_date(data, start)

        self.send_response(206)
        self.send_header("Content-Type", "application/octet-stream")
        self.send_header("Content-Range", "bytes %d-%d/%d" % (start, end, size))
        self.send_header("Content-Length", str(len(data)))
        self.send_header("Accept-Ranges", "bytes")
        self.end_headers()
        self.wfile.write(data)

    def end_headers(self):
        # The image is rewritten as it is served, so a cached copy is a stale
        # date. Everything else is small enough that no-store costs nothing.
        self.send_header("Cache-Control", "no-store")
        self.send_header("Accept-Ranges", "bytes")
        SimpleHTTPRequestHandler.end_headers(self)

    def log_message(self, fmt, *args):
        pass    # one line per disk block is thousands of lines per boot


class Server(ThreadingMixIn, HTTPServer):
    """Threaded: v86 has several block requests outstanding at once, and a
    single-threaded server turns that into a queue the guest waits on."""
    daemon_threads = True

    # HTTPServer sets this, and on Windows it does not mean what it means on
    # Unix: two processes can bind the same port and the connections go to
    # whichever one the stack feels like. A stale server from an earlier session
    # then answers half the requests, which looks like the new one hanging.
    # Refusing to start is far easier to understand.
    allow_reuse_address = False


def main():
    global IMAGE, PATCH_AT

    # capture.mjs asks for this offset so it can check that the capture did not
    # bake a stale date into the savestate. Running it here rather than
    # reimplementing the scan keeps one definition of where that field is.
    if len(sys.argv) > 2 and sys.argv[1] == "--scan":
        offset = find_date_field(sys.argv[2])
        if offset is None:
            raise SystemExit(1)
        print(offset)
        return

    port = int(sys.argv[1]) if len(sys.argv) > 1 else 8899
    root = sys.argv[2] if len(sys.argv) > 2 else "."
    IMAGE = sys.argv[3] if len(sys.argv) > 3 else IMAGE

    os.chdir(root)
    if os.path.exists(IMAGE):
        PATCH_AT = find_date_field(IMAGE)
    else:
        print("  no %s here; serving without a date patch" % IMAGE)

    print("serving %s on http://localhost:%d" % (os.getcwd(), port))
    print("  now: %s" % dateblock.render().decode())
    Server(("127.0.0.1", port), Handler).serve_forever()


if __name__ == "__main__":
    main()
