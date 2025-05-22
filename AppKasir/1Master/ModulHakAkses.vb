Friend Module ModulHakAkses
    ' Fungsi untuk membaca hak akses dari database
    Public Function BacaHakAkses(ByVal userName As String, ByVal modulName As String, ByVal conn As MySqlConnection) As Boolean()
        Dim hakAkses(3) As Boolean ' Indeks 0 untuk CanRead, 1 untuk CanAdd, 2 untuk CanEdit, 3 untuk CanDelete

        Dim query As String = "SELECT CanRead, CanAdd, CanEdit, CanDelete " &
                              "FROM hakaksesuser " &
                              "WHERE UserName = @UserName AND ModuleName = @ModuleName"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@UserName", userName)
            cmd.Parameters.AddWithValue("@ModuleName", modulName)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    rd.Read() ' Move to the first row if there are rows
                    hakAkses(0) = DirectCast(rd("CanRead"), Boolean)
                    hakAkses(1) = DirectCast(rd("CanAdd"), Boolean)
                    hakAkses(2) = DirectCast(rd("CanEdit"), Boolean)
                    hakAkses(3) = DirectCast(rd("CanDelete"), Boolean)
                Else
                    rd.Close()
                    FormHakUser.DataHakaksesuser()
                End If
            End Using
        End Using

        Return hakAkses
    End Function

    Public Function BacaHakAksesSemua(ByVal role As String) As String
        Dim ModulName As String = ""

        Dim SelectQuery As String = "SELECT ModuleName FROM hakaksesuser WHERE Role = @Role"

        Using cmd As New MySqlCommand(SelectQuery, conn)
            cmd.Parameters.AddWithValue("@Role", role)

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ModulName = reader("ModuleName").ToString()
                End If
            End Using
        End Using

        Return ModulName
    End Function




End Module
