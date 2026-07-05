DROP PROCEDURE IF EXISTS `sp_hlp_stok_validasi`;
DELIMITER //
sp_hlp_stok_validasi

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_stok_validasi`(
    IN  p_kode_barang     VARCHAR(50),
    IN  p_qty_dibutuhkan  DECIMAL(15,4),
    IN  p_lokasi          VARCHAR(10),   -- 'TOKO' atau 'GUDANG'
    IN  p_izinkan_minus   TINYINT(1),    -- 0 = tolak jika kurang, 1 = izinkan minus
    OUT p_error_code      VARCHAR(50),
    OUT p_error_message   VARCHAR(255)
)
BEGIN
    DECLARE v_stok      DECIMAL(15,4) DEFAULT 0;
    DECLARE v_nama      VARCHAR(200)  DEFAULT '';

    SET p_error_code    = '';
    SET p_error_message = '';

    IF p_lokasi = 'GUDANG' THEN
        SELECT COALESCE(STOK_GUDANG, 0), COALESCE(NAMA_BARANG, '')
        INTO v_stok, v_nama
        FROM tbl_barang
        WHERE ID_BARANG = p_kode_barang
        FOR UPDATE;
    ELSE
        SELECT COALESCE(STOK_TOKO, 0), COALESCE(NAMA_BARANG, '')
        INTO v_stok, v_nama
        FROM tbl_barang
        WHERE ID_BARANG = p_kode_barang
        FOR UPDATE;
    END IF;

    IF p_izinkan_minus = 0 AND v_stok < p_qty_dibutuhkan THEN
        SET p_error_code    = 'STOK_KURANG';
        SET p_error_message = CONCAT('Stok barang "', v_nama, '" tidak cukup. ',
                                     'Tersedia: ', TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM CAST(v_stok AS CHAR))),
                                     ', Dibutuhkan: ', TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM CAST(p_qty_dibutuhkan AS CHAR))));
    END IF;
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
