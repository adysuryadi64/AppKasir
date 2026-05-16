Imports System.Runtime.InteropServices

' ================================================================
' RawPrinterHelper
' Kirim byte array mentah ke printer via Windows Spooler (winspool)
' Dipakai oleh PrinterEscPos untuk thermal dan dot matrix
' ================================================================
Public Class RawPrinterHelper

#Region "P/Invoke - Windows Spooler API"

    <DllImport("winspool.drv", CharSet:=CharSet.Ansi, SetLastError:=True)>
    Private Shared Function OpenPrinter(pPrinterName As String,
                                         ByRef phPrinter As IntPtr,
                                         pDefault As IntPtr) As Boolean
    End Function

    <DllImport("winspool.drv", CharSet:=CharSet.Ansi, SetLastError:=True)>
    Private Shared Function ClosePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.drv", CharSet:=CharSet.Ansi, SetLastError:=True)>
    Private Shared Function StartDocPrinter(hPrinter As IntPtr,
                                             Level As Integer,
                                             ByRef pDocInfo As DOCINFO) As Integer
    End Function

    <DllImport("winspool.drv", CharSet:=CharSet.Ansi, SetLastError:=True)>
    Private Shared Function EndDocPrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.drv", CharSet:=CharSet.Ansi, SetLastError:=True)>
    Private Shared Function StartPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.drv", CharSet:=CharSet.Ansi, SetLastError:=True)>
    Private Shared Function EndPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.drv", CharSet:=CharSet.Ansi, SetLastError:=True)>
    Private Shared Function WritePrinter(hPrinter As IntPtr,
                                          pBytes As IntPtr,
                                          dwCount As Integer,
                                          ByRef dwWritten As Integer) As Boolean
    End Function

#End Region

#Region "Struktur DOCINFO"

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
    Private Structure DOCINFO
        <MarshalAs(UnmanagedType.LPStr)> Public pDocName As String
        <MarshalAs(UnmanagedType.LPStr)> Public pOutputFile As String
        <MarshalAs(UnmanagedType.LPStr)> Public pDataType As String
    End Structure

#End Region

#Region "Kirim Data ke Printer"

    ''' <summary>
    ''' Kirim byte array ke printer secara raw (ESC/POS).
    ''' Gunakan ini untuk thermal dan dot matrix.
    ''' </summary>
    Public Shared Function KirimKePrinter(namaPrinter As String,
                                           data As Byte()) As Boolean
        If String.IsNullOrEmpty(namaPrinter) OrElse data Is Nothing OrElse data.Length = 0 Then
            Return False
        End If

        ' Tampilkan daftar printer terinstal untuk perbandingan
        Dim daftarPrinter As String = String.Join(" | ", System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast(Of String)())

        Dim hPrinter As IntPtr = IntPtr.Zero
        If Not OpenPrinter(namaPrinter, hPrinter, IntPtr.Zero) Then
            Dim errCode As Integer = Marshal.GetLastWin32Error()
            Return False
        End If

        Try
            Dim infosDokumen As New DOCINFO() With {
                .pDocName = "ESC/POS Raw",
                .pOutputFile = Nothing,
                .pDataType = "RAW"
            }
            Dim jobId As Integer = StartDocPrinter(hPrinter, 1, infosDokumen)
            If jobId = 0 Then
                Return False
            End If
            If Not StartPagePrinter(hPrinter) Then
                EndDocPrinter(hPrinter)
                Return False
            End If

            Dim pointerData As IntPtr = Marshal.AllocCoTaskMem(data.Length)
            Marshal.Copy(data, 0, pointerData, data.Length)
            Dim jumlahDitulis As Integer = 0
            Dim berhasil = WritePrinter(hPrinter, pointerData, data.Length, jumlahDitulis)
            Marshal.FreeCoTaskMem(pointerData)

            EndPagePrinter(hPrinter)
            EndDocPrinter(hPrinter)
            Return berhasil
        Finally
            ClosePrinter(hPrinter)
        End Try
    End Function

#End Region

#Region "ALIAS LAMA - Kompatibilitas (hapus setelah semua file dimigrasi)"

    Public Shared Function SendBytesToPrinter(namaPrinter As String,
                                               data As Byte()) As Boolean
        Return KirimKePrinter(namaPrinter, data)
    End Function

#End Region

End Class
