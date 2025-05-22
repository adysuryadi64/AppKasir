Imports System.IO
Imports System.Text.Json

Public Module DatabaseModule
    Public conn As MySqlConnection
    Public cmd As MySqlCommand
    Public rd As MySqlDataReader
    Public dt As New DataTable
    Public da As MySqlDataAdapter
    Public ds As DataSet
    Public transaction As MySqlTransaction
    Public koneksiDatabase As Boolean
    Public cultureIndonesia As New Globalization.CultureInfo("id-ID")


    Public ReadOnly configFilePath As String = Path.Combine(Application.StartupPath, "database.json")


    Public Function OpenConnection(Optional useSSL As Boolean = False) As Boolean
        Try
            ' Cek apakah koneksi sudah terbuka
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                Return True ' Koneksi sudah terbuka
            End If

            ' Cek apakah file konfigurasi ada
            If Not File.Exists(configFilePath) Then
                Throw New FileNotFoundException("File konfigurasi tidak ditemukan.")
            End If

            ' Deserialisasi konfigurasi dari file biner
            Using stream As New FileStream(configFilePath, FileMode.Open, FileAccess.Read)
                Dim json As String = File.ReadAllText(configFilePath)
                Dim konfigurasi As DatabaseConfiguration = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)
                konfigurasi.Password = DecryptPassword(konfigurasi.Password)

                ' Ambil informasi koneksi dari konfigurasi
                Dim server As String = konfigurasi.Server
                Dim port As Integer = If(Integer.TryParse(konfigurasi.Port, Nothing), CInt(konfigurasi.Port), 3306)
                Dim database As String = konfigurasi.Database
                Dim user As String = konfigurasi.User
                Dim password As String = konfigurasi.Password
                Dim sslMode As String = If(useSSL, "Preferred", "None") ' Gunakan SSL hanya jika diperlukan

                ' Buat connection string
                Dim connectionString As String = $"Server={server};Port={port};Database={database};User ID={user};Password={password};SslMode={sslMode};charset=utf8mb4;"

                ' Buat koneksi baru
                conn = New MySqlConnection(connectionString)
                conn.Open()

                ' Cek apakah koneksi berhasil
                If conn.State = ConnectionState.Open Then
                    Return True
                Else
                    HandleConnectionError("Koneksi gagal.")
                    Return False
                End If
            End Using

        Catch ex As MySqlException When ex.Number = 1042
            ' Kesalahan jaringan (host tidak ditemukan)
            HandleConnectionError("Koneksi ke server gagal. Pastikan server dapat dijangkau.")
            Return False
        Catch ex As MySqlException When ex.Number = 1045
            ' Kesalahan login (user/password salah)
            HandleConnectionError("User atau password salah.")
            Return False
        Catch ex As Exception
            ' Kesalahan umum lainnya
            HandleConnectionError(ex.Message)
            Return False
        End Try
    End Function

    Private Sub HandleConnectionError(ByVal errorMessage As String)
        ' Tampilkan pesan error
        MessageBox.Show(String.Format("Terjadi kesalahan di Inisialisasi Koneksi: {0}", errorMessage), "Gagal Inisialisasi Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Error)

        ' Pastikan koneksi ditutup saat terjadi error
        CloseConnection()

        ' Cek apakah form SettingDatabase sudah terbuka
        For Each f As Form In Application.OpenForms
            If TypeOf f Is SettingDatabase Then
                ' Jika sudah terbuka, keluar dari sub
                Return
            End If
        Next

        ' Jika belum terbuka, tampilkan form SettingDatabase
        SettingDatabase.ShowDialog()
    End Sub

    Public Sub CloseConnection()
        ' Pastikan koneksi ditutup dengan benar jika masih terbuka
        If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
            conn.Close()
        End If
    End Sub


    <Serializable()>
    Public Class DatabaseConfiguration
        Public Property Server As String
        Public Property Port As String
        Public Property User As String
        Public Property Password As String
        Public Property Database As String
    End Class




    Private ReadOnly akunSet As New HashSet(Of String)()

    Public Sub Rekeningkasbank()
        Dim namaakun As String = "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' OR Type_Akun LIKE 'BANK' ORDER BY Kode_akun ASC"

        Using cmd As New MySqlCommand(namaakun, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        Dim combinedValue As String = rd("Nama_Akun").ToString()

                        ' HashSet otomatis mencegah duplikasi
                        akunSet.Add(combinedValue)
                    End While
                End If
            End Using
        End Using
    End Sub

    Public Function GetAkunList() As List(Of String)
        ' Gunakan konstruktor List untuk konversi
        Return New List(Of String)(akunSet)
    End Function


    Private ReadOnly daftarAkun As New HashSet(Of String)()

    Public Sub AmbilAkunKasBankEkuitas()
        Dim queryAkun As String = "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' OR Type_Akun LIKE 'BANK' OR Type_Akun LIKE 'EKUITAS' ORDER BY Kode_akun ASC"

        Using cmd As New MySqlCommand(queryAkun, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        Dim namaAkun As String = rd("Nama_Akun").ToString()

                        ' HashSet otomatis mencegah duplikasi
                        daftarAkun.Add(namaAkun)
                    End While
                End If
            End Using
        End Using
    End Sub

    Public Function GetDaftarAkun() As List(Of String)
        ' Gunakan konstruktor List untuk konversi
        Return New List(Of String)(daftarAkun)
    End Function

    Public KODE_PERUSAHAAN As String = ""
    Public NAMA_PERUSAHAAN As String = ""
    Public ALAMAT_PERUSAHAAN As String = ""
    Public KOTA_PERUSAHAAN As String = ""
    Public KONTAK_PERUSAHAAN As String = ""
    Public PEMILIK_PERUSAHAAN As String = ""
    Public FOOTER1 As String = ""
    Public FOOTER2 As String = ""
    Public JENIS_TUTUP_BULAN As String = ""
    Public TANGGAL_TUTUP_BULAN As Integer = 1

    Public KODE_REK_BARANG As String = ""
    Public NAMA_REK_BARANG As String = ""
    Public LAWAN_KODE_REK_BARANG As String = ""
    Public LAWAN_NAMA_REK_BARANG As String = ""
    Public Kode_rek_Beli_toko As String = ""
    Public nama_rek_Beli_toko As String = ""
    Public Kode_rek_Beli_Gudang As String = ""
    Public nama_rek_Beli_Gudang As String = ""
    Public Kode_rek_Jual_Toko As String = ""
    Public nama_rek_Jual_Toko As String = ""
    Public Kode_rek_Jual_Gudang As String = ""
    Public nama_rek_Jual_Gudang As String = ""
    Public Kode_rek_Hutang_Beli As String = ""
    Public nama_rek_Hutang_Beli As String = ""
    Public Kode_rek_Piutang_Jual As String = ""
    Public nama_rek_Piutang_Jual As String = ""


    Public Sub AmbilDataMasterPerusahaan()
        Dim sql As String = "SELECT KODE, NAMA, ALAMAT, KOTA, HP, PEMILIK, FOOTER1, FOOTER2, System_tutup_bulan, Tanggal_Tutup_bulan, " &
                            "KODE_REK_BARANG, NAMA_REK_BARANG, lawan_nama_rek_barang, lawan_Kode_rek_barang, " &
                            "Kode_rek_Beli_toko, nama_rek_Beli_toko, Kode_rek_Beli_Gudang, nama_rek_Beli_Gudang, " &
                            "Kode_rek_Jual_Toko, nama_rek_Jual_Toko, Kode_rek_Jual_Gudang, nama_rek_Jual_Gudang, " &
                            "nama_rek_Hutang_Beli, Kode_rek_Hutang_Beli, nama_rek_Piutang_Jual, Kode_rek_Piutang_Jual " &
                            "FROM tbl_perusahaan"

        Using cmd As New MySqlCommand(sql, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Set nilai ke variabel-variabel perusahaan
                    KODE_PERUSAHAAN = reader("KODE").ToString()
                    NAMA_PERUSAHAAN = reader("NAMA").ToString()
                    ALAMAT_PERUSAHAAN = reader("ALAMAT").ToString()
                    KOTA_PERUSAHAAN = reader("KOTA").ToString()
                    KONTAK_PERUSAHAAN = reader("HP").ToString()
                    PEMILIK_PERUSAHAAN = reader("PEMILIK").ToString()
                    FOOTER1 = reader("FOOTER1").ToString()
                    FOOTER2 = reader("FOOTER2").ToString()
                    JENIS_TUTUP_BULAN = reader("System_tutup_bulan").ToString()
                    TANGGAL_TUTUP_BULAN = reader("Tanggal_Tutup_bulan").ToString()
                    KODE_REK_BARANG = reader("KODE_REK_BARANG").ToString()
                    NAMA_REK_BARANG = reader("NAMA_REK_BARANG").ToString()
                    LAWAN_KODE_REK_BARANG = reader("lawan_Kode_rek_barang").ToString()
                    LAWAN_NAMA_REK_BARANG = reader("lawan_nama_rek_barang").ToString()
                    Kode_rek_Beli_toko = reader("Kode_rek_Beli_toko").ToString()
                    nama_rek_Beli_toko = reader("nama_rek_Beli_toko").ToString()
                    Kode_rek_Beli_Gudang = reader("Kode_rek_Beli_Gudang").ToString()
                    nama_rek_Beli_Gudang = reader("nama_rek_Beli_Gudang").ToString()
                    Kode_rek_Jual_Toko = reader("Kode_rek_Jual_Toko").ToString()
                    nama_rek_Jual_Toko = reader("nama_rek_Jual_Toko").ToString()
                    Kode_rek_Jual_Gudang = reader("Kode_rek_Jual_Gudang").ToString()
                    nama_rek_Jual_Gudang = reader("nama_rek_Jual_Gudang").ToString()
                    Kode_rek_Hutang_Beli = reader("Kode_rek_Hutang_Beli").ToString()
                    nama_rek_Hutang_Beli = reader("nama_rek_Hutang_Beli").ToString()
                    Kode_rek_Piutang_Jual = reader("Kode_rek_Piutang_Jual").ToString()
                    nama_rek_Piutang_Jual = reader("nama_rek_Piutang_Jual").ToString()

                    ' Mengisi data ke form yang relevan
                    FormUtama.Text = "KASIR LANCAR " & FormUtama.SLokasi.Text & " " & NAMA_PERUSAHAAN

                    FormStokOpnameBahan.TxtPerusahaan.Text = NAMA_PERUSAHAAN
                    FormLapTransferStok.TxtPerusahaan.Text = NAMA_PERUSAHAAN
                    FormHistory.TxtPerusahaan.Text = NAMA_PERUSAHAAN

                    AmbilDataPeriodeTanggal()
                End If
            End Using
        End Using
    End Sub

    ' Deklarasi variabel tanggal awal dan akhir sebagai public
    Public tanggalAwalPeriodeKerja As Date
    Public tanggalAkhirPeriodeKerja As Date

    Public Sub AmbilDataPeriodeTanggal()
        ' Cek apakah TANGGAL_TUTUP_BULAN dapat dikonversi menjadi Integer
        Dim tanggalInput As Integer
        If Integer.TryParse(TANGGAL_TUTUP_BULAN.ToString(), tanggalInput) Then
            ' Dapatkan bulan dan tahun sekarang
            Dim bulanSekarang As Integer = DateTime.Now.Month ' Mendapatkan bulan sekarang
            Dim tahunSekarang As Integer = DateTime.Now.Year ' Mendapatkan tahun sekarang

            ' Tentukan tanggal awal dan akhir
            Dim tanggalAwal As Date
            Dim tanggalAkhir As Date

            If JENIS_TUTUP_BULAN = "Berdasar tanggal manual" Then
                ' Jika hari ini lebih besar dari tanggal input (10)
                If DateTime.Now.Day > tanggalInput Then
                    ' Tentukan tanggal awal dan akhir untuk periode berikutnya
                    tanggalAwal = New Date(tahunSekarang, bulanSekarang, tanggalInput + 1) ' 11 bulan ini
                    tanggalAkhir = tanggalAwal.AddMonths(1).AddDays(-1) ' 10 bulan depan
                Else
                    ' Tentukan tanggal awal dan akhir untuk periode sebelumnya
                    If bulanSekarang = 1 Then
                        ' Jika bulan Januari, tanggal awal adalah 11 Desember tahun sebelumnya
                        tanggalAwal = New Date(tahunSekarang - 1, 12, tanggalInput + 1)
                        tanggalAkhir = tanggalAwal.AddMonths(1).AddDays(-1) ' 10 Januari
                    Else
                        ' Jika tidak Januari, tanggal awal adalah 11 bulan sebelumnya
                        tanggalAwal = New Date(tahunSekarang, bulanSekarang - 1, tanggalInput + 1)
                        tanggalAkhir = tanggalAwal.AddMonths(1).AddDays(-1) ' 10 bulan depan
                    End If
                End If

            Else
                ' Berdasar bulan saat ini
                tanggalAwal = New Date(tahunSekarang, bulanSekarang, 1) ' Tanggal pertama bulan ini
                tanggalAkhir = tanggalAwal.AddMonths(1).AddDays(-1) ' Tanggal terakhir bulan ini

                ' Cek apakah tanggal input lebih besar dari hari ini pada bulan ini
                If tanggalInput > DateTime.Now.Day Then
                    ' Jika tanggal input lebih besar, atur tanggal akhir ke akhir bulan ini
                    tanggalAkhir = New Date(tahunSekarang, bulanSekarang, 1).AddMonths(1).AddDays(-1)
                End If
            End If

            ' Menetapkan nilai ke variabel public
            tanggalAwalPeriodeKerja = tanggalAwal
            tanggalAkhirPeriodeKerja = tanggalAkhir

        Else
            ' Menangani jika TANGGAL_TUTUP_BULAN tidak valid
            MessageBox.Show("Tanggal tutup bulan tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub


    Public Sub HitungSemuaKode()
        Using transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                Dim query As String =
                "UPDATE tbl_barang SET " &
                "STOK_TOKO = COALESCE(AWAL_TOKO, 0) " &
                "+ COALESCE(TAMBAH_TOKO, 0) " &
                "- COALESCE(KURANG_TOKO, 0) " &
                "+ COALESCE(PEMBELIAN_TOKO, 0) " &
                "- COALESCE(PENJUALAN_TOKO, 0) " &
                "- COALESCE(RETUR_BELI_TOKO, 0) " &
                "+ COALESCE(RETUR_JUAL_TOKO, 0) " &
                "+ COALESCE(OPNAME_TOKO, 0) " &
                "+ COALESCE(TRANSFER_STOK_MASUK_TOKO, 0) " &
                "- COALESCE(TRANSFER_STOK_KELUAR_TOKO, 0) " &
                "+ COALESCE(TRANSFER_BARANG_MASUK_TOKO, 0) " &
                "- COALESCE(TRANSFER_BARANG_KELUAR_TOKO, 0), " &
                "STOK_GUDANG = COALESCE(AWAL_GUDANG, 0) " &
                "+ COALESCE(TAMBAH_GUDANG, 0) " &
                "- COALESCE(KURANG_GUDANG, 0) " &
                "+ COALESCE(PEMBELIAN_GUDANG, 0) " &
                "- COALESCE(PENJUALAN_GUDANG, 0) " &
                "- COALESCE(RETUR_BELI_GUDANG, 0) " &
                "+ COALESCE(RETUR_JUAL_GUDANG, 0) " &
                "+ COALESCE(OPNAME_GUDANG, 0) " &
                "+ COALESCE(TRANSFER_STOK_MASUK_GUDANG, 0) " &
                "- COALESCE(TRANSFER_STOK_KELUAR_GUDANG, 0) " &
                "+ COALESCE(TRANSFER_BARANG_MASUK_GUDANG, 0) " &
                "- COALESCE(TRANSFER_BARANG_KELUAR_GUDANG, 0)"

                Using command As New MySqlCommand(query, conn, transaction)
                    command.ExecuteNonQuery()
                End Using

                transaction.Commit() ' Commit transaksi jika berhasil
            Catch ex As Exception
                transaction.Rollback() ' Rollback jika terjadi error
                ' Tampilkan pesan kesalahan
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub


    Public Sub HitungByKode(ByVal kode As String)
        Using transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                Dim query As String =
                "UPDATE tbl_barang SET " &
                "STOK_TOKO = COALESCE(AWAL_TOKO, 0) " &
                "+ COALESCE(TAMBAH_TOKO, 0) " &
                "- COALESCE(KURANG_TOKO, 0) " &
                "+ COALESCE(PEMBELIAN_TOKO, 0) " &
                "- COALESCE(PENJUALAN_TOKO, 0) " &
                "- COALESCE(RETUR_BELI_TOKO, 0) " &
                "+ COALESCE(RETUR_JUAL_TOKO, 0) " &
                "+ COALESCE(OPNAME_TOKO, 0) " &
                "+ COALESCE(TRANSFER_STOK_MASUK_TOKO, 0) " &
                "- COALESCE(TRANSFER_STOK_KELUAR_TOKO, 0) " &
                "+ COALESCE(TRANSFER_BARANG_MASUK_TOKO, 0) " &
                "- COALESCE(TRANSFER_BARANG_KELUAR_TOKO, 0), " &
                "STOK_GUDANG = COALESCE(AWAL_GUDANG, 0) " &
                "+ COALESCE(TAMBAH_GUDANG, 0) " &
                "- COALESCE(KURANG_GUDANG, 0) " &
                "+ COALESCE(PEMBELIAN_GUDANG, 0) " &
                "- COALESCE(PENJUALAN_GUDANG, 0) " &
                "- COALESCE(RETUR_BELI_GUDANG, 0) " &
                "+ COALESCE(RETUR_JUAL_GUDANG, 0) " &
                "+ COALESCE(OPNAME_GUDANG, 0) " &
                "+ COALESCE(TRANSFER_STOK_MASUK_GUDANG, 0) " &
                "- COALESCE(TRANSFER_STOK_KELUAR_GUDANG, 0) " &
                "+ COALESCE(TRANSFER_BARANG_MASUK_GUDANG, 0) " &
                "- COALESCE(TRANSFER_BARANG_KELUAR_GUDANG, 0) " &
                "WHERE ID_BARANG = @Kode"

                Using command As New MySqlCommand(query, conn, transaction)
                    command.Parameters.AddWithValue("@Kode", kode)
                    command.ExecuteNonQuery()
                End Using

                transaction.Commit() ' Commit transaksi jika berhasil
            Catch ex As Exception
                transaction.Rollback() ' Rollback jika terjadi error
                ' Tampilkan pesan kesalahan
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub


    Public Sub HitungStokPerubahan(ByVal kode As String, ByVal transaction As MySqlTransaction)
        Dim query As String =
            "UPDATE tbl_barang SET " &
            "STOK_TOKO = COALESCE(AWAL_TOKO, 0) " &
            "+ COALESCE(TAMBAH_TOKO, 0) " &
            "- COALESCE(KURANG_TOKO, 0) " &
            "+ COALESCE(PEMBELIAN_TOKO, 0) " &
            "- COALESCE(PENJUALAN_TOKO, 0) " &
            "- COALESCE(RETUR_BELI_TOKO, 0) " &
            "+ COALESCE(RETUR_JUAL_TOKO, 0) " &
            "+ COALESCE(OPNAME_TOKO, 0) " &
            "+ COALESCE(TRANSFER_STOK_MASUK_TOKO, 0) " &
            "- COALESCE(TRANSFER_STOK_KELUAR_TOKO, 0) " &
            "+ COALESCE(TRANSFER_BARANG_MASUK_TOKO, 0) " &
            "- COALESCE(TRANSFER_BARANG_KELUAR_TOKO, 0), " &
            "STOK_GUDANG = COALESCE(AWAL_GUDANG, 0) " &
            "+ COALESCE(TAMBAH_GUDANG, 0) " &
            "- COALESCE(KURANG_GUDANG, 0) " &
            "+ COALESCE(PEMBELIAN_GUDANG, 0) " &
            "- COALESCE(PENJUALAN_GUDANG, 0) " &
            "- COALESCE(RETUR_BELI_GUDANG, 0) " &
            "+ COALESCE(RETUR_JUAL_GUDANG, 0) " &
            "+ COALESCE(OPNAME_GUDANG, 0) " &
            "+ COALESCE(TRANSFER_STOK_MASUK_GUDANG, 0) " &
            "- COALESCE(TRANSFER_STOK_KELUAR_GUDANG, 0) " &
            "+ COALESCE(TRANSFER_BARANG_MASUK_GUDANG, 0) " &
            "- COALESCE(TRANSFER_BARANG_KELUAR_GUDANG, 0) " &
            "WHERE ID_BARANG = @Kode"

        Using command As New MySqlCommand(query, conn, transaction)
            command.Parameters.AddWithValue("@Kode", kode)
            command.ExecuteNonQuery()
        End Using
    End Sub


    Public Sub UpdateAllBarangTokoModule()
        Dim queryTemplates As New Dictionary(Of String, String)() From {
        {"TAMBAH", "TAMBAH_TOKO"},
        {"KURANG", "KURANG_TOKO"},
        {"PEMBELIAN", "PEMBELIAN_TOKO"},
        {"PENJUALAN", "PENJUALAN_TOKO"},
        {"RETUR BELI", "RETUR_BELI_TOKO"},
        {"RETUR JUAL", "RETUR_JUAL_TOKO"},
        {"OPNAME", "OPNAME_TOKO"},
        {"TRANSFER STOK MASUK", "TRANSFER_STOK_MASUK_TOKO"},
        {"TRANSFER STOK KELUAR", "TRANSFER_STOK_KELUAR_TOKO"},
        {"TRANSFER BARANG MASUK", "TRANSFER_BARANG_MASUK_TOKO"},
        {"TRANSFER BARANG KELUAR", "TRANSFER_BARANG_KELUAR_TOKO"}
    }

        Dim barangTotals As New Dictionary(Of String, List(Of Tuple(Of String, Decimal)))()

        For Each jenis As String In queryTemplates.Keys
            Dim sumQuery As String = String.Format("SELECT ID_BARANG, SUM(TOTAL_QTY) AS TotalQty FROM HistoryBarang WHERE JENIS = '{0}' AND LOKASI = 'TOKO' GROUP BY ID_BARANG", jenis)

            Using cmd As New MySqlCommand(sumQuery, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If Not barangTotals.ContainsKey(jenis) Then
                        barangTotals(jenis) = New List(Of Tuple(Of String, Decimal))()
                    End If

                    While reader.Read()
                        Dim kodeBarang As String = reader("ID_BARANG").ToString()
                        Dim totalQty As Decimal = If(reader("TotalQty") IsNot DBNull.Value, Convert.ToDecimal(reader("TotalQty")), 0)
                        barangTotals(jenis).Add(Tuple.Create(kodeBarang, totalQty))
                    End While
                End Using
            End Using
        Next

        For Each jenis As String In barangTotals.Keys
            Dim updateQuery As String = String.Format("UPDATE tbl_barang SET {0} = @TotalQty WHERE ID_BARANG = @IDBarang", queryTemplates(jenis))

            Using updateCmd As New MySqlCommand(updateQuery, conn)
                For Each barang As Tuple(Of String, Decimal) In barangTotals(jenis)
                    updateCmd.Parameters.Clear()
                    updateCmd.Parameters.AddWithValue("@TotalQty", barang.Item2)
                    updateCmd.Parameters.AddWithValue("@IDBarang", barang.Item1)
                    updateCmd.ExecuteNonQuery()
                Next
            End Using
        Next
    End Sub


    Public Sub UpdateAllBarangGudangModule()
        ' Dictionary untuk menyimpan nama kolom query dan jenis barang
        Dim queryTemplates As New Dictionary(Of String, String)() From {
        {"TAMBAH", "TAMBAH_GUDANG"},
        {"KURANG", "KURANG_GUDANG"},
        {"PEMBELIAN", "PEMBELIAN_GUDANG"},
        {"PENJUALAN", "PENJUALAN_GUDANG"},
        {"RETUR BELI", "RETUR_BELI_GUDANG"},
        {"RETUR JUAL", "RETUR_JUAL_GUDANG"},
        {"OPNAME", "OPNAME_GUDANG"},
        {"TRANSFER STOK MASUK", "TRANSFER_STOK_MASUK_GUDANG"},
        {"TRANSFER STOK KELUAR", "TRANSFER_STOK_KELUAR_GUDANG"},
        {"TRANSFER BARANG MASUK", "TRANSFER_BARANG_MASUK_GUDANG"},
        {"TRANSFER BARANG KELUAR", "TRANSFER_BARANG_KELUAR_GUDANG"}
    }

        ' Dictionary untuk menyimpan hasil pembacaan sebelum pembaruan
        Dim barangTotals As New Dictionary(Of String, List(Of Tuple(Of String, Decimal)))()

        ' Membaca data dari database dan menyimpan ke dictionary
        For Each jenis As String In queryTemplates.Keys
            Dim sumQuery As String = String.Format("SELECT ID_BARANG, SUM(TOTAL_QTY) AS TotalQty FROM HistoryBarang WHERE JENIS = '{0}' AND LOKASI = 'GUDANG' GROUP BY ID_BARANG", jenis)

            Using cmd As New MySqlCommand(sumQuery, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If Not barangTotals.ContainsKey(jenis) Then
                        barangTotals(jenis) = New List(Of Tuple(Of String, Decimal))()
                    End If

                    While reader.Read()
                        Dim kodeBarang As String = reader("ID_BARANG").ToString()
                        Dim totalQty As Decimal = If(reader("TotalQty") IsNot DBNull.Value, Convert.ToDecimal(reader("TotalQty")), 0)
                        barangTotals(jenis).Add(Tuple.Create(kodeBarang, totalQty))
                    End While
                End Using
            End Using
        Next

        ' Menggunakan data dari dictionary untuk memperbarui tabel
        For Each jenis As String In barangTotals.Keys
            Dim updateQuery As String = String.Format("UPDATE tbl_barang SET {0} = @TotalQty WHERE ID_BARANG = @IDBarang", queryTemplates(jenis))

            Using updateCmd As New MySqlCommand(updateQuery, conn)
                For Each barang As Tuple(Of String, Decimal) In barangTotals(jenis)
                    updateCmd.Parameters.Clear()
                    updateCmd.Parameters.AddWithValue("@TotalQty", barang.Item2)
                    updateCmd.Parameters.AddWithValue("@IDBarang", barang.Item1)
                    updateCmd.ExecuteNonQuery()
                Next
            End Using
        Next
    End Sub


    Public Sub CatatanAksiHistory(ByVal deskripsiAksi As String)
        Dim query As String = "INSERT INTO History (Tanggal, Aksi) VALUES (@Tanggal, @Aksi)"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Tanggal", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@Aksi", FormUtama.SLokasi.Text & " = " & "[" & FormUtama.Comp.Text & " - " & FormUtama.SLogin.Text & "] " & deskripsiAksi)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub UpdateTotalBonDanTotalBayarKaryawan()
        Using transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' Set initial values to 0
                Dim resetValuesQuery As String = "UPDATE tbl_karyawan SET TotalBon = 0, TotalBayar = 0"
                Using resetValuesCommand As New MySqlCommand(resetValuesQuery, conn, transaction)
                    resetValuesCommand.ExecuteNonQuery()
                End Using

                ' Create adapters for aggregated results
                Dim tempBonQuery As String = "SELECT Kode, SUM(NOMINAL) AS TotalBon FROM Bon_karyawan WHERE JENIS = 'BON' GROUP BY Kode"
                Dim tempBayarQuery As String = "SELECT Kode, SUM(NOMINAL) AS TotalBayar FROM Bon_karyawan WHERE JENIS = 'BAYAR' GROUP BY Kode"

                Dim tempBonTable As New DataTable()
                Dim tempBayarTable As New DataTable()

                Using tempBonAdapter As New MySqlDataAdapter(tempBonQuery, conn)
                    tempBonAdapter.SelectCommand.Transaction = transaction
                    tempBonAdapter.Fill(tempBonTable)
                End Using

                Using tempBayarAdapter As New MySqlDataAdapter(tempBayarQuery, conn)
                    tempBayarAdapter.SelectCommand.Transaction = transaction
                    tempBayarAdapter.Fill(tempBayarTable)
                End Using

                ' Update tbl_karyawan with aggregated results
                For Each row As DataRow In tempBonTable.Rows
                    Using updateBonCommand As New MySqlCommand("UPDATE tbl_karyawan SET TotalBon = ? WHERE Kode = ?", conn, transaction)
                        updateBonCommand.Parameters.AddWithValue("@TotalBon", row("TotalBon"))
                        updateBonCommand.Parameters.AddWithValue("@Kode", row("Kode"))
                        updateBonCommand.ExecuteNonQuery()
                    End Using
                Next

                For Each row As DataRow In tempBayarTable.Rows
                    Using updateBayarCommand As New MySqlCommand("UPDATE tbl_karyawan SET TotalBayar = ? WHERE Kode = ?", conn, transaction)
                        updateBayarCommand.Parameters.AddWithValue("@TotalBayar", row("TotalBayar"))
                        updateBayarCommand.Parameters.AddWithValue("@Kode", row("Kode"))
                        updateBayarCommand.ExecuteNonQuery()
                    End Using
                Next

                Dim updateQuery As String = "UPDATE tbl_karyawan " &
                                   "SET SaldoAkhir = SaldoAwal + TotalBon - TotalBayar"

                Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                    cmd.ExecuteNonQuery()
                End Using


                ' Commit transaction
                transaction.Commit()
            Catch ex As Exception
                ' Rollback transaction in case of error
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using

    End Sub


    Public Sub SaldoAkunTambah(ByVal KodeDebet As String, ByVal KodeKredit As String, ByVal Nominal As Decimal, ByVal transaction As MySqlTransaction)
        If KodeDebet <> "" Then
            Using updateDebetCommand As New MySqlCommand("UPDATE tbl_datareferensi SET Saldo_Akhir = Saldo_Akhir + ? WHERE Kode_akun = ?", conn, transaction)
                updateDebetCommand.Parameters.AddWithValue("@Saldo_Akhir", Nominal)
                updateDebetCommand.Parameters.AddWithValue("@Kode_akun", KodeDebet)
                updateDebetCommand.ExecuteNonQuery()
            End Using
        End If

        If KodeKredit <> "" Then
            Using updateKreditCommand As New MySqlCommand("UPDATE tbl_datareferensi SET Saldo_Akhir = Saldo_Akhir + ? WHERE Kode_akun = ?", conn, transaction)
                updateKreditCommand.Parameters.AddWithValue("@Saldo_Akhir", Nominal)
                updateKreditCommand.Parameters.AddWithValue("@Kode_akun", KodeKredit)
                updateKreditCommand.ExecuteNonQuery()
            End Using
        End If
    End Sub

    Public Sub SaldoAkunKurang(ByVal KodeDebet As String, ByVal KodeKredit As String, ByVal Nominal As Decimal, ByVal transaction As MySqlTransaction)
        ' Jika KodeDebet tidak kosong, lakukan pengurangan
        If KodeDebet <> "" Then
            Using updateDebetCommand As New MySqlCommand("UPDATE tbl_datareferensi SET Saldo_Akhir = Saldo_Akhir - ? WHERE Kode_akun = ?", conn, transaction)
                updateDebetCommand.Parameters.AddWithValue("@Saldo_Akhir", Nominal)
                updateDebetCommand.Parameters.AddWithValue("@Kode_akun", KodeDebet)
                updateDebetCommand.ExecuteNonQuery()
            End Using
        End If

        ' Jika KodeKredit tidak kosong, lakukan pengurangan
        If KodeKredit <> "" Then
            Using updateKreditCommand As New MySqlCommand("UPDATE tbl_datareferensi SET Saldo_Akhir = Saldo_Akhir - ? WHERE Kode_akun = ?", conn, transaction)
                updateKreditCommand.Parameters.AddWithValue("@Saldo_Akhir", Nominal)
                updateKreditCommand.Parameters.AddWithValue("@Kode_akun", KodeKredit)
                updateKreditCommand.ExecuteNonQuery()
            End Using
        End If
    End Sub


End Module
