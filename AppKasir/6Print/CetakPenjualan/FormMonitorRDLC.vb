Imports Microsoft.Reporting.WinForms

' ================================================================
' FormMonitorRDLC
' Tampilkan nota penjualan di layar via ReportViewer (RDLC).
' Data diambil dari Jual_* di ModulePrinterJual — tidak query DB.
'
' Cara pakai:
'   ModulePrinterJual.MuatDataPenjualan(noFaktur)  ' sudah dipanggil otomatis
'   FormMonitorRDLC.TampilkanNota(tampilF1, tampilF2, tampilF3)
' ================================================================
Public Class FormMonitorRDLC

    ' ── Entry point statis ────────────────────────────────────
    Public Shared Sub TampilkanNota(Optional tampilF1 As Boolean = True,
                                     Optional tampilF2 As Boolean = True,
                                     Optional tampilF3 As Boolean = True)
        Dim frm As New FormMonitorRDLC()
        frm.MuatLaporan(tampilF1, tampilF2, tampilF3)
        frm.ShowDialog()
    End Sub

    ' ── Muat data ke ReportViewer ─────────────────────────────
    Private Sub MuatLaporan(tampilF1 As Boolean, tampilF2 As Boolean, tampilF3 As Boolean)
        Dim dt As New DataTable("nota_penjualan")
        dt.Columns.Add("ID_BARANG", GetType(String))
        dt.Columns.Add("NAMA_BARANG", GetType(String))
        dt.Columns.Add("QTY", GetType(Decimal))
        dt.Columns.Add("SATUAN", GetType(String))
        dt.Columns.Add("HARGA_JUAL", GetType(Decimal))
        dt.Columns.Add("TOTAL_DISKON", GetType(Decimal))
        dt.Columns.Add("TOTAL_HARGA", GetType(Decimal))

        For Each item As ItemNotaJual In Jual_DaftarItem
            Dim row As DataRow = dt.NewRow()
            row("ID_BARANG") = ""
            row("NAMA_BARANG") = item.NamaBarang
            row("QTY") = item.Qty
            row("SATUAN") = item.Satuan
            row("HARGA_JUAL") = item.Harga
            row("TOTAL_DISKON") = item.TotalDiskon
            row("TOTAL_HARGA") = item.TotalHarga
            dt.Rows.Add(row)
        Next

        ' Parameter laporan
        Dim tanggalJT As String = If(Jual_AdaJatuhTempo, Jual_JatuhTempo, "")
        Dim labelJT As String = If(Jual_AdaJatuhTempo, "Tanggal JT :", "")
        Dim adaTransfer As Boolean = Jual_NominalTransfer > 0
        Dim jenisPembayaran As String = If(adaTransfer, Jual_NamaAkunTransfer, Jual_Penerima)
        Dim bank As String = If(adaTransfer, "From " & Jual_Bank, "")
        Dim noRekening As String = If(adaTransfer, "Rek " & Jual_NoRekening, "")
        Dim namaRekening As String = If(adaTransfer, Jual_NamaRekening, "")
        Dim noRefferensi As String = If(adaTransfer, "Ref " & Jual_NoReferensi, "")
        Dim infoBayarTunai As String = If(adaTransfer AndAlso Jual_Bayar > 0,
            Jual_Bayar.ToString("N0", cultureIndonesia), "")
        Dim infoBayarTransfer As String = If(adaTransfer,
            Jual_NominalTransfer.ToString("N0", cultureIndonesia), "")

        Dim params As New List(Of ReportParameter) From {
            New ReportParameter("NamaToko", NAMA_PERUSAHAAN),
            New ReportParameter("AlamatToko", ALAMAT_PERUSAHAAN),
            New ReportParameter("KotaToko", KOTA_PERUSAHAAN),
            New ReportParameter("KontakToko", KONTAK_PERUSAHAAN),
            New ReportParameter("Footer1", If(tampilF1, FOOTER1, "")),
            New ReportParameter("Footer2", If(tampilF2, FOOTER2, "")),
            New ReportParameter("Footer3", If(tampilF3, FOOTER3, "")),
            New ReportParameter("Faktur", Jual_NoFaktur),
            New ReportParameter("TglTransaksi", Jual_Tanggal.ToString("dd-MM-yy HH:mm:ss")),
            New ReportParameter("NamaPelanggan", Jual_NamaPelanggan),
            New ReportParameter("JenisPelanggan", Jual_JenisPelanggan),
            New ReportParameter("DiskonTotalRp", Jual_Diskon.ToString("N0", cultureIndonesia)),
            New ReportParameter("PajakRp", Jual_Pajak.ToString("N0", cultureIndonesia)),
            New ReportParameter("GrandTotalStlPajak", Jual_Total.ToString("N0", cultureIndonesia)),
            New ReportParameter("Terbilang", Terbilang(Jual_Total)),
            New ReportParameter("Bayar", Jual_Bayar.ToString("N0", cultureIndonesia)),
            New ReportParameter("StatusTransaksi", Jual_LabelPembayaran),
            New ReportParameter("Kembali", Jual_Kembali.ToString("N0", cultureIndonesia)),
            New ReportParameter("TanggalJT", labelJT),
            New ReportParameter("JatuhTempo", tanggalJT),
            New ReportParameter("JenisPembayaran", jenisPembayaran),
            New ReportParameter("Metode", Jual_Metode),
            New ReportParameter("Bank", bank),
            New ReportParameter("NamaRekening", namaRekening),
            New ReportParameter("NoRekening", noRekening),
            New ReportParameter("NoRefferensi", noRefferensi),
            New ReportParameter("InfoBayarTunai", infoBayarTunai),
            New ReportParameter("InfoBayarTransfer", infoBayarTransfer),
            New ReportParameter("IdUser", Jual_IdUser),
            New ReportParameter("IdKomputer", Jual_IdKomputer)
        }

        ' Pasang ke ReportViewer
        With ReportViewer1.LocalReport
            .DataSources.Clear()
            .DataSources.Add(New ReportDataSource("DataSet1", dt))

            ' Coba load RDLC dari embedded resource, fallback ke file
            Dim rdlcStream As System.IO.Stream =
                System.Reflection.Assembly.GetExecutingAssembly().
                GetManifestResourceStream("KasirLancar.ReportCetakJual.rdlc")
            If rdlcStream IsNot Nothing Then
                .LoadReportDefinition(rdlcStream)
            Else
                Dim rdlcPath As String = System.IO.Path.Combine(
                    Application.StartupPath, "ReportCetakJual.rdlc")
                If System.IO.File.Exists(rdlcPath) Then
                    .ReportPath = rdlcPath
                Else
                    MessageBox.Show("File ReportCetakJual.rdlc tidak ditemukan.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            End If

            .SetParameters(params)
        End With

        ReportViewer1.RefreshReport()
    End Sub

    ' ── Tombol ESC untuk tutup ────────────────────────────────
    Private Sub FormMonitorRDLC_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then Close()
    End Sub

    Private Sub FormMonitorRDLC_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.KeyPreview = True
        Me.Text = "Nota Penjualan - " & Jual_NoFaktur
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(900, 700)
    End Sub

End Class
