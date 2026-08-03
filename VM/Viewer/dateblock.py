"""Layout of C:\\DATE.TXT, the block the server rewrites with the current date.

A savestate thaws with the guest clock frozen at whatever it read when the
snapshot was taken, and a resumed Windows kernel never looks at the hardware clock
again, so nothing the emulator does to its RTC will help. The date is delivered as
bytes instead: STARTDEMO.CMD reads this file just after the resume, and serve.py
rewrites the date field as those bytes go past.

Three programs have to agree about where that field sits, so the layout lives
here rather than in any of them.

    line 1   MAGIC, so the block can be found by scanning the raw image
    line 2   the date field, fixed width, what the server rewrites
    rest     padding

The size is not arbitrary. At 4 KB NTFS stores the contents in their own cluster
instead of resident inside the MFT record, and that matters: an MFT record shares
a block with unrelated metadata that boot activity would have pulled into the
guest's file cache long before the file is read. The cache is part of the RAM
image the savestate restores, so a cached block means every visitor sees the
capture date for ever.
"""

import datetime

MAGIC = b"CONSIGNMENT-DEMO-DATE-BLOCK-V1"
SIZE = 4096
EOL = b"\r\n"

# "MM-DD-YYYY HH:MM:SS", the forms XP's own date and time commands accept.
FIELD_FMT = "%m-%d-%Y %H:%M:%S"
FIELD_LEN = 19
FIELD_OFFSET = len(MAGIC) + len(EOL)


def render(when=None):
    """The date field exactly as it appears on disk."""
    when = when or datetime.datetime.now()
    out = when.strftime(FIELD_FMT).encode("ascii")
    if len(out) != FIELD_LEN:
        raise ValueError("date field is %d bytes, expected %d" % (len(out), FIELD_LEN))
    return out


def build(when=None):
    """The whole 4 KB file."""
    body = MAGIC + EOL + render(when) + EOL
    return body + b" " * (SIZE - len(body))
