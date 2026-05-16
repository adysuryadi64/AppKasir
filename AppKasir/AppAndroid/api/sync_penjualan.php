<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

// ── Helper log ────────────────────────────────────────────────────────────
function _log(string $tag, string $msg): void {
    error_log("[Penjualan][$tag] $msg");
}

$raw  = file_get_contents('php://input');
$data = json_decode($raw, true) ?? [];

_log('INPUT', 'raw=' . substr($raw, 0, 300));

// ── Baca parameter header dari payload ───────────────────────────────────
$tgl_transaksi    = !empty($data['tgl_transaksi'])
                        ? date('Y-m-d H:i:s', strtotime($data['tgl_transaksi']))
                        : date('Y-m-d H:i:s');
$lokasi           = $data['lokasi'];
$id_user          = $data['id_user'];
$id_komputer      = $data['id_komputer'];

$id_pelanggan     = $data['id_pelanggan']     ?? '';
$nama_pelanggan   = $data['nama_pelanggan']   ?? '';
$alamat_pelanggan = $data['alamat_pelanggan'] ?? '';
$jenis_pelanggan  = $data['jenis_pelanggan']  ?? 'UMUM';

$grand_total_sbl_pajak = floatval($data['grand_total_sbl_pajak'] ?? 0);
$diskon_total_persen   = floatval($data['diskon_total_persen']   ?? 0);
$diskon_total_rp       = floatval($data['diskon_total_rp']       ?? 0);
$pajak_persen          = floatval($data['pajak_persen']          ?? 0);
$pajak_rp              = floatval($data['pajak_rp']              ?? 0);
$grand_total           = floatval($data['grand_total_stl_pajak'] ?? 0);
$total_hpp             = floatval($data['total_hpp']             ?? 0);
$laba                  = floatval($data['laba']                  ?? 0);
$bayar                 = floatval($data['bayar']                 ?? 0);
$nominal_transfer      = floatval($data['nominal_transfer']      ?? 0);
$biaya_kirim           = floatval($data['biaya_kirim']           ?? 0);
$kembali               = floatval($data['kembali']               ?? 0);
$sisa_tagihan          = floatval($data['sisa_tagihan']          ?? 0);
$status_bayar          = $data['status_bayar']    ?? 'TERBAYAR';
$jatuh_tempo           = !empty($data['jatuh_tempo'])
                            ? date('Y-m-d H:i:s', strtotime($data['jatuh_tempo']))
                            : date('Y-m-d H:i:s', strtotime('+30 days'));

// Parameter hak akses dari payload Flutter
$izinkan_stok_minus = intval($data['izinkan_stok_minus'] ?? 0);
$izinkan_backdate   = intval($data['izinkan_backdate']   ?? 0);

// ID draft (jika dari penjualan_ditahan)
$id_draft = $data['id_draft'] ?? '';

// Akun transfer dari payload (user pilih di Flutter)
$kode_akun_transfer = $data['kode_akun_transfer'] ?? '';
$nama_akun_transfer = $data['nama_akun_transfer'] ?? 'BANK';

// ── Ambil kode akun dari tbl_perusahaan ──────────────────────────────────
try {
    $stmt_co = $conn->query("SELECT
        KODE_REK_JUAL_TOKO,    NAMA_REK_JUAL_TOKO,
        KODE_REK_JUAL_GUDANG,  NAMA_REK_JUAL_GUDANG,
        KODE_REK_PIUTANG_JUAL, NAMA_REK_PIUTANG_JUAL,
        KODE_REK_BARANG,       NAMA_REK_BARANG
    FROM tbl_perusahaan LIMIT 1");
    $co = $stmt_co->fetch(PDO::FETCH_ASSOC) ?: [];
} catch (Exception $e) {
    $co = [];
}

// Akun kas sesuai lokasi
if ($lokasi === 'TOKO') {
    $kode_akun_kas = $co['KODE_REK_JUAL_TOKO']   ?? '01.01.001';
    $nama_akun_kas = $co['NAMA_REK_JUAL_TOKO']   ?? 'KAS TOKO';
} else {
    $kode_akun_kas = $co['KODE_REK_JUAL_GUDANG'] ?? '01.01.002';
    $nama_akun_kas = $co['NAMA_REK_JUAL_GUDANG'] ?? 'KAS GUDANG';
}

// Akun piutang
$kode_rek_piutang = $co['KODE_REK_PIUTANG_JUAL'] ?? '03.01.001';
$nama_rek_piutang = $co['NAMA_REK_PIUTANG_JUAL'] ?? 'PIUTANG USAHA';

// Akun persediaan barang (dipakai di tmp_penjualan_items per item jika item tidak punya kode sendiri)
$kode_rek_barang_default = $co['KODE_REK_BARANG'] ?? '01.03.001';
$nama_rek_barang_default = $co['NAMA_REK_BARANG'] ?? 'PERSEDIAAN BARANG';

try {
    _log('PARAM', "lokasi=$lokasi | id_user=$id_user | tgl=$tgl_transaksi | items=" . count($data['items'] ?? []) . " | grand_total=$grand_total | bayar=$bayar");
    // ── 1. Buat temporary table ───────────────────────────────────────────
    $conn->exec("CREATE TEMPORARY TABLE IF NOT EXISTS tmp_penjualan_items (
        ID_BARANG        VARCHAR(50),
        NAMA_BARANG      VARCHAR(200),
        HARGA_BELI       DECIMAL(15,2),
        HARGA_JUAL       DECIMAL(15,2),
        QTY              DECIMAL(15,4),
        SATUAN           VARCHAR(50),
        ISI_SATUAN       DECIMAL(15,4),
        QTY_SATUAN       DECIMAL(15,4),
        TOTAL_HARGA      DECIMAL(15,2),
        DISKON_PERSEN    DECIMAL(5,2),
        DISKON_RP        DECIMAL(15,2),
        TOTAL_DISKON     DECIMAL(15,2),
        LABA             DECIMAL(15,2),
        SERIAL_NUMBER    VARCHAR(100),
        KODE_REK_BARANG  VARCHAR(20),
        NAMA_REK_BARANG  VARCHAR(50),
        TOTAL_HARGA_BELI DECIMAL(15,2),
        KODE_REK_JUAL    VARCHAR(20),
        NAMA_REK_JUAL    VARCHAR(50)
    )");

    // Pastikan kosong (jika koneksi di-reuse)
    $conn->exec("DELETE FROM tmp_penjualan_items");

    // ── 2. INSERT items dari payload ke tmp table ─────────────────────────
    $stmt_item = $conn->prepare("INSERT INTO tmp_penjualan_items (
        ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_JUAL,
        QTY, SATUAN, ISI_SATUAN, QTY_SATUAN,
        TOTAL_HARGA, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON,
        LABA, SERIAL_NUMBER,
        KODE_REK_BARANG, NAMA_REK_BARANG,
        TOTAL_HARGA_BELI, KODE_REK_JUAL, NAMA_REK_JUAL
    ) VALUES (
        :id_barang, :nama_barang, :harga_beli, :harga_jual,
        :qty, :satuan, :isi_satuan, :qty_satuan,
        :total_harga, :diskon_persen, :diskon_rp, :total_diskon,
        :laba, :serial_number,
        :kode_rek_barang, :nama_rek_barang,
        :total_harga_beli, :kode_rek_jual, :nama_rek_jual
    )");

    foreach ($data['items'] as $item) {
        $qty_satuan      = floatval($item['qty_satuan'] ?? (floatval($item['qty'] ?? 0) * floatval($item['isi_satuan'] ?? 1)));
        $harga_beli      = floatval($item['harga_beli']  ?? 0);
        $total_harga_beli = $harga_beli * $qty_satuan;

        // Kode rekening barang per item (fallback ke default perusahaan)
        $kode_rek_brg = $item['kode_rek_barang'] ?? $kode_rek_barang_default;
        $nama_rek_brg = $item['nama_rek_barang'] ?? $nama_rek_barang_default;

        $stmt_item->execute([
            ':id_barang'       => $item['id_barang'],
            ':nama_barang'     => $item['nama_barang'],
            ':harga_beli'      => $harga_beli,
            ':harga_jual'      => floatval($item['harga_jual']    ?? 0),
            ':qty'             => floatval($item['qty']           ?? 0),
            ':satuan'          => $item['satuan']                 ?? '',
            ':isi_satuan'      => floatval($item['isi_satuan']    ?? 1),
            ':qty_satuan'      => $qty_satuan,
            ':total_harga'     => floatval($item['total_harga']   ?? 0),
            ':diskon_persen'   => floatval($item['diskon_persen'] ?? 0),
            ':diskon_rp'       => floatval($item['diskon_rp']     ?? 0),
            ':total_diskon'    => floatval($item['total_diskon']  ?? 0),
            ':laba'            => floatval($item['laba']          ?? 0),
            ':serial_number'   => $item['serial_number']          ?? '',
            ':kode_rek_barang' => $kode_rek_brg,
            ':nama_rek_barang' => $nama_rek_brg,
            ':total_harga_beli' => $total_harga_beli,
            ':kode_rek_jual'   => '',
            ':nama_rek_jual'   => '',
        ]);
    }

    // ── 3. Set semua IN parameter sebagai user variables ──────────────────
    $conn->exec("SET
        @p_id_penjualan          = '',
        @p_id_pelanggan          = " . $conn->quote($id_pelanggan)          . ",
        @p_nama_pelanggan        = " . $conn->quote($nama_pelanggan)        . ",
        @p_alamat_pelanggan      = " . $conn->quote($alamat_pelanggan)      . ",
        @p_jenis_pelanggan       = " . $conn->quote($jenis_pelanggan)       . ",
        @p_lokasi                = " . $conn->quote($lokasi)                . ",
        @p_tgl_transaksi         = " . $conn->quote($tgl_transaksi)         . ",
        @p_grand_total_sbl_pajak = " . floatval($grand_total_sbl_pajak)     . ",
        @p_diskon_total_persen   = " . floatval($diskon_total_persen)       . ",
        @p_diskon_total_rp       = " . floatval($diskon_total_rp)           . ",
        @p_pajak_persen          = " . floatval($pajak_persen)              . ",
        @p_pajak_rp              = " . floatval($pajak_rp)                  . ",
        @p_grand_total           = " . floatval($grand_total)               . ",
        @p_total_hpp             = " . floatval($total_hpp)                 . ",
        @p_laba                  = " . floatval($laba)                      . ",
        @p_bayar                 = " . floatval($bayar)                     . ",
        @p_nominal_transfer      = " . floatval($nominal_transfer)          . ",
        @p_biaya_kirim           = " . floatval($biaya_kirim)               . ",
        @p_kembali               = " . floatval($kembali)                   . ",
        @p_sisa_tagihan          = " . floatval($sisa_tagihan)              . ",
        @p_jatuh_tempo           = " . $conn->quote($jatuh_tempo)           . ",
        @p_status_bayar          = " . $conn->quote($status_bayar)          . ",
        @p_kode_akun_kas         = " . $conn->quote($kode_akun_kas)         . ",
        @p_nama_akun_kas         = " . $conn->quote($nama_akun_kas)         . ",
        @p_kode_akun_transfer    = " . $conn->quote($kode_akun_transfer)    . ",
        @p_nama_akun_transfer    = " . $conn->quote($nama_akun_transfer)    . ",
        @p_kode_rek_piutang      = " . $conn->quote($kode_rek_piutang)      . ",
        @p_nama_rek_piutang      = " . $conn->quote($nama_rek_piutang)      . ",
        @p_id_draft              = " . $conn->quote($id_draft)              . ",
        @p_id_user               = " . $conn->quote($id_user)               . ",
        @p_id_komputer           = " . $conn->quote($id_komputer)           . ",
        @p_izinkan_stok_minus    = " . intval($izinkan_stok_minus)          . ",
        @p_izinkan_backdate      = " . intval($izinkan_backdate)
    );

    // ── 4. CALL SP ────────────────────────────────────────────────────────
    _log('SP', 'CALL sp_trx_penjualan_simpan...');
    $conn->exec("CALL sp_trx_penjualan_simpan(
        @p_id_penjualan,
        @p_id_pelanggan, @p_nama_pelanggan, @p_alamat_pelanggan, @p_jenis_pelanggan,
        @p_lokasi, @p_tgl_transaksi,
        @p_grand_total_sbl_pajak, @p_diskon_total_persen, @p_diskon_total_rp,
        @p_pajak_persen, @p_pajak_rp, @p_grand_total,
        @p_total_hpp, @p_laba,
        @p_bayar, @p_nominal_transfer, @p_biaya_kirim, @p_kembali, @p_sisa_tagihan,
        @p_jatuh_tempo, @p_status_bayar,
        @p_kode_akun_kas, @p_nama_akun_kas,
        @p_kode_akun_transfer, @p_nama_akun_transfer,
        @p_kode_rek_piutang, @p_nama_rek_piutang,
        @p_id_user, @p_id_komputer,
        @p_izinkan_stok_minus, @p_izinkan_backdate,
        @p_id_draft,
        @p_success, @p_error_code, @p_error_message, @p_id_penjualan_out
    )");

    // ── 5. Baca OUT parameter ─────────────────────────────────────────────
    $row = $conn->query("SELECT
        @p_success          AS success,
        @p_error_code       AS error_code,
        @p_error_message    AS error_message,
        @p_id_penjualan_out AS id_penjualan
    ")->fetch(PDO::FETCH_ASSOC);

    _log('SP', "OUT: success={$row['success']} | error_code={$row['error_code']} | message={$row['error_message']} | id_penjualan={$row['id_penjualan']}");

    // ── 6. DROP TEMPORARY TABLE ───────────────────────────────────────────
    $conn->exec("DROP TEMPORARY TABLE IF EXISTS tmp_penjualan_items");

    // ── 7. Response JSON ──────────────────────────────────────────────────
    if (intval($row['success']) === 1) {
        _log('OK', "id_penjualan={$row['id_penjualan']}");
        echo json_encode([
            'status'       => 'success',
            'id_penjualan' => $row['id_penjualan'],
        ]);
    } else {
        _log('FAIL', "error_code={$row['error_code']} | message={$row['error_message']}");
        echo json_encode([
            'status'     => 'error',
            'error_code' => $row['error_code']    ?? 'UNKNOWN',
            'message'    => $row['error_message'] ?? 'Terjadi kesalahan',
        ]);
    }

} catch (Exception $e) {
    // Cleanup tmp table jika ada error sebelum DROP
    try { $conn->exec("DROP TEMPORARY TABLE IF EXISTS tmp_penjualan_items"); } catch (Exception $_) {}

    _log('EXCEPTION', $e->getMessage() . ' | ' . $e->getTraceAsString());
    echo json_encode([
        'status'     => 'error',
        'error_code' => 'SQL_ERROR',
        'message'    => 'Gagal menyimpan transaksi: ' . $e->getMessage(),
    ]);
}
?>
