<?php
require_once __DIR__ . '/../db_connect.php';
require_once __DIR__ . '/../no_browser.php';

try {
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    echo "1. Set user variables..." . PHP_EOL;
    $conn->exec("SET
        @p_id_transfer       = '',
        @p_jenis_transfer    = 'Transfer Stok',
        @p_uraian            = 'Test transfer stok',
        @p_tgl_transfer      = NOW(),
        -- Barang K (Keluar)
        @p_id_barang_k       = 'RKK-0000941',
        @p_nama_barang_k     = 'A Mild 16 Pcs',
        @p_qty_k             = 1,
        @p_satuan_k          = 'Pcs',
        @p_isi_k             = 1,
        @p_qty_sat_k         = 1,
        @p_harga_sat_k       = 34350,
        @p_total_harga_k     = 34350,
        -- Barang M (Masuk)
        @p_id_barang_m       = 'RKK-0000941',
        @p_nama_barang_m     = 'A Mild 16 Pcs',
        @p_qty_m             = 10,
        @p_satuan_m          = 'Pcs',
        @p_isi_m             = 1,
        @p_qty_sat_m         = 10,
        @p_harga_sat_m       = 3435,
        @p_total_harga_m     = 34350,
        -- Lainnya
        @p_lokasi            = 'TOKO',
        @p_id_user           = 'ADMIN',
        @p_id_komputer       = 'PC-001',
        @p_izinkan_backdate  = 0,
        @p_izinkan_stok_minus = 1
    ");

    echo "2. Calling sp_trx_transfer_stok_simpan..." . PHP_EOL;
    $conn->exec("CALL sp_trx_transfer_stok_simpan(
        @p_id_transfer, @p_jenis_transfer, @p_uraian, @p_tgl_transfer,
        @p_id_barang_k, @p_nama_barang_k, @p_qty_k, @p_satuan_k, @p_isi_k, @p_qty_sat_k, @p_harga_sat_k, @p_total_harga_k,
        @p_id_barang_m, @p_nama_barang_m, @p_qty_m, @p_satuan_m, @p_isi_m, @p_qty_sat_m, @p_harga_sat_m, @p_total_harga_m,
        @p_lokasi, @p_id_user, @p_id_komputer,
        @p_izinkan_backdate, @p_izinkan_stok_minus,
        @p_success, @p_error_code, @p_error_message, @p_id_transfer_out
    )");

    $row = $conn->query("SELECT
        @p_success        AS success,
        @p_error_code     AS error_code,
        @p_error_message  AS error_message,
        @p_id_transfer_out AS id_transfer
    ")->fetch(PDO::FETCH_ASSOC);

    echo PHP_EOL . "=== SUCCESS! ===" . PHP_EOL;
    echo json_encode($row, JSON_PRETTY_PRINT) . PHP_EOL;

} catch (Exception $e) {
    echo "Error: " . $e->getMessage() . PHP_EOL;
    echo "Trace: " . $e->getTraceAsString() . PHP_EOL;
}
?>
