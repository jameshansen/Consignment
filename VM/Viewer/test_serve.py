"""Check the range server: correct slices, and the date spliced across boundaries.

The splice is the part that can be quietly wrong. v86 asks for whatever range it
feels like, so the date field can land wholly inside a slice, half in it, or one
byte into it, and a slice that is off by a character hands the guest a date
command it will sit at a prompt over.

    python test_serve.py
"""

import datetime
import os
import shutil
import sys
import tempfile
import threading
import urllib.request

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
import dateblock
import serve

MAGIC_AT = 100000       # where the fake image puts the date block
SIZE = 300000


def build_image(path):
    data = bytearray(b"\xEE" * SIZE)
    block = dateblock.build()
    data[MAGIC_AT:MAGIC_AT + len(block)] = block
    open(path, "wb").write(bytes(data))
    return MAGIC_AT + dateblock.FIELD_OFFSET


def get(port, path, start, end):
    req = urllib.request.Request("http://127.0.0.1:%d/%s" % (port, path))
    req.add_header("Range", "bytes=%d-%d" % (start, end))
    with urllib.request.urlopen(req) as r:
        return r.status, r.headers.get("Content-Range"), r.read()


def main():
    tmp = tempfile.mkdtemp()
    try:
        img = os.path.join(tmp, "xp.img")
        field_at = build_image(img)

        # The scan finds the block, and caches the answer.
        found = serve.find_date_field(img)
        assert found == field_at, "scan found %s, expected %d" % (found, field_at)
        assert os.path.exists(img + ".datepatch"), "no sidecar written"
        assert serve.find_date_field(img) == field_at, "cached scan disagrees"
        print("  scan ok: date field at %d, cached" % field_at)

        serve.PATCH_AT = field_at
        # Pinned, so a second ticking over between rendering the expected value
        # and rendering the served one cannot fail the run.
        WHEN = datetime.datetime(2031, 12, 25, 13, 45, 59)
        today = dateblock.render(WHEN)

        # Wholly inside, straddling each end, one byte of overlap at each end,
        # and clear of it entirely.
        n = dateblock.FIELD_LEN
        cases = {
            "inside":       (field_at - 10, field_at + n + 10),
            "straddle-lo":  (field_at + 5, field_at + n + 50),
            "straddle-hi":  (field_at - 50, field_at + 5),
            "one-byte-lo":  (field_at + n - 1, field_at + n + 20),
            "one-byte-hi":  (field_at - 20, field_at),
            "clear":        (field_at + n, field_at + n + 40),
        }
        for name, (start, end) in cases.items():
            raw = open(img, "rb").read()[start:end + 1]
            out = serve.apply_date(raw, start, WHEN)
            assert len(out) == len(raw), "%s changed the slice length" % name
            # Every byte of the field that this slice covers must be today's.
            lo, hi = max(start, field_at), min(end + 1, field_at + n)
            if lo < hi:
                got = out[lo - start:hi - start]
                assert got == today[lo - field_at:hi - field_at], \
                    "%s spliced wrong: %r" % (name, got)
            # and nothing outside it may move.
            for i in range(len(raw)):
                at = start + i
                if not (field_at <= at < field_at + n):
                    assert out[i] == raw[i], "%s changed byte at %d" % (name, at)
            print("  splice ok: %s" % name)

        # And over the wire, which is where the status code and Content-Range
        # have to be right or v86 aborts the load.
        serve.IMAGE = "xp.img"
        os.chdir(tmp)
        srv = serve.Server(("127.0.0.1", 0), serve.Handler)
        port = srv.server_address[1]
        threading.Thread(target=srv.serve_forever, daemon=True).start()
        try:
            status, crange, body = get(port, "xp.img", field_at - 4, field_at + n + 4)
            assert status == 206, "expected 206, got %d" % status
            assert crange == "bytes %d-%d/%d" % (field_at - 4, field_at + n + 4, SIZE), \
                "bad Content-Range: %s" % crange
            # This one goes through the live server, which stamps the real
            # clock, so check the shape and that it is actually now.
            served = body[4:4 + n].decode("ascii")
            stamped = datetime.datetime.strptime(served, dateblock.FIELD_FMT)
            drift = abs((datetime.datetime.now() - stamped).total_seconds())
            assert drift < 120, "served date is %s, %.0fs from now" % (served, drift)
            assert len(body) == n + 9, "served %d bytes, expected %d" % (len(body), n + 9)
            print("  http ok: 206, %s" % crange)

            # v86 asks the file size this way before it reads anything.
            status, crange, body = get(port, "xp.img", 0, 0)
            assert status == 206 and crange.endswith("/%d" % SIZE), \
                "size probe returned %s" % crange
            assert len(body) == 1
            print("  http ok: size probe reports %d" % SIZE)
        finally:
            srv.shutdown()

        print("PASS")
    finally:
        os.chdir(HERE)
        shutil.rmtree(tmp, ignore_errors=True)


if __name__ == "__main__":
    main()
