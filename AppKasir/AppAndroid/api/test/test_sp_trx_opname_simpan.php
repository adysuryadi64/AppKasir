<?php
require_once __DIR__ . '/../db_connect.php';
require_once __DIR__ . '/../no_browser.php';

try {
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    echo "1. Create tmp_stokopname_items..." . PHP_EOL;
    $conn->exec("CREATE TEMPORARY TABLE IF NOT EXISTS tmp_stokopname_items (
        ID_BARANG VARCHAR(50),
        NAMA_BARANG VARCHAR(100),
        KATEGORI VARCHAR(50),
        HARGA DECIMAL(10,2),
        STOK_SYSTEM DECIMAL(10,2),
        STOK_NYATA DECIMAL(10,2),
        SATUAN VARCHAR(20),
        ISI_SATUAN SMALLINT,
        TOTAL_QTY DECIMAL(10,2),
        TOTAL_HARGA DECIMAL(15,0),
        KETERANGAN VARCHAR(255)
    )");
    $conn->exec("DELETE FROM tmp_stokopname_items");

    echo "2. Insert item ke tmp..." . PHP_EOL;
    $stmt_item = $conn->prepare("INSERT INTO tmp_stokopname_items VALUES (
        'RKK-0000941', 'A Mild 16 Pcs', 'Rokok', 34350, 10, 12, 'Pcs', 1, 12, 34350*12, 'Test opname'
    )");
    $stmt_item->execute();

    echo "3. Set user variables..." . PHP_EOL;
    $conn->exec("SET
        @p_id_opname         = '',
        @p_lokasi            = 'TOKO',
        @p_tgl_transaksi     = NOW(),
        @p_keterangan        = 'Test opname dari API',
        @p_id_user           = 'ADMIN',
        @p_id_komputer       = 'PC-001',
        @p_izinkan_backdate  = 0
    ");

    echo "4. Calling sp_trx_opname_simpan..." . PHP_EOL;
    $conn->exec("CALL sp_trx_opname_simpan(
        @p_id_opname, @p_lokasi, @p_tgl_transaksi, @p_keterangan,
        @p_id_user, @p_id_komputer, @p_izinkan_backdate,
        @p_success, @p_error_code, @p_error_message, @p_id_opname_out
    )");

    $row = $conn->query("SELECT
        @p_success        AS success,
        @p_error_code     AS error_code,
        @p_error_message  AS error_message,
        @p_id_opname_out  AS id_opname
    ")->fetch(PDO::FETCH_ASSOC);

    $conn->exec("DROP TEMPORARY TABLE IF EXISTS tmp_stokopname_items");

    echo PHP_EOL . "=== SUCCESS! ===" . PHP_EOL;
    echo json_encode($row, JSON_PRETTY_PRINT) . PHP_EOL;

} catch (Exception $e) {
    try { $conn->exec("DROP TEMPORARY TABLE IF EXISTS tmp_stokopname_items"); } catch (Exception $_) {}
    echo "Error: " . $e->getMessage() . PHP_EOL;
    echo "Trace: " . $e->getTraceAsString() . PHP_EOL;
}
?>
