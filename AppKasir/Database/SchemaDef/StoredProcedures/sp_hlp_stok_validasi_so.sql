DROP PROCEDURE IF EXISTS `sp_hlp_stok_validasi_so`;
DELIMITER //
sp_hlp_stok_validasi_so

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_stok_validasi_so`(
    IN  p_kode_barang     VARCHAR(50),
    IN  p_qty_dibutuhkan  DECIMAL(15,4),
    IN  p_faktur_so       VARCHAR(30),
    IN  p_lokasi          VARCHAR(10),   -- 'TOKO' atau 'GUDANG'
    IN  p_izinkan_minus   TINYINT(1),    -- 0 = tolak jika kurang, 1 = izinkan minus
    OUT p_error_code      VARCHAR(50),
    OUT p_error_message   VARCHAR(255)
)
BEGIN
    DECLARE v_stok      DECIMAL(15,4) DEFAULT 0;
    DECLARE v_nama      VARCHAR(200)  DEFAULT '';
    DECLARE v_qty_so    DECIMAL(15,4) DEFAULT 0;
    DECLARE v_stok_ef   DECIMAL(15,4) DEFAULT 0;

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

    SELECT COALESCE(SUM(QTY_SATUAN), 0)
    INTO v_qty_so
    FROM sales_order_detail
    WHERE FAKTUR_JUAL = p_faktur_so
      AND ID_BARANG   = p_kode_barang
      AND LOKASIBARANG = p_lokasi;

    SET v_stok_ef = v_stok + v_qty_so;

    IF p_izinkan_minus = 0 AND v_stok_ef < p_qty_dibutuhkan THEN
        SET p_error_code    = 'STOK_KURANG';
        SET p_error_message = CONCAT('Stok barang "', v_nama, '" tidak cukup. ',
                                     'Tersedia Efektif (Stok + Reservasi SO): ', 
                                     TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM CAST(v_stok_ef AS CHAR))),
                                     ', Dibutuhkan: ', 
                                     TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM CAST(p_qty_dibutuhkan AS CHAR))));
    END IF;
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
