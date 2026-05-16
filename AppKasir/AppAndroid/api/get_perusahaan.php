<?php
require_once 'db_connect.php';
require_once 'auth_check.php';

function _log(string $tag, string $msg): void { error_log("[GetPerusahaan][$tag] $msg"); }

requireAuth($conn);

try {
    $stmt = $conn->query("
        SELECT
            KODE,
            NAMA,
            ALAMAT,
            KOTA,
            HP,
            PEMILIK,
            COALESCE(FOOTER1, '') AS FOOTER1,
            COALESCE(FOOTER2, '') AS FOOTER2,
            COALESCE(FOOTER3, '') AS FOOTER3,
            COALESCE(Kode_rek_Jual_Toko,    '') AS kode_kas_toko,
            COALESCE(nama_rek_Jual_Toko,    '') AS nama_kas_toko,
            COALESCE(Kode_rek_Jual_Gudang,  '') AS kode_kas_gudang,
            COALESCE(nama_rek_Jual_Gudang,  '') AS nama_kas_gudang,
            COALESCE(Kode_rek_Transfer_Jual,'') AS kode_transfer,
            COALESCE(nama_rek_Transfer_Jual,'') AS nama_transfer,
            COALESCE(Kode_rek_Piutang_Jual, '') AS kode_piutang,
            COALESCE(nama_rek_Piutang_Jual, '') AS nama_piutang,
            COALESCE(KODE_REK_BARANG,       '') AS kode_barang,
            COALESCE(NAMA_REK_BARANG,       '') AS nama_barang
        FROM tbl_perusahaan
        LIMIT 1
    ");

    $row = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$row) {
        echo json_encode(['status' => 'error', 'message' => 'Data perusahaan tidak ditemukan']);
        exit();
    }

    echo json_encode([
        'status' => 'success',
        'data'   => [
            'kode'    => $row['KODE']    ?? '',
            'nama'    => $row['NAMA']    ?? '',
            'alamat'  => $row['ALAMAT']  ?? '',
            'kota'    => $row['KOTA']    ?? '',
            'hp'      => $row['HP']      ?? '',
            'pemilik' => $row['PEMILIK'] ?? '',
            'footer1' => $row['FOOTER1'] ?? '',
            'footer2' => $row['FOOTER2'] ?? '',
            'footer3' => $row['FOOTER3'] ?? '',
            'akun_kas_toko'    => ['kode' => $row['kode_kas_toko'],    'nama' => $row['nama_kas_toko']],
            'akun_kas_gudang'  => ['kode' => $row['kode_kas_gudang'],  'nama' => $row['nama_kas_gudang']],
            'akun_transfer'    => ['kode' => $row['kode_transfer'],    'nama' => $row['nama_transfer']],
            'akun_piutang'     => ['kode' => $row['kode_piutang'],     'nama' => $row['nama_piutang']],
            'akun_barang'      => ['kode' => $row['kode_barang'],      'nama' => $row['nama_barang']],
        ],
    ]);

} catch (Exception $e) {
    error_log("[GetPerusahaan][EXCEPTION] " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Gagal mengambil data perusahaan: ' . $e->getMessage()]);
}
?>
