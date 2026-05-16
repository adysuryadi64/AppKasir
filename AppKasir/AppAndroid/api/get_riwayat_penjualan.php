<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

function _log(string $tag, string $msg): void { error_log("[GetRiwayat][$tag] $msg"); }

/*
 * Riwayat Penjualan — list + detail
 * mode=list   → daftar transaksi (identik query VB Datapenjualan)
 * mode=detail → header + items per faktur
 */

$mode      = trim($_GET['mode']     ?? 'list');
$lokasi    = trim($_GET['lokasi']   ?? '');
$tglDari   = trim($_GET['tgl_dari']   ?? '');
$tglSampai = trim($_GET['tgl_sampai'] ?? '');
$search    = trim($_GET['search']   ?? '');
$faktur    = trim($_GET['faktur']   ?? '');
$limit     = max(1, min((int)($_GET['limit']  ?? 30), 100));
$offset    = max(0, (int)($_GET['offset'] ?? 0));

if (empty($tglDari))   $tglDari   = date('Y-m-d');
if (empty($tglSampai)) $tglSampai = date('Y-m-d');

// Batas waktu: tgl_dari 00:00:00 s/d tgl_sampai 23:59:59
$tAwal  = $tglDari   . ' 00:00:00';
$tAkhir = $tglSampai . ' 23:59:59';

try {
    // ── MODE LIST ─────────────────────────────────────────────────
    if ($mode === 'list') {
        $sf = '%' . $search . '%';

        // Rangkuman (total record + total nilai)
        $stmtSum = $conn->prepare("
            SELECT COUNT(*) AS RECORD,
                   COALESCE(SUM(GRAND_TOTAL_STL_PAJAK), 0) AS TOTAL
            FROM penjualan
            WHERE TGL_TRANSAKSI >= :tAwal
              AND TGL_TRANSAKSI <= :tAkhir
              AND (:lokasi = '' OR LOKASIBARANG = :lokasi2)
              AND (ID_PENJUALAN LIKE :sf OR NAMA_PELANGGAN LIKE :sf2)
        ");
        $stmtSum->execute([
            ':tAwal'   => $tAwal,
            ':tAkhir'  => $tAkhir,
            ':lokasi'  => $lokasi,
            ':lokasi2' => $lokasi,
            ':sf'      => $sf,
            ':sf2'     => $sf,
        ]);
        $summary = $stmtSum->fetch(PDO::FETCH_ASSOC);

        // Data list — kolom identik dengan VB Datapenjualan()
        $stmt = $conn->prepare("
            SELECT
                ID_PENJUALAN,
                TGL_TRANSAKSI,
                NAMA_PELANGGAN,
                LOKASIBARANG,
                JENIS_PEMBAYARAN,
                GRAND_TOTAL_STL_PAJAK,
                BAYAR,
                NOMINAL_TRANSFER,
                KEMBALI,
                COALESCE(NILAI_RETUR, 0)  AS NILAI_RETUR,
                SISA_TAGIHAN,
                STATUS_TRANSAKSI,
                ID_USER
            FROM penjualan
            WHERE TGL_TRANSAKSI >= :tAwal
              AND TGL_TRANSAKSI <= :tAkhir
              AND (:lokasi = '' OR LOKASIBARANG = :lokasi2)
              AND (ID_PENJUALAN LIKE :sf OR NAMA_PELANGGAN LIKE :sf2)
            ORDER BY TGL_TRANSAKSI DESC, ID_PENJUALAN DESC
            LIMIT :limit OFFSET :offset
        ");
        $stmt->bindValue(':tAwal',   $tAwal);
        $stmt->bindValue(':tAkhir',  $tAkhir);
        $stmt->bindValue(':lokasi',  $lokasi);
        $stmt->bindValue(':lokasi2', $lokasi);
        $stmt->bindValue(':sf',      $sf);
        $stmt->bindValue(':sf2',     $sf);
        $stmt->bindValue(':limit',   $limit,  PDO::PARAM_INT);
        $stmt->bindValue(':offset',  $offset, PDO::PARAM_INT);
        $stmt->execute();
        $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

        echo json_encode([
            'status'  => 'success',
            'summary' => $summary,
            'data'    => $rows,
            'count'   => count($rows),
        ]);
        exit;
    }

    // ── MODE DETAIL ───────────────────────────────────────────────
    if ($mode === 'detail') {
        if (empty($faktur)) {
            echo json_encode(['status' => 'error', 'message' => 'Faktur wajib diisi']);
            exit;
        }

        // Header penjualan
        $stmtH = $conn->prepare("
            SELECT
                ID_PENJUALAN, TGL_TRANSAKSI,
                ID_PELANGGAN, NAMA_PELANGGAN, ALAMAT_PELANGGAN, JENIS_PELANGGAN,
                LOKASIBARANG,
                GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_PERSEN, DISKON_TOTAL_RP,
                PAJAK_PERSEN, PAJAK_RP, GRAND_TOTAL_STL_PAJAK,
                LABA, BAYAR, NOMINAL_TRANSFER, TOTAL_HPP, BIAYA_KIRIM,
                KEMBALI, SISA_TAGIHAN, JATUH_TEMPO,
                STATUS_BAYAR, STATUS_TRANSAKSI,
                JENIS_PEMBAYARAN, KODE_AKUN,
                KODE_AKUN_TF, NAMA_AKUN_TF,
                METODE, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI,
                ID_SALES, NAMA_SALES,
                ID_USER, ID_KOMPUTER
            FROM penjualan
            WHERE ID_PENJUALAN = :faktur
            LIMIT 1
        ");
        $stmtH->execute([':faktur' => $faktur]);
        $header = $stmtH->fetch(PDO::FETCH_ASSOC);

        if (!$header) {
            echo json_encode(['status' => 'error', 'message' => 'Faktur tidak ditemukan']);
            exit;
        }

        // Items — kolom identik dengan VB detail penjualan
        $stmtI = $conn->prepare("
            SELECT
                ID_BARANG,
                NAMA_BARANG,
                QTY,
                SATUAN,
                HARGA_JUAL,
                QTY_SATUAN,
                TOTAL_DISKON,
                TOTAL_HARGA
            FROM penjualan_detail
            WHERE FAKTUR_JUAL = :faktur
            ORDER BY ID_BARANG ASC
        ");
        $stmtI->execute([':faktur' => $faktur]);
        $items = $stmtI->fetchAll(PDO::FETCH_ASSOC);

        echo json_encode([
            'status' => 'success',
            'header' => $header,
            'items'  => $items,
        ]);
        exit;
    }

    echo json_encode(['status' => 'error', 'message' => 'Mode tidak valid']);

} catch (Exception $e) {
    error_log("[GetRiwayat][EXCEPTION] " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Gagal: ' . $e->getMessage()]);
}
?>
