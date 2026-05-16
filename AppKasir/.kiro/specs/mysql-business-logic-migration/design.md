# Design Document — Migrasi Business Logic ke MySQL Stored Procedures

## Overview
Dokumentasi ini menjelaskan desain arsitektur untuk migrasi logika bisnis AppKasir dari klien VB.NET ke MySQL Stored Procedures, dengan prinsip utama: **Simpan & Hapus Tetap di Klien**, hanya logika bisnis kritis yang dipindahkan ke SP helper.

## Arsitektur Sistem

### Pembagian Tanggung Jawab

| Lapisan | Klien VB.NET | Klien Flutter/PHP |
|---------|---------------|-------------------|
| **Generate Nomor Faktur** | `sp_hlp_faktur_generate` | `sp_hlp_faktur_generate` |
| **Simpan Transaksi** | Inline SQL + `conn.BeginTransaction()` | Inline SQL + `$pdo->beginTransaction()` + validasi via SP |
| **Hapus Transaksi** | Inline SQL + `conn.BeginTransaction()` | Inline SQL + `$pdo->beginTransaction()` |
| **Validasi Stok** | Inline di VB | `sp_hlp_stok_validasi` |
| **Hitung Stok** | `HitungStokPerubahan()` (VB) | `sp_hlp_stok_hitung` |
| **Update Saldo Akun** | `UpdateSaldoSemuaAkun()` (VB - bug fixed) | `sp_hlp_saldo_akun_update` |

## Daftar Stored Procedures

### Kategori Helper (hlp)

| Nama SP | Parameter | Tujuan |
|---------|-----------|--------|
| `sp_hlp_faktur_generate` | `p_prefix`, `p_tanggal`, `p_tabel`, `p_kolom`, OUT `p_nomor` | Generate nomor faktur unik dengan format `{PREFIX}-{YYMMDD}{XXXX}`, aman multi-user |
| `sp_hlp_stok_validasi` | `p_kode_barang`, `p_qty`, `p_lokasi`, `p_izinkan_minus`, OUT `p_error_code`, OUT `p_error_message` | Cek stok cukup sebelum transaksi |
| `sp_hlp_stok_hitung` | `p_kode_barang` | Recalculate `STOK_TOKO` dan `STOK_GUDANG` dari semua counter |
| `INSERT INTO JurnalUmum` (inline) | Semua parameter JurnalUmum | INSERT satu baris ke `JurnalUmum` |
| `sp_hlp_saldo_akun_update` | `p_kode_akun` | Recalculate `Saldo_Akhir` dengan `CASE WHEN AKUN_DK` |
| `sp_hlp_saldo_kas_validasi` | `p_kode_akun`, `p_nominal_keluar`, OUT `p_error_code`, OUT `p_error_message` | Cek saldo kas cukup sebelum pengeluaran |

### Kategori Batch (bat)

| Nama SP | Tujuan |
|---------|--------|
| `sp_bat_stok_semua_barang` | Recalculate stok semua barang |
| `sp_bat_stok_toko` | Recalculate `STOK_TOKO` saja |
| `sp_bat_stok_gudang` | Recalculate `STOK_GUDANG` saja |
| `sp_bat_saldo_semua_akun` | Recalculate `Saldo_Akhir` semua akun |
| `sp_bat_piutang_semua_pelanggan` | Recalculate `HutangAkhir` semua pelanggan |
| `sp_bat_hutang_semua_supplier` | Recalculate `HutangAkhir` semua supplier |
| `sp_bat_bon_semua_karyawan` | Recalculate `SaldoAkhir` semua karyawan |

## Konvensi Naming & Struktur

### Format Nomor Faktur
`{PREFIX}-{YYMMDD}{XXXX}`
- Contoh: `PJ-2604200001` (Penjualan, tanggal 20 April 2026, urutan 0001)
- Reset urutan setiap hari
- Menggunakan `SELECT ... FOR UPDATE` untuk mencegah race condition

### Struktur SP Standar
```sql
DROP PROCEDURE IF EXISTS sp_nama_sp;
DELIMITER $$
CREATE PROCEDURE sp_nama_sp(
    IN  p_param1 VARCHAR(50),
    OUT p_success TINYINT(1),
    OUT p_error_code VARCHAR(50),
    OUT p_error_message VARCHAR(255)
)
proc_body: BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_success = 0;
        GET DIAGNOSTICS CONDITION 1 p_error_message = MESSAGE_TEXT;
    END;

    SET p_success = 0;
    START TRANSACTION;

    -- Logika bisnis

    COMMIT;
    SET p_success = 1;
END proc_body$$
DELIMITER ;
```

## Alur Transaksi Klien

### Alur VB.NET (FormPenjualan.vb)
1. Panggil `sp_hlp_faktur_generate('PJ', ...)` → dapat nomor
2. Terapkan `TerapkanModeDTP()` untuk validasi backdate
3. `conn.BeginTransaction()`
4. INSERT header ke `penjualan` (inline)
5. Loop items: INSERT detail, `HistoryBarang`, UPDATE counter `PENJUALAN_TOKO`
6. Panggil `HitungStokPerubahan()` per item
7. INSERT jurnal ke `JurnalUmum` (inline)
8. Panggil `UpdateSaldoAkun()` per akun
9. `conn.Commit()`

### Alur PHP (sync_penjualan.php)
1. Panggil `CALL sp_hlp_faktur_generate('PJ', ...)` → dapat nomor
2. `$pdo->beginTransaction()`
3. Loop items: `CALL sp_hlp_stok_validasi(...)` per item
4. INSERT header ke `penjualan` (inline)
5. Loop items: INSERT detail, `HistoryBarang`, UPDATE counter `PENJUALAN_TOKO`
6. `CALL sp_hlp_stok_hitung(...)` per item
7. INSERT jurnal ke `JurnalUmum` (inline)
8. `CALL sp_hlp_saldo_akun_update(...)` per akun
9. `$pdo->commit()`

## Perbaikan Bug Penting

### Bug 1: HITUNGSEMUASALDO Step 3 (FormLapNeracaLR.vb)
- **Masalah:** Menggunakan `SALDO_SEBELUMNYA` untuk LABA/RUGI
- **Perbaikan:** Ganti dengan `SALDO_AKHIR`

### Bug 2: UpdateSaldoAkun (ModuleVariabel.vb)
- **Masalah:** Tidak menghormati `AKUN_DK`
- **Perbaikan:**
  ```sql
  CASE
    WHEN AKUN_DK = 'DEBET'  THEN Saldo_Awal + DEBET - KREDIT
    WHEN AKUN_DK = 'KREDIT' THEN Saldo_Awal - DEBET + KREDIT
  END
  ```

## Daftar Form Terpengaruh

| Folder | Form | GN | VS | HS | PJ | US | VB | VK |
|--------|------|:--:|:--:|:--:|:--:|:--:|:--:|:--:|
| 2Trans | FormPenjualan | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | — |
| 2Trans | FormPembelian | ✅ | — | ✅ | ✅ | ✅ | ✅ | — |
| 2Trans | FormReturPenjualan | ✅ | — | ✅ | ✅ | ✅ | ✅ | — |
| 2Trans | FormReturBeli | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | — |
| 2Trans | FormStokOpname | ✅ | — | ✅ | ✅ | ✅ | ✅ | — |
| 2Trans | FormTransferBarang | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | — |
| 2Trans | FormBayarHutang | ✅ | — | — | ✅ | ✅ | ✅ | ✅ |
| 3Jurnal | FormKeuangan | — | — | — | ✅ | ✅ | — | — |
| 4Gaji | FormGaji | ✅ | — | — | ✅ | ✅ | — | — |
| 4Gaji | FormBon | ✅ | — | — | ✅ | ✅ | — | — |
| 0Form | FormUtama | — | — | ✅ | ✅ | ✅ | — | — |
| 0Form | FormLoading | — | — | ✅ | — | ✅ | — | — |

## Urutan Migrasi

1. **Fase 1** — Helper SP & Batch SP ✅
2. **Fase 2** — Penjualan ✅
3. **Fase 3** — Pembelian ✅
4. **Fase 4** — Retur & Opname ✅
5. **Fase 5** — Transfer & Bayar ✅
6. **Fase 6** — Hapus Transaksi ✅
7. **Fase 7** — Gaji & Bon ✅
8. **Fase 8** — Batch, Sync & Bug Fix ✅ (Requirement 17 & 18 selesai)
9. **Fase 9** — Master dengan Jurnal ✅
