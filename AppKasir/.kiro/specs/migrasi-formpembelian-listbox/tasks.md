# Tasks — Migrasi FormPembelian: ListView → ListBox

## ATURAN MIGRASI

### Aturan 1: Utamakan Keamanan Data
- **Selalu backup** file sebelum melakukan perubahan besar
- Test di development environment sebelum deploy ke production
- Jangan hapus logic bisnis, hanya hapus UI control

### Aturan 2: Perubahan Minimal
- Jangan ubah logic bisnis yang sudah berfungsi
- Hanya ganti UI control (ListView → ListBox)
- Pertahankan semua setting dan konfigurasi

### Aturan 3: Verifikasi Setelah Setiap Task
- Centang checklist TASK-26 dan TASK-27 setelah selesai
- Build dan test sebelum lanjut ke task berikutnya
- Jika ada error, rollback ke backup dan perbaiki

### Aturan 4: Dokumentasi Flag
- **FLAG YANG DIHAPUS:** `_lstBarangSelectedIndex`, `_lstBarangBaruMasuk`, `_rowSaatPindahKeLst`
- **FLAG YANG PERTAHANKAN:** `_konteksLstBarang`, `_sedangSetNilaiDariListBox`, `_sedangPindahKeLstBarang`
- **FLAG YANG DITAMBAH:** `_teksSebelumPindahKeLstBarang`, `_dgvEditingTextBox`, `_listBoxDibukaDiRow`, `_listBoxDibukaDiCol`
- Pastikan tidak ada referensi ke flag yang dihapus di kode lain

**Catatan Revisi:** `_sedangPindahKeLstBarang` tetap diperlukan sebagai guard di `DgvData_CellLeave` (dengan `BeginInvoke`) untuk mencegah ListBox ditutup saat transisi fokus ke ListBox.

### Aturan 5: Event Handler
- **YANG DIHAPUS:** `LstBarang_MouseClick` (ListView version), `LstBarang_SizeChanged`, `DgvNamaBarang_PreviewKeyDown`
- **YANG DITAMBAH:** `LstBarang_SelectedIndexChanged` (tracking only), `LstBarang_Click`, `LstBarang_KeyDown` (versi baru), `DgvNamaBarang_KeyDown` (versi baru)
- **YANG PERTAHANKAN:** Semua event DGV (CellEndEdit, CellLeave, KeyDown, dll)

### Aturan 6: Setting Application
- Pastikan setting pembelian tetap berfungsi setelah migrasi
- Setting terkait ListView hanya `SettingTampilInfoStok` yang perlu diupdate di TASK-10
- Setting lain tidak terkait UI control

### Aturan 7: Testing Wajib
- Test jalur TxtNama (barcode, manual, format input)
- Test jalur DGV (inline edit, backspace, escape)
- Test setting (tampil stok on/off, duplikat)
- Test error handling (semua operasi tanpa error)

### Aturan 8: Rollback Plan
- Jika ada masalah kritis, restore dari `FormPembelian.vb.backup`
- Catat semua perubahan yang sudah dilakukan
- Prioritaskan fix bug sebelum lanjut task berikutnya

### Aturan 9: EditMode DataGridView dan Kolom Satuan (KRITIS)
- **EditMode:** FormPembelian menggunakan `EditMode = EditOnKeystrokeOrF2` (bukan EditOnEnter)
- **Alasan:** EditOnEnter menyebabkan banyak kesalahan di FormPembelian sebelumnya
- **Kolom Satuan:** FormJual TIDAK set kolom Satuan readonly - hanya buka dropdown jika SettingAutoLevelSatuan = False
- **FormPembelian:** Tidak punya SettingAutoLevelSatuan, jadi logic buka dropdown di `CellEnter` kolom Satuan **PERTAHANKAN**
- **PERTAHANKAN:** `DgvData_CellEnter` untuk kolom Satuan (buka dropdown ComboBox) - user bisa pilih satuan manual
- **PERTAHANKAN:** `EditMode = EditOnKeystrokeOrF2` di Designer FormPembelian

### Aturan 10: Perbedaan Fitur Bisnis FormPembelian vs FormJual
- **FormPembelian TIDAK punya:**
  - Jenis pelanggan (Umum/Partai) → hapus logika partai dari query
  - SettingAutoLevelSatuan → **TIDAK diimplementasikan** untuk pembelian
  - SettingIzinkanUbahHargaJual → ganti dengan SettingIzinkanUbahHargaBeli
  - SettingIzinkanJualRugi → ganti dengan SettingIzinkanBeliRugi

- **FormPembelian PUNYA:**
  - SettingIzinkanUbahHargaBeli → kontrol kolom HargaBeli ReadOnly
  - SettingBeliOtomatisUpdateHargaJual → dialog update harga jual
  - SettingMetodeUpdateHargaBeli → "Harga Terbaru" / "Average" / "Tidak Ada"
  - SettingAverageHargaBerdasarkanStok → "Toko" / "Gudang" / "Toko dan Gudang"
  - SettingIzinkanBeliTanpaSupplier → validasi supplier
  - SettingIzinkanNominalBeliNol → validasi total > 0

### Aturan 10b: Lock Satuan ke Kecil untuk Pembelian (KRITIS)
- **Kebijakan:** Pembelian selalu dicatat dalam satuan kecil (per unit) secara default
- **Alasan:**
  - Qty selalu 1 untuk pembelian
  - Harga beli selalu sama per unit
  - Untuk pembelian dalam satuan besar (dus/karton), buat barang baru dengan kode berbeda
- **Implementasi:**
  - `IsiBarangKeRow` selalu pakai satuan kecil (level 1) sebagai default
  - Kolom Satuan TIDAK readonly - user bisa ubah manual jika perlu
  - Tidak ada auto level untuk pembelian (SettingAutoLevelSatuan tidak diimplementasikan)
  - Jangan merusak SettingAutoLevelSatuan = tidak (setting ini hanya untuk FormJual)
- **Catatan:** "Lock satuan ke kecil" berarti default isi kecil, bukan readonly. User bisa ubah manual jika ada kasus khusus.

### Aturan 11: Alur Fokus Mode 2 (Edit Langsung DGV) dengan ListBox (KRITIS untuk UX Responsif)
- **Konteks:** Mode 2 (SettingFokusOtomatis = False) → Fokus ke DGV, bukan TxtNama
- **Masalah yang Dihindari:** ListView lama memerlukan flag state kompleks untuk navigasi keyboard
- **Solusi dengan ListBox:** Navigasi keyboard-first, mouse sebagai optional

**Alur UX yang Diharapkan (PRIORITAS KEYBOARD):**
```
1. User di sel NamaBarang DGV → ketik "gula"
2. TextChanged trigger search → ListBox muncul dengan hasil
3. Fokus tetap di TextBox, user bisa:
   - Enter → langsung ambil item pertama (TERCEPAT untuk kasir)
   - Down → pindah ke ListBox, navigasi Up/Down, Enter untuk pilih
   - Up (di item pertama ListBox) → kembali ke TextBox untuk refine search
   - Escape → tutup ListBox
   - Klik mouse → pilih item (optional)
4. Setelah item dipilih → fokus otomatis ke baris kosong berikutnya
```

**Masalah yang Ditemukan Saat Implementasi FormJual (Referensi):**

| # | Masalah | Penyebab | Solusi |
|---|---------|----------|--------|
| 1 | ListBox tertutup sebelum user sempat klik | `CellLeave` terpicu sebelum ListBox mendapat fokus | Gunakan `Me.BeginInvoke` di `CellLeave` |
| 2 | Down arrow di ListBox tidak navigasi | `ProcessCmdKey` dicegat oleh form | Tambahkan guard `If LstBarang.Focused Then Return MyBase.ProcessCmdKey(...)` |
| 3 | `SelectedIndexChanged` ambil barang saat navigasi | Event terpicu saat navigasi, bukan hanya saat memilih | Hapus `AmbilDataDariListBox()` dari `SelectedIndexChanged` |
| 4 | Up arrow di ListBox tidak kembali ke TextBox | Editing control di-destroy saat ListBox fokus | Gunakan `DgvData.Focus()` → `BeginInvoke` → `BeginEdit()` → `EditingControl.Focus()` |
| 5 | Up arrow tidak berfungsi di konteks TxtNama | Kondisi `_dgvEditingTextBox IsNot Nothing` selalu False | Cek `_konteksLstBarang`: jika DGV → `BeginEdit`, jika TXTNAMA → `TxtNama.Focus()` |
| 6 | `BeginInvoke` error saat form load kedua | Handle belum terbentuk | Tambahkan guard `If Not Me.IsHandleCreated Then Return` |
| 7 | Teks hilang saat kembali ke TextBox | Tidak ada mekanisme simpan/restore | Tambahkan `_teksSebelumPindahKeLstBarang` |
| 8 | Backspace di sel NamaBarang menghapus semua teks | `ProcessCmdKey` handler Keys.Back tanpa cek konteks | Tambahkan guard `AndAlso _konteksLstBarang = "TXTNAMA"` |
| 9 | ListBox ditutup saat backspace | `CellLeave` terpicu oleh internal DGV | Simpan posisi sel saat ListBox dibuka (`_listBoxDibukaDiRow/Col`) |
| 10 | Setelah hapus baris, fokus tidak kembali | `DgvData_KeyDown` handler Delete tidak memanggil `SetupFocusToGrid()` | Tambahkan `SetupFocusToGrid()` setelah `Hapusbaris()` |

**Flag yang Diperlukan:**
- `_sedangPindahKeLstBarang` — Guard transisi fokus di `CellLeave`
- `_teksSebelumPindahKeLstBarang` — Simpan/restore teks
- `_konteksLstBarang` — Bedakan jalur TXTNAMA vs DGV
- `_dgvEditingTextBox` — Reference ke TextBox editing DGV
- `_listBoxDibukaDiRow` — Posisi sel saat ListBox dibuka
- `_listBoxDibukaDiCol` — Posisi kolom saat ListBox dibuka

**Prioritas Keyboard (implementasi final):**
1. **Enter** (di TextBox) → Langsung ambil item pertama — paling cepat
2. **Down** (di TextBox) → Pindah ke ListBox, navigasi Up/Down
3. **Enter** (di ListBox) → Ambil item yang di-highlight
4. **Up** (di item pertama ListBox) → Kembali ke TextBox + restore teks
5. **Escape** → Tutup ListBox + restore teks
6. **Klik mouse** → Ambil item (optional)

### Aturan 12: Dokumentasi Bahasa Indonesia untuk Semua Perubahan (WAJIB)
- **Konteks:** Setiap perubahan kode harus dilengkapi keterangan bahasa Indonesia
- **Tujuan:** Memudahkan pencarian dan pemahaman fungsi setiap perubahan
- **Format:** Gunakan komentar singkat di atas kode yang diubah

**Contoh Format:**
```vb
' [FP1-T04-1] HAPUS: Navigasi ListView dengan flag state kompleks
' Alasan: ListBox tidak memerlukan flag state untuk navigasi keyboard
Private _sedangPindahKeLstBarang As Boolean = False  ' DIHAPUS

' [FP2-T11b-1] TAMBAH: Event handler SelectedIndexChanged untuk ListBox
' Fungsi: Handle pemilihan item dari ListBox (mouse click atau keyboard)
Private Sub LstBarang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LstBarang.SelectedIndexChanged
```

**ID Format:** `[FASE-TASK-NOMOR]` contoh: `[FP1-T04-1]` untuk FASE PEMBELIAN 1 TASK-04 item 1

**Kategori Keterangan:**
- **HAPUS:** Kode yang dihapus (dengan alasan)
- **TAMBAH:** Kode baru yang ditambahkan (dengan fungsi)
- **UBAH:** Kode yang dimodifikasi (dengan perubahan)
- **PERTAHANKAN:** Kode yang tidak diubah (dengan catatan)

---

## Status Legend
- `[ ]` Belum dikerjakan
- `[x]` Selesai
- `[~]` Sedang dikerjakan
- `[!]` Blocked / perlu keputusan

---

## FASE 1 — Persiapan

### TASK-01: Backup FormPembelian.vb
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
1. Copy file `FormPembelian.vb` ke `FormPembelian.vb.backup`
2. Pastikan backup tersimpan dengan baik

---

## FASE 2 — Perubahan Designer (Manual di Visual Studio)

### TASK-02: Ganti LstBarang dari ListView ke ListBox
**File:** `2Trans/FormPembelian.Designer.vb`
**Status:** `[ ]`

**Langkah:**
1. Buka `FormPembelian` di Visual Studio Designer
2. Klik `LstBarang` (ListView) → Delete
3. Dari Toolbox, drag `ListBox` ke posisi yang sama
4. Rename menjadi `LstBarang`
5. Set properties:
   - `Font = Consolas, 10pt` (monospace untuk alignment)
   - `Visible = False`
   - `IntegralHeight = False` (opsional)
6. Position di lokasi yang sama dengan ListView lama

**Catatan:** Verifikasi manual di `FormPembelian.Designer.vb` untuk memastikan tipe data berubah dari `ListView` ke `ListBox`.

---

## FASE 3 — Hapus Kode Navigasi ListView

### TASK-03: Hapus Flag State Navigasi ListView
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Cari dan hapus baris berikut (sekitar line 1517-1518):
```vb
Private _lstBarangSelectedIndex As Integer = -1
Private _lstBarangBaruMasuk As Boolean = False
```

**Catatan:** Setelah menghapus flag ini, pastikan untuk menghapus juga referensi ke flag ini di kode lain:
- `_lstBarangSelectedIndex = -1` di AmbilDataDariListBox
- `_lstBarangBaruMasuk = True/False` di semua lokasi

**JANGAN DIHAPUS (Masih diperlukan):**
- `_sedangPindahKeLstBarang` - Digunakan sebagai guard transisi fokus
- `_rowSaatPindahKeLst` - Akan diganti dengan `_listBoxDibukaDiRow/Col`
- `_sedangSetNilaiDariListBox` - Guard CellEndEdit

---

### TASK-04: Hapus Event Handler Navigasi ListView
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Hapus seluruh subroutine berikut:
- `Private Sub LstBarang_KeyDown(...)` (sekitar line 1928-1975) - versi ListView
- `Private Sub LstBarang_MouseClick(...)` (sekitar line 1957-1985) - versi ListView
- `Private Sub LstBarang_SizeChanged(...)` (sekitar line 2512-2520) - untuk hitung lebar kolom ListView

---

### TASK-05: Hapus ProcessCmdKey untuk Navigasi ListView
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Di `Protected Overrides Function ProcessCmdKey` (sekitar line 573-637):
1. Hapus block `If LstBarang.Visible Then` beserta isinya
2. Hapus handler untuk Keys.Down, Keys.Up, Keys.Escape, Keys.Enter yang terkait ListView
3. Hapus handler Keys.Back, Keys.Delete untuk ListView

---

### TASK-06: Hapus DgvNamaBarang_KeyDown dan DgvNamaBarang_PreviewKeyDown (Jika Ada)
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Cari dan hapus jika ada:
- `Private Sub DgvNamaBarang_KeyDown(...)` - versi ListView
- `Private Sub DgvNamaBarang_PreviewKeyDown(...)` - subroutine kosong

**Step tambahan:** Di `EditingControlShowing`, hapus juga:
- RemoveHandler untuk DgvNamaBarang_PreviewKeyDown
- AddHandler untuk DgvNamaBarang_PreviewKeyDown

---

### TASK-07: Tambah Flag State untuk ListBox
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Tambahkan flag berikut di bagian deklarasi variabel (sekitar line 100-150):
```vb
Private _dgvEditingTextBox As TextBox = Nothing       ' TextBox editing control DGV aktif
Private _teksSebelumPindahKeLstBarang As String = ""  ' Simpan teks untuk restore saat Up/Escape
Private _listBoxDibukaDiRow As Integer = -1           ' Baris DGV saat ListBox dibuka
Private _listBoxDibukaDiCol As Integer = -1           ' Kolom DGV saat ListBox dibuka
```

**Catatan:** Flag `_sedangPindahKeLstBarang`, `_konteksLstBarang`, `_sedangSetNilaiDariListBox` sudah ada dan PERTAHANKAN.

---

### TASK-08: Tambah Navigasi Keyboard Sederhana untuk Mode 2 (KRITIS untuk UX)
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Tujuan:** Navigasi keyboard-first antara TextBox DGV dan ListBox. Kasir tidak perlu sentuh mouse.

**Implementasi (adaptasi dari FormJual):**

```vb
' ── Event 1: DgvNamaBarang_KeyDown ──────────────────────────────────────────
' Handle Down arrow dan Enter dari TextBox DGV saat ListBox visible
Private Sub DgvNamaBarang_KeyDown(sender As Object, e As KeyEventArgs)
    If Not LstBarang.Visible OrElse LstBarang.Items.Count = 0 Then Return
    Select Case e.KeyCode
        Case Keys.Down
            ' Simpan teks sebelum pindah ke ListBox untuk restore saat Up
            If _dgvEditingTextBox IsNot Nothing Then
                _teksSebelumPindahKeLstBarang = _dgvEditingTextBox.Text
            End If
            _sedangPindahKeLstBarang = True
            If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
            LstBarang.Focus()
            _sedangPindahKeLstBarang = False
            e.SuppressKeyPress = True
        Case Keys.Enter
            ' Enter langsung → ambil item pertama tanpa perlu Down dulu
            If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
            _sedangPindahKeLstBarang = True
            AmbilDataDariListBox()
            _sedangPindahKeLstBarang = False
            e.SuppressKeyPress = True
        Case Keys.Escape
            LstBarang.Visible = False
            LstBarang.Items.Clear()
            e.SuppressKeyPress = True
    End Select
End Sub

' ── Event 2: LstBarang_KeyDown ───────────────────────────────────────────────
' Handle navigasi di dalam ListBox
Private Sub LstBarang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles LstBarang.KeyDown
    Select Case e.KeyCode
        Case Keys.Up
            If LstBarang.SelectedIndex <= 0 Then
                ' Kembali ke TextBox + restore teks yang sudah diketik
                _sedangPindahKeLstBarang = True
                e.SuppressKeyPress = True
                If _konteksLstBarang = "DGV" Then
                    Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                    _teksSebelumPindahKeLstBarang = ""
                    DgvData.Focus()
                    DgvData.BeginInvoke(New Action(Sub()
                        If DgvData.CurrentCell IsNot Nothing Then
                            DgvData.BeginEdit(True)
                            Dim editCtrl = TryCast(DgvData.EditingControl, TextBox)
                            If editCtrl IsNot Nothing AndAlso Not String.IsNullOrEmpty(teksSimpan) Then
                                editCtrl.Text = teksSimpan
                                editCtrl.SelectionStart = teksSimpan.Length
                            End If
                            editCtrl?.Focus()
                        End If
                        _sedangPindahKeLstBarang = False
                    End Sub))
                Else
                    Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                    _teksSebelumPindahKeLstBarang = ""
                    TxtNama.Focus()
                    If Not String.IsNullOrEmpty(teksSimpan) Then
                        TxtNama.Text = teksSimpan
                        TxtNama.SelectionStart = teksSimpan.Length
                    End If
                    _sedangPindahKeLstBarang = False
                End If
            End If
        Case Keys.Enter
            If LstBarang.SelectedIndex >= 0 Then
                _sedangPindahKeLstBarang = True
                AmbilDataDariListBox()
                _sedangPindahKeLstBarang = False
            End If
            e.SuppressKeyPress = True
        Case Keys.Escape
            LstBarang.Visible = False
            LstBarang.Items.Clear()
            ' Restore teks dan kembali ke TextBox (sama dengan logika Up arrow)
            ...
    End Select
End Sub

' ── Event 3: LstBarang_SelectedIndexChanged ──────────────────────────────────
' HANYA tracking — TIDAK memanggil AmbilDataDariListBox()
Private Sub LstBarang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LstBarang.SelectedIndexChanged
    ' tracking only
End Sub

' ── Event 4: LstBarang_Click ─────────────────────────────────────────────────
' Handle pemilihan via mouse click
Private Sub LstBarang_Click(sender As Object, e As EventArgs) Handles LstBarang.Click
    If LstBarang.SelectedIndex >= 0 Then
        _sedangPindahKeLstBarang = True
        AmbilDataDariListBox()
        _sedangPindahKeLstBarang = False
    End If
End Sub
```

---

### TASK-09: Update DgvData_CellLeave dengan BeginInvoke
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Di `Private Sub DgvData_CellLeave` (sekitar line 2498-2510):
1. Hapus guard lama yang terkait ListView (`_rowSaatPindahKeLst`, dll)
2. Ganti dengan logika baru menggunakan `BeginInvoke`

**Implementasi:**
```vb
Private Sub DgvData_CellLeave(sender As Object, e As DataGridViewCellEventArgs) Handles DgvData.CellLeave
    If Not Me.IsHandleCreated Then Return

    Me.BeginInvoke(New Action(Sub()
        If LstBarang.Visible Then
            If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then
                ' Skip — ListBox sedang aktif atau dalam transisi fokus
            ElseIf _listBoxDibukaDiRow >= 0 AndAlso
                   DgvData.CurrentCell IsNot Nothing AndAlso
                   DgvData.CurrentCell.RowIndex = _listBoxDibukaDiRow AndAlso
                   DgvData.CurrentCell.ColumnIndex = _listBoxDibukaDiCol Then
                ' Skip — masih di sel yang sama dengan saat ListBox dibuka
            Else
                LstBarang.Visible = False
                LstBarang.Items.Clear()
                _listBoxDibukaDiRow = -1
                _listBoxDibukaDiCol = -1
            End If
        End If
    End Sub))
End Sub
```

---

### TASK-10: Update EditingControlShowing untuk Hapus Guard LstBarang.Focused
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Di `EditingControlShowing` (sekitar line 3068-3095), update:
- Hapus guard `If Not LstBarang.Focused Then`
- Tambah logika simpan posisi saat ListBox dibuka

**Implementasi:**
```vb
If DgvData.CurrentCell.ColumnIndex = 1 AndAlso DgvData.Columns(1).HeaderText = "Nama Barang" Then
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
        
        ' Simpan posisi sel saat ListBox dibuka
        _listBoxDibukaDiRow = DgvData.CurrentCell.RowIndex
        _listBoxDibukaDiCol = DgvData.CurrentCell.ColumnIndex
        PosisikanLstBarangDiBawahSel()
    End If
Else
    ' Tutup ListBox jika tidak di kolom NamaBarang
    LstBarang.Visible = False
    LstBarang.Items.Clear()
    _listBoxDibukaDiRow = -1
    _listBoxDibukaDiCol = -1
End If
```

---

## FASE 4 — Update Logic SearchBarang

### TASK-11: Update SearchBarangToListView untuk Isi ListBox
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Di `Private Sub SearchBarangToListView` (sekitar line 2135-2210):

**Step 1: Ubah Query SQL - Batasi kolom yang diambil berdasarkan setting**

Ubah dari:
```vb
Dim query = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
    "WHERE STATUS = 'Aktif' AND (" &
    "   ID_BARANG LIKE @key " &
    "   OR NAMA_BARANG LIKE @key " &
    "   OR BARCODE_KECIL LIKE @key " &
    "   OR BARCODE_SEDANG LIKE @key " &
    "   OR BARCODE_BESAR LIKE @key) " &
    "ORDER BY " & orderBy
```

Ke:
```vb
Dim query As String
If ModulHakAkses.SettingTampilInfoStok Then
    query = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
            "WHERE STATUS = 'Aktif' AND (" &
            "   ID_BARANG LIKE @key " &
            "   OR NAMA_BARANG LIKE @key " &
            "   OR BARCODE_KECIL LIKE @key " &
            "   OR BARCODE_SEDANG LIKE @key " &
            "   OR BARCODE_BESAR LIKE @key) " &
            "ORDER BY " & orderBy
Else
    query = "SELECT NAMA_BARANG FROM tbl_barang " &
            "WHERE STATUS = 'Aktif' AND (" &
            "   ID_BARANG LIKE @key " &
            "   OR NAMA_BARANG LIKE @key " &
            "   OR BARCODE_KECIL LIKE @key " &
            "   OR BARCODE_SEDANG LIKE @key " &
            "   OR BARCODE_BESAR LIKE @key) " &
            "ORDER BY NAMA_BARANG"
End If
```

**Step 2: Ubah Loop Isi ListBox**

Ubah dari:
```vb
While rd.Read()
    listHasil.Add((
        rd("NAMA_BARANG").ToString(),
        ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D),
        ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
    ))
End While

' Setelah loop:
For Each barang In listHasil
    Dim item As New ListViewItem(barang.Nama)
    ' ... tambah subitems untuk stok ...
    LstBarang.Items.Add(item)
Next
```

Ke:
```vb
LstBarang.Items.Clear()

While rd.Read()
    Dim namaBarang = rd("NAMA_BARANG").ToString()
    Dim displayString As String

    If ModulHakAkses.SettingTampilInfoStok Then
        Dim stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
        Dim stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
        displayString = String.Format("{0} | T: {1} | G: {2}", namaBarang, stokToko.ToString("N0"), stokGudang.ToString("N0"))
    Else
        displayString = namaBarang
    End If

    LstBarang.Items.Add(displayString)
End While
```

**Step 3: Hapus listHasil dan loop tambahan**

Hapus variabel `listHasil` dan loop tambahan yang mengisi ListView dengan ListViewItem.

---

### TASK-12: Update AmbilDataDariListBox untuk Ambil dari ListBox
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Di `Private Sub AmbilDataDariListBox` (sekitar line 1604-1785):

Ubah dari:
```vb
Dim selectedValue As String = ""
If _lstBarangSelectedIndex >= 0 AndAlso _lstBarangSelectedIndex < LstBarang.Items.Count Then
    selectedValue = DirectCast(LstBarang.Items(_lstBarangSelectedIndex), ListViewItem).Text
ElseIf LstBarang.Items.Count = 1 Then
    selectedValue = DirectCast(LstBarang.Items(0), ListViewItem).Text
ElseIf LstBarang.SelectedItems.Count > 0 Then
    selectedValue = DirectCast(LstBarang.SelectedItems(0), ListViewItem).Text
End If
```

Ke:
```vb
Dim selectedValue As String = ""
If LstBarang.SelectedIndex >= 0 Then
    selectedValue = LstBarang.Items(LstBarang.SelectedIndex).ToString()
ElseIf LstBarang.Items.Count = 1 Then
    selectedValue = LstBarang.Items(0).ToString()
ElseIf LstBarang.SelectedItem IsNot Nothing Then
    selectedValue = LstBarang.SelectedItem.ToString()
End If

' Parse nama barang dari format string
If selectedValue.Contains("|"c) Then
    Dim parts As String() = selectedValue.Split("|"c)
    If parts.Length > 0 Then
        selectedValue = parts(0).Trim()
    End If
End If
```

Hapus juga:
- `_lstBarangSelectedIndex = -1` (reset index tersimpan)
- `_rowSaatPindahKeLst = -1` (reset row tersimpan) - ganti dengan `_listBoxDibukaDiRow = -1`

---

### TASK-13: Set Font ListBox ke Monospace
**File:** `2Trans/FormPembelian.Designer.vb` atau `FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Di `FormPembelian.Designer.vb` atau di `Form_Load`:
```vb
LstBarang.Font = New Font("Consolas", 10)
```

---

## FASE 5 — Lock Satuan ke Kecil (Khusus FormPembelian)

### TASK-14: Default Satuan ke Kecil (Bukan Readonly)
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
1. Di `IsiBarangKeRow`, pastikan selalu pakai satuan kecil (level 1) sebagai default:
   - Hapus logika penentuan level dari barcode
   - Hapus logika penentuan level dari qty
   - Selalu pakai satuan kecil, isi kecil, harga beli kecil
2. Di `CellEndEdit` kolom QTY, hapus logika auto level (jika ada)
3. PERTAHANKAN logic `DgvData_CellEnter` untuk kolom Satuan (buka dropdown ComboBox) - user bisa ubah satuan manual jika perlu
4. JANGAN set kolom Satuan readonly - user harus bisa ubah manual jika ada kasus khusus

**Catatan:** "Lock satuan ke kecil" berarti default isi kecil, bukan readonly. User bisa ubah manual jika ada kasus khusus. Jangan merusak SettingAutoLevelSatuan di ModulHakAkses - setting ini hanya untuk FormJual.

---

### TASK-14b: Tambah SetupFocusToGrid Setelah Hapus Baris (KRITIS untuk UX Keyboard)
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
1. Di handler `Keys.Delete` (sekitar line 3038), tambahkan `SetupFocusToGrid()` setelah hapus baris:
   ```vb
   ' Hapus baris jika nilai di kolom "Nama" tidak kosong
   DgvData.Rows.RemoveAt(rowIndex)
   DgvData.ClearSelection()
   ' Kembalikan fokus ke baris kosong berikutnya untuk UX keyboard
   SetupFocusToGrid()
   ```

2. Di `HapusToolStripMenuItem_Click` (sekitar line 3226), tambahkan `SetupFocusToGrid()` setelah `Hapusbaris()`:
   ```vb
   Private Sub HapusToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HapusToolStripMenuItem.Click
       Call Hapusbaris()
       ' Kembalikan fokus ke baris kosong berikutnya untuk UX keyboard
       SetupFocusToGrid()
   End Sub
   ```

3. Di `Hapusbaris()` (sekitar line 960), tambahkan `SetupFocusToGrid()` setelah hapus baris:
   ```vb
   If result = DialogResult.Yes Then
       DgvData.Rows.RemoveAt(baris)
       UpdateSemuaTotal()
       ' Kembalikan fokus ke baris kosong berikutnya untuk UX keyboard
       SetupFocusToGrid()
   End If
   ```

**Catatan:** Konsisten dengan FormJual - SetupFocusToGrid dipanggil setelah setiap penghapusan baris agar user bisa langsung input barang berikutnya tanpa perlu klik manual.

---

## FASE 6 — Verifikasi Events TxtNama & DGV

### TASK-15: Verifikasi Events TxtNama dan DGV Tidak Berubah
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
Verifikasi bahwa events berikut **TIDAK PERLU DIUBAH** karena hanya trigger search/logic bisnis:

| Event | Status | Keterangan |
|-------|--------|------------|
| `TxtNama_TextChanged` | ✅ Tetap | Trigger search, barcode detection |
| `TxtNama_KeyDown` | ⚠️ Hapus navigasi Keys.Down | Sudah di TASK-09, sisanya tetap |
| `DgvNamaBarang_TextChanged` | ✅ Tetap | Trigger search untuk DGV inline edit |
| `DgvDataData_CellEndEdit` | ✅ Tetap | Logic parsing dan query DB |

**Catatan:** Hapus hanya bagian navigasi Keys.Down ke ListView dari `TxtNama_KeyDown`. Semua logic bisnis parsing dan search tetap sama.

---

## FASE 7 — Testing

### TASK-16: Test Barcode Detection
**Status:** `[ ]`

**Langkah:**
1. Scan barcode numeric
2. Scan barcode alphanumeric
3. Verifikasi barang ditemukan dan ditambahkan ke DGV

---

### TASK-17: Test Format Input
**Status:** `[ ]`

**Langkah:**
1. Input `5*nama` (qty*nama)
2. Input `5*2*nama` (qty*level*nama)
3. Input `5*barcode` (qty*barcode)
4. Verifikasi parsing dan qty sesuai

---

### TASK-18: Test Jalur TxtNama
**Status:** `[ ]`

**Langkah:**
1. Ketik nama barang manual
2. Verifikasi ListBox muncul dengan hasil search
3. Klik item di ListBox
4. Verifikasi barang ditambahkan ke DGV
5. Verifikasi backspace berfungsi untuk koreksi

---

### TASK-19: Test Jalur DGV
**Status:** `[ ]`

**Langkah:**
1. Edit sel NamaBarang di DGV
2. Ketik nama barang manual
3. Verifikasi ListBox muncul dengan hasil search
4. Klik item di ListBox
5. Verifikasi data diisi ke sel
6. Verifikasi backspace berfungsi untuk koreksi
7. Verifikasi pindah ke sel lain → ListBox tertutup

---

### TASK-20: Test Cek Duplikat
**Status:** `[ ]`

**Langkah:**
1. Tambah barang yang sama dua kali
2. Verifikasi error message muncul
3. Verifikasi barang tidak ditambahkan duplikat

---

### TASK-21: Test Logic Bisnis Pembelian
**Status:** `[ ]`

**Langkah:**
1. Verifikasi barcode detection tetap berfungsi
2. Verifikasi parsing format input tetap berfungsi
3. Verifikasi cek duplikat tetap berfungsi
4. Verifikasi kalkulasi total harga tetap berfungsi
5. Verifikasi setting pembelian (IzinkanUbahHargaBeli, dll) tetap berfungsi

---

### TASK-22: Test Lock Satuan ke Kecil
**Status:** `[ ]`

**Langkah:**
1. Tambah barang ke DGV
2. Verifikasi kolom Satuan readonly (tidak bisa diubah)
3. Verifikasi satuan selalu kecil
4. Verifikasi isi selalu isi kecil
5. Verifikasi harga beli selalu harga beli kecil
6. Coba edit kolom QTY → verifikasi satuan TIDAK berubah (tidak ada auto level)

---

## FASE 8 — Cleanup

### TASK-23: Hapus Kode Tidak Terpakai
**File:** `2Trans/FormPembelian.vb`
**Status:** `[ ]`

**Langkah:**
1. Cari semua referensi ke flag yang dihapus
2. Hapus variabel yang tidak digunakan
3. Hapus komentar yang tidak relevan

---

### TASK-24: Build dan Verifikasi
**Status:** `[ ]`

**Langkah:**
1. Build project
2. Verifikasi tidak ada error compile
3. Verifikasi tidak ada warning

---

## FASE 9 — Verifikasi Akhir

### TASK-25: Checklist Verifikasi Perubahan Kode
**Status:** `[ ]`

**Checklist Hapus:**
- [ ] Flag `_lstBarangSelectedIndex` dihapus
- [ ] Flag `_lstBarangBaruMasuk` dihapus
- [ ] Referensi `_lstBarangSelectedIndex` dihapus dari semua lokasi
- [ ] Referensi `_lstBarangBaruMasuk` dihapus dari semua lokasi
- [ ] Subroutine `LstBarang_KeyDown` (versi ListView) dihapus
- [ ] Subroutine `LstBarang_MouseClick` (versi ListView) dihapus
- [ ] Subroutine `LstBarang_SizeChanged` dihapus
- [ ] Block `If LstBarang.Visible Then` di ProcessCmdKey dihapus
- [ ] Handler Keys.Down/Up/Escape/Enter untuk ListView di ProcessCmdKey dihapus
- [ ] Guard `If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then Return` di DgvData_CellLeave dihapus
- [ ] Guard `If LstBarang.Visible AndAlso e.RowIndex = _rowSaatPindahKeLst Then Return` di DgvData_CellLeave dihapus

**Checklist Tambah/Update:**
- [ ] Flag `_dgvEditingTextBox` ditambah
- [ ] Flag `_teksSebelumPindahKeLstBarang` ditambah
- [ ] Flag `_listBoxDibukaDiRow` ditambah
- [ ] Flag `_listBoxDibukaDiCol` ditambah
- [ ] Event `DgvNamaBarang_KeyDown` (versi baru) ditambah
- [ ] Event `LstBarang_KeyDown` (versi baru) ditambah
- [ ] Event `LstBarang_SelectedIndexChanged` ditambah
- [ ] Event `LstBarang_Click` ditambah
- [ ] `SearchBarangToListView` diubah ke `SearchBarangToListBox` dengan format string
- [ ] `AmbilDataDariListBox` diubah untuk parse dari format string
- [ ] `DgvData_CellLeave` diupdate dengan BeginInvoke dan guard baru
- [ ] `EditingControlShowing` diupdate untuk simpan posisi ListBox
- [ ] Font ListBox di-set ke monospace
- [ ] Kolom Satuan di-set readonly di Form_Load
- [ ] `IsiBarangKeRow` selalu pakai satuan kecil (level 1)
- [ ] Logika auto level dihapus dari `CellEndEdit` kolom QTY
- [ ] Logic buka dropdown dihapus dari `CellEnter` kolom Satuan

---

### TASK-26: Verifikasi Logic Bisnis Pembelian
**Status:** `[ ]`

**Checklist:**
- [ ] SettingIzinkanUbahHargaBeli berfungsi
- [ ] SettingBeliOtomatisUpdateHargaJual berfungsi
- [ ] SettingMetodeUpdateHargaBeli berfungsi
- [ ] SettingAverageHargaBerdasarkanStok berfungsi
- [ ] SettingIzinkanBeliTanpaSupplier berfungsi
- [ ] SettingIzinkanNominalBeliNol berfungsi
- [ ] SettingIzinkanBeliRugi berfungsi
- [ ] SettingTampilInfoStok berfungsi
- [ ] SettingIzinkanSatuanBerbeda berfungsi
- [ ] SettingFokusOtomatis berfungsi

---

## CATATAN KHUSUS FORMPEMBELIAN

### Perbedaan Query dengan FormJual
- FormPembelian query mengambil: ID_BARANG, NAMA_BARANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR
- FormPembelian TIDAK ada logika Partai/Umum → hapus prefix SATUAN_PARTAI/SATUAN_UMUM
- FormPembelian ordering berdasarkan stok lokasi (TOKO/GUDANG)

### Perbedaan IsiBarangKeRow
- FormPembelian `IsiBarangKeRow` menerima parameter: `rowIdx`, `namaBarang`, `qty`, `level`, `barcodeInput`
- FormPembelian TIDAK perlu cek jenis pelanggan → hapus logika `isPartai`
- FormPembelian mengambil harga beli, bukan harga jual

### Perbedaan Setting
- FormPembelian TIDAK punya SettingAutoLevelSatuan → hapus logic auto level
- FormPembelian punya setting pembelian-specific → pertahankan semua
