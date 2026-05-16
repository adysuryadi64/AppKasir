<?php
require_once __DIR__ . '/../db_connect.php';
require_once __DIR__ . '/../no_browser.php';

$testData = [
    'tgl_transfer'     => date('Y-m-d H:i:s'),
    'lokasi'           => 'TOKO',
    'id_user'          => 'ADMIN',
    'id_komputer'      => 'PC-001',
    'jenis_transfer'   => 'Transfer Stok',
    'uraian'           => 'Test transfer',
    'izinkan_backdate' => 1,
    'izinkan_stok_minus' => 1,
    'id_barang_k'      => 'RKK-0000941',
    'nama_barang_k'    => 'A Mild 16 Pcs',
    'qty_k'            => 1,
    'satuan_k'         => 'Pcs',
    'isi_k'            => 1,
    'qty_sat_k'        => 1,
    'harga_sat_k'      => 34350,
    'total_harga_k'    => 34350,
    'id_barang_m'      => 'RKK-0000941',
    'nama_barang_m'    => 'A Mild 16 Pcs',
    'qty_m'            => 10,
    'satuan_m'         => 'Pcs',
    'isi_m'            => 1,
    'qty_sat_m'        => 10,
    'harga_sat_m'      => 3435,
    'total_harga_m'    => 34350,
];

echo "1. Testing sync_transfer_stok.php with payload:\n";
echo json_encode($testData, JSON_PRETTY_PRINT) . "\n";
echo "2. Simulating HTTP POST request...\n";

try {
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $data = $testData;
    $tgl_transfer    = !empty($data['tgl_transfer'])
                            ? date('Y-m-d H:i:s', strtotime($data['tgl_transfer']))
                            : date('Y-m-d H:i:s');
    $lokasi           = $data['lokasi'];
    $id_user          = $data['id_user'];
    $id_komputer      = $data['id_komputer'];
    $jenis_transfer   = $data['jenis_transfer'] ?? 'Transfer Stok';
    $uraian           = $data['uraian'] ?? '';
    $izinkan_backdate   = intval($data['izinkan_backdate'] ?? 0);
    $izinkan_stok_minus = intval($data['izinkan_stok_minus'] ?? 0);
    $id_barang_k      = $data['id_barang_k'];
    $nama_barang_k    = $data['nama_barang_k'];
    $qty_k            = floatval($data['qty_k'] ?? 0);
    $satuan_k         = $data['satuan_k'] ?? '';
    $isi_k            = intval($data['isi_k'] ?? 1);
    $qty_sat_k        = floatval($data['qty_sat_k'] ?? ($qty_k * $isi_k));
    $harga_sat_k      = floatval($data['harga_sat_k'] ?? 0);
    $total_harga_k    = floatval($data['total_harga_k'] ?? ($qty_sat_k * $harga_sat_k));
    $id_barang_m      = $data['id_barang_m'];
    $nama_barang_m    = $data['nama_barang_m'];
    $qty_m            = floatval($data['qty_m'] ?? 0);
    $satuan_m         = $data['satuan_m'] ?? '';
    $isi_m            = intval($data['isi_m'] ?? 1);
    $qty_sat_m        = floatval($data['qty_sat_m'] ?? ($qty_m * $isi_m));
    $harga_sat_m      = floatval($data['harga_sat_m'] ?? 0);
    $total_harga_m    = floatval($data['total_harga_m'] ?? ($qty_sat_m * $harga_sat_m));

    $conn->exec("SET
        @p_id_transfer          = '',
        @p_jenis_transfer       = " . $conn->quote($jenis_transfer)       . ",
        @p_uraian               = " . $conn->quote($uraian)               . ",
        @p_tgl_transfer         = " . $conn->quote($tgl_transfer)         . ",
        -- Barang K (Keluar)
        @p_id_barang_k          = " . $conn->quote($id_barang_k)          . ",
        @p_nama_barang_k        = " . $conn->quote($nama_barang_k)        . ",
        @p_qty_k                = " . floatval($qty_k)                . ",
        @p_satuan_k             = " . $conn->quote($satuan_k)             . ",
        @p_isi_k                = " . intval($isi_k)                . ",
        @p_qty_sat_k            = " . floatval($qty_sat_k)            . ",
        @p_harga_sat_k          = " . floatval($harga_sat_k)          . ",
        @p_total_harga_k        = " . floatval($total_harga_k)        . ",
        -- Barang M (Masuk)
        @p_id_barang_m          = " . $conn->quote($id_barang_m)          . ",
        @p_nama_barang_m        = " . $conn->quote($nama_barang_m)        . ",
        @p_qty_m                = " . floatval($qty_m)                . ",
        @p_satuan_m             = " . $conn->quote($satuan_m)             . ",
        @p_isi_m                = " . intval($isi_m)                . ",
        @p_qty_sat_m            = " . floatval($qty_sat_m)            . ",
        @p_harga_sat_m          = " . floatval($harga_sat_m)          . ",
        @p_total_harga_m        = " . floatval($total_harga_m)        . ",
        -- Lainnya
        @p_lokasi               = " . $conn->quote($lokasi)               . ",
        @p_id_user              = " . $conn->quote($id_user)              . ",
        @p_id_komputer          = " . $conn->quote($id_komputer)          . ",
        @p_izinkan_backdate     = " . intval($izinkan_backdate)     . ",
        @p_izinkan_stok_minus   = " . intval($izinkan_stok_minus)
    );

    $conn->exec("CALL sp_trx_transfer_stok_simpan(
        @p_id_transfer,
        @p_jenis_transfer, @p_uraian, @p_tgl_transfer,
        -- Barang K (Keluar)
        @p_id_barang_k, @p_nama_barang_k, @p_qty_k, @p_satuan_k, @p_isi_k, @p_qty_sat_k, @p_harga_sat_k, @p_total_harga_k,
        -- Barang M (Masuk)
        @p_id_barang_m, @p_nama_barang_m, @p_qty_m, @p_satuan_m, @p_isi_m, @p_qty_sat_m, @p_harga_sat_m, @p_total_harga_m,
        -- Lainnya
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

    echo "\n=== RESULT ===" . "\n";
    if (intval($row['success']) === 1) {
        $response = [
            'status'        => 'success',
            'id_transfer'   => $row['id_transfer'],
        ];
    } else {
        $response = [
            'status'        => 'error',
            'error_code'    => $row['error_code']    ?? 'UNKNOWN',
            'message'       => $row['error_message'] ?? 'Terjadi kesalahan',
        ];
    }
    echo json_encode($response, JSON_PRETTY_PRINT) . "\n";
} catch (Exception $e) {
    echo "\n=== ERROR ===" . "\n";
    echo "Error: " . $e->getMessage() . "\n";
    echo "Trace: " . $e->getTraceAsString() . "\n";
}
?>
