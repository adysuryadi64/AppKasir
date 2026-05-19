-- =============================================================================
-- sp_hlp_saldo_akun_delta
-- =============================================================================
-- Tujuan: Update saldo akun secara INCREMENTAL (delta) tanpa scan JurnalUmum.
--
-- Perbedaan dengan sp_hlp_saldo_akun_update:
--   sp_hlp_saldo_akun_update : SUM seluruh JurnalUmum (lambat, 627k+ baris)
--   sp_hlp_saldo_akun_delta  : Hanya tambah/kurang delta ke nilai yang sudah ada
--                              (cepat, tidak menyentuh JurnalUmum sama sekali)
--
-- Parameter:
--   p_kode_akun    : Kode akun yang akan diupdate
--   p_delta_debet  : Nominal yang masuk ke sisi DEBET (positif = tambah, negatif = kurangi)
--   p_delta_kredit : Nominal yang masuk ke sisi KREDIT (positif = tambah, negatif = kurangi)
--
-- Cara pakai:
--   Simpan transaksi baru  → delta positif sesuai jurnal yang di-INSERT
--   Hapus/reversal         → delta negatif (kebalikan dari jurnal yang di-DELETE)
--
-- Catatan:
--   Saldo bisa drift jika ada bug di pemanggil. Gunakan sp_bat_saldo_semua_akun
--   atau PostingResmi_HitungSemuaSaldo_KeTblDatareferensi() untuk rekonsiliasi penuh.
--
-- Benchmark (db_moroseneng, 627k baris JurnalUmum):
--   sp_hlp_saldo_akun_update x4 : ~3 detik (dengan FORCE INDEX)
--   sp_hlp_saldo_akun_delta  x4 : < 10 ms  (tidak scan JurnalUmum)
-- =============================================================================

DROP PROCEDURE IF EXISTS sp_hlp_saldo_akun_delta;

DELIMITER $$

CREATE PROCEDURE sp_hlp_saldo_akun_delta(
    IN p_kode_akun    VARCHAR(20),
    IN p_delta_debet  DECIMAL(20,0),
    IN p_delta_kredit DECIMAL(20,0)
)
BEGIN
    DECLARE v_laba_rugi             DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_debet_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_kredit_laba_rugi DECIMAL(18,2) DEFAULT 0;

    IF p_kode_akun IS NOT NULL AND p_kode_akun <> '' THEN

        -- ── LANGKAH 1: Update saldo akun secara incremental ──────────────────
        -- Tidak scan JurnalUmum — hanya tambah delta ke nilai yang sudah ada.
        -- SALDO_AKHIR dihitung berdasarkan AKUN_DK:
        --   DEBET  : saldo naik jika ada debet, turun jika ada kredit
        --   KREDIT : saldo naik jika ada kredit, turun jika ada debet
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

        -- ── LANGKAH 2: Recalculate akun LABA RUGI dari saldo akun terkini ────
        -- tbl_datareferensi kecil — query ini cepat, tidak scan JurnalUmum.
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
END$$

DELIMITER ;
