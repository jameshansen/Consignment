"""Check the merged install ISO: boot media intact, payload readable.

Appending 140 MB to a Microsoft install disc is the step most able to fail
quietly. A broken directory record costs a multi-hour install run before anything
complains, so this reads the finished image back the way the guest will.

    python test_install_iso.py [XPDrives/xpsetup.iso]
"""

import os
import struct
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
import iso_add


def read_file(img, vol, isopath):
    """Resolve a path through the directory tree and return its bytes."""
    parent, name = isopath.rsplit("/", 1)
    node = img.getdir(vol, parent or "/")
    want = vol.encode(name).upper()
    for rec in node.records:
        if not rec.is_dir and rec.name.upper() == want:
            start = rec.lba * iso_add.SECTOR
            return bytes(img.buf[start:start + rec.length])
    raise KeyError(isopath)


def main():
    iso = sys.argv[1] if len(sys.argv) > 1 else \
        os.path.join(HERE, "XPDrives", "xpsetup.iso")
    src = os.path.join(HERE, "XPSetupFiles")

    img = iso_add.Image(iso)
    # This media carries no Joliet tree, so the primary one is what both
    # SETUPLDR and, later, XP's CDFS read. Names there are uppercased and keep
    # their spaces and second extension ("MULTI EXPRESS CONSIGNMENT.EXE.CONFIG"),
    # which is outside ISO9660 but which CDFS passes through untouched. Windows
    # being case insensitive is what makes the uppercase harmless.
    assert img.joliet is None, \
        "media grew a Joliet tree; the name assumptions below no longer hold"
    tree = img.primary

    # El Torito. The boot record sits at sector 17 and points at the catalogue;
    # if appending moved either, the disc no longer boots.
    br = bytes(img.buf[17 * iso_add.SECTOR:17 * iso_add.SECTOR + 2048])
    assert br[0] == 0 and br[1:6] == b"CD001", "no boot record at sector 17"
    assert br[7:30].rstrip(b"\0") == b"EL TORITO SPECIFICATION", \
        "boot record is not El Torito: %r" % br[7:30]
    cat_lba = struct.unpack_from("<I", br, 71)[0]
    cat = bytes(img.buf[cat_lba * iso_add.SECTOR:cat_lba * iso_add.SECTOR + 64])
    assert cat[0] == 1 and cat[30:32] == b"\x55\xaa", "boot catalogue is corrupt"
    assert cat[32] == 0x88, "boot entry is not marked bootable"
    boot_lba = struct.unpack_from("<I", cat, 40)[0]
    assert boot_lba * iso_add.SECTOR < os.path.getsize(iso), "boot image past EOF"
    print("  el torito ok: catalogue lba %d, boot image lba %d" % (cat_lba, boot_lba))

    # The answer file, which is what makes the install unattended, and the one
    # line in it that decides whether the result boots under v86 at all.
    sif = read_file(img, tree, "/I386/WINNT.SIF").decode("latin1")
    assert "UnattendMode = FullUnattended" in sif, "answer file is not unattended"
    assert 'ComputerType = "Standard PC", Retail' in sif, \
        "answer file does not force the Standard PC HAL"
    assert "@PRODUCTKEY@" not in sif, "product key placeholder was not substituted"
    print("  WINNT.SIF ok: FullUnattended, Standard PC HAL, key substituted")

    # The payload, byte for byte against what stage_setupcd.py wrote.
    for name in ("INSTALL.CMD", "STARTDEMO.CMD", "SHELL.VBS"):
        on_iso = read_file(img, tree, "/SETUP/" + name)
        on_disk = open(os.path.join(src, name), "rb").read()
        assert on_iso == on_disk, "%s differs from XPSetupFiles" % name
    print("  /SETUP scripts ok: identical to XPSetupFiles")

    # GuiRunOnce runs this, and the guest signals the host through it.
    install = read_file(img, tree, "/SETUP/INSTALL.CMD").decode("latin1")
    assert "INSTALL-COMPLETE" in install, "INSTALL.CMD has no completion marker"
    assert ">COM1" in install, "INSTALL.CMD does not report on the serial port"
    startdemo = read_file(img, tree, "/SETUP/STARTDEMO.CMD").decode("latin1")
    assert "DEMO-READY>COM1" in startdemo, "STARTDEMO.CMD has no capture marker"
    print("  serial markers ok: INSTALL-COMPLETE, DEMO-READY")

    # The trees the install actually walks. MYSQL holds only subdirectories at
    # its top level, so count entries rather than files.
    for d, want in (("/SETUP/APP", 20), ("/SETUP/MYSQL", 1), ("/SETUP/SQL", 2)):
        node = img.getdir(tree, d)
        entries = [r for r in node.records if r.name not in (b"\x00", b"\x01")]
        assert len(entries) >= want, \
            "%s has %d entries, expected at least %d" % (d, len(entries), want)
        print("  %-14s %d entries" % (d, len(entries)))

    # mysqld is the one binary the install cannot proceed without, and it is
    # three levels down, so this also proves the nested directories resolve.
    mysqld = read_file(img, tree, "/SETUP/MYSQL/bin/mysqld.exe")
    assert mysqld[:2] == b"MZ", "mysqld.exe on the disc is not a PE image"
    print("  mysqld ok: %d bytes" % len(mysqld))

    exe = read_file(img, tree, "/SETUP/APP/Multi Express Consignment.exe")
    assert exe[:2] == b"MZ", "the program on the disc is not a PE image"
    print("  program ok: %d bytes, MZ header intact" % len(exe))

    # The display driver, which setup installs itself. Without this the guest is
    # stuck at 640x480 and the [Display] section above is decoration.
    assert "OemPreinstall = Yes" in sif, "answer file does not read $OEM$"
    assert 'OemPnPDriversPath = "drivers\\video"' in sif, \
        "answer file does not point at the staged driver"
    sysfile = read_file(img, tree, "/$OEM$/$1/drivers/video/vbemp.sys")
    assert sysfile[:2] == b"MZ", "vbemp.sys on the disc is not a PE image"
    inf = read_file(img, tree, "/$OEM$/$1/drivers/video/vbemppnp.inf").decode("latin1")
    # v86 presents its VGA as a PCI display controller, class 0300.
    assert "PCI\\CC_0300" in inf, "the driver INF does not match a PCI VGA device"
    print("  display driver ok: vbemp.sys %d bytes, INF matches PCI\\CC_0300"
          % len(sysfile))

    print("PASS")


if __name__ == "__main__":
    main()
