DROP PROCEDURE IF EXISTS `sp_hlp_faktur_generate`;
DELIMITER //
sp_hlp_faktur_generate

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_faktur_generate`(
    IN  p_prefix        VARCHAR(5),    -- contoh: 'PJ', 'PB', 'SO', 'RJ', 'RB', 'TC'
    IN  p_tanggal       DATE,          -- tanggal transaksi (support backdate)
    IN  p_tabel         VARCHAR(50),   -- nama tabel utama: 'penjualan', 'pembelian', dll
    IN  p_kolom_id      VARCHAR(50),   -- nama kolom PK: 'ID_PENJUALAN', 'ID_PEMBELIAN', dll
    OUT p_nomor_faktur  VARCHAR(30)
)
BEGIN

    DECLARE v_tgl_kode   VARCHAR(6);   -- YYMMDD, contoh: '260401'
    DECLARE v_prefix_tgl VARCHAR(12);  -- PREFIX-YYMMDD, contoh: 'PJ-260401'
    DECLARE v_max_val    VARCHAR(30) DEFAULT NULL;
    DECLARE v_urut       INT DEFAULT 0;
    DECLARE v_prefix_len INT;

    SET v_tgl_kode   = DATE_FORMAT(p_tanggal, '%y%m%d');
    SET v_prefix_tgl = CONCAT(p_prefix, '-', v_tgl_kode);
    SET v_prefix_len = LENGTH(v_prefix_tgl);

    SET @sql_max = CONCAT(
        'SELECT MAX(', p_kolom_id, ') INTO @v_max ',
        'FROM ', p_tabel, ' ',
        'WHERE ', p_kolom_id, ' LIKE ? ',
        'FOR UPDATE'
    );
    SET @prefix_like = CONCAT(v_prefix_tgl, '%');

    PREPARE stmt_max FROM @sql_max;
    EXECUTE stmt_max USING @prefix_like;
    DEALLOCATE PREPARE stmt_max;

    SET v_max_val = @v_max;

    IF v_max_val IS NOT NULL AND LEFT(v_max_val, v_prefix_len) = v_prefix_tgl THEN
        SET v_urut = CAST(RIGHT(v_max_val, 4) AS UNSIGNED) + 1;
    ELSE
        SET v_urut = 1;
    END IF;

    SET p_nomor_faktur = CONCAT(v_prefix_tgl, LPAD(v_urut, 4, '0'));
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
