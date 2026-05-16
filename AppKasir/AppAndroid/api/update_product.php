<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type, Authorization');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit();
}

require_once 'db_connect.php';
require_once 'no_browser.php';

function _log(string $tag, string $msg): void { error_log("[UpdateProduct][$tag] $msg"); }

if (empty($data['id_barang'])) {
    echo json_encode(['status' => 'error', 'message' => 'ID Barang wajib diisi']);
    exit();
}

$id_barang = $data['id_barang'];

// Kolom yang boleh diupdate dari mobile
// Sesuai schema tbl_barang: NAMA_KATEGORI (denormalized), NAMA_MERK (denormalized)
$sets   = [];
$params = [':id_barang' => $id_barang];

if (isset($data['kategori']) && $data['kategori'] !== null) {
    $sets[]               = 'NAMA_KATEGORI = :nama_kategori';
    $params[':nama_kategori'] = trim($data['kategori']);
}

if (isset($data['merk']) && $data['merk'] !== null) {
    $sets[]           = 'NAMA_MERK = :nama_merk';
    $params[':nama_merk'] = trim($data['merk']);
}

if (empty($sets)) {
    echo json_encode(['status' => 'error', 'message' => 'Tidak ada field yang diupdate']);
    exit();
}

try {
    $stmt = $conn->prepare(
        'UPDATE tbl_barang SET ' . implode(', ', $sets) . ' WHERE ID_BARANG = :id_barang'
    );
    $stmt->execute($params);

    if ($stmt->rowCount() > 0) {
        echo json_encode(['status' => 'success', 'message' => 'Produk berhasil diupdate']);
    } else {
        echo json_encode(['status' => 'error', 'message' => 'Produk tidak ditemukan atau tidak ada perubahan']);
    }

} catch (Exception $e) {
    error_log("[UpdateProduct][EXCEPTION] " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Gagal update produk: ' . $e->getMessage()]);
}
?>
