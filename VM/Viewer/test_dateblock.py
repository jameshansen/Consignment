"""Checks for the date block's layout.

The failure this guards against is silent. If the field is not exactly where
FIELD_OFFSET claims, the server still serves something, the guest still reads
something, and the only symptom is a demo stuck on the date the snapshot was
taken. Finding and splicing the field is covered by test_serve.py.

    python test_dateblock.py
"""

import datetime

import dateblock


def test_layout():
    b = dateblock.build()
    assert len(b) == dateblock.SIZE, len(b)
    assert b.startswith(dateblock.MAGIC)

    lines = b.split(b"\r\n")
    assert lines[0] == dateblock.MAGIC
    assert len(lines[1]) == dateblock.FIELD_LEN, lines[1]

    # The field has to sit exactly where FIELD_OFFSET claims, because that is all
    # the server has to go on.
    at = dateblock.FIELD_OFFSET
    assert b[at:at + dateblock.FIELD_LEN] == dateblock.render()
    print("ok  layout")


def test_size_keeps_the_file_out_of_the_mft():
    # Resident files live inside their MFT record, which shares a block with
    # unrelated metadata that boot activity pulls into the guest's file cache
    # long before this file is read. A cached block means every visitor sees the
    # capture date. NTFS stops storing data resident well below 1 KB.
    assert dateblock.SIZE >= 4096, dateblock.SIZE
    print("ok  size is large enough to get its own cluster")


def test_render_is_a_form_xp_accepts():
    when = datetime.datetime(2031, 12, 25, 13, 45, 59)
    out = dateblock.render(when)
    assert out == b"12-25-2031 13:45:59", out
    assert len(out) == dateblock.FIELD_LEN

    # The guest splits this on whitespace and hands the halves to date and time.
    stamp, clock = out.split(b" ")
    assert stamp.count(b"-") == 2 and clock.count(b":") == 2
    print("ok  render is MM-DD-YYYY HH:MM:SS")


def test_field_width_never_moves():
    # A different width would shift every byte after it, and the server writes a
    # fixed-length slice at a fixed offset.
    for when in (datetime.datetime(2001, 1, 1, 0, 0, 0),
                 datetime.datetime(2031, 12, 25, 23, 59, 59)):
        assert len(dateblock.render(when)) == dateblock.FIELD_LEN, when
    print("ok  field width is constant")


if __name__ == "__main__":
    test_layout()
    test_size_keeps_the_file_out_of_the_mft()
    test_render_is_a_form_xp_accepts()
    test_field_width_never_moves()
    print("all date block checks passed")
