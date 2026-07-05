DROP PROCEDURE IF EXISTS `sp_hlp_stok_ambil_edit_retur_beli`;
DELIMITER //
sp_hlp_stok_ambil_edit_retur_beli

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_stok_ambil_edit_retur_beli`(
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

    SELECT COALESCE(LOKASI, '')
    INTO v_lokasi_faktur
    FROM retur_pembelian r
    INNER JOIN pembelian p ON r.ID_PEMBELIAN = p.ID_PEMBELIAN
    WHERE r.ID_RETUR_PEMBELIAN = p_faktur_lama
    LIMIT 1;

    SELECT COALESCE(SUM(QTY_SAT), 0)
    INTO v_qty_lama
    FROM retur_pembelian_detail
    WHERE ID_RETUR_PEMBELIAN = p_faktur_lama
      AND ID_BARANG   = p_kode_barang;

    IF v_lokasi_faktur = p_lokasi THEN
        SET p_stok_toko   = IF(p_lokasi = 'TOKO',   v_stok_toko   - v_qty_lama, v_stok_toko);
        SET p_stok_gudang = IF(p_lokasi = 'GUDANG',  v_stok_gudang - v_qty_lama, v_stok_gudang);
    ELSE
        SET p_stok_toko   = v_stok_toko;
        SET p_stok_gudang = v_stok_gudang;
    END IF;
    SET p_nama_barang = v_nama;
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
