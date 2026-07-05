DROP PROCEDURE IF EXISTS `sp_hlp_saldo_akun_update`;
DELIMITER //
sp_hlp_saldo_akun_update

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_saldo_akun_update`(
    IN p_kode_akun VARCHAR(20)
)
BEGIN
    DECLARE v_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_debet_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_kredit_laba_rugi DECIMAL(18,2) DEFAULT 0;

    IF p_kode_akun IS NOT NULL AND p_kode_akun <> '' THEN
        UPDATE tbl_datareferensi r
        LEFT JOIN (
            SELECT NOMOR_AKUN_D AS kode, SUM(NOMINAL) AS total
            FROM JurnalUmum
            WHERE NOMOR_AKUN_D = p_kode_akun
            GROUP BY NOMOR_AKUN_D
        ) d ON d.kode = r.KODE_AKUN
        LEFT JOIN (
            SELECT NOMOR_AKUN_K AS kode, SUM(NOMINAL) AS total
            FROM JurnalUmum
            WHERE NOMOR_AKUN_K = p_kode_akun
            GROUP BY NOMOR_AKUN_K
        ) k ON k.kode = r.KODE_AKUN
        SET 
            r.S_DEBET = IFNULL(d.total, 0),
            r.S_KREDIT = IFNULL(k.total, 0),
            r.SALDO_AKHIR = CASE
                WHEN r.AKUN_DK = 'DEBET'  THEN IFNULL(r.SALDO_AWAL, 0) + IFNULL(d.total, 0) - IFNULL(k.total, 0)
                WHEN r.AKUN_DK = 'KREDIT' THEN IFNULL(r.SALDO_AWAL, 0) - IFNULL(d.total, 0) + IFNULL(k.total, 0)
                ELSE 0
            END
        WHERE r.KODE_AKUN = p_kode_akun;

        SELECT
            SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) -
            SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) -
            SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) +
            SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) AS LABA_RUGI,
            SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_DEBET ELSE 0 END) AS TOTAL_DEBET,
            SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_KREDIT ELSE 0 END) AS TOTAL_KREDIT
        INTO v_laba_rugi, v_total_debet_laba_rugi, v_total_kredit_laba_rugi
        FROM tbl_datareferensi
        WHERE SUB_AKUN IN ('LABA','RUGI');

        UPDATE tbl_datareferensi
        SET
            SALDO_SEBELUMNYA = v_laba_rugi,
            S_DEBET = v_total_debet_laba_rugi,
            S_KREDIT = v_total_kredit_laba_rugi
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
