DROP PROCEDURE IF EXISTS `sp_hlp_saldo_kas_validasi`;
DELIMITER //
sp_hlp_saldo_kas_validasi

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_saldo_kas_validasi`(
    IN  p_kode_akun       VARCHAR(20),
    IN  p_nominal_keluar  DECIMAL(15,2),
    OUT p_error_code      VARCHAR(50),
    OUT p_error_message   VARCHAR(255)
)
BEGIN
    DECLARE v_saldo     DECIMAL(20,0) DEFAULT 0;
    DECLARE v_nama      VARCHAR(100)  DEFAULT '';

    SET p_error_code    = '';
    SET p_error_message = '';

    SELECT COALESCE(SALDO_AKHIR, 0), COALESCE(NAMA_AKUN, '')
    INTO v_saldo, v_nama
    FROM tbl_datareferensi
    WHERE KODE_AKUN = p_kode_akun
    FOR UPDATE;

    IF v_saldo < p_nominal_keluar THEN
        SET p_error_code    = 'SALDO_KAS_KURANG';
        SET p_error_message = CONCAT('Saldo akun "', v_nama, '" tidak cukup. ',
                                     'Tersedia: Rp ', FORMAT(v_saldo, 0, 'id_ID'), ', ',
                                     'Dibutuhkan: Rp ', FORMAT(p_nominal_keluar, 0, 'id_ID'));
    END IF;
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
