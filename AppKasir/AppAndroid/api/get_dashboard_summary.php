<?php
require_once 'db_connect.php';
require_once 'auth_check.php';

function _log(string $tag, string $msg): void { error_log("[GetDashboard][$tag] $msg"); }

requireAuth($conn);

$lokasi = trim($_GET['lokasi'] ?? '');

/*
 * OPTIMASI: Gunakan range comparison (>= dan <) bukan DATE(kolom) = CURDATE()
 * Alasan: DATE(TGL_TRANSAKSI) = CURDATE() mencegah MySQL menggunakan index.
 * Range comparison memungkinkan index scan yang jauh lebih cepat.
 */
try {
    $lokasiCond  = $lokasi !== '' ? "AND LOKASIBARANG = :lokasi" : '';
    $lokasiCond2 = $lokasi !== '' ? "AND LOKASI = :lokasi" : '';

    // ── Query 1: Total penjualan + jumlah transaksi hari ini ──────────
    $sql1 = "
        SELECT
            COALESCE(SUM(GRAND_TOTAL_STL_PAJAK), 0) AS total_penjualan,
            COUNT(*) AS jumlah_transaksi
        FROM penjualan
        WHERE TGL_TRANSAKSI >= CURDATE()
          AND TGL_TRANSAKSI <  DATE_ADD(CURDATE(), INTERVAL 1 DAY)
          $lokasiCond
    ";
    $stmt1 = $conn->prepare($sql1);
    if ($lokasi !== '') $stmt1->bindParam(':lokasi', $lokasi);
    $stmt1->execute();
    $row1 = $stmt1->fetch(PDO::FETCH_ASSOC);

    // ── Query 2: Jumlah item stok opname hari ini ─────────────────────
    $sql2 = "
        SELECT COUNT(*) AS jumlah_opname
        FROM Stok_Opname
        WHERE TANGGAL >= CURDATE()
          AND TANGGAL <  DATE_ADD(CURDATE(), INTERVAL 1 DAY)
          $lokasiCond2
    ";
    $stmt2 = $conn->prepare($sql2);
    if ($lokasi !== '') $stmt2->bindParam(':lokasi', $lokasi);
    $stmt2->execute();
    $row2 = $stmt2->fetch(PDO::FETCH_ASSOC);

    echo json_encode([
        'status' => 'success',
        'data'   => [
            'total_penjualan'  => (float)($row1['total_penjualan']  ?? 0),
            'jumlah_transaksi' => (int)($row1['jumlah_transaksi']   ?? 0),
            'jumlah_opname'    => (int)($row2['jumlah_opname']      ?? 0),
        ],
    ]);

} catch (Exception $e) {
    error_log("[GetDashboard][EXCEPTION] " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Gagal mengambil summary: ' . $e->getMessage()]);
}
?>
