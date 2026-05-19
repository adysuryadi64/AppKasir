Friend Module ModulHakAkses
    ' === CACHE HAK AKSES USER DI MEMORY ===
    Private hakAksesCache As New Dictionary(Of String, Boolean())
    Private currentUserCache As String = ""

    ' === CACHE GENERAL SETTING DI MEMORY ===
    Private generalSettingCache As New Dictionary(Of String, String)()
    Private generalSettingLastUpdated As DateTime = DateTime.MinValue

    ' ============================================================
    ' GENERAL SETTING CACHE
    ' ============================================================

    ''' <summary>
    ''' Load semua general setting (UserName='Semua') ke cache.
    ''' Dipanggil saat login dan saat timer deteksi perubahan.
    ''' </summary>
    Public Sub CacheGeneralSetting()
        Try
            Dim newCache As New Dictionary(Of String, String)()
            Dim latestUpdated As DateTime = DateTime.MinValue

            Dim query As String = "SELECT Role, ModuleName, updated_at FROM hakaksesuser WHERE UserName = 'Semua' AND ModuleName <> ''"
            Using cmd As New MySqlCommand(query, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim role As String = rd("Role").ToString()
                        Dim moduleName As String = rd("ModuleName").ToString()
                        Dim updAt As DateTime = Convert.ToDateTime(rd("updated_at"))

                        ' Ambil nilai terbaru jika ada duplikat Role
                        newCache(role) = moduleName

                        If updAt > latestUpdated Then latestUpdated = updAt
                    End While
                End Using
            End Using

            generalSettingCache = newCache
            generalSettingLastUpdated = latestUpdated
        Catch
            ' Jika gagal, biarkan cache lama tetap dipakai
        End Try
    End Sub

    ''' <summary>
    ''' Cek apakah ada perubahan setting dari client lain.
    ''' Query ringan — hanya ambil MAX(updated_at).
    ''' Dipanggil oleh timer di FormUtama setiap 60 detik.
    ''' </summary>
    Public Sub CekDanRefreshGeneralSetting()
        Try
            Dim query As String = "SELECT MAX(updated_at) FROM hakaksesuser WHERE UserName = 'Semua'"
            Using cmd As New MySqlCommand(query, conn)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    Dim latestDB As DateTime = Convert.ToDateTime(result)
                    ' Jika DB lebih baru dari cache, reload
                    If latestDB > generalSettingLastUpdated Then
                        CacheGeneralSetting()
                    End If
                End If
            End Using
        Catch
            ' Gagal cek — biarkan cache lama, tidak crash
        End Try
    End Sub

    ''' <summary>
    ''' Baca general setting dari cache (INSTANT, tanpa query DB).
    ''' Pengganti BacaHakAksesSemua yang lama.
    ''' </summary>
    Public Function BacaSettingDariCache(ByVal role As String) As String
        If generalSettingCache.ContainsKey(role) Then
            Return generalSettingCache(role)
        End If
        ' Fallback: cache kosong (belum diload), query DB langsung
        Return BacaHakAksesSemua(role)
    End Function

    ''' <summary>
    ''' Load semua hak akses user ke dalam cache (dipanggil saat login)
    ''' </summary>
    Public Sub CacheHakAksesUser(ByVal userName As String)
        currentUserCache = userName
        hakAksesCache.Clear()

        Dim query As String = "SELECT ModuleName, CanRead, CanAdd, CanEdit, CanDelete FROM hakaksesuser WHERE UserName = @UserName"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@UserName", userName)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim moduleName As String = rd("ModuleName").ToString()
                    Dim hakAkses(3) As Boolean

                    hakAkses(0) = CBool(rd("CanRead"))
                    hakAkses(1) = CBool(rd("CanAdd"))
                    hakAkses(2) = CBool(rd("CanEdit"))
                    hakAkses(3) = CBool(rd("CanDelete"))

                    If Not hakAksesCache.ContainsKey(moduleName) Then
                        hakAksesCache.Add(moduleName, hakAkses)
                    End If
                End While
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Baca hak akses dari cache (INSTANT, tanpa query DB)
    ''' Dipanggil saat klik kanan untuk menampilkan context menu
    ''' </summary>
    Public Function BacaHakAksesDariCache(ByVal modulName As String) As Boolean()
        Dim hakAkses(3) As Boolean ' Default: semua false

        ' Cek apakah module ada di cache
        If hakAksesCache.ContainsKey(modulName) Then
            hakAkses = hakAksesCache(modulName)
        Else
            ' Jika tidak ada di cache, fallback ke DB query (jarang terjadi)
            ' Ini hanya backup, biasanya cache sudah lengkap dari login
            hakAkses = BacaHakAksesDariDatabase(FormUtama.StatusLevelUser.Text, modulName)
        End If

        Return hakAkses
    End Function

    ''' <summary>
    ''' Fungsi LAMA - Baca dari Database (hanya dipanggil dari cache miss atau admin forms)
    ''' JANGAN gunakan untuk klik kanan context menu
    ''' </summary>
    Public Function BacaHakAkses(ByVal userName As String, ByVal modulName As String, ByVal conn As MySqlConnection) As Boolean()
        ' Gunakan cache jika user sedang login
        If userName = currentUserCache Then
            Return BacaHakAksesDariCache(modulName)
        Else
            ' Fallback ke DB jika user berbeda (admin operation)
            Return BacaHakAksesDariDatabase(userName, modulName)
        End If
    End Function

    ''' <summary>
    ''' Internal function - Query database (PRIVATE)
    ''' </summary>
    Private Function BacaHakAksesDariDatabase(ByVal userName As String, ByVal modulName As String) As Boolean()
        Dim hakAkses(3) As Boolean

        Dim query As String = "SELECT CanRead, CanAdd, CanEdit, CanDelete " &
                              "FROM hakaksesuser " &
                              "WHERE UserName = @UserName AND ModuleName = @ModuleName"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@UserName", userName)
            cmd.Parameters.AddWithValue("@ModuleName", modulName)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    rd.Read()
                    hakAkses(0) = CBool(rd("CanRead"))
                    hakAkses(1) = CBool(rd("CanAdd"))
                    hakAkses(2) = CBool(rd("CanEdit"))
                    hakAkses(3) = CBool(rd("CanDelete"))
                Else
                    ' Modul belum ada di DB — default visible (True) agar tidak hilang
                    ' SinkronkanDatabaseDenganTemplate di FormHakUser akan insert entry ini
                    hakAkses(0) = True
                    hakAkses(1) = True
                    hakAkses(2) = True
                    hakAkses(3) = True
                End If
            End Using
        End Using

        Return hakAkses
    End Function

    ''' <summary>
    ''' Clear cache saat logout
    ''' </summary>
    Public Sub ClearHakAksesCache()
        hakAksesCache.Clear()
        currentUserCache = ""
    End Sub

    ''' <summary>
    ''' Refresh cache (dipanggil setelah update hak akses di FormHakUser)
    ''' </summary>
    Public Sub RefreshHakAksesCache()
        If Not String.IsNullOrEmpty(currentUserCache) Then
            CacheHakAksesUser(currentUserCache)
        End If
    End Sub

    Public Function BacaHakAksesSemua(ByVal role As String) As String
        Dim ModulName As String = ""
        Dim rowFound As Boolean = False

        Dim SelectQuery As String = "SELECT ModuleName FROM hakaksesuser WHERE Role = @Role AND UserName = 'Semua'"

        Using cmd As New MySqlCommand(SelectQuery, conn)
            cmd.Parameters.AddWithValue("@Role", role)

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ModulName = reader("ModuleName").ToString()
                    rowFound = True
                End If
            End Using
        End Using

        ' Panggil sinkronisasi SETELAH reader ditutup, bukan di dalam Using reader
        If Not rowFound Then
            FormGeneralSetting.SinkronkanHakAksesTanpaDuplikat()
        End If

        Return ModulName
    End Function

    ' ============================================================
    ' PROPERTIES SETTING GLOBAL — Form tinggal pakai, tanpa BacaSettingDariCache
    ' Nilai "Iya"/"Tidak" → Boolean. Multi-nilai → String.
    ' ============================================================

    ''' <summary>True jika transaksi dengan tanggal lampau diizinkan.</summary>
    Public ReadOnly Property SettingIzinkanTanggalLampau As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblGlobalTransaksiLampau.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika transaksi keluar barang meski stok jadi minus diizinkan.</summary>
    Public ReadOnly Property SettingIzinkanBarangMinus As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblGlobalBarangMinus.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika mode fokus = "Pencarian" (fokus ke TxtNama). False = "Kolom data" (fokus ke grid).</summary>
    Public ReadOnly Property SettingFokusOtomatis As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblGlobalFokus.Text) = "Pencarian"
        End Get
    End Property

    ''' <summary>True jika satuan berbeda antar item diizinkan.</summary>
    Public ReadOnly Property SettingIzinkanSatuanBerbeda As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblGlobalSatuan.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika langsung isi nominal total saat bayar.</summary>
    Public ReadOnly Property SettingLangsungIsiNominalTotal As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblGlobalIsiNominal.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika info stok ditampilkan di form transaksi.</summary>
    Public ReadOnly Property SettingTampilInfoStok As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblGlobalInfoStok.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika panel pencarian di atas data grid disembunyikan.</summary>
    Public ReadOnly Property SettingSembunyikanPencarianAtas As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblHidePencarianAtas.Text) = "Iya"
        End Get
    End Property

    ' --- Setting Penjualan ---

    ''' <summary>True jika user boleh ubah harga jual saat transaksi.</summary>
    Public ReadOnly Property SettingIzinkanUbahHargaJual As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblJualEditHarga.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika penjualan rugi (harga jual lebih kecil harga beli) diizinkan.</summary>
    Public ReadOnly Property SettingIzinkanJualRugi As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblJualRugi.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika nominal penjualan nol diizinkan.</summary>
    Public ReadOnly Property SettingIzinkanNominalJualNol As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblJualNominal0.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika harga jual otomatis update master saat simpan.</summary>
    Public ReadOnly Property SettingHargaJualOtomatisUpdateMaster As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblEditHargaJual.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika diskon per item diizinkan.</summary>
    Public ReadOnly Property SettingIzinkanDiskonItem As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblDiskonItem.Text) = "Iya"
        End Get
    End Property

    ' --- Setting Pembelian ---

    ''' <summary>True jika user boleh ubah harga beli saat transaksi.</summary>
    Public ReadOnly Property SettingIzinkanUbahHargaBeli As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblBeliEditHarga.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika harga beli otomatis update harga jual.</summary>
    Public ReadOnly Property SettingBeliOtomatisUpdateHargaJual As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblBeliMuculJual.Text) = "Iya"
        End Get
    End Property

    ''' <summary>
    ''' Metode update harga beli — multi-nilai, tetap String.
    ''' Contoh: "Harga Terbaru", "Metode Average (Rata - Rata)", "Tidak Ada"
    ''' </summary>
    Public ReadOnly Property SettingMetodeUpdateHargaBeli As String
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblBeliUpdate.Text)
        End Get
    End Property

    ''' <summary>
    ''' Metode average berdasarkan stok — multi-nilai, tetap String.
    ''' Contoh: "Toko dan Gudang", "Toko Saja", "Gudang Saja"
    ''' </summary>
    Public ReadOnly Property SettingAverageHargaBerdasarkanStok As String
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblBeliAverage.Text)
        End Get
    End Property

    ''' <summary>True jika beli tanpa supplier diizinkan.</summary>
    Public ReadOnly Property SettingIzinkanBeliTanpaSupplier As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblBeliTanpaSupplier.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika nominal pembelian nol diizinkan.</summary>
    Public ReadOnly Property SettingIzinkanNominalBeliNol As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblbeliNominal0.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika pembelian rugi diizinkan.</summary>
    Public ReadOnly Property SettingIzinkanBeliRugi As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblBeliRugi.Text) = "Iya"
        End Get
    End Property

    ' --- Setting Retur ---

    ''' <summary>True jika wajib isi alasan retur beli.</summary>
    Public ReadOnly Property SettingWajibAlasanReturBeli As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblReturBeliAlasan.Text) = "Iya"
        End Get
    End Property

    ''' <summary>True jika wajib isi alasan retur jual.</summary>
    Public ReadOnly Property SettingWajibAlasanReturJual As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblReturJualAlasan.Text) = "Iya"
        End Get
    End Property

    ' --- Setting Auto Level Satuan ---

    ''' <summary>True jika satuan otomatis berubah berdasarkan qty.</summary>
    Public ReadOnly Property SettingAutoLevelSatuan As Boolean
        Get
            Return BacaSettingDariCache(FormGeneralSetting.LblJualAutoLevelSatuan.Text) = "Iya"
        End Get
    End Property

    ' Cache batas qty satuan — disimpan terpisah karena bertipe Integer, bukan Iya/Tidak
    Private _batasSatuanSedang As Integer = 3
    Private _batasSatuanBesar As Integer = 6

    ''' <summary>
    ''' Load batas qty satuan dari DB ke cache memory.
    ''' Dipanggil saat startup dan setelah simpan General Setting.
    ''' </summary>
    Public Sub CacheBatasSatuan()
        Try
            Using cmd As New MySqlCommand(
                "SELECT Role, ModuleName FROM hakaksesuser WHERE Role IN ('JualBatasSatuanSedang','JualBatasSatuanBesar') AND UserName = 'SYSTEM'", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim role As String = rd("Role").ToString()
                        Dim val As Integer = ModuleAngka.ParseInteger(rd("ModuleName"), 0)
                        If role = "JualBatasSatuanSedang" AndAlso val > 0 Then _batasSatuanSedang = val
                        If role = "JualBatasSatuanBesar" AndAlso val > 0 Then _batasSatuanBesar = val
                    End While
                End Using
            End Using
        Catch
            ' Biarkan nilai default jika gagal
        End Try
    End Sub

    ''' <summary>Batas qty untuk naik ke satuan sedang (qty >= nilai ini).</summary>
    Public ReadOnly Property SettingBatasSatuanSedang As Integer
        Get
            Return _batasSatuanSedang
        End Get
    End Property

    ''' <summary>Batas qty untuk naik ke satuan besar (qty >= nilai ini).</summary>
    Public ReadOnly Property SettingBatasSatuanBesar As Integer
        Get
            Return _batasSatuanBesar
        End Get
    End Property

    ' ============================================================
    ' HELPER DTP — MODE BACKDATE
    ' ============================================================

    ''' <summary>
    ''' Terapkan mode DTP berdasarkan setting izin transaksi lampau dari cache.
    ''' Form tidak perlu baca setting sendiri — langsung pakai fungsi ini.
    '''
    ''' Mode TAMBAH (isEditMode = False):
    '''   - Tidak diizinkan: DTP di-reset ke DateTime.Now, Enabled = False
    '''   - Diizinkan: DTP di DateTime.Now, Enabled = True
    '''
    ''' Mode EDIT (isEditMode = True):
    '''   - Tidak diizinkan: DTP diisi tanggal lama, Enabled = False
    '''   - Diizinkan: DTP diisi tanggal lama, Enabled = True
    ''' </summary>
    Public Sub TerapkanModeDTP(dtp As DateTimePicker,
                               isEditMode As Boolean,
                               Optional tanggalEdit As DateTime = Nothing)
        If isEditMode Then
            Dim tglEdit As DateTime = If(tanggalEdit = Nothing, DateTime.Now, tanggalEdit)
            dtp.Value = tglEdit
            dtp.Enabled = True  ' Mode edit: selalu bisa ubah tanggal, tanggal lama bisa lampau
        Else
            dtp.Value = DateTime.Now
            dtp.Enabled = SettingIzinkanTanggalLampau
        End If
    End Sub

    ''' <summary>
    ''' Reset DTP ke DateTime.Now dan kunci jika backdate tidak diizinkan.
    ''' Baca setting langsung dari cache — form tidak perlu kirim parameter.
    ''' Panggil setiap kali form di-reset ke mode tambah.
    ''' </summary>
    Public Sub ResetDTPKeTanggalHariIni(dtp As DateTimePicker)
        dtp.Value = DateTime.Now
        dtp.Enabled = SettingIzinkanTanggalLampau
    End Sub

    ''' <summary>
    ''' Cek apakah tanggal yang dipilih user valid untuk disimpan.
    ''' Return True jika valid, False jika backdate tidak diizinkan.
    ''' </summary>
    Public Function ValidasiTanggalTransaksi(tglDipilih As DateTime) As Boolean
        If Not SettingIzinkanTanggalLampau AndAlso tglDipilih.Date < DateTime.Now.Date Then
            Return False
        End If
        Return True
    End Function

End Module
