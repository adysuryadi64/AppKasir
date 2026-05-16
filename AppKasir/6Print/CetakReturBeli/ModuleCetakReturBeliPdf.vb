Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Module ModuleCetakReturBeliPdf

    Public Sub ExportPdf(Optional tampilF1 As Boolean = True,
                          Optional tampilF2 As Boolean = True,
                          Optional tampilF3 As Boolean = True)
        Dim sfd As New SaveFileDialog() With {
            .Title = "Simpan Nota Retur Beli sebagai PDF",
            .Filter = "PDF Files (*.pdf)|*.pdf",
            .FileName = "ReturBeli_" & ReturBeli_NoRetur.Replace("/", "-") & ".pdf",
            .DefaultExt = "pdf"}
        If sfd.ShowDialog() <> DialogResult.OK Then Exit Sub

        Try
            Dim cetak As New GdiCetakReturBeliThermalMatrik()
            cetak.TampilFooter1Override = tampilF1
            cetak.TampilFooter2Override = tampilF2
            cetak.TampilFooter3Override = tampilF3
            Dim bitmaps As New List(Of System.Drawing.Bitmap)
            cetak.RenderToBitmaps(bitmaps)
            If bitmaps.Count = 0 Then
                MessageBox.Show("Gagal merender nota.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            Using fs As New FileStream(sfd.FileName, FileMode.Create)
                Dim firstBmp As System.Drawing.Bitmap = bitmaps(0)
                Dim pageW As Single = firstBmp.Width * 72.0F / firstBmp.HorizontalResolution
                Dim pageH As Single = firstBmp.Height * 72.0F / firstBmp.VerticalResolution
                Dim doc As New Document(New iTextSharp.text.Rectangle(pageW, pageH), 0, 0, 0, 0)
                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                doc.Open()
                For Each bmp As System.Drawing.Bitmap In bitmaps
                    Dim imgBytes As Byte() = BitmapToBytes(bmp)
                    Dim pdfImg As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(imgBytes)
                    pdfImg.ScaleToFit(pageW, pageH) : pdfImg.SetAbsolutePosition(0, 0)
                    doc.Add(pdfImg)
                    If bitmaps.IndexOf(bmp) < bitmaps.Count - 1 Then doc.NewPage()
                    bmp.Dispose()
                Next
                doc.Close()
            End Using
            Try
                Process.Start(New ProcessStartInfo(sfd.FileName) With {.UseShellExecute = True})
            Catch : End Try
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
