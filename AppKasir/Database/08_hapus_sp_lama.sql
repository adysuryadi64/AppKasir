-- =============================================================================
-- 08_hapus_sp_lama.sql
-- Hapus Stored Procedures Lama yang Tidak Dipakai
-- =============================================================================
-- Versi  : 1.0.0
-- Tanggal: 2026-04-19
-- Deskripsi:
--   Menghapus SP lama dari spl_ama.sql yang sudah digantikan oleh SP baru
--   di 06_migrasi_stored_procedures.sql dan 07_migrasi_sp_transaksi.sql.
--
-- PRASYARAT:
--   Jalankan 06_migrasi_stored_procedures.sql terlebih dahulu agar SP baru
--   sudah tersedia sebelum SP lama dihapus.
--
-- AMAN DIJALANKAN BERULANG KALI:
--   Semua perintah menggunakan DROP PROCEDURE IF EXISTS.
--
-- ALASAN PENGHAPUSAN:
--   sp_HitungPeriodeSaldo_Neraca  → Menulis ke tbl_datareferensi langsung
--                                    (melanggar Req 17). Ada bug Step 3.
--                                    Digantikan oleh logika di FormLapNeracaLR
--                                    yang akan diarahkan ke temp_datareferensi.
--
--   sp_HitungSemuaSaldo_Neraca    → Digantikan sp_bat_saldo_semua_akun
--                                    yang sudah diperbaiki bug AKUN_DK.
--
--   sp_hitung_by_kode             → Digantikan sp_hlp_stok_hitung
--                                    yang lebih lengkap (ada TRANSFER_CABANG_*).
--
--   sp_hitung_semua_stok          → Digantikan sp_bat_stok_semua_barang
--                                    yang lebih lengkap (ada TRANSFER_CABANG_*).
--
--   sp_hitung_stok_toko           → Digantikan sp_bat_stok_toko.
--
--   sp_hitung_stok_gudang         → Digantikan sp_bat_stok_gudang.
--
--   sp_saldo_akun_tambah          → Pendekatan delta berbahaya — rawan
--   sp_saldo_akun_kurang            inkonsistensi jika transaksi dihapus.
--                                    Digantikan sp_hlp_saldo_akun_update
--                                    yang recalculate penuh dari JurnalUmum.
--
--   sp_update_all_barang_toko_module → Tidak lengkap (hanya 3 dari 13 jenis
--                                       mutasi). Tidak ada padanan langsung —
--                                       logika ini ada di FormLoading.vb.
--
--   sp_update_total_bon_karyawan  → Digantikan sp_bat_bon_semua_karyawan
--                                    yang lebih efisien (LEFT JOIN tanpa reset).
--
-- TIDAK ADA YANG MEMANGGIL SP INI:
--   Sudah diverifikasi — tidak ada VB.NET maupun PHP yang memanggil
--   SP-SP di bawah ini. Semua masih tahap rencana di spl_ama.sql.
-- =============================================================================

-- ── Neraca & Saldo ────────────────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_HitungPeriodeSaldo_Neraca;
DROP PROCEDURE IF EXISTS sp_HitungSemuaSaldo_Neraca;
DROP PROCEDURE IF EXISTS sp_saldo_akun_tambah;
DROP PROCEDURE IF EXISTS sp_saldo_akun_kurang;

-- ── Stok ──────────────────────────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_hitung_by_kode;
DROP PROCEDURE IF EXISTS sp_hitung_semua_stok;
DROP PROCEDURE IF EXISTS sp_hitung_stok_toko;
DROP PROCEDURE IF EXISTS sp_hitung_stok_gudang;
DROP PROCEDURE IF EXISTS sp_update_all_barang_toko_module;

-- ── Karyawan ──────────────────────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_update_total_bon_karyawan;

-- =============================================================================
-- VERIFIKASI — pastikan SP lama sudah tidak ada
-- =============================================================================
SELECT
    ROUTINE_NAME AS sp_lama,
    'MASIH ADA — periksa manual' AS status
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = DATABASE()
  AND ROUTINE_NAME IN (
      'sp_HitungPeriodeSaldo_Neraca',
      'sp_HitungSemuaSaldo_Neraca',
      'sp_saldo_akun_tambah',
      'sp_saldo_akun_kurang',
      'sp_hitung_by_kode',
      'sp_hitung_semua_stok',
      'sp_hitung_stok_toko',
      'sp_hitung_stok_gudang',
      'sp_update_all_barang_toko_module',
      'sp_update_total_bon_karyawan'
  );

-- Jika query di atas tidak mengembalikan baris, semua SP lama sudah terhapus.
SELECT 'SP lama berhasil dihapus. Jalankan verifikasi di atas untuk konfirmasi.' AS catatan;

-- =============================================================================
-- SP BARU YANG MENGGANTIKAN (dari 06_migrasi_stored_procedures.sql)
-- =============================================================================
-- sp_HitungPeriodeSaldo_Neraca  → (tidak ada padanan SP — logika pindah ke
--                                   temp_datareferensi di FormLapNeracaLR)
-- sp_HitungSemuaSaldo_Neraca    → sp_bat_saldo_semua_akun
-- sp_saldo_akun_tambah/kurang   → sp_hlp_saldo_akun_update
-- sp_hitung_by_kode             → sp_hlp_stok_hitung
-- sp_hitung_semua_stok          → sp_bat_stok_semua_barang
-- sp_hitung_stok_toko           → sp_bat_stok_toko
-- sp_hitung_stok_gudang         → sp_bat_stok_gudang
-- sp_update_all_barang_toko_module → (tidak ada padanan SP)
-- sp_update_total_bon_karyawan  → sp_bat_bon_semua_karyawan
