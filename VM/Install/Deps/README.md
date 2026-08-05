# Deps

Everything the install needs that is not in this repository. None of it is
committed: some of it is third-party redistributable that should not be, and the
Windows media is yours to supply.

Put these here before running `python ../../build.py`.

| | |
|:--- |:--- |
| `en_windows_xp_professional_with_service_pack_3_x86*.iso` | Windows XP SP3 install media |
| `dotNetFx40_Full_x86_x64.exe` | .NET Framework 4.0 full offline installer |
| `CRRuntime_32bit_13_0.msi` | SAP Crystal Reports runtime, 32-bit |
| `MYSQL/` | an unpacked MySQL server, with `bin/mysqld.exe` and `bin/mysql.exe` |
| `3of9.ttf` | the barcode font |
| `vbempk.zip` | the VBE Miniport display driver, from <https://bearwindows.zcm.com.au/vbemp.htm> |

`vbempk.zip` is taken as the zip, not unpacked. `stage_setupcd.py` pulls
`VBE30/XP/PNP/vbemp.sys` and `vbemppnp.inf` out of it into the `$OEM$` tree, and
setup installs the driver itself. Without it the guest is stuck at 640x480,
because XP ships no display driver that can do more on this hardware.

That driver is **free for noncommercial use only**. Check that against whatever you
are demoing.

`stage_setupcd.py` checks for each of these and stops with the missing path rather
than building a broken disc.

The schema and the demo data are not listed here. `stage_setupcd.py` takes
`consignment_db_structure.sql` and `demo_database.sql` from the repository root, so
the guest and the host test database load the same files.

## Use volume licence XP media

`build_install_iso.py` prefers any ISO whose filename contains `_vl_`, and you want
it to find one.

Retail media forces Windows Product Activation, which this demo cannot survive. The
guest has no network, Microsoft's XP activation servers are retired, and the clock is
moved to the visitor's real date on every resume, which expires the grace period. The
result is a logon loop. A volume licence key never activates, so the problem does not
arise.

A key only validates against media from its own channel, so a volume key on retail
media gets "The CD Key you entered is not valid" from setup. Match the key to the
media, and put the key in `../xpkey.txt` or the `XPKEY` environment variable.

## MySQL

Any 5.x server works. It is configured by `../XPSetupFiles/my.ini` to listen on a
named pipe only, because the guest has no network device, and `settings.ini` passes
the matching connection string to Connector/Net.

Take the server as an unpacked directory rather than an installer, since the install
script copies it to `C:\MYSQL` and registers the service by hand:

```bat
C:\MYSQL\bin\mysqld.exe --install MySQL --defaults-file=C:\MYSQL\my.ini
```
