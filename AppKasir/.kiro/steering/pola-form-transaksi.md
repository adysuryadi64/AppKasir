# Pola Teknis Form Transaksi — AppKasir

> Pengetahuan permanen dari pengalaman nyata debug & migrasi form transaksi.
> Berlaku setiap kali menyentuh form transaksi manapun (FormJual, FormPembelian, FormReturJual, dll).

---

## ComboBox DGV — Dropdown Langsung Terbuka saat Sel Aktif

> Perilaku ini dicari bertahun-tahun: ComboBox di DGV bisa langsung dipilih pakai panah atas/bawah
> begitu sel aktif, tanpa perlu F2 atau klik mouse — meski kolom lain tidak `EditOnEnter`.

### Masalah

`EditOnEnter` global di DGV menyebabkan **semua kolom** langsung masuk edit mode saat sel aktif —
termasuk kolom teks seperti Nama/Qty yang seharusnya tidak berubah hanya karena Tab/Enter.

### Solusi

- Set `EditMode = EditOnKeystrokeOrF2` di designer (global)
- Tambahkan `CellEnter` handler — khusus kolom ComboBox (Satuan), langsung `BeginEdit` + `DroppedDown = True`

```vb
Private Sub DgvData_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles DgvData.CellEnter
    ' Kolom Satuan — langsung BeginEdit + buka dropdown agar panah atas/bawah
    ' bisa memilih satuan tanpa F2 atau klik, meski EditMode = EditOnKeystrokeOrF2
    If DgvData.Columns(e.ColumnIndex).Name = "Satuan" Then
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
    ' ... sisa logika CellEnter lainnya ...
End Sub
```

### Kenapa `BeginInvoke`?

`CellEnter` terpicu sebelum DGV selesai memproses perpindahan sel.
Memanggil `BeginEdit` langsung di dalam `CellEnter` menyebabkan `reentrant call` exception.
`BeginInvoke` menunda eksekusi ke message loop berikutnya — aman.

### Kenapa `DroppedDown = True`?

`BeginEdit` saja hanya mengaktifkan editing control (ComboBox dalam mode edit),
tapi dropdown belum terbuka. `DroppedDown = True` membuka dropdown secara programatik
sehingga panah atas/bawah langsung bisa memilih item.

### Form yang sudah menerapkan

| Form | Designer | CellEnter |
|---|---|---|
| `FormPembelian` | `EditOnKeystrokeOrF2` | ✅ Ada — kolom `Satuan` |
| `FormJual` | `EditOnKeystroke` (global) | ✅ Ada — kolom `Satuan` via `BeginInvoke` |

> FormJual pakai `EditOnKeystroke` — bukan `EditOnEnter`. Ini penting karena `EditOnEnter`
> menyebabkan DGV `BeginEdit` ulang setiap kali fokus kembali, yang mengganggu navigasi ListBox.

---

## Referensi File Utama

| File | Keterangan |
|---|---|
| `2Trans/FormJual.vb` | Referensi utama — implementasi terbaru yang sudah terbukti |
| `2Trans/FormPenjualan.vb` | Form lama — referensi fitur bisnis yang sudah terbukti akurat |
| `Modules/ModuleTheme.vb` | Warna dan pengaturan DGV — token semantik `L/D_DgvRow*` |
| `Modules/ModuleAngka.vb` | ParseDecimal, ParseInteger, FormatRupiah |
| `Modules/ModulHakAkses.vb` | SettingFokusOtomatis, SettingIzinkanSatuanBerbeda, SettingTampilInfoStok |
| `Database/16_sp_hlp_stok_ambil.sql` | SP ambil info stok mode tambah |
| `Database/17_sp_hlp_stok_ambil_edit.sql` | SP ambil info stok mode edit (stok efektif) |

---

## Aturan Warna DGV — WAJIB Konsisten

Gunakan token semantik dari `ModuleTheme`, **jangan hardcode warna**:

| Situasi | Token | Warna |
|---|---|---|
| Kolom stok = 0 (`CellFormatting`) | `L/D_DgvRowStokHabis` | Amber — informasi |
| Stok tidak cukup (`CekStok`) | `L/D_DgvRowPeringatan` | Amber — peringatan, user bisa ubah |
| Harga jual rugi (`Cekjualrugi`) | `L/D_DgvRowPeringatan` | Amber — peringatan, user bisa ubah |
| Race condition SP (`TekanSimpan`) | `L/D_DgvRowKonflik` | Amber — konflik multi-user |
| Error sistem (`DataError`) | `L/D_DgvRowError` | Merah — error tidak bisa dilanjutkan |

```vb
' ✅ BENAR
e.CellStyle.BackColor = ModuleTheme.C(ModuleTheme.L_Danger, ModuleTheme.D_Danger)
PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_SearchFocusBg, ModuleTheme.D_SearchFocusBg)

' ❌ DILARANG
e.CellStyle.BackColor = Color.Red
PanelCari.BackColor = Color.Yellow
```

---

## Fitur Stok via SP — Wajib di Semua Form Transaksi

### SP yang tersedia

| SP | Fungsi | Kapan dipakai |
|---|---|---|
| `sp_hlp_stok_ambil` | SELECT stok terkini — mode tambah | Tampil info stok saat barang dipilih, refresh, load draft |
| `sp_hlp_stok_ambil_edit` | SELECT stok efektif — mode edit (stok DB + qty di faktur lama) | Load edit — stok yang ditampilkan sudah memperhitungkan pengembalian |
| `sp_hlp_stok_validasi` | SELECT + FOR UPDATE — validasi real-time anti race condition | Tepat sebelum simpan ke DB |
| `sp_hlp_stok_hitung` | UPDATE stok dari komponen — recalculate penuh | Setelah transaksi tersimpan |

> **Mengapa perlu 2 SP untuk ambil stok?**
> Saat edit, stok di `tbl_barang` sudah dikurangi oleh faktur yang sedang diedit.
> Contoh: stok awal 100, faktur lama jual 10 → `STOK_TOKO = 90`.
> `sp_hlp_stok_ambil_edit` mengembalikan nilai akurat: `90 + 10 = 100`.

### Kapan `RefreshStokSemuaBaris` dipanggil

```vb
' Saat load mode edit — di akhir Editpenjualanheader() / Editpembelianheader() / dll
' PENTING: pastikan baris DGV sudah terisi dulu
RefreshStokSemuaBaris()

' Saat load draft — di akhir AmbilDataDitahan() setelah UpdateSemuaTotal()
RefreshStokSemuaBaris()
```

### Context menu klik kanan — wajib ada 2 menu item ini

```
"Refresh Stok Baris Ini"    → RefreshStokBaris(DgvData.CurrentCell.RowIndex)
"Refresh Stok Semua Baris"  → RefreshStokSemuaBaris()
```

---

## Pola `SetupFocusToGrid` yang Benar

> ⚠️ Ganti `"Kode"` dengan nama kolom ID/kode barang di designer form target.
> ⚠️ Index `1` = kolom NamaBarang di FormJual — sesuaikan jika berbeda.

```vb
Public Sub SetupFocusToGrid()
    If ModulHakAkses.SettingFokusOtomatis Then
        TxtNama.Focus()
        Return
    End If

    If DgvData.Rows.Count = 0 Then Return

    Dim targetRow As Integer = 0
    Dim lastFilledRow As Integer = -1

    ' Cari baris terakhir yang terisi
    For i As Integer = DgvData.Rows.Count - 1 To 0 Step -1
        If Not DgvData.Rows(i).IsNewRow Then
            Dim kodeVal = Convert.ToString(DgvData.Rows(i).Cells("Kode").Value).Trim()
            If Not String.IsNullOrEmpty(kodeVal) Then
                lastFilledRow = i
                Exit For
            End If
        End If
    Next

    If lastFilledRow >= 0 Then
        Dim foundEmptyRow As Boolean = False
        For i As Integer = lastFilledRow + 1 To DgvData.Rows.Count - 1
            If Not DgvData.Rows(i).IsNewRow Then
                Dim kodeVal = Convert.ToString(DgvData.Rows(i).Cells("Kode").Value).Trim()
                If String.IsNullOrEmpty(kodeVal) Then
                    targetRow = i
                    foundEmptyRow = True
                    Exit For
                End If
            End If
        Next

        If Not foundEmptyRow Then
            Dim isNewRowIdx As Integer = -1
            For i As Integer = lastFilledRow + 1 To DgvData.Rows.Count - 1
                If DgvData.Rows(i).IsNewRow Then
                    isNewRowIdx = i
                    Exit For
                End If
            Next
            ' ✅ Pakai IsNewRow jika ada — JANGAN Rows.Add() yang menyebabkan baris ekstra
            targetRow = If(isNewRowIdx >= 0, isNewRowIdx, DgvData.Rows.Add())
        End If
    Else
        targetRow = 0
    End If

    If targetRow < DgvData.Rows.Count Then
        ' ✅ Set CurrentCell SYNCHRONOUS — bukan di dalam BeginInvoke
        DgvData.CurrentCell = DgvData(1, targetRow)
        Me.ActiveControl = DgvData
        ' BeginEdit ditunda via nested BeginInvoke
        DgvData.BeginInvoke(New Action(Sub()
            DgvData.BeginInvoke(New Action(Sub()
                If DgvData.CurrentCell IsNot Nothing Then
                    DgvData.BeginEdit(True)
                    DgvData.EditingControl?.Focus()
                End If
            End Sub))
        End Sub))
    End If
End Sub
```

**Aturan kritis:**
- `CurrentCell = ...` dan `ActiveControl = DgvData` harus **synchronous** (sebelum `BeginInvoke`)
- Jangan panggil `SetupFocusToGrid` dari jalur barang tidak ditemukan di `CellEndEdit` — menyebabkan loop tak terbatas

---

## Pola `barisDiisi` yang Benar di `AmbilDataDariListBox`

```vb
' ❌ SALAH — CurrentCell.RowIndex bisa bergeser akibat BeginEdit pada IsNewRow
Dim barisDiisi As Integer = DgvData.CurrentCell.RowIndex

' ✅ BENAR — cari baris kosong non-IsNewRow pertama
Dim barisDiisi As Integer = DgvData.CurrentCell.RowIndex  ' fallback
For i As Integer = 0 To DgvData.Rows.Count - 1
    If Not DgvData.Rows(i).IsNewRow Then
        ' Cek DUA kolom — kode DAN nama — agar baris yang sedang diisi tidak dianggap kosong
        Dim kodeVal = Convert.ToString(DgvData.Rows(i).Cells("KolomKode").Value).Trim()
        Dim namaVal = Convert.ToString(DgvData.Rows(i).Cells("KolomNama").Value).Trim()
        If String.IsNullOrEmpty(kodeVal) AndAlso String.IsNullOrEmpty(namaVal) Then
            barisDiisi = i
            Exit For
        End If
    End If
Next
```

---

## Scope Flag `_sedangSetNilaiDariListBox`

Flag harus aktif selama **seluruh proses** di `AmbilDataDariListBox`:

```vb
' ❌ SALAH — flag dimatikan terlalu cepat
_sedangSetNilaiDariListBox = True
DgvData.EndEdit(True)
DgvData.CurrentCell = Nothing
_sedangSetNilaiDariListBox = False  ' ← CellEndEdit masih bisa terpicu setelah ini
IsiBarangKeRow(...)

' ✅ BENAR — flag aktif sampai IsiBarangKeRow selesai
_sedangSetNilaiDariListBox = True
DgvData.EndEdit(True)
DgvData.CurrentCell = Nothing
' ... cek duplikat ...
IsiBarangKeRow(...)
_sedangSetNilaiDariListBox = False  ' ← baru dimatikan setelah semua selesai
```

---

## Inkonsistensi Nama Kolom DGV — Case-Sensitive

> WinForms DGV **case-sensitive**. `Cells("qty")` dan `Cells("QTY")` adalah dua hal berbeda.
> Nilai ditulis ke kolom yang tidak ada → cell tetap null → kalkulasi hasilkan 0 tanpa error.

### Script verifikasi nama kolom (jalankan setelah migrasi)

```powershell
# Ekstrak nama kolom resmi dari designer
$designerContent = Get-Content "2Trans/FormNama.Designer.vb" -Raw
[regex]::Matches($designerContent, '\.HeaderText = "([^"]+)"[\s\S]{1,200}?\.Name = "([^"]+)"') | ForEach-Object {
    "$($_.Groups[1].Value) → $($_.Groups[2].Value)"
}

# Bandingkan dengan semua Cells("...") di kode
$validColumns = @("Kode","NamaBarang","HargaBeli","QTY","Satuan","Isi","Totalhargabeli","Harga","QtySat","DiskonPersen","DiskonRp","TotalDiskon","TotalHarga","StokToko","StokGudang","Stok","SerialNumber")
$lines = Get-Content "2Trans/FormNama.vb"
for ($i = 0; $i -lt $lines.Count; $i++) {
    $lineMatches = [regex]::Matches($lines[$i], '\.Cells\("([^"]+)"\)')
    foreach ($m in $lineMatches) {
        $name = $m.Groups[1].Value
        if (-not ($validColumns -ccontains $name)) {
            $caseMatch = ($validColumns | Where-Object { $_ -ieq $name }) -join ","
            Write-Host "Baris $($i+1): '$name' → mungkin '$caseMatch'"
        }
    }
}
```

---

## Masalah yang Pernah Terjadi & Solusinya

| Masalah | Penyebab | Solusi |
|---|---|---|
| ListBox hilang saat tekan panah bawah dari DGV | DGV `EditOnKeystroke` merebut fokus kembali setelah `LstBarang.Focus()` dipanggil — `BeginEdit` ulang terpicu | `EndEdit()` dulu (dengan guard `_sedangSetNilaiDariListBox`) sebelum `Focus()`, keduanya di dalam nested `BeginInvoke` |
| `LstBarang.Focus()` butuh dua kali panah bawah | `BeginInvoke` satu lapis dieksekusi sebelum `EditingControlShowing` selesai — DGV masih `BeginEdit` | Gunakan nested `BeginInvoke` (2 lapis) agar `Focus()` dipanggil setelah DGV benar-benar selesai |
| `TextChanged` teks kosong menutup ListBox saat pindah ke ListBox | DGV `BeginEdit` ulang → TextBox baru kosong → `TextChanged` terpicu | Guard di `TextChanged`: jika `LstBarang.Visible = True` dan teks kosong → skip sembunyikan |
| `EditingControlShowing` re-attach handler ke TextBox kosong saat transisi | DGV `BeginEdit` ulang saat `LstBarang.Focus()` belum berhasil | Guard di `EditingControlShowing`: jika `_sedangPindahKeLstBarang = True` → skip re-attach |
| `CellEndEdit` memproses keyword saat `EndEdit()` dipanggil untuk pindah ke ListBox | `EndEdit()` memicu `CellEndEdit` dengan teks keyword yang masih ada di sel | Set `_sedangSetNilaiDariListBox = True` sebelum `EndEdit()`, reset setelahnya |
| Duplikat qty saat tambah barang baru | `CellEndEdit` terpicu oleh `DgvData.EndEdit(True)` di `AmbilDataDariListBox` | Guard `_sedangSetNilaiDariListBox` di awal `CellEndEdit`, scope flag diperluas |
| Fokus tidak pindah ke baris kosong berikutnya | `SetupFocusToGrid` tidak menemukan baris kosong karena baris baru adalah `IsNewRow` | Pakai `IsNewRow` sebagai `targetRow` — jangan `Rows.Add()` |
| Barcode di DGV tidak menambah baris baru | Scanning barcode saat `CurrentCell` sedang edit mode menyebabkan konflik state | `EndEdit(True)` + `CurrentCell = Nothing` sebelum lookup, set `CurrentCell` ke `IsNewRow` TANPA `BeginEdit` setelah isi |
| Barcode tidak ditemukan menyisakan baris kosong | Input salah/barcode tidak terdaftar meninggalkan baris dengan NamaBarang kosong | Logic `isScanLikeInput` di `CellEndEdit`: jika lookup gagal dan input murni numerik panjang, hapus baris kosong dan refocus ke `IsNewRow` |
| `ArgumentOutOfRangeException` saat duplikat ditemukan | Loop duplikat melanjutkan iterasi setelah `RemoveAt`, RowCount berubah | Tambahkan `Exit Sub` setelah `SetupFocusToGrid` di jalur duplikat |
| Data duplikat tersisa di baris IsNewRow | Baris `IsNewRow` tidak dihapus karena `IsNewRow` check | Jika baris duplikat adalah `IsNewRow`, kosongkan semua cell alih-alih menghapus baris |
| `CellEndEdit` memproses string kosong di baris IsNewRow | `IsNot Nothing AndAlso Not IsDBNull` tidak cek string kosong | Gunakan `Not String.IsNullOrEmpty()` |
| Duplikat dengan satuan berbeda tidak di-merge | Ada cek satuan berbeda yang mencegah merge | Hapus cek satuan berbeda — duplikat tetap di-merge (sesuai FormJual) |
| Format `qty*X*namaBarang` salah diinterpretasi | Middle part diperlakukan sebagai "harga" bukan "level" | Middle part adalah "level" untuk menentukan satuan (kecil/sedang/besar) |
| Barcode tidak menentukan level satuan dengan benar | `IsiBarangKeRow` tidak handle barcode-based level | Tambah query untuk barcode dan logika Select Case untuk level |
| `reentrant call to BeginEdit` | `CurrentCell = ...` di dalam `BeginInvoke` memicu `OnEnter → BeginEditInternal` saat DGV masih editing | Pindahkan `CurrentCell = ...` ke synchronous, sebelum `BeginInvoke` |
| Loop tak terbatas + `NullReferenceException` | Jalur barang tidak ditemukan memanggil `SetupFocusToGrid` → `CellEndEdit` terpicu lagi | Hapus `SetupFocusToGrid` dari jalur barang tidak ditemukan |
| `barisDiisi` selalu 1 | `BeginEdit` pada `IsNewRow` geser `CurrentCell` ke index 1 | Cari baris kosong non-IsNewRow pertama, bukan pakai `CurrentCell.RowIndex` |
| 2 baris ekstra saat input barang pertama | `SetupFocusToGrid` memanggil `Rows.Add()` padahal `IsNewRow` sudah ada | Cek `IsNewRow` dulu sebelum `Rows.Add()` |
| Harga jual tidak terisi dari jalur DGV | `IsiBarangKeRow` tidak query kolom harga jual | Perluas query untuk ambil semua kolom harga + satuan Umum dan Partai |
| Jenis pelanggan Partai tidak dipakai dari jalur DGV | `IsiBarangKeRow` hanya query satuan Umum | Tambahkan logika `isPartai = LblJenisPl.Text = "Partai"` |

---

## ListBox Pencarian Barang — Pola yang Benar (FormJual)

> **Mengapa ListBox, bukan ListView?**
> ListView memerlukan banyak flag state (`_lstBarangBaruMasuk`, `_lstBarangSelectedIndex`, dll)
> untuk menangani navigasi keyboard yang tidak stabil. ListBox jauh lebih sederhana dan stabil.

### Flag yang Diperlukan

```vb
Private _dgvEditingTextBox As TextBox = Nothing       ' TextBox editing control DGV aktif
Private _sedangPindahKeLstBarang As Boolean = False   ' Guard CellLeave saat transisi fokus
Private _teksSebelumPindahKeLstBarang As String = ""  ' Simpan teks untuk restore saat Up/Escape
Private _listBoxDibukaDiRow As Integer = -1           ' Baris DGV saat ListBox dibuka
Private _listBoxDibukaDiCol As Integer = -1           ' Kolom DGV saat ListBox dibuka
Private _konteksLstBarang As String = "TXTNAMA"       ' "TXTNAMA" atau "DGV"
Private _sedangSetNilaiDariListBox As Boolean = False ' Guard CellEndEdit saat isi programatik
```

### Pola Navigasi Keyboard — Prioritas Kasir

```
1. Ketik di sel NamaBarang → ListBox muncul
2. Enter → langsung ambil item pertama (TERCEPAT)
3. Panah Bawah → pindah ke ListBox, navigasi Up/Down
4. Enter di ListBox → ambil item yang di-highlight
5. Panah Atas di item pertama → kembali ke TextBox + restore teks
6. Escape → tutup ListBox
7. Klik mouse → ambil item (optional)
```

### Pola `ProcessCmdKey` untuk Down Arrow

```vb
Case Keys.Down
    If LstBarang.Focused Then
        Return MyBase.ProcessCmdKey(msg, keyData)  ' biarkan ListBox navigasi sendiri
    End If
    ' Simpan teks sebelum pindah
    If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing Then
        _teksSebelumPindahKeLstBarang = _dgvEditingTextBox.Text
    Else
        _teksSebelumPindahKeLstBarang = TxtNama.Text
    End If
    _sedangPindahKeLstBarang = True
    If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
    ' KRITIS: nested BeginInvoke + EndEdit agar DGV tidak merebut fokus kembali
    ' Lapis 1: tunggu CellLeave.BeginInvoke selesai
    ' Lapis 2: EndEdit (guard _sedangSetNilaiDariListBox) → Focus()
    Me.BeginInvoke(New Action(Sub()
        Me.BeginInvoke(New Action(Sub()
            If LstBarang.Visible Then
                _sedangSetNilaiDariListBox = True
                DgvDataTransaksi.EndEdit()
                _sedangSetNilaiDariListBox = False
                LstBarang.Focus()
            End If
            _sedangPindahKeLstBarang = False
        End Sub))
    End Sub))
    Return True
```

**Kenapa nested BeginInvoke + EndEdit?**
- DGV `EditOnKeystroke` merebut fokus kembali setelah `LstBarang.Focus()` dipanggil
- `EndEdit()` mengeluarkan DGV dari edit mode → DGV tidak bisa merebut fokus lagi
- `_sedangSetNilaiDariListBox = True` mencegah `CellEndEdit` memproses keyword sebagai nama barang
- Nested `BeginInvoke` memastikan `EndEdit()` dipanggil setelah `EditingControlShowing` selesai

### Pola `EditingControlShowing` — Guard saat Transisi

```vb
Private Sub DgvData_EditingControlShowing(...) Handles DgvData.EditingControlShowing
    If DgvData.CurrentCell.ColumnIndex = 1 AndAlso DgvData.Columns(1).HeaderText = "Nama Barang" Then
        ' KRITIS: skip re-attach saat sedang pindah ke ListBox
        ' DGV BeginEdit ulang karena fokus kembali — biarkan handler lama tetap aktif
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

### Pola `DgvNamaBarang_TextChanged` — Guard Teks Kosong

```vb
If String.IsNullOrEmpty(currentText) Then
    ' KRITIS: jangan sembunyikan jika ListBox masih visible
    ' Teks kosong karena DGV BeginEdit ulang (bukan user hapus teks)
    If _sedangPindahKeLstBarang OrElse LstBarang.Focused OrElse LstBarang.Visible Then
        Return
    End If
    LstBarang.Items.Clear()
    LstBarang.Visible = False
    ResetBarcodeDetection()
    Return
End If
```

### Pola `DgvData_CellLeave` — Guard dengan BeginInvoke

```vb
Private Sub DgvData_CellLeave(...) Handles DgvData.CellLeave
    If Not Me.IsHandleCreated Then Return
    Me.BeginInvoke(New Action(Sub()
        If LstBarang.Visible Then
            If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then Return
            ' Jangan tutup jika masih di baris yang sama dengan saat ListBox dibuka
            If _listBoxDibukaDiRow >= 0 AndAlso
               DgvData.CurrentCell IsNot Nothing AndAlso
               DgvData.CurrentCell.RowIndex = _listBoxDibukaDiRow AndAlso
               DgvData.CurrentCell.ColumnIndex = _listBoxDibukaDiCol Then Return
            LstBarang.Visible = False
            LstBarang.Items.Clear()
            _listBoxDibukaDiRow = -1
            _listBoxDibukaDiCol = -1
        End If
    End Sub))
End Sub
```

### Format String ListBox — Nama + Stok

```vb
' Isi ListBox dengan format string (bukan ListViewItem)
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

' Parse nama dari format string saat ambil
Dim namayangdiambil As String = selectedValue
If selectedValue.Contains("|") Then
    namayangdiambil = selectedValue.Split({"|"c}, StringSplitOptions.RemoveEmptyEntries)(0).Trim()
End If
```

---

## Barcode di DGV — Pola yang Benar

### Skenario Barcode Ditemukan

1. `EndEdit(True)` + `CurrentCell = Nothing` — stabilkan Grid
2. Loop cari baris kosong (kode DAN nama kosong)
3. `IsiBarangKeRow` dengan index baris yang ditemukan
4. Set `CurrentCell` ke `IsNewRow` — **TANPA `BeginEdit`**
5. Scanner berikutnya otomatis trigger `EditingControlShowing` saat karakter pertama masuk

### Skenario Barcode TIDAK Ditemukan

1. Hapus baris sampah jika bukan `IsNewRow` dan kode masih kosong
2. Cari `IsNewRow`, set sebagai `CurrentCell`
3. **WAJIB** panggil `BeginEdit` via `BeginInvoke` — agar scanner berikutnya langsung masuk ke sel NamaBarang

```vb
If isScanLikeInput Then
    If String.IsNullOrEmpty(kodeNow) Then
        If Not DgvDataTransaksi.Rows(e.RowIndex).IsNewRow Then
            DgvDataTransaksi.Rows.RemoveAt(e.RowIndex)
        End If
        For i As Integer = 0 To DgvDataTransaksi.Rows.Count - 1
            If DgvDataTransaksi.Rows(i).IsNewRow Then
                DgvDataTransaksi.CurrentCell = DgvDataTransaksi(1, i)
                DgvDataTransaksi.BeginInvoke(New Action(Sub()
                    DgvDataTransaksi.BeginEdit(True)
                End Sub))
                Exit For
            End If
        Next
    End If
End If
```

### Gunakan `barcodeTimer` yang sudah ada — jangan buat mekanisme baru

Feed karakter dari `DgvNamaBarang_TextChanged` ke `barcodeChars` yang sama, biarkan `BarcodeTimer_Tick` yang memproses. Tambahkan cabang `_konteksLstBarang = "DGV"` di `BarcodeTimer_Tick`.

### Qty dan Level dari jalur DGV

```vb
' Di DgvNamaBarang_TextChanged — simpan ke TxtQty dan TxtLevelSat
If currentText.Contains("*") Then
    Dim parts = currentText.Split("*"c)
    Dim qty As Decimal = ModuleAngka.ParseDecimal(parts(0).Trim())
    If qty > 0 Then TxtQty.Text = qty.ToString()
    ' Format qty*level*nama — middle part adalah level satuan
    If parts.Length >= 3 Then
        Dim lvl As Integer = 0
        If Integer.TryParse(parts(1).Trim(), lvl) AndAlso lvl >= 1 AndAlso lvl <= 3 Then
            TxtLevelSat.Text = lvl.ToString()
        End If
    End If
    keyword = parts(parts.Length - 1).Trim()
End If

' Di AmbilDataDariListBox jalur DGV — baca dari TxtQty (bukan _dgvEditingTextBox.Text)
Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
If qtyValue <= 0 Then qtyValue = 1D
```

> `TxtQty` dan `TxtLevelSat` di-reset otomatis oleh `KosongTxtboxcari()` — tidak perlu reset manual.

---

## Setting `ModulHakAkses` — Pemetaan per Jenis Form

> Semua setting dibaca dari `ModulHakAkses` (cache dari `tbl_general_setting`).
> Jangan hardcode nilai — selalu baca via property `ModulHakAkses.SettingXxx`.

### Setting Global (berlaku di semua form transaksi)

| Setting | Dipakai di | Fungsi |
|---|---|---|
| `SettingIzinkanTanggalLampau` | Semua form | Kunci/buka DTP tanggal; validasi sebelum simpan |
| `SettingIzinkanBarangMinus` | Semua form | Izinkan stok jadi negatif; validasi di `TekanBayar/Simpan` |
| `SettingFokusOtomatis` | Semua form | `True` = fokus ke TxtNama; `False` = fokus ke DGV (Mode 2) |
| `SettingIzinkanSatuanBerbeda` | Semua form | Cek duplikat barang — merge qty jika False |
| `SettingLangsungIsiNominalTotal` | Semua form | Isi otomatis nominal bayar saat panel bayar dibuka |
| `SettingTampilInfoStok` | Semua form | Tampilkan kolom stok di DGV + info stok di ListBox |
| `SettingSembunyikanPencarianAtas` | Semua form | Sembunyikan PanelCari di atas DGV |

### Setting Khusus Penjualan (FormJual)

| Setting | Dipakai di | Fungsi |
|---|---|---|
| `SettingIzinkanUbahHargaJual` | FormJual | Kolom Harga ReadOnly jika False |
| `SettingIzinkanJualRugi` | FormJual | Validasi `Cekjualrugi()` sebelum bayar |
| `SettingIzinkanNominalJualNol` | FormJual | Validasi total > 0 sebelum bayar |
| `SettingHargaJualOtomatisUpdateMaster` | FormJual | Update harga master saat harga diubah di DGV |
| `SettingIzinkanDiskonItem` | FormJual | Tampilkan/sembunyikan kolom DiskonPersen, DiskonRp, TotalDiskon |
| `SettingAutoLevelSatuan` | FormJual | Auto ganti satuan berdasarkan qty; nonaktifkan dropdown Satuan |
| `SettingBatasSatuanSedang` | FormJual | Threshold qty untuk naik ke satuan sedang |
| `SettingBatasSatuanBesar` | FormJual | Threshold qty untuk naik ke satuan besar |

### Setting Khusus Pembelian (FormPembelian)

| Setting | Dipakai di | Fungsi |
|---|---|---|
| `SettingIzinkanUbahHargaBeli` | FormPembelian | Kolom HargaBeli ReadOnly jika False |
| `SettingBeliOtomatisUpdateHargaJual` | FormPembelian | Tampilkan dialog update harga jual saat harga beli berubah |
| `SettingMetodeUpdateHargaBeli` | FormPembelian | `"Harga Terbaru"` / `"Metode Average (Rata - Rata)"` / `"Tidak Ada"` |
| `SettingAverageHargaBerdasarkanStok` | FormPembelian | `"Toko"` / `"Gudang"` / `"Toko dan Gudang"` — basis kalkulasi average |
| `SettingIzinkanBeliTanpaSupplier` | FormPembelian | Validasi supplier sebelum bayar |
| `SettingIzinkanNominalBeliNol` | FormPembelian | Validasi total > 0 sebelum bayar |
| `SettingIzinkanBeliRugi` | FormPembelian | Validasi `Cekjualrugi()` sebelum bayar |

### Setting Khusus Retur

| Setting | Dipakai di | Fungsi |
|---|---|---|
| `SettingWajibAlasanReturJual` | FormReturPenjualan | Validasi field alasan tidak boleh kosong |
| `SettingWajibAlasanReturBeli` | FormReturBeli | Validasi field alasan tidak boleh kosong |

### Pola Implementasi di Form Load

```vb
' ── Setting kolom DGV ────────────────────────────────────────────────────────
' Penjualan
If Not ModulHakAkses.SettingIzinkanUbahHargaJual Then DgvData.Columns("Harga").ReadOnly = True
If Not ModulHakAkses.SettingIzinkanDiskonItem Then
    DgvData.Columns("DiskonPersen").Visible = False
    DgvData.Columns("DiskonRp").Visible = False
    DgvData.Columns("TotalDiskon").Visible = False
End If

' Pembelian
If Not ModulHakAkses.SettingIzinkanUbahHargaBeli Then DgvData.Columns("Hargabeli").ReadOnly = True

' Semua form — stok dan panel pencarian
If ModulHakAkses.SettingTampilInfoStok Then
    DgvData.Columns("StokToko").Visible = True
    DgvData.Columns("StokGudang").Visible = True
End If
If ModulHakAkses.SettingSembunyikanPencarianAtas Then PanelCari.Visible = False

' ── Setting tanggal ──────────────────────────────────────────────────────────
DtpTanggal.Enabled = ModulHakAkses.SettingIzinkanTanggalLampau
' Atau pakai helper:
ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)  ' reset + kunci jika tidak izinkan lampau
```

### Pola Validasi Sebelum Simpan

```vb
' Validasi tanggal (semua form)
If Not ModulHakAkses.ValidasiTanggalTransaksi(DtpTanggal.Value) Then
    ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)
    Exit Sub
End If

' Validasi penjualan
If Not ModulHakAkses.SettingIzinkanNominalJualNol AndAlso totalJual = 0 Then Exit Sub
If Not ModulHakAkses.SettingIzinkanJualRugi AndAlso Cekjualrugi() Then Exit Sub
If Not ModulHakAkses.SettingIzinkanBarangMinus AndAlso CekStok() Then Exit Sub

' Validasi pembelian
If String.IsNullOrEmpty(TxtNamaSupplier.Text) AndAlso Not ModulHakAkses.SettingIzinkanBeliTanpaSupplier Then Exit Sub
If grandTotal = 0 AndAlso Not ModulHakAkses.SettingIzinkanNominalBeliNol Then Exit Sub

' Validasi retur
If ModulHakAkses.SettingWajibAlasanReturJual AndAlso String.IsNullOrWhiteSpace(RTBAlasan.Text) Then
    MessageBox.Show("Alasan retur wajib diisi.", "Peringatan", ...)
    Exit Sub
End If
```

---

## Catatan Khusus per Form

### FormJual

- Setting yang **wajib** diimplementasikan: semua setting Global + semua setting Penjualan
- `SettingAutoLevelSatuan` punya **dua jalur**: `IsiBarangKeRow` (saat pilih barang) dan `CellEndEdit` kolom QTY (saat edit qty langsung)
- `SettingIzinkanDiskonItem` mengontrol 3 kolom sekaligus: `DiskonPersen`, `DiskonRp`, `TotalDiskon`
- Jenis pelanggan (`LblJenisPl.Text = "Partai"`) mempengaruhi harga yang diambil di `IsiBarangKeRow`

### FormPembelian

- Setting yang **wajib** diimplementasikan: semua setting Global + semua setting Pembelian
- `SettingMetodeUpdateHargaBeli` dan `SettingAverageHargaBerdasarkanStok` dipakai bersama — selalu baca keduanya
- Tidak ada jenis pelanggan (Umum/Partai) — hapus logika partai dari `IsiBarangKeRow`
- `SettingBeliOtomatisUpdateHargaJual` memunculkan dialog konfirmasi update harga jual — perlu form `FormEditHargaJual` atau sejenisnya

### FormReturPenjualan

- Setting yang **wajib**: `SettingIzinkanTanggalLampau`, `SettingWajibAlasanReturJual`
- Setting yang **tidak relevan**: `SettingAutoLevelSatuan`, `SettingIzinkanDiskonItem`, `SettingIzinkanJualRugi`
- Ada dua mode: **mode normal** (terikat nota penjualan) dan **mode bebas** (tanpa nota)
- Mode normal: pencarian barang dibatasi dari `penjualan_detail` nota yang dipilih — `IsiBarangKeRow` query ke `penjualan_detail`, bukan `tbl_barang`
- Mode bebas: pencarian barang dari `tbl_barang` seperti biasa

### FormReturBeli

- Setting yang **wajib**: `SettingIzinkanTanggalLampau`, `SettingWajibAlasanReturBeli`
- Mirip FormReturPenjualan tapi terikat nota pembelian
- Tidak ada jenis pelanggan — tidak ada logika Umum/Partai

### FormTransferCabang

- Setting yang **wajib**: `SettingIzinkanTanggalLampau`, `SettingTampilInfoStok`, `SettingFokusOtomatis`
- Tidak ada pembayaran — tidak perlu setting nominal, rugi, atau supplier
- Tidak ada jenis pelanggan
- Stok yang ditampilkan adalah stok lokasi asal (bukan tujuan)
