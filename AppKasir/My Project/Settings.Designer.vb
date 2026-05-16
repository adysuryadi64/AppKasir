Namespace My

    Partial Friend NotInheritable Class MySettings
        Private Shared _default As MySettings

        Private _databasemorosenengConnectionString As String = ""

        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                If _default Is Nothing Then
                    _default = New MySettings()
                End If
                Return _default
            End Get
        End Property

        Public Property databasemorosenengConnectionString As String
            Get
                Return _databasemorosenengConnectionString
            End Get
            Set(value As String)
                _databasemorosenengConnectionString = value
            End Set
        End Property

    End Class

End Namespace
