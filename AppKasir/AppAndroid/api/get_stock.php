<?php
require_once 'db_connect.php';
require_once 'no_browser.php';

function _log(string $tag, string $msg): void { error_log("[GetStock][$tag] $msg"); }

$search = isset($_GET['search']) ? trim($_GET['search']) : '';
$limit  = isset($_GET['limit'])  ? max(1, min((int)$_GET['limit'], 200)) : 50;
$offset = isset($_GET['offset']) ? max(0, (int)$_GET['offset']) : 0;

try {
    /*
     * Kolom dipilih berdasarkan kebutuhan nyata di aplikasi mobile:
     *
     * Penjualan  : identitas, barcode, satuan+isi+harga (umum & partai),
     *              harga beli, STOK_TOKO (stok aktif lokasi toko)
     * Stok Opname: identitas, STOK_TOKO + STOK_GUDANG (nilai akhir stok)
     * Kategori/Merk: JOIN ke tabel master, fallback ke kolom denormalized
     *
     * Kolom yang TIDAK diambil (tidak dipakai di mobile):
     *   ID_BARANG_BANTU, NAMA_BARANG_BANTU, JENIS, KODE_SUPLIYER/MERK,
     *   semua kolom mutasi stok (TAMBAH/KURANG/PEMBELIAN/RETUR/TRANSFER/OPNAME),
     *   STOK_AWAL, STOK_MIN, STOK_MAX, LOKASI_RAK, POINT, KOMISI
     */
    $sql = "
        SELECT
            -- ── Identitas ─────────────────────────────────────────────
            b.ID_BARANG,
            b.NAMA_BARANG,
            b.STATUS,

            -- ── Kategori & Merk (JOIN → fallback denormalized) ────────
            b.KODE_KATEGORI,
            COALESCE(k.NAMA, b.NAMA_KATEGORI, '') AS Kategori,
            COALESCE(m.NAMA, b.NAMA_MERK,     '') AS Merk,

            -- ── Harga beli ────────────────────────────────────────────
            b.HARGA_BELI,

            -- ── Barcode ───────────────────────────────────────────────
            b.BARCODE_KECIL,
            b.BARCODE_SEDANG,
            b.BARCODE_BESAR,

            -- ── Satuan & isi — UMUM ───────────────────────────────────
            b.SATUAN_UMUM_KECIL,
            b.SATUAN_UMUM_SEDANG,
            b.SATUAN_UMUM_BESAR,
            b.ISI_UMUM_KECIL,
            b.ISI_UMUM_SEDANG,
            b.ISI_UMUM_BESAR,

            -- ── Harga jual — UMUM ─────────────────────────────────────
            b.HARGA_JUAL_UMUM_KECIL,
            b.HARGA_JUAL_UMUM_SEDANG,
            b.HARGA_JUAL_UMUM_BESAR,

            -- ── Satuan & isi — PARTAI ─────────────────────────────────
            b.SATUAN_PARTAI_KECIL,
            b.SATUAN_PARTAI_SEDANG,
            b.SATUAN_PARTAI_BESAR,
            b.ISI_PARTAI_KECIL,
            b.ISI_PARTAI_SEDANG,
            b.ISI_PARTAI_BESAR,

            -- ── Harga jual — PARTAI ───────────────────────────────────
            b.HARGA_JUAL_PARTAI_KECIL,
            b.HARGA_JUAL_PARTAI_SEDANG,
            b.HARGA_JUAL_PARTAI_BESAR,

            -- ── Stok akhir (nilai akhir saja) ─────────────────────────
            b.STOK_TOKO,
            b.STOK_GUDANG,

            -- ── Alias pendek untuk Flutter ────────────────────────────
            b.STOK_TOKO             AS STOK_AKHIR,
            b.HARGA_JUAL_UMUM_KECIL AS HARGA_JUAL,
            b.SATUAN_UMUM_KECIL     AS SATUAN,
            b.ISI_UMUM_KECIL        AS ISI_SATUAN

        FROM tbl_barang b
        LEFT JOIN tbl_kategori k ON k.KODE = b.KODE_KATEGORI
        LEFT JOIN tbl_merk     m ON m.KODE = b.KODE_MERK
    ";

    $params = [];

    if ($search !== '') {
        $sql .= "
        WHERE (
            b.ID_BARANG         LIKE :search_prefix
            OR b.BARCODE_KECIL  LIKE :search_prefix
            OR b.BARCODE_SEDANG LIKE :search_prefix
            OR b.BARCODE_BESAR  LIKE :search_prefix
            OR b.KODE_KATEGORI  LIKE :search_prefix
            OR b.NAMA_BARANG    LIKE :search_any
            OR k.NAMA           LIKE :search_any
            OR m.NAMA           LIKE :search_any
        )";
        $params[':search_prefix'] = "$search%";
        $params[':search_any']    = "%$search%";
    }

    $sql .= " ORDER BY b.NAMA_BARANG ASC LIMIT :limit OFFSET :offset";

    $stmt = $conn->prepare($sql);

    foreach ($params as $key => &$val) {
        $stmt->bindParam($key, $val, PDO::PARAM_STR);
    }
    unset($val);

    $stmt->bindParam(':limit',  $limit,  PDO::PARAM_INT);
    $stmt->bindParam(':offset', $offset, PDO::PARAM_INT);

    $stmt->execute();
    $data = $stmt->fetchAll(PDO::FETCH_ASSOC);

    echo json_encode(["status" => "success", "data" => $data, "count" => count($data)]);

} catch (Exception $e) {
    error_log("[GetStock][EXCEPTION] " . $e->getMessage());
    echo json_encode([
        "status"  => "error",
        "message" => "Gagal mengambil data stok: " . $e->getMessage(),
    ]);
}
?>
