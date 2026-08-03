<?php
/*
 * Serve xp.img a block at a time, with today's date written into it on the way
 * past. This is the deployed half of the demo's Viewer/serve.py.
 *
 * The disk is never downloaded whole. v86 asks for the blocks XP actually
 * touches with HTTP range requests, so a 4 GB image costs the visitor a few tens
 * of megabytes.
 *
 * The date is the reason this is PHP and not a plain static file. A savestate
 * thaws with the guest clock frozen at capture time, and a resumed Windows kernel
 * never looks at the hardware clock again, so the date is delivered as bytes
 * instead: STARTDEMO.CMD reads C:\DATE.TXT just after the resume, and the offset
 * of that field inside the image is in xp.img.datepatch, written at build time
 * from the MFT. Patch the block as it goes past and the guest believes it is
 * today.
 *
 * Serve it as demo.html?img=disk.php, or hardcode IMAGE in a copy of the page.
 */

/* **Change this line if you are not on the west coast.** It is set unconditionally
 * and on purpose. PHP falls back to UTC when date.timezone is unset, and most
 * servers that do set it set it to UTC as well, which is seven or eight hours
 * ahead of here: the guest then shows tomorrow's date to anyone visiting after
 * late afternoon. Deferring to the server's setting was tried and gives exactly
 * that. The date the guest shows should be the date where the demo lives. */
date_default_timezone_set('America/Vancouver');

const IMAGE = __DIR__ . '/xp.img';
// The name build.py writes next to the image, so the sidecar can be copied up
// with the disk and this file is the same in Viewer/ and on the server.
const SIDECAR = __DIR__ . '/xp.img.datepatch';
// "MM-DD-YYYY HH:MM:SS", the forms XP's own date and time commands accept.
// Must stay in step with Viewer/dateblock.py, which is where the layout lives.
const FIELD_FMT = 'm-d-Y H:i:s';
const FIELD_LEN = 19;

function fail($code, $msg) {
    http_response_code($code);
    header('Content-Type: text/plain');
    exit($msg . "\n");
}

if (!is_file(IMAGE)) {
    fail(503, 'xp.img is not on this server yet.');
}
$size = filesize(IMAGE);

/* Where the date field sits. Written by build.py from the MFT, because scanning
 * the image for the block's magic finds more than one copy and the first is not
 * the file. A missing or stale sidecar is not fatal: the demo still runs, the
 * guest just shows the capture date, so say so in the log rather than 500. */
$patch_at = null;
if (is_file(SIDECAR)) {
    $side = json_decode(file_get_contents(SIDECAR), true);
    if (isset($side['offset'], $side['size']) && $side['size'] === $size) {
        $patch_at = $side['offset'];
    } else {
        error_log('consignment demo: xp.img.datepatch does not match xp.img; serving the capture date');
    }
}

$range = $_SERVER['HTTP_RANGE'] ?? '';
if ($range === '' || !preg_match('/^bytes=(\d+)-(\d*)$/', trim($range), $m)) {
    /* v86 only ever asks for ranges, and the whole file is 4 GB. Refusing the
     * unranged request keeps a stray click from trying to download all of it. */
    header('Accept-Ranges: bytes');
    header('Content-Range: bytes */' . $size);
    fail(416, 'This image is served in ranges only.');
}

$start = (int) $m[1];
$end = $m[2] === '' ? $size - 1 : (int) $m[2];
if ($end > $size - 1) $end = $size - 1;
if ($start > $end) {
    header('Content-Range: bytes */' . $size);
    fail(416, 'Range out of bounds.');
}

$len = $end - $start + 1;
$fh = fopen(IMAGE, 'rb');
fseek($fh, $start);
$data = stream_get_contents($fh, $len);
fclose($fh);

/* Rewrite whatever part of the date field this slice happens to hold. A block
 * boundary can land in the middle of it, so clip both ends rather than assuming
 * the field arrives whole. */
$dirty = false;
if ($patch_at !== null) {
    $lo = max($start, $patch_at);
    $hi = min($start + strlen($data), $patch_at + FIELD_LEN);
    if ($lo < $hi) {
        $field = date(FIELD_FMT);
        $data = substr_replace($data, substr($field, $lo - $patch_at, $hi - $lo), $lo - $start, $hi - $lo);
        $dirty = true;
    }
}

/* Only the one block carrying the date changes between requests. Everything else
 * on this disk is immutable, and it is the bulk of the traffic, so let the
 * browser keep it: a second visit costs almost nothing. Caching the date block
 * would freeze the clock, which is the bug this whole file exists to avoid. */
header($dirty ? 'Cache-Control: no-store' : 'Cache-Control: public, max-age=31536000, immutable');
header('Content-Type: application/octet-stream');
header('Content-Range: bytes ' . $start . '-' . $end . '/' . $size);
header('Content-Length: ' . strlen($data));
header('Accept-Ranges: bytes');
http_response_code(206);
echo $data;
