Imports System.Drawing.Printing
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

' ================================================================
' ModuleCetakJualPdf
' Export nota penjualan ke PDF menggunakan GdiCetakJualThermalMatrik
' yang di-render ke Graphics lalu disimpan sebagai PDF via iTextSharp.
'
' Cara pakai:
'   ModuleCetakJualPdf.ExportPdf(tampilF1, tampilF2, tampilF3)
' ================================================================
Module ModuleCetakJualPdf

    Public Sub ExportPdf(Optional tampilF1 As Boolean = True,
                          Optional tampilF2 As Boolean = True,
                          Optional tampilF3 As Boolean = True)

        ' Pilih lokasi simpan
        Dim sfd As New SaveFileDialog() With {
            .Title = "Simpan Nota sebagai PDF",
            .Filter = "PDF Files (*.pdf)|*.pdf",
            .FileName = "Nota_" & Jual_NoFaktur.Replace("/", "-") & ".pdf",
            .DefaultExt = "pdf"
        }
        If sfd.ShowDialog() <> DialogResult.OK Then Exit Sub

        Dim filePath As String = sfd.FileName

        Try
            ' Render GDI+ ke metafile/bitmap lalu simpan ke PDF
            Dim cetak As New GdiCetakJualThermalMatrik()
            cetak.TampilFooter1Override = tampilF1
            cetak.TampilFooter2Override = tampilF2
            cetak.TampilFooter3Override = tampilF3

            ' Render ke bitmap menggunakan PrintDocument
            Dim bitmaps As New List(Of System.Drawing.Bitmap)
            cetak.RenderToBitmaps(bitmaps)

            If bitmaps.Count = 0 Then
                MessageBox.Show("Gagal merender nota.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' Tulis ke PDF via iTextSharp
            Using fs As New FileStream(filePath, FileMode.Create)
                Dim firstBmp As System.Drawing.Bitmap = bitmaps(0)
                Dim pageW As Single = firstBmp.Width * 72.0F / firstBmp.HorizontalResolution
                Dim pageH As Single = firstBmp.Height * 72.0F / firstBmp.VerticalResolution

                Dim doc As New Document(New iTextSharp.text.Rectangle(pageW, pageH), 0, 0, 0, 0)
                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                doc.Open()

                For Each bmp As System.Drawing.Bitmap In bitmaps
                    Dim imgBytes As Byte() = BitmapToBytes(bmp)
                    Dim pdfImg As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(imgBytes)
                    pdfImg.ScaleToFit(pageW, pageH)
                    pdfImg.SetAbsolutePosition(0, 0)
                    doc.Add(pdfImg)
                    If bitmaps.IndexOf(bmp) < bitmaps.Count - 1 Then doc.NewPage()
                    bmp.Dispose()
                Next

                doc.Close()
            End Using

            ' Buka PDF setelah disimpan
            Try
                Process.Start(New ProcessStartInfo(filePath) With {.UseShellExecute = True})
            Catch
            End Try

        Catch ex As Exception
            MessageBox.Show("Gagal export PDF: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function BitmapToBytes(bmp As System.Drawing.Bitmap) As Byte()
        Using ms As New MemoryStream()
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            Return ms.ToArray()
        End Using
    End Function

End Module
