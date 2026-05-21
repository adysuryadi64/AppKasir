using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        string path = @"e:\0.AppVisulaStudio\AppKasir_2026\AppKasir\0Form\FormUtama.vb";
        string content = File.ReadAllText(path, Encoding.UTF8);

        int idx = content.IndexOf("Private Sub BtnSuratJalan_Click");
        if (idx > 0)
        {
            string replacement = @"    Public Sub DataSalesOrder()
        Dim sf As String = """"%"""" & TxtFilter.Text & """"%""""
        Dim tAwal As Date = DtpTransaksi.Value.Date
        Dim tAkhir As Date = tAwal.AddDays(1).AddTicks(-1)
        HitungRangkuman(""""SELECT COUNT(*) AS RECORD, COALESCE(SUM(GRAND_TOTAL_STL_PAJAK), 0) AS TOTAL FROM sales_order WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_PENJUALAN LIKE @SearchText"""", """"Total Sales Order"""", tAwal, tAkhir, sf)
        LoadDataTransaksi(""""SELECT ID_PENJUALAN, NAMA_PELANGGAN, LOKASIBARANG, NAMA_SALES, GRAND_TOTAL_STL_PAJAK, STATUS_TRANSAKSI, ID_USER FROM sales_order WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir AND ID_PENJUALAN LIKE @SearchText ORDER BY ID_PENJUALAN ASC"""", """"sales_order"""", tAwal, tAkhir, sf)
        With DGVTransaksi
            .Columns(0).HeaderText = """"NOTA SO"""" : .Columns(0).FillWeight = 130
            .Columns(1).HeaderText = """"PELANGGAN"""" : .Columns(2).HeaderText = """"LOKASI""""
            .Columns(3).HeaderText = """"SALES"""" : .Columns(4).HeaderText = """"TOTAL""""
            .Columns(5).HeaderText = """"STATUS"""" : .Columns(6).HeaderText = """"USER""""
            AturKolomAngka(DGVTransaksi, 4)
            UbahTampilanDataTransaksi() : .ClearSelection()
        End With
        BersihkanKontrolTransaksi(""""Detail Sales Order : """")
    End Sub

";
            content = content.Insert(idx, replacement);
            File.WriteAllText(path, content, new UTF8Encoding(true));
            Console.WriteLine("Injection successful.");
        }
        else
        {
            Console.WriteLine("Search string not found!");
        }
    }
}
