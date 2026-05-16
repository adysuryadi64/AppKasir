# Tasks — Perbaikan Histori Hutang/Piutang

## Task 1: Create Database Migration Script
**File:** `Database/18_migrasi_hutang_piutang_detail.sql`
**Requirements:** 1, 2

- [x] 1.1 Create new SQL file — jalankan UPDATE dulu sebelum ALTER (agar NULL tidak error)
- [x] 1.2 Add `UPDATE hutang_detail SET JENIS = 'BAYAR' WHERE JENIS IS NULL OR JENIS = ''`
- [x] 1.3 Add `ALTER TABLE hutang_detail MODIFY COLUMN JENIS VARCHAR(10) NOT NULL DEFAULT 'BAYAR'`
- [x] 1.4 Add `UPDATE piutang_detail SET JENIS = 'BAYAR' WHERE JENIS IS NULL OR JENIS = ''`
- [x] 1.5 Add `ALTER TABLE piutang_detail MODIFY COLUMN JENIS VARCHAR(20) NOT NULL DEFAULT 'BAYAR'`
- [x] 1.6 Write idempotent INSERT for hutang_detail TIMBUL rows from pembelian table:
  - Filter: `STATUS_TRANSAKSI_BELI = 'Belum Lunas'` (nilai enum yang benar di DB)
  - Isi `DIBAYAR` dari `pembelian.PEMBAYARAN` (bukan 0)
  - Isi `RETUR` dari `pembelian.RETUR` (bukan 0)
  - Isi `HUTANG` dari `pembelian.TAGIHAN`
  - Isi `TOTAL_HUTANG` dari `pembelian.GRAND_TOTAL_BELI`
- [x] 1.7 Write idempotent INSERT for piutang_detail TIMBUL rows from penjualan table:
  - Filter: `STATUS_TRANSAKSI IN ('Belum Lunas', 'TERHUTANG')` (kedua nilai ada di DB produksi)
  - Isi `PIUTANG` dari `penjualan.GRAND_TOTAL_STL_PAJAK` (bukan GRAND_TOTAL)
  - Isi `DIBAYAR` dari `penjualan.NOMINALBAYARPIUTANG` (bukan 0)
  - Isi `HUTANG` dari `penjualan.SISA_TAGIHAN`
- [x] 1.8 Add performance indexes: `idx_hutang_detail_jenis_beli` dan `idx_piutang_detail_jenis_jual`
- [x] 1.9 Add comments about backup requirement and idempotent nature
- [x] 1.10 Test migration script on backup database to verify no duplicate rows on re-run

---

## Task 2: Modify FormPembelian.vb — TIMBUL Recording on New Save
**File:** `2Trans/FormPembelian.vb`
**Requirements:** 3

- [x] 2.1 Locate the save function where `INSERT INTO pembelian` occurs
- [x] 2.2 After successful pembelian INSERT, add conditional check: `If sisaHutang > 0 Then`
- [x] 2.3 Create MySqlCommand to INSERT into hutang_detail with JENIS='TIMBUL'
- [x] 2.4 Set ID_BAYAR parameter with format 'TIMBUL-{ID_PEMBELIAN}'
- [x] 2.5 Set TOTAL_HUTANG from GRAND_TOTAL_BELI value
- [x] 2.6 Set HUTANG from TAGIHAN (remaining debt) value
- [x] 2.7 Set DIBAYAR=0, RETUR=0, PEMBAYARAN=0 for TIMBUL row
- [x] 2.8 Set JATUH_TEMPO from faktur due date
- [x] 2.9 Set STATUS='Belum Lunas' for new TIMBUL row
- [x] 2.10 Ensure INSERT is within same database transaction as pembelian INSERT
- [ ] 2.11 Test: Create new credit purchase → Verify hutang_detail has TIMBUL row with correct values

---

## Task 3: Modify FormPembelian.vb — Handle Delete/Edit of Credit Purchases
**File:** `2Trans/FormPembelian.vb`
**Requirements:** 4

- [x] 3.1 Locate delete/edit function for pembelian records
- [x] 3.2 Before delete/edit, add query to check if JENIS='BAYAR' rows exist in hutang_detail
- [x] 3.3 If payments exist, show warning MessageBox to user before proceeding
- [x] 3.4 Add DELETE statement to remove TIMBUL row: `DELETE FROM hutang_detail WHERE ID_BELI = @ID_BELI AND JENIS = 'TIMBUL'`
- [x] 3.5 Ensure DELETE is within same transaction as pembelian delete/edit
- [x] 3.6 On re-save after edit, re-insert TIMBUL row with updated values (reuse Task 2 logic)
- [ ] 3.7 Test: Edit credit purchase with no payments → Verify TIMBUL row deleted and recreated
- [ ] 3.8 Test: Edit credit purchase with payments → Verify warning appears

---

## Task 4: Modify FormBayarHutang.vb — BAYAR Recording and TIMBUL Update
**File:** `2Trans/FormBayarHutang.vb`
**Requirements:** 5

- [x] 4.1 Locate BtnBayar_Click function (around line 312)
- [x] 4.2 Find existing INSERT into hutang_detail (around line 380)
- [x] 4.3 Tambahkan kolom `JENIS` ke INSERT query dengan nilai hardcode `'BAYAR'` (bukan dibaca dari variabel)
- [x] 4.4 After existing INSERT (around line 421), add UPDATE statement for TIMBUL row
- [x] 4.5 UPDATE should: HUTANG = HUTANG - @BAYAR, DIBAYAR = DIBAYAR + @BAYAR
- [x] 4.6 UPDATE STATUS with CASE: 'Lunas' if HUTANG<=0, else 'Belum Lunas'
- [x] 4.7 Ensure UPDATE targets: WHERE ID_BELI = @ID_BELI AND JENIS = 'TIMBUL'
- [x] 4.8 Wrap both INSERT and UPDATE in same transaction
- [x] 4.9 Add error handling: if TIMBUL row not found, continue without error (legacy invoices)
- [ ] 4.10 Test: Pay invoice partially → Verify BAYAR row inserted and TIMBUL HUTANG reduced
- [ ] 4.11 Test: Pay invoice fully → Verify TIMBUL STATUS becomes 'Lunas'

---

## Task 5: Modify FormJual.vb — TIMBUL Recording on New Credit Sale
**File:** `2Trans/FormJual.vb`
**Requirements:** 6

- [x] 5.1 Locate the save function where `INSERT INTO penjualan` occurs
- [x] 5.2 After successful penjualan INSERT, add conditional check: `If sisaTagihan > 0 Then`
- [x] 5.3 Create MySqlCommand to INSERT into piutang_detail with JENIS='TIMBUL'
- [x] 5.4 Set ID_BAYAR parameter with format 'TIMBUL-{ID_PENJUALAN}'
- [x] 5.5 Set `piutang_detail.PIUTANG` from `penjualan.GRAND_TOTAL_STL_PAJAK` value (bukan GRAND_TOTAL)
- [x] 5.6 Set `piutang_detail.HUTANG` from `penjualan.SISA_TAGIHAN` (remaining receivable) value
- [x] 5.7 Set DIBAYAR=0, RETUR=0, PEMBAYARAN=0 for TIMBUL row
- [x] 5.8 Set JATUH_TEMPO from faktur due date
- [x] 5.9 Set STATUS='Belum Lunas' for new TIMBUL row
- [x] 5.10 Ensure INSERT is within same database transaction as penjualan INSERT
- [ ] 5.11 Test: Create new credit sale → Verify piutang_detail has TIMBUL row with correct values

---

## Task 6: Modify FormJual.vb — Handle Delete/Edit of Credit Sales
**File:** `2Trans/FormJual.vb`
**Requirements:** 7

- [x] 6.1 Locate delete/edit function for penjualan records
- [x] 6.2 Before delete/edit, add query to check if JENIS='BAYAR' rows exist in piutang_detail
- [x] 6.3 If payments exist, show warning MessageBox to user before proceeding
- [x] 6.4 Add DELETE statement to remove TIMBUL row: `DELETE FROM piutang_detail WHERE ID_JUAL = @ID_JUAL AND JENIS = 'TIMBUL'`
- [x] 6.5 Ensure DELETE is within same transaction as penjualan delete/edit
- [x] 6.6 On re-save after edit, re-insert TIMBUL row with updated values (reuse Task 5 logic)
- [ ] 6.7 Test: Edit credit sale with no payments → Verify TIMBUL row deleted and recreated
- [ ] 6.8 Test: Edit credit sale with payments → Verify warning appears

---

## Task 7: Modify FormBayarPiutang.vb — BAYAR Recording and TIMBUL Update
**File:** `2Trans/FormBayarPiutang.vb`
**Requirements:** 8

- [x] 7.1 Locate BtnBayar_Click function (around line 319)
- [x] 7.2 Find existing INSERT into piutang_detail (around line 390)
- [x] 7.3 Ganti baca JENIS dari `DgvData.Cells(4).Value` menjadi hardcode `'BAYAR'` di INSERT query
- [x] 7.4 After existing INSERT (around line 431), add UPDATE statement for TIMBUL row
- [x] 7.5 UPDATE should: HUTANG = HUTANG - @BAYAR, DIBAYAR = DIBAYAR + @BAYAR
- [x] 7.6 UPDATE STATUS with CASE: 'Lunas' if HUTANG<=0, else 'Belum Lunas'
- [x] 7.7 Ensure UPDATE targets: WHERE ID_JUAL = @ID_JUAL AND JENIS = 'TIMBUL'
- [x] 7.8 Wrap both INSERT and UPDATE in same transaction
- [x] 7.9 Add error handling: if TIMBUL row not found, continue without error (legacy invoices)
- [ ] 7.10 Test: Receive partial payment → Verify BAYAR row inserted and TIMBUL HUTANG reduced
- [ ] 7.11 Test: Receive full payment → Verify TIMBUL STATUS becomes 'Lunas'

---

## Task 8: Modify FormEditBayarJual.vb — Update TIMBUL on Payment Edit
**File:** `2Trans/FormEditBayarJual.vb`
**Requirements:** 8b

- [x] 8.1 Locate save function for payment changes (search for UPDATE penjualan or BtnSimpan_Click)
- [x] 8.2 After updating SISA_TAGIHAN in penjualan table, add UPDATE for piutang_detail TIMBUL row
- [x] 8.3 UPDATE should set: HUTANG = @SISA_TAGIHAN_BARU, DIBAYAR = @TOTAL_BAYAR_BARU
- [x] 8.4 UPDATE STATUS with CASE: 'Lunas' if SISA_TAGIHAN_BARU=0, else 'Belum Lunas'
- [x] 8.5 Ensure UPDATE targets: WHERE ID_JUAL = @ID_JUAL AND JENIS = 'TIMBUL'
- [x] 8.6 Wrap in same transaction as penjualan update
- [x] 8.7 Add error handling: if TIMBUL row not found, continue without error (do not create new row)
- [ ] 8.8 Test: Edit payment amount → Verify TIMBUL HUTANG and DIBAYAR updated correctly

---

## Task 9: Modify FormReturPembelian.vb — RETUR Recording
**File:** `2Trans/FormReturPembelian.vb`
**Requirements:** 9

- [x] 9.1 Locate save function for retur (search for BtnSimpan_Click or INSERT into retur_pembelian)
- [x] 9.2 Add conditional: Only proceed if Mode Normal (CbJenisRetur.Checked = False) AND CbPotongHutang.Checked = True
- [x] 9.3 After INSERT retur_pembelian, create INSERT for hutang_detail with JENIS='RETUR'
- [x] 9.4 Set ID_BAYAR with format 'RETUR-{ID_RETUR_PEMBELIAN}'
- [x] 9.5 Set ID_BELI to original purchase invoice ID
- [x] 9.6 Set PEMBAYARAN to TOTAL_RUPIAH of retur
- [x] 9.7 After RETUR INSERT, add UPDATE for TIMBUL row: HUTANG = HUTANG - @RETUR, RETUR = RETUR + @RETUR
- [x] 9.8 UPDATE STATUS with CASE: 'Lunas' if HUTANG<=0, else 'Belum Lunas'
- [x] 9.9 Ensure all operations in same database transaction
- [ ] 9.10 Test: Mode Normal + PotongHutang checked → Verify RETUR row inserted and TIMBUL updated
- [ ] 9.11 Test: Mode Bebas → Verify NO rows added to hutang_detail
- [ ] 9.12 Test: PotongHutang unchecked → Verify NO rows added to hutang_detail

---

## Task 10: Modify FormReturPenjualan.vb — RETUR Recording
**File:** `2Trans/FormReturPenjualan.vb`
**Requirements:** 10

- [x] 10.1 Locate save function for retur (search for BtnSimpan_Click or INSERT into retur_penjualan)
- [x] 10.2 Add conditional: Only proceed if Mode Normal (CbJenisRetur.Checked = False) AND CbPotongHutang.Checked = True
- [x] 10.3 After INSERT retur_penjualan, create INSERT for piutang_detail with JENIS='RETUR'
- [x] 10.4 Set ID_BAYAR with format 'RETUR-{ID_RETUR_PENJUALAN}'
- [x] 10.5 Set ID_JUAL to original sales invoice ID
- [x] 10.6 Set PEMBAYARAN to TOTAL_RUPIAH of retur
- [x] 10.7 After RETUR INSERT, add UPDATE for TIMBUL row: HUTANG = HUTANG - @RETUR, RETUR = RETUR + @RETUR
- [x] 10.8 UPDATE STATUS with CASE: 'Lunas' if HUTANG<=0, else 'Belum Lunas'
- [x] 10.9 Ensure all operations in same database transaction
- [ ] 10.10 Test: Mode Normal + PotongHutang checked → Verify RETUR row inserted and TIMBUL updated
- [ ] 10.11 Test: Mode Bebas → Verify NO rows added to piutang_detail
- [ ] 10.12 Test: PotongHutang unchecked → Verify NO rows added to piutang_detail

---

## Task 11: Modify FormLapHutang.vb — Query from hutang_detail
**File:** `5Lap/FormLapHutang.vb`
**Requirements:** 13

- [x] 11.1 Locate function that populates DataGridView for hutang report
- [x] 11.2 Find existing query that SELECTs FROM pembelian WHERE STATUS_TRANSAKSI_BELI='Belum Lunas'
- [x] 11.3 Replace query to SELECT from hutang_detail WHERE JENIS='TIMBUL' AND STATUS='Belum Lunas'
- [x] 11.4 Select columns: ID_BELI (as NO_FAKTUR), NAMA (supplier), TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG (as SISA_HUTANG), JATUH_TEMPO
- [ ] 11.5 JOIN with supplier table if needed for additional info
- [x] 11.6 ORDER BY JATUH_TEMPO ASC
- [x] 11.7 Verify DataGridView column bindings match new query columns
- [ ] 11.8 Test: Run report → Verify data matches old query totals after migration

---

## Task 12: Modify FormLapPiutang.vb — Query from piutang_detail
**File:** `5Lap/FormLapPiutang.vb`
**Requirements:** 14

- [x] 12.1 Locate function that populates DataGridView for piutang report
- [x] 12.2 Find existing query that SELECTs FROM penjualan WHERE STATUS_TRANSAKSI='Belum Lunas'
- [x] 12.3 Replace query to SELECT from piutang_detail WHERE JENIS='TIMBUL' AND STATUS='Belum Lunas'
- [x] 12.4 Select columns: ID_JUAL (as NO_FAKTUR), NAMA (customer), TANGGAL_JUAL, PIUTANG, DIBAYAR, RETUR, HUTANG (as SISA_PIUTANG), JATUH_TEMPO
- [ ] 12.5 JOIN with customer table if needed for additional info
- [x] 12.6 ORDER BY JATUH_TEMPO ASC
- [x] 12.7 Verify DataGridView column bindings match new query columns
- [ ] 12.8 Test: Run report → Verify data matches old query totals after migration

---

## Task 13: Integration Testing and Data Consistency Verification
**Requirements:** 11, 12, 15

- [x] 13.1 Backup production database before any changes
- [x] 13.2 Run migration script (Task 1) on backup database
- [x] 13.3 Verify migration counts: `COUNT(hutang_detail TIMBUL)` = `COUNT(pembelian WHERE STATUS_TRANSAKSI_BELI='Belum Lunas')`
- [x] 13.4 Verify migration counts: `COUNT(piutang_detail TIMBUL)` = `COUNT(penjualan WHERE STATUS_TRANSAKSI IN ('Belum Lunas','TERHUTANG'))`
- [x] 13.5 Run migration script again → Verify no duplicate rows (idempotent test)
- [ ] 13.6 Test complete workflow: Create credit purchase → Pay partial → Pay full → Verify DIBAYAR + RETUR + HUTANG = TOTAL_HUTANG
- [ ] 13.7 Test complete workflow: Create credit sale → Receive partial → Receive full → Verify DIBAYAR + RETUR + HUTANG = PIUTANG
- [ ] 13.8 Compare FormLapHutang total before and after migration (should match)
- [ ] 13.9 Compare FormLapPiutang total before and after migration (should match)
- [ ] 13.10 Test rollback scenarios: Failed save → Verify no orphan rows in hutang_detail/piutang_detail
- [x] 13.11 Performance test: Migration on ~30 kredit aktif di produksi → Verify completes in < 10 minutes
- [ ] 13.12 Document all test results and any issues found

---

## Execution Order

1. **Task 1** — Database migration script (MUST be completed first)
2. **Task 2-3** — FormPembelian modifications (TIMBUL recording)
3. **Task 4** — FormBayarHutang modifications (BAYAR recording)
4. **Task 5-6** — FormJual modifications (TIMBUL recording)
5. **Task 7** — FormBayarPiutang modifications (BAYAR recording)
6. **Task 8** — FormEditBayarJual modifications (TIMBUL update)
7. **Task 9** — FormReturPembelian modifications (RETUR recording)
8. **Task 10** — FormReturPenjualan modifications (RETUR recording)
9. **Task 11** — FormLapHutang modifications (report query)
10. **Task 12** — FormLapPiutang modifications (report query)
11. **Task 13** — Integration testing (MUST be completed last)

## Notes

- All database operations MUST be wrapped in transactions
- All modifications should include error handling with try-catch blocks
- Test each task individually before proceeding to next task
- Maintain backup before running migration on production data
- Coordinate with users before deploying to production
