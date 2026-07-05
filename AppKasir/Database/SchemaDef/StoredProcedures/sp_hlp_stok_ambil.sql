DROP PROCEDURE IF EXISTS `sp_hlp_stok_ambil`;
DELIMITER //
sp_hlp_stok_ambil

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_stok_ambil`(
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
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
