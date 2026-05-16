<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

function _log(string $tag, string $msg): void { error_log("[GetOpnameList][$tag] $msg"); }

/*
 * Ambil daftar stok opname — 1 transaksi = 1 item
 * Langsung ambil detail barang: nama, stok sistem, nyata, selisih, rupiah
 */
$limit     = max(1, min((int)($_GET['limit']  ?? 50), 200));
$offset    = max(0, (int)($_GET['offset'] ?? 0));
$lokasi    = trim($_GET['lokasi']     ?? '');
$tglDari   = trim($_GET['tgl_dari']   ?? '');
$tglSampai = trim($_GET['tgl_sampai'] ?? '');

// Default: hari ini
if (empty($tglDari))   $tglDari   = date('Y-m-d');
if (empty($tglSampai)) $tglSampai = date('Y-m-d');

try {
    $whereClause = $lokasi !== '' ? "WHERE so.LOKASI = :lokasi_w" : "";
    $params_w    = $lokasi !== '' ? [':lokasi_w' => $lokasi] : [];

    $stmt = $conn->prepare("
        SELECT
            so.ID_STOK_OPNAME,
            so.TANGGAL,
            so.LOKASI,
            so.ID_USER,
            so.ID_BARANG,
            so.NAMA_BARANG,
            so.STOK_SYSTEM                          AS STOK_SYSTEM,
            so.STOK_NYATA                           AS STOK_NYATA,
            so.STOK_SELISIH                         AS STOK_SELISIH,
            COALESCE(so.TOTAL_HARGA, 0)             AS TOTAL_RUPIAH,
            COALESCE(so.SATUAN, '')                 AS SATUAN
        FROM Stok_Opname so
        $whereClause
        HAVING so.TANGGAL >= :tgl_dari
           AND so.TANGGAL <  DATE_ADD(:tgl_sampai, INTERVAL 1 DAY)
        ORDER BY so.TANGGAL DESC
        LIMIT :limit OFFSET :offset
    ");

    foreach ($params_w as $k => &$v) $stmt->bindParam($k, $v);
    unset($v);

    $stmt->bindParam(':tgl_dari',   $tglDari);
    $stmt->bindParam(':tgl_sampai', $tglSampai);
    $stmt->bindParam(':limit',  $limit,  PDO::PARAM_INT);
    $stmt->bindParam(':offset', $offset, PDO::PARAM_INT);
    $stmt->execute();

    $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

    echo json_encode([
        'status'     => 'success',
        'data'       => $rows,
        'count'      => count($rows),
        'tgl_dari'   => $tglDari,
        'tgl_sampai' => $tglSampai,
    ]);

} catch (Exception $e) {
    error_log("[GetOpnameList][EXCEPTION] " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Gagal: ' . $e->getMessage()]);
}
?>
