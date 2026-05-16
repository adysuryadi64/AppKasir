-- =============================================================================
-- 06_migrasi_stored_procedures.sql
-- Migrasi Business Logic ke MySQL Stored Procedures
-- =============================================================================
-- Versi  : 1.0.0
-- Tanggal: 2026-04-19
-- Deskripsi:
--   Membuat semua Stored Procedure untuk migrasi business logic dari VB.NET
--   ke MySQL. Semua SP menggunakan prefix baru (sp_hlp_, sp_trx_, sp_bat_)
--   yang tidak ada di database sebelumnya — aman dijalankan berulang kali.
--
-- Konvensi nama: sp_{kategori}_{entitas}_{aksi}
--   hlp = Helper (internal, dipanggil SP lain)
--   trx = Transaksi (dipanggil langsung oleh klien)
--   bat = Batch (rekonsiliasi massal)
--
-- Cara pakai:
--   USE nama_database;
--   SOURCE 06_migrasi_stored_procedures.sql;
--
-- CATATAN KEAMANAN:
--   - Semua nama SP baru (sp_hlp_*, sp_trx_*, sp_bat_*) tidak ada di database
--     sebelumnya — tidak ada risiko konflik dengan SP lama.
--   - SP lama yang ada hanya berupa helper DDL sementara (AddTimestampColumns,
--     AddSyncId, dll) yang sudah di-DROP setelah dipakai. Tidak ada SP bisnis lama.
--   - Script ini menggunakan DROP PROCEDURE IF EXISTS sebelum CREATE agar aman
--     dijalankan berulang kali (idempoten).
-- =============================================================================

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

-- =============================================================================
-- FASE 1: HELPER SP
-- Sub-prosedur internal yang dipanggil oleh SP transaksi
-- =============================================================================

-- -----------------------------------------------------------------------------
-- sp_hlp_stok_hitung
-- Recalculate STOK_TOKO dan STOK_GUDANG untuk satu barang dari semua counter.
-- Migrasi dari: ModuleVariabel.HitungStokPerubahan()
-- Dipanggil oleh: sp_trx_penjualan_simpan, sp_trx_pembelian_simpan, dll
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_hlp_stok_hitung;
DELIMITER $$
CREATE PROCEDURE sp_hlp_stok_hitung(
    IN p_kode_barang VARCHAR(50)
)
BEGIN
    UPDATE tbl_barang SET
        STOK_TOKO = COALESCE(AWAL_TOKO, 0)
            + COALESCE(TAMBAH_TOKO, 0)              - COALESCE(KURANG_TOKO, 0)
            + COALESCE(PEMBELIAN_TOKO, 0)           - COALESCE(PENJUALAN_TOKO, 0)
            - COALESCE(RETUR_BELI_TOKO, 0)          + COALESCE(RETUR_JUAL_TOKO, 0)
            + COALESCE(OPNAME_TOKO, 0)
            + COALESCE(TRANSFER_STOK_MASUK_TOKO, 0)    - COALESCE(TRANSFER_STOK_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_TOKO, 0),
        STOK_GUDANG = COALESCE(AWAL_GUDANG, 0)
            + COALESCE(TAMBAH_GUDANG, 0)            - COALESCE(KURANG_GUDANG, 0)
            + COALESCE(PEMBELIAN_GUDANG, 0)         - COALESCE(PENJUALAN_GUDANG, 0)
            - COALESCE(RETUR_BELI_GUDANG, 0)        + COALESCE(RETUR_JUAL_GUDANG, 0)
            + COALESCE(OPNAME_GUDANG, 0)
            + COALESCE(TRANSFER_STOK_MASUK_GUDANG, 0)    - COALESCE(TRANSFER_STOK_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_GUDANG, 0)
    WHERE ID_BARANG = p_kode_barang;
    -- Zero rows affected dianggap valid (barang tidak ditemukan)
END$$
DELIMITER ;

-- -----------------------------------------------------------------------------
-- sp_hlp_stok_validasi
-- Cek apakah stok barang cukup untuk transaksi.
-- Dipanggil oleh: sp_trx_penjualan_simpan sebelum INSERT detail
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_hlp_stok_validasi;
DELIMITER $$
CREATE PROCEDURE sp_hlp_stok_validasi(
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

    -- Baca stok dengan lock untuk mencegah race condition
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
        -- Gunakan FORMAT(x, 0, 'id_ID') agar angka tampil tanpa desimal trailing
        -- dan tanpa separator ribuan yang membingungkan (misal 5 bukan 5.0000 atau 5.000)
        SET p_error_message = CONCAT('Stok barang "', v_nama, '" tidak cukup. ',
                                     'Tersedia: ', TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM CAST(v_stok AS CHAR))),
                                     ', Dibutuhkan: ', TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM CAST(p_qty_dibutuhkan AS CHAR))));
    END IF;
END$$
DELIMITER ;

-- -----------------------------------------------------------------------------
-- sp_hlp_faktur_generate
-- Generate nomor faktur unik, aman untuk multi-user (menggunakan SELECT FOR UPDATE).
-- Format: {PREFIX}-{YYMMDD}{XXXX}  — ada '-' setelah prefix, TIDAK ada '-' sebelum urut
-- Contoh: PJ-2604010454, PB-2604010015, TC-2604150001
-- Konsisten dengan format data lama di semua tabel transaksi.
-- Dipanggil oleh: sp_trx_*_simpan untuk transaksi baru (bukan dari draft)
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_hlp_faktur_generate;
DELIMITER $$
CREATE PROCEDURE sp_hlp_faktur_generate(
    IN  p_prefix        VARCHAR(5),    -- contoh: 'PJ', 'PB', 'SO', 'RJ', 'RB', 'TC'
    IN  p_tanggal       DATE,          -- tanggal transaksi (support backdate)
    IN  p_tabel         VARCHAR(50),   -- nama tabel utama: 'penjualan', 'pembelian', dll
    IN  p_kolom_id      VARCHAR(50),   -- nama kolom PK: 'ID_PENJUALAN', 'ID_PEMBELIAN', dll
    OUT p_nomor_faktur  VARCHAR(30)
)
BEGIN
    -- Format data lama: {PREFIX}-{YYMMDD}{XXXX}
    -- Contoh: PJ-2604010453, PB-2604010015, TC-2604150001
    -- Ada separator '-' antara prefix dan tanggal,
    -- TIDAK ada separator antara tanggal dan nomor urut 4 digit.

    DECLARE v_tgl_kode   VARCHAR(6);   -- YYMMDD, contoh: '260401'
    DECLARE v_prefix_tgl VARCHAR(12);  -- PREFIX-YYMMDD, contoh: 'PJ-260401'
    DECLARE v_max_val    VARCHAR(30) DEFAULT NULL;
    DECLARE v_urut       INT DEFAULT 0;
    DECLARE v_prefix_len INT;

    SET v_tgl_kode   = DATE_FORMAT(p_tanggal, '%y%m%d');
    SET v_prefix_tgl = CONCAT(p_prefix, '-', v_tgl_kode);
    SET v_prefix_len = LENGTH(v_prefix_tgl);

    -- Prepared statement untuk nama tabel/kolom dinamis
    -- LIKE 'PJ-260401%' mencocokkan semua nomor pada tanggal tersebut
    -- FOR UPDATE mengunci baris untuk mencegah race condition multi-user
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

    -- Ambil nomor urut dari 4 digit terakhir nomor maksimum
    IF v_max_val IS NOT NULL AND LEFT(v_max_val, v_prefix_len) = v_prefix_tgl THEN
        SET v_urut = CAST(RIGHT(v_max_val, 4) AS UNSIGNED) + 1;
    ELSE
        SET v_urut = 1;
    END IF;

    -- Hasil: PREFIX-YYMMDDXXXX (tanpa separator antara tanggal dan urut)
    SET p_nomor_faktur = CONCAT(v_prefix_tgl, LPAD(v_urut, 4, '0'));
END$$
DELIMITER ;

-- -----------------------------------------------------------------------------
-- sp_hlp_saldo_akun_update
-- Recalculate S_DEBET, S_KREDIT, dan SALDO_AKHIR untuk SATU akun dari JurnalUmum.
-- PENTING: Menggunakan CASE WHEN AKUN_DK — berbeda dari UpdateSaldoAkun() lama
--          di VB.NET yang memiliki bug (tidak menghormati AKUN_DK).
-- Langkah-langkah:
-- 1. Update S_DEBET, S_KREDIT, dan SALDO_AKHIR untuk akun terpengaruh
-- 2. Update akun LABA RUGI terakhir
-- Dipanggil oleh: PHP setelah INSERT jurnal, dan VB.NET via wrapper UpdateSaldoAkun()
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_hlp_saldo_akun_update;
DELIMITER $$
CREATE PROCEDURE sp_hlp_saldo_akun_update(
    IN p_kode_akun VARCHAR(20)
)
BEGIN
    DECLARE v_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_debet_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_kredit_laba_rugi DECIMAL(18,2) DEFAULT 0;

    IF p_kode_akun IS NOT NULL AND p_kode_akun <> '' THEN
        -- LANGKAH 1: Update S_DEBET, S_KREDIT, dan SALDO_AKHIR untuk akun terpengaruh
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

        -- LANGKAH 2: Hitung dan update akun LABA RUGI
        -- Formula yang benar: Laba = KREDIT_LABA - DEBET_LABA - DEBET_RUGI + KREDIT_RUGI
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

        -- Update SALDO_SEBELUMNYA, S_DEBET, S_KREDIT untuk akun LABA RUGI
        UPDATE tbl_datareferensi
        SET
            SALDO_SEBELUMNYA = v_laba_rugi,
            S_DEBET = v_total_debet_laba_rugi,
            S_KREDIT = v_total_kredit_laba_rugi
        WHERE TYPE_AKUN = 'LABA RUGI';

        -- Hitung SALDO_AKHIR untuk akun LABA RUGI
        UPDATE tbl_datareferensi
        SET SALDO_AKHIR = SALDO_SEBELUMNYA + (-(S_DEBET)) + S_KREDIT
        WHERE TYPE_AKUN = 'LABA RUGI';
    END IF;
END$$
DELIMITER ;

-- -----------------------------------------------------------------------------
-- sp_hlp_saldo_kas_validasi
-- Cek apakah saldo kas/bank cukup sebelum pengeluaran.
-- Dipanggil oleh: sp_trx_bayar_hutang_simpan, sp_trx_bayar_piutang_simpan
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_hlp_saldo_kas_validasi;
DELIMITER $$
CREATE PROCEDURE sp_hlp_saldo_kas_validasi(
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

    -- Lock baris untuk mencegah race condition
    SELECT COALESCE(SALDO_AKHIR, 0), COALESCE(NAMA_AKUN, '')
    INTO v_saldo, v_nama
    FROM tbl_datareferensi
    WHERE KODE_AKUN = p_kode_akun
    FOR UPDATE;

    IF v_saldo < p_nominal_keluar THEN
        SET p_error_code    = 'SALDO_KAS_KURANG';
        -- Hindari FORMAT() karena menghasilkan separator ribuan (5.000 bukan 5)
        -- Gunakan CAST ke CHAR lalu trim trailing zero
        SET p_error_message = CONCAT('Saldo akun "', v_nama, '" tidak cukup. ',
                                     'Tersedia: Rp ', FORMAT(v_saldo, 0, 'id_ID'), ', ',
                                     'Dibutuhkan: Rp ', FORMAT(p_nominal_keluar, 0, 'id_ID'));
    END IF;
END$$
DELIMITER ;

-- =============================================================================
-- FASE 2: BATCH SP
-- Recalculate massal — dipanggil dari FormLoading atau terjadwal
-- =============================================================================

-- -----------------------------------------------------------------------------
-- sp_bat_stok_semua_barang
-- Recalculate STOK_TOKO dan STOK_GUDANG semua barang sekaligus.
-- Migrasi dari: ModuleVariabel.HitungSemuaKode()
-- Dipanggil dari: FormLoading.MulaiPosting() dengan Jenis = "Semua"
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_bat_stok_semua_barang;
DELIMITER $$
CREATE PROCEDURE sp_bat_stok_semua_barang()
BEGIN
    UPDATE tbl_barang SET
        STOK_TOKO = COALESCE(AWAL_TOKO, 0)
            + COALESCE(TAMBAH_TOKO, 0)              - COALESCE(KURANG_TOKO, 0)
            + COALESCE(PEMBELIAN_TOKO, 0)           - COALESCE(PENJUALAN_TOKO, 0)
            - COALESCE(RETUR_BELI_TOKO, 0)          + COALESCE(RETUR_JUAL_TOKO, 0)
            + COALESCE(OPNAME_TOKO, 0)
            + COALESCE(TRANSFER_STOK_MASUK_TOKO, 0)    - COALESCE(TRANSFER_STOK_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_TOKO, 0),
        STOK_GUDANG = COALESCE(AWAL_GUDANG, 0)
            + COALESCE(TAMBAH_GUDANG, 0)            - COALESCE(KURANG_GUDANG, 0)
            + COALESCE(PEMBELIAN_GUDANG, 0)         - COALESCE(PENJUALAN_GUDANG, 0)
            - COALESCE(RETUR_BELI_GUDANG, 0)        + COALESCE(RETUR_JUAL_GUDANG, 0)
            + COALESCE(OPNAME_GUDANG, 0)
            + COALESCE(TRANSFER_STOK_MASUK_GUDANG, 0)    - COALESCE(TRANSFER_STOK_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_GUDANG, 0);
END$$
DELIMITER ;

-- -----------------------------------------------------------------------------
-- sp_bat_stok_toko
-- Recalculate STOK_TOKO saja (tanpa STOK_GUDANG).
-- Migrasi dari: ModuleVariabel.HitungStokToko()
-- Dipanggil dari: FormLoading.MulaiPosting() dengan Jenis = "Toko"
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_bat_stok_toko;
DELIMITER $$
CREATE PROCEDURE sp_bat_stok_toko()
BEGIN
    UPDATE tbl_barang SET
        STOK_TOKO = COALESCE(AWAL_TOKO, 0)
            + COALESCE(TAMBAH_TOKO, 0)              - COALESCE(KURANG_TOKO, 0)
            + COALESCE(PEMBELIAN_TOKO, 0)           - COALESCE(PENJUALAN_TOKO, 0)
            - COALESCE(RETUR_BELI_TOKO, 0)          + COALESCE(RETUR_JUAL_TOKO, 0)
            + COALESCE(OPNAME_TOKO, 0)
            + COALESCE(TRANSFER_STOK_MASUK_TOKO, 0)    - COALESCE(TRANSFER_STOK_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_TOKO, 0);
END$$
DELIMITER ;

-- -----------------------------------------------------------------------------
-- sp_bat_stok_gudang
-- Recalculate STOK_GUDANG saja (tanpa STOK_TOKO).
-- Migrasi dari: ModuleVariabel.HitungStokGudang()
-- Dipanggil dari: FormLoading.MulaiPosting() dengan Jenis = "Gudang"
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_bat_stok_gudang;
DELIMITER $$
CREATE PROCEDURE sp_bat_stok_gudang()
BEGIN
    UPDATE tbl_barang SET
        STOK_GUDANG = COALESCE(AWAL_GUDANG, 0)
            + COALESCE(TAMBAH_GUDANG, 0)            - COALESCE(KURANG_GUDANG, 0)
            + COALESCE(PEMBELIAN_GUDANG, 0)         - COALESCE(PENJUALAN_GUDANG, 0)
            - COALESCE(RETUR_BELI_GUDANG, 0)        + COALESCE(RETUR_JUAL_GUDANG, 0)
            + COALESCE(OPNAME_GUDANG, 0)
            + COALESCE(TRANSFER_STOK_MASUK_GUDANG, 0)    - COALESCE(TRANSFER_STOK_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_GUDANG, 0);
END$$
DELIMITER ;

-- -----------------------------------------------------------------------------
-- sp_bat_saldo_semua_akun
-- Recalculate S_DEBET, S_KREDIT, dan SALDO_AKHIR semua akun dari JurnalUmum.
-- PENTING: Menggunakan CASE WHEN AKUN_DK — perbaikan dari UpdateSaldoSemuaAkun() lama.
-- Langkah-langkah:
-- 1. Update S_DEBET, S_KREDIT, dan SALDO_AKHIR untuk semua akun (kecuali LABA RUGI)
-- 2. Hitung dan update akun LABA RUGI terakhir
-- Migrasi dari: ModuleVariabel.UpdateSaldoSemuaAkun()
-- Dipanggil dari: FormLoading.MulaiPosting()
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_bat_saldo_semua_akun;
DELIMITER $$
CREATE PROCEDURE sp_bat_saldo_semua_akun()
BEGIN
    DECLARE v_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_debet_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_kredit_laba_rugi DECIMAL(18,2) DEFAULT 0;

    -- LANGKAH 1: Update S_DEBET, S_KREDIT, dan SALDO_AKHIR untuk semua akun (kecuali LABA RUGI)
    UPDATE tbl_datareferensi r
    LEFT JOIN (
        SELECT NOMOR_AKUN_D, SUM(NOMINAL) AS total_debet
        FROM JurnalUmum
        GROUP BY NOMOR_AKUN_D
    ) d ON d.NOMOR_AKUN_D = r.KODE_AKUN
    LEFT JOIN (
        SELECT NOMOR_AKUN_K, SUM(NOMINAL) AS total_kredit
        FROM JurnalUmum
        GROUP BY NOMOR_AKUN_K
    ) k ON k.NOMOR_AKUN_K = r.KODE_AKUN
    SET
        r.S_DEBET = IFNULL(d.total_debet, 0),
        r.S_KREDIT = IFNULL(k.total_kredit, 0),
        r.SALDO_SEBELUMNYA = r.SALDO_AWAL,
        r.SALDO_AKHIR = CASE
            WHEN r.AKUN_DK = 'DEBET'  THEN IFNULL(r.SALDO_AWAL, 0) + IFNULL(d.total_debet, 0) - IFNULL(k.total_kredit, 0)
            WHEN r.AKUN_DK = 'KREDIT' THEN IFNULL(r.SALDO_AWAL, 0) - IFNULL(d.total_debet, 0) + IFNULL(k.total_kredit, 0)
            ELSE 0
        END
    WHERE r.TYPE_AKUN <> 'LABA RUGI';

    -- LANGKAH 2: Hitung dan update akun LABA RUGI terakhir
    -- Formula yang benar: Laba = KREDIT_LABA - DEBET_LABA - DEBET_RUGI + KREDIT_RUGI
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

    -- Update SALDO_SEBELUMNYA, S_DEBET, S_KREDIT untuk akun LABA RUGI
    UPDATE tbl_datareferensi
    SET
        SALDO_SEBELUMNYA = v_laba_rugi,
        S_DEBET = v_total_debet_laba_rugi,
        S_KREDIT = v_total_kredit_laba_rugi
    WHERE TYPE_AKUN = 'LABA RUGI';

    -- Hitung SALDO_AKHIR untuk akun LABA RUGI
    UPDATE tbl_datareferensi
    SET SALDO_AKHIR = SALDO_SEBELUMNYA + (-(S_DEBET)) + S_KREDIT
    WHERE TYPE_AKUN = 'LABA RUGI';
END$$
DELIMITER ;

-- -----------------------------------------------------------------------------
-- sp_bat_piutang_semua_pelanggan
-- Recalculate HutangAkhir semua pelanggan dari tabel penjualan.
-- Migrasi dari: ModuleVariabel.UpdatePiutangDibayar()
-- Dipanggil dari: FormLoading.MulaiLoading() dan MulaiPosting()
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_bat_piutang_semua_pelanggan;
DELIMITER $$
CREATE PROCEDURE sp_bat_piutang_semua_pelanggan()
BEGIN
    UPDATE tbl_pelanggan p
    LEFT JOIN (
        SELECT ID_PELANGGAN, SUM(IFNULL(SISA_TAGIHAN, 0)) AS HUTANG
        FROM penjualan
        GROUP BY ID_PELANGGAN
    ) x ON x.ID_PELANGGAN = p.KODE
    SET p.HUTANGAKHIR = IFNULL(x.HUTANG, 0) + IFNULL(p.HUTANGAWAL, 0);
END$$
DELIMITER ;

-- -----------------------------------------------------------------------------
-- sp_bat_hutang_semua_supplier
-- Recalculate HutangAkhir semua supplier dari tabel pembelian.
-- Migrasi dari: ModuleVariabel.UpdateSupliyerFromPembelianHutangDibayar()
-- Dipanggil dari: FormLoading.MulaiLoading() dan MulaiPosting()
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_bat_hutang_semua_supplier;
DELIMITER $$
CREATE PROCEDURE sp_bat_hutang_semua_supplier()
BEGIN
    UPDATE tbl_supliyer s
    LEFT JOIN (
        SELECT ID_SUPPLIER, SUM(IFNULL(TAGIHAN, 0)) AS HUTANG
        FROM pembelian
        GROUP BY ID_SUPPLIER
    ) x ON x.ID_SUPPLIER = s.KODE
    SET s.HUTANGAKHIR = IFNULL(x.HUTANG, 0) + IFNULL(s.HUTANGAWAL, 0);
END$$
DELIMITER ;

-- -----------------------------------------------------------------------------
-- sp_bat_bon_semua_karyawan
-- Recalculate SaldoAkhir semua karyawan dari tabel Bon_karyawan.
-- Migrasi dari: ModuleVariabel.UpdateTotalBonDanTotalBayarKaryawan()
-- Dipanggil dari: FormLoading.MulaiLoading() dan MulaiPosting()
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_bat_bon_semua_karyawan;
DELIMITER $$
CREATE PROCEDURE sp_bat_bon_semua_karyawan()
BEGIN
    UPDATE tbl_karyawan k
    LEFT JOIN (
        SELECT KODE, SUM(NOMINAL) AS TotalBon
        FROM bon_karyawan
        WHERE JENIS = 'BON'
        GROUP BY KODE
    ) b ON b.KODE = k.KODE
    LEFT JOIN (
        SELECT KODE, SUM(NOMINAL) AS TotalBayar
        FROM bon_karyawan
        WHERE JENIS = 'BAYAR'
        GROUP BY KODE
    ) p ON p.KODE = k.KODE
    SET k.TOTALBON    = IFNULL(b.TotalBon, 0),
        k.TOTALBAYAR  = IFNULL(p.TotalBayar, 0),
        k.SALDOAKHIR  = IFNULL(k.SALDOAWAL, 0) + IFNULL(b.TotalBon, 0) - IFNULL(p.TotalBayar, 0);
END$$
DELIMITER ;

-- =============================================================================
-- VERIFIKASI
-- Format nomor faktur yang dihasilkan sp_hlp_faktur_generate:
--   {PREFIX}-{YYMMDD}{XXXX}  — ada '-' setelah prefix, TIDAK ada '-' sebelum urut
--   Contoh: PJ-2604010454, PB-2604010015, TC-2604150001
--   Konsisten dengan data lama di semua tabel transaksi.
-- =============================================================================

SELECT
    ROUTINE_NAME AS sp_name,
    ROUTINE_TYPE AS tipe,
    CREATED      AS dibuat
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = DATABASE()
  AND ROUTINE_NAME LIKE 'sp_%'
ORDER BY ROUTINE_NAME;

SELECT CONCAT(
    'Fase 1 (Helper SP): sp_hlp_faktur_generate, sp_hlp_stok_hitung, ',
    'sp_hlp_stok_validasi, ',
    'sp_hlp_saldo_akun_update, sp_hlp_saldo_kas_validasi'
) AS fase_1_helper;

SELECT CONCAT(
    'Fase 2 (Batch SP): sp_bat_stok_semua_barang, sp_bat_stok_toko, sp_bat_stok_gudang, ',
    'sp_bat_saldo_semua_akun, sp_bat_piutang_semua_pelanggan, ',
    'sp_bat_hutang_semua_supplier, sp_bat_bon_semua_karyawan'
) AS fase_2_batch;

SELECT 'SP Transaksi (sp_trx_*) akan dibuat di file 07_migrasi_sp_transaksi.sql' AS catatan;
