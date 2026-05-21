using System;
using System.IO;
using System.Text;

class Program {
    static void Main() {
        string jualPath = @"e:\0.AppVisulaStudio\AppKasir_2026\AppKasir\2Trans\FormJual.vb";
        string soPath = @"e:\0.AppVisulaStudio\AppKasir_2026\AppKasir\2Trans\FormSalesOrder.vb";
        string backupSimpan = @"e:\0.AppVisulaStudio\AppKasir_2026\AppKasir\2Trans\backup_simpan.txt";
        string backupEdit = @"e:\0.AppVisulaStudio\AppKasir_2026\AppKasir\2Trans\backup_edit.txt";

        // Read FormJual using exact UTF8
        string jual = File.ReadAllText(jualPath, Encoding.UTF8);

        // Replace class name & inject dummy vars
        jual = jual.Replace("Public Class FormJual", "Public Class FormSalesOrder\r\n    ' DUMMY CONTROLS UNTUK MENCEGAH ERROR DESIGNER\r\n    Friend WithEvents LblStatusTrans As Label = New Label()\r\n    Friend WithEvents LblJatuhTempo As Label = New Label()\r\n    Friend WithEvents DTPJatuhTempo As DateTimePicker = New DateTimePicker()\r\n    Friend WithEvents LblPembayaran As Label = New Label()\r\n    Friend WithEvents LblBayarTunai As Label = New Label()\r\n    Friend WithEvents LblBayarTransfer As Label = New Label()\r\n    Friend WithEvents TxtNominalBayarTunai As TextBox = New TextBox()\r\n    Friend WithEvents TxtNominalBayarTransfer As TextBox = New TextBox()\r\n    Friend WithEvents CmbBayarTunai As ComboBox = New ComboBox()\r\n    Friend WithEvents TxtKodeBayarTunai As TextBox = New TextBox()\r\n    Friend WithEvents CmbBayarTransfer As ComboBox = New ComboBox()\r\n    Friend WithEvents TxtKodeBayarBank As TextBox = New TextBox()\r\n    Friend WithEvents GBBayar As GroupBox = New GroupBox()\r\n    Friend WithEvents BtnTahan As Button = New Button()\r\n    Friend WithEvents BtnPanggil As Button = New Button()\r\n");

        int endJualIndex = jual.IndexOf("    Public Sub TekanSimpan()");
        if (endJualIndex > 0) {
            jual = jual.Substring(0, endJualIndex);
        }

        string simpanCode = File.ReadAllText(backupSimpan, Encoding.UTF8);
        string editCode = File.ReadAllText(backupEdit, Encoding.UTF8);

        string finalCode = jual + simpanCode + editCode;
        
        // Write out with BOM signature!
        File.WriteAllText(soPath, finalCode, new UTF8Encoding(true));
        Console.WriteLine("Restored beautifully with BOM!");
    }
}
