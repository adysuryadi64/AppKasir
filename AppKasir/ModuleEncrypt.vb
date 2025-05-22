Imports System.Security.Cryptography
Imports System.Text

Module ModuleEncrypt

    Private ReadOnly encryptionKey As String = "KasirLancar12345" ' Panjang 16, 24, atau 32 karakter (AES key)

    Public Function EncryptPassword(plainText As String) As String
        Dim keyBytes = Encoding.UTF8.GetBytes(encryptionKey)
        Using aes As Aes = Aes.Create()
            aes.Key = keyBytes
            aes.IV = keyBytes.Take(16).ToArray()
            Using encryptor = aes.CreateEncryptor()
                Dim plainBytes = Encoding.UTF8.GetBytes(plainText)
                Dim encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length)
                Return Convert.ToBase64String(encryptedBytes)
            End Using
        End Using
    End Function

    Public Function DecryptPassword(encryptedText As String) As String
        Dim keyBytes = Encoding.UTF8.GetBytes(encryptionKey)
        Using aes As Aes = Aes.Create()
            aes.Key = keyBytes
            aes.IV = keyBytes.Take(16).ToArray()
            Using decryptor = aes.CreateDecryptor()
                Dim encryptedBytes = Convert.FromBase64String(encryptedText)
                Dim decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length)
                Return Encoding.UTF8.GetString(decryptedBytes)
            End Using
        End Using
    End Function

End Module
