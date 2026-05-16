# Tasks — Migrasi FormJual: ListView → ListBox

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
- **FLAG YANG DIHAPUS:** `_rowSaatPindahKeLst`, `_lstBarangSelectedIndex`, `_lstBarangBaruMasuk`
- **FLAG YANG PERTAHANKAN:** `_konteksLstBarang`, `_sedangSetNilaiDariListBox`, `_sedangPindahKeLstBarang`
- **FLAG YANG DITAMBAH:** `_teksSebelumPindahKeLstBarang`
- Pastikan tidak ada referensi ke flag yang dihapus di kode lain

**Catatan Revisi:** `_sedangPindahKeLstBarang` awalnya direncanakan dihapus, tapi ternyata **tetap diperlukan** sebagai guard di `DgvData_CellLeave` (dengan `BeginInvoke`) untuk mencegah ListBox ditutup saat transisi fokus ke ListBox.

### Aturan 5: Event Handler
- **YANG DIHAPUS:** `LstBarang_MouseClick`, `LstBarang_SizeChanged`, `DgvNamaBarang_PreviewKeyDown`
- **YANG DITAMBAH:** `LstBarang_SelectedIndexChanged` (tracking only), `LstBarang_Click`, `LstBarang_KeyDown`, `DgvNamaBarang_KeyDown` (versi baru — lebih sederhana)
- **YANG PERTAHANKAN:** Semua event DGV (CellEndEdit, CellLeave, KeyDown, dll)

**Catatan Revisi:** `LstBarang_KeyDown` dan `DgvNamaBarang_KeyDown` awalnya direncanakan dihapus, tapi ternyata **perlu ditambahkan kembali** dengan implementasi baru untuk navigasi keyboard-first.

### Aturan 6: Setting Application
- Pastikan 14 setting tetap berfungsi setelah migrasi
- Setting terkait ListView hanya `SettingTampilInfoStok` yang perlu diupdate di TASK-10
- Setting lain tidak terkait UI control

### Aturan 7: Testing Wajib
- Test jalur TxtNama (barcode, manual, format input)
- Test jalur DGV (inline edit, backspace, escape)
- Test setting (tampil stok on/off, duplikat, auto level)
- Test error handling (semua operasi tanpa error)

### Aturan 8: Rollback Plan
- Jika ada masalah kritis, restore dari `FormJual.vb.backup`
- Catat semua perubahan yang sudah dilakukan
- Prioritaskan fix bug sebelum lanjut task berikutnya

### Aturan 9: EditMode DataGridView dan Kolom Satuan (KRITIS)
- **PERTAHANKAN** event `DgvData_CellEnter` - penting untuk UX kolom Satuan
- **Fungsi:** Saat user masuk ke kolom "Satuan", otomatis buka dropdown ComboBox
- **Logic:**
  ```vb
  If kolom = "Satuan" AND SettingAutoLevelSatuan = False Then
      BeginEdit(True)
      ComboBox.DroppedDown = True
  End If
  ```
- **Catatan:** Jika `SettingAutoLevelSatuan = True`, satuan otomatis ditentukan dari QTY → dropdown tidak perlu dibuka
- **EditMode:** Meski `EditMode = EditOnEnter`, BeginEdit tetap dipanggil untuk buka dropdown
- **JANGAN HAPUS:** Logic ini memudahkan user memilih satuan tanpa F2 atau klik manual

### Aturan 10: SettingAutoLevelSatuan - Dua Jalur Implementasi (KRITIS)
- **Jalur 1: TxtNama dan DGV (sel NamaBarang)**
  - Implementasi di `IsiBarangKeRow` (line 1834-1838)
  - Saat isi data barang (dari barcode atau manual), cek auto level jika:
    - Tidak ada barcode input (`barcodeInput` kosong)
    - Level masih default (1)
  - Function: `TentukanLevelDariQty(qty)` → return level (1/2/3) atau 0 (nonaktif)
  
- **Jalur 2: Edit langsung kolom QTY di DGV**
  - Implementasi di `CellEndEdit` kolom QTY (line 2342-2355)
  - Saat user selesai edit QTY, otomatis update level satuan
  - Call: `UpdateLevelSatuanBaris(rowIdx, levelBaru)`
  
- **Catatan:** Kedua jalur menggunakan function yang sama `TentukanLevelDariQty()`
- **PERTAHANKAN:** Logic ini sudah ada di kedua jalur, jangan dihapus atau ubah

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

**Masalah yang Ditemukan Saat Implementasi (Catatan Debugging):**

| # | Masalah | Penyebab | Solusi |
|---|---------|----------|--------|
| 1 | ListBox tertutup sebelum user sempat klik | `CellLeave` terpicu sebelum ListBox mendapat fokus → `LstBarang.Focused = False` | Gunakan `Me.BeginInvoke` di `CellLeave` agar cek dilakukan setelah fokus benar-benar berpindah |
| 2 | Down arrow di ListBox tidak navigasi, malah loop ke item pertama | `ProcessCmdKey` berlaku untuk seluruh form — Down arrow di ListBox dicegat dan memanggil `LstBarang.Focus()` lagi | Tambahkan guard `If LstBarang.Focused Then Return MyBase.ProcessCmdKey(...)` |
| 3 | `SelectedIndexChanged` ambil barang saat navigasi keyboard | Event terpicu saat `SelectedIndex = 0` di-set di `ProcessCmdKey`, bukan hanya saat user memilih | Hapus `AmbilDataDariListBox()` dari `SelectedIndexChanged` — pindahkan ke `LstBarang_Click` dan `LstBarang_KeyDown` (Enter) |
| 4 | Up arrow di ListBox tidak kembali ke TextBox DGV | `_dgvEditingTextBox.Focus()` gagal — editing control sudah di-destroy saat ListBox fokus | Gunakan `DgvDataTransaksi.Focus()` → `BeginInvoke` → `BeginEdit(True)` → `EditingControl.Focus()` |
| 5 | Up arrow tidak berfungsi di konteks TxtNama | Kondisi `AndAlso _dgvEditingTextBox IsNot Nothing` selalu False saat konteks TXTNAMA | Cek `_konteksLstBarang`: jika DGV → `BeginEdit`, jika TXTNAMA → `TxtNama.Focus()` |
| 6 | `BeginInvoke` error saat form load kedua | `BeginInvoke` dipanggil sebelum window handle terbentuk | Tambahkan guard `If Not Me.IsHandleCreated Then Return` di `DgvData_CellLeave` |
| 7 | Teks hilang saat kembali ke TextBox dari ListBox | Tidak ada mekanisme simpan/restore teks | Tambahkan `_teksSebelumPindahKeLstBarang` — simpan saat Down, restore saat Up/Escape, reset setelah dipakai |
| 8 | Backspace di sel NamaBarang menghapus semua teks dan pindah fokus ke TxtNama | `ProcessCmdKey` handler `Keys.Back` memanggil `TxtNama.Select()` tanpa cek konteks — berlaku untuk DGV juga | Tambahkan guard `AndAlso _konteksLstBarang = "TXTNAMA"` agar hanya berlaku di jalur TxtNama |
| 9 | ListBox ditutup saat backspace (bukan saat pindah sel) | `CellLeave` terpicu oleh internal DGV saat editing, `BeginInvoke` mengecek `LstBarang.Focused=False` karena fokus di TextBox DGV | Simpan posisi sel saat ListBox dibuka (`_listBoxDibukaDiRow/Col`), skip tutup jika `CellLeave` dari sel yang sama |
| 10 | Setelah hapus baris (Delete), fokus tidak kembali ke baris berikutnya | `DgvData_KeyDown` handler Delete tidak memanggil `SetupFocusToGrid()` setelah `Hapusbaris()` | Tambahkan `SetupFocusToGrid()` setelah `Hapusbaris()` dan `ClearSelection()` |

**Flag yang Diperlukan (berbeda dari rencana awal):**
- `_sedangPindahKeLstBarang` — **TETAP DIPERLUKAN** sebagai guard transisi fokus di `CellLeave`
- `_teksSebelumPindahKeLstBarang` — **TAMBAHAN BARU** untuk simpan/restore teks
- `_konteksLstBarang` — **TETAP** untuk bedakan jalur TXTNAMA vs DGV

**Prioritas Keyboard (implementasi final):**
1. **Enter** (di TextBox) → Langsung ambil item pertama — paling cepat untuk kasir
2. **Down** (di TextBox) → Pindah ke ListBox, navigasi Up/Down
3. **Enter** (di ListBox) → Ambil item yang di-highlight
4. **Up** (di item pertama ListBox) → Kembali ke TextBox + restore teks
5. **Escape** → Tutup ListBox + restore teks
6. **Klik mouse** → Ambil item (optional)

**Catatan Penting — `ProcessCmdKey` vs `KeyDown`:**
- `ProcessCmdKey` berlaku untuk **seluruh form** — termasuk saat ListBox fokus
- Down arrow di ListBox harus dilewatkan ke ListBox dengan `Return MyBase.ProcessCmdKey(...)`
- Enter di `DgvNamaBarang_KeyDown` perlu ditangani di sini karena TextBox DGV mungkin mengkonsumsi Enter sebelum `ProcessCmdKey`

### Aturan 12: Dokumentasi Bahasa Indonesia untuk Semua Perubahan (WAJIB)
- **Konteks:** Setiap perubahan kode harus dilengkapi keterangan bahasa Indonesia
- **Tujuan:** Memudahkan pencarian dan pemahaman fungsi setiap perubahan
- **Format:** Gunakan komentar singkat di atas kode yang diubah

**Contoh Format:**
```vb
' [F3-T04-1] HAPUS: Navigasi ListView dengan flag state kompleks
' Alasan: ListBox tidak memerlukan flag state untuk navigasi keyboard
Private _sedangPindahKeLstBarang As Boolean = False  ' DIHAPUS

' [F4-T11b-1] TAMBAH: Event handler SelectedIndexChanged untuk ListBox
' Fungsi: Handle pemilihan item dari ListBox (mouse click atau keyboard)
Private Sub LstBarang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LstBarang.SelectedIndexChanged
```

**ID Format:** `[FASE-TASK-NOMOR]` contoh: `[F3-T04-1]` untuk FASE 3 TASK-04 item 1

**Kategori Keterangan:**
- **HAPUS:** Kode yang dihapus (dengan alasan)
- **TAMBAH:** Kode baru yang ditambahkan (dengan fungsi)
- **UBAH:** Kode yang dimodifikasi (dengan perubahan)
- **PERTAHANKAN:** Kode yang tidak diubah (dengan catatan)

**Catatan:** Gunakan akses-database.md untuk melihat struktur kolom database jika perlu dokumentasi query SQL.

---

## Status Legend
- `[ ]` Belum dikerjakan
- `[x]` Selesai
- `[~]` Sedang dikerjakan
- `[!]` Blocked / perlu keputusan

---

## FASE 1 — Persiapan

### TASK-01: Backup FormJual.vb
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
1. Copy file `FormJual.vb` ke `FormJual.vb.backup`
2. Pastikan backup tersimpan dengan baik

---

## FASE 2 — Perubahan Designer (Manual di Visual Studio)

### TASK-02: Ganti LstBarang dari ListView ke ListBox
**File:** `2Trans/FormJual.Designer.vb`
**Dikerjakan oleh:** Cascade (edit langsung di Designer.vb)
**Status:** `[x]`

**Langkah:**
1. Buka `FormJual` di Visual Studio Designer
2. Klik `LstBarang` (ListView) → Delete
3. Dari Toolbox, drag `ListBox` ke posisi yang sama
4. Rename menjadi `LstBarang`
5. Set properties:
   - `Font = Consolas, 10pt` (monospace untuk alignment)
   - `Visible = False`
   - `IntegralHeight = False` (opsional)
6. Position di lokasi yang sama dengan ListView lama

**Catatan:** Visual Studio akan otomatis mengupdate deklarasi di `FormJual.Designer.vb` saat Anda ganti kontrol. Namun, verifikasi manual lebih aman untuk memastikan tipe data berubah dari `ListView` ke `ListBox`.

---

## FASE 3 — Hapus Kode Navigasi ListView

### TASK-04: Hapus Flag State Navigasi (Hanya 3 flag)
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Cari dan hapus baris berikut (sekitar line 882-887):
```vb
Private _sedangPindahKeLstBarang As Boolean = False
Private _rowSaatPindahKeLst As Integer = -1
Private _lstBarangSelectedIndex As Integer = -1
Private _lstBarangBaruMasuk As Boolean = False
```

**Catatan:** Setelah menghapus flag ini, pastikan untuk menghapus juga referensi ke flag ini di kode lain:
- `_rowSaatPindahKeLst = -1` di AmbilDataDariListBox (sekitar line 1501)
- `_rowSaatPindahKeLst = -1` di DgvData_CellLeave (sekitar line 2674)
- `_lstBarangSelectedIndex = -1` di AmbilDataDariListBox (sekitar line 1502)

**JANGAN DIHAPUS (Masih diperlukan untuk logic bisnis):**
- `_konteksLstBarang` - Digunakan untuk membedakan jalur TXTNAMA vs DGV (barcode detection, dll)
- `_sedangSetNilaiDariListBox` - Digunakan sebagai guard untuk CellEndEdit agar tidak terpicu saat programmatic update

---

### TASK-05: Hapus Event Handler Navigasi ListView
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Hapus seluruh subroutine berikut:
- `Private Sub LstBarang_KeyDown(...)` (sekitar line 1444-1471)
- `Private Sub LstBarang_MouseClick(...)` (sekitar line 1473-1479)
- `Private Sub LstBarang_SizeChanged(...)` (sekitar line 1429-1442) - untuk hitung lebar kolom ListView, tidak diperlukan untuk ListBox

---

### TASK-06: Hapus ProcessCmdKey untuk Navigasi ListView
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Di `Protected Overrides Function ProcessCmdKey` (sekitar line 195-338):
1. Hapus block `If LstBarang.Visible Then` beserta isinya (line 196-221)
2. Hapus handler untuk Keys.Down, Keys.Up, Keys.Escape, Keys.Enter yang terkait ListView (line 224-257)
3. Hapus handler Keys.Back, Keys.Delete untuk ListView (line 3331-3335)

---

### TASK-07: Hapus DgvNamaBarang_KeyDown dan DgvNamaBarang_PreviewKeyDown
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Hapus seluruh subroutine berikut:
- `Private Sub DgvNamaBarang_KeyDown(...)` (sekitar line 2589-2628)
- `Private Sub DgvNamaBarang_PreviewKeyDown(...)` (sekitar line 2513-2515) - subroutine kosong, tidak diperlukan

**Step tambahan:** Di `EditingControlShowing` (sekitar line 2478-2510), hapus juga:
- RemoveHandler untuk DgvNamaBarang_PreviewKeyDown (sekitar line 2485)
- AddHandler untuk DgvNamaBarang_PreviewKeyDown (sekitar line 2493)

---

### TASK-07b: Tambah Navigasi Keyboard Sederhana untuk Mode 2 (KRITIS untuk UX)
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Tujuan:** Navigasi keyboard-first antara TextBox DGV dan ListBox. Kasir tidak perlu sentuh mouse.

**Implementasi Final (setelah debugging):**

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
                    ' PENTING: Tidak bisa Focus() langsung ke editing control dari luar.
                    ' Harus: DGV.Focus() → BeginInvoke → BeginEdit() → EditingControl.Focus()
                    Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                    _teksSebelumPindahKeLstBarang = ""
                    DgvDataTransaksi.Focus()
                    DgvDataTransaksi.BeginInvoke(New Action(Sub()
                        If DgvDataTransaksi.CurrentCell IsNot Nothing Then
                            DgvDataTransaksi.BeginEdit(True)
                            Dim editCtrl = TryCast(DgvDataTransaksi.EditingControl, TextBox)
                            If editCtrl IsNot Nothing AndAlso Not String.IsNullOrEmpty(teksSimpan) Then
                                editCtrl.Text = teksSimpan
                                editCtrl.SelectionStart = teksSimpan.Length
                            End If
                            editCtrl?.Focus()
                        End If
                        _sedangPindahKeLstBarang = False
                    End Sub))
                Else
                    ' Konteks TXTNAMA — langsung fokus ke TxtNama
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
            ' Restore teks dan kembali ke TextBox
            ' (sama dengan logika Up arrow)
            ...
    End Select
End Sub

' ── Event 3: LstBarang_SelectedIndexChanged ──────────────────────────────────
' HANYA tracking — TIDAK memanggil AmbilDataDariListBox()
' Alasan: event ini terpicu saat navigasi keyboard juga, bukan hanya saat memilih
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

**Variabel yang Diperlukan:**
```vb
Private _sedangPindahKeLstBarang As Boolean = False  ' Guard CellLeave saat transisi fokus
Private _teksSebelumPindahKeLstBarang As String = "" ' Simpan teks untuk restore saat Up/Escape
Private _listBoxDibukaDiRow As Integer = -1          ' Posisi sel saat ListBox dibuka
Private _listBoxDibukaDiCol As Integer = -1          ' Untuk guard CellLeave dari sel yang sama
```

**Aturan Reset `_teksSebelumPindahKeLstBarang`:**
- Di-set saat Down arrow (sebelum pindah ke ListBox)
- Di-reset setelah dipakai di Up arrow atau Escape
- Di-reset di awal `AmbilDataDariListBox()` (user sudah memilih, teks lama tidak relevan)

**Catatan Penting — `ProcessCmdKey`:**
Down arrow dari TxtNama/DGV dicegat oleh `ProcessCmdKey` (DGV mengkonsumsi arrow keys).
Tambahkan guard agar Down arrow di ListBox tidak dicegat:
```vb
Case Keys.Down
    If LstBarang.Focused Then
        Return MyBase.ProcessCmdKey(msg, keyData)  ' biarkan ListBox handle sendiri
    End If
    ' ... pindah fokus ke ListBox ...
```

---

### TASK-08: Hapus DgvData_CellLeave Guard untuk ListView
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Di `Private Sub DgvData_CellLeave`:
1. Hapus guard lama yang terkait ListView (`_rowSaatPindahKeLst`, dll)
2. Ganti dengan logika baru menggunakan `BeginInvoke`

**Implementasi Final:**
```vb
Private Sub DgvData_CellLeave(sender As Object, e As DataGridViewCellEventArgs) Handles DgvDataTransaksi.CellLeave
    ' Guard: BeginInvoke hanya bisa dipanggil setelah window handle terbentuk.
    ' Terjadi saat form di-load ulang atau data diisi ke DGV sebelum form siap.
    If Not Me.IsHandleCreated Then Return

    ' PENTING: Gunakan BeginInvoke agar cek dilakukan SETELAH fokus benar-benar berpindah.
    ' Tanpa BeginInvoke: saat user klik ListBox, CellLeave terpicu sebelum ListBox mendapat
    ' fokus → LstBarang.Focused masih False → ListBox ditutup sebelum user bisa memilih.
    Me.BeginInvoke(New Action(Sub()
        If LstBarang.Visible Then
            If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then
                ' Skip — ListBox sedang aktif atau dalam transisi fokus
            Else
                LstBarang.Visible = False
                LstBarang.Items.Clear()
            End If
        End If
    End Sub))
End Sub
```

**Masalah yang Ditemukan:**
- Tanpa `BeginInvoke`: ListBox ditutup sebelum user sempat klik karena `CellLeave` terpicu lebih dulu dari perpindahan fokus
- Tanpa guard `IsHandleCreated`: exception `InvalidOperationException` saat form di-load kedua kali (handle belum terbentuk)

---

### TASK-08b: Update EditingControlShowing untuk Hapus Guard LstBarang.Focused
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Di `EditingControlShowing` (sekitar line 2496-2500), hapus atau update:
```vb
Else
    If Not LstBarang.Focused Then
        LstBarang.Visible = False
        LstBarang.Items.Clear()
    End If
End If
```

**Ke:**
```vb
Else
    ' Tutup ListBox jika tidak di kolom NamaBarang
    LstBarang.Visible = False
    LstBarang.Items.Clear()
End If
```

**Catatan:** Cek `LstBarang.Focused` tidak diperlukan karena dengan ListBox, fokus akan kembali ke kontrol asli setelah selection.

---

### TASK-09: Hapus BeginInvoke untuk Paksa Ulang Selection
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Cari dan hapus semua `LstBarang.BeginInvoke(...)` yang digunakan untuk paksa ulang selection:
- Di `TxtNama_KeyDown` (sekitar line 904-911)
- Di `ProcessCmdKey` (sekitar line 240-247)
- Di `DgvNamaBarang_KeyDown` (sekitar line 2610-2617)
- Di `LstBarang_KeyDown` (sekitar line 1455-1459)

---

## FASE 4 — Update Logic SearchBarang

### TASK-10: Update SearchBarangToListView untuk Isi ListBox
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Di `Private Sub SearchBarangToListView` (sekitar line 1311-1414):

**Step 1: Ubah Query SQL - Batasi kolom yang diambil berdasarkan setting**

Ubah dari:
```vb
Dim query As String =
    "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
    "WHERE STATUS = 'Aktif' AND NAMA_BARANG LIKE @key " &
    "LIMIT 200"
```

Ke:
```vb
Dim query As String
If ModulHakAkses.SettingTampilInfoStok Then
    ' Ambil nama dan stok jika setting tampil stok = True
    query = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
            "WHERE STATUS = 'Aktif' AND NAMA_BARANG LIKE @key " &
            "LIMIT 200"
Else
    ' Hanya ambil nama jika setting tampil stok = False (lebih efisien)
    query = "SELECT NAMA_BARANG FROM tbl_barang " &
            "WHERE STATUS = 'Aktif' AND NAMA_BARANG LIKE @key " &
            "LIMIT 200"
End If
```

**Step 2: Ubah Loop Isi ListBox**

Ubah dari:
```vb
While rd.Read()
    Dim namaBarang = rd("NAMA_BARANG").ToString()
    Dim stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
    Dim stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)

    Dim item As New ListViewItem(namaBarang)
    Dim stokDisplay As String = ""
    If ModulHakAkses.SettingTampilInfoStok Then
        stokDisplay = String.Format("T : {0} | G : {1}", stokToko.ToString("N0"), stokGudang.ToString("N0"))
    Else
        Dim stok = ModuleAngka.SafeGetValue(Of Decimal)(rd, stokField, 0D)
        stokDisplay = String.Format("{0:N0}", stok)
    End If
    item.SubItems.Add(stokDisplay)
    LstBarang.Items.Add(item)
End While
```

Ke:
```vb
While rd.Read()
    Dim namaBarang = rd("NAMA_BARANG").ToString()
    Dim displayString As String = ""

    If ModulHakAkses.SettingTampilInfoStok Then
        ' Ambil stok dari query (kolom STOK_TOKO dan STOK_GUDANG ada)
        Dim stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
        Dim stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
        Dim stokDisplay As String = String.Format("T: {0} | G: {1}", stokToko.ToString("N0"), stokGudang.ToString("N0"))
        displayString = String.Format("{0,-30} | {1}", namaBarang, stokDisplay)
    Else
        ' Hanya tampilkan nama barang saja (query tidak mengambil stok)
        displayString = namaBarang
    End If

    LstBarang.Items.Add(displayString)
End While
```

**Catatan:** Dengan perubahan ini, query database lebih efisien karena tidak mengambil data stok jika tidak diperlukan.

---

### TASK-11: Update AmbilDataDariListBox untuk Ambil dari ListBox
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Di `Private Sub AmbilDataDariListBox` (sekitar line 1481-1597):

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
' Jika setting tampil stok = True, format: "Nama Barang | T: 100 | G: 50"
' Jika setting tampil stok = False, format: "Nama Barang" saja
If selectedValue.Contains("|"c) Then
    Dim parts As String() = selectedValue.Split("|"c)
    If parts.Length > 0 Then
        selectedValue = parts(0).Trim()
    End If
End If
```

Hapus juga:
- `_rowSaatPindahKeLst = -1` (reset row tersimpan) - flag ini sudah dihapus di TASK-04
- `_lstBarangSelectedIndex = -1` (reset index tersimpan) - flag ini sudah dihapus di TASK-04

---

### TASK-11b: Tambah Event Handler untuk ListBox (SelectedIndexChanged + Click)
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Tambah dua event handler untuk ListBox:

```vb
' HANYA tracking — TIDAK memanggil AmbilDataDariListBox()
' ALASAN KRITIS: SelectedIndexChanged terpicu saat navigasi keyboard (Up/Down) juga,
' bukan hanya saat user benar-benar memilih. Jika AmbilDataDariListBox() dipanggil di sini,
' barang akan diambil otomatis saat user masih navigasi → UX rusak.
Private Sub LstBarang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LstBarang.SelectedIndexChanged
    ' tracking only
End Sub

' Handle pemilihan via mouse click
Private Sub LstBarang_Click(sender As Object, e As EventArgs) Handles LstBarang.Click
    If LstBarang.SelectedIndex >= 0 Then
        _sedangPindahKeLstBarang = True
        AmbilDataDariListBox()
        _sedangPindahKeLstBarang = False
    End If
End Sub
```

**Catatan Kritis:**
- `SelectedIndexChanged` dipanggil saat `SelectedIndex = 0` di-set di `ProcessCmdKey` (Down arrow) — ini bukan pemilihan user
- Pemilihan aktual hanya via: `LstBarang_Click` (mouse) dan `LstBarang_KeyDown` (Enter)

---

### TASK-12: Set Font ListBox ke Monospace
**File:** `2Trans/FormJual.Designer.vb` atau `FormJual.vb`
**Status:** `[x]`

**Langkah:**
Di `FormJual.Designer.vb` atau di `Form_Load`:
```vb
LstBarang.Font = New Font("Consolas", 10)
```

---

## FASE 5 — Verifikasi Events TxtNama & DGV (Tidak Berubah)

### TASK-13: Verifikasi Events TxtNama dan DGV Tidak Berubah
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
Verifikasi bahwa events berikut **TIDAK PERLU DIUBAH** karena hanya trigger search/logic bisnis:

| Event | Status | Keterangan |
|-------|--------|------------|
| `TxtNama_TextChanged` | ✅ Tetap | Trigger search, barcode detection |
| `TxtNama_KeyDown` | ⚠️ Hapus navigasi Keys.Down | Sudah di TASK-09, sisanya tetap |
| `DgvNamaBarang_TextChanged` | ✅ Tetap | Trigger search untuk DGV inline edit |
| `DgvDataData_CellEndEdit` | ✅ Tetap | Logic parsing dan query DB |

**Catatan:** Hapus hanya bagian navigasi Keys.Down ke ListView dari `TxtNama_KeyDown` (sudah tercover di TASK-09). Semua logic bisnis parsing dan search tetap sama.

---

## FASE 6 — Testing (7 Tasks)

### TASK-14: Test Barcode Detection
**Status:** `[x]`

**Langkah:**
1. Scan barcode numeric (8991234567890)
2. Scan barcode alphanumeric (ABC-123)
3. Verifikasi barang ditemukan dan ditambahkan ke DGV

---

### TASK-15: Test Format Input
**Status:** `[x]`

**Langkah:**
1. Input `5*nama` (qty*nama)
2. Input `5*2*nama` (qty*level*nama)
3. Input `5*barcode` (qty*barcode)
4. Verifikasi parsing dan qty sesuai

---

### TASK-16: Test Jalur TxtNama
**Status:** `[x]`

**Langkah:**
1. Ketik nama barang manual
2. Verifikasi ListBox muncul dengan hasil search
3. Klik item di ListBox
4. Verifikasi barang ditambahkan ke DGV
5. Verifikasi backspace berfungsi untuk koreksi

---

### TASK-17: Test Jalur DGV
**Status:** `[x]`

**Langkah:**
1. Edit sel NamaBarang di DGV
2. Ketik nama barang manual
3. Verifikasi ListBox muncul dengan hasil search
4. Klik item di ListBox
5. Verifikasi data diisi ke sel
6. Verifikasi backspace berfungsi untuk koreksi
7. Verifikasi pindah ke sel lain → ListBox tertutup

---

### TASK-18: Test Cek Duplikat
**Status:** `[x]`

**Langkah:**
1. Tambah barang yang sama dua kali
2. Verifikasi error message muncul
3. Verifikasi barang tidak ditambahkan duplikat

---

### TASK-19: Test Auto Level
**Status:** `[x]`

**Langkah:**
1. Edit kolom QTY
2. Verifikasi satuan dan harga update otomatis berdasarkan level

---

### TASK-20: Verifikasi Logic Bisnis
**Status:** `[x]`

**Langkah:**
1. Verifikasi barcode detection hybrid tetap berfungsi
2. Verifikasi parsing format input tetap berfungsi
3. Verifikasi cek duplikat tetap berfungsi
4. Verifikasi auto level tetap berfungsi
5. Verifikasi kalkulasi total harga tetap berfungsi

---

## FASE 7 — Cleanup (2 Tasks)

### TASK-21: Hapus Kode Tidak Terpakai
**File:** `2Trans/FormJual.vb`
**Status:** `[x]`

**Langkah:**
1. Cari semua referensi ke flag yang dihapus
2. Hapus variabel yang tidak digunakan
3. Hapus komentar yang tidak relevan

---

### TASK-22: Build dan Verifikasi
**Status:** `[x]`

**Langkah:**
1. Build project
2. Verifikasi tidak ada error compile
3. Verifikasi tidak ada warning

---

## FASE 8 — Verifikasi Akhir (2 Tasks dengan 80 Checklist)

### TASK-23: Checklist Verifikasi Perubahan Kode
**Status:** `[x]`

**Checklist Hapus:**
- [ ] Flag `_sedangPindahKeLstBarang` dihapus
- [ ] Flag `_rowSaatPindahKeLst` dihapus
- [ ] Flag `_lstBarangSelectedIndex` dihapus
- [ ] Flag `_lstBarangBaruMasuk` dihapus
- [ ] Referensi `_rowSaatPindahKeLst = -1` di AmbilDataDariListBox dihapus
- [ ] Referensi `_rowSaatPindahKeLst = -1` di DgvData_CellLeave dihapus
- [ ] Referensi `_lstBarangSelectedIndex = -1` di AmbilDataDariListBox dihapus
- [ ] Subroutine `LstBarang_KeyDown` dihapus
- [ ] Subroutine `LstBarang_MouseClick` dihapus
- [ ] Subroutine `LstBarang_SizeChanged` dihapus
- [ ] Subroutine `DgvNamaBarang_KeyDown` dihapus
- [ ] Subroutine `DgvNamaBarang_PreviewKeyDown` dihapus
- [ ] RemoveHandler `DgvNamaBarang_PreviewKeyDown` di EditingControlShowing dihapus
- [ ] AddHandler `DgvNamaBarang_PreviewKeyDown` di EditingControlShowing dihapus
- [ ] Block `If LstBarang.Visible Then` di ProcessCmdKey dihapus
- [ ] Handler Keys.Down/Up/Escape/Enter untuk ListView di ProcessCmdKey dihapus
- [ ] Handler Keys.Back/Delete untuk ListView di ProcessCmdKey dihapus
- [ ] Guard `If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then Return` di DgvData_CellLeave dihapus
- [ ] Guard `If LstBarang.Visible AndAlso e.RowIndex = _rowSaatPindahKeLst Then Return` di DgvData_CellLeave dihapus
- [ ] Semua `LstBarang.BeginInvoke(...)` untuk paksa ulang selection dihapus
- [ ] Navigasi Keys.Down di TxtNama_KeyDown dihapus

**Checklist Tambah/Update:**
- [ ] Kontrol `LstBarang` di Designer berubah dari ListView ke ListBox
- [ ] Deklarasi `LstBarang` di Designer berubah dari ListView ke ListBox
- [ ] Font `LstBarang` set ke Consolas 10pt
- [ ] Event handler `LstBarang_SelectedIndexChanged` ditambah
- [ ] Query SQL di SearchBarangToListView conditional berdasarkan SettingTampilInfoStok
- [ ] Loop isi ListBox menggunakan format string (bukan ListViewItem)
- [ ] AmbilDataDariListBox menggunakan `LstBarang.SelectedIndex` dan `SelectedItem`
- [ ] Parsing nama barang dengan cek tanda `|`
- [ ] EditingControlShowing hapus cek `LstBarang.Focused`
- [ ] EditingControlShowing langsung tutup ListBox jika tidak di kolom NamaBarang

**Checklist Pertahankan:**
- [ ] Flag `_konteksLstBarang` TETAP ada
- [ ] Flag `_sedangSetNilaiDariListBox` TETAP ada
- [ ] Event `DgvNamaBarang_TextChanged` TETAP ada
- [ ] Event `DgvDataData_CellEndEdit` TETAP ada
- [ ] Event `DgvData_KeyDown` TETAP ada
- [ ] Event `DgvData_CellMouseUp` TETAP ada
- [ ] Event `DgvData_CellEnter` TETAP ada
- [ ] Event `DgvData_Leave` TETAP ada
- [ ] Event `DgvData_CellFormatting` TETAP ada
- [ ] Event `DgvData_DataError` TETAP ada
- [ ] Event `DgvData_RowPostPaint` TETAP ada
- [ ] Semua setting (14 setting) TETAP berfungsi
- [ ] Logic barcode detection TETAP berfungsi
- [ ] Logic parsing format input TETAP berfungsi
- [ ] Logic cek duplikat TETAP berfungsi
- [ ] Logic auto level TETAP berfungsi
- [ ] Logic kalkulasi total harga TETAP berfungsi

---

### TASK-24: Checklist Verifikasi Runtime
**Status:** `[~]` (Perlu testing manual oleh user)

**Checklist Jalur TxtNama:**
- [ ] Ketik nama barang manual → ListBox muncul
- [ ] Format string ditampilkan dengan benar (dengan/ tanpa stok)
- [ ] Klik item di ListBox → barang ditambahkan ke DGV
- [ ] Backspace berfungsi untuk koreksi
- [ ] Escape menutup ListBox
- [ ] Scan barcode numeric → barang ditemukan
- [ ] Scan barcode alphanumeric → barang ditemukan
- [ ] Format `5*nama` → parsing qty dan nama benar
- [ ] Format `5*2*nama` → parsing qty, level, dan nama benar
- [ ] Format `5*barcode` → parsing qty dan barcode benar

**Checklist Jalur DGV:**
- [ ] Edit sel NamaBarang → ListBox muncul
- [ ] Format string ditampilkan dengan benar (dengan/ tanpa stok)
- [ ] Klik item di ListBox → data diisi ke sel
- [ ] Backspace berfungsi untuk koreksi
- [ ] Escape menutup ListBox
- [ ] Pindah ke sel lain → ListBox tertutup
- [ ] Cek duplikat barang berfungsi
- [ ] Auto level berdasarkan QTY berfungsi

**Checklist Setting:**
- [ ] SettingTampilInfoStok = True → stok ditampilkan di ListBox dan DGV
- [ ] SettingTampilInfoStok = False → hanya nama di ListBox, stok tidak ditampilkan
- [ ] SettingIzinkanSatuanBerbeda = True → barang sama dengan satuan berbeda diizinkan
- [ ] SettingIzinkanSatuanBerbeda = False → barang sama dengan satuan berbeda dicegah
- [ ] SettingFokusOtomatis = True → fokus ke TxtNama
- [ ] SettingFokusOtomatis = False → fokus ke DGV
- [ ] SettingAutoLevelSatuan = True → satuan otomatis berdasarkan qty
  - [ ] **Jalur TxtNama:** Auto level aktif saat tambah barang dari TxtNama (IsiBarangKeRow)
  - [ ] **Jalur DGV (sel NamaBarang):** Auto level aktif saat edit inline dan tambah barang (IsiBarangKeRow)
  - [ ] **Jalur DGV (kolom QTY):** Auto level aktif saat edit QTY langsung (CellEndEdit)
  - [ ] Dropdown satuan tidak muncul (tidak perlu karena auto)
- [ ] SettingAutoLevelSatuan = False → dropdown satuan otomatis terbuka saat masuk kolom Satuan (DgvData_CellEnter)

**Checklist Error Handling:**
- [ ] Tidak ada error saat build
- [ ] Tidak ada warning saat build
- [ ] Tidak ada runtime error saat buka form
- [ ] Tidak ada error saat search barang
- [ ] Tidak ada error saat tambah barang
- [ ] Tidak ada error saat edit barang
- [ ] Tidak ada error saat hapus barang
- [ ] Tidak ada error saat pembayaran

---

## FASE 9 — Dokumentasi Setting dan Event DGV (2 Tasks)

### TASK-25: Dokumentasi Semua Setting yang Perlu Diperhatikan
**File:** `2Trans/FormJual.vb`
**Status:** `[ ]`

**Langkah:**
Verifikasi bahwa semua setting berikut tetap berfungsi setelah migrasi:

| Setting | Lokasi Penggunaan | Fungsi | Status Setelah Migrasi |
|---------|------------------|--------|------------------------|
| `SettingTampilInfoStok` | SearchBarangToListView, DGV columns | Tampilkan/sembunyikan info stok | ✅ TETAP - sudah dihandle di TASK-10 |
| `SettingIzinkanSatuanBerbeda` | Cek duplikat (TambahDataLangsung, AmbilDataDariListBox, CellEndEdit) | Izinkan barang sama dengan satuan berbeda | ✅ TETAP - logic bisnis |
| `SettingFokusOtomatis` | SetupFocusToGrid | Fokus ke TxtNama atau DGV | ✅ TETAP - logic bisnis |
| `SettingIzinkanUbahHargaJual` | DGV column setup | Kolom Harga ReadOnly | ✅ TETAP - tidak terkait ListView |
| `SettingIzinkanDiskonItem` | DGV column setup | Kolom Diskon Visible | ✅ TETAP - tidak terkait ListView |
| `SettingSembunyikanPencarianAtas` | PanelCari Visible | Sembunyikan panel pencarian | ✅ TETAP - tidak terkait ListView |
| `SettingHargaJualOtomatisUpdateMaster` | CellEndEdit | Update master barang saat harga diubah | ✅ TETAP - logic bisnis |
| `SettingAutoLevelSatuan` | CellEnter, TentukanLevelDariQty, UpdateLevelSatuanBaris | Auto level satuan berdasarkan qty | ✅ TETAP - logic bisnis |
| `SettingIzinkanNominalJualNol` | Validasi pembayaran | Izinkan total jual 0 | ✅ TETAP - logic bisnis |
| `SettingIzinkanJualRugi` | Validasi pembayaran | Izinkan jual rugi | ✅ TETAP - logic bisnis |
| `SettingIzinkanBarangMinus` | Validasi pembayaran, CekStok | Izinkan stok minus | ✅ TETAP - logic bisnis |
| `SettingLangsungIsiNominalTotal` | Pembayaran | Isi nominal bayar otomatis | ✅ TETAP - logic bisnis |
| `SettingBatasSatuanSedang` | TentukanLevelDariQty | Threshold auto level sedang | ✅ TETAP - logic bisnis |
| `SettingBatasSatuanBesar` | TentukanLevelDariQty | Threshold auto level besar | ✅ TETAP - logic bisnis |

---

### TASK-26: Dokumentasi Event DGV dan Status Setelah Migrasi
**File:** `2Trans/FormJual.vb`
**Status:** `[ ]`

**Langkah:**
Verifikasi status setiap event DGV setelah migrasi:

| Event | Fungsi | Terkait ListView? | Status Setelah Migrasi |
|-------|--------|-------------------|------------------------|
| `CellEndEdit` | Parsing nama barang, query DB, update harga, cek duplikat | ❌ TIDAK (logic bisnis) | ✅ PERTAHANKAN - logic bisnis |
| `RowPostPaint` | Gambar nomor urut row header | ❌ TIDAK | ✅ PERTAHANKAN - visual |
| `EditingControlShowing` | Attach handler untuk inline edit (TextChanged, KeyDown, PreviewKeyDown) | ⚠️ YA (hapus KeyDown, PreviewKeyDown) | ⚠️ UPDATE - TASK-07, TASK-08b |
| `CellLeave` | Guard untuk ListView (tutup jika pindah sel) | ✅ YA | ⚠️ UPDATE - TASK-08, TASK-08b |
| `KeyDown` | Hapus baris dengan Delete | ❌ TIDAK | ✅ PERTAHANKAN - logic bisnis |
| `CellMouseUp` | Right click context menu | ❌ TIDAK | ✅ PERTAHANKAN - UX |
| `CellEnter` | Auto buka dropdown satuan | ❌ TIDAK | ✅ PERTAHANKAN - UX |
| `Leave` | Clear petunjuk | ❌ TIDAK | ✅ PERTAHANKAN - UX |
| `CellFormatting` | Format stok warna (merah jika minus) | ❌ TIDAK | ✅ PERTAHANKAN - visual |
| `DataError` | Error handling | ❌ TIDAK | ✅ PERTAHANKAN - error handling |

---

### TASK-27: Sederhanakan Percabangan (Optional - Setelah Migrasi Stabil)
**Status:** `[x]` (Sudah tercover di TASK-04, TASK-08, TASK-08b)

**Langkah:**
1. **AmbilDataDariListBox** - Sederhanakan selection logic dari 3 cabang ke 2 cabang
2. **DgvData_CellLeave** - Hapus guard yang tidak diperlukan lagi
3. **EditingControlShowing** - Hapus cek `LstBarang.Focused`
4. **Hapus reset flag** yang tidak diperlukan lagi (`_rowSaatPindahKeLst`, `_lstBarangSelectedIndex`)

**Catatan:** Task ini optional, hanya untuk cleanup. Jangan dikerjakan bersamaan dengan migrasi utama.

---

### TASK-28: Perbaikan SetupFocusToGrid (Optional - Setelah Migrasi Stabil)
**Status:** `[x]`

**Analisis Kode Saat Ini:**
- **15 lokasi pemanggilan** - Terdistribusi di seluruh form
- **2 mode:** Pencarian (TxtNama) dan Edit Langsung (DGV)
- **Issue:** 3x BeginInvoke bersarang menyebabkan delay
- **Issue:** Right-click ContextMenu tidak panggil SetupFocusToGrid setelah action

**Perbaikan yang Disarankan:**

**1. Sederhanakan BeginInvoke (dari 3 level ke 1 level):**
```vb
' Sebelumnya (3 level - terlalu kompleks)
DgvDataTransaksi.BeginInvoke(New Action(Sub()
    DgvDataTransaksi.BeginInvoke(New Action(Sub()
        DgvDataTransaksi.BeginEdit(True)
    End Sub))
End Sub))

' Sesudah (1 level - lebih cepat)
DgvDataTransaksi.BeginInvoke(New Action(Sub()
    If DgvDataTransaksi.CurrentCell IsNot Nothing Then
        DgvDataTransaksi.BeginEdit(True)
        DgvDataTransaksi.EditingControl?.Focus()
    End If
End Sub))
```

**2. Tambah SetupFocusToGrid setelah ContextMenu action:**
```vb
' Di event handler ContextMenu (HapusBaris, Edit, dll)
Private Sub ContextMenuAction_Click(sender As Object, e As EventArgs)
    ' ... logic action ...
    
    ' Kembalikan fokus sesuai setting
    SetupFocusToGrid()
End Sub
```

**3. Simpan CurrentCell sebelum BeginInvoke:**
```vb
' Simpan reference sebelum async operation
Dim targetCell As DataGridViewCell = DgvDataTransaksi.CurrentCell
DgvDataTransaksi.BeginInvoke(New Action(Sub()
    ' Cek apakah masih cell yang sama
    If DgvDataTransaksi.CurrentCell IsNot Nothing AndAlso 
       DgvDataTransaksi.CurrentCell.RowIndex = targetCell.RowIndex AndAlso
       DgvDataTransaksi.CurrentCell.ColumnIndex = targetCell.ColumnIndex Then
        DgvDataTransaksi.BeginEdit(True)
    End If
End Sub))
```

**Catatan:** Task ini optional untuk improvement UX. Tidak terkait migrasi ListView→ListBox. Jangan dikerjakan bersamaan dengan migrasi utama.

---

## SUMMARY

**Total Langkah:** 28 tasks (26 utama + 2 optional)

**Breakdown:**
- FASE 1: Persiapan (1 task)
- FASE 2: Perubahan Designer (1 task) - TASK-03 dihapus, gabung ke TASK-02
- FASE 3: Hapus Kode Navigasi ListView (6 tasks) - TASK-07b ditambahkan untuk UX Mode 2
- FASE 4: Update Logic SearchBarang (4 tasks) - TASK-11b ditambahkan di sini
- FASE 5: Verifikasi Events (1 task) - TASK-13,14,15,16 digabung jadi 1 task
- FASE 6: Testing (7 tasks)
- FASE 7: Cleanup (2 tasks)
- FASE 8: Verifikasi Akhir (2 tasks dengan 80 checklist)
- FASE 9: Dokumentasi Setting dan Event DGV (2 tasks)
- **FASE 10: Optional Improvements (2 tasks)** - TASK-27, TASK-28

**Total Checklist Verifikasi:**
- TASK-23: 46 checklist (21 hapus + 10 tambah/update + 15 pertahankan)
- TASK-24: 34 checklist (10 TxtNama + 7 DGV + 9 setting + 8 error handling)
- **Total: 80 checklist**

**Estimasi Pengurangan Kode:**
- Hapus ~200-250 baris (flag state, event handler navigasi, BeginInvoke)
- Tambah ~50-100 baris (format string, parsing)
- Net: Reduce ~100-200 baris

**Risk:** Rendah - logic bisnis tidak berubah, hanya UI control

**Rollback:** Jika ada masalah, restore dari backup `FormJual.vb.backup`
