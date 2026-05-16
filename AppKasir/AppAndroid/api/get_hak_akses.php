<?php
require_once 'db_connect.php';
require_once 'auth_check.php';

function _log(string $tag, string $msg): void { error_log("[GetHakAkses][$tag] $msg"); }

requireAuth($conn);

/*
 * Tabel: hakaksesuser
 * Kolom: UserName, Role, ModuleName (nilai setting), CanRead, CanAdd, CanEdit, CanDelete
 *
 * General setting disimpan dengan UserName = 'Semua'.
 * Role = nama setting (Label.Text di FormGeneralSetting.Designer.vb)
 * ModuleName = nilai (misal "Iya" atau "Tidak")
 *
 * roleMap bisa di-override via POST body { "role_mapping": { "key": "nama_role" } }
 * agar tidak perlu ubah kode jika nama label di VB berubah.
 */

// Default mapping — sesuai Label.Text di FormGeneralSetting.Designer.vb
$defaultRoleMap = [
    'izinkan_ubah_harga'      => 'Izinkan user mengubah harga jual',
    'izinkan_jual_rugi'       => 'Izinkan jual barang di bawah harga beli',
    'izinkan_jual_stok_minus' => 'Izinkan transaksi keluar barang meski stok jadi minus',
    'izinkan_satuan_berbeda'  => 'Izinkan kode barang dengan satuan berbeda',
    'tampil_info_stok'        => 'Tampilkan informasi stok saat transaksi',
    'langsung_isi_nominal'    => 'Langsung isi nominal total transaksi',
    'izinkan_nominal_nol'     => 'Izinkan penjualan dengan nominal 0',
    'izinkan_tanggal_lampau'  => 'Semua transaksi boleh menggunakan tanggal lampau',
];

// Override dari Flutter jika dikirim via POST
$roleMap = $defaultRoleMap;
$body = json_decode(file_get_contents('php://input'), true) ?? [];
if (!empty($body['role_mapping']) && is_array($body['role_mapping'])) {
    foreach ($body['role_mapping'] as $key => $roleName) {
        if (isset($roleMap[$key]) && is_string($roleName) && trim($roleName) !== '') {
            $roleMap[$key] = trim($roleName);
        }
    }
}

// Default semua true — aman untuk mobile (tidak memblokir kasir)
$result = [
    'izinkan_ubah_harga'      => true,
    'izinkan_jual_rugi'       => true,
    'izinkan_jual_stok_minus' => true,
    'izinkan_satuan_berbeda'  => true,
    'tampil_info_stok'        => true,
    'langsung_isi_nominal'    => false,
    'izinkan_nominal_nol'     => false,
    'izinkan_tanggal_lampau'  => false,
];

try {
    $placeholders = implode(',', array_fill(0, count($roleMap), '?'));
    $roles        = array_values($roleMap); // nama role (bukan key)

    $stmt = $conn->prepare("
        SELECT Role, ModuleName
        FROM hakaksesuser
        WHERE UserName = 'Semua'
          AND Role IN ($placeholders)
          AND ModuleName <> ''
    ");
    $stmt->execute($roles);

    // Balik roleMap: nama_role → key JSON
    $reverseMap = array_flip($roleMap);

    while ($row = $stmt->fetch(PDO::FETCH_ASSOC)) {
        $roleName = $row['Role'];
        $nilai    = $row['ModuleName']; // "Iya" atau "Tidak"

        if (isset($reverseMap[$roleName])) {
            $key          = $reverseMap[$roleName];
            $result[$key] = ($nilai === 'Iya');
        }
    }

    echo json_encode(['status' => 'success', 'data' => $result]);

} catch (Exception $e) {
    error_log("[GetHakAkses][EXCEPTION] " . $e->getMessage());
    // Jika query gagal (misal tabel belum ada), kembalikan default
    echo json_encode(['status' => 'success', 'data' => $result, 'note' => 'default']);
}
?>
