<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

// ── Helper log ────────────────────────────────────────────────────────────
function _log(string $tag, string $msg): void {
    error_log("[TransferStok][$tag] $msg");
}

$raw  = file_get_contents('php://input');
$data = json_decode($raw, true) ?? [];

_log('INPUT', 'raw=' . substr($raw, 0, 500));

$tgl_transfer    = !empty($data['tgl_transfer'])
                        ? date('Y-m-d H:i:s', strtotime($data['tgl_transfer']))
                        : date('Y-m-d H:i:s');
$lokasi           = $data['lokasi']        ?? '';
$id_user          = $data['id_user']       ?? '';
$id_komputer      = $data['id_komputer']   ?? '';
$jenis_transfer   = $data['jenis_transfer'] ?? 'Transfer Stok';
$uraian           = $data['uraian']        ?? '';

$izinkan_backdate   = intval($data['izinkan_backdate']   ?? 0);
$izinkan_stok_minus = intval($data['izinkan_stok_minus'] ?? 0);

$id_barang_k   = $data['id_barang_k']   ?? '';
$nama_barang_k = $data['nama_barang_k'] ?? '';
$qty_k         = floatval($data['qty_k']         ?? 0);
$satuan_k      = $data['satuan_k']      ?? '';
$isi_k         = intval($data['isi_k']           ?? 1);
$qty_sat_k     = floatval($data['qty_sat_k']     ?? ($qty_k * $isi_k));
$harga_sat_k   = floatval($data['harga_sat_k']   ?? 0);
$total_harga_k = floatval($data['total_harga_k'] ?? ($qty_sat_k * $harga_sat_k));

$id_barang_m   = $data['id_barang_m']   ?? '';
$nama_barang_m = $data['nama_barang_m'] ?? '';
$qty_m         = floatval($data['qty_m']         ?? 0);
$satuan_m      = $data['satuan_m']      ?? '';
$isi_m         = intval($data['isi_m']           ?? 1);
$qty_sat_m     = floatval($data['qty_sat_m']     ?? ($qty_m * $isi_m));
$harga_sat_m   = floatval($data['harga_sat_m']   ?? 0);
$total_harga_m = floatval($data['total_harga_m'] ?? ($qty_sat_m * $harga_sat_m));

_log('PARAM', "lokasi=$lokasi | id_user=$id_user | tgl=$tgl_transfer");
_log('PARAM', "KELUAR: id=$id_barang_k | nama=$nama_barang_k | qty=$qty_k | sat=$satuan_k | isi=$isi_k | qty_sat=$qty_sat_k | harga=$harga_sat_k | total=$total_harga_k");
_log('PARAM', "MASUK : id=$id_barang_m | nama=$nama_barang_m | qty=$qty_m | sat=$satuan_m | isi=$isi_m | qty_sat=$qty_sat_m | harga=$harga_sat_m | total=$total_harga_m");
_log('PARAM', "izinkan_backdate=$izinkan_backdate | izinkan_stok_minus=$izinkan_stok_minus");

// Validasi field wajib
if (empty($lokasi) || empty($id_barang_k) || empty($id_barang_m)) {
    $missing = [];
    if (empty($lokasi))     $missing[] = 'lokasi';
    if (empty($id_barang_k)) $missing[] = 'id_barang_k';
    if (empty($id_barang_m)) $missing[] = 'id_barang_m';
    _log('VALIDASI', 'Field wajib kosong: ' . implode(', ', $missing));
    echo json_encode([
        'status'  => 'error',
        'error_code' => 'MISSING_FIELD',
        'message' => 'Field wajib kosong: ' . implode(', ', $missing),
    ]);
    exit;
}

try {
    _log('SP', 'SET user variables...');
    $conn->exec("SET
        @p_id_transfer          = '',
        @p_jenis_transfer       = " . $conn->quote($jenis_transfer)       . ",
        @p_uraian               = " . $conn->quote($uraian)               . ",
        @p_tgl_transfer         = " . $conn->quote($tgl_transfer)         . ",
        @p_id_barang_k          = " . $conn->quote($id_barang_k)          . ",
        @p_nama_barang_k        = " . $conn->quote($nama_barang_k)        . ",
        @p_qty_k                = " . floatval($qty_k)                    . ",
        @p_satuan_k             = " . $conn->quote($satuan_k)             . ",
        @p_isi_k                = " . intval($isi_k)                      . ",
        @p_qty_sat_k            = " . floatval($qty_sat_k)                . ",
        @p_harga_sat_k          = " . floatval($harga_sat_k)              . ",
        @p_total_harga_k        = " . floatval($total_harga_k)            . ",
        @p_id_barang_m          = " . $conn->quote($id_barang_m)          . ",
        @p_nama_barang_m        = " . $conn->quote($nama_barang_m)        . ",
        @p_qty_m                = " . floatval($qty_m)                    . ",
        @p_satuan_m             = " . $conn->quote($satuan_m)             . ",
        @p_isi_m                = " . intval($isi_m)                      . ",
        @p_qty_sat_m            = " . floatval($qty_sat_m)                . ",
        @p_harga_sat_m          = " . floatval($harga_sat_m)              . ",
        @p_total_harga_m        = " . floatval($total_harga_m)            . ",
        @p_lokasi               = " . $conn->quote($lokasi)               . ",
        @p_id_user              = " . $conn->quote($id_user)              . ",
        @p_id_komputer          = " . $conn->quote($id_komputer)          . ",
        @p_izinkan_backdate     = " . intval($izinkan_backdate)           . ",
        @p_izinkan_stok_minus   = " . intval($izinkan_stok_minus)
    );

    _log('SP', 'CALL sp_trx_transfer_stok_simpan...');
    $conn->exec("CALL sp_trx_transfer_stok_simpan(
        @p_id_transfer,
        @p_jenis_transfer, @p_uraian, @p_tgl_transfer,
        @p_id_barang_k, @p_nama_barang_k, @p_qty_k, @p_satuan_k, @p_isi_k, @p_qty_sat_k, @p_harga_sat_k, @p_total_harga_k,
        @p_id_barang_m, @p_nama_barang_m, @p_qty_m, @p_satuan_m, @p_isi_m, @p_qty_sat_m, @p_harga_sat_m, @p_total_harga_m,
        @p_lokasi, @p_id_user, @p_id_komputer,
        @p_izinkan_backdate, @p_izinkan_stok_minus,
        @p_success, @p_error_code, @p_error_message, @p_id_transfer_out
    )");

    $row = $conn->query("SELECT
        @p_success         AS success,
        @p_error_code      AS error_code,
        @p_error_message   AS error_message,
        @p_id_transfer_out AS id_transfer
    ")->fetch(PDO::FETCH_ASSOC);

    _log('SP', "OUT: success={$row['success']} | error_code={$row['error_code']} | message={$row['error_message']} | id_transfer={$row['id_transfer']}");

    if (intval($row['success']) === 1) {
        _log('OK', "id_transfer={$row['id_transfer']}");
        echo json_encode([
            'status'      => 'success',
            'id_transfer' => $row['id_transfer'],
        ]);
    } else {
        _log('FAIL', "error_code={$row['error_code']} | message={$row['error_message']}");
        echo json_encode([
            'status'     => 'error',
            'error_code' => $row['error_code']    ?? 'UNKNOWN',
            'message'    => $row['error_message'] ?? 'Terjadi kesalahan',
        ]);
    }

} catch (Exception $e) {
    _log('EXCEPTION', $e->getMessage() . ' | ' . $e->getTraceAsString());
    echo json_encode([
        'status'     => 'error',
        'error_code' => 'SQL_ERROR',
        'message'    => 'Gagal menyimpan transaksi: ' . $e->getMessage(),
    ]);
}
?>
