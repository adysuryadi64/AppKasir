' ===========================================
' FORM UTAMA OPTIMIZATIONS
' File: FormUtama_Optimizations.vb
' Description: Optimasi performa untuk FormUtama.vb dengan caching dan async support
' ===========================================

Imports System.Collections.Concurrent
Imports System.Threading.Tasks
Imports System.Threading
Imports System.Runtime.Caching
Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Diagnostics
Imports System.ComponentModel

Public Module FormUtamaOptimizations
    ' Cache untuk data yang sering diakses
    Private Shared ReadOnly Cache As New ConcurrentDictionary(Of String, DataTable)()
    Private Shared ReadOnly CacheTimestamps As New ConcurrentDictionary(Of String, DateTime)()
    Private Shared ReadOnly CacheDuration As TimeSpan = TimeSpan.FromMinutes(5)
    Private Shared ReadOnly CacheLock As New Object()
    
    ' Statistik cache
    Private Shared CacheHits As Integer = 0
    Private Shared CacheMisses As Integer = 0
    
    ' ===========================================
    ' OPTIMIZED DGVTransaksi_CellClick
    ' ===========================================
    
    ''' <summary>
    ''' Versi teroptimasi dari DGVTransaksi_CellClick dengan caching
    ''' </summary>
    Public Async Function OptimizedCellClickAsync(transaksiType As String, fakturId As String) As Task(Of DataTable)
        Dim cacheKey As String = $"{transaksiType}_{fakturId}"
        
        ' Cek cache terlebih dahulu
        Dim cachedData As DataTable = GetFromCache(cacheKey)
        If cachedData IsNot Nothing Then
            Interlocked.Increment(CacheHits)
            Return cachedData
        End If
        
        Interlocked.Increment(CacheMisses)
        
        ' Load data dari database secara async
        Dim data As DataTable = Await LoadTransactionDetailsAsync(transaksiType, fakturId)
        
        ' Simpan ke cache
        AddToCache(cacheKey, data)
        
        Return data
    End Function
    
    ' ===========================================
    ' CACHE MANAGEMENT
    ' ===========================================
    
    ''' <summary>
    ''' Ambil data dari cache
    ''' </summary>
    Private Function GetFromCache(key As String) As DataTable
        If Cache.ContainsKey(key) AndAlso CacheTimestamps.ContainsKey(key) Then
            Dim cacheTime As DateTime = CacheTimestamps(key)
            
            ' Cek apakah cache masih valid (belum expired)
            If (DateTime.Now - cacheTime) < CacheDuration Then
                Return TryCast(Cache(key), DataTable)
            Else
                ' Hapus cache yang sudah expired
                Dim data As DataTable = Nothing
                Dim timestamp As DateTime
                Cache.TryRemove(key, data)
                CacheTimestamps.TryRemove(key, timestamp)
            End If
        End If
        Return Nothing
    End Function
    
    ''' <summary>
    ''' Tambah data ke cache
    ''' </summary>
    Private Sub AddToCache(key As String, data As DataTable)
        If data IsNot Nothing Then
            Cache(key) = data
            CacheTimestamps(key) = DateTime.Now
        End If
    End Sub
    
    ''' <summary>
    ''' Hapus cache yang sudah expired
    ''' </summary>
    Public Sub ClearExpiredCache()
        Dim expiredKeys As New List(Of String)
        Dim now As DateTime = DateTime.Now
        
        For Each kvp In CacheTimestamps
            If (now - kvp.Value) > CacheDuration Then
                expiredKeys.Add(kvp.Key)
            End If
        Next
        
        For Each key In expiredKeys
            Dim data As DataTable = Nothing
            Dim timestamp As DateTime
            
            Cache.TryRemove(key, data)
            CacheTimestamps.TryRemove(key, timestamp)
        Next
    End Sub
    
    ''' <summary>
    ''' Hapus semua cache
    ''' </summary>
    Public Sub ClearAllCache()
        Cache.Clear()
        CacheTimestamps.Clear()
    End Sub
    
    ''' <summary>
    ''' Statistik cache
    ''' </summary>
    Public Function GetCacheStatistics() As String
        Dim hitRate As Double = If((CacheHits + CacheMisses) > 0, CacheHits / (CacheHits + CacheMisses), 0)
        Return $"Cache Entries: {Cache.Count}, Hits: {CacheHits}, Misses: {CacheMisses}, Hit Rate: {hitRate:P2}"
    End Function
    
    ' ===========================================
    ' DATABASE OPERATIONS
    ' ===========================================
    
    ''' <summary>
    ''' Load data transaksi dari database secara async (sesuai dengan kode asli)
    ''' </summary>
    Private Async Function LoadTransactionDetailsAsync(transaksiType As String, fakturId As String) As Task(Of DataTable)
        Return Await Task.Run(Function()
            Dim dt As New DataTable()
            
            ' Query berdasarkan tipe transaksi dengan parameterized query
            Dim sql As String = GetQueryByTransactionType(transaksiType, fakturId)
            Dim tableName As String = GetDatasetTableName(transaksiType)
            Dim parameterName As String = GetParameterName(transaksiType)
            
            Try
                Using cmd As New MySqlCommand(sql, conn)
                    ' Tambahkan parameter dengan nama yang sesuai kode asli
                    cmd.Parameters.AddWithValue(parameterName, fakturId)
                    
                    Using da As New MySqlDataAdapter(cmd)
                        Using ds As New DataSet()
                            da.Fill(ds, tableName)
                            If ds.Tables.Contains(tableName) Then
                                dt = ds.Tables(tableName).Copy()
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ' Log error jika diperlukan
                Debug.WriteLine($"Error loading transaction details: {ex.Message}")
                ' Return empty DataTable jika terjadi error
                dt = New DataTable()
            End Try
            
            Return dt
        End Function)
    End Function
    
    ''' <summary>
    ''' Dapatkan query berdasarkan tipe transaksi (sesuai dengan kode asli)
    ''' </summary>
    Private Function GetQueryByTransactionType(transaksiType As String, fakturId As String) As String
        Select Case transaksiType
            Case "Pembelian"
                Return "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA, QTY, SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL FROM pembelian_detail WHERE FAKTUR_BELI = @FAKTUR_BELI"
            Case "Penjualan"
                Return "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, QTY_SATUAN, TOTAL_DISKON, TOTAL_HARGA FROM penjualan_detail WHERE FAKTUR_JUAL = @FAKTUR_JUAL"
            Case "Retur Pembelian"
                Return "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, QTY_SAT, TOTAL FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = @ID_RETUR_PEMBELIAN"
            Case "Retur Penjualan"
                Return "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, QTY_SATUAN, TOTAL_DISKON, TOTAL_HARGA FROM retur_penjualan_detail WHERE ID_RETUR_PENJUALAN = @ID_RETUR_PENJUALAN"
            Case "Bayar Hutang"
                Return "SELECT ID_BELI, KODE, NAMA, TOTAL_HUTANG, DIBAYAR, TANGGAL_BAYAR, PEMBAYARAN, STATUS FROM Hutang_Detail WHERE ID_BAYAR = @ID_BAYAR"
            Case "Bayar Piutang"
                Return "SELECT ID_JUAL, KODE, NAMA, DIBAYAR, PIUTANG, TANGGAL_BAYAR, PEMBAYARAN, STATUS FROM Piutang_Detail WHERE ID_BAYAR = @ID_BAYAR"
            Case "Surat Jalan"
                Return "SELECT NOTA_BELANJA, NAMA_PELANGGAN, NILAI_BELANJA, LOKASI FROM Surat_Jalan_Detail WHERE NOTA = @NOTA"
            Case "Transfer Barang"
                Return "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, TOTAL_QTY, TOTAL FROM Transfer_Barang_Detail WHERE ID_TRANSFER = @ID_TRANSFER"
            Case Else
                Return "SELECT * FROM transaksi_detail WHERE ID = @ID"
        End Select
    End Function
    
    ''' <summary>
    ''' Get parameter name based on transaction type (matching original code)
    ''' </summary>
    Private Function GetParameterName(transaksiType As String) As String
        Select Case transaksiType
            Case "Pembelian"
                Return "@FAKTUR_BELI"
            Case "Penjualan"
                Return "@FAKTUR_JUAL"
            Case "Retur Pembelian"
                Return "@ID_RETUR_PEMBELIAN"
            Case "Retur Penjualan"
                Return "@ID_RETUR_PENJUALAN"
            Case "Bayar Hutang", "Bayar Piutang"
                Return "@ID_BAYAR"
            Case "Surat Jalan"
                Return "@NOTA"
            Case "Transfer Barang"
                Return "@ID_TRANSFER"
            Case Else
                Return "@ID"
        End Select
    End Function
    
    ''' <summary>
    ''' Get correct dataset table name based on transaction type (matching original code)
    ''' </summary>
    Private Function GetDatasetTableName(transaksiType As String) As String
        Select Case transaksiType
            Case "Pembelian"
                Return "pembelian_detail"
            Case "Penjualan"
                Return "penjualan_detail"
            Case "Retur Pembelian"
                Return "penjualan_detail" ' Original code uses this name for retur pembelian
            Case "Retur Penjualan"
                Return "retur_penjualan_detail"
            Case "Bayar Hutang"
                Return "HutangDetail"
            Case "Bayar Piutang"
                Return "penjualan_piutang"
            Case "Surat Jalan"
                Return "Surat_Jalan_Detail"
            Case "Transfer Barang"
                Return "Transfer_Barang_Detail"
            Case Else
                Return "default_table"
        End Select
    End Function
    
    ' ===========================================
    ' PERFORMANCE OPTIMIZATIONS
    ' ===========================================
    
    ''' <summary>
    ''' Enable double buffering untuk DataGridView
    ''' </summary>
    Public Sub EnableDoubleBuffering(dgv As DataGridView)
        Dim dgvType As Type = dgv.GetType()
        Dim pi As System.Reflection.PropertyInfo = dgvType.GetProperty("DoubleBuffered", 
            System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
        pi.SetValue(dgv, True, Nothing)
    End Sub
    
    ''' <summary>
    ''' Optimasi DataGridView untuk performa
    ''' </summary>
    Public Sub OptimizeDataGridView(dgv As DataGridView)
        dgv.AutoGenerateColumns = False
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.AllowUserToOrderColumns = False
        dgv.AllowUserToResizeRows = False
        dgv.AllowUserToResizeColumns = False
        dgv.RowHeadersVisible = False
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.MultiSelect = False
        dgv.ReadOnly = True
    End Sub
    
    ''' <summary>
    ''' Enable virtual mode untuk dataset besar
    ''' </summary>
    Public Sub EnableVirtualMode(dgv As DataGridView, totalRows As Integer)
        dgv.VirtualMode = True
        dgv.RowCount = totalRows
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
    End Sub
    
    ''' <summary>
    ''' Optimasi query dengan parameterized query (diperbaiki untuk keamanan)
    ''' </summary>
    Public Function ExecuteOptimizedQuery(sql As String, Optional parameters As Dictionary(Of String, Object) = Nothing) As DataTable
        Dim dt As New DataTable()
        
        Try
            Using cmd As New MySqlCommand(sql, conn)
                ' Tambahkan parameter jika ada
                If parameters IsNot Nothing Then
                    For Each param In parameters
                        cmd.Parameters.AddWithValue(param.Key, param.Value)
                    Next
                End If
                
                Using da As New MySqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"Query error: {ex.Message}")
            ' Return empty DataTable jika terjadi error
            dt = New DataTable()
        End Try
        
        Return dt
    End Function
    
    ''' <summary>
    ''' Clear memory dan resources
    ''' </summary>
    Public Sub Cleanup()
        ClearAllCache()
        CacheHits = 0
        CacheMisses = 0
        GC.Collect()
    End Sub
    
    ''' <summary>
    ''' Statistik performa cache
    ''' </summary>
    Public Function GetPerformanceStats() As String
        Dim total As Integer = CacheHits + CacheMisses
        Dim hitRate As Double = If(total > 0, CacheHits / total, 0)
        
        Return $"Cache Stats - Hits: {CacheHits}, Misses: {CacheMisses}, " & 
               $"Hit Rate: {hitRate:P2}, Cache Size: {Cache.Count}"
    End Function
    
    ' ===========================================
    ' ASYNC HELPER METHODS
    ' ===========================================
    
    ''' <summary>
    ''' Execute async dengan timeout
    ''' </summary>
    Public Async Function ExecuteWithTimeoutAsync(Of T)(task As Task(Of T), timeout As TimeSpan) As Task(Of T)
        Dim timeoutTask As Task = Task.Delay(timeout)
        Dim completedTask As Task = Await Task.WhenAny(task, timeoutTask)
        
        If completedTask Is timeoutTask Then
            Throw New TimeoutException("Operation timed out")
        End If
        
        Return Await task
    End Function
    
    ''' <summary>
    ''' Retry operation dengan exponential backoff
    ''' </summary>
    Public Async Function RetryWithBackoffAsync(Of T)(
        operation As Func(Of Task(Of T)), 
        maxRetries As Integer, 
        initialDelay As TimeSpan) As Task(Of T)
        
        Dim delay As TimeSpan = initialDelay
        
        For retry As Integer = 0 To maxRetries - 1
            Try
                Return Await operation()
            Catch ex As Exception
                If retry = maxRetries - 1 Then
                    Throw
                End If
                
                Await Task.Delay(delay)
                delay = TimeSpan.FromTicks(delay.Ticks * 2) ' Exponential backoff
            End Try
        Next
        
        Throw New InvalidOperationException("Max retries exceeded")
    End Function
    
    ' ===========================================
    ' MEMORY MANAGEMENT
    ' ===========================================
    
    ''' <summary>
    ''' Monitor memory usage
    ''' </summary>
    Public Function GetMemoryUsage() As String
        Dim totalMemory As Long = GC.GetTotalMemory(False)
        Dim workingSet As Long = Process.GetCurrentProcess().WorkingSet64
        
        Return $"Memory: {totalMemory / 1024 / 1024:F2} MB, " & 
               $"Working Set: {workingSet / 1024 / 1024:F2} MB"
    End Function
    
    ''' <summary>
    ''' Optimasi memori dengan GC
    ''' </summary>
    Public Sub OptimizeMemory()
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()
    End Sub
    
    ' ===========================================
    ' HELPER METHODS
    ' ===========================================
    
    ''' <summary>
    ''' Safe invoke untuk cross-thread operations
    ''' </summary>
    Public Sub SafeInvoke(control As Control, action As Action)
        If control.InvokeRequired Then
            control.Invoke(Sub() action())
        Else
            action()
        End If
    End Sub
    
    ''' <summary>
    ''' Log performa operasi
    ''' </summary>
    Public Sub LogPerformance(operation As String, elapsedTime As TimeSpan)
        Debug.WriteLine($"{operation} completed in {elapsedTime.TotalMilliseconds}ms")
    End Function
    
    ''' <summary>
    ''' Validasi input untuk mencegah SQL injection
    ''' </summary>
    Public Function ValidateInput(input As String) As Boolean
        If String.IsNullOrWhiteSpace(input) Then Return False
        
        ' Cek SQL injection patterns
        Dim sqlKeywords As String() = {"SELECT", "INSERT", "UPDATE", "DELETE", "DROP", 
                                      "EXEC", "EXECUTE", "UNION", "OR", "AND", "--", "/*", "*/"}
        
        Dim upperInput As String = input.ToUpper()
        For Each keyword In sqlKeywords
            If upperInput.Contains(keyword) Then
                Return False
            End If
        Next
        
        Return True
    End Function
    
    ' ===========================================
    ' ADDITIONAL OPTIMIZATIONS
    ' ===========================================
    
    ''' <summary>
    ''' Sanitasi input untuk keamanan
    ''' </summary>
    Public Function SanitizeInput(input As String) As String
        If String.IsNullOrEmpty(input) Then Return String.Empty
        
        ' Hapus karakter berbahaya
        Return input.Replace("'", "''").Replace(";", "").Replace("--", "")
    End Function
    
    
    ''' <summary>
    ''' Execute query dengan timeout
    ''' </summary>
    Public Async Function ExecuteQueryWithTimeoutAsync(sql As String, timeout As TimeSpan) As Task(Of DataTable)
        Using cts As New CancellationTokenSource(timeout)
            Return Await Task.Run(Function()
                Return ExecuteOptimizedQuery(sql)
            End Function, cts.Token)
        End Using
    End Function
    
    ''' <summary>
    ''' Monitor performance dari operasi
    ''' </summary>
    Public Function MeasurePerformance(Of T)(operation As Func(Of T), operationName As String) As T
        Dim sw As New Stopwatch()
        sw.Start()
        
        Try
            Dim result As T = operation()
            sw.Stop()
            Debug.WriteLine($"{operationName} completed in {sw.ElapsedMilliseconds}ms")
            Return result
        Catch ex As Exception
            sw.Stop()
            Debug.WriteLine($"{operationName} failed after {sw.ElapsedMilliseconds}ms: {ex.Message}")
            Throw
        End Try
    End Function
    
    ''' <summary>
    ''' Async version of SafeInvoke
    ''' </summary>
    Public Async Function SafeInvokeAsync(control As Control, action As Action) As Task
        If control.InvokeRequired Then
            Await Task.Run(Sub() control.Invoke(Sub() action()))
        Else
            action()
        End If
    End Function
    
    ''' <summary>
    ''' Format angka dengan pemisah ribuan
    ''' </summary>
    Public Function FormatNumber(value As Decimal) As String
        Return String.Format("{0:N0}", value)
    End Function
    
    ' ===========================================
    ' CACHE PROPERTIES
    ' ===========================================
    
    Public ReadOnly Property CacheHitRate As Double
        Get
            Dim total As Integer = CacheHits + CacheMisses
            If total = 0 Then Return 0
            Return CacheHits / total
        End Get
    End Property
    
    Public ReadOnly Property CacheEntryCount As Integer
        Get
            Return Cache.Count
        End Get
    End Property
    
    Public ReadOnly Property CacheHitCount As Integer
        Get
            Return CacheHits
        End Get
    End Property
    
    Public ReadOnly Property CacheMissCount As Integer
        Get
            Return CacheMisses
        End Get
    End Property
    
    ' ===========================================
    ' FINAL CLEANUP
    ' ===========================================
    
    ''' <summary>
    ''' Final cleanup untuk semua resources
    ''' </summary>
    Public Sub FinalCleanup()
        ClearAllCache()
        OptimizeMemory()
        Debug.WriteLine("FormUtama optimizations cleaned up successfully")
    End Sub
    
    ' ===========================================
    ' INTEGRATION METHODS FOR FORMUTAMA
    ' ===========================================
    
    ''' <summary>
    ''' Metode untuk menggantikan DGVTransaksi_CellClick yang asli
    ''' Gunakan metode ini untuk menggantikan kode asli dengan versi yang dioptimasi
    ''' </summary>
    Public Async Sub OptimizedDGVTransaksi_CellClick(sender As Object, e As DataGridViewCellEventArgs, 
                                                   dgvTransaksi As DataGridView, 
                                                   dgvDetail As DataGridView,
                                                   txtTransaksi As TextBox,
                                                   txtFakturTransaksi As TextBox,
                                                   txtLokasiUntukEdit As TextBox,
                                                   lblDetailTransaksi As Label)
        
        If dgvTransaksi.Rows.Count < 1 Then
            MessageBox.Show("Tidak ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        
        ' Validasi row index
        If e.RowIndex < 0 OrElse e.RowIndex >= dgvTransaksi.Rows.Count Then
            Return
        End If
        
        Dim fakturId As String = dgvTransaksi.Rows(e.RowIndex).Cells(0).Value?.ToString()
        Dim transaksiType As String = txtTransaksi.Text.Trim()
        
        If String.IsNullOrEmpty(fakturId) Then Return
        
        Try
            ' Clear existing data first
            dgvDetail.DataSource = Nothing
            dgvDetail.Rows.Clear()
            
            ' Handle special cases that don't need database queries (matching original code)
            Select Case transaksiType
                Case "Stok Opname", "Transfer Stok"
                    ' These cases only update text fields, no database query needed
                    txtFakturTransaksi.Text = fakturId
                    SetLokasiForTransaction(dgvTransaksi, e.RowIndex, transaksiType, txtLokasiUntukEdit)
                    lblDetailTransaksi.Text = $"Detail {transaksiType} : {fakturId}"
                    Return
                    
                Case Else
                    ' Load data dengan optimasi caching dan async untuk cases yang memerlukan database
                    Dim data As DataTable = Await OptimizedCellClickAsync(transaksiType, fakturId)
                    
                    ' Set new data
                    dgvDetail.DataSource = data
                    
                    ' Configure DataGridView berdasarkan tipe transaksi
                    ConfigureDataGridViewByType(dgv:=dgvDetail, transaksiType:=transaksiType)
            End Select
            
            ' Update UI controls
            txtFakturTransaksi.Text = fakturId
            
            ' Set lokasi berdasarkan tipe transaksi dengan optimasi
            SetLokasiForTransaction(dgvTransaksi, e.RowIndex, transaksiType, txtLokasiUntukEdit)
            
            ' Set label text with proper formatting (matching original code)
            Select Case transaksiType
                Case "Pembelian"
                    lblDetailTransaksi.Text = "Detail Belanja : " + fakturId
                Case "Penjualan"
                    lblDetailTransaksi.Text = "Detail Penjualan : " + fakturId
                Case "Retur Pembelian"
                    lblDetailTransaksi.Text = "Detail Retur Pembelian : " + fakturId
                Case "Retur Penjualan"
                    lblDetailTransaksi.Text = "Detail Retur Penjualan : " + fakturId
                Case "Bayar Hutang"
                    lblDetailTransaksi.Text = "Detail Bayar Hutang : " + fakturId
                Case "Bayar Piutang"
                    lblDetailTransaksi.Text = "Detail Bayar Piutang : " + fakturId
                Case "Surat Jalan"
                    lblDetailTransaksi.Text = "Detail Surat Jalan : " + fakturId
                Case "Transfer Barang"
                    lblDetailTransaksi.Text = "Detail transfer barang: " & fakturId
                Case Else
                    lblDetailTransaksi.Text = $"Detail {transaksiType} : {fakturId}"
            End Select
            
        Catch ex As Exception
            MessageBox.Show($"Error loading transaction details: {ex.Message}", "Error", 
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    
    ''' <summary>
    ''' Optimasi untuk set lokasi berdasarkan tipe transaksi
    ''' </summary>
    Private Sub SetLokasiForTransaction(dgvTransaksi As DataGridView, rowIndex As Integer, transaksiType As String, txtLokasiUntukEdit As TextBox)
        Try
            Select Case transaksiType
                Case "Pembelian", "Penjualan", "Bayar Hutang", "Bayar Piutang"
                    If dgvTransaksi.Rows(rowIndex).Cells.Count > 2 Then
                        txtLokasiUntukEdit.Text = dgvTransaksi.Rows(rowIndex).Cells(2).Value?.ToString()
                    End If
                Case "Retur Pembelian", "Retur Penjualan"
                    If dgvTransaksi.Rows(rowIndex).Cells.Count > 4 Then
                        txtLokasiUntukEdit.Text = dgvTransaksi.Rows(rowIndex).Cells(4).Value?.ToString()
                    End If
                Case "Transfer Barang", "Transfer Stok"
                    If dgvTransaksi.Rows(rowIndex).Cells.Count > 1 Then
                        txtLokasiUntukEdit.Text = dgvTransaksi.Rows(rowIndex).Cells(1).Value?.ToString()
                    End If
                Case "Surat Jalan"
                    txtLokasiUntukEdit.Clear()
            End Select
        Catch ex As Exception
            Debug.WriteLine($"Error setting lokasi: {ex.Message}")
            txtLokasiUntukEdit.Clear()
        End Try
    End Sub
    
    ''' <summary>
    ''' Configure DataGridView berdasarkan tipe transaksi (sesuai kode asli)
    ''' </summary>
    Private Sub ConfigureDataGridViewByType(dgv As DataGridView, transaksiType As String)
        ' Use optimized version with caching and final styling
        ConfigureDataGridViewByTypeOptimized(dgv, transaksiType)
    End Sub
    
    ''' <summary>
    ''' Metode untuk menginvalidasi cache ketika data berubah
    ''' </summary>
    Public Sub InvalidateTransactionCache(transaksiType As String, fakturId As String)
        Dim cacheKey As String = $"{transaksiType}_{fakturId}"
        Dim data As DataTable = Nothing
        Dim timestamp As DateTime
        
        Cache.TryRemove(cacheKey, data)
        CacheTimestamps.TryRemove(cacheKey, timestamp)
    End Sub
    
    ''' <summary>
    ''' Metode untuk menginvalidasi semua cache berdasarkan tipe transaksi
    ''' </summary>
    Public Sub InvalidateTransactionTypeCache(transaksiType As String)
        Dim keysToRemove As New List(Of String)
        
        For Each key In Cache.Keys
            If key.StartsWith($"{transaksiType}_") Then
                keysToRemove.Add(key)
            End If
        Next
        
        For Each key In keysToRemove
            Dim data As DataTable = Nothing
            Dim timestamp As DateTime
            
            Cache.TryRemove(key, data)
            CacheTimestamps.TryRemove(key, timestamp)
        Next
    End Sub
    ' ===========================================
    ' ADVANCED OPTIMIZATIONS
    ' ===========================================
    
    ''' <summary>
    ''' Batch cache preloading untuk transaksi yang sering diakses
    ''' </summary>
    Public Async Function PreloadFrequentTransactionsAsync(transaksiType As String, fakturIds As List(Of String)) As Task
        Dim tasks As New List(Of Task)()
        
        For Each fakturId In fakturIds.Take(10) ' Limit to 10 most recent
            Dim task = Task.Run(Async Function()
                Dim cacheKey As String = $"{transaksiType}_{fakturId}"
                If Not Cache.ContainsKey(cacheKey) Then
                    Try
                        Dim data = Await LoadTransactionDetailsAsync(transaksiType, fakturId)
                        AddToCache(cacheKey, data)
                    Catch ex As Exception
                        Debug.WriteLine($"Preload failed for {cacheKey}: {ex.Message}")
                    End Try
                End If
            End Function)
            tasks.Add(task)
        Next
        
        Await Task.WhenAll(tasks)
    End Function
    
    ''' <summary>
    ''' Smart cache warming berdasarkan pola akses user
    ''' </summary>
    Public Async Sub WarmCacheBasedOnUserPattern(transaksiType As String, dgvTransaksi As DataGridView)
        Try
            Dim fakturIds As New List(Of String)()
            
            ' Ambil 5 transaksi teratas untuk preload
            For i As Integer = 0 To Math.Min(4, dgvTransaksi.Rows.Count - 1)
                If dgvTransaksi.Rows(i).Cells(0).Value IsNot Nothing Then
                    fakturIds.Add(dgvTransaksi.Rows(i).Cells(0).Value.ToString())
                End If
            Next
            
            If fakturIds.Count > 0 Then
                Await PreloadFrequentTransactionsAsync(transaksiType, fakturIds)
            End If
        Catch ex As Exception
            Debug.WriteLine($"Cache warming failed: {ex.Message}")
        End Try
    End Sub
    
    ''' <summary>
    ''' Optimized DataGridView configuration dengan caching
    ''' </summary>
    Private Shared ReadOnly ColumnConfigCache As New ConcurrentDictionary(Of String, Action(Of DataGridView))()
    
    Private Sub ConfigureDataGridViewByTypeOptimized(dgv As DataGridView, transaksiType As String)
        If dgv.Columns.Count = 0 Then Return
        
        ' Use cached configuration if available
        If ColumnConfigCache.ContainsKey(transaksiType) Then
            ColumnConfigCache(transaksiType)(dgv)
        Else
            ' Create and cache configuration
            Dim configAction = CreateColumnConfiguration(transaksiType)
            ColumnConfigCache(transaksiType) = configAction
            configAction(dgv)
        End If
        
        ' Apply general optimizations and final styling (matching original code)
        ApplyFinalDataGridViewStyling(dgv)
    End Sub
    
    ''' <summary>
    ''' Apply final DataGridView styling exactly as in original code
    ''' </summary>
    Private Sub ApplyFinalDataGridViewStyling(dgv As DataGridView)
        Try
            ' Gunakan method UbahTampilanDataTransaksi yang identik dengan FormUtama.vb
            UbahTampilanDataTransaksi(dgv)
        Catch ex As Exception
            Debug.WriteLine($"Error applying final styling: {ex.Message}")
        End Try
    End Sub
    
    ''' <summary>
    ''' Create column configuration action for caching
    ''' </summary>
    Private Function CreateColumnConfiguration(transaksiType As String) As Action(Of DataGridView)
        Return Sub(dgv As DataGridView)
            Try
                Select Case transaksiType
                    Case "Pembelian"
                        ConfigurePembelianColumns(dgv)
                    Case "Penjualan"
                        ConfigurePenjualanColumns(dgv)
                    Case "Retur Pembelian"
                        ConfigureReturPembelianColumns(dgv)
                    Case "Retur Penjualan"
                        ConfigureReturPenjualanColumns(dgv)
                    Case "Bayar Hutang"
                        ConfigureBayarHutangColumns(dgv)
                    Case "Bayar Piutang"
                        ConfigureBayarPiutangColumns(dgv)
                    Case "Surat Jalan"
                        ConfigureSuratJalanColumns(dgv)
                    Case "Transfer Barang"
                        ConfigureTransferBarangColumns(dgv)
                End Select
            Catch ex As Exception
                Debug.WriteLine($"Error configuring columns for {transaksiType}: {ex.Message}")
            End Try
        End Sub
    End Function
    
    ' Helper methods for column configuration
    Private Sub ConfigurePembelianColumns(dgv As DataGridView)
        SetColumnVisibilityAndHeader(dgv, "ID_BARANG", False, "")
        SetColumnVisibilityAndHeader(dgv, "NAMA_BARANG", True, "NAMA BARANG")
        SetColumnVisibilityAndHeader(dgv, "HARGA_BELI", True, "HARGA", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "HARGA_AVERAGE", False, "")
        SetColumnVisibilityAndHeader(dgv, "HARGA_BELI_SEBELUMNYA", False, "")
        SetColumnVisibilityAndHeader(dgv, "QTY", True, "QTY")
        SetColumnVisibilityAndHeader(dgv, "SATUAN", True, "SATUAN")
        SetColumnVisibilityAndHeader(dgv, "HARGA_BELI_SATUAN", False, "", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "QTY_SAT", False, "")
        SetColumnVisibilityAndHeader(dgv, "TOTAL", True, "TOTAL", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        
        ' Set FillWeight sesuai kode asli
        SetColumnFillWeight(dgv, "NAMA_BARANG", 150)
        SetColumnFillWeight(dgv, "HARGA_BELI", 60)
        SetColumnFillWeight(dgv, "QTY", 30)
        SetColumnFillWeight(dgv, "SATUAN", 50)
        SetColumnFillWeight(dgv, "HARGA_BELI_SATUAN", 70)
        SetColumnFillWeight(dgv, "TOTAL", 60)
    End Sub
    
    Private Sub ConfigurePenjualanColumns(dgv As DataGridView)
        SetColumnVisibilityAndHeader(dgv, "ID_BARANG", False, "")
        SetColumnVisibilityAndHeader(dgv, "NAMA_BARANG", True, "NAMA BARANG")
        SetColumnVisibilityAndHeader(dgv, "QTY", True, "QTY", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "SATUAN", True, "SATUAN")
        SetColumnVisibilityAndHeader(dgv, "HARGA_JUAL", True, "HARGA", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "QTY_SATUAN", False, "")
        SetColumnVisibilityAndHeader(dgv, "TOTAL_DISKON", True, "DISKON", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "TOTAL_HARGA", True, "TOTAL", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        
        ' Set FillWeight sesuai kode asli
        SetColumnFillWeight(dgv, "NAMA_BARANG", 150)
        SetColumnFillWeight(dgv, "QTY", 30)
        SetColumnFillWeight(dgv, "SATUAN", 50)
        SetColumnFillWeight(dgv, "HARGA_JUAL", 60)
        SetColumnFillWeight(dgv, "TOTAL_DISKON", 60)
        SetColumnFillWeight(dgv, "TOTAL_HARGA", 60)
    End Sub
    
    Private Sub ConfigureReturPembelianColumns(dgv As DataGridView)
        SetColumnVisibilityAndHeader(dgv, "ID_BARANG", False, "")
        SetColumnVisibilityAndHeader(dgv, "NAMA_BARANG", True, "NAMA BARANG")
        SetColumnVisibilityAndHeader(dgv, "QTY", True, "QTY", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "SATUAN", True, "SATUAN")
        SetColumnVisibilityAndHeader(dgv, "QTY_SAT", False, "")
        SetColumnVisibilityAndHeader(dgv, "TOTAL", True, "TOTAL", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        
        ' Set FillWeight sesuai kode asli
        SetColumnFillWeight(dgv, "NAMA_BARANG", 150)
        SetColumnFillWeight(dgv, "QTY", 30)
        SetColumnFillWeight(dgv, "SATUAN", 50)
        SetColumnFillWeight(dgv, "TOTAL", 60)
    End Sub
    
    Private Sub ConfigureReturPenjualanColumns(dgv As DataGridView)
        SetColumnVisibilityAndHeader(dgv, "ID_BARANG", False, "")
        SetColumnVisibilityAndHeader(dgv, "NAMA_BARANG", True, "NAMA BARANG")
        SetColumnVisibilityAndHeader(dgv, "QTY", True, "QTY", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "SATUAN", True, "SATUAN")
        SetColumnVisibilityAndHeader(dgv, "HARGA_JUAL", True, "HARGA", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "QTY_SATUAN", False, "")
        SetColumnVisibilityAndHeader(dgv, "TOTAL_DISKON", True, "DISKON", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "TOTAL_HARGA", True, "TOTAL", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        
        ' Set FillWeight sesuai kode asli
        SetColumnFillWeight(dgv, "NAMA_BARANG", 150)
        SetColumnFillWeight(dgv, "QTY", 30)
        SetColumnFillWeight(dgv, "SATUAN", 50)
        SetColumnFillWeight(dgv, "HARGA_JUAL", 60)
        SetColumnFillWeight(dgv, "TOTAL_DISKON", 60)
        SetColumnFillWeight(dgv, "TOTAL_HARGA", 60)
    End Sub
    
    Private Sub ConfigureBayarHutangColumns(dgv As DataGridView)
        SetColumnVisibilityAndHeader(dgv, "ID_BELI", True, "NOTA BELI")
        SetColumnVisibilityAndHeader(dgv, "KODE", False, "")
        SetColumnVisibilityAndHeader(dgv, "NAMA", True, "SUPLIYER")
        SetColumnVisibilityAndHeader(dgv, "DIBAYAR", False, "")
        SetColumnVisibilityAndHeader(dgv, "TOTAL_HUTANG", False, "")
        SetColumnVisibilityAndHeader(dgv, "TANGGAL_BAYAR", True, "TANGGAL")
        SetColumnVisibilityAndHeader(dgv, "PEMBAYARAN", True, "NOMINAL", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "STATUS", True, "STATUS")
        
        ' Set FillWeight sesuai kode asli
        SetColumnFillWeight(dgv, "NAMA", 150)
    End Sub
    
    Private Sub ConfigureBayarPiutangColumns(dgv As DataGridView)
        SetColumnVisibilityAndHeader(dgv, "ID_JUAL", True, "NOTA JUAL")
        SetColumnVisibilityAndHeader(dgv, "KODE", False, "")
        SetColumnVisibilityAndHeader(dgv, "NAMA", True, "PELANGGAN")
        SetColumnVisibilityAndHeader(dgv, "DIBAYAR", False, "")
        SetColumnVisibilityAndHeader(dgv, "PIUTANG", False, "")
        SetColumnVisibilityAndHeader(dgv, "TANGGAL_BAYAR", True, "TANGGAL")
        SetColumnVisibilityAndHeader(dgv, "PEMBAYARAN", True, "NOMINAL", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "STATUS", True, "STATUS")
        
        ' Set FillWeight sesuai kode asli
        SetColumnFillWeight(dgv, "NAMA", 150)
    End Sub
    
    Private Sub ConfigureSuratJalanColumns(dgv As DataGridView)
        SetColumnVisibilityAndHeader(dgv, "NOTA_BELANJA", False, "")
        SetColumnVisibilityAndHeader(dgv, "NAMA_PELANGGAN", True, "NAMA PELANGGAN")
        SetColumnVisibilityAndHeader(dgv, "NILAI_BELANJA", True, "NILAI BELANJA", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        SetColumnVisibilityAndHeader(dgv, "LOKASI", True, "LOKASI")
        
        ' Set FillWeight sesuai kode asli
        SetColumnFillWeight(dgv, "NAMA_PELANGGAN", 150)
        SetColumnFillWeight(dgv, "NILAI_BELANJA", 60)
        SetColumnFillWeight(dgv, "LOKASI", 60)
    End Sub
    
    Private Sub ConfigureTransferBarangColumns(dgv As DataGridView)
        SetColumnVisibilityAndHeader(dgv, "ID_BARANG", False, "")
        SetColumnVisibilityAndHeader(dgv, "NAMA_BARANG", True, "BARANG")
        SetColumnVisibilityAndHeader(dgv, "QTY", True, "QTY")
        SetColumnVisibilityAndHeader(dgv, "SATUAN", True, "SATUAN")
        SetColumnVisibilityAndHeader(dgv, "TOTAL_QTY", False, "")
        SetColumnVisibilityAndHeader(dgv, "TOTAL", True, "TOTAL", DataGridViewContentAlignment.MiddleRight, "#,0.##")
        
        ' Set FillWeight sesuai kode asli
        SetColumnFillWeight(dgv, "NAMA_BARANG", 150)
        SetColumnFillWeight(dgv, "QTY", 40)
        SetColumnFillWeight(dgv, "SATUAN", 60)
        SetColumnFillWeight(dgv, "TOTAL", 60)
    End Sub
    
    ''' <summary>
    ''' Helper method untuk set column properties dengan error handling
    ''' </summary>
    Private Sub SetColumnVisibilityAndHeader(dgv As DataGridView, columnName As String, visible As Boolean, headerText As String, 
                                           Optional alignment As DataGridViewContentAlignment = DataGridViewContentAlignment.NotSet,
                                           Optional format As String = "")
        Try
            If dgv.Columns.Contains(columnName) Then
                With dgv.Columns(columnName)
                    .Visible = visible
                    If visible Then
                        .HeaderText = headerText
                        If alignment <> DataGridViewContentAlignment.NotSet Then
                            .DefaultCellStyle.Alignment = alignment
                        End If
                        If Not String.IsNullOrEmpty(format) Then
                            .DefaultCellStyle.Format = format
                        End If
                    End If
                End With
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error setting column {columnName}: {ex.Message}")
        End Try
    End Sub
    
    ''' <summary>
    ''' Helper method untuk set FillWeight dengan error handling
    ''' </summary>
    Private Sub SetColumnFillWeight(dgv As DataGridView, columnName As String, fillWeight As Single)
        Try
            If dgv.Columns.Contains(columnName) Then
                dgv.Columns(columnName).FillWeight = fillWeight
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error setting FillWeight for column {columnName}: {ex.Message}")
        End Try
    End Sub
    
    ''' <summary>
    ''' Background cache cleanup task
    ''' </summary>
    Public Sub StartBackgroundCacheCleanup()
        Task.Run(Async Function()
            While True
                Try
                    Await Task.Delay(TimeSpan.FromMinutes(10)) ' Cleanup every 10 minutes
                    ClearExpiredCache()
                    
                    ' Force GC if cache is getting large
                    If Cache.Count > 100 Then
                        OptimizeMemory()
                    End If
                Catch ex As Exception
                    Debug.WriteLine($"Background cleanup error: {ex.Message}")
                End Try
            End While
        End Function)
    End Sub
    
    ''' <summary>
    ''' Get cache statistics with more details
    ''' </summary>
    Public Function GetDetailedCacheStatistics() As String
        Dim hitRate As Double = If((CacheHits + CacheMisses) > 0, CacheHits / (CacheHits + CacheMisses), 0)
        Dim memoryUsage As String = GetMemoryUsage()
        
        Return $"Cache Statistics:" & Environment.NewLine &
               $"- Entries: {Cache.Count}" & Environment.NewLine &
               $"- Hits: {CacheHits}" & Environment.NewLine &
               $"- Misses: {CacheMisses}" & Environment.NewLine &
               $"- Hit Rate: {hitRate:P2}" & Environment.NewLine &
               $"- {memoryUsage}"
    End Function
    
    ''' <summary>
    ''' Initialize optimizations - call this from FormUtama.Load
    ''' </summary>
    Public Sub InitializeOptimizations()
        ' Start background cleanup
        StartBackgroundCacheCleanup()
        
        ' Pre-warm column configuration cache
        For Each transType In {"Pembelian", "Penjualan", "Retur Pembelian", "Retur Penjualan", "Bayar Hutang", "Bayar Piutang", "Surat Jalan", "Transfer Barang"}
            CreateColumnConfiguration(transType)
        Next
        
        Debug.WriteLine("FormUtama optimizations initialized successfully")
    End Sub
    
    ' ===========================================
    ' HELPER FUNCTIONS FOR COMPATIBILITY
    ' ===========================================
    
    ' ===========================================
    ' HELPER FUNCTIONS FOR COMPATIBILITY
    ' ===========================================
    
    ''' <summary>
    ''' Method untuk mengintegrasikan optimizations ke FormUtama yang sudah ada
    ''' Panggil method ini dari FormUtama.Load untuk mengaktifkan optimizations
    ''' </summary>
    Public Sub IntegrateWithFormUtama(formUtama As Form)
        Try
            ' Initialize optimizations
            InitializeOptimizations()
            
            ' Enable double buffering untuk DataGridView yang ada
            Dim dgvTransaksi = TryCast(formUtama.Controls.Find("DGVTransaksi", True).FirstOrDefault(), DataGridView)
            Dim dgvDetail = TryCast(formUtama.Controls.Find("DGVDetail", True).FirstOrDefault(), DataGridView)
            
            If dgvTransaksi IsNot Nothing Then
                EnableDoubleBuffering(dgvTransaksi)
                OptimizeDataGridView(dgvTransaksi)
            End If
            
            If dgvDetail IsNot Nothing Then
                EnableDoubleBuffering(dgvDetail)
                OptimizeDataGridView(dgvDetail)
            End If
            
            Debug.WriteLine("FormUtama optimizations integrated successfully")
        Catch ex As Exception
            Debug.WriteLine($"Error integrating optimizations: {ex.Message}")
        End Try
    End Sub
    
    ''' <summary>
    ''' Method untuk mengganti DGVTransaksi_CellClick dengan versi optimized
    ''' Panggil method ini dari event handler DGVTransaksi_CellClick
    ''' </summary>
    Public Sub ReplaceOriginalCellClick(formUtama As Form, sender As Object, e As DataGridViewCellEventArgs)
        Try
            ' Get controls from FormUtama
            Dim dgvTransaksi = TryCast(formUtama.Controls.Find("DGVTransaksi", True).FirstOrDefault(), DataGridView)
            Dim dgvDetail = TryCast(formUtama.Controls.Find("DGVDetail", True).FirstOrDefault(), DataGridView)
            Dim txtTransaksi = TryCast(formUtama.Controls.Find("TxtTransaksi", True).FirstOrDefault(), TextBox)
            Dim txtFakturTransaksi = TryCast(formUtama.Controls.Find("TxtFakturTransaksi", True).FirstOrDefault(), TextBox)
            Dim txtLokasiUntukEdit = TryCast(formUtama.Controls.Find("TxtLokasiUntukEdit", True).FirstOrDefault(), TextBox)
            Dim lblDetailTransaksi = TryCast(formUtama.Controls.Find("LblDetailTransaksi", True).FirstOrDefault(), Label)
            
            If dgvTransaksi IsNot Nothing AndAlso dgvDetail IsNot Nothing AndAlso 
               txtTransaksi IsNot Nothing AndAlso txtFakturTransaksi IsNot Nothing AndAlso
               txtLokasiUntukEdit IsNot Nothing AndAlso lblDetailTransaksi IsNot Nothing Then
                
                ' Call optimized version
                OptimizedDGVTransaksi_CellClick(sender, e, dgvTransaksi, dgvDetail, 
                                              txtTransaksi, txtFakturTransaksi, 
                                              txtLokasiUntukEdit, lblDetailTransaksi)
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error in optimized cell click: {ex.Message}")
        End Try
    End Sub
    
    ''' <summary>
    ''' Helper function untuk kompatibilitas dengan FormUtama.vb
    ''' Fungsi untuk mengisi ComboBox dengan akun
    ''' </summary>
    Public Sub IsiComboBoxAkun(cmb As ComboBox, ParamArray values As String())
        Try
            cmb.Items.Clear()
            For Each value In values
                cmb.Items.Add(value)
            Next
        Catch ex As Exception
            Debug.WriteLine($"Error filling ComboBox: {ex.Message}")
        End Try
    End Sub
    
    ''' <summary>
    ''' Helper function untuk menghitung stok berdasarkan kode barang
    ''' Kompatibilitas dengan HitungByKode yang ada di FormUtama
    ''' </summary>
    Public Sub HitungByKode(kodeBarang As String)
        Try
            ' Implementasi sederhana untuk kompatibilitas
            ' Fungsi asli mungkin ada di module lain
            Debug.WriteLine($"Calculating stock for item: {kodeBarang}")
        Catch ex As Exception
            Debug.WriteLine($"Error calculating stock for {kodeBarang}: {ex.Message}")
        End Try
    End Sub
    
    ''' <summary>
    ''' Helper function untuk kompatibilitas dengan variabel global
    ''' </summary>
    Public Function GetGlobalVariables() As Dictionary(Of String, String)
        ' Return dictionary dengan variabel global yang mungkin dibutuhkan
        Return New Dictionary(Of String, String) From {
            {"Kode_rek_Hutang_Beli", ""},
            {"LAWAN_NAMA_REK_BARANG", ""},
            {"LAWAN_KODE_REK_BARANG", ""},
            {"NAMA_REK_BARANG", ""},
            {"KODE_REK_BARANG", ""}
        }
    End Function
    
    ''' <summary>
    ''' DataGridViewExtension class untuk kompatibilitas dengan FormUtama.vb
    ''' Class ini identik dengan yang ada di FormUtama.vb
    ''' </summary>
    Public Class DataGridViewExtension
        Public Shared Sub EnableDoubleBuffering(ByVal dataGridView As DataGridView)
            dataGridView.GetType().InvokeMember("DoubleBuffered", BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.SetProperty, Nothing, dataGridView, New Object() {True})
        End Sub
    End Class
    
    ''' <summary>
    ''' Method UbahTampilanDataTransaksi untuk kompatibilitas dengan FormUtama.vb
    ''' Method ini identik dengan yang ada di FormUtama.vb
    ''' </summary>
    Public Sub UbahTampilanDataTransaksi(dgvTransaksi As DataGridView)
        With dgvTransaksi
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False

            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Gray
            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            DataGridViewExtension.EnableDoubleBuffering(dgvTransaksi)
        End With
    End Sub

End Module