-- ============================================================
-- SP: sp_hlp_stok_ambil
-- Tujuan: Ambil informasi stok barang untuk ditampilkan di form
--         transaksi (penjualan, pembelian, retur, transfer, dll).
--
-- Alur pemakaian di VB.NET:
--   1. Saat barang dipilih dari ListView → panggil SP ini untuk
--      tampil info stok di kolom StokToko/StokGudang/Stok
--   2. Klik kanan "Refresh Stok" di DGV → panggil SP ini untuk
--      update info stok baris yang dipilih
--   3. Saat load edit/draft → AmbilInfoStok() otomatis pilih SP
--      yang tepat berdasarkan mode:
--        IsModeTambahPenjualan = True  → sp_hlp_stok_ambil (SP ini)
--        IsModeTambahPenjualan = False → sp_hlp_stok_ambil_edit
--   4. Validasi stok sebelum simpan → tetap pakai sp_hlp_stok_validasi
--      (bukan SP ini, karena validasi butuh FOR UPDATE)
--
-- Catatan:
--   - SP ini hanya SELECT, tidak mengubah data apapun
--   - Ringan dan aman dipanggil berkali-kali
--   - sp_hlp_stok_hitung (yang sudah ada) dipakai untuk RECALCULATE
--     stok dari komponen — berbeda fungsi, jangan dicampur
--   - Wajib diinstall di db_kasirlancar DAN db_moroseneng
-- ============================================================

DROP PROCEDURE IF EXISTS sp_hlp_stok_ambil;

DELIMITER $$

CREATE PROCEDURE sp_hlp_stok_ambil(
    IN  p_kode_barang  VARCHAR(50),
    OUT p_stok_toko    DECIMAL(10,2),
    OUT p_stok_gudang  DECIMAL(10,2),
    OUT p_nama_barang  VARCHAR(200)
)
BEGIN
    SET p_stok_toko   = 0;
    SET p_stok_gudang = 0;
    SET p_nama_barang = '';

    SELECT
        COALESCE(STOK_TOKO,   0),
        COALESCE(STOK_GUDANG, 0),
        COALESCE(NAMA_BARANG, '')
    INTO
        p_stok_toko,
        p_stok_gudang,
        p_nama_barang
    FROM tbl_barang
    WHERE ID_BARANG = p_kode_barang
    LIMIT 1;
END$$

DELIMITER ;

-- ============================================================
-- Cara pakai di VB.NET:
--
-- Using cmd As New MySqlCommand(
--     "CALL sp_hlp_stok_ambil(@kode, @toko, @gudang, @nama)", conn)
--     cmd.Parameters.AddWithValue("@kode", idBarang)
--     Dim pToko   = cmd.Parameters.Add("@toko",   MySqlDbType.Decimal) : pToko.Direction   = ParameterDirection.Output
--     Dim pGudang = cmd.Parameters.Add("@gudang", MySqlDbType.Decimal) : pGudang.Direction = ParameterDirection.Output
--     Dim pNama   = cmd.Parameters.Add("@nama",   MySqlDbType.VarChar, 200) : pNama.Direction = ParameterDirection.Output
--     cmd.ExecuteNonQuery()
--     Dim stokToko   As Decimal = ModuleAngka.ParseDecimal(pToko.Value)
--     Dim stokGudang As Decimal = ModuleAngka.ParseDecimal(pGudang.Value)
-- End Using
-- ============================================================
