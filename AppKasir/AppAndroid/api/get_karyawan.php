<?php
require_once 'db_connect.php';
require_once 'auth_check.php';

requireAuth($conn);

try {
    $stmt = $conn->query("
        SELECT Kode, Nama
        FROM tbl_karyawan
        WHERE Status = 'Aktif'
        ORDER BY Nama ASC
    ");

    echo json_encode([
        'status' => 'success',
        'data'   => $stmt->fetchAll(PDO::FETCH_ASSOC),
    ]);

} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => 'Gagal mengambil data karyawan: ' . $e->getMessage()]);
}
?>
