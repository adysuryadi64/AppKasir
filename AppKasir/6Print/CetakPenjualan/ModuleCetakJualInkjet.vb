Imports System.Drawing.Printing

' ================================================================
' ModuleCetakJualInkjet
' Cetak nota penjualan ke printer Inkjet/Laser via GDI+.
'
' Semua layout (margin, font, lebar kolom, model nota) dibaca
' dari KonfigurasiInkjet("Jual") — ubah di FormPengaturanPrinter,
' tidak perlu ubah kode ini.
'
' Model Nota:
'   "Model 1 Lengkap"      — semua kolom + tanda tangan (dikontrol checkbox)
'   "Model 2 Tanpa Diskon" — kolom diskon disembunyikan
'
' Cara pakai:
'   ModuleCetakJualInkjet.CetakNota()
' ================================================================
Module ModuleCetakJualInkjet

#Region "State"
    Private _cfg As KonfigurasiInkjet
    Private WithEvents _pd As New PrintDocument

    ' Posisi kolom — dihitung sekali di CetakNota, dipakai di PrintPage
    Private _xNo As Integer
    Private _xNama As Integer
    Private _xQty As Integer
    Private _xHarga As Integer
    Private _xDiskon As Integer
    Private _xJml As Integer
    Private _xKanan As Integer

    ' Flag model nota
    Private _tampilDiskon As Boolean
    Private _tampilTtd As Boolean

    ' Posisi kolom tanda tangan — disimpan agar CetakFooter bisa sejajar
    Private _xTtd1 As Integer
    Private _xTtd2 As Integer
    Private _xTtd3 As Integer
#End Region

#Region "Entry Point"
    Public Sub CetakNota()
        _cfg = New KonfigurasiInkjet("Jual")

        If String.IsNullOrEmpty(_cfg.NamaPrinter) Then
            MessageBox.Show("Printer Inkjet/Laser belum diatur di pengaturan printer.",
                            "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        _pd = New PrintDocument()
        _pd.PrinterSettings.PrinterName = _cfg.NamaPrinter
        _pd.DefaultPageSettings.Landscape = (_cfg.Orientasi = "Landscape")

        ' Ukuran kertas
        For Each ps As PaperSize In _pd.PrinterSettings.PaperSizes
            Dim nama As String = _cfg.UkuranKertas.ToLower()
            If (nama.Contains("a4") AndAlso ps.Kind = PaperKind.A4) OrElse
               (nama.Contains("folio") AndAlso ps.Kind = PaperKind.Folio) OrElse
               (nama.Contains("letter") AndAlso ps.Kind = PaperKind.Letter) OrElse
               (nama.Contains("a5") AndAlso ps.Kind = PaperKind.A5) Then
                _pd.DefaultPageSettings.PaperSize = ps
                Exit For
            End If
        Next

        ' Margin mm -> 1/100 inch
        Dim mm As Func(Of Integer, Integer) = Function(v) CInt(v * 3.937)
        _pd.DefaultPageSettings.Margins = New Margins(
            mm(_cfg.MarginKiri), mm(_cfg.MarginKanan),
            mm(_cfg.MarginAtas), mm(_cfg.MarginBawah))

        ' Flag model nota — ModelNota hanya urusan diskon, tanda tangan dari checkbox
        _tampilDiskon = (_cfg.ModelNota <> "Model 2 Tanpa Diskon")
        _tampilTtd = _cfg.TampilTandaTangan

        AddHandler _pd.PrintPage, AddressOf Pd_PrintPage

        For i As Integer = 1 To Math.Max(1, _cfg.JumlahCetak)
            _pd.Print()
        Next
    End Sub
#End Region

#Region "Print Page"
    Private Sub Pd_PrintPage(sender As Object, e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim b As Rectangle = e.MarginBounds

        Dim fJudul As New Font(_cfg.FontJudul, _cfg.UkuranJudul, FontStyle.Bold)
        Dim fIsi As New Font(_cfg.FontIsi, _cfg.UkuranIsi)
        Dim fBold As New Font(_cfg.FontIsi, _cfg.UkuranIsi, FontStyle.Bold)
        Dim fKecil As New Font(_cfg.FontIsi, Math.Max(6, _cfg.UkuranIsi - 1))
        Dim lh As Integer = CInt(fIsi.GetHeight(g)) + 2

        Dim fmtKanan As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim fmtTengah As New StringFormat() With {.Alignment = StringAlignment.Center}
        Dim tengah As Integer = b.Left + b.Width \ 2
        Dim y As Integer = b.Top

        HitungKolom(b)

        y = CetakHeader(g, b, fJudul, fIsi, fmtTengah, tengah, y, lh)
        y = CetakInfoTransaksi(g, b, fIsi, fBold, tengah, y, lh)
        y = CetakTabelItem(g, b, fIsi, fBold, fKecil, fmtKanan, y, lh)
        y = CetakTotal(g, b, fIsi, fBold, fKecil, fmtKanan, y, lh)
        If _tampilTtd Then y = CetakTandaTangan(g, b, fKecil, fmtTengah, y, lh)
        CetakFooter(g, fKecil, fmtTengah, tengah, y, lh)

        e.HasMorePages = False
    End Sub
#End Region

#Region "Hitung Kolom"
    ' Posisi kolom dihitung dari persen konfigurasi.
    ' Kolom: No | Nama Barang | Qty | Harga | [Diskon] | Jumlah
    ' Kolom Nama = sisa setelah semua kolom lain dialokasikan.
    ' Kolom Jumlah = lebar sama dengan Harga, rata kanan (b.Right).
    Private Sub HitungKolom(b As Rectangle)
        Dim w As Integer = b.Width
        Dim pNo As Integer = _cfg.PctKolomNo
        Dim pQty As Integer = _cfg.PctKolomQty
        Dim pHarga As Integer = _cfg.PctKolomHarga
        Dim pDiskon As Integer = If(_tampilDiskon, _cfg.PctKolomDiskon, 0)
        Dim pJml As Integer = pHarga
        Dim pNama As Integer = 100 - pNo - pQty - pHarga - pDiskon - pJml
        If pNama < 10 Then pNama = 10

        _xNo = b.Left
        _xNama = b.Left + CInt(w * pNo / 100.0)
        _xQty = b.Left + CInt(w * (pNo + pNama) / 100.0)
        _xHarga = b.Left + CInt(w * (pNo + pNama + pQty) / 100.0)
        _xDiskon = b.Left + CInt(w * (pNo + pNama + pQty + pHarga) / 100.0)
        _xJml = b.Right
        _xKanan = b.Right
    End Sub
#End Region

#Region "Header Toko"
    Private Function CetakHeader(g As Graphics, b As Rectangle,
                                  fJudul As Font, fIsi As Font,
                                  fmtTengah As StringFormat,
                                  tengah As Integer, y As Integer, lh As Integer) As Integer
        Dim yTeks As Integer = y   ' posisi y awal teks — sama dengan y awal logo
        Dim xTeks As Integer = b.Left  ' default: teks mulai dari kiri jika tidak ada logo
        Dim lebarTeks As Integer = b.Width

        If _cfg.TampilLogo Then
            Try
                Dim logoPath As String = IO.Path.Combine(Application.StartupPath, "logo.png")
                If Not IO.File.Exists(logoPath) Then
                    logoPath = IO.Path.Combine(Application.StartupPath, "logo.jpg")
                End If
                If IO.File.Exists(logoPath) Then
                    Using logo As Image = Image.FromFile(logoPath)
                        Dim logoH As Integer = 50
                        Dim logoW As Integer = CInt(logo.Width * (logoH / logo.Height))
                        Dim gap As Integer = 8

                        ' Gambar logo di kiri, y sama dengan y teks — benar-benar sejajar satu baris
                        g.DrawImage(logo, b.Left, yTeks, logoW, logoH)

                        ' Teks dimulai di sebelah kanan logo
                        xTeks = b.Left + logoW + gap
                        lebarTeks = b.Right - xTeks

                        ' y akhir = bawah logo (jika logo lebih tinggi dari teks)
                        y = Math.Max(y, yTeks + logoH + 4)
                    End Using
                End If
            Catch
            End Try
        End If

        ' Teks header rata tengah di area kanan logo (atau seluruh lebar jika tidak ada logo)
        Dim tengahTeks As Integer = xTeks + lebarTeks \ 2
        Dim yT As Integer = yTeks

        g.DrawString(NAMA_PERUSAHAAN, fJudul, Brushes.Black, tengahTeks, yT, fmtTengah)
        yT += CInt(fJudul.GetHeight(g)) + 2
        g.DrawString(ALAMAT_PERUSAHAAN, fIsi, Brushes.Black, tengahTeks, yT, fmtTengah)
        yT += lh
        g.DrawString(KOTA_PERUSAHAAN & "  " & KONTAK_PERUSAHAAN, fIsi, Brushes.Black, tengahTeks, yT, fmtTengah)
        yT += lh + 4

        ' y akhir = nilai terbesar antara bawah logo dan bawah teks
        y = Math.Max(y, yT)

        g.DrawLine(New Pen(Color.Black, 2), b.Left, y, _xKanan, y) : y += 3
        g.DrawLine(New Pen(Color.Black, 1), b.Left, y, _xKanan, y) : y += 6

        g.DrawString("NOTA PENJUALAN",
                     New Font(_cfg.FontJudul, _cfg.UkuranJudul + 1, FontStyle.Bold),
                     Brushes.Black, tengah, y, fmtTengah)
        y += CInt(fJudul.GetHeight(g)) + 6
        Return y
    End Function
#End Region

#Region "Info Transaksi"
    ' 2 kolom: kiri (No Faktur, Tanggal, Pelanggan) | kanan (Kasir, Sales)
    ' Lebar kolom nilai mengikuti lebar area cetak secara proporsional.
    Private Function CetakInfoTransaksi(g As Graphics, b As Rectangle,
                                         fIsi As Font, fBold As Font,
                                         tengah As Integer, y As Integer, lh As Integer) As Integer
        Dim xKiri As Integer = b.Left
        Dim xVal1 As Integer = b.Left + CInt(b.Width * 0.12)
        Dim xKanan As Integer = tengah + 10
        Dim xVal2 As Integer = tengah + CInt(b.Width * 0.12)

        g.DrawString("No. Faktur", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & Jual_NoFaktur, fBold, Brushes.Black, xVal1, y)
        g.DrawString("Kasir", fIsi, Brushes.Black, xKanan, y)
        g.DrawString(": " & Jual_IdUser & " / " & Jual_IdKomputer, fIsi, Brushes.Black, xVal2, y)
        y += lh

        g.DrawString("Tanggal", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & Jual_Tanggal.ToString("dd-MM-yyyy HH:mm:ss"), fIsi, Brushes.Black, xVal1, y)
        If Not String.IsNullOrEmpty(Jual_NamaSales) Then
            g.DrawString("Sales", fIsi, Brushes.Black, xKanan, y)
            g.DrawString(": " & Jual_NamaSales, fIsi, Brushes.Black, xVal2, y)
        End If
        y += lh

        g.DrawString("Pelanggan", fIsi, Brushes.Black, xKiri, y)
        g.DrawString(": " & Jual_JenisPelanggan & " - " & Jual_NamaPelanggan,
                     fIsi, Brushes.Black, xVal1, y)
        y += lh + 4

        g.DrawLine(Pens.Black, b.Left, y, _xKanan, y) : y += 6
        Return y
    End Function
#End Region

#Region "Tabel Item"
    Private Function CetakTabelItem(g As Graphics, b As Rectangle,
                                     fIsi As Font, fBold As Font, fKecil As Font,
                                     fmtKanan As StringFormat,
                                     y As Integer, lh As Integer) As Integer
        ' Header kolom
        g.DrawString("No", fBold, Brushes.Black, _xNo, y)
        g.DrawString("Nama Barang", fBold, Brushes.Black, _xNama, y)
        g.DrawString("Qty", fBold, Brushes.Black, _xQty, y, fmtKanan)
        g.DrawString("Harga", fBold, Brushes.Black, _xHarga, y, fmtKanan)
        If _tampilDiskon Then
            g.DrawString("Diskon", fBold, Brushes.Black, _xDiskon, y, fmtKanan)
        End If
        g.DrawString("Jumlah", fBold, Brushes.Black, _xJml, y, fmtKanan)
        y += lh
        g.DrawLine(Pens.Black, b.Left, y, _xKanan, y) : y += 4

        ' Baris item
        Dim nomor As Integer = 1
        For Each item As ItemNotaJual In Jual_DaftarItem
            g.DrawString(nomor & ".", fIsi, Brushes.Black, _xNo, y)
            g.DrawString(item.NamaBarang, fIsi, Brushes.Black, _xNama, y)
            g.DrawString(item.Qty.ToString("#,0.##", cultureIndonesia) & " " & item.Satuan,
                         fIsi, Brushes.Black, _xQty, y, fmtKanan)
            g.DrawString(item.Harga.ToString("N0", cultureIndonesia),
                         fIsi, Brushes.Black, _xHarga, y, fmtKanan)
            If _tampilDiskon Then
                g.DrawString(item.TotalDiskon.ToString("N0", cultureIndonesia),
                             fIsi, Brushes.Black, _xDiskon, y, fmtKanan)
            End If
            g.DrawString(item.TotalHarga.ToString("N0", cultureIndonesia),
                         fIsi, Brushes.Black, _xJml, y, fmtKanan)
            y += lh

            If Not String.IsNullOrEmpty(item.SerialNumber) Then
                g.DrawString("   SN: " & item.SerialNumber, fKecil, Brushes.Gray, _xNama, y)
                y += lh
            End If
            nomor += 1
        Next

        g.DrawLine(Pens.Black, b.Left, y, _xKanan, y) : y += 4
        Return y
    End Function
#End Region

#Region "Total"
    Private Function CetakTotal(g As Graphics, b As Rectangle,
                                 fIsi As Font, fBold As Font, fKecil As Font,
                                 fmtKanan As StringFormat,
                                 y As Integer, lh As Integer) As Integer
        Dim xLbl As Integer = b.Left + CInt(b.Width * 0.58)
        Dim xVal As Integer = _xKanan

        ' Helper: cetak label rata kanan di area b.Left..xLbl, nilai rata kanan di xVal
        Dim fmtLbl As New StringFormat() With {.Alignment = StringAlignment.Far}
        Dim TulisLbl = Sub(lbl As String, fnt As Font, br As Brush, yy As Integer)
                           Dim r As New RectangleF(b.Left, yy, xLbl - b.Left, fnt.GetHeight(g) + 2)
                           g.DrawString(lbl, fnt, br, r, fmtLbl)
                       End Sub

        g.DrawString(Jual_DaftarItem.Count & " item", fKecil, Brushes.Gray, b.Left, y)
        TulisLbl("Subtotal :", fIsi, Brushes.Black, y)
        g.DrawString(Jual_Total.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xVal, y, fmtKanan)
        y += lh

        If _tampilDiskon AndAlso Jual_Diskon <> 0 Then
            TulisLbl("Diskon :", fIsi, Brushes.Black, y)
            g.DrawString(Jual_Diskon.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xVal, y, fmtKanan)
            y += lh
        End If

        If Jual_Pajak <> 0 Then
            TulisLbl("Pajak :", fIsi, Brushes.Black, y)
            g.DrawString(Jual_Pajak.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xVal, y, fmtKanan)
            y += lh
        End If

        If Jual_BiayaKirim <> 0 Then
            TulisLbl("Biaya Kirim :", fIsi, Brushes.Black, y)
            g.DrawString(Jual_BiayaKirim.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xVal, y, fmtKanan)
            y += lh
        End If

        ' Jika split bayar: tampilkan Tunai + Transfer langsung (tanpa baris Bayar)
        ' Jika tidak ada transfer: tampilkan Bayar seperti biasa
        If Jual_JudulNota <> "Nota Order" Then
            If Jual_NominalTransfer > 0 Then
                If Jual_Bayar > 0 Then
                    TulisLbl("Tunai (" & Jual_Penerima & ") :", fIsi, Brushes.Black, y)
                    g.DrawString(Jual_Bayar.ToString("N0", cultureIndonesia),
                                 fIsi, Brushes.Black, xVal, y, fmtKanan)
                    y += lh
                End If
                TulisLbl("Transfer (" & Jual_NamaAkunTransfer & ") :", fIsi, Brushes.Black, y)
                g.DrawString(Jual_NominalTransfer.ToString("N0", cultureIndonesia),
                             fIsi, Brushes.Black, xVal, y, fmtKanan)
                y += lh
            Else
                TulisLbl("Bayar :", fIsi, Brushes.Black, y)
                g.DrawString(Jual_Bayar.ToString("N0", cultureIndonesia), fIsi, Brushes.Black, xVal, y, fmtKanan)
                y += lh
            End If

            g.DrawLine(New Pen(Color.Black, 2), xLbl, y, xVal, y) : y += 4

            TulisLbl(Jual_LabelPembayaran, fBold, Brushes.Black, y)
            g.DrawString(Jual_Kembali.ToString("N0", cultureIndonesia), fBold, Brushes.Black, xVal, y, fmtKanan)
            y += lh
        End If

        If Jual_StatusTransaksi = "Belum Lunas" AndAlso Jual_AdaJatuhTempo Then
            TulisLbl("Jatuh Tempo :", fIsi, Brushes.Black, y)
            g.DrawString(Jual_JatuhTempoDate.ToString("dd-MM-yyyy"), fIsi, Brushes.Black, xVal, y, fmtKanan)
            y += lh
        End If

        ' Info transfer — detail bank/rekening (Metode, Bank, No Rek, No Reff)
        If Jual_NominalTransfer > 0 Then
            y += 4
            g.DrawString("Metode: " & Jual_Metode, fKecil, Brushes.Gray, b.Left, y) : y += lh
            g.DrawString("Bank: " & Jual_Bank & " - " & Jual_NamaRekening & " | No: " & Jual_NoRekening,
                         fKecil, Brushes.Gray, b.Left, y) : y += lh
            If Not String.IsNullOrEmpty(Jual_NoReferensi) Then
                g.DrawString("No Reff: " & Jual_NoReferensi, fKecil, Brushes.Gray, b.Left, y) : y += lh
            End If
        End If

        ' Terbilang
        y += 4
        g.DrawLine(Pens.LightGray, b.Left, y, _xKanan, y) : y += 4
        g.DrawString("Terbilang: " & Terbilang(Jual_Total),
                     fKecil, Brushes.Gray, b.Left, y)
        y += lh + 8
        Return y
    End Function
#End Region

#Region "Tanda Tangan"
    Private Function CetakTandaTangan(g As Graphics, b As Rectangle,
                                       fKecil As Font, fmtTengah As StringFormat,
                                       y As Integer, lh As Integer) As Integer
        Dim xT1 As Integer = b.Left + CInt(b.Width * 0.1)
        Dim xT2 As Integer = b.Left + CInt(b.Width * 0.45)
        Dim xT3 As Integer = b.Left + CInt(b.Width * 0.75)

        ' Simpan posisi agar CetakFooter bisa sejajar
        _xTtd1 = xT1
        _xTtd2 = xT2
        _xTtd3 = xT3

        g.DrawString("Hormat Kami", fKecil, Brushes.Black, xT1, y, fmtTengah)
        g.DrawString("Penerima", fKecil, Brushes.Black, xT2, y, fmtTengah)
        g.DrawString("Kasir", fKecil, Brushes.Black, xT3, y, fmtTengah)
        y += 40

        g.DrawLine(Pens.Black, xT1 - 30, y, xT1 + 30, y)
        g.DrawLine(Pens.Black, xT2 - 30, y, xT2 + 30, y)
        g.DrawLine(Pens.Black, xT3 - 30, y, xT3 + 30, y)
        y += 4

        g.DrawString("( .............. )", fKecil, Brushes.Black, xT1, y, fmtTengah)
        g.DrawString("( .............. )", fKecil, Brushes.Black, xT2, y, fmtTengah)
        g.DrawString("( " & Jual_IdUser & " )", fKecil, Brushes.Black, xT3, y, fmtTengah)
        y += lh + 10
        Return y
    End Function
#End Region

#Region "Footer"
    Private Sub CetakFooter(g As Graphics, fKecil As Font,
                             fmtTengah As StringFormat,
                             tengah As Integer, y As Integer, lh As Integer)
        If _tampilTtd Then
            ' Footer sejajar dengan kolom tanda tangan
            If _cfg.TampilFooter1 Then
                g.DrawString(FOOTER1, fKecil, Brushes.Gray, _xTtd1, y, fmtTengah)
            End If
            If _cfg.TampilFooter2 Then
                g.DrawString(FOOTER2, fKecil, Brushes.Gray, _xTtd2, y, fmtTengah)
            End If
            If _cfg.TampilFooter3 Then
                g.DrawString(FOOTER3, fKecil, Brushes.Gray, _xTtd3, y, fmtTengah)
            End If
        Else
            ' Tanpa tanda tangan — footer di tengah berurutan ke bawah
            If _cfg.TampilFooter1 Then
                g.DrawString(FOOTER1, fKecil, Brushes.Gray, tengah, y, fmtTengah) : y += lh
            End If
            If _cfg.TampilFooter2 Then
                g.DrawString(FOOTER2, fKecil, Brushes.Gray, tengah, y, fmtTengah) : y += lh
            End If
            If _cfg.TampilFooter3 Then
                g.DrawString(FOOTER3, fKecil, Brushes.Gray, tengah, y, fmtTengah)
            End If
        End If
    End Sub
#End Region

End Module