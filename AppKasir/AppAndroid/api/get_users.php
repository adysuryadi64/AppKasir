<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

// get_users dipanggil di login screen SEBELUM user punya token
// Cukup validasi bahwa request bukan dari browser biasa (sudah ditangani no_browser.php)
// Tidak perlu token karena ini untuk mengisi dropdown login

try {
    $stmt = $conn->prepare(
        "SELECT KODE_USER, NAMA_USER, USER_NAME, LVL
         FROM tbl_user
         WHERE status = 'Aktif'
         ORDER BY NAMA_USER ASC"
    );
    $stmt->execute();

    echo json_encode([
        'status' => 'success',
        'data'   => $stmt->fetchAll(PDO::FETCH_ASSOC),
    ]);

} catch (Exception $e) {
    http_response_code(500);
    echo json_encode(['status' => 'error', 'message' => 'Gagal mengambil data user']);
}
?>
