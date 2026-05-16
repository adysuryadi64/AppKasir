Imports MySql.Data.MySqlClient

''' <summary>
''' Konfigurasi dan state sinkronisasi (last_sync, kode_cabang, dll)
''' </summary>
Public Class SyncConfig

    ''' <summary>
    ''' Device ID unik per instalasi. Dibuat sekali saat pertama kali diakses,
    ''' disimpan di sync_config. Dipakai untuk validasi kepemilikan kode_cabang di cloud.
    ''' </summary>
    Public Shared ReadOnly Property DeviceId As String
        Get
            Dim id As String = GetNilai("device_id")
            If String.IsNullOrEmpty(id) Then
                id = Guid.NewGuid().ToString()
                SetNilai("device_id", id)
            End If
            Return id
        End Get
    End Property

    Public Shared Function GetNilai(kunci As String) As String
        Using cmd As New MySqlCommand(
            "SELECT nilai FROM sync_config WHERE kunci = @kunci LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@kunci", kunci)
            Dim val = cmd.ExecuteScalar()
            Return If(val IsNot Nothing AndAlso val IsNot DBNull.Value, val.ToString(), "")
        End Using
    End Function

    Public Shared Sub SetNilai(kunci As String, nilai As String)
        Using cmd As New MySqlCommand(
            "INSERT INTO sync_config (kunci, nilai) VALUES (@kunci, @nilai)
             ON DUPLICATE KEY UPDATE nilai = @nilai", conn)
            cmd.Parameters.AddWithValue("@kunci", kunci)
            cmd.Parameters.AddWithValue("@nilai", nilai)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Shared ReadOnly Property KodeCabang As String
        Get
            Dim kode As String = GetNilai("kode_cabang")
            If String.IsNullOrEmpty(kode) Then
                kode = GenerateKodeCabang()
                SetNilai("kode_cabang", kode)
            End If
            Return kode
        End Get
    End Property

    ''' <summary>
    ''' Generate kode_cabang otomatis: kode perusahaan + suffix 4 karakter dari DeviceId.
    ''' Contoh: "TOKO1-A3F2". Tidak bisa bentrok antar instalasi.
    ''' </summary>
    Private Shared Function GenerateKodeCabang() As String
        ' Ambil kode perusahaan sebagai prefix
        Dim prefix As String = GetNilai("kode_perusahaan_lokal")

        If String.IsNullOrEmpty(prefix) Then
            ' Fallback: ambil langsung dari tbl_perusahaan
            Try
                Using cmd As New MySqlCommand(
                    "SELECT KODE FROM tbl_perusahaan LIMIT 1", conn)
                    Dim val = cmd.ExecuteScalar()
                    If val IsNot Nothing AndAlso val IsNot DBNull.Value Then
                        prefix = val.ToString().ToUpper().Trim()
                    End If
                End Using
            Catch
                ' Jika DB belum siap, pakai fallback generik
            End Try
        End If

        If String.IsNullOrEmpty(prefix) Then prefix = "CABANG"

        ' Suffix 4 karakter hex dari DeviceId — unik per instalasi
        Dim suffix As String = DeviceId.Replace("-", "").Substring(0, 4).ToUpper()

        Return $"{prefix}-{suffix}"
    End Function

    Public Shared ReadOnly Property LastSyncBarang As String
        Get
            Return GetNilai("last_sync_barang")
        End Get
    End Property

    Public Shared Sub UpdateLastSyncBarang()
        SetNilai("last_sync_barang", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
    End Sub

    Public Shared ReadOnly Property LastSyncTransfer As String
        Get
            Return GetNilai("last_sync_transfer")
        End Get
    End Property

    Public Shared Sub UpdateLastSyncTransfer()
        SetNilai("last_sync_transfer", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
    End Sub

    Public Shared ReadOnly Property LastSyncKategori As String
        Get
            Return GetNilai("last_sync_kategori")
        End Get
    End Property
    Public Shared Sub UpdateLastSyncKategori()
        SetNilai("last_sync_kategori", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
    End Sub

    Public Shared ReadOnly Property LastSyncSatuan As String
        Get
            Return GetNilai("last_sync_satuan")
        End Get
    End Property
    Public Shared Sub UpdateLastSyncSatuan()
        SetNilai("last_sync_satuan", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
    End Sub

    Public Shared ReadOnly Property LastSyncMerk As String
        Get
            Return GetNilai("last_sync_merk")
        End Get
    End Property
    Public Shared Sub UpdateLastSyncMerk()
        SetNilai("last_sync_merk", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
    End Sub

    Public Shared ReadOnly Property LastSyncSupliyer As String
        Get
            Return GetNilai("last_sync_supliyer")
        End Get
    End Property
    Public Shared Sub UpdateLastSyncSupliyer()
        SetNilai("last_sync_supliyer", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
    End Sub

    Public Shared ReadOnly Property LastSyncPelanggan As String
        Get
            Return GetNilai("last_sync_pelanggan")
        End Get
    End Property
    Public Shared Sub UpdateLastSyncPelanggan()
        SetNilai("last_sync_pelanggan", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
    End Sub

    Public Shared ReadOnly Property LastSyncArmada As String
        Get
            Return GetNilai("last_sync_armada")
        End Get
    End Property
    Public Shared Sub UpdateLastSyncArmada()
        SetNilai("last_sync_armada", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
    End Sub

    Public Shared ReadOnly Property LastSyncCabang As String
        Get
            Return GetNilai("last_sync_cabang")
        End Get
    End Property
    Public Shared Sub UpdateLastSyncCabang()
        SetNilai("last_sync_cabang", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
    End Sub

End Class
