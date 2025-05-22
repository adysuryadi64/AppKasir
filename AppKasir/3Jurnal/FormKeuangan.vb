Public Class FormKeuangan
    Private Sub FormKeuangan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim JURNAL As Boolean() = ModulHakAkses.BacaHakAkses(FormUtama.SLevel.Text, "JURNAL", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnSimpanKeuangan.Visible = JURNAL(1) ' CanAdd 

        PanelPemasukan.Visible = False
        PanelRinciKeuangan.Visible = False
    End Sub


    Private ReadOnly originalColor As Color

    Private Sub HandleButtonClick(clickedButton As Button, transactionName As String, detailText As String)
        ' Reset all button colors to originalColor
        BtnPemasukan.BackColor = originalColor
        BtnPengeluaran.BackColor = originalColor
        BtnBiaya.BackColor = originalColor
        BtnSetorBos.BackColor = originalColor
        BtnBayarBon.BackColor = originalColor
        BtnPindahR.BackColor = originalColor

        ' Set clicked button color to OrangeRed
        clickedButton.BackColor = Color.OrangeRed

        ' Update panel visibility and labels
        PanelPemasukan.Visible = True
        PanelRinciKeuangan.Visible = True
        LblNamaTransaksi.Text = transactionName
        LblRinciPengeluaran.Text = detailText

        DTPTglKeuangan.Value = DateTime.Now

        ' Execute common actions
        Kondisisetalhproseskeuangan()
        Ambildataakunnama()
    End Sub

    Private Sub BtnPemasukan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPemasukan.Click
        HandleButtonClick(BtnPemasukan, "PEMASUKAN", "RINCIAN PEMASUKAN")
    End Sub

    Private Sub BtnPengeluaran_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPengeluaran.Click
        HandleButtonClick(BtnPengeluaran, "PENGELUARAN", "RINCIAN PENGELUARAN")
    End Sub

    Private Sub BtnBiaya_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBiaya.Click
        HandleButtonClick(BtnBiaya, "BIAYA", "RINCIAN BIAYA")
    End Sub

    Private Sub BtnSetorBos_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSetorBos.Click
        HandleButtonClick(BtnSetorBos, "SETOR KE BOS", "RINCIAN SETOR KE BOS")
    End Sub

    Private Sub BtnBayarBon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBayarBon.Click
        HandleButtonClick(BtnBayarBon, "BAYAR BON PRIBADI", "RINCIAN BAYAR BON PRIBADI")
    End Sub

    Private Sub BtnPindahR_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPindahR.Click
        HandleButtonClick(BtnPindahR, "PINDAH REKENING", "RINCIAN PINDAH REKENING")
    End Sub


    Private Sub Kondisisetalhproseskeuangan()
        BtnEditKeuangan.Visible = False
        BtnBatalKeuangan.Visible = False
        BtnSimpanKeuangan.Visible = True

        DTPTglKeuangan.Format = DateTimePickerFormat.Custom
        DTPTglKeuangan.CustomFormat = "dd/MM/yyyy"

        LblIdBayar.Text = ""
        TxtNoNota.Text = ""
        If LblNamaTransaksi.Text = "SETOR KE BOS" Then
            TxtUraianKeuangan.Text = "Setoran uang tunai dari kasir " & FormUtama.SLogin.Text
        Else
            TxtUraianKeuangan.Text = ""
        End If

        'CmbDebetKeuangan.Text = ""
        'TxtDebetKeuanganNama.Text = ""
        'TxtDebetKeuangan.Text = ""
        'CmbKreditKeuangan.Text = ""
        'TxtKreditKeuanganNama.Text = ""
        'TxtKreditKeuangan.Text = ""

        LblBantuDKeuangan.Text = "Bantu D:"
        'CmbBantuDKeuangan.Text = ""
        'TxtBantuDKeuanganNama.Text = ""
        'TxtBantuDKeuangan.Text = ""
        LblBantuDKeuangan.Visible = False
        CmbBantuDKeuangan.Visible = False
        TxtBantuDKeuanganNama.Visible = False
        TxtBantuDKeuangan.Visible = False

        LblBantuKKeuangan.Text = "Bantu K"
        'CmbBantuKKeuangan.Text = ""
        'TxtBantuKKeuanganNama.Text = ""
        'TxtBantuKKeuangan.Text = ""
        LblBantuKKeuangan.Visible = False
        CmbBantuKKeuangan.Visible = False
        TxtBantuKKeuanganNama.Visible = False
        TxtBantuKKeuangan.Visible = False

        TxtNominalKeuangan.Text = ""
        LblNominalKeuangan.Text = "Rp. 0"

        Idkeuangan()
        DGVTAMPILDATAKEUANGAN()
    End Sub

    Private Sub Ambildataakunnama()
        CmbDebetKeuangan.Items.Clear()
        CmbKreditKeuangan.Items.Clear()
        If LblNamaTransaksi.Text = "PEMASUKAN" Then
            Dim sqlAtas As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' OR Type_Akun LIKE 'BANK' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlAtas, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbDebetKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using

            Dim sqlBawah As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun NOT LIKE 'KAS' AND Type_Akun NOT LIKE 'BANK' AND Type_Akun NOT LIKE 'LABA RUGI' AND Jenis_Akun NOT LIKE 'ASET TETAP' AND Jenis_Akun NOT LIKE 'BIAYA' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlBawah, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbKreditKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using
            CmbDebetKeuangan.SelectedIndex = 0

        ElseIf LblNamaTransaksi.Text = "PENGELUARAN" Then
            Dim sqlAtas As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun NOT LIKE 'KAS' AND Type_Akun NOT LIKE 'BANK' AND Type_Akun NOT LIKE 'LABA RUGI'  AND Type_Akun NOT LIKE 'BIAYA' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlAtas, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbDebetKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using

            Dim sqlBawah As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' OR Type_Akun LIKE 'BANK' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlBawah, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbKreditKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using
            CmbKreditKeuangan.SelectedIndex = 0

        ElseIf LblNamaTransaksi.Text = "BIAYA" Then
            Dim sqlAtas As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'BIAYA' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlAtas, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbDebetKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using

            Dim sqlBawah As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' OR Type_Akun LIKE 'BANK' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlBawah, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbKreditKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using
            CmbKreditKeuangan.SelectedIndex = 0

        ElseIf LblNamaTransaksi.Text = "SETOR KE BOS" Then
            Dim sqlAtas As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'EKUITAS' and kode_akun LIKE '04.02.001' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlAtas, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbDebetKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using

            Dim sqlBawah As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlBawah, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbKreditKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using
            CmbDebetKeuangan.SelectedIndex = 0
            CmbKreditKeuangan.SelectedIndex = 0

        ElseIf LblNamaTransaksi.Text = "BAYAR BON PRIBADI" Then
            Dim sqlAtas As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' OR Type_Akun LIKE 'BANK' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlAtas, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbDebetKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using

            Dim sqlBawah As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'Piutang' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlBawah, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbKreditKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using


        ElseIf LblNamaTransaksi.Text = "PINDAH REKENING" Then
            Dim sqlAtas As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun NOT LIKE 'Laba Rugi' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlAtas, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbDebetKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using

            Dim sqlBawah As String = "SELECT Type_Akun, Nama_Akun FROM tbl_datareferensi WHERE Type_Akun NOT LIKE 'Laba Rugi' ORDER BY Kode_akun"
            Using cmd As New MySqlCommand(sqlBawah, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            Dim combinedValue As String = rd("Type_Akun").ToString() & " = " & rd("Nama_Akun").ToString()
                            CmbKreditKeuangan.Items.Add(combinedValue)
                        End While
                    End If
                End Using
            End Using
        End If


    End Sub
    Private Sub CmbDebetKeuangan_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbDebetKeuangan.SelectedIndexChanged
        Dim selectedData As String = CmbDebetKeuangan.SelectedItem.ToString()
        Dim parts() As String = selectedData.Split("="c)

        If parts.Length >= 2 Then
            Dim namaAkunD As String = parts(1).Trim()
            TxtDebetKeuanganNama.Text = namaAkunD

            Dim SQL As String = "SELECT KODE_AKUN FROM TBL_DATAREFERENSI WHERE NAMA_AKUN = @SELECTED_NAMA"
            Using cmd As New MySqlCommand(SQL, conn)
                cmd.Parameters.AddWithValue("@SELECTED_NAMA", namaAkunD)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        TxtDebetKeuangan.Text = reader("KODE_AKUN").ToString()
                    End If
                End Using
            End Using

        End If
        CmbKreditKeuangan.Focus()
    End Sub

    Private Sub CmbDebetKeuangan_TextUpdate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbDebetKeuangan.TextUpdate
        If BtnEditKeuangan.Visible = True Then
            Dim selectedData As String = CmbDebetKeuangan.SelectedItem.ToString()
            Dim parts() As String = selectedData.Split("="c)

            If parts.Length >= 2 Then
                Dim namaAkunD As String = parts(1).Trim()
                TxtDebetKeuanganNama.Text = namaAkunD

                Dim SQL As String = "SELECT KODE_AKUN FROM TBL_DATAREFERENSI WHERE NAMA_AKUN = @SELECTED_NAMA"
                Using cmd As New MySqlCommand(SQL, conn)
                    cmd.Parameters.AddWithValue("@SELECTED_NAMA", namaAkunD)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            reader.Read()
                            TxtDebetKeuangan.Text = reader("KODE_AKUN").ToString()
                        End If
                    End Using
                End Using

            End If
        End If
    End Sub

    Private Sub CmbKreditKeuangan_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbKreditKeuangan.SelectedIndexChanged
        Dim selectedData As String = CmbKreditKeuangan.SelectedItem.ToString()
        Dim parts() As String = selectedData.Split("="c)

        If parts.Length >= 2 Then
            Dim namaAkunK As String = parts(1).Trim()
            TxtKreditKeuanganNama.Text = namaAkunK

            Dim SQL As String = "SELECT KODE_AKUN FROM TBL_DATAREFERENSI WHERE NAMA_AKUN = @SELECTED_NAMA"
            Using cmd As New MySqlCommand(SQL, conn)
                cmd.Parameters.AddWithValue("@SELECTED_NAMA", namaAkunK)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        TxtKreditKeuangan.Text = reader("KODE_AKUN").ToString()
                    End If
                End Using
            End Using

        End If
        If CmbBantuDKeuangan.Visible = True Then
            CmbBantuDKeuangan.Focus()
        ElseIf CmbBantuKKeuangan.Visible = True Then
            CmbBantuKKeuangan.Focus()
        Else
            TxtNominalKeuangan.Focus()
        End If
    End Sub
    Private Sub CmbKreditKeuangan_TextUpdate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbKreditKeuangan.TextUpdate
        If BtnEditKeuangan.Visible = True Then
            Dim selectedData As String = CmbKreditKeuangan.SelectedItem.ToString()
            Dim parts() As String = selectedData.Split("="c)

            If parts.Length >= 2 Then
                Dim namaAkunK As String = parts(1).Trim()
                TxtKreditKeuanganNama.Text = namaAkunK

                Dim SQL As String = "SELECT KODE_AKUN FROM TBL_DATAREFERENSI WHERE NAMA_AKUN = @SELECTED_NAMA"

                Using cmd As New MySqlCommand(SQL, conn)
                    cmd.Parameters.AddWithValue("@SELECTED_NAMA", namaAkunK)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            reader.Read()
                            TxtKreditKeuangan.Text = reader("KODE_AKUN").ToString()
                        End If
                    End Using
                End Using
            End If
        End If
    End Sub

    Private Sub CmbBantuDKeuangan_KeyDown(sender As Object, e As KeyEventArgs) Handles CmbBantuDKeuangan.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            If CmbBantuKKeuangan.Visible = True Then
                CmbBantuKKeuangan.Focus()
            Else
                TxtNominalKeuangan.Focus()
            End If
        End If
    End Sub

    Private Sub CmbBantuKKeuangan_KeyDown(sender As Object, e As KeyEventArgs) Handles CmbBantuKKeuangan.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtNominalKeuangan.Focus()
        End If
    End Sub


    Public Sub DGVTAMPILDATAKEUANGAN()
        ' Definisikan tanggal awal dan akhir
        Dim TANGGAL_AWAL As Date = DTPTglKeuangan.Value.Date
        Dim TANGGAL_AKHIR As Date = TANGGAL_AWAL.AddDays(1).AddTicks(-1)
        Dim TOTAL_NOMINAL As Decimal = 0

        ' Kosongkan DataGridView dan buat ulang kolom
        DgvKeuangan.Columns.Clear()

        ' Tambahkan kolom tombol "EDIT" dan "HAPUS"
        Dim editButton As New DataGridViewButtonColumn With {
        .Name = "EDIT",
        .HeaderText = "EDIT",
        .Text = "Edit",
        .UseColumnTextForButtonValue = True
    }
        DgvKeuangan.Columns.Add(editButton)

        Dim deleteButton As New DataGridViewButtonColumn With {
        .Name = "HAPUS",
        .HeaderText = "HAPUS",
        .Text = "Hapus",
        .UseColumnTextForButtonValue = True
    }
        DgvKeuangan.Columns.Add(deleteButton)

        ' Tambahkan kolom berdasarkan data dari database
        Dim kolomDatabase As String() = {
        "NO_TRANSAKSI", "TGL_TRANSAKSI", "NO_NOTA", "URAIAN",
        "AKUN_D", "NAMA_AKUN_D", "NOMOR_AKUN_D",
        "AKUN_K", "NAMA_AKUN_K", "NOMOR_AKUN_K",
        "NAMA_BANTU_D", "KODE_BANTU_D",
        "NAMA_BANTU_K", "KODE_BANTU_K", "NOMINAL", "ID_USER"
    }

        For Each kolom As String In kolomDatabase
            DgvKeuangan.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = kolom,
            .HeaderText = kolom.Replace("_", " "), ' Header kolom lebih user-friendly
            .DataPropertyName = kolom
        })
        Next

        ' Ambil data dari database
        Using cmd As New MySqlCommand("
    SELECT NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D, 
           AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_D, KODE_BANTU_D, 
           NAMA_BANTU_K, KODE_BANTU_K, NOMINAL, ID_USER
    FROM jurnalumum
    WHERE TGL_TRANSAKSI BETWEEN @TANGGAL_AWAL AND @TANGGAL_AKHIR 
      AND JENIS_TRANSAKSI LIKE @JENIS_TRANSAKSI", conn)

            cmd.Parameters.AddWithValue("@TANGGAL_AWAL", TANGGAL_AWAL.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@TANGGAL_AKHIR", TANGGAL_AKHIR.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", LblNamaTransaksi.Text)


            Using rd As MySqlDataReader = cmd.ExecuteReader()
                DgvKeuangan.Rows.Clear()

                While rd.Read()
                    ' Tambahkan baris ke DataGridView
                    DgvKeuangan.Rows.Add("", "", rd("NO_TRANSAKSI"), rd("TGL_TRANSAKSI"), rd("NO_NOTA"), rd("URAIAN"),
                                     rd("AKUN_D"), rd("NAMA_AKUN_D"), rd("NOMOR_AKUN_D"),
                                     rd("AKUN_K"), rd("NAMA_AKUN_K"), rd("NOMOR_AKUN_K"),
                                     rd("NAMA_BANTU_D"), rd("KODE_BANTU_D"),
                                     rd("NAMA_BANTU_K"), rd("KODE_BANTU_K"), rd("NOMINAL"), rd("ID_USER"))

                    ' Akumulasi total nominal
                    TOTAL_NOMINAL += If(Not IsDBNull(rd("NOMINAL")), CDec(rd("NOMINAL")), 0)
                End While
            End Using
        End Using

        ' Atur properti tampilan kolom
        With DgvKeuangan
            .Columns("EDIT").DisplayIndex = 0
            .Columns("HAPUS").DisplayIndex = 1
            ' Mengatur lebar kolom Edit dan Hapus sesuai dengan isinya
            .Columns("EDIT").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            .Columns("HAPUS").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

            .Columns("TGL_TRANSAKSI").DefaultCellStyle.Format = "dd/MM/yyyy"
            .Columns("NOMINAL").DefaultCellStyle.Format = "N0"
            .Columns("NOMINAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            .Columns("NO_NOTA").Visible = False
            .Columns("AKUN_D").Visible = False
            .Columns("NOMOR_AKUN_D").Visible = False
            .Columns("AKUN_K").Visible = False
            .Columns("NOMOR_AKUN_K").Visible = False
            .Columns("NAMA_BANTU_D").Visible = False
            .Columns("KODE_BANTU_D").Visible = False
            .Columns("NAMA_BANTU_K").Visible = False
            .Columns("KODE_BANTU_K").Visible = False

            .ClearSelection()
        End With

        Dim JURNAL As Boolean() = ModulHakAkses.BacaHakAkses(FormUtama.SLevel.Text, "JURNAL", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnSimpanKeuangan.Visible = JURNAL(1) ' CanAdd 
        DgvKeuangan.Columns("EDIT").Visible = JURNAL(2) ' CanEdit 
        DgvKeuangan.Columns("HAPUS").Visible = JURNAL(3) ' CanDelete 

        ' Tampilkan total nominal
        LblTotalNominal.Text = "Total Nominal: Rp " & TOTAL_NOMINAL.ToString("N0")


        With DgvKeuangan
            ' Set header style
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            EnableDoubleBuffering(DgvKeuangan)
        End With
    End Sub

    ' Method to enable double buffering
    Public Shared Sub EnableDoubleBuffering(ByVal dgv As DataGridView)
        dgv.GetType().InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, dgv, New Object() {True})
    End Sub


    Private Sub DTPTglKeuangan_ValueChanged(sender As Object, e As EventArgs) Handles DTPTglKeuangan.ValueChanged
        Idkeuangan()
        DGVTAMPILDATAKEUANGAN()
        TxtUraianKeuangan.Focus()
        TxtUraianKeuangan.Select()
    End Sub

    Private Sub Idkeuangan()
        Dim Tanggal As String = Microsoft.VisualBasic.Format(DTPTglKeuangan.Value, "yyMMdd")
        Dim urutanKode As String = ""


        If LblNamaTransaksi.Text = "PEMASUKAN" Then
            Dim ceknomorMasuk As String = "MS-" & Tanggal

            Using cmd As New MySqlCommand("SELECT MAX(NO_TRANSAKSI) FROM JurnalUmum WHERE NO_TRANSAKSI LIKE @ceknomor", conn)
                cmd.Parameters.AddWithValue("@ceknomor", ceknomorMasuk & "%")

                ' Gunakan ExecuteScalar untuk mendapatkan nilai maksimum
                Dim maxKode As Object = cmd.ExecuteScalar()

                If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                    Dim MaxNilaiKode As String = maxKode.ToString()
                    If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "MS-" & Tanggal Then
                        ' Hitung nomor berikutnya
                        Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                        urutanKode = "MS-" & Tanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                    End If
                End If
            End Using

            ' Jika urutanKode masih kosong, buat nomor pertama
            If String.IsNullOrEmpty(urutanKode) Then
                urutanKode = "MS-" & Tanggal & "0001"
            End If


        ElseIf LblNamaTransaksi.Text = "PENGELUARAN" Then
            Dim ceknomorMasuk As String = "KL-" & Tanggal

            Using cmd As New MySqlCommand("SELECT MAX(NO_TRANSAKSI) FROM JurnalUmum WHERE NO_TRANSAKSI LIKE @ceknomor", conn)
                cmd.Parameters.AddWithValue("@ceknomor", ceknomorMasuk & "%")

                ' Gunakan ExecuteScalar untuk mendapatkan nilai MAX(NO_TRANSAKSI)
                Dim maxKode As Object = cmd.ExecuteScalar()

                ' Pastikan nilai yang dikembalikan bukan NULL atau DBNull
                If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                    Dim MaxNilaiKode As String = maxKode.ToString()

                    ' Periksa apakah kode transaksi yang ditemukan sesuai dengan format yang diinginkan
                    If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "KL-" & Tanggal Then
                        ' Hitung nomor transaksi berikutnya
                        Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                        urutanKode = "KL-" & Tanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                    End If
                End If
            End Using

            ' Jika urutanKode masih kosong, buat nomor pertama
            If String.IsNullOrEmpty(urutanKode) Then
                urutanKode = "KL-" & Tanggal & "0001"
            End If


        ElseIf LblNamaTransaksi.Text = "BIAYA" Then
            Dim ceknomorMasuk As String = "BY-" & Tanggal

            Using cmd As New MySqlCommand("SELECT MAX(NO_TRANSAKSI) FROM JurnalUmum WHERE NO_TRANSAKSI LIKE @ceknomor", conn)
                cmd.Parameters.AddWithValue("@ceknomor", ceknomorMasuk & "%")

                ' Gunakan ExecuteScalar untuk mendapatkan nilai MAX(NO_TRANSAKSI)
                Dim maxKode As Object = cmd.ExecuteScalar()

                ' Pastikan nilai yang dikembalikan bukan NULL atau DBNull
                If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                    Dim MaxNilaiKode As String = maxKode.ToString()

                    ' Periksa apakah kode transaksi yang ditemukan sesuai dengan format yang diinginkan
                    If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "BY-" & Tanggal Then
                        ' Hitung nomor transaksi berikutnya
                        Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                        urutanKode = "BY-" & Tanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                    End If
                End If
            End Using

            ' Jika urutanKode masih kosong, buat nomor pertama
            If String.IsNullOrEmpty(urutanKode) Then
                urutanKode = "BY-" & Tanggal & "0001"
            End If


        ElseIf LblNamaTransaksi.Text = "SETOR KE BOS" Then
            Dim ceknomorMasuk As String = "SB-" & Tanggal

            Using cmd As New MySqlCommand("SELECT MAX(NO_TRANSAKSI) FROM JurnalUmum WHERE NO_TRANSAKSI LIKE @ceknomor", conn)
                cmd.Parameters.AddWithValue("@ceknomor", ceknomorMasuk & "%")

                ' Gunakan ExecuteScalar untuk mendapatkan nilai MAX(NO_TRANSAKSI)
                Dim maxKode As Object = cmd.ExecuteScalar()

                ' Pastikan nilai yang dikembalikan bukan NULL atau DBNull
                If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                    Dim MaxNilaiKode As String = maxKode.ToString()

                    ' Periksa apakah kode transaksi yang ditemukan sesuai dengan format yang diinginkan
                    If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "SB-" & Tanggal Then
                        ' Hitung nomor transaksi berikutnya
                        Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                        urutanKode = "SB-" & Tanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                    End If
                End If
            End Using

            ' Jika urutanKode masih kosong, buat nomor pertama
            If String.IsNullOrEmpty(urutanKode) Then
                urutanKode = "SB-" & Tanggal & "0001"
            End If


        ElseIf LblNamaTransaksi.Text = "BAYAR BON PRIBADI" Then
            Dim ceknomorMasuk As String = "BB-" & Tanggal

            Using cmd As New MySqlCommand("SELECT MAX(NO_TRANSAKSI) FROM JurnalUmum WHERE NO_TRANSAKSI LIKE @ceknomor", conn)
                cmd.Parameters.AddWithValue("@ceknomor", ceknomorMasuk & "%")

                ' Gunakan ExecuteScalar untuk mendapatkan nilai MAX(NO_TRANSAKSI)
                Dim maxKode As Object = cmd.ExecuteScalar()

                ' Pastikan nilai yang dikembalikan bukan NULL atau DBNull
                If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                    Dim MaxNilaiKode As String = maxKode.ToString()

                    ' Periksa apakah kode transaksi yang ditemukan sesuai dengan format yang diinginkan
                    If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "BB-" & Tanggal Then
                        ' Hitung nomor transaksi berikutnya
                        Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                        urutanKode = "BB-" & Tanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                    End If
                End If
            End Using

            ' Jika urutanKode masih kosong, buat nomor pertama
            If String.IsNullOrEmpty(urutanKode) Then
                urutanKode = "BB-" & Tanggal & "0001"
            End If


        ElseIf LblNamaTransaksi.Text = "PINDAH REKENING" Then
            Dim ceknomorMasuk As String = "PR-" & Tanggal

            Using cmd As New MySqlCommand("SELECT MAX(NO_TRANSAKSI) FROM JurnalUmum WHERE NO_TRANSAKSI LIKE @ceknomor", conn)
                cmd.Parameters.AddWithValue("@ceknomor", ceknomorMasuk & "%")

                ' Gunakan ExecuteScalar untuk mendapatkan nilai MAX(NO_TRANSAKSI)
                Dim maxKode As Object = cmd.ExecuteScalar()

                ' Pastikan nilai yang dikembalikan bukan NULL atau DBNull
                If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                    Dim MaxNilaiKode As String = maxKode.ToString()

                    ' Periksa apakah kode transaksi yang ditemukan sesuai dengan format yang diinginkan
                    If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "PR-" & Tanggal Then
                        ' Hitung nomor transaksi berikutnya
                        Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                        urutanKode = "PR-" & Tanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                    End If
                End If
            End Using

            ' Jika urutanKode masih kosong, buat nomor pertama
            If String.IsNullOrEmpty(urutanKode) Then
                urutanKode = "PR-" & Tanggal & "0001"
            End If


        End If

        LblIdBayar.Text = urutanKode
    End Sub

    Private Sub TxtNominalMasuk_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNominalKeuangan.TextChanged
        Dim nominalMasuk As Double
        If Double.TryParse(TxtNominalKeuangan.Text, nominalMasuk) Then
            LblNominalKeuangan.Text = "Rp. " & nominalMasuk.ToString("N0")
        Else
            LblNominalKeuangan.Text = "Rp. 0"
            TxtNominalKeuangan.Text = ""
            TxtNominalKeuangan.Focus()
        End If
    End Sub


    Private Sub DgvKeuangan_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DgvKeuangan.CellContentClick
        Try
            ' Pastikan indeks kolom valid dan klik pada baris data
            If e.RowIndex >= 0 AndAlso e.ColumnIndex = DgvKeuangan.Columns("EDIT").Index Then
                ' Tampilkan tombol yang sesuai untuk mode edit
                BtnSimpanKeuangan.Visible = False
                BtnEditKeuangan.Visible = True
                BtnBatalKeuangan.Visible = True

                ' Validasi apakah data di baris tersebut lengkap sebelum dimuat ke form
                Dim selectedRow As DataGridViewRow = DgvKeuangan.Rows(e.RowIndex)

                ' Memastikan setiap sel memiliki nilai sebelum digunakan
                LblIdBayar.Text = If(selectedRow.Cells("NO_TRANSAKSI").Value IsNot Nothing, selectedRow.Cells("NO_TRANSAKSI").Value.ToString(), String.Empty)
                TxtNoNota.Text = If(selectedRow.Cells("NO_NOTA").Value IsNot Nothing, selectedRow.Cells("NO_NOTA").Value.ToString(), String.Empty)
                TxtUraianKeuangan.Text = If(selectedRow.Cells("URAIAN").Value IsNot Nothing, selectedRow.Cells("URAIAN").Value.ToString(), String.Empty)
                CmbDebetKeuangan.Text = If(selectedRow.Cells("AKUN_D").Value IsNot Nothing, selectedRow.Cells("AKUN_D").Value.ToString(), String.Empty)
                CmbKreditKeuangan.Text = If(selectedRow.Cells("AKUN_K").Value IsNot Nothing, selectedRow.Cells("AKUN_K").Value.ToString(), String.Empty)
                CmbBantuDKeuangan.Text = If(selectedRow.Cells("NAMA_BANTU_D").Value IsNot Nothing, selectedRow.Cells("NAMA_BANTU_D").Value.ToString(), String.Empty)
                CmbBantuKKeuangan.Text = If(selectedRow.Cells("NAMA_BANTU_K").Value IsNot Nothing, selectedRow.Cells("NAMA_BANTU_K").Value.ToString(), String.Empty)

                ' Memastikan nilai nominal adalah angka yang valid
                Dim nominal As Decimal
                If Decimal.TryParse(selectedRow.Cells("NOMINAL").Value?.ToString(), nominal) Then
                    TxtNominalKeuangan.Text = nominal.ToString("N0") ' Format angka
                Else
                    TxtNominalKeuangan.Text = "0"
                End If
            End If
        Catch ex As Exception
            ' Menangani kesalahan dengan menampilkan pesan
            MessageBox.Show("Terjadi kesalahan saat memuat data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try



        If e.ColumnIndex = DgvKeuangan.Columns("HAPUS").Index AndAlso e.RowIndex >= 0 Then
            Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin akan menghapus data ini?", "Hapus Data", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                Dim kode As String = DgvKeuangan.Rows(e.RowIndex).Cells("NO_TRANSAKSI").Value.ToString()

                Using cmd As New MySqlCommand("DELETE FROM JurnalUmum WHERE NO_TRANSAKSI=@Kode", conn)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    cmd.ExecuteNonQuery()
                End Using

                Kondisisetalhproseskeuangan()
            End If
        End If

    End Sub

    Private Sub BtnBatalKeuangan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBatalKeuangan.Click
        Kondisisetalhproseskeuangan()
    End Sub


    Private Sub BtnSimpanKeuangan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpanKeuangan.Click
        If ValidasiInput() Then
            Try
                Dim TANGGAL As String = DTPTglKeuangan.Value.ToString("yyyy-MM-dd HH:mm:ss")

                Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D, AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_D, KODE_BANTU_D, NAMA_BANTU_K, KODE_BANTU_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                 "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @AKUN_D, @NAMA_AKUN_D, @NOMOR_AKUN_D, @AKUN_K, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NAMA_BANTU_D, @KODE_BANTU_D, @NAMA_BANTU_K, @KODE_BANTU_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn)

                    cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblIdBayar.Text)
                    cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", TANGGAL)
                    cmd.Parameters.AddWithValue("@NO_NOTA", TxtNoNota.Text)
                    cmd.Parameters.AddWithValue("@URAIAN", TxtUraianKeuangan.Text)
                    cmd.Parameters.AddWithValue("@AKUN_D", CmbDebetKeuangan.Text)
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_D", TxtDebetKeuanganNama.Text)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", TxtDebetKeuangan.Text)
                    cmd.Parameters.AddWithValue("@AKUN_K", CmbKreditKeuangan.Text)
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_K", TxtKreditKeuanganNama.Text)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", TxtKreditKeuangan.Text)
                    cmd.Parameters.AddWithValue("@NAMA_BANTU_D", CmbBantuDKeuangan.Text)
                    cmd.Parameters.AddWithValue("@KODE_BANTU_D", TxtBantuDKeuangan.Text)
                    cmd.Parameters.AddWithValue("@NAMA_BANTU_K", CmbBantuKKeuangan.Text)
                    cmd.Parameters.AddWithValue("@KODE_BANTU_K", TxtBantuKKeuangan.Text)
                    cmd.Parameters.AddWithValue("@NOMINAL", If(Decimal.TryParse(TxtNominalKeuangan.Text, New Decimal()), Decimal.Parse(TxtNominalKeuangan.Text), 0D))
                    cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", LblNamaTransaksi.Text)
                    cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

                    cmd.ExecuteNonQuery()

                    Dim transaksiText As String = ""

                    Select Case LblNamaTransaksi.Text
                        Case "PEMASUKAN"
                            transaksiText = "PEMASUKAN"
                        Case "PENGELUARAN"
                            transaksiText = "PENGELUARAN"
                        Case "BIAYA"
                            transaksiText = "BIAYA"
                        Case "SETOR KE BOS"
                            transaksiText = "SETOR KE BOS"
                        Case "BAYAR BON PRIBADI"
                            transaksiText = "BAYAR BON PRIBADI"
                        Case "PINDAH REKENING"
                            transaksiText = "PINDAH REKENING"
                    End Select

                End Using


                Kondisisetalhproseskeuangan()
            Catch ex As Exception
                MessageBox.Show("Terjadi kesalahan saat menyimpan data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

    End Sub


    Private Sub BtnEditKeuangan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnEditKeuangan.Click
        If ValidasiInput() Then
            Try
                Dim TANGGAL As String = DTPTglKeuangan.Value.ToString("yyyy-MM-dd HH:mm:ss")

                Using cmd As New MySqlCommand("UPDATE JurnalUmum SET TGL_TRANSAKSI = @TGL_TRANSAKSI, NO_NOTA = @NO_NOTA, URAIAN = @URAIAN, AKUN_D = @AKUN_D, NAMA_AKUN_D = @NAMA_AKUN_D, NOMOR_AKUN_D = @NOMOR_AKUN_D, AKUN_K = @AKUN_K, NAMA_AKUN_K = @NAMA_AKUN_K, NOMOR_AKUN_K = @NOMOR_AKUN_K, NAMA_BANTU_D = @NAMA_BANTU_D, KODE_BANTU_D = @KODE_BANTU_D, NAMA_BANTU_K = @NAMA_BANTU_K, KODE_BANTU_K = @KODE_BANTU_K, NOMINAL = @NOMINAL, JENIS_TRANSAKSI = @JENIS_TRANSAKSI, LOKASI = @LOKASI, ID_USER = @ID_USER, ID_KOMPUTER = @ID_KOMPUTER WHERE NO_TRANSAKSI = @NO_TRANSAKSI", conn)

                    cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", TANGGAL)
                    cmd.Parameters.AddWithValue("@NO_NOTA", TxtNoNota.Text)
                    cmd.Parameters.AddWithValue("@URAIAN", TxtUraianKeuangan.Text)
                    cmd.Parameters.AddWithValue("@AKUN_D", CmbDebetKeuangan.Text)
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_D", TxtDebetKeuanganNama.Text)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", TxtDebetKeuangan.Text)
                    cmd.Parameters.AddWithValue("@AKUN_K", CmbKreditKeuangan.Text)
                    cmd.Parameters.AddWithValue("@NAMA_AKUN_K", TxtKreditKeuanganNama.Text)
                    cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", TxtKreditKeuangan.Text)
                    cmd.Parameters.AddWithValue("@NAMA_BANTU_D", CmbBantuDKeuangan.Text)
                    cmd.Parameters.AddWithValue("@KODE_BANTU_D", TxtBantuDKeuangan.Text)
                    cmd.Parameters.AddWithValue("@NAMA_BANTU_K", CmbBantuKKeuangan.Text)
                    cmd.Parameters.AddWithValue("@KODE_BANTU_K", TxtBantuKKeuangan.Text)
                    cmd.Parameters.AddWithValue("@NOMINAL", If(Decimal.TryParse(TxtNominalKeuangan.Text, New Decimal()), Decimal.Parse(TxtNominalKeuangan.Text), 0D))
                    cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", LblNamaTransaksi.Text)
                    cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
                    cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblIdBayar.Text)

                    cmd.ExecuteNonQuery()
                End Using



                Dim transaksiText As String = ""

                Select Case LblNamaTransaksi.Text
                    Case "PEMASUKAN"
                        transaksiText = "EDIT PEMASUKAN"
                    Case "PENGELUARAN"
                        transaksiText = "EDIT PENGELUARAN"
                    Case "BIAYA"
                        transaksiText = "EDIT BIAYA"
                    Case "SETOR KE BOS"
                        transaksiText = "EDIT SETOR KE BOS"
                    Case "BAYAR BON PRIBADI"
                        transaksiText = "EDIT BAYAR BON PRIBADI"
                    Case "PINDAH REKENING"
                        transaksiText = "EDIT PINDAH REKENING"
                End Select



                Kondisisetalhproseskeuangan()
            Catch ex As Exception
                MessageBox.Show("Terjadi kesalahan saat mengedit data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

    End Sub

    Private Function ValidasiInput() As Boolean

        If String.IsNullOrWhiteSpace(TxtUraianKeuangan.Text) Then
            MessageBox.Show("Uraian harus diisi.")
            TxtUraianKeuangan.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(CmbDebetKeuangan.Text) Then
            MessageBox.Show("Akun Debet harus dipilih.")
            CmbDebetKeuangan.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(CmbKreditKeuangan.Text) Then
            MessageBox.Show("Akun Debet harus dipilih.")
            CmbKreditKeuangan.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(CmbBantuDKeuangan.Text) AndAlso CmbBantuDKeuangan.Visible = True Then
            MessageBox.Show("Akun Debet harus dipilih.")
            CmbBantuDKeuangan.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(CmbBantuKKeuangan.Text) AndAlso CmbBantuKKeuangan.Visible = True Then
            MessageBox.Show("Akun Debet harus dipilih.")
            CmbBantuKKeuangan.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(TxtNominalKeuangan.Text) Then
            MessageBox.Show("Uraian harus diisi.")
            TxtNominalKeuangan.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub BTNKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTNKeluar.Click
        Close()
    End Sub


End Class