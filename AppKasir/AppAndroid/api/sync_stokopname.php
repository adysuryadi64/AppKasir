<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

// ── Helper log ────────────────────────────────────────────────────────────
function _log(string $tag, string $msg): void {
    error_log("[StokOpname][$tag] $msg");
}

$raw  = file_get_contents('php://input');
$data = json_decode($raw, true) ?? [];

_log('INPUT', 'raw=' . substr($raw, 0, 500));

// ── Baca parameter header dari payload ───────────────────────────────────
$tgl_transaksi    = !empty($data['tgl_transaksi'])
                        ? date('Y-m-d H:i:s', strtotime($data['tgl_transaksi']))
                        : date('Y-m-d H:i:s');
$lokasi           = $data['lokasi']      ?? '';
$id_user          = $data['id_user']     ?? '';
$id_komputer      = $data['id_komputer'] ?? '';
$keterangan       = $data['keterangan']  ?? '';

// Parameter hak akses dari payload Flutter
$izinkan_backdate   = intval($data['izinkan_backdate'] ?? 0);

_log('PARAM', "lokasi=$lokasi | id_user=$id_user | tgl=$tgl_transaksi | items=" . count($data['items'] ?? []));

// Validasi field wajib
if (empty($lokasi) || empty($data['items'])) {
    _log('VALIDASI', 'Field wajib kosong');
    echo json_encode([
        'status'  => 'error',
        'error_code' => 'MISSING_FIELD',
        'message' => 'Lokasi dan items wajib diisi',
    ]);
    exit;
}

try {
    // ── 1. Buat temporary table ───────────────────────────────────────────
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

    // Pastikan kosong (jika koneksi di-reuse)
    $conn->exec("DELETE FROM tmp_stokopname_items");

    // ── 2. INSERT items dari payload ke tmp table ─────────────────────────
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

    // ── 3. Set semua IN parameter sebagai user variables ──────────────────
    _log('SP', 'SET user variables...');
    $conn->exec("SET
        @p_id_opname          = '',
        @p_lokasi             = " . $conn->quote($lokasi)             . ",
        @p_tgl_transaksi      = " . $conn->quote($tgl_transaksi)      . ",
        @p_keterangan         = " . $conn->quote($keterangan)         . ",
        @p_id_user            = " . $conn->quote($id_user)            . ",
        @p_id_komputer        = " . $conn->quote($id_komputer)        . ",
        @p_izinkan_backdate   = " . intval($izinkan_backdate)
    );

    // ── 4. CALL SP ────────────────────────────────────────────────────────
    _log('SP', 'CALL sp_trx_opname_simpan...');
    $conn->exec("CALL sp_trx_opname_simpan(
        @p_id_opname,
        @p_lokasi, @p_tgl_transaksi, @p_keterangan,
        @p_id_user, @p_id_komputer, @p_izinkan_backdate,
        @p_success, @p_error_code, @p_error_message, @p_id_opname_out
    )");

    // ── 5. Baca OUT parameter ─────────────────────────────────────────────
    $row = $conn->query("SELECT
        @p_success        AS success,
        @p_error_code     AS error_code,
        @p_error_message  AS error_message,
        @p_id_opname_out  AS id_opname
    ")->fetch(PDO::FETCH_ASSOC);

    _log('SP', "OUT: success={$row['success']} | error_code={$row['error_code']} | message={$row['error_message']} | id_opname={$row['id_opname']}");

    // ── 6. DROP TEMPORARY TABLE ───────────────────────────────────────────
    $conn->exec("DROP TEMPORARY TABLE IF EXISTS tmp_stokopname_items");

    // ── 7. Response JSON ──────────────────────────────────────────────────
    if (intval($row['success']) === 1) {
        _log('OK', "id_opname={$row['id_opname']}");
        echo json_encode([
            'status'    => 'success',
            'id_opname' => $row['id_opname'],
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
    // Cleanup tmp table jika ada error sebelum DROP
    try { $conn->exec("DROP TEMPORARY TABLE IF EXISTS tmp_stokopname_items"); } catch (Exception $_) {}

    _log('EXCEPTION', $e->getMessage() . ' | ' . $e->getTraceAsString());
    echo json_encode([
        'status'     => 'error',
        'error_code' => 'SQL_ERROR',
        'message'    => 'Gagal menyimpan transaksi: ' . $e->getMessage(),
    ]);
}
?>
