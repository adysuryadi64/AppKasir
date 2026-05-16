<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

// Endpoint ringan — return nama database aktif
// Tidak butuh auth karena hanya return info non-sensitif
try {
    $config = file_exists('config.php') ? include 'config.php' : [];
    echo json_encode([
        'status'   => 'success',
        'data'     => [
            'db_name' => $config['db_name'] ?? '',
        ],
    ]);
} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
?>
