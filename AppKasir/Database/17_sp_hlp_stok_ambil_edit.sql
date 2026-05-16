-- ============================================================
-- SP: sp_hlp_stok_ambil_edit
-- Tujuan: Ambil stok EFEKTIF untuk mode edit transaksi.
--
-- Masalah yang diselesaikan:
--   Saat edit penjualan, stok di tbl_barang sudah dikurangi
--   oleh faktur yang sedang diedit. Jika ditampilkan apa adanya,
--   user melihat stok yang lebih kecil dari yang sebenarnya tersedia.
--
--   Contoh:
--     Stok awal = 100, faktur lama jual 10 → STOK_TOKO = 90
--     User buka edit → tampil 90 (tidak akurat)
--     Stok efektif = 90 + 10 = 100 (yang benar untuk konteks edit)
--
-- Cara kerja:
--   Stok efektif = STOK_TOKO/GUDANG + SUM(QTY_SATUAN di faktur ini)
--   Faktur lama akan dihapus dan disimpan ulang saat simpan,
--   sehingga qty di faktur ini "dikembalikan" ke stok.
--
-- Perbedaan dari sp_hlp_stok_ambil:
--   sp_hlp_stok_ambil       → untuk mode TAMBAH (stok DB apa adanya)
--   sp_hlp_stok_ambil_edit  → untuk mode EDIT (stok DB + qty di faktur lama)
--
-- Kapan dipakai di VB.NET:
--   RefreshStokBaris() / RefreshStokSemuaBaris() saat IsModeTambahPenjualan = False
-- ============================================================

DROP PROCEDURE IF EXISTS sp_hlp_stok_ambil_edit;

DELIMITER $$

CREATE PROCEDURE sp_hlp_stok_ambil_edit(
    IN  p_kode_barang  VARCHAR(50),
    IN  p_faktur_lama  VARCHAR(30),
    IN  p_lokasi       VARCHAR(10),   -- 'TOKO' atau 'GUDANG'
    OUT p_stok_toko    DECIMAL(10,2),
    OUT p_stok_gudang  DECIMAL(10,2),
    OUT p_nama_barang  VARCHAR(200)
)
BEGIN
    DECLARE v_stok_toko   DECIMAL(10,2) DEFAULT 0;
    DECLARE v_stok_gudang DECIMAL(10,2) DEFAULT 0;
    DECLARE v_nama        VARCHAR(200)  DEFAULT '';
    DECLARE v_qty_lama    DECIMAL(10,2) DEFAULT 0;

    SET p_stok_toko   = 0;
    SET p_stok_gudang = 0;
    SET p_nama_barang = '';

    -- Ambil stok saat ini dari tbl_barang
    SELECT
        COALESCE(STOK_TOKO,   0),
        COALESCE(STOK_GUDANG, 0),
        COALESCE(NAMA_BARANG, '')
    INTO
        v_stok_toko,
        v_stok_gudang,
        v_nama
    FROM tbl_barang
    WHERE ID_BARANG = p_kode_barang
    LIMIT 1;

    -- Ambil qty yang sudah terjual di faktur lama (akan dikembalikan saat simpan)
    SELECT COALESCE(SUM(QTY_SATUAN), 0)
    INTO v_qty_lama
    FROM penjualan_detail
    WHERE FAKTUR_JUAL = p_faktur_lama
      AND ID_BARANG   = p_kode_barang
      AND LOKASIBARANG = p_lokasi;

    -- Stok efektif = stok DB + qty yang akan dikembalikan
    SET p_stok_toko   = IF(p_lokasi = 'TOKO',   v_stok_toko   + v_qty_lama, v_stok_toko);
    SET p_stok_gudang = IF(p_lokasi = 'GUDANG',  v_stok_gudang + v_qty_lama, v_stok_gudang);
    SET p_nama_barang = v_nama;
END$$

DELIMITER ;

-- ============================================================
-- Cara pakai di VB.NET (RefreshStokBaris mode edit):
--
-- Using cmd As New MySqlCommand(
--     "CALL sp_hlp_stok_ambil_edit(@kode, @faktur, @lokasi, @toko, @gudang, @nama)", conn)
--     cmd.Parameters.AddWithValue("@kode",   kodeBarang)
--     cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
--     cmd.Parameters.AddWithValue("@lokasi", LblLokasiBarang.Text)
--     Dim pToko   = cmd.Parameters.Add("@toko",   MySqlDbType.Decimal) : pToko.Direction   = ParameterDirection.Output
--     Dim pGudang = cmd.Parameters.Add("@gudang", MySqlDbType.Decimal) : pGudang.Direction = ParameterDirection.Output
--     Dim pNama   = cmd.Parameters.Add("@nama",   MySqlDbType.VarChar, 200) : pNama.Direction = ParameterDirection.Output
--     cmd.ExecuteNonQuery()
--     Dim stokToko   As Decimal = ModuleAngka.ParseDecimal(pToko.Value)
--     Dim stokGudang As Decimal = ModuleAngka.ParseDecimal(pGudang.Value)
-- End Using
-- ============================================================
