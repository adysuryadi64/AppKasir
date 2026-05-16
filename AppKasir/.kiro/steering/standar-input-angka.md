# Standar Input & Penanganan Angka — AppKasir

## Prinsip Utama

Pisahkan tiga lapisan secara ketat:

| Lapisan | Tujuan | Format |
|---|---|---|
| **Data** | Kalkulasi & simpan ke DB | Decimal murni, tidak pernah string berformat |
| **Display** | Tampilkan ke user | String berformat (`#,0.##`) |
| **Input** | Terima dari user | String bebas → wajib parse dulu |

---

## 1. Aturan TextBox

### TextBox untuk DISPLAY saja (tidak dibaca logika)
```vb
' Isi dengan format ribuan — hanya untuk mata user
TxtTotalRupiah.Text = grandTotal.ToString("#,0.##", cultureIndonesia)
LblHarga.Text = "Rp. " & harga.ToString("N0", cultureIndonesia)
```

### TextBox untuk INPUT (dibaca logika)
```vb
' JANGAN format ribuan saat mengisi
TxtHarga.Text = harga.ToString()          ' ✅ plain: "1500000"
TxtQty.Text   = qty.ToString()            ' ✅ plain: "1.5"

' JANGAN langsung Convert saat membaca
' ❌ Convert.ToDecimal(TxtHarga.Text)     → crash jika ada titik ribuan
' ❌ Val(TxtHarga.Text)                   → berhenti di titik pertama
' ✅ Selalu lewat ParseDecimal
Dim harga As Decimal = ParseDecimal(TxtHarga.Text)
```

### TextBox yang SEKALIGUS display dan input
**Pendekatan yang direkomendasikan — Simpan nilai di variabel Private, TextBox hanya display. Saat simpan pakai variabel.**
```vb
' Variabel adalah sumber kebenaran
Private _harga As Decimal = 0D

' TextBox diisi plain (tanpa format) agar bisa dibaca kembali
TxtHarga.Text = _harga.ToString()

' Label terpisah untuk tampilan berformat (jika ada ruang)
LblHargaDisplay.Text = "Rp. " & _harga.ToString("#,0.##", cultureIndonesia)
```

---

## 2. Aturan DataGridView Cell

### BENAR — isi cell dengan Decimal
```vb
' ✅ Isi dengan nilai Decimal murni
row.Cells("HARGA").Value    = harga           ' Decimal
row.Cells("QTY").Value      = qty             ' Decimal
row.Cells("TOTAL").Value    = qty * harga     ' Decimal

' Format tampilan diatur di kolom — panggil ModuleGridFormat.TerapkanFormatAngka(DgvData)
DgvData.Columns("HARGA").DefaultCellStyle.Format    = "#,0.##"
DgvData.Columns("QTY").DefaultCellStyle.Format      = "#,0.##"
DgvData.Columns("TOTAL").DefaultCellStyle.Format    = "#,0.##"
DgvData.Columns("TOTAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
```

### SALAH — isi cell dengan string berformat
```vb
' ❌ JANGAN — akan crash saat dibaca dengan Convert.ToDecimal
row.Cells("HARGA").Value = harga.ToString("#,0.##")
row.Cells("TOTAL").Value = (qty * harga).ToString("N0")
```

### Membaca nilai dari cell
```vb
' ✅ Cell berisi Decimal — langsung Convert, guard Nothing/DBNull
Dim harga As Decimal = If(row.Cells("HARGA").Value Is Nothing OrElse
                          IsDBNull(row.Cells("HARGA").Value), 0D,
                          Convert.ToDecimal(row.Cells("HARGA").Value))

' ✅ Atau pakai ModuleAngka.ParseDecimal jika tidak yakin isi cell
Dim harga As Decimal = ModuleAngka.ParseDecimal(row.Cells("HARGA").Value)
```

### Cell yang bisa diedit user (harga, diskon, dll)
```vb
' Saat CellEndEdit — normalisasi dulu sebelum simpan ke cell
Private Sub DgvData_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) _
        Handles DgvData.CellEndEdit
    If DgvData.Columns(e.ColumnIndex).Name = "HARGA" Then
        Dim harga As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(e.RowIndex).Cells("HARGA").Value)
        If harga < 0D Then harga = 0D
        ' Simpan kembali sebagai Decimal — BUKAN string berformat
        DgvData.Rows(e.RowIndex).Cells("HARGA").Value = harga
        HitungBaris(e.RowIndex)
    End If
End Sub
```

---

## 3. Module Global — ModuleAngka

**Buat satu module global** agar `ParseDecimal`, `ParseInteger`, dan setup format DGV bisa dipanggil dari semua form tanpa duplikasi kode.

```vb
' File: Modules/ModuleAngka.vb
Module ModuleAngka

    ''' <summary>
    ''' Parse angka dari berbagai format input user.
    ''' Handle: "1500000", "1.500.000", "1,500,000", "1500000.50",
    '''         "1.500,50", "1,5", "1.5", nilai negatif "-500"
    ''' Selalu kembalikan 0 jika gagal parse, tidak pernah throw exception.
    ''' </summary>
    Public Function ParseDecimal(ByVal value As Object) As Decimal
        If value Is Nothing OrElse IsDBNull(value) Then Return 0D
        Dim s As String = value.ToString().Trim()
        If String.IsNullOrEmpty(s) Then Return 0D

        ' Tangani tanda negatif
        Dim isNegative As Boolean = s.StartsWith("-")
        If isNegative Then s = s.Substring(1).Trim()

        Dim hasComma As Boolean = s.Contains(",")
        Dim hasDot   As Boolean = s.Contains(".")
        Dim normalized As String

        If hasComma AndAlso hasDot Then
            ' Ada keduanya — tentukan mana ribuan, mana desimal
            ' Pemisah ribuan selalu di kiri, desimal di kanan
            If s.IndexOf(".") < s.IndexOf(",") Then
                ' Format Indonesia: "1.500,50" → hapus titik, koma jadi titik
                normalized = s.Replace(".", "").Replace(",", ".")
            Else
                ' Format US: "1,500.50" → hapus koma
                normalized = s.Replace(",", "")
            End If
        ElseIf hasComma AndAlso Not hasDot Then
            ' Hanya koma: "1,5" (desimal) atau "1,500" (ribuan)
            Dim parts = s.Split(","c)
            If parts.Length = 2 AndAlso parts(1).Length <= 2 Then
                normalized = s.Replace(",", ".")   ' desimal
            Else
                normalized = s.Replace(",", "")    ' ribuan
            End If
        ElseIf hasDot AndAlso Not hasComma Then
            ' Hanya titik: "1.5" (desimal) atau "1.500" (ribuan)
            Dim parts = s.Split("."c)
            If parts.Length = 2 AndAlso parts(1).Length <= 2 Then
                normalized = s                     ' desimal, biarkan
            Else
                normalized = s.Replace(".", "")    ' ribuan, hapus titik
            End If
        Else
            normalized = s                         ' angka bulat biasa
        End If

        Dim result As Decimal = 0D
        Decimal.TryParse(normalized, Globalization.NumberStyles.Any,
                         Globalization.CultureInfo.InvariantCulture, result)
        Return If(isNegative, -result, result)
    End Function

    ''' <summary>
    ''' Parse Integer — untuk field yang tidak boleh desimal (isi satuan, jumlah item, dll).
    ''' Selalu kembalikan defaultValue jika gagal atau hasil <= 0.
    ''' </summary>
    Public Function ParseInteger(ByVal value As Object,
                                 Optional defaultValue As Integer = 0) As Integer
        Dim d As Decimal = ParseDecimal(value)
        Dim i As Integer = CInt(Math.Truncate(d))
        Return If(i > 0, i, defaultValue)
    End Function

    ''' <summary>
    ''' Terapkan format angka ke kolom-kolom DGV secara seragam.
    ''' Panggil sekali saat SetupGrid() atau form Load.
    ''' </summary>
    Public Sub TerapkanFormatKolomAngka(dgv As DataGridView,
                                        ParamArray namaKolom() As String)
        For Each nama As String In namaKolom
            If dgv.Columns.Contains(nama) Then
                dgv.Columns(nama).DefaultCellStyle.Format    = "#,0.##"
                dgv.Columns(nama).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                dgv.Columns(nama).DefaultCellStyle.FormatProvider =
                    Globalization.CultureInfo.GetCultureInfo("id-ID")
            End If
        Next
    End Sub

    ''' <summary>
    ''' Format Decimal untuk display Indonesia.
    ''' </summary>
    Public Function FormatRupiah(value As Decimal) As String
        Return value.ToString("#,0.##", Globalization.CultureInfo.GetCultureInfo("id-ID"))
    End Function

    Public Function FormatRupiahBulat(value As Decimal) As String
        Return value.ToString("N0", Globalization.CultureInfo.GetCultureInfo("id-ID"))
    End Function

End Module
```

### Cara pakai di form
```vb
' Setup format kolom DGV — cukup sekali saat load
ModuleAngka.TerapkanFormatKolomAngka(DgvData, "HARGA", "QTY", "TOTAL", "DISKON")

' Parse input user
Dim qty   As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
Dim harga As Decimal = ModuleAngka.ParseDecimal(TxtHarga.Text)

' Parse isi satuan (Integer)
Dim isi As Integer = ModuleAngka.ParseInteger(TxtIsi.Text, defaultValue:=1)

' Format display
LblTotal.Text = "Rp. " & ModuleAngka.FormatRupiah(qty * harga)
```

---

## 4. Aturan Input Qty — Support Pecahan

Qty boleh berupa bilangan desimal (contoh: 1,5 kg atau 1.5 kg).

### Validasi input KeyPress untuk field Qty (desimal diizinkan)
```vb
Private Sub TxtQty_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtQty.KeyPress
    ' Izinkan: angka, backspace, titik, koma (keduanya sebagai desimal)
    If Not (Char.IsDigit(e.KeyChar) OrElse
            e.KeyChar = ControlChars.Back OrElse
            e.KeyChar = "."c OrElse
            e.KeyChar = ","c) Then
        e.Handled = True
        Return
    End If
    ' Cegah lebih dari satu pemisah desimal
    Dim current As String = DirectCast(sender, TextBox).Text
    If (e.KeyChar = "."c OrElse e.KeyChar = ","c) AndAlso
       (current.Contains(".") OrElse current.Contains(",")) Then
        e.Handled = True
    End If
End Sub
```

### Validasi input KeyPress untuk field yang hanya boleh Integer (isi satuan, dll)
```vb
Private Sub TxtIsi_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtIsi.KeyPress
    ' Hanya angka dan backspace
    If Not (Char.IsDigit(e.KeyChar) OrElse e.KeyChar = ControlChars.Back) Then
        e.Handled = True
    End If
End Sub
```

### Penanganan Paste dari clipboard
```vb
' Tambahkan di TextChanged untuk normalisasi setelah paste
Private Sub TxtQty_TextChanged(sender As Object, e As EventArgs) Handles TxtQty.TextChanged
    ' Hitung dan update display — ParseDecimal handle semua format paste
    Dim qty As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
    ' ... kalkulasi ...
End Sub
```

### Membaca dan menampilkan Qty
```vb
' Baca — ParseDecimal handle "1,5" dan "1.5" → keduanya 1.5D
Dim qty As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)

' Isi TextBox input — plain, tanpa format
TxtQty.Text = qty.ToString()                              ' "1.5"

' Isi label display — berformat
LblQty.Text = ModuleAngka.FormatRupiah(qty) & " " & satuan  ' "1,5 kg"
```

---

## 5. Aturan Simpan ke Database

```vb
' ✅ Selalu kirim Decimal ke parameter — BUKAN string
cmd.Parameters.AddWithValue("@qty",   qty)      ' Decimal
cmd.Parameters.AddWithValue("@harga", harga)    ' Decimal
cmd.Parameters.AddWithValue("@total", total)    ' Decimal

' ❌ JANGAN kirim string — berbahaya
cmd.Parameters.AddWithValue("@qty",   TxtQty.Text)    ' ❌
cmd.Parameters.AddWithValue("@harga", TxtHarga.Text)  ' ❌
cmd.Parameters.AddWithValue("@total", LblTotal.Text)  ' ❌
```

---

## 6. Aturan Baca dari Database

```vb
' ✅ Pattern standar dengan guard IsDBNull
Dim harga As Decimal = If(IsDBNull(rd("HARGA")), 0D, Convert.ToDecimal(rd("HARGA")))
Dim qty   As Decimal = If(IsDBNull(rd("QTY")),   0D, Convert.ToDecimal(rd("QTY")))
Dim isi   As Integer = If(IsDBNull(rd("ISI")),    1,  Convert.ToInt32(rd("ISI")))

' ✅ Atau pakai ModuleAngka untuk kode lebih ringkas
Dim harga As Decimal = ModuleAngka.ParseDecimal(rd("HARGA"))
Dim isi   As Integer = ModuleAngka.ParseInteger(rd("ISI"), defaultValue:=1)
```

---

## 6b. Kapan Convert.ToDecimal dan Decimal.TryParse Boleh Dipakai

### ✅ BOLEH — sumber sudah pasti numerik (tidak ada format ribuan)

```vb
' Dari database reader — tipe kolom MySQL sudah Decimal/Int
Dim harga As Decimal = If(IsDBNull(rd("HARGA")), 0D, Convert.ToDecimal(rd("HARGA")))

' Dari cell DGV yang diisi dengan Decimal murni (bukan string berformat)
Dim total As Decimal = If(row.Cells("TOTAL").Value Is Nothing, 0D,
                          Convert.ToDecimal(row.Cells("TOTAL").Value))

' Decimal.TryParse dari TextBox yang DIISI PLAIN (tidak berformat)
' Aman karena TextBox diisi dengan .ToString() tanpa format ribuan
Dim pokok As Decimal = If(Decimal.TryParse(TxtPokok.Text, pokok), pokok, 0D)
```

### ❌ DILARANG — sumber dari TextBox atau Label yang berformat ribuan

```vb
' TextBox diisi dengan .ToString("N0") → ada titik ribuan "1.500"
Dim pokok As Decimal = Convert.ToDecimal(TxtPokok.Text)    ' ❌ crash
Dim saldo As Decimal = Convert.ToDecimal(LblSaldoBon.Text) ' ❌ crash

' Decimal.TryParse biasa dari TextBox berformat → gagal diam-diam hasilkan 0
Decimal.TryParse(TxtNominal.Text, result)                  ' ❌ silent wrong value
```

### Aturan praktis

| Sumber | Fungsi | Status |
|---|---|---|
| DB reader `rd("kolom")` | `Convert.ToDecimal` + guard `IsDBNull` | ✅ Standar |
| Cell DGV (diisi Decimal) | `Convert.ToDecimal(cell.Value)` + guard Nothing | ✅ Standar |
| Cell DGV (tidak yakin) | `ModuleAngka.ParseDecimal(cell.Value)` | ✅ Aman |
| TextBox INPUT (diisi plain `.ToString()`) | `Decimal.TryParse` atau `ModuleAngka.ParseDecimal` | ✅ Keduanya aman |
| TextBox DISPLAY (diisi berformat `.ToString("N0")`) | `ModuleAngka.ParseDecimal` | ⚠️ Wajib `ParseDecimal` |
| Label berformat | `ModuleAngka.ParseDecimal` | ⚠️ Wajib `ParseDecimal` |
| `Convert.ToDecimal(Txt*.Text)` langsung | — | ❌ **DILARANG** |

> **Catatan:** Semua aturan ini sudah diuji dan lulus 87/87 test case di `Tests/Test-ModuleAngka.ps1`.
> Jalankan ulang test setiap kali ada perubahan di `ModuleAngka.vb`:
> ```
> powershell -ExecutionPolicy Bypass -File Tests/Test-ModuleAngka.ps1
> ```

---

## 7. CultureInfo yang Dipakai

```vb
' Untuk format display Indonesia (titik ribuan, koma desimal)
Private ReadOnly cultureIndonesia As New Globalization.CultureInfo("id-ID")
' Atau pakai ModuleAngka.FormatRupiah() agar tidak perlu deklarasi di setiap form

' Untuk parse — selalu InvariantCulture setelah normalisasi (sudah di dalam ModuleAngka)
```

---

## 8. Penanganan Nilai Negatif

Nilai negatif muncul di: selisih stok opname, selisih transfer konversi satuan.

```vb
' ParseDecimal sudah handle tanda minus di depan
Dim selisih As Decimal = ModuleAngka.ParseDecimal("-1500")   ' → -1500D
Dim selisih As Decimal = ModuleAngka.ParseDecimal("-1.500")  ' → -1500D

' Untuk jurnal — selalu simpan nominal positif, arah D/K ditentukan dari tanda
Dim nominal As Decimal = Math.Abs(selisih)
If selisih < 0 Then
    ' Debit PENYESUAIAN STOK MINUS, Kredit PERSEDIAAN
Else
    ' Debit PERSEDIAAN, Kredit PENYESUAIAN STOK MINUS
End If
```

---

## 9. Checklist Sebelum Simpan Transaksi

Sebelum `cmd.ExecuteNonQuery()`, pastikan:

- [ ] Semua nilai nominal berasal dari variabel Decimal, bukan dari `.Text` langsung
- [ ] Semua TextBox input sudah dilewatkan `ModuleAngka.ParseDecimal` sebelum dipakai
- [ ] Semua cell DGV diisi dengan Decimal murni, bukan string berformat
- [ ] Semua cell DGV yang dibaca menggunakan `Convert.ToDecimal(cell.Value)` atau `ModuleAngka.ParseDecimal(cell.Value)`
- [ ] Tidak ada `Val()` di manapun — **DILARANG**
- [ ] Tidak ada `Convert.ToDecimal(Txt*.Text)` langsung tanpa parse — **DILARANG**
- [ ] Tidak ada `cmd.Parameters.AddWithValue("@x", Txt*.Text)` untuk field angka — **DILARANG**

---

## 10. Contoh Lengkap — Pola yang Benar

```vb
Public Class FormContoh

    ' ── Variabel data (sumber kebenaran) ──────────────────────────
    Private _hargaBeli As Decimal = 0D
    Private _qty       As Decimal = 0D

    Private Sub FormContoh_Load(...)
        ' Setup format kolom DGV sekali saja
        ModuleAngka.TerapkanFormatKolomAngka(DgvData, "HARGA", "QTY", "TOTAL")
    End Sub

    ' ── Saat barang dipilih dari DB ───────────────────────────────
    Private Sub IsiDataBarang(rd As MySqlDataReader)
        _hargaBeli = If(IsDBNull(rd("HARGA_BELI")), 0D, Convert.ToDecimal(rd("HARGA_BELI")))
        _qty       = 1D

        ' TextBox input — plain
        TxtHarga.Text = _hargaBeli.ToString()
        TxtQty.Text   = _qty.ToString()

        ' Label display — berformat
        LblHarga.Text = "Rp. " & ModuleAngka.FormatRupiah(_hargaBeli)
        HitungTotal()
    End Sub

    ' ── Saat user mengubah qty ────────────────────────────────────
    Private Sub TxtQty_TextChanged(...) Handles TxtQty.TextChanged
        _qty = ModuleAngka.ParseDecimal(TxtQty.Text)
        HitungTotal()
    End Sub

    Private Sub HitungTotal()
        Dim total As Decimal = _qty * _hargaBeli
        LblTotal.Text = "Rp. " & ModuleAngka.FormatRupiah(total)
    End Sub

    ' ── Saat simpan ───────────────────────────────────────────────
    Private Sub Simpan()
        cmd.Parameters.AddWithValue("@harga", _hargaBeli)        ' Decimal ✅
        cmd.Parameters.AddWithValue("@qty",   _qty)               ' Decimal ✅
        cmd.Parameters.AddWithValue("@total", _qty * _hargaBeli)  ' Decimal ✅
    End Sub

End Class
```

---

## 11. Ringkasan Aturan Cepat

| Situasi | Yang Dilakukan |
|---|---|
| Isi TextBox untuk display | `.ToString("#,0.##", cultureIndonesia)` atau `ModuleAngka.FormatRupiah()` |
| Isi TextBox untuk input | `.ToString()` — plain, tanpa format |
| Isi cell DGV | Nilai Decimal langsung |
| Format cell DGV | `ModuleAngka.TerapkanFormatKolomAngka(dgv, "kol1", "kol2")` |
| Baca TextBox input | `ModuleAngka.ParseDecimal(Txt*.Text)` |
| Baca cell DGV (Decimal) | `Convert.ToDecimal(cell.Value)` dengan guard Nothing/DBNull |
| Baca cell DGV (tidak yakin) | `ModuleAngka.ParseDecimal(cell.Value)` |
| Baca field Integer dari DGV/DB | `ModuleAngka.ParseInteger(value, defaultValue:=1)` |
| Simpan ke DB | Variabel Decimal, bukan `.Text` |
| Baca dari DB | `Convert.ToDecimal(rd("kolom"))` dengan guard `IsDBNull` |
| Nilai negatif (selisih) | `Math.Abs()` untuk nominal jurnal, tanda untuk arah D/K |
| `Val()` | **DILARANG** |
| `Convert.ToDecimal(Txt*.Text)` langsung | **DILARANG** |
| `AddWithValue("@x", Txt*.Text)` untuk angka | **DILARANG** |
