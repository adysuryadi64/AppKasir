<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type, Authorization');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit();
}

require_once 'db_connect.php';
require_once 'no_browser.php';

try {
    $stmt = $conn->prepare("
        SELECT
            KODE,
            NAMA,
            ALAMAT,
            NO_TELP,
            JENIS,
            JANGKAPIUTANG,
            HUTANGAKHIR
        FROM tbl_pelanggan
        WHERE Status = 'Aktif'
        ORDER BY NAMA ASC
    ");
    $stmt->execute();

    echo json_encode([
        'status'  => 'success',
        'message' => 'Pelanggan retrieved successfully',
        'data'    => $stmt->fetchAll(PDO::FETCH_ASSOC),
    ]);

} catch (Exception $e) {
    echo json_encode([
        'status'  => 'error',
        'message' => 'Failed to retrieve pelanggan: ' . $e->getMessage(),
    ]);
}
?>
