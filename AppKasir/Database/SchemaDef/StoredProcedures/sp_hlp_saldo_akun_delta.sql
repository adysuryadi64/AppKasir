DROP PROCEDURE IF EXISTS `sp_hlp_saldo_akun_delta`;
DELIMITER //
sp_hlp_saldo_akun_delta

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_saldo_akun_delta`(
    IN p_kode_akun    VARCHAR(20),
    IN p_delta_debet  DECIMAL(20,0),
    IN p_delta_kredit DECIMAL(20,0)
)
BEGIN
    DECLARE v_laba_rugi             DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_debet_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_kredit_laba_rugi DECIMAL(18,2) DEFAULT 0;

    IF p_kode_akun IS NOT NULL AND p_kode_akun <> '' THEN

        UPDATE tbl_datareferensi
        SET
            S_DEBET   = S_DEBET   + p_delta_debet,
            S_KREDIT  = S_KREDIT  + p_delta_kredit,
            SALDO_AKHIR = CASE
                WHEN AKUN_DK = 'DEBET'  THEN SALDO_AKHIR + p_delta_debet  - p_delta_kredit
                WHEN AKUN_DK = 'KREDIT' THEN SALDO_AKHIR - p_delta_debet  + p_delta_kredit
                ELSE SALDO_AKHIR
            END
        WHERE KODE_AKUN = p_kode_akun;

        SELECT
            SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) -
            SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) -
            SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) +
            SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) AS LABA_RUGI,
            SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_DEBET  ELSE 0 END) AS TOTAL_DEBET,
            SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_KREDIT ELSE 0 END) AS TOTAL_KREDIT
        INTO v_laba_rugi, v_total_debet_laba_rugi, v_total_kredit_laba_rugi
        FROM tbl_datareferensi
        WHERE SUB_AKUN IN ('LABA','RUGI');

        UPDATE tbl_datareferensi
        SET
            SALDO_SEBELUMNYA = v_laba_rugi,
            S_DEBET          = v_total_debet_laba_rugi,
            S_KREDIT         = v_total_kredit_laba_rugi
        WHERE TYPE_AKUN = 'LABA RUGI';

        UPDATE tbl_datareferensi
        SET SALDO_AKHIR = SALDO_SEBELUMNYA + (-(S_DEBET)) + S_KREDIT
        WHERE TYPE_AKUN = 'LABA RUGI';

    END IF;
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
