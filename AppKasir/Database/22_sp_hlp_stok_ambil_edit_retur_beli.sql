-- ============================================================
-- SP: sp_hlp_stok_ambil_edit_retur_beli
-- Tujuan: Ambil stok EFEKTIF untuk mode edit retur pembelian.
--
-- Masalah yang diselesaikan:
--   Saat edit retur pembelian, stok di tbl_barang sudah bertambah
--   oleh retur yang sedang diedit. Jika ditampilkan apa adanya,
--   user melihat stok yang lebih besar dari yang sebenarnya tersedia.
--
--   Contoh:
--     Stok awal = 90, retur lama 10 → STOK_TOKO = 100
--     User buka edit → tampil 100 (tidak akurat)
--     Stok efektif = 100 - 10 = 90 (yang benar untuk konteks edit)
--
-- Cara kerja:
--   Stok efektif = STOK_TOKO/GUDANG - SUM(QTY_SAT di retur ini)
--   Retur lama akan dihapus dan disimpan ulang saat simpan,
--   sehingga qty di retur ini "dikurangi" dari stok.
--
-- Perbedaan dari sp_hlp_stok_ambil_edit:
--   sp_hlp_stok_ambil_edit            → untuk mode EDIT penjualan (tambah stok efektif)
--   sp_hlp_stok_ambil_edit_retur_beli → untuk mode EDIT retur pembelian (kurangi stok efektif)
--
-- Catatan:
--   Lokasi diambil dari pembelian asli (pembelian.LOKASI) via
--   retur_pembelian.ID_PEMBELIAN, karena retur_pembelian_detail
--   tidak memiliki kolom lokasi.
-- ============================================================

DROP PROCEDURE IF EXISTS sp_hlp_stok_ambil_edit_retur_beli;

DELIMITER $$

CREATE PROCEDURE sp_hlp_stok_ambil_edit_retur_beli(
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
    DECLARE v_lokasi_faktur VARCHAR(20) DEFAULT '';

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

    -- Ambil lokasi dari pembelian asli
    SELECT COALESCE(LOKASI, '')
    INTO v_lokasi_faktur
    FROM retur_pembelian r
    INNER JOIN pembelian p ON r.ID_PEMBELIAN = p.ID_PEMBELIAN
    WHERE r.ID_RETUR_PEMBELIAN = p_faktur_lama
    LIMIT 1;

    -- Ambil qty yang sudah diretur di faktur lama (akan dikurangi saat simpan)
    SELECT COALESCE(SUM(QTY_SAT), 0)
    INTO v_qty_lama
    FROM retur_pembelian_detail
    WHERE ID_RETUR_PEMBELIAN = p_faktur_lama
      AND ID_BARANG   = p_kode_barang;

    -- Stok efektif = stok DB - qty yang akan dikurangi
    -- Hanya kurangi jika lokasi faktur sama dengan lokasi yang dicek
    IF v_lokasi_faktur = p_lokasi THEN
        SET p_stok_toko   = IF(p_lokasi = 'TOKO',   v_stok_toko   - v_qty_lama, v_stok_toko);
        SET p_stok_gudang = IF(p_lokasi = 'GUDANG',  v_stok_gudang - v_qty_lama, v_stok_gudang);
    ELSE
        SET p_stok_toko   = v_stok_toko;
        SET p_stok_gudang = v_stok_gudang;
    END IF;
    SET p_nama_barang = v_nama;
END$$

DELIMITER ;

-- ============================================================
-- Cara pakai di VB.NET (RefreshStokBaris mode edit retur pembelian):
--
-- Using cmd As New MySqlCommand(
--     "CALL sp_hlp_stok_ambil_edit_retur_beli(@kode, @faktur, @lokasi, @toko, @gudang, @nama)", conn)
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
