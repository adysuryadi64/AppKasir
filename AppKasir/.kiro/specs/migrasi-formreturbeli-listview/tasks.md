# Tasks — Migrasi FormReturBeli: ListBox → ListView + Perbaikan DGV

## Status Legend
- `[ ]` Belum dikerjakan
- `[x]` Selesai
- `[~]` Sedang dikerjakan
- `[!]` Blocked / perlu keputusan

---

## FASE 1 — Perubahan Designer (Manual di Visual Studio)

### TASK-01: Ganti LstBarang dari ListBox ke ListView
**File:** `2Trans/FormReturBeli.Designer.vb`
**Dikerjakan oleh:** User (manual di Visual Studio Designer)
**Status:** `[ ]`

**Langkah:**
1. Buka `FormReturBeli` di Visual Studio Designer
2. Klik `LstBarang` → Delete
3. Dari Toolbox, drag `ListView` ke posisi yang sama (Location: `6, 173`, Size: `533, 293`)
4. Rename menjadi `LstBarang`
5. Set properties:
   - `View = Details`
   - `FullRowSelect = True`
   - `GridLines = True`
   - `MultiSelect = False`
   - `HeaderStyle = Nonclickable` (opsional, agar header tidak bisa diklik untuk sort)
   - `Font = Century Gothic, 9.75pt` (sama dengan sebelumnya)
6. Tambahkan 2 kolom via property `Columns`:
   - Kolom 1: `Text = "Nama Barang"`, `Width = 430`, `TextAlign = Left`
   - Kolom 2: `Text = "Stok"`, `Width = 80`, `TextAlign = Right`
7. Pastikan `TabIndex = 138` (sama dengan sebelumnya)

**Verifikasi:** Di Designer.vb, `LstBarang` harus bertipe `System.Windows.Forms.ListView`

---

### TASK-02: Ubah EditMode DGV ke EditOnKeystrokeOrF2
**File:** `2Trans/FormReturBeli.Designer.vb`
**Dikerjakan oleh:** User (manual di Visual Studio Designer)
**Status:** `[ ]`

**Langkah:**
1. Buka `FormReturBeli` di Visual Studio Designer
2. Klik `DgvData`
3. Di Properties panel, cari `EditMode`
4. Ubah dari `EditOnEnter` ke `EditOnKeystrokeOrF2`

**Verifikasi:** Di Designer.vb, baris `Me.DgvData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystrokeOrF2`

---

## FASE 2 — Perubahan Kode (Dikerjakan AI)

> **PENTING:** Semua task di bawah dikerjakan setelah TASK-01 dan TASK-02 selesai.
> Gunakan `strReplace` untuk semua perubahan. Jangan `fsWrite` pada file yang sudah ada.

---

### TASK-03: Tambah Variabel Baru di Section Deklarasi
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-03
**Status:** `[ ]`

Tambahkan setelah blok variabel barcode yang sudah ada:
```vb
' ── State management untuk pencarian ListView ─────────────────────
Private _dgvEditingTextBox As TextBox = Nothing         ' TextBox editing control di DGV
Private _sedangPindahKeLstBarang As Boolean = False     ' Flag saat fokus pindah ke ListView
Private _rowSaatPindahKeLst As Integer = -1             ' Baris DGV saat pindah ke ListView
Private _lstBarangSelectedIndex As Integer = -1         ' Index item terpilih di ListView
Private _lstBarangBaruMasuk As Boolean = False          ' Flag saat ListView baru dapat fokus dari DGV
Private _konteksLstBarang As String = "TXTNAMA"         ' Konteks: "TXTNAMA" atau "DGV"
Private _sedangSetNilaiDariListBox As Boolean = False   ' Guard CellEndEdit saat isi dari ListView
```

---

### TASK-04: Ganti SearchBarangByText untuk Isi ListView
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-04
**Status:** `[ ]`

Ganti seluruh fungsi `SearchBarangByText` dengan versi yang mengisi `ListView`:
- Buat `ListViewItem` dengan `Text = nama barang`, `SubItems.Add(stok.ToString("N0"))`
- Simpan `ID_BARANG` di `lvi.Tag`
- Stok ≤ 0 → `lvi.ForeColor = Color.Red` (atau token ModuleTheme jika tersedia)
- Panggil `PosisikanLstBarangDiBawahTxtNama()` setelah isi ListView (jika konteks TXTNAMA)
- Panggil `PosisikanLstBarangDiBawahSel()` setelah isi ListView (jika konteks DGV)

---

### TASK-05: Tambah PosisikanLstBarangDiBawahTxtNama dan PosisikanLstBarangDiBawahSel
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-14
**Status:** `[ ]`

Tambahkan 2 fungsi helper untuk posisi ListView:

```vb
' Posisikan ListView di bawah TxtNama (pencarian dari TxtNama)
Private Sub PosisikanLstBarangDiBawahTxtNama()
    Dim pt = Me.PointToClient(PanelCari.PointToScreen(New Point(0, PanelCari.Height)))
    LstBarang.Location = New Point(PanelCari.Left, pt.Y)
    LstBarang.Width = PanelCari.Width
    LstBarang.BringToFront()
End Sub

' Posisikan ListView di bawah sel NAMA_BARANG yang sedang diedit di DGV
Private Sub PosisikanLstBarangDiBawahSel()
    If DgvData.CurrentCell Is Nothing Then Return
    Dim cellRect = DgvData.GetCellDisplayRectangle(DgvData.CurrentCell.ColumnIndex,
                                                    DgvData.CurrentCell.RowIndex, False)
    Dim pt = Me.PointToClient(DgvData.PointToScreen(New Point(cellRect.Left, cellRect.Bottom)))
    LstBarang.Location = New Point(pt.X, pt.Y)
    LstBarang.Width = Math.Max(cellRect.Width, 510)
    LstBarang.BringToFront()
End Sub
```

---

### TASK-06: Ganti AmbilDataDariListBox untuk Baca dari ListView
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-05
**Status:** `[ ]`

Ganti seluruh fungsi `AmbilDataDariListBox`:
- Baca dari `LstBarang.Items(_lstBarangSelectedIndex)` bukan `LstBarang.SelectedItem`
- Ambil nama dari `.Text`, ambil ID dari `.Tag`
- `_sedangSetNilaiDariListBox = True` di awal
- `_sedangSetNilaiDariListBox = False` di akhir, setelah `TambahDataLangsung`/`IsiBarangKeRow` selesai
- Setelah selesai: `LstBarang.Visible = False`, fokus kembali ke `TxtNama` atau DGV sesuai `_konteksLstBarang`

---

### TASK-07: Ganti LstBarang_KeyDown untuk ListView
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-06
**Status:** `[ ]`

Ganti seluruh `LstBarang_KeyDown`:
- `Keys.Enter` → update `_lstBarangSelectedIndex` dari `LstBarang.FocusedItem.Index`, panggil `AmbilDataDariListBox()`
- `Keys.Down` → `_lstBarangSelectedIndex += 1`, pilih item berikutnya
- `Keys.Up` → jika index > 0: `_lstBarangSelectedIndex -= 1`; jika index = 0: kembali ke `TxtNama`
- `Keys.Escape` → `LstBarang.Visible = False`, kembali ke `TxtNama`
- Handle `_lstBarangBaruMasuk`: saat pertama masuk, paksa select item pertama via `BeginInvoke`

---

### TASK-08: Ganti LstBarang_MouseClick untuk ListView
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-07
**Status:** `[ ]`

Ganti `LstBarang_MouseClick`:
- Ambil item yang diklik dari `LstBarang.GetItemAt(e.X, e.Y)`
- Update `_lstBarangSelectedIndex`
- Panggil `AmbilDataDariListBox()`

---

### TASK-09: Ganti LstBarang_GotFocus, LstBarang_LostFocus, LstBarang_Enter
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-15
**Status:** `[ ]`

- `LstBarang_GotFocus`: set `_lstBarangBaruMasuk = True`, pilih item pertama jika belum ada yang terpilih
- `LstBarang_LostFocus`: biarkan atau hapus (tidak kritis)
- `LstBarang_Enter`: sesuaikan untuk ListView (pilih item pertama jika belum ada)

---

### TASK-10: Hapus LstBarang_DrawItem
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-15
**Status:** `[ ]`

Hapus seluruh fungsi `LstBarang_DrawItem` — tidak relevan untuk `ListView`.

---

### TASK-11: Tambah DgvNamaBarang_TextChanged, DgvNamaBarang_KeyDown, DgvNamaBarang_PreviewKeyDown
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-08
**Status:** `[ ]`

Tambahkan 3 fungsi baru (salin pola dari FormJual, sesuaikan nama kontrol):

**DgvNamaBarang_TextChanged:**
- Parse format `qty*nama` dari teks yang diketik
- Set `TxtQty.Text` jika ada qty
- Feed keyword ke `SearchBarangByText`
- Set `_konteksLstBarang = "DGV"`
- Posisikan ListView via `PosisikanLstBarangDiBawahSel()`

**DgvNamaBarang_KeyDown:**
- `Keys.Down` → pindah fokus ke `LstBarang`, set `_lstBarangBaruMasuk = True`
- `Keys.Escape` → `LstBarang.Visible = False`

**DgvNamaBarang_PreviewKeyDown:**
- Set `e.IsInputKey = True` untuk `Keys.Down` dan `Keys.Up` agar tidak "bocor"

---

### TASK-12: Ganti DgvData_EditingControlShowing
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-08
**Status:** `[ ]`

Ganti isi `DgvData_EditingControlShowing`:
- Kolom `NAMA_BARANG` (index 1):
  - `autoText.AutoCompleteMode = AutoCompleteMode.None` (matikan AutoComplete bawaan)
  - Remove handler lama dari `_dgvEditingTextBox`
  - Set `_dgvEditingTextBox = autoText`
  - Attach `DgvNamaBarang_TextChanged`, `DgvNamaBarang_KeyDown`, `DgvNamaBarang_PreviewKeyDown`
  - Panggil `PosisikanLstBarangDiBawahSel()`
- Kolom `SATUAN` (index 4): pertahankan handler `ComboBox_SelectedIndexChanged` dan `ComboBox_KeyDown`
- Hapus blok AutoComplete lama yang pakai `AutoCompleteStringCollection`

---

### TASK-13: Tambah DgvData_CellEnter Handler
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-09
**Status:** `[ ]`

Tambahkan fungsi baru `DgvData_CellEnter`:
```vb
Private Sub DgvData_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles DgvData.CellEnter
    ' Kolom SATUAN — langsung BeginEdit + buka dropdown
    If DgvData.Columns(e.ColumnIndex).Name = "SATUAN" Then
        DgvData.BeginInvoke(New Action(Sub()
            If DgvData.CurrentCell IsNot Nothing AndAlso
               DgvData.CurrentCell.ColumnIndex = e.ColumnIndex AndAlso
               DgvData.CurrentCell.RowIndex = e.RowIndex Then
                DgvData.BeginEdit(True)
                Dim combo = TryCast(DgvData.EditingControl, ComboBox)
                If combo IsNot Nothing Then combo.DroppedDown = True
            End If
        End Sub))
    End If

    ' Kolom NAMA_BARANG — langsung BeginEdit agar bisa langsung ketik
    If DgvData.Columns(e.ColumnIndex).Name = "NAMA_BARANG" Then
        DgvData.BeginInvoke(New Action(Sub()
            If DgvData.CurrentCell IsNot Nothing AndAlso
               DgvData.CurrentCell.ColumnIndex = e.ColumnIndex AndAlso
               DgvData.CurrentCell.RowIndex = e.RowIndex Then
                DgvData.BeginEdit(True)
            End If
        End Sub))
    End If

    ' Sembunyikan ListView saat pindah ke kolom lain
    If DgvData.Columns(e.ColumnIndex).Name <> "NAMA_BARANG" Then
        LstBarang.Visible = False
    End If
End Sub
```

---

### TASK-14: Tambah SetupFocusToGrid
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-10
**Status:** `[ ]`

Tambahkan fungsi `SetupFocusToGrid` dengan pola dari `pola-form-transaksi.md`:
- Nama kolom kode: `"ID_BARANG"`
- Index kolom NamaBarang: `1` (kolom `NAMA_BARANG`)
- `CurrentCell = DgvData(1, targetRow)` synchronous
- `BeginEdit` via nested `BeginInvoke`

---

### TASK-15: Ganti Semua Pemanggil Fokuskepencarianbarang → SetupFocusToGrid
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-10
**Status:** `[ ]`

Cari semua pemanggilan `Fokuskepencarianbarang()` dan ganti dengan `SetupFocusToGrid()`.
Gunakan PowerShell untuk cari dulu:
```powershell
Select-String -Path "2Trans/FormReturBeli.vb" -Pattern "Fokuskepencarianbarang"
```

---

### TASK-16: Tambah Guard _sedangSetNilaiDariListBox di CellEndEdit
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-11
**Status:** `[ ]`

Tambahkan di baris pertama `DgvData_CellEndEdit` (setelah `Try`):
```vb
If _sedangSetNilaiDariListBox Then Return
```

---

### TASK-17: Perbaiki Warna Hardcoded → ModuleTheme
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-12
**Status:** `[ ]`

1. `TxtNama_GotFocus`:
   ```vb
   ' Ganti:
   PanelCari.BackColor = Color.Yellow
   ' Dengan:
   PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_SearchFocusBg, ModuleTheme.D_SearchFocusBg)
   ```

2. `HighlightProblemRow`:
   ```vb
   ' Ganti:
   cell.Style.BackColor = Color.LightCoral
   ' Dengan:
   cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowPeringatan, ModuleTheme.D_DgvRowPeringatan)
   ```

---

### TASK-18: Perbaiki ProsesMergeBarisDuplikat
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-13
**Status:** `[ ]`

1. Hapus `SendKeys.Send("{down}")`
2. Tambah `Exit Sub` setelah `Call Hapusbaris()` untuk mencegah `ArgumentOutOfRangeException`
3. Ganti dengan `UpdateSemuaTotal()` saja (tidak perlu navigasi)

---

### TASK-19: Ganti AturTinggiListBarang untuk ListView
**File:** `2Trans/FormReturBeli.vb`
**Ref:** REQ-04
**Status:** `[ ]`

Ganti `AturTinggiListBarang` agar kompatibel dengan `ListView`:
- `ListView` tidak punya `ItemHeight` property langsung
- Gunakan tinggi tetap per item (misal 20px) atau hitung dari `LstBarang.Items.Count * 20`
- Batasi maksimum 300px

---

### TASK-20: Verifikasi Akhir
**File:** `2Trans/FormReturBeli.vb`
**Status:** `[ ]`

Jalankan PowerShell comparison:
```powershell
# Tidak ada fungsi bisnis yang hilang
$lama = Select-String -Path "2Trans/FormReturBeli.vb" -Pattern "^\s*(Private|Public|Protected)\s+(Sub|Function)\s+(\w+)" | ForEach-Object { $_.Matches[0].Groups[3].Value } | Sort-Object
# Bandingkan dengan daftar fungsi yang diharapkan ada
```

Verifikasi nama kolom DGV:
```powershell
# Semua Cells("...") harus pakai nama kolom yang benar
Select-String -Path "2Trans/FormReturBeli.vb" -Pattern '\.Cells\("([^"]+)"\)' | ForEach-Object { $_.Matches | ForEach-Object { $_.Groups[1].Value } } | Sort-Object -Unique
```

---

## Ringkasan Task

| Task | Deskripsi | Dikerjakan | Status |
|------|-----------|------------|--------|
| TASK-01 | Ganti LstBarang ke ListView di Designer | User | `[ ]` |
| TASK-02 | Ubah EditMode ke EditOnKeystrokeOrF2 | User | `[ ]` |
| TASK-03 | Tambah variabel baru | AI | `[ ]` |
| TASK-04 | Ganti SearchBarangByText untuk ListView | AI | `[ ]` |
| TASK-05 | Tambah PosisikanLstBarang* | AI | `[ ]` |
| TASK-06 | Ganti AmbilDataDariListBox | AI | `[ ]` |
| TASK-07 | Ganti LstBarang_KeyDown | AI | `[ ]` |
| TASK-08 | Ganti LstBarang_MouseClick | AI | `[ ]` |
| TASK-09 | Sesuaikan LstBarang_GotFocus/Enter | AI | `[ ]` |
| TASK-10 | Hapus LstBarang_DrawItem | AI | `[ ]` |
| TASK-11 | Tambah DgvNamaBarang_* handlers | AI | `[ ]` |
| TASK-12 | Ganti EditingControlShowing | AI | `[ ]` |
| TASK-13 | Tambah CellEnter handler | AI | `[ ]` |
| TASK-14 | Tambah SetupFocusToGrid | AI | `[ ]` |
| TASK-15 | Ganti pemanggil Fokuskepencarianbarang | AI | `[ ]` |
| TASK-16 | Tambah guard di CellEndEdit | AI | `[ ]` |
| TASK-17 | Perbaiki warna hardcoded | AI | `[ ]` |
| TASK-18 | Perbaiki ProsesMergeBarisDuplikat | AI | `[ ]` |
| TASK-19 | Ganti AturTinggiListBarang | AI | `[ ]` |
| TASK-20 | Verifikasi akhir | AI | `[ ]` |
