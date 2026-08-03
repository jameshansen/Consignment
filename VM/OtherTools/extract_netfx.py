"""Pull the genuine .NET Framework 4.0 x86 files out of netfx_Core.mzz.

The v4.0.30319 directory on a modern Windows box holds .NET 4.8, not 4.0: 4.5
replaced 4.0 in place and dropped XP support, so those binaries cannot run in
the guest. The runtime has to come from the 4.0 redistributable instead.

Get dotNetFx40_Full_x86_x64.exe from Microsoft, then:

    7z x dotNetFx40_Full_x86_x64.exe -onetfx

netfx_Core.mzz inside it is a plain MS cabinet despite the extension, so 7-Zip
reads it directly. The files are flattened and suffixed _x86 / _amd64, with
inconsistent separators, so the mapping below is explicit rather than derived.

What comes out is 34 MB, not the 190 MB the installed framework occupies,
because none of it is NGEN native images.

Usage:
    python extract_netfx.py <path to netfx_Core.mzz> <payload dir>/DOTNET
"""
import os, shutil, subprocess, sys

SEVENZIP = r"C:\Program Files\7-Zip\7z.exe"
CAB = sys.argv[1] if len(sys.argv) > 1 else None
OUT = sys.argv[2] if len(sys.argv) > 2 else None
STAGE = os.path.join(os.path.dirname(OUT or "."), "_netfx_stage")

# cabinet name -> destination, relative to OUT
FRAMEWORK = "Framework/v4.0.30319"
FILES = {
    # the runtime itself
    "clr_dll_x86":                FRAMEWORK + "/clr.dll",
    "clrjit_dll_x86":             FRAMEWORK + "/clrjit.dll",
    "mscorlib_dll_x86":           FRAMEWORK + "/mscorlib.dll",
    "mscoreei_dll_x86":           FRAMEWORK + "/mscoreei.dll",
    "mscoreeis_dll_x86":          FRAMEWORK + "/mscoreeis.dll",
    "mscorrc_dll_x86":            FRAMEWORK + "/mscorrc.dll",
    "mscorier_dll_x86":           FRAMEWORK + "/mscorier.dll",
    "mscorsecr_dll_x86":          FRAMEWORK + "/mscorsecr.dll",
    "mscorsecimpl_dll_x86":       FRAMEWORK + "/mscorsecimpl.dll",
    "msvcr_clr_dll_x86":          FRAMEWORK + "/msvcr100_clr0400.dll",
    "nlssorting_dll_x86":         FRAMEWORK + "/nlssorting.dll",
    "normalization_dll_x86":      FRAMEWORK + "/normalization.dll",
    "culture_dll_x86":            FRAMEWORK + "/culture.dll",
    "diasymreader_dll_x86":       FRAMEWORK + "/diasymreader.dll",
    "mscordacwks_dll_x86":        FRAMEWORK + "/mscordacwks.dll",
    "sbscmp20_mscorwks_dll_x86":  FRAMEWORK + "/sbscmp20_mscorwks.dll",
    "machine_config_x86":         FRAMEWORK + "/Config/machine.config",

    # the shim, which the OS loader resolves from the application directory
    "_003_mscoree_dll_x86":       "app/mscoree.dll",

    # base class libraries. Without a GAC the CLR probes the application
    # directory, so these ship next to the executable.
    "system_dll_x86":                       "app/System.dll",
    "system.core.dll_x86":                  "app/System.Core.dll",
    "system_data_dll_x86":                  "app/System.Data.dll",
    "system_drawing_dll_x86":               "app/System.Drawing.dll",
    "system_windows_forms_dll_x86":         "app/System.Windows.Forms.dll",
    "system_xml_dll_x86":                   "app/System.Xml.dll",
    "system_configuration_dll_x86":         "app/System.Configuration.dll",
    "system_deployment_dll_x86":            "app/System.Deployment.dll",
    "system_web_services_dll_x86":          "app/System.Web.Services.dll",
    "system_transactions_dll_x86":          "app/System.Transactions.dll",
    "system_numerics_dll_x86":              "app/System.Numerics.dll",
    "system_enterpriseservices_dll_x86":    "app/System.EnterpriseServices.dll",
    "system.xml.linq.dll_x86":              "app/System.Xml.Linq.dll",
    "system.data.datasetextensions.dll_x86":"app/System.Data.DataSetExtensions.dll",
    "microsoft.csharp.dll_x86":             "app/Microsoft.CSharp.dll",
    "accessibility_dll_x86":                "app/Accessibility.dll",
}


def main():
    if not CAB or not OUT:
        raise SystemExit(__doc__)
    if os.path.isdir(STAGE):
        shutil.rmtree(STAGE)
    os.makedirs(STAGE)

    cmd = [SEVENZIP, "e", CAB, "-o" + STAGE, "-y"] + list(FILES)
    r = subprocess.run(cmd, capture_output=True, text=True)
    if r.returncode != 0:
        sys.exit("7z failed:\n" + r.stdout[-2000:] + r.stderr[-2000:])

    missing, total = [], 0
    for src, dst in FILES.items():
        s = os.path.join(STAGE, src)
        if not os.path.exists(s):
            missing.append(src)
            continue
        d = os.path.join(OUT, dst.replace("/", os.sep))
        os.makedirs(os.path.dirname(d), exist_ok=True)
        shutil.copy2(s, d)
        total += os.path.getsize(d)

    if missing:
        print("MISSING from cabinet: " + ", ".join(missing))
    print("wrote %d files, %.1f MB to %s" % (len(FILES) - len(missing), total / 1048576.0, OUT))


if __name__ == "__main__":
    main()
