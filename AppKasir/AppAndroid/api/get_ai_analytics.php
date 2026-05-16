<?php
require_once 'db_connect.php';
require_once 'auth_check.php';

requireAuth($conn);

$type   = trim($_GET['type']   ?? '');
$lokasi = trim($_GET['lokasi'] ?? '');

if ($type === '') {
    echo json_encode(['status' => 'error', 'message' => 'Parameter type wajib diisi']);
    exit();
}

// ── Helper: format Rupiah ringkas (1.500.000 → "1,5jt") ──────────────
function formatRingkas(float $val): string {
    if ($val >= 1_000_000_000) return number_format($val / 1_000_000_000, 1, ',', '.') . 'M';
    if ($val >= 1_000_000)     return number_format($val / 1_000_000,     1, ',', '.') . 'jt';
    if ($val >= 1_000)         return number_format($val / 1_000,         0, ',', '.') . 'rb';
    return number_format($val, 0, ',', '.');
}

// ── Helper: kondisi WHERE lokasi ─────────────────────────────────────
// $alias kosong → kolom tanpa prefix (untuk query tanpa alias tabel)
function lokasiWhere(string $lokasi, string $alias = 'p'): string {
    if ($lokasi === '') return '';
    $col = $alias !== '' ? "{$alias}.LOKASIBARANG" : 'LOKASIBARANG';
    return "AND $col = '" . addslashes($lokasi) . "'";
}
function lokasiWhereOpname(string $lokasi, string $alias = 'so'): string {
    if ($lokasi === '') return '';
    $col = $alias !== '' ? "{$alias}.LOKASI" : 'LOKASI';
    return "AND $col = '" . addslashes($lokasi) . "'";
}

try {
    switch ($type) {

        // ── 1. PRODUK TERLARIS ────────────────────────────────────────
        case 'produk_terlaris': {
            $lokasiCond = lokasiWhere($lokasi);
            $stmt = $conn->prepare("
                SELECT
                    pd.ID_BARANG,
                    pd.NAMA_BARANG,
                    SUM(CASE WHEN p.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
                             THEN pd.QTY_SATUAN ELSE 0 END)                AS qty_7hari,
                    SUM(CASE WHEN p.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
                             THEN pd.TOTAL_HARGA ELSE 0 END)               AS omzet_7hari,
                    SUM(CASE WHEN p.TGL_TRANSAKSI BETWEEN DATE_SUB(CURDATE(), INTERVAL 14 DAY)
                                                      AND DATE_SUB(CURDATE(), INTERVAL 8 DAY)
                             THEN pd.QTY_SATUAN ELSE 0 END)                AS qty_7hari_lalu
                FROM penjualan_detail pd
                JOIN penjualan p ON p.ID_PENJUALAN = pd.FAKTUR_JUAL
                WHERE p.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 14 DAY)
                  $lokasiCond
                GROUP BY pd.ID_BARANG, pd.NAMA_BARANG
                HAVING qty_7hari > 0
                ORDER BY qty_7hari DESC
                LIMIT 10
            ");
            $stmt->execute();
            $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

            if (count($rows) === 0) {
                echo json_encode(['status' => 'success', 'data' => [], 'summary' => ['key_metric' => null, 'insight' => 'Data penjualan belum cukup (min. 1 hari)']]);
                exit();
            }

            // Hitung trend untuk setiap item
            foreach ($rows as &$r) {
                $qty7     = (float)$r['qty_7hari'];
                $qty7lalu = (float)$r['qty_7hari_lalu'];
                if ($qty7lalu > 0) {
                    $pct = round(($qty7 - $qty7lalu) / $qty7lalu * 100);
                    $r['trend'] = ($pct >= 0 ? '+' : '') . $pct . '%';
                } else {
                    $r['trend'] = $qty7 > 0 ? 'Baru' : '—';
                }
                $r['qty_7hari']   = (float)$r['qty_7hari'];
                $r['omzet_7hari'] = (float)$r['omzet_7hari'];
                unset($r['qty_7hari_lalu']);
            }
            unset($r);

            $top = $rows[0];
            echo json_encode([
                'status'  => 'success',
                'data'    => $rows,
                'summary' => [
                    'key_metric' => $top['NAMA_BARANG'],
                    'insight'    => number_format($top['qty_7hari'], 0, ',', '.') . ' qty · Rp ' . formatRingkas($top['omzet_7hari']) . ' (7 hari)',
                ],
            ]);
            break;
        }

        // ── 2. BARANG LAMBAT ──────────────────────────────────────────
        case 'barang_lambat': {
            $colStok    = ($lokasi === 'GUDANG') ? 'b.STOK_GUDANG' : 'b.STOK_TOKO';
            $colStokRaw = ($lokasi === 'GUDANG') ? 'STOK_GUDANG'   : 'STOK_TOKO';
            $lokasiCondSub = lokasiWhere($lokasi, 'p2');

            $stmt = $conn->prepare("
                SELECT
                    b.ID_BARANG,
                    b.NAMA_BARANG,
                    $colStok                                                     AS stok,
                    COALESCE(b.HARGA_BELI, 0)                                    AS harga_beli,
                    $colStok * COALESCE(b.HARGA_BELI, 0)                         AS nilai_tertahan,
                    last_sale.terakhir_terjual,
                    COALESCE(DATEDIFF(CURDATE(), last_sale.terakhir_terjual), 999) AS hari_tidak_terjual
                FROM tbl_barang b
                LEFT JOIN (
                    SELECT pd2.ID_BARANG, MAX(p2.TGL_TRANSAKSI) AS terakhir_terjual
                    FROM penjualan_detail pd2
                    JOIN penjualan p2 ON p2.ID_PENJUALAN = pd2.FAKTUR_JUAL
                    WHERE p2.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 365 DAY)
                      $lokasiCondSub
                    GROUP BY pd2.ID_BARANG
                ) last_sale ON last_sale.ID_BARANG = b.ID_BARANG
                WHERE b.$colStokRaw > 0
                HAVING hari_tidak_terjual > 30
                ORDER BY nilai_tertahan DESC
                LIMIT 20
            ");
            $stmt->execute();
            $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

            if (count($rows) === 0) {
                echo json_encode(['status' => 'success', 'data' => [], 'summary' => ['key_metric' => '0 item', 'insight' => 'Semua barang terjual dalam 30 hari terakhir 👍']]);
                exit();
            }

            $totalNilai = array_sum(array_column($rows, 'nilai_tertahan'));
            foreach ($rows as &$r) {
                $r['stok']          = (float)$r['stok'];
                $r['harga_beli']    = (float)$r['harga_beli'];
                $r['nilai_tertahan']= (float)$r['nilai_tertahan'];
                $r['hari_tidak_terjual'] = (int)$r['hari_tidak_terjual'];
            }
            unset($r);

            echo json_encode([
                'status'  => 'success',
                'data'    => $rows,
                'summary' => [
                    'key_metric' => count($rows) . ' item',
                    'insight'    => 'Nilai tertahan Rp ' . formatRingkas($totalNilai) . ' · Terparah: ' . $rows[0]['NAMA_BARANG'] . ' (' . $rows[0]['hari_tidak_terjual'] . ' hari)',
                ],
            ]);
            break;
        }

        // ── 3. REORDER ALERT ─────────────────────────────────────────
        case 'reorder_alert': {
            $colStok    = ($lokasi === 'GUDANG') ? 'b.STOK_GUDANG' : 'b.STOK_TOKO';
            $colStokRaw = ($lokasi === 'GUDANG') ? 'STOK_GUDANG'   : 'STOK_TOKO';
            $lokasiCondSub = lokasiWhere($lokasi, 'p2');

            $stmt = $conn->prepare("
                SELECT
                    b.ID_BARANG,
                    b.NAMA_BARANG,
                    $colStok                                                        AS stok_saat_ini,
                    COALESCE(sales.total_qty, 0) / 7                               AS rata_per_hari,
                    CASE
                        WHEN COALESCE(sales.total_qty, 0) / 7 > 0
                        THEN FLOOR($colStok / (COALESCE(sales.total_qty, 0) / 7))
                        ELSE 9999
                    END                                                             AS estimasi_hari_habis,
                    CASE
                        WHEN COALESCE(sales.total_qty, 0) / 7 > 0
                        THEN CEIL(COALESCE(sales.total_qty, 0) / 7 * 14)
                        ELSE 0
                    END                                                             AS saran_order_qty
                FROM tbl_barang b
                LEFT JOIN (
                    SELECT pd2.ID_BARANG, SUM(pd2.QTY_SATUAN) AS total_qty
                    FROM penjualan_detail pd2
                    JOIN penjualan p2 ON p2.ID_PENJUALAN = pd2.FAKTUR_JUAL
                    WHERE p2.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
                      $lokasiCondSub
                    GROUP BY pd2.ID_BARANG
                ) sales ON sales.ID_BARANG = b.ID_BARANG
                WHERE b.$colStokRaw > 0
                HAVING estimasi_hari_habis <= 7 AND rata_per_hari > 0
                ORDER BY estimasi_hari_habis ASC
                LIMIT 20
            ");
            $stmt->execute();
            $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

            if (count($rows) === 0) {
                echo json_encode(['status' => 'success', 'data' => [], 'summary' => ['key_metric' => '0 item', 'insight' => 'Stok semua barang aman untuk 7 hari ke depan 👍']]);
                exit();
            }

            foreach ($rows as &$r) {
                $r['stok_saat_ini']      = (float)$r['stok_saat_ini'];
                $r['rata_per_hari']      = round((float)$r['rata_per_hari'], 2);
                $r['estimasi_hari_habis']= (int)$r['estimasi_hari_habis'];
                $r['saran_order_qty']    = (int)$r['saran_order_qty'];
            }
            unset($r);

            $kritis = $rows[0];
            echo json_encode([
                'status'  => 'success',
                'data'    => $rows,
                'summary' => [
                    'key_metric' => count($rows) . ' item kritis',
                    'insight'    => 'Paling kritis: ' . $kritis['NAMA_BARANG'] . ' (habis ~' . $kritis['estimasi_hari_habis'] . ' hari lagi)',
                ],
            ]);
            break;
        }

        // ── 4. JAM PUNCAK ─────────────────────────────────────────────
        case 'jam_puncak': {
            // Query tanpa alias tabel — pakai lokasiWhere tanpa alias
            $lokasiCond = lokasiWhere($lokasi, '');

            $stmt = $conn->prepare("
                SELECT
                    HOUR(TGL_TRANSAKSI)                  AS jam,
                    COUNT(*)                             AS jumlah_transaksi,
                    COALESCE(SUM(GRAND_TOTAL_STL_PAJAK), 0) AS total_omzet
                FROM penjualan
                WHERE TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
                  $lokasiCond
                GROUP BY HOUR(TGL_TRANSAKSI)
                ORDER BY jam ASC
            ");
            $stmt->execute();
            $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

            if (count($rows) === 0) {
                echo json_encode(['status' => 'success', 'data' => [], 'summary' => ['key_metric' => null, 'insight' => 'Belum ada data transaksi 7 hari terakhir']]);
                exit();
            }

            // Isi jam yang kosong (07-22) dengan 0
            $byJam = [];
            foreach ($rows as $r) {
                $byJam[(int)$r['jam']] = [
                    'jam'               => (int)$r['jam'],
                    'jumlah_transaksi'  => (int)$r['jumlah_transaksi'],
                    'total_omzet'       => (float)$r['total_omzet'],
                ];
            }
            $full = [];
            for ($j = 7; $j <= 22; $j++) {
                $full[] = $byJam[$j] ?? ['jam' => $j, 'jumlah_transaksi' => 0, 'total_omzet' => 0.0];
            }

            // Cari jam puncak
            usort($rows, fn($a, $b) => (int)$b['jumlah_transaksi'] - (int)$a['jumlah_transaksi']);
            $puncak    = $rows[0];
            $jamPuncak = sprintf('%02d:00-%02d:00', $puncak['jam'], $puncak['jam'] + 1);

            // Rekomendasi sederhana
            $jam = (int)$puncak['jam'];
            if ($jam >= 7 && $jam <= 10)       $rek = 'Siapkan kasir tambahan pagi hari';
            elseif ($jam >= 11 && $jam <= 13)  $rek = 'Siapkan kasir tambahan jam makan siang';
            elseif ($jam >= 14 && $jam <= 17)  $rek = 'Siapkan kasir tambahan sore hari';
            else                               $rek = 'Siapkan kasir tambahan malam hari';

            echo json_encode([
                'status'  => 'success',
                'data'    => $full,
                'summary' => [
                    'key_metric'   => $jamPuncak,
                    'insight'      => (int)$puncak['jumlah_transaksi'] . ' transaksi · ' . $rek,
                    'rekomendasi'  => $rek,
                ],
            ]);
            break;
        }

        // ── 5. MARGIN PROFIT ─────────────────────────────────────────
        case 'margin_profit': {
            // Filter lokasi di penjualan_detail (lebih akurat — detail punya LOKASIBARANG sendiri)
            $lokasiCondPd = lokasiWhere($lokasi, 'pd');

            $stmt = $conn->prepare("
                SELECT
                    pd.ID_BARANG,
                    pd.NAMA_BARANG,
                    AVG(pd.HARGA_JUAL)  AS avg_harga_jual,
                    AVG(pd.HARGA_BELI)  AS avg_harga_beli,
                    AVG(
                        CASE WHEN pd.HARGA_JUAL > 0
                             THEN (pd.HARGA_JUAL - pd.HARGA_BELI) / pd.HARGA_JUAL * 100
                             ELSE 0 END
                    )                   AS margin_persen,
                    SUM(pd.TOTAL_HARGA) AS total_omzet,
                    SUM(pd.QTY_SATUAN)  AS total_qty
                FROM penjualan_detail pd
                WHERE pd.TANGGAL_JUAL >= DATE_SUB(CURDATE(), INTERVAL 30 DAY)
                  $lokasiCondPd
                  AND pd.HARGA_JUAL > 0
                GROUP BY pd.ID_BARANG, pd.NAMA_BARANG
                HAVING total_omzet > 0
                ORDER BY margin_persen DESC
            ");
            $stmt->execute();
            $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

            if (count($rows) < 2) {
                echo json_encode(['status' => 'success', 'data' => [], 'summary' => ['key_metric' => null, 'insight' => 'Data penjualan 30 hari belum cukup untuk analisis margin']]);
                exit();
            }

            foreach ($rows as &$r) {
                $r['avg_harga_jual'] = (float)$r['avg_harga_jual'];
                $r['avg_harga_beli'] = (float)$r['avg_harga_beli'];
                $r['margin_persen']  = round((float)$r['margin_persen'], 1);
                $r['total_omzet']    = (float)$r['total_omzet'];
                $r['total_qty']      = (float)$r['total_qty'];
            }
            unset($r);

            // Top 10 tertinggi + 10 terendah
            $top5    = array_slice($rows, 0, 10);
            $bottom5 = array_slice($rows, -10);
            $avgAll  = count($rows) > 0
                ? round(array_sum(array_column($rows, 'margin_persen')) / count($rows), 1)
                : 0;

            echo json_encode([
                'status'  => 'success',
                'data'    => ['top' => $top5, 'bottom' => $bottom5, 'all' => $rows],
                'summary' => [
                    'key_metric' => 'Avg ' . $avgAll . '%',
                    'insight'    => 'Tertinggi: ' . $top5[0]['NAMA_BARANG'] . ' (' . $top5[0]['margin_persen'] . '%) · Terendah: ' . end($bottom5)['NAMA_BARANG'] . ' (' . end($bottom5)['margin_persen'] . '%)',
                ],
            ]);
            break;
        }

        // ── 6. PELANGGAN AKTIF ────────────────────────────────────────
        case 'pelanggan_aktif': {
            $lokasiCond = lokasiWhere($lokasi);

            $stmt = $conn->prepare("
                SELECT
                    p.ID_PELANGGAN,
                    p.NAMA_PELANGGAN,
                    COUNT(*)                             AS frekuensi,
                    SUM(p.GRAND_TOTAL_STL_PAJAK)        AS total_belanja,
                    MAX(p.TGL_TRANSAKSI)                AS terakhir_beli,
                    DATEDIFF(CURDATE(), MAX(p.TGL_TRANSAKSI)) AS hari_sejak_beli
                FROM penjualan p
                WHERE p.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 90 DAY)
                  $lokasiCond
                  AND p.ID_PELANGGAN IS NOT NULL AND p.ID_PELANGGAN != ''
                GROUP BY p.ID_PELANGGAN, p.NAMA_PELANGGAN
                ORDER BY total_belanja DESC
                LIMIT 10
            ");
            $stmt->execute();
            $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

            // Hitung total pelanggan aktif (90 hari)
            $lokasiCondCount = lokasiWhere($lokasi, 'p');
            $stmtCount = $conn->prepare("
                SELECT COUNT(DISTINCT p.ID_PELANGGAN) AS total
                FROM penjualan p
                WHERE p.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 90 DAY)
                  $lokasiCondCount
                  AND p.ID_PELANGGAN IS NOT NULL AND p.ID_PELANGGAN != ''
            ");
            $stmtCount->execute();
            $totalAktif = (int)$stmtCount->fetchColumn();

            // Pelanggan baru bulan ini — pakai LEFT JOIN bukan NOT IN (lebih cepat)
            $lokasiCondBaru = lokasiWhere($lokasi, 'p');
            $stmtBaru = $conn->prepare("
                SELECT COUNT(DISTINCT p.ID_PELANGGAN) AS total
                FROM penjualan p
                LEFT JOIN penjualan p_lama
                    ON p_lama.ID_PELANGGAN = p.ID_PELANGGAN
                    AND p_lama.TGL_TRANSAKSI < DATE_FORMAT(CURDATE(), '%Y-%m-01')
                    AND p_lama.ID_PELANGGAN IS NOT NULL AND p_lama.ID_PELANGGAN != ''
                WHERE p.TGL_TRANSAKSI >= DATE_FORMAT(CURDATE(), '%Y-%m-01')
                  $lokasiCondBaru
                  AND p.ID_PELANGGAN IS NOT NULL AND p.ID_PELANGGAN != ''
                  AND p_lama.ID_PELANGGAN IS NULL
            ");
            $stmtBaru->execute();
            $pelangganBaru = (int)$stmtBaru->fetchColumn();

            if (count($rows) === 0) {
                echo json_encode(['status' => 'success', 'data' => [], 'summary' => ['key_metric' => '0 pelanggan', 'insight' => 'Belum ada transaksi dengan pelanggan terdaftar']]);
                exit();
            }

            // Tentukan badge berdasarkan frekuensi + total belanja
            $maxBelanja = (float)$rows[0]['total_belanja'];
            foreach ($rows as &$r) {
                $r['frekuensi']     = (int)$r['frekuensi'];
                $r['total_belanja'] = (float)$r['total_belanja'];
                $r['hari_sejak_beli'] = (int)$r['hari_sejak_beli'];
                // Badge: VIP jika top 20% belanja, Baru jika hari_sejak_beli < 30, lainnya Reguler
                $pct = $maxBelanja > 0 ? $r['total_belanja'] / $maxBelanja : 0;
                if ($pct >= 0.8)                    $r['badge'] = 'VIP';
                elseif ($r['hari_sejak_beli'] <= 30) $r['badge'] = 'Baru';
                else                                 $r['badge'] = 'Reguler';
            }
            unset($r);

            $top = $rows[0];
            echo json_encode([
                'status'  => 'success',
                'data'    => $rows,
                'summary' => [
                    'key_metric' => $totalAktif . ' pelanggan aktif',
                    'insight'    => 'Top: ' . $top['NAMA_PELANGGAN'] . ' (Rp ' . formatRingkas($top['total_belanja']) . ') · ' . $pelangganBaru . ' pelanggan baru bulan ini',
                    'total_aktif'  => $totalAktif,
                    'pelanggan_baru' => $pelangganBaru,
                ],
            ]);
            break;
        }

        default:
            echo json_encode(['status' => 'error', 'message' => 'Type tidak dikenal: ' . htmlspecialchars($type)]);
    }

} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => 'Gagal memproses AI Analytics: ' . $e->getMessage()]);
}
?>
