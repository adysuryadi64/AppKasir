<?php
require_once 'db_connect.php';
require_once 'auth_check.php';

requireAuth($conn);

/*
 * ?tipe=KAS   → hanya akun kas
 * ?tipe=BANK  → hanya akun bank
 * (kosong)    → semua akun aktif
 */
$tipe = trim($_GET['tipe'] ?? '');

try {
    if ($tipe !== '') {
        $stmt = $conn->prepare("
            SELECT KODE_AKUN, NAMA_AKUN, TYPE_AKUN
            FROM tbl_datareferensi
            WHERE (Type_Akun = :tipe OR Type_Akun LIKE :tipe_like)
              AND STATUS = 'Aktif'
            ORDER BY KODE_AKUN ASC
        ");
        $stmt->execute([':tipe' => $tipe, ':tipe_like' => $tipe . ' %']);
    } else {
        $stmt = $conn->query("
            SELECT KODE_AKUN, NAMA_AKUN, TYPE_AKUN
            FROM tbl_datareferensi
            WHERE STATUS = 'Aktif'
            ORDER BY KODE_AKUN ASC
        ");
    }

    echo json_encode([
        'status' => 'success',
        'data'   => $stmt->fetchAll(PDO::FETCH_ASSOC),
    ]);

} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => 'Gagal mengambil data akun COA: ' . $e->getMessage()]);
}
?>
