-- =============================================================================
-- 19_sp_val_pembelian_harga_beli_vs_jual.sql
-- Validasi apakah harga beli lebih tinggi dari harga jual (rugi)
-- =============================================================================
-- Versi  : 2.0.0
-- Tanggal: 2026-04-27
-- Deskripsi:
--   Validasi harga beli vs harga jual dengan 3 level:
--   1. Rugi Kritis: Rugi di SEMUA level harga jual (BLOCK - tidak boleh simpan)
--   2. Rugi Umum + Partai: Rugi di kedua jenis (WARNING - konfirmasi user)
--   3. Rugi Salah Satu: Rugi di satu jenis saja (INFO - bisa lanjut)
--
-- Fungsi:
--   - Mengambil harga jual umum dan partai dari tbl_barang (satuan kecil)
--   - Menghitung harga beli per satuan kecil
--   - Membandingkan dengan semua harga jual yang tersedia
--   - Return flag rugi kritis, rugi umum, rugi partai, dan harga jual minimum
--
-- Cara pakai:
--   USE db_kasirlancar;
--   SOURCE 19_sp_val_pembelian_harga_beli_vs_jual.sql;
--
-- Migrasi dari: FormPembelian.Cekjualrugi()
-- Dipanggil dari: FormPembelian saat simpan transaksi (sebelum simpan)
-- =============================================================================

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

-- -----------------------------------------------------------------------------
-- sp_val_pembelian_harga_beli_vs_jual
-- Validasi apakah harga beli lebih tinggi dari harga jual (umum dan partai)
-- dengan 3 level validasi: Kritis, Warning, Info
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_val_pembelian_harga_beli_vs_jual;
DELIMITER $$

CREATE PROCEDURE sp_val_pembelian_harga_beli_vs_jual(
    IN  p_id_barang           VARCHAR(50),
    IN  p_harga_beli_total    DECIMAL(15,2),
    IN  p_qty_sat             DECIMAL(15,4),
    OUT p_rugi_kritis         TINYINT(1),    -- Rugi di SEMUA level (BLOCK)
    OUT p_rugi_umum           TINYINT(1),    -- Rugi di harga umum
    OUT p_rugi_partai         TINYINT(1),    -- Rugi di harga partai
    OUT p_harga_jual_min      DECIMAL(15,2), -- Harga jual minimum per satuan kecil
    OUT p_harga_jual_umum     DECIMAL(15,2), -- Harga jual umum total
    OUT p_harga_jual_partai   DECIMAL(15,2)  -- Harga jual partai total
)
sp_val_pembelian_harga_beli_vs_jual: BEGIN
    DECLARE v_harga_beli_per_kecil    DECIMAL(15,4) DEFAULT 0;
    DECLARE v_harga_jual_umum_kecil   DECIMAL(15,2) DEFAULT 0;
    DECLARE v_harga_jual_partai_kecil DECIMAL(15,2) DEFAULT 0;
    DECLARE v_ada_harga_umum          TINYINT(1) DEFAULT 0;
    DECLARE v_ada_harga_partai        TINYINT(1) DEFAULT 0;
    
    -- Inisialisasi output dengan nilai default
    SET p_rugi_kritis = 0;
    SET p_rugi_umum = 0;
    SET p_rugi_partai = 0;
    SET p_harga_jual_min = 0;
    SET p_harga_jual_umum = 0;
    SET p_harga_jual_partai = 0;
    
    -- Ambil harga jual umum dan partai dari tbl_barang (satuan kecil)
    SELECT COALESCE(HARGA_JUAL_UMUM_KECIL, 0),
           COALESCE(HARGA_JUAL_PARTAI_KECIL, 0)
    INTO v_harga_jual_umum_kecil, v_harga_jual_partai_kecil
    FROM tbl_barang
    WHERE ID_BARANG = p_id_barang
    LIMIT 1;
    
    -- Cek apakah ada harga jual yang sudah diset
    SET v_ada_harga_umum = IF(v_harga_jual_umum_kecil > 0, 1, 0);
    SET v_ada_harga_partai = IF(v_harga_jual_partai_kecil > 0, 1, 0);
    
    -- Jika tidak ada harga jual sama sekali, skip validasi
    IF v_ada_harga_umum = 0 AND v_ada_harga_partai = 0 THEN
        LEAVE sp_val_pembelian_harga_beli_vs_jual;
    END IF;
    
    -- Hitung harga beli per satuan kecil
    SET v_harga_beli_per_kecil = p_harga_beli_total / p_qty_sat;
    
    -- Hitung harga jual total (dikali qty satuan)
    -- Gunakan ROUND untuk konsistensi dengan VB.NET
    SET p_harga_jual_umum = ROUND(v_harga_jual_umum_kecil * p_qty_sat, 0);
    SET p_harga_jual_partai = ROUND(v_harga_jual_partai_kecil * p_qty_sat, 0);
    
    -- Cari harga jual minimum (yang > 0)
    IF v_ada_harga_umum = 1 AND v_ada_harga_partai = 1 THEN
        SET p_harga_jual_min = LEAST(v_harga_jual_umum_kecil, v_harga_jual_partai_kecil);
    ELSEIF v_ada_harga_umum = 1 THEN
        SET p_harga_jual_min = v_harga_jual_umum_kecil;
    ELSE
        SET p_harga_jual_min = v_harga_jual_partai_kecil;
    END IF;
    
    -- Validasi per jenis harga
    -- Hanya cek jika harga jual sudah diset (> 0)
    IF v_ada_harga_umum = 1 AND v_harga_beli_per_kecil > v_harga_jual_umum_kecil THEN
        SET p_rugi_umum = 1;
    END IF;
    
    IF v_ada_harga_partai = 1 AND v_harga_beli_per_kecil > v_harga_jual_partai_kecil THEN
        SET p_rugi_partai = 1;
    END IF;
    
    -- Rugi Kritis: Rugi di SEMUA level yang ada harganya
    -- Logika: Jika ada harga umum DAN rugi umum, DAN (tidak ada harga partai ATAU rugi partai)
    --         ATAU Jika ada harga partai DAN rugi partai, DAN (tidak ada harga umum ATAU rugi umum)
    IF (v_ada_harga_umum = 1 AND p_rugi_umum = 1 AND (v_ada_harga_partai = 0 OR p_rugi_partai = 1))
       OR (v_ada_harga_partai = 1 AND p_rugi_partai = 1 AND (v_ada_harga_umum = 0 OR p_rugi_umum = 1)) THEN
        SET p_rugi_kritis = 1;
    END IF;
    
END$$

DELIMITER ;

-- =============================================================================
-- VERIFIKASI
-- =============================================================================

SELECT
    ROUTINE_NAME AS sp_name,
    ROUTINE_TYPE AS tipe,
    CREATED      AS dibuat
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = DATABASE()
  AND ROUTINE_NAME = 'sp_val_pembelian_harga_beli_vs_jual';

SELECT 'SP sp_val_pembelian_harga_beli_vs_jual berhasil dibuat.' AS status;
