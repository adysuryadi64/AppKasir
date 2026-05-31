**Catatan Rilis Teknis Kasir - Versi 15.2026.531.5**

**Peristiwa Perubahan:**
1. **UI/UX dan Navigation**
   - **`FormLoading.vb`**
     - Menambahkan tiga menu utama baru: "Master Poin", "Tukar Poin", dan "Lap Poin". 
     - Potensi mengaruh pada waktu startup minimal, pantau performa.

2. **Modul Utama (Dashboard)**
   - **`FormUtama.designer.vb`**
     - Tambahkan otomatisasi menu untuk fitur poin (PoinToolStripMenuItem, MutasiPoinToolStripMenuItem, dll). 
     - Potensi impact: struktur navigasi lebih terorganisasi, berapa dph 3 menu baris baru.

3. **Master Data**
   - **`FormGeneralSetting.vb`**
     - Penambahan kontrol `TxtBatasSatuanSedang` & `TxtBatasSatuanBesar` untuk aturan auto-scaling qty. 
     - Potensi impact: perubahan konfigurasi utamanyaan aplikasi, pastikan dokumentasi akan di-update.
   - **`FormHakUser.vb`**
     - Lisensi pribadi kategori `Master Poin`. 
     - Potensi impact: penyesuaian hak akses di menu master.

4. **Transaksi**
   - **`FormTransferCabang.vb`**
     - Restructuring alur flow transfer: validasi lokasi dengan `CekLokasiBarang()`, notifikasi poin salah sesuai lokasi.
     - Potensi impact: perubahan logika kritis berisiko high-risk, test regressive diperlukan.
   - **`FormTukarPoin.vb`**
     - Integrasi fitur history stok di interface (inisialisasi data alokasi poin via `ModuleLoyaltyPoin.MuatBarangTukar()`). 
     - Potensi impact: UI sesuai persyaratan lisensi.

5. **Audit & Jurnal**
   - **`ModuleAuditTrail.vb`**
     - Persiapan untuk audit stok transaksi poin (CEO auditor akan love ini).
   - **`FormUtama.vb`**
     - Diperubah logic cetak surat jalan: tambahan validasi `CekLokasiBarang()` sebelum proses. 

6. **Fix & Refactoring**
   - **Database Scripts**
     - Menghapus kolom `MINIMUM_REDEEM` dari `poin_config` (35_migrasi):
       ```sql
       CALL DropColumnSafely('poin_config', 'MINIMUM_REDEEM');
       ```
       Lanjutkan dengan aturan baru provisi poin per item.
     - Tambahan kolom `URUTAN` di tabel detail transaksi (36_migrasi) untuk melestarikan cetakan receipt berurutan.

7. **Reporting**
   - Menambahkan report baru:
     - `FormLapPoin`: Rekap saldos per pelanggan (`ReportSaldoPoin`)
     - Mutasi poin (`ReportMutasiPoin`) dan rekap tukar poin (`ReportRekapTukarPoin`). 
   - Laporanè¿äº driven oleh data dari `poin_ledger`, `Transaction`, dan `tbl_barang`.

**Rekomendasi Penerimaan:**
- Test interoperabilitas moduler dengan tropo berurutan
- Validasi struktur data history poin di mode edit Tukar Poin
- Pastikan simpul DB untuk audit stok update secara konsisten
- Update panduan pengguna untuk refSAFETY tab form utama nilai-basis

*Tag: 15.2026.531.5 (2026-02-01)*  
*Catatan Penulis: Kelas refactoring `SaleService` diperbolehkan untuk v15.2026.532.0 (sabtu injekture).*

