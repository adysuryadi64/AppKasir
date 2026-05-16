# Requirements — Migrasi FormReturBeli: ListBox → ListView + Perbaikan DGV

## Latar Belakang

`FormReturBeli` saat ini menggunakan `ListBox` untuk menampilkan hasil pencarian barang.
Tujuan migrasi ini adalah:
1. Mengganti `LstBarang` dari `ListBox` ke `ListView` agar bisa menampilkan kolom Nama Barang dan Stok secara terpisah dan rapi
2. Memperbaiki `EditMode` DGV dari `EditOnEnter` (global) ke `EditOnKeystrokeOrF2` + `CellEnter` khusus kolom `SATUAN`
3. Menambahkan pencarian inline dari kolom `NAMA_BARANG` di DGV (seperti FormJual)
4. Memperbaiki semua pola yang sudah terbukti bermasalah di FormJual/FormPembelian

## Referensi

- `2Trans/FormJual.vb` — referensi utama implementasi ListView + DGV inline search
- `2Trans/FormReturBeli.vb` — file yang akan dimodifikasi
- `2Trans/FormReturBeli.Designer.vb` — file designer yang akan dimodifikasi
- `.kiro/steering/pola-form-transaksi.md` — pola teknis permanen
- `.kiro/steering/panduan-migrasi-form-transaksi.md` — panduan migrasi

---

## Requirements

### REQ-01: Ganti LstBarang dari ListBox ke ListView

**Deskripsi:**
`LstBarang` saat ini adalah `System.Windows.Forms.ListBox`. Harus diganti ke `System.Windows.Forms.ListView` dengan tampilan detail (2 kolom).

**Kriteria Penerimaan:**
- [ ] `LstBarang` di Designer berubah tipe dari `ListBox` ke `ListView`
- [ ] `ListView` dikonfigurasi: `View = Details`, `FullRowSelect = True`, `GridLines = True`, `MultiSelect = False`
- [ ] Kolom 1: "Nama Barang" — lebar mengisi sisa ruang (FillWeight atau lebar tetap ~430px)
- [ ] Kolom 2: "Stok" — lebar ~80px, rata kanan
- [ ] `LstBarang` tidak lagi punya `DrawMode = OwnerDrawFixed` (tidak relevan untuk ListView)
- [ ] Ukuran dan posisi `LstBarang` tetap sama seperti sebelumnya

**Catatan:** Perubahan ini dilakukan di Designer Visual Studio, bukan lewat kode.

---

### REQ-02: Ubah EditMode DGV ke EditOnKeystrokeOrF2

**Deskripsi:**
`DgvData.EditMode` saat ini `EditOnEnter` (global). Ini menyebabkan semua kolom langsung masuk edit mode saat sel aktif — termasuk kolom yang tidak perlu. Harus diubah ke `EditOnKeystrokeOrF2`.

**Kriteria Penerimaan:**
- [ ] `DgvData.EditMode = EditOnKeystrokeOrF2` di Designer
- [ ] Kolom `SATUAN` (ComboBox) tetap bisa langsung dibuka dropdown saat sel aktif — via `CellEnter` handler
- [ ] Kolom `NAMA_BARANG` tetap bisa langsung diketik saat sel aktif — via `CellEnter` handler
- [ ] Kolom `QTY` dan `HARGA_BELI_TERAKHIR` hanya masuk edit saat user mulai mengetik atau tekan F2

**Catatan:** Perubahan `EditMode` dilakukan di Designer. Handler `CellEnter` ditambahkan di kode.

---

### REQ-03: Tambah Variabel Baru untuk State Management

**Deskripsi:**
Variabel-variabel berikut belum ada di FormReturBeli dan diperlukan untuk pola pencarian ListView yang benar.

**Kriteria Penerimaan:**
- [ ] `_dgvEditingTextBox As TextBox = Nothing` — referensi ke TextBox editing control di DGV
- [ ] `_sedangPindahKeLstBarang As Boolean = False` — flag saat fokus pindah ke ListView
- [ ] `_rowSaatPindahKeLst As Integer = -1` — baris DGV saat pindah ke ListView
- [ ] `_lstBarangSelectedIndex As Integer = -1` — index item terpilih di ListView
- [ ] `_lstBarangBaruMasuk As Boolean = False` — flag saat ListView baru mendapat fokus dari DGV
- [ ] `_konteksLstBarang As String = "TXTNAMA"` — konteks pencarian: "TXTNAMA" atau "DGV"
- [ ] `_sedangSetNilaiDariListBox As Boolean = False` — guard agar `CellEndEdit` tidak terpicu saat isi baris dari ListView

---

### REQ-04: Ganti SearchBarangByText agar Isi ListView

**Deskripsi:**
`SearchBarangByText` saat ini mengisi `LstBarang.Items.Add(item)` (ListBox API). Harus diganti agar mengisi `ListView` dengan `ListViewItem` yang punya 2 sub-item (Nama + Stok).

**Kriteria Penerimaan:**
- [ ] `SearchBarangByText` menggunakan `LstBarang.Items.Add(lvi)` dengan `ListViewItem`
- [ ] Kolom pertama ListViewItem = Nama Barang
- [ ] Kolom kedua (SubItem) = Stok dengan format `N0`
- [ ] Stok ≤ 0 → teks stok "0" dengan warna merah (atau tanda "-")
- [ ] `LstBarang.Items.Count` dipakai untuk cek apakah ada hasil
- [ ] `AturTinggiListBarang` disesuaikan untuk `ListView` (pakai `LstBarang.Items.Count * tinggiItem`)
- [ ] `LstBarang.Visible = True/False` tetap berfungsi

---

### REQ-05: Ganti AmbilDataDariListBox agar Baca dari ListView

**Deskripsi:**
`AmbilDataDariListBox` saat ini membaca `LstBarang.SelectedItem` (ListBox API). Harus diganti agar membaca dari `ListView.SelectedItems` atau `_lstBarangSelectedIndex`.

**Kriteria Penerimaan:**
- [ ] Baca nama barang dari `LstBarang.Items(_lstBarangSelectedIndex).Text` (kolom pertama)
- [ ] Baca ID barang dari `LstBarang.Items(_lstBarangSelectedIndex).Tag` (disimpan saat isi ListView)
- [ ] Guard `_sedangSetNilaiDariListBox = True` aktif dari awal sampai `IsiBarangKeRow`/`TambahDataLangsung` selesai
- [ ] `_sedangSetNilaiDariListBox = False` hanya dimatikan setelah semua proses selesai
- [ ] Setelah ambil data, `LstBarang.Visible = False` dan fokus kembali ke `TxtNama` atau DGV sesuai konteks

---

### REQ-06: Ganti LstBarang_KeyDown untuk ListView

**Deskripsi:**
`LstBarang_KeyDown` saat ini menggunakan `LstBarang.SelectedIndex` (ListBox API). Harus diganti untuk `ListView`.

**Kriteria Penerimaan:**
- [ ] `Keys.Enter` → panggil `AmbilDataDariListBox()`, update `_lstBarangSelectedIndex` dari item yang terfokus
- [ ] `Keys.Down` → pindah ke item berikutnya, update `_lstBarangSelectedIndex`
- [ ] `Keys.Up` → pindah ke item sebelumnya; jika di item pertama, kembali ke `TxtNama` atau DGV
- [ ] `Keys.Escape` → sembunyikan ListView, kembali ke `TxtNama`
- [ ] `_lstBarangBaruMasuk` flag: saat ListView baru mendapat fokus dari DGV (panah bawah), item pertama dipilih via `BeginInvoke` untuk mencegah "bocor" ke item ke-2

---

### REQ-07: Ganti LstBarang_MouseClick untuk ListView

**Deskripsi:**
`LstBarang_MouseClick` saat ini menggunakan `LstBarang.SelectedIndex` (ListBox API).

**Kriteria Penerimaan:**
- [ ] Klik pada item ListView → update `_lstBarangSelectedIndex` dari item yang diklik
- [ ] Panggil `AmbilDataDariListBox()`
- [ ] Sembunyikan ListView setelah pilih

---

### REQ-08: Tambah Pencarian Inline dari Kolom NAMA_BARANG di DGV

**Deskripsi:**
Saat ini, saat user mengetik di kolom `NAMA_BARANG` di DGV, tidak ada ListView yang muncul. Harus ditambahkan seperti di FormJual.

**Kriteria Penerimaan:**
- [ ] `DgvData_EditingControlShowing` mendeteksi kolom `NAMA_BARANG` (index 1)
- [ ] Saat kolom `NAMA_BARANG` aktif, attach handler `DgvNamaBarang_TextChanged`, `DgvNamaBarang_KeyDown`, `DgvNamaBarang_PreviewKeyDown` ke TextBox editing control
- [ ] Remove handler lama sebelum attach yang baru (cegah duplikasi)
- [ ] `DgvNamaBarang_TextChanged` → parse format `qty*nama`, feed ke `SearchBarangByText`, posisikan ListView di bawah sel aktif
- [ ] `DgvNamaBarang_KeyDown` → `Keys.Down` pindah fokus ke ListView; `Keys.Escape` sembunyikan ListView
- [ ] `PosisikanLstBarangDiBawahSel()` — posisikan ListView tepat di bawah sel NAMA_BARANG yang sedang diedit
- [ ] `_konteksLstBarang = "DGV"` saat pencarian dari DGV

---

### REQ-09: Tambah CellEnter Handler untuk Kolom SATUAN dan NAMA_BARANG

**Deskripsi:**
Dengan `EditMode = EditOnKeystrokeOrF2`, kolom `SATUAN` (ComboBox) tidak otomatis buka dropdown saat sel aktif. Perlu `CellEnter` handler.

**Kriteria Penerimaan:**
- [ ] `DgvData_CellEnter` handler ditambahkan
- [ ] Kolom `SATUAN`: `BeginInvoke` → `BeginEdit(True)` + `combo.DroppedDown = True`
- [ ] Kolom `NAMA_BARANG`: `BeginInvoke` → `BeginEdit(True)` agar user bisa langsung ketik
- [ ] Gunakan pola `BeginInvoke` (bukan langsung) untuk menghindari `reentrant call` exception

---

### REQ-10: Tambah SetupFocusToGrid (Ganti Fokuskepencarianbarang)

**Deskripsi:**
`Fokuskepencarianbarang()` saat ini menggunakan `Rows.Add()` langsung yang bisa menyebabkan baris ekstra. Harus diganti dengan `SetupFocusToGrid` yang menggunakan pola `IsNewRow`.

**Kriteria Penerimaan:**
- [ ] `SetupFocusToGrid` ditambahkan dengan pola dari `pola-form-transaksi.md`
- [ ] Nama kolom kode disesuaikan: `"ID_BARANG"` (bukan `"Kode"`)
- [ ] Index kolom NamaBarang disesuaikan: index `1` = `NAMA_BARANG`
- [ ] `CurrentCell = DgvData(1, targetRow)` dilakukan **synchronous** sebelum `BeginInvoke`
- [ ] `BeginEdit` dilakukan via nested `BeginInvoke`
- [ ] `Fokuskepencarianbarang()` diganti dengan `SetupFocusToGrid()` di semua tempat yang memanggilnya
- [ ] `SetupFocusToGrid` **tidak** dipanggil dari jalur barang tidak ditemukan di `CellEndEdit`

---

### REQ-11: Tambah Guard _sedangSetNilaiDariListBox di CellEndEdit

**Deskripsi:**
`DgvData_CellEndEdit` saat ini tidak punya guard. Saat `AmbilDataDariListBox` memanggil `DgvData.EndEdit(True)`, `CellEndEdit` bisa terpicu dan memproses data yang belum lengkap.

**Kriteria Penerimaan:**
- [ ] Baris pertama `DgvData_CellEndEdit`: `If _sedangSetNilaiDariListBox Then Return`
- [ ] Guard ini mencegah pemrosesan ganda saat isi baris dari ListView

---

### REQ-12: Perbaiki Warna Hardcoded → ModuleTheme

**Deskripsi:**
Ada beberapa warna hardcoded yang harus diganti dengan token semantik `ModuleTheme`.

**Kriteria Penerimaan:**
- [ ] `TxtNama_GotFocus`: `Color.Yellow` → `ModuleTheme.C(ModuleTheme.L_SearchFocusBg, ModuleTheme.D_SearchFocusBg)`
- [ ] `HighlightProblemRow`: `Color.LightCoral` → `ModuleTheme.C(ModuleTheme.L_DgvRowPeringatan, ModuleTheme.D_DgvRowPeringatan)`
- [ ] `LstBarang_DrawItem` dihapus (tidak relevan untuk ListView) — ListView tidak pakai `DrawItem`

---

### REQ-13: Perbaiki ProsesMergeBarisDuplikat

**Deskripsi:**
`ProsesMergeBarisDuplikat` menggunakan `SendKeys.Send("{down}")` yang tidak aman.

**Kriteria Penerimaan:**
- [ ] `SendKeys.Send("{down}")` dihapus
- [ ] Setelah hapus baris duplikat, panggil `SetupFocusToGrid()` atau tidak perlu navigasi (cukup `UpdateSemuaTotal`)
- [ ] Tambah `Exit Sub` setelah hapus baris duplikat untuk mencegah `ArgumentOutOfRangeException`

---

### REQ-14: Tambah PosisikanLstBarangDiBawahSel dan PosisikanLstBarangDiBawahTxtNama

**Deskripsi:**
Fungsi untuk memposisikan ListView tepat di bawah kontrol yang sedang aktif (sel DGV atau TxtNama).

**Kriteria Penerimaan:**
- [ ] `PosisikanLstBarangDiBawahSel()` — posisikan di bawah sel `NAMA_BARANG` yang sedang diedit di DGV
- [ ] `PosisikanLstBarangDiBawahTxtNama()` — posisikan di bawah `TxtNama` (untuk pencarian dari TxtNama)
- [ ] Dipanggil dari `DgvNamaBarang_TextChanged` dan `SearchDebounceTimer_Tick`

---

### REQ-15: Hapus/Nonaktifkan Kode Lama yang Tidak Relevan

**Deskripsi:**
Beberapa kode lama perlu dihapus atau dinonaktifkan setelah migrasi.

**Kriteria Penerimaan:**
- [ ] `LstBarang_DrawItem` dihapus (ListBox-specific, tidak relevan untuk ListView)
- [ ] `LstBarang_GotFocus` dan `LstBarang_LostFocus` disesuaikan untuk ListView
- [ ] `LstBarang_Enter` disesuaikan untuk ListView
- [ ] `GetTextAfterAsterisk` bisa dipertahankan atau dihapus jika tidak dipakai lagi
- [ ] `ListBarangItem` class bisa dipertahankan (masih dipakai untuk menyimpan data di `Tag`)
- [ ] `AutoText_TextChanged` dan `AddItems` (AutoComplete DGV) — pertimbangkan apakah masih dipakai setelah ada pencarian inline

---

## Urutan Pengerjaan

### Fase 1 — Perubahan Designer (dilakukan manual di Visual Studio)
1. Ganti `LstBarang` dari `ListBox` ke `ListView` (REQ-01)
2. Ubah `DgvData.EditMode` ke `EditOnKeystrokeOrF2` (REQ-02)

### Fase 2 — Perubahan Kode (dikerjakan oleh AI)
3. Tambah variabel baru (REQ-03)
4. Ganti `SearchBarangByText` untuk ListView (REQ-04)
5. Ganti `AmbilDataDariListBox` untuk ListView (REQ-05)
6. Ganti `LstBarang_KeyDown` untuk ListView (REQ-06)
7. Ganti `LstBarang_MouseClick` untuk ListView (REQ-07)
8. Tambah pencarian inline DGV: `DgvNamaBarang_TextChanged`, `DgvNamaBarang_KeyDown`, `DgvNamaBarang_PreviewKeyDown` (REQ-08)
9. Ganti `DgvData_EditingControlShowing` untuk attach handler DGV (REQ-08)
10. Tambah `DgvData_CellEnter` handler (REQ-09)
11. Tambah `SetupFocusToGrid`, ganti semua pemanggil `Fokuskepencarianbarang` (REQ-10)
12. Tambah guard di `CellEndEdit` (REQ-11)
13. Perbaiki warna hardcoded (REQ-12)
14. Perbaiki `ProsesMergeBarisDuplikat` (REQ-13)
15. Tambah `PosisikanLstBarangDiBawahSel` dan `PosisikanLstBarangDiBawahTxtNama` (REQ-14)
16. Hapus/sesuaikan kode lama (REQ-15)

### Fase 3 — Verifikasi
17. Jalankan PowerShell comparison fungsi (tidak ada fungsi bisnis yang hilang)
18. Verifikasi nama kolom DGV case-sensitive
19. Test manual: pencarian dari TxtNama, pencarian dari DGV, barcode, navigasi keyboard

---

## Checklist Akhir

- [ ] PowerShell: tidak ada fungsi bisnis yang hilang
- [ ] `_sedangSetNilaiDariListBox` guard ada di `CellEndEdit`
- [ ] `_lstBarangBaruMasuk` flag ada di `LstBarang_KeyDown`
- [ ] `_lstBarangSelectedIndex` diupdate di semua path (KeyDown, MouseClick)
- [ ] `SetupFocusToGrid` menggunakan nested `BeginInvoke`
- [ ] `SetupFocusToGrid` menggunakan `"ID_BARANG"` sebagai nama kolom kode
- [ ] `CellEndEdit` menggunakan `e.RowIndex` bukan `DgvData.CurrentCell.RowIndex`
- [ ] Tidak ada `SendKeys.Send("{down}")` — sudah diganti
- [ ] Tidak ada `Color.Yellow` hardcoded — sudah pakai `ModuleTheme`
- [ ] Tidak ada `Color.LightCoral` hardcoded — sudah pakai `ModuleTheme`
- [ ] `EditMode = EditOnKeystrokeOrF2` di Designer
- [ ] `CellEnter` handler ada untuk kolom `SATUAN` dan `NAMA_BARANG`
- [ ] `DgvNamaBarang_TextChanged` terhubung via `EditingControlShowing`
- [ ] `PosisikanLstBarangDiBawahSel` dan `PosisikanLstBarangDiBawahTxtNama` ada
- [ ] ListView punya 2 kolom: Nama Barang + Stok
- [ ] `Tag` di setiap `ListViewItem` menyimpan `ID_BARANG`
- [ ] `AmbilDataDariListBox` membaca dari `_lstBarangSelectedIndex` bukan `SelectedItem`
