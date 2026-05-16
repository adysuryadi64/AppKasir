<?php
require_once __DIR__ . '/../db_connect.php';
require_once __DIR__ . '/../no_browser.php';

$testData = [
    'tgl_transaksi'    => date('Y-m-d H:i:s'),
    'lokasi'           => 'TOKO',
    'id_user'          => 'ADMIN',
    'id_komputer'      => 'PC-001',
    'keterangan'       => 'Test opname',
    'izinkan_backdate' => 1,
    'items'            => [
        [
            'id_barang'     => 'RKK-0000941',
            'nama_barang'   => 'A Mild 16 Pcs',
            'kategori'      => 'Rokok',
            'merk'          => 'Mild',
            'harga'         => 34350,
            'stok_system'   => -3,
            'stok_nyata'    => 5,
            'stok_selisih'  => 8,
            'satuan'        => 'Pcs',
            'isi_satuan'    => 1,
            'total_qty'     => 5,
            'total_harga'   => 8 * 34350,
            'keterangan'    => 'Test item',
        ],
    ],
];

echo "1. Testing sync_stokopname.php with payload:\n";
echo json_encode($testData, JSON_PRETTY_PRINT) . "\n";
echo "2. Simulating HTTP POST request...\n";

try {
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $data = $testData;
    $tgl_transaksi    = !empty($data['tgl_transaksi'])
                            ? date('Y-m-d H:i:s', strtotime($data['tgl_transaksi']))
                            : date('Y-m-d H:i:s');
    $lokasi           = $data['lokasi'];
    $id_user          = $data['id_user'];
    $id_komputer      = $data['id_komputer'];
    $keterangan       = $data['keterangan'] ?? '';
    $izinkan_backdate = intval($data['izinkan_backdate'] ?? 0);

    $conn->exec("CREATE TEMPORARY TABLE IF NOT EXISTS tmp_stokopname_items (
        ID_BARANG        VARCHAR(50),
        NAMA_BARANG      VARCHAR(200),
        KATEGORI         VARCHAR(50),
        HARGA            DECIMAL(10,2),
        STOK_SYSTEM      DECIMAL(10,2),
        STOK_NYATA       DECIMAL(10,2),
        SATUAN           VARCHAR(20),
        ISI_SATUAN       SMALLINT,
        TOTAL_QTY        DECIMAL(10,2),
        TOTAL_HARGA      DECIMAL(15,0),
        KETERANGAN       VARCHAR(255)
    )");

    $conn->exec("DELETE FROM tmp_stokopname_items");

    $stmt_item = $conn->prepare("INSERT INTO tmp_stokopname_items (
        ID_BARANG, NAMA_BARANG, KATEGORI, HARGA,
        STOK_SYSTEM, STOK_NYATA, SATUAN, ISI_SATUAN,
        TOTAL_QTY, TOTAL_HARGA, KETERANGAN
    ) VALUES (
        :id_barang, :nama_barang, :kategori, :harga,
        :stok_system, :stok_nyata, :satuan, :isi_satuan,
        :total_qty, :total_harga, :keterangan
    )");

    foreach ($data['items'] as $item) {
        $total_qty   = floatval($item['total_qty'] ?? (floatval($item['stok_nyata'] ?? 0)));
        $total_harga = floatval($item['total_harga'] ?? (floatval($item['harga'] ?? 0) * $total_qty));

        $stmt_item->execute([
            ':id_barang'     => $item['id_barang'],
            ':nama_barang'   => $item['nama_barang'],
            ':kategori'      => $item['kategori'] ?? '',
            ':harga'         => floatval($item['harga'] ?? 0),
            ':stok_system'   => floatval($item['stok_system'] ?? 0),
            ':stok_nyata'    => floatval($item['stok_nyata'] ?? 0),
            ':satuan'        => $item['satuan'] ?? '',
            ':isi_satuan'    => intval($item['isi_satuan'] ?? 1),
            ':total_qty'     => $total_qty,
            ':total_harga'   => $total_harga,
            ':keterangan'    => $item['keterangan'] ?? $keterangan,
        ]);
    }

    $conn->exec("SET
        @p_id_opname          = '',
        @p_lokasi             = " . $conn->quote($lokasi)             . ",
        @p_tgl_transaksi      = " . $conn->quote($tgl_transaksi)      . ",
        @p_keterangan         = " . $conn->quote($keterangan)         . ",
        @p_id_user            = " . $conn->quote($id_user)            . ",
        @p_id_komputer        = " . $conn->quote($id_komputer)        . ",
        @p_izinkan_backdate   = " . intval($izinkan_backdate)
    );

    $conn->exec("CALL sp_trx_opname_simpan(
        @p_id_opname,
        @p_lokasi, @p_tgl_transaksi, @p_keterangan,
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

    echo "\n=== RESULT ===" . "\n";
    if (intval($row['success']) === 1) {
        $response = [
            'status'     => 'success',
            'id_opname'  => $row['id_opname'],
        ];
    } else {
        $response = [
            'status'     => 'error',
            'error_code' => $row['error_code']    ?? 'UNKNOWN',
            'message'    => $row['error_message'] ?? 'Terjadi kesalahan',
        ];
    }

    echo json_encode($response, JSON_PRETTY_PRINT) . "\n";
} catch (Exception $e) {
    try { $conn->exec("DROP TEMPORARY TABLE IF EXISTS tmp_stokopname_items"); } catch (Exception $_) {}
    echo "\n=== ERROR ===" . "\n";
    echo "Error: " . $e->getMessage() . "\n";
    echo "Trace: " . $e->getTraceAsString() . "\n";
}
?>
