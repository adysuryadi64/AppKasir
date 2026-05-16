Friend Module ModulHakAkses
    ' === CACHE HTAK AKSES DI MEMORY ===
    Private hakAksesCache As New Dictionary(Of String, Boolean())
    Private currentUserCache As String = ""

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
            hakAkses = BacaHakAksesDariDatabase(FormUtama.SLevel.Text, modulName)
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
                    rd.Close()
                    FormHakUser.IsiDataGridViewTemplate()
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

        Dim SelectQuery As String = "SELECT ModuleName FROM hakaksesuser WHERE Role = @Role"

        Using cmd As New MySqlCommand(SelectQuery, conn)
            cmd.Parameters.AddWithValue("@Role", role)

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ModulName = reader("ModuleName").ToString()
                Else
                    FormGeneralSetting.SinkronkanHakAksesTanpaDuplikat()
                End If
            End Using
        End Using

        Return ModulName
    End Function

End Module
