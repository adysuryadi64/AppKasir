<?php
require_once 'db_connect.php';
require_once 'auth_check.php';

function _log(string $tag, string $msg): void { error_log("[GetLaporanStok][$tag] $msg"); }

requireAuth($conn);

$search   = trim($_GET['search']   ?? '');
$kategori = trim($_GET['kategori'] ?? '');
$limit    = max(1, min((int)($_GET['limit']  ?? 50), 200));
$offset   = max(0, (int)($_GET['offset'] ?? 0));

try {
    // ── WHERE conditions ──────────────────────────────────────────────
    $where  = [];
    $params = [];

    if ($search !== '') {
        $where[]              = "(b.NAMA_BARANG LIKE :search OR b.BARCODE_KECIL = :barcode OR b.BARCODE_SEDANG = :barcode OR b.BARCODE_BESAR = :barcode OR b.ID_BARANG LIKE :search_prefix)";
        $params[':search']        = '%' . $search . '%';
        $params[':barcode']       = $search;
        $params[':search_prefix'] = $search . '%';
    }

    if ($kategori !== '') {
        $where[]              = "b.NAMA_KATEGORI = :kategori";
        $params[':kategori']  = $kategori;
    }

    $whereClause = count($where) > 0 ? 'WHERE ' . implode(' AND ', $where) : '';

    // ── Count total ───────────────────────────────────────────────────
    $stmtCount = $conn->prepare("SELECT COUNT(*) FROM tbl_barang b $whereClause");
    foreach ($params as $k => &$v) $stmtCount->bindParam($k, $v);
    unset($v);
    $stmtCount->execute();
    $totalCount = (int)$stmtCount->fetchColumn();

    // ── Data query ────────────────────────────────────────────────────
    $sql = "
        SELECT
            b.ID_BARANG,
            b.NAMA_BARANG,
            b.BARCODE_KECIL     AS BARCODE,
            COALESCE(b.STOK_TOKO,   0) AS STOK_TOKO,
            COALESCE(b.STOK_GUDANG, 0) AS STOK_GUDANG,
            COALESCE(b.NAMA_KATEGORI, '') AS NAMA_KATEGORI,
            COALESCE(b.NAMA_MERK,     '') AS NAMA_MERK,
            COALESCE(b.SATUAN_UMUM_KECIL, '') AS SATUAN,
            b.HARGA_BELI,
            b.HARGA_JUAL_UMUM_KECIL AS HARGA_JUAL
        FROM tbl_barang b
        $whereClause
        ORDER BY b.NAMA_BARANG ASC
        LIMIT :limit OFFSET :offset
    ";

    $stmt = $conn->prepare($sql);
    foreach ($params as $k => &$v) $stmt->bindParam($k, $v);
    unset($v);
    $stmt->bindParam(':limit',  $limit,  PDO::PARAM_INT);
    $stmt->bindParam(':offset', $offset, PDO::PARAM_INT);
    $stmt->execute();

    echo json_encode([
        'status'      => 'success',
        'data'        => $stmt->fetchAll(PDO::FETCH_ASSOC),
        'total_count' => $totalCount,
        'limit'       => $limit,
        'offset'      => $offset,
    ]);

} catch (Exception $e) {
    error_log("[GetLaporanStok][EXCEPTION] " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Gagal mengambil laporan stok: ' . $e->getMessage()]);
}
?>
