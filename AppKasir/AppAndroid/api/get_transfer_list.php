<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

/*
 * Ambil daftar transfer stok
 */

// ── Helper log ────────────────────────────────────────────────────────────
function _log(string $tag, string $msg): void {
    error_log("[TransferList][$tag] $msg");
}

$limit     = max(1, min((int)($_GET['limit']   ?? 50), 200));
$offset    = max(0,          (int)($_GET['offset']  ?? 0));
$lokasi    = trim($_GET['lokasi']     ?? '');
$tglDari   = trim($_GET['tgl_dari']   ?? '');
$tglSampai = trim($_GET['tgl_sampai'] ?? '');
$search    = trim($_GET['search']     ?? '');

if (empty($tglDari))   $tglDari   = date('Y-m-d');
if (empty($tglSampai)) $tglSampai = date('Y-m-d');

_log('PARAM', "lokasi=$lokasi | tgl=$tglDari~$tglSampai | search=$search | limit=$limit | offset=$offset");

try {
    $where   = [];
    $params  = [];

    // Filter tanggal
    $where[]              = 'ts.TANGGAL >= :tgl_dari';
    $where[]              = 'ts.TANGGAL <  DATE_ADD(:tgl_sampai, INTERVAL 1 DAY)';
    $params[':tgl_dari']   = $tglDari;
    $params[':tgl_sampai'] = $tglSampai;

    // Filter lokasi — pakai JENIS_TRANSFER karena tabel tidak punya kolom LOKASI
    // JENIS_TRANSFER berisi 'TOKO' atau 'GUDANG' (konsisten dengan VB)
    if ($lokasi !== '') {
        $where[]           = 'ts.JENIS_TRANSFER = :lokasi';
        $params[':lokasi'] = $lokasi;
    }

    // Filter search — no. transfer atau nama barang
    if ($search !== '') {
        $where[]            = '(ts.ID_TRANSFER LIKE :search
                                OR ts.NAMA_BARANG_K LIKE :search
                                OR ts.NAMA_BARANG_M LIKE :search
                                OR ts.URAIAN        LIKE :search)';
        $params[':search']  = '%' . $search . '%';
    }

    $whereSQL = count($where) ? 'WHERE ' . implode(' AND ', $where) : '';

    $sql = "
        SELECT
            ts.ID_TRANSFER,
            ts.TANGGAL,
            COALESCE(ts.JENIS_TRANSFER, '') AS LOKASI,
            COALESCE(ts.URAIAN,  '')        AS URAIAN,
            COALESCE(ts.ID_USER, '')        AS ID_USER,

            -- Barang Keluar
            COALESCE(ts.NAMA_BARANG_K, '')    AS NAMA_BARANG_K,
            COALESCE(ts.QTY_K,         0)     AS QTY_K,
            COALESCE(ts.SATUAN_K,      '')    AS SATUAN_K,
            COALESCE(ts.TOTAL_HARGA_K, 0)     AS TOTAL_HARGA_K,

            -- Barang Masuk
            COALESCE(ts.NAMA_BARANG_M, '')    AS NAMA_BARANG_M,
            COALESCE(ts.QTY_M,         0)     AS QTY_M,
            COALESCE(ts.SATUAN_M,      '')    AS SATUAN_M,
            COALESCE(ts.TOTAL_HARGA_M, 0)     AS TOTAL_HARGA_M,

            COALESCE(ts.Selisih,       0)     AS Selisih
        FROM Transfer_stok ts
        $whereSQL
        ORDER BY ts.TANGGAL DESC, ts.ID_TRANSFER DESC
        LIMIT :limit OFFSET :offset
    ";

    $stmt = $conn->prepare($sql);

    foreach ($params as $k => &$v) {
        $stmt->bindParam($k, $v);
    }
    unset($v);

    $stmt->bindParam(':limit',  $limit,  PDO::PARAM_INT);
    $stmt->bindParam(':offset', $offset, PDO::PARAM_INT);
    $stmt->execute();

    $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

    _log('RESULT', "count=" . count($rows));

    echo json_encode([
        'status'     => 'success',
        'data'       => $rows,
        'count'      => count($rows),
        'tgl_dari'   => $tglDari,
        'tgl_sampai' => $tglSampai,
    ]);

} catch (Exception $e) {
    _log('EXCEPTION', $e->getMessage());
    echo json_encode([
        'status'  => 'error',
        'message' => 'Gagal: ' . $e->getMessage(),
    ]);
}
?>
