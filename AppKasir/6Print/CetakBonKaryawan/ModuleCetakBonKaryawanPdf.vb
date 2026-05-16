Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Module ModuleCetakBonKaryawanPdf

    Public Sub ExportPdf(Optional f1 As Boolean = True, Optional f2 As Boolean = True, Optional f3 As Boolean = True)
        Dim sfd As New SaveFileDialog() With {
            .Title = "Simpan Slip Bon Karyawan sebagai PDF",
            .Filter = "PDF Files (*.pdf)|*.pdf",
            .FileName = "BonKaryawan_" & BK_Faktur.Replace("/", "-") & ".pdf",
            .DefaultExt = "pdf"}
        If sfd.ShowDialog() <> DialogResult.OK Then Exit Sub
        Try
            Dim cetak As New GdiCetakBonKaryawan()
            cetak.TampilFooter1Override = f1 : cetak.TampilFooter2Override = f2 : cetak.TampilFooter3Override = f3
            Dim bitmaps As New List(Of System.Drawing.Bitmap)
            cetak.RenderToBitmaps(bitmaps)
            If bitmaps.Count = 0 Then
                MessageBox.Show("Gagal merender nota.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            Using fs As New FileStream(sfd.FileName, FileMode.Create)
                Dim bmp As System.Drawing.Bitmap = bitmaps(0)
                Dim pageW As Single = bmp.Width * 72.0F / bmp.HorizontalResolution
                Dim pageH As Single = bmp.Height * 72.0F / bmp.VerticalResolution
                Dim doc As New Document(New iTextSharp.text.Rectangle(pageW, pageH), 0, 0, 0, 0)
                PdfWriter.GetInstance(doc, fs)
                doc.Open()
                For Each b As System.Drawing.Bitmap In bitmaps
                    Dim img As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(BitmapToBytes(b))
                    img.ScaleToFit(pageW, pageH) : img.SetAbsolutePosition(0, 0)
                    doc.Add(img)
                    If bitmaps.IndexOf(b) < bitmaps.Count - 1 Then doc.NewPage()
                    b.Dispose()
                Next
                doc.Close()
            End Using
            Try : Process.Start(New ProcessStartInfo(sfd.FileName) With {.UseShellExecute = True}) : Catch : End Try
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
