---
inclusion: manual
---

# Panduan Migrasi Form Transaksi — AppKasir

> Dibuat berdasarkan pengalaman nyata migrasi FormPenjualan → FormJual.
> Ikuti urutan ini untuk menghindari masalah yang sudah pernah terjadi.
>
> **Pola teknis permanen** (SetupFocusToGrid, barisDiisi, barcode, inkonsistensi nama kolom, dll)
> ada di `pola-form-transaksi.md` yang selalu dimuat otomatis.

---

## Metode Migrasi

**Copy file lama → paste sebagai file baru → ubah hanya bagian yang perlu.**

Artinya:
- Semua fitur bisnis sudah ada dari awal (karena copy dari file lama yang sudah terbukti akurat)
- Yang perlu dilakukan hanya: **hapus kode lama yang diganti**, **tambahkan kode baru**, **pertahankan yang sudah benar**
- Bukan menulis ulang dari nol

---

## Prinsip Utama

- **File lama TIDAK dihapus** — tetap dipertahankan sebagai referensi akurat
- **File baru = copy file lama + perubahan minimal** — jangan ubah yang tidak perlu
- **Verifikasi dengan PowerShell** sebelum dan sesudah migrasi
- **Tidak ada git** — zero tolerance untuk kesalahan destruktif

---

## Status Fitur Stok di Form Transaksi

> Banyak form lama belum mendukung fitur stok barang. Target: semua form transaksi konsisten.

| Form | Fitur Stok | Status |
|---|---|---|
| FormPenjualan | StokToko, StokGudang di DGV + CellFormatting merah | Ada (hardcoded Color.Red) |
| FormJual | StokToko, StokGudang di DGV + CellFormatting ModuleTheme | Ada ✅ |
| FormPembelian | Belum diketahui | Perlu cek saat migrasi |
| FormReturJual | Belum diketahui | Perlu cek saat migrasi |
| FormReturBeli | Belum diketahui | Perlu cek saat migrasi |
| FormTransferCabang | Belum diketahui | Perlu cek saat migrasi |

Saat migrasi, **tambahkan fitur stok jika belum ada** menggunakan pola dari FormJual.

---

## Fase 0 — Persiapan (WAJIB sebelum mulai)

### 0.1 Langkah awal

1. **Copy file lama** (misal `FormPembelian.vb`) → **paste sebagai file baru** (misal `FormBeli.vb`)
2. **Rename class** di dalam file baru sesuai nama file baru
3. **Jangan ubah apapun dulu** — baca dan pahami dulu seluruh file

### 0.2 Yang wajib dipahami dari file lama sebelum mulai

- Semua variabel Private di bagian atas form
- Apakah sudah ada variabel barcode (`isBarcodeMode`, `barcodeChars`, dll) atau belum
- Nama kontrol: DGV, TextBox pencarian, ListView/ListBox, Label jenis pelanggan, Label lokasi
- Nama kolom DGV — **bisa berbeda antar form** (misal `Qty` vs `QTY`, `Harga` vs `HargaJual`)
- Apakah sudah ada kolom stok (`Stok`, `StokToko`, `StokGudang`) di DGV atau belum
- Apakah sudah ada `CellFormatting` untuk warna stok atau belum
- Index kolom NamaBarang di DGV (di FormJual = index 1, di form lain bisa berbeda)

### 0.3 Buat daftar fungsi untuk verifikasi akhir

```powershell
# Jalankan di root proyek — ganti nama file sesuai form yang dimigrasikan
$lama = Select-String -Path "2Trans/FormPembelian.vb" -Pattern "^\s*(Private|Public|Protected)\s+(Sub|Function)\s+(\w+)" | ForEach-Object { $_.Matches[0].Groups[3].Value } | Sort-Object

$baru = Select-String -Path "2Trans/FormBeli.vb" -Pattern "^\s*(Private|Public|Protected)\s+(Sub|Function)\s+(\w+)" | ForEach-Object { $_.Matches[0].Groups[3].Value } | Sort-Object

Write-Host "=== FUNGSI BISNIS YANG HILANG (HARUS KOSONG) ==="
Compare-Object $lama $baru | Where-Object { $_.SideIndicator -eq "<=" } | ForEach-Object { $_.InputObject }

Write-Host "=== FUNGSI BARU YANG DITAMBAHKAN ==="
Compare-Object $lama $baru | Where-Object { $_.SideIndicator -eq "=>" } | ForEach-Object { $_.InputObject }
```

**Yang diharapkan saat migrasi selesai:**
- "FUNGSI BISNIS YANG HILANG" → **KOSONG**
- "FUNGSI BARU YANG DITAMBAHKAN" → hanya 10 fungsi pencarian baru

---

## Fase 1 — Yang WAJIB DIPERTAHANKAN (jangan diubah)

Karena file baru adalah copy dari file lama, semua ini sudah ada dan **jangan disentuh**:

- Semua logika bisnis: simpan, edit, hapus, jurnal, audit trail
- Semua fungsi pembayaran: tunai, transfer, piutang
- Semua fungsi draft/tahan
- Semua fungsi cetak nota
- Semua validasi sebelum simpan (`CekStok`, `Cekjualrugi`, `ValidateDataBarangGrid`)
- Semua keyboard shortcut (F1-F12)
- Semua context menu DGV
- Semua fungsi diskon, pajak, biaya kirim
- Nomor faktur otomatis
- Fungsi pelanggan/supplier
- `DgvData_RowPostPaint` (nomor urut baris)
- `DgvData_CellFormatting` jika sudah ada (hanya perbaiki warna hardcoded)
- `UpdateHargaBerdasarJenisPelanggan`
- `HitungNilaiSetiapBaris`, `UpdateSemuaTotal`, `Hitungbaris`

---

## Fase 2 — Yang WAJIB DIHAPUS / DIGANTI dari file lama

### 2.1 Ganti `ProcessManualSearchList`

Form lama memanggil ListBox biasa. Ganti isi fungsinya:

```vb
' Ganti isi lama dengan satu baris ini:
Private Sub ProcessManualSearchList(searchKeyword As String)
    SearchBarangToListBox(searchKeyword, "TXTNAMA")
End Sub
```

### 2.2 Ganti seluruh `AmbilDataDariListBox`

Form lama menggunakan `ListBox.SelectedItem` sederhana. Salin versi FormJual yang:
- Membaca `LstBarang.SelectedIndex` atau `LstBarang.Items.Count = 1`
- Mem-parse nama dari format string `"Nama | T: x | G: y"` dengan split `|`
- Menggunakan `_sedangSetNilaiDariListBox` sebagai guard scope penuh

### 2.3 Ganti `LstBarang_KeyDown`

Salin versi FormJual yang menangani:
- `Keys.Up` di item pertama → kembali ke TextBox + restore `_teksSebelumPindahKeLstBarang`
- `Keys.Enter` → `AmbilDataDariListBox()`
- `Keys.Escape` → tutup ListBox + restore teks

### 2.4 Ganti `SetupFocusToGrid`

Salin versi FormJual yang menggunakan nested `BeginInvoke` dan menambahkan baris baru jika tidak ada baris kosong.

### 2.5 Ganti `ProcessCmdKey` (jika ada)

Salin versi FormJual yang menangani Down key ke ListBox dengan nested `BeginInvoke` + `EndEdit()`.
Lihat pola lengkap di `pola-form-transaksi.md` section "ListBox Pencarian Barang".

### 2.6 Hapus `ClearFocus` jika ada

Fungsi ini tidak dipakai di FormJual — sudah digantikan oleh `SetupFocusToGrid`.

### 2.7 Di `CellEndEdit` — ganti `SendKeys.Send("{down}")`

```vb
' HAPUS:
SendKeys.Send("{down}")

' GANTI DENGAN:
SetupFocusToGrid()
```

### 2.8 Di `CellFormatting` — ganti warna hardcoded

```vb
' HAPUS:
e.CellStyle.BackColor = Color.Red
e.CellStyle.ForeColor = Color.White

' GANTI DENGAN:
e.CellStyle.BackColor = ModuleTheme.C(ModuleTheme.L_Danger, ModuleTheme.D_Danger)
e.CellStyle.ForeColor = Color.White
```

### 2.9 Di `TxtNama_GotFocus` — ganti warna hardcoded

```vb
' HAPUS:
PanelCari.BackColor = Color.Yellow

' GANTI DENGAN:
PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_SearchFocusBg, ModuleTheme.D_SearchFocusBg)
```

---

## Fase 3 — Yang WAJIB DITAMBAHKAN (tidak ada di file lama)

### 3.1 Variabel baru di bagian atas form

Tambahkan setelah variabel yang sudah ada:

```vb
' ===== DGV INLINE EDIT — LISTBOX PENCARIAN =====
Private _dgvEditingTextBox As TextBox = Nothing
Private _sedangPindahKeLstBarang As Boolean = False
Private _teksSebelumPindahKeLstBarang As String = ""
Private _listBoxDibukaDiRow As Integer = -1
Private _listBoxDibukaDiCol As Integer = -1
Private _konteksLstBarang As String = "TXTNAMA"
Private _sedangSetNilaiDariListBox As Boolean = False
```

> Variabel barcode (`isBarcodeMode`, `barcodeChars`, dll) biasanya sudah ada di file lama — cek dulu sebelum menambahkan.
> Flag lama yang TIDAK dipakai lagi: `_rowSaatPindahKeLst`, `_lstBarangSelectedIndex`, `_lstBarangBaruMasuk` — jangan tambahkan.

### 3.2 Tambahkan fungsi pencarian baru

Salin dari FormJual dan sesuaikan nama kontrol/kolom:

| Fungsi | Penyesuaian yang diperlukan |
|---|---|
| `SearchBarangToListBox` | Nama kolom stok, nama label lokasi (`LblLokasiBarang`) |
| `IsiBarangKeRow` | Nama kolom DGV, logika jenis pelanggan (hapus jika tidak ada) |
| `AmbilKodeBarangDariNama` | Tidak perlu perubahan |
| `TutupListBox` | Tidak perlu perubahan |
| `DgvNamaBarang_TextChanged` | Tidak perlu perubahan |
| `DgvNamaBarang_KeyDown` | Tidak perlu perubahan |
| `DgvData_CellLeave` | Tidak perlu perubahan |
| `PosisikanLstBarangDiBawahSel` | Nama kontrol DGV jika berbeda |
| `PosisikanLstBarangDiBawahTxtNama` | Nama kontrol TxtNama jika berbeda |
| `LstBarang_KeyDown` | Tidak perlu perubahan |
| `LstBarang_Click` | Tidak perlu perubahan |
| `LstBarang_SelectedIndexChanged` | Tidak perlu perubahan (tracking only) |

### 3.3 Tambahkan guard di awal `CellEndEdit`

```vb
Private Sub DgvDataData_CellEndEdit(...) Handles DgvData.CellEndEdit
    ' Guard: jangan proses jika sedang diisi dari ListBox
    If _sedangSetNilaiDariListBox Then Return
    ' ... sisa kode lama tetap ...
End Sub
```

### 3.4 Tambahkan blok NamaBarang di `DgvData_EditingControlShowing`

Jika file lama sudah punya `EditingControlShowing`, tambahkan blok ini di dalamnya.
Jika belum ada, buat fungsi baru. Lihat pola lengkap di `pola-form-transaksi.md`.

```vb
Private Sub DgvData_EditingControlShowing(...) Handles DgvData.EditingControlShowing
    ' ⚠️ Sesuaikan index kolom NamaBarang — cek di designer
    If DgvData.CurrentCell.ColumnIndex = 1 AndAlso DgvData.Columns(1).HeaderText = "Nama Barang" Then
        ' KRITIS: skip re-attach saat sedang pindah ke ListBox
        If _sedangPindahKeLstBarang Then Return

        Dim autoText As TextBox = TryCast(e.Control, TextBox)
        If autoText IsNot Nothing Then
            autoText.AutoCompleteMode = AutoCompleteMode.None
            If _dgvEditingTextBox IsNot Nothing Then
                RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                RemoveHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
            End If
            _dgvEditingTextBox = autoText
            AddHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
            AddHandler _dgvEditingTextBox.KeyDown, AddressOf DgvNamaBarang_KeyDown
            PosisikanLstBarangDiBawahSel()
        End If
    Else
        LstBarang.Visible = False
        LstBarang.Items.Clear()
    End If
End Sub
```

### 3.5 Tambahkan fitur stok jika belum ada di form lama

**a. Tambahkan kolom di designer DGV:** `StokToko`, `StokGudang`, `Stok` (ReadOnly, Visible=False default)

**b. Tambahkan `CellFormatting` jika belum ada:**

```vb
' ⚠️ Pakai token semantik ModuleTheme — JANGAN hardcode Color.Red
Private Sub DgvData_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DgvData.CellFormatting
    If DgvData.Columns.Contains("StokToko") AndAlso DgvData.Columns.Contains("StokGudang") Then
        Dim stokTokoIndex As Integer = DgvData.Columns("StokToko").Index
        Dim stokGudangIndex As Integer = DgvData.Columns("StokGudang").Index
        If e.ColumnIndex = stokTokoIndex OrElse e.ColumnIndex = stokGudangIndex Then
            If e.Value IsNot Nothing AndAlso ModuleAngka.ParseDecimal(e.Value) < 1 Then
                ' Stok habis = warna informasi (amber), bukan merah
                e.CellStyle.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowStokHabis, ModuleTheme.D_DgvRowStokHabis)
                e.CellStyle.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
            End If
        End If
    End If
End Sub
```

```vb
If ModulHakAkses.SettingTampilInfoStok Then
    DgvData.Columns("StokToko").Visible = True
    DgvData.Columns("StokGudang").Visible = True
Else
    DgvData.Columns("StokToko").Visible = False
    DgvData.Columns("StokGudang").Visible = False
End If
```

**d. Pastikan `IsiBarangKeRow` dan `CellEndEdit` mengisi kolom stok.**

---

## Fase 4 — Perbaiki Bug Umum di `CellEndEdit`

| Bug Lama | Perbaikan |
|---|---|
| `DgvData.CurrentCell.RowIndex` tanpa guard | Gunakan `e.RowIndex` dari event args, bukan `CurrentCell.RowIndex` |
| Nama kolom salah (misal `HARGABELI`) | Sesuaikan dengan nama kolom yang benar di designer |
| Query `LIKE` tanpa exact match | Ganti ke `= @NamaBarang` untuk exact match |
| `SendKeys.Send("{down}")` | Ganti dengan `SetupFocusToGrid()` |
| Tidak mendukung jenis pelanggan | Tambahkan `prefix = If(LblJenisPl.Text = "Partai", "PARTAI", "UMUM")` |

---

## Fase 5 — Perbaiki `_sedangSetNilaiDariListBox` Scope

Flag harus aktif selama **seluruh proses** di `AmbilDataDariListBox`, bukan hanya saat `EndEdit`:

```vb
' SALAH — flag dimatikan terlalu cepat:
_sedangSetNilaiDariListBox = True
DgvData.EndEdit(True)
DgvData.CurrentCell = Nothing
_sedangSetNilaiDariListBox = False  ' ← terlalu cepat, CellEndEdit masih bisa terpicu
IsiBarangKeRow(...)

' BENAR — flag aktif sampai IsiBarangKeRow selesai:
_sedangSetNilaiDariListBox = True
DgvData.EndEdit(True)
DgvData.CurrentCell = Nothing
' ... cek duplikat ...
IsiBarangKeRow(...)
_sedangSetNilaiDariListBox = False  ' ← baru dimatikan setelah semua selesai
```

---

## Fase 5b — Fitur Stok via SP (Wajib di Semua Form Transaksi)

### SP yang tersedia di database

| SP | Fungsi | Kapan dipakai |
|---|---|---|
| `sp_hlp_stok_ambil` | SELECT stok terkini — mode tambah | Tampil info stok saat barang dipilih, refresh, load draft |
| `sp_hlp_stok_ambil_edit` | SELECT stok efektif — mode edit (stok DB + qty di faktur lama) | Load edit — stok yang ditampilkan sudah memperhitungkan pengembalian |
| `sp_hlp_stok_validasi` | SELECT + FOR UPDATE — validasi real-time anti race condition | Tepat sebelum simpan ke DB |
| `sp_hlp_stok_hitung` | UPDATE stok dari komponen — recalculate penuh | Setelah transaksi tersimpan |

> **Mengapa perlu 2 SP untuk ambil stok?**
> Saat edit, stok di `tbl_barang` sudah dikurangi oleh faktur yang sedang diedit.
> Contoh: stok awal 100, faktur lama jual 10 → `STOK_TOKO = 90`.
> Jika ditampilkan apa adanya, user melihat 90 padahal sebenarnya 100 tersedia
> (karena 10 akan dikembalikan saat hapus-simpan ulang).
> `sp_hlp_stok_ambil_edit` mengembalikan nilai yang akurat: `90 + 10 = 100`.

### Fungsi helper yang wajib ditambahkan (salin dari FormJual)

```vb
' Ambil info stok via SP — otomatis pilih SP yang tepat berdasarkan mode
' Mode tambah → sp_hlp_stok_ambil
' Mode edit   → sp_hlp_stok_ambil_edit (stok DB + qty di faktur lama)
Private Function AmbilInfoStok(kodeBarang As String, ByRef stokToko As Decimal, ByRef stokGudang As Decimal) As Boolean
    ' ... salin dari FormJual ...
    ' ⚠️ Sesuaikan: IsModeTambahPenjualan → nama property mode di form target
    ' ⚠️ Sesuaikan: TxtFaktur.Text → nama TextBox nomor faktur di form target
    ' ⚠️ Sesuaikan: LblLokasiBarang.Text → nama Label lokasi di form target
End Function

' Refresh satu baris DGV
Private Sub RefreshStokBaris(rowIdx As Integer)
    ' ... salin dari FormJual, sesuaikan nama DGV dan kolom ...
End Sub

' Refresh semua baris DGV
Private Sub RefreshStokSemuaBaris()
    ' ... salin dari FormJual, tidak perlu perubahan ...
End Sub
```

### Kapan dipanggil

```vb
' Saat load mode edit — AmbilInfoStok otomatis pakai sp_hlp_stok_ambil_edit
' Tambahkan di akhir Editpenjualanheader() / Editpembelianheader() / dll
' PENTING: pastikan baris DGV sudah terisi dulu sebelum RefreshStokSemuaBaris()
RefreshStokSemuaBaris()

' Saat load draft — AmbilInfoStok otomatis pakai sp_hlp_stok_ambil (mode tambah)
' Tambahkan di akhir AmbilDataDitahan() setelah UpdateSemuaTotal()
RefreshStokSemuaBaris()
```

### Context menu klik kanan — tambahkan 2 menu item baru di designer

```
"Refresh Stok Baris Ini"    → RefreshStokBaris(DgvData.CurrentCell.RowIndex)
"Refresh Stok Semua Baris"  → RefreshStokSemuaBaris()
```

### Validasi race condition di `TekanSimpan` (Validasi Level terakhir)

Tambahkan tepat sebelum `Simpanatauedit()` — setelah semua validasi VB selesai:

```vb
' Validasi stok real-time via SP — menangkap kasus user lain sudah transaksi duluan
If Not ModulHakAkses.SettingIzinkanBarangMinus Then
    For Each dgvRow As DataGridViewRow In DgvData.Rows
        If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso
           Not String.IsNullOrEmpty(dgvRow.Cells("Kode").Value.ToString()) Then

            Dim kodeBarang As String = dgvRow.Cells("Kode").Value.ToString()
            Dim qtySat As Decimal = ModuleAngka.ParseDecimal(dgvRow.Cells("QtySat").Value)

            ' Mode edit: kurangi qty yang sudah tersimpan di faktur ini
            Dim qtyDibutuhkan As Decimal = qtySat
            ' ... (lihat FormJual untuk logika lengkap mode edit) ...

            Try
                Using cmdSP As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan, @errcode, @errmsg)", conn)
                    cmdSP.Parameters.AddWithValue("@kode", kodeBarang)
                    cmdSP.Parameters.AddWithValue("@qty", qtyDibutuhkan)
                    cmdSP.Parameters.AddWithValue("@lokasi", LblLokasiBarang.Text)
                    cmdSP.Parameters.AddWithValue("@izinkan", 0)
                    Dim pErrCode = cmdSP.Parameters.Add("@errcode", MySqlDbType.VarChar, 50)
                    pErrCode.Direction = ParameterDirection.Output
                    Dim pErrMsg = cmdSP.Parameters.Add("@errmsg", MySqlDbType.VarChar, 255)
                    pErrMsg.Direction = ParameterDirection.Output
                    cmdSP.ExecuteNonQuery()

                    If Not String.IsNullOrEmpty(pErrCode.Value?.ToString()) Then
                        MessageBox.Show("⚠️ Stok berubah sejak form dibuka!" & vbCrLf & vbCrLf &
                            pErrMsg.Value?.ToString() & vbCrLf & vbCrLf &
                            "Kemungkinan ada transaksi lain yang baru saja memproses barang ini.",
                            "Konflik Stok", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        ' Highlight baris dengan warna konflik
                        For Each cell As DataGridViewCell In dgvRow.Cells
                            cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowKonflik, ModuleTheme.D_DgvRowKonflik)
                        Next
                        Exit Sub
                    End If
                End Using
            Catch
                ' SP gagal → lanjutkan, validasi VB di TekanBayar sudah cukup
            End Try
        End If
    Next
End If
```

### Aturan warna highlight baris DGV — WAJIB konsisten

Gunakan token semantik dari `ModuleTheme`, **jangan hardcode warna**:

| Situasi | Token | Warna |
|---|---|---|
| Kolom stok = 0 (`CellFormatting`) | `L/D_DgvRowStokHabis` | Amber — informasi |
| Stok tidak cukup (`CekStok`) | `L/D_DgvRowPeringatan` | Amber — peringatan, user bisa ubah |
| Harga jual rugi (`Cekjualrugi`) | `L/D_DgvRowPeringatan` | Amber — peringatan, user bisa ubah |
| Race condition SP (`TekanSimpan`) | `L/D_DgvRowKonflik` | Amber — konflik multi-user |
| Error sistem (`DataError`) | `L/D_DgvRowError` | Merah — error tidak bisa dilanjutkan |

---

## Fase 6 — Verifikasi Akhir

### 6.1 Jalankan PowerShell comparison (dari Fase 0.3)

**Yang diharapkan:**
- "FUNGSI BISNIS YANG HILANG" → **KOSONG**
- "FUNGSI BARU YANG DITAMBAHKAN" → hanya 10 fungsi pencarian baru

### 6.2 Checklist manual

- [ ] PowerShell: tidak ada fungsi bisnis yang hilang
- [ ] Fungsi pencarian baru sudah ditambahkan (`SearchBarangToListBox`, `IsiBarangKeRow`, `TutupListBox`, dll)
- [ ] `_sedangSetNilaiDariListBox` guard ada di `CellEndEdit`
- [ ] `_sedangPindahKeLstBarang` guard ada di `DgvData_CellLeave` (via `BeginInvoke`)
- [ ] `_sedangPindahKeLstBarang` guard ada di `EditingControlShowing`
- [ ] `_listBoxDibukaDiRow/Col` diset saat ListBox ditampilkan, dipakai di `CellLeave`
- [ ] `SetupFocusToGrid` menggunakan nested `BeginInvoke`
- [ ] `IsiBarangKeRow` mendukung jenis pelanggan/supplier yang relevan
- [ ] `CellEndEdit` menggunakan `e.RowIndex` bukan `DgvData.CurrentCell.RowIndex`
- [ ] Tidak ada `SendKeys.Send("{down}")` — sudah diganti `SetupFocusToGrid()`
- [ ] Tidak ada `Color.Red` hardcoded — sudah pakai `ModuleTheme.C(L_Danger, D_Danger)`
- [ ] Tidak ada `Color.Yellow` hardcoded — sudah pakai `ModuleTheme.C(L_SearchFocusBg, D_SearchFocusBg)`
- [ ] Tidak ada flag lama: `_rowSaatPindahKeLst`, `_lstBarangSelectedIndex`, `_lstBarangBaruMasuk`
- [ ] Fitur stok sudah ada (kolom DGV + CellFormatting + toggle visibility)
- [ ] `AmbilInfoStok`, `RefreshStokBaris`, `RefreshStokSemuaBaris` sudah ditambahkan
- [ ] `RefreshStokSemuaBaris` dipanggil di akhir load edit dan load draft
- [ ] Context menu punya "Refresh Stok Baris Ini" dan "Refresh Stok Semua Baris"
- [ ] Validasi race condition SP ada di `TekanSimpan` sebelum `Simpanatauedit()`
- [ ] Semua highlight baris DGV pakai token semantik ModuleTheme (bukan hardcode warna)

---

## Referensi File

| File | Keterangan |
|---|---|
| `2Trans/FormJual.vb` | Form hasil migrasi — **referensi utama untuk semua perubahan** |
| `2Trans/FormPenjualan.vb` | Form lama — referensi fitur bisnis yang sudah terbukti akurat |
| `Modules/ModuleTheme.vb` | Warna dan pengaturan DGV — token semantik `L/D_DgvRow*` |
| `Modules/ModuleAngka.vb` | ParseDecimal, ParseInteger, FormatRupiah |
| `Modules/ModulHakAkses.vb` | SettingFokusOtomatis, SettingIzinkanSatuanBerbeda, SettingTampilInfoStok |
| `Database/16_sp_hlp_stok_ambil.sql` | SP ambil info stok mode tambah — salin ke db lain saat migrasi |
| `Database/17_sp_hlp_stok_ambil_edit.sql` | SP ambil info stok mode edit (stok efektif) — salin ke db lain saat migrasi |

---

## Masalah yang Pernah Terjadi & Solusinya

> Daftar lengkap beserta pola kode ada di `pola-form-transaksi.md` (selalu dimuat otomatis).

| Masalah | Penyebab | Solusi |
|---|---|---|
| ListBox hilang saat tekan panah bawah dari DGV | DGV `EditOnKeystroke` merebut fokus kembali setelah `Focus()` | `EndEdit()` + nested `BeginInvoke` — lihat pola di `pola-form-transaksi.md` |
| `TextChanged` teks kosong menutup ListBox | DGV `BeginEdit` ulang → TextBox baru kosong | Guard: jika `LstBarang.Visible = True` dan teks kosong → skip |
| `EditingControlShowing` re-attach ke TextBox kosong | DGV `BeginEdit` ulang saat transisi fokus | Guard `_sedangPindahKeLstBarang` di `EditingControlShowing` |
| Duplikat qty saat tambah barang baru | `CellEndEdit` terpicu oleh `EndEdit()` di `AmbilDataDariListBox` | Guard `_sedangSetNilaiDariListBox` di awal `CellEndEdit`, scope flag diperluas |
| Fokus tidak pindah ke baris kosong berikutnya | `SetupFocusToGrid` tidak menemukan baris kosong karena baris baru adalah `IsNewRow` | Pakai `IsNewRow` sebagai `targetRow` — jangan `Rows.Add()` |

---

## Catatan Khusus per Form

### FormPembelian → FormBeli (rencana)

- Tidak ada jenis pelanggan (Umum/Partai) — hapus logika partai dari `IsiBarangKeRow`
- Tidak ada `LblJenisPl` — hapus semua referensinya
- Kolom DGV mungkin berbeda — cek di designer sebelum mulai
- Stok tetap ditampilkan (pembelian menambah stok, tapi user tetap perlu lihat stok saat ini)
- Setting khusus yang perlu ditambahkan: `SettingIzinkanUbahHargaBeli`, `SettingBeliOtomatisUpdateHargaJual`, `SettingMetodeUpdateHargaBeli`, `SettingAverageHargaBerdasarkanStok`, `SettingIzinkanBeliTanpaSupplier`, `SettingIzinkanNominalBeliNol`, `SettingIzinkanBeliRugi`
- Setting yang **tidak relevan** dan jangan ditambahkan: `SettingIzinkanUbahHargaJual`, `SettingIzinkanJualRugi`, `SettingIzinkanNominalJualNol`, `SettingHargaJualOtomatisUpdateMaster`, `SettingIzinkanDiskonItem`, `SettingAutoLevelSatuan`

### FormReturJual, FormReturBeli

- Ada mode bebas dan mode terikat nota — cek apakah pencarian barang dibatasi oleh nota
- Mode normal: `IsiBarangKeRow` query ke `penjualan_detail`/`pembelian_detail`, bukan `tbl_barang`
- Mode bebas: `IsiBarangKeRow` query ke `tbl_barang` seperti biasa
- Kolom DGV mungkin punya kolom tambahan (HargaAsli, dll)
- Setting khusus: `SettingWajibAlasanReturJual` / `SettingWajibAlasanReturBeli` — validasi sebelum simpan
- Setting yang **tidak relevan**: `SettingAutoLevelSatuan`, `SettingIzinkanDiskonItem`, `SettingIzinkanJualRugi`, `SettingIzinkanBeliRugi`

### FormTransferCabang

- Tidak ada pembayaran — tidak perlu setting nominal, rugi, atau supplier
- Tidak ada jenis pelanggan
- Setting yang **wajib**: `SettingIzinkanTanggalLampau`, `SettingTampilInfoStok`, `SettingFokusOtomatis`, `SettingIzinkanSatuanBerbeda`, `SettingIzinkanBarangMinus`
- Setting yang **tidak relevan**: semua setting Penjualan dan Pembelian
