<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

function _log(string $tag, string $msg): void { error_log("[KategoriMerk][$tag] $msg"); }

$method = $_SERVER['REQUEST_METHOD'];

if ($method === 'OPTIONS') {
    http_response_code(200);
    exit();
}

$type = isset($_GET['type']) ? $_GET['type'] : '';

if ($type !== 'kategori' && $type !== 'merk') {
    echo json_encode(['status' => 'error', 'message' => "Type harus 'kategori' atau 'merk'"]);
    exit();
}

// ── Helper: ProperCase (sama dengan VB StrConv vbProperCase) ──────────────
function properCase(string $str): string {
    return mb_convert_case(mb_strtolower($str), MB_CASE_TITLE, 'UTF-8');
}

// ── Helper: Generate kode sequential (cari gap, lalu increment) ───────────
function generateKode(PDO $conn, string $tabel, string $prefix): string {
    $stmt = $conn->prepare("SELECT KODE FROM $tabel ORDER BY KODE ASC");
    $stmt->execute();
    $existing = $stmt->fetchAll(PDO::FETCH_COLUMN);

    // Cari gap
    for ($i = 1; $i <= count($existing) + 1; $i++) {
        $kode = $prefix . str_pad($i, 4, '0', STR_PAD_LEFT);
        if (!in_array($kode, $existing)) return $kode;
    }
    return $prefix . str_pad(count($existing) + 1, 4, '0', STR_PAD_LEFT);
}

try {
    switch ($method) {

        // ── GET: ambil semua data atau generate kode baru ────────────────
        case 'GET':
            // ?action=generate_kode → kembalikan kode berikutnya
            if (isset($_GET['action']) && $_GET['action'] === 'generate_kode') {
                if ($type === 'kategori') {
                    $kode = generateKode($conn, 'tbl_kategori', 'KAT-');
                } else {
                    $kode = generateKode($conn, 'tbl_merk', 'MRK-');
                }
                echo json_encode(['status' => 'success', 'kode' => $kode]);
                break;
            }

            if ($type === 'kategori') {
                $stmt = $conn->query(
                    "SELECT KODE AS kode, NAMA AS nama, JENIS AS jenis
                     FROM tbl_kategori ORDER BY NAMA ASC"
                );
            } else {
                $stmt = $conn->query(
                    "SELECT KODE AS kode, NAMA AS nama, KETERANGAN AS keterangan
                     FROM tbl_merk ORDER BY NAMA ASC"
                );
            }
            echo json_encode(['status' => 'success', 'data' => $stmt->fetchAll(PDO::FETCH_ASSOC)]);
            break;

        // ── POST: tambah baru — kode duplikat auto-generate, nama duplikat error ──
        case 'POST':
            $input = json_decode(file_get_contents('php://input'), true);

            if (empty($input['nama'])) {
                echo json_encode(['status' => 'error', 'message' => 'Nama harus diisi']);
                exit();
            }

            // Nama: ProperCase (sama dengan VB)
            $nama = properCase(trim($input['nama']));

            if ($type === 'kategori') {
                $jenis = properCase(trim($input['jenis'] ?? ''));
                if (empty($jenis)) $jenis = 'Barang';

                // Cek duplikat nama dulu
                $chk = $conn->prepare("SELECT 1 FROM tbl_kategori WHERE NAMA = :nama LIMIT 1");
                $chk->execute([':nama' => $nama]);
                if ($chk->fetch()) {
                    echo json_encode(['status' => 'error', 'message' => 'Nama kategori sudah ada, silakan ganti dengan yang lain']);
                    exit();
                }

                // Kode: pakai dari input jika ada, jika duplikat → generate ulang
                $kode = !empty($input['kode']) ? strtoupper(trim($input['kode'])) : generateKode($conn, 'tbl_kategori', 'KAT-');
                $chk = $conn->prepare("SELECT 1 FROM tbl_kategori WHERE KODE = :kode LIMIT 1");
                $chk->execute([':kode' => $kode]);
                if ($chk->fetch()) {
                    $kode = generateKode($conn, 'tbl_kategori', 'KAT-');
                }

                $stmt = $conn->prepare(
                    "INSERT INTO tbl_kategori (KODE, NAMA, JENIS) VALUES (:kode, :nama, :jenis)"
                );
                $stmt->execute([':kode' => $kode, ':nama' => $nama, ':jenis' => $jenis]);

            } else {
                $keterangan = trim($input['keterangan'] ?? '');

                // Cek duplikat nama dulu
                $chk = $conn->prepare("SELECT 1 FROM tbl_merk WHERE NAMA = :nama LIMIT 1");
                $chk->execute([':nama' => $nama]);
                if ($chk->fetch()) {
                    echo json_encode(['status' => 'error', 'message' => 'Nama merk sudah ada, silakan ganti dengan yang lain']);
                    exit();
                }

                // Kode: pakai dari input jika ada, jika duplikat → generate ulang
                $kode = !empty($input['kode']) ? strtoupper(trim($input['kode'])) : generateKode($conn, 'tbl_merk', 'MRK-');
                $chk = $conn->prepare("SELECT 1 FROM tbl_merk WHERE KODE = :kode LIMIT 1");
                $chk->execute([':kode' => $kode]);
                if ($chk->fetch()) {
                    $kode = generateKode($conn, 'tbl_merk', 'MRK-');
                }

                $stmt = $conn->prepare(
                    "INSERT INTO tbl_merk (KODE, NAMA, KETERANGAN) VALUES (:kode, :nama, :ket)"
                );
                $stmt->execute([':kode' => $kode, ':nama' => $nama, ':ket' => $keterangan]);
            }

            echo json_encode(['status' => 'success', 'message' => 'Data berhasil ditambahkan', 'kode' => $kode]);
            break;

        // ── PUT: update — kode tidak boleh berubah ────────────────────────
        case 'PUT':
            if (empty($_GET['kode'])) {
                echo json_encode(['status' => 'error', 'message' => 'Kode diperlukan untuk update']);
                exit();
            }

            $input = json_decode(file_get_contents('php://input'), true);

            if (empty($input['nama'])) {
                echo json_encode(['status' => 'error', 'message' => 'Nama harus diisi']);
                exit();
            }

            $kode = strtoupper(trim($_GET['kode']));
            $nama = properCase(trim($input['nama']));

            if ($type === 'kategori') {
                $jenis = properCase(trim($input['jenis'] ?? ''));
                if (empty($jenis)) $jenis = 'Barang';
                $stmt = $conn->prepare(
                    "UPDATE tbl_kategori SET NAMA = :nama, JENIS = :jenis WHERE KODE = :kode"
                );
                $stmt->execute([':nama' => $nama, ':jenis' => $jenis, ':kode' => $kode]);
            } else {
                $keterangan = trim($input['keterangan'] ?? '');
                $stmt = $conn->prepare(
                    "UPDATE tbl_merk SET NAMA = :nama, KETERANGAN = :keterangan WHERE KODE = :kode"
                );
                $stmt->execute([':nama' => $nama, ':keterangan' => $keterangan, ':kode' => $kode]);
            }

            echo json_encode(['status' => 'success', 'message' => 'Data berhasil diupdate']);
            break;

        // ── DELETE: cek pemakaian di tbl_barang sebelum hapus ────────────
        case 'DELETE':
            if (empty($_GET['kode'])) {
                echo json_encode(['status' => 'error', 'message' => 'Kode diperlukan untuk hapus']);
                exit();
            }

            $kode = strtoupper(trim($_GET['kode']));
            $col  = ($type === 'kategori') ? 'KODE_KATEGORI' : 'KODE_MERK';

            $chk = $conn->prepare("SELECT COUNT(*) FROM tbl_barang WHERE $col = :kode");
            $chk->execute([':kode' => $kode]);
            if ($chk->fetchColumn() > 0) {
                echo json_encode(['status' => 'error', 'message' => 'Tidak bisa dihapus, data sedang digunakan di master barang']);
                exit();
            }

            $tabel = ($type === 'kategori') ? 'tbl_kategori' : 'tbl_merk';
            $stmt  = $conn->prepare("DELETE FROM $tabel WHERE KODE = :kode");
            $stmt->execute([':kode' => $kode]);

            echo json_encode(['status' => 'success', 'message' => 'Data berhasil dihapus']);
            break;

        default:
            echo json_encode(['status' => 'error', 'message' => 'Method tidak didukung']);
            break;
    }

} catch (Exception $e) {
    error_log("[KategoriMerk][EXCEPTION] " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Terjadi kesalahan sistem: ' . $e->getMessage()]);
}
?>
