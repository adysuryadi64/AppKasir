Imports MySql.Data.MySqlClient

''' <summary>
''' ModuleVerifikasiJurnal — Verifikasi kebenaran saldo tbl_datareferensi dari JurnalUmum.
'''
''' Mekanisme sama dengan HITUNGSEMUASALDO / HITUNGSALDOAWAL di FormLapNeracaLR,
''' tapi hasilnya TIDAK ditulis ke tbl_datareferensi — hanya dibaca ke memori
''' untuk dibandingkan dengan nilai yang tersimpan saat ini.
'''
''' Prinsip double-entry (referensi: accountinginsights.org, superfastcpa.com):
'''   - Total Debet JurnalUmum = Total Kredit JurnalUmum (keseimbangan jurnal)
'''   - Akun DEBET  normal: Saldo = Saldo_Awal + Total_D - Total_K
'''   - Akun KREDIT normal: Saldo = Saldo_Awal - Total_D + Total_K
'''
''' Formula Laba Rugi (dari COA aktual db_kasirlancar):
'''   Pendapatan Kotor  = PENJUALAN (05.02.001) - RETUR PENJUALAN (05.03.001) - DISKON PENJUALAN (05.04.001)
'''   HPP               = HPP POKOK (06.01.001) + ANGKUT BELI (06.02.001) + ANGKUT JUAL (06.03.001)
'''                       + PENY. STOK (06.04.001) - DISKON BELI (06.05.001) - RETUR BELI (06.06.001)
'''   Laba Kotor        = Pendapatan Kotor - HPP
'''   Biaya Operasional = SUM semua akun JENIS_AKUN='BIAYA'
'''   Pendapatan Lain   = SUM semua akun JENIS_AKUN='PENDAPATAN LAIN'
'''   Beban Pajak       = SUM semua akun JENIS_AKUN='PAJAK'
'''   Laba Bersih       = Laba Kotor - Biaya Operasional + Pendapatan Lain - Beban Pajak
''' </summary>
Module ModuleVerifikasiJurnal

    ''' <summary>Hasil verifikasi satu akun.</summary>
    Public Class HasilAkun
        Public Property KodeAkun As String
        Public Property NamaAkun As String
        Public Property TypeAkun As String
        Public Property JenisAkun As String
        Public Property SubAkun As String
        Public Property AkunDK As String
        Public Property SaldoAwal As Decimal
        Public Property TotalDebet As Decimal
        Public Property TotalKredit As Decimal
        ''' <summary>Saldo yang dihitung ulang dari JurnalUmum (nilai kebenaran).</summary>
        Public Property SaldoHitung As Decimal
        ''' <summary>Saldo yang tersimpan di tbl_datareferensi saat ini.</summary>
        Public Property SaldoTersimpan As Decimal
        ''' <summary>True jika SaldoHitung = SaldoTersimpan (dalam toleransi 1 rupiah).</summary>
        Public ReadOnly Property Cocok As Boolean
            Get
                Return Math.Abs(SaldoHitung - SaldoTersimpan) < 1D
            End Get
        End Property
        ''' <summary>Selisih = SaldoHitung - SaldoTersimpan.</summary>
        Public ReadOnly Property Selisih As Decimal
            Get
                Return SaldoHitung - SaldoTersimpan
            End Get
        End Property
    End Class

    ''' <summary>Hasil verifikasi keseimbangan jurnal (Total D = Total K).</summary>
    Public Class HasilKeseimbangan
        Public Property TotalDebet As Decimal
        Public Property TotalKredit As Decimal
        Public ReadOnly Property Seimbang As Boolean
            Get
                Return Math.Abs(TotalDebet - TotalKredit) < 1D
            End Get
        End Property
        Public ReadOnly Property Selisih As Decimal
            Get
                Return TotalDebet - TotalKredit
            End Get
        End Property
    End Class

    ''' <summary>Hasil kalkulasi Laba Rugi.</summary>
    Public Class HasilLabaRugi
        Public Property PendapatanKotor As Decimal
        Public Property Penjualan As Decimal
        Public Property ReturPenjualan As Decimal
        Public Property DiskonPenjualan As Decimal

        Public Property TotalHPP As Decimal
        Public Property HppPokok As Decimal
        Public Property AngkutBeli As Decimal
        Public Property AngkutJual As Decimal
        Public Property PenyesuaianStok As Decimal
        Public Property DiskonBeli As Decimal
        Public Property ReturBeli As Decimal

        Public Property LabaKotor As Decimal

        Public Property TotalBiaya As Decimal
        Public Property PendapatanLain As Decimal
        Public Property BebanPajak As Decimal

        Public Property LabaBersih As Decimal

        ''' <summary>
        ''' Laba Rugi Berjalan yang tersimpan di tbl_datareferensi (akun 05.01.001).
        ''' Dipakai sebagai pembanding.
        ''' </summary>
        Public Property LabaRugiTersimpan As Decimal

        Public ReadOnly Property Cocok As Boolean
            Get
                Return Math.Abs(LabaBersih - LabaRugiTersimpan) < 1D
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Verifikasi keseimbangan JurnalUmum: Total Debet harus = Total Kredit.
    ''' Bisa difilter per rentang tanggal (opsional).
    ''' </summary>
    Public Function CekKeseimbanganJurnal(
            Optional tglAwal As Date? = Nothing,
            Optional tglAkhir As Date? = Nothing) As HasilKeseimbangan

        EnsureConnectionReady()
        Dim hasil As New HasilKeseimbangan()

        Dim where As String = ""
        If tglAwal.HasValue AndAlso tglAkhir.HasValue Then
            where = " WHERE TGL_TRANSAKSI >= @tglAwal AND TGL_TRANSAKSI <= @tglAkhir"
        End If

        Dim sql As String =
            "SELECT " &
            "  COALESCE(SUM(NOMINAL), 0) AS TOTAL_DEBET " &
            "FROM JurnalUmum" & where

        Using cmd As New MySqlCommand(sql, conn)
            If tglAwal.HasValue Then
                cmd.Parameters.AddWithValue("@tglAwal", tglAwal.Value.ToString("yyyy-MM-dd 00:00:00"))
                cmd.Parameters.AddWithValue("@tglAkhir", tglAkhir.Value.ToString("yyyy-MM-dd 23:59:59"))
            End If
            hasil.TotalDebet = Convert.ToDecimal(cmd.ExecuteScalar())
        End Using

        ' Kredit = SUM NOMINAL dari sisi kredit (sama karena setiap baris punya D dan K)
        ' Dalam skema ini setiap baris JurnalUmum = 1 pasang D/K dengan NOMINAL yang sama
        ' Jadi Total D = Total K secara definisi — tapi kita verifikasi via NOMOR_AKUN_D vs K
        Dim sqlK As String =
            "SELECT " &
            "  COALESCE(SUM(CASE WHEN NOMOR_AKUN_D <> '' THEN NOMINAL ELSE 0 END), 0) AS TOTAL_D, " &
            "  COALESCE(SUM(CASE WHEN NOMOR_AKUN_K <> '' THEN NOMINAL ELSE 0 END), 0) AS TOTAL_K " &
            "FROM JurnalUmum" & where

        Using cmd As New MySqlCommand(sqlK, conn)
            If tglAwal.HasValue Then
                cmd.Parameters.AddWithValue("@tglAwal", tglAwal.Value.ToString("yyyy-MM-dd 00:00:00"))
                cmd.Parameters.AddWithValue("@tglAkhir", tglAkhir.Value.ToString("yyyy-MM-dd 23:59:59"))
            End If
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    hasil.TotalDebet = ModuleAngka.ParseDecimal(rd("TOTAL_D"))
                    hasil.TotalKredit = ModuleAngka.ParseDecimal(rd("TOTAL_K"))
                End If
            End Using
        End Using

        Return hasil
    End Function

    ''' <summary>
    ''' Hitung ulang saldo semua akun dari JurnalUmum (nilai kebenaran),
    ''' lalu bandingkan dengan nilai yang tersimpan di tbl_datareferensi.
    '''
    ''' Mekanisme identik dengan HITUNGSEMUASALDO tapi hasilnya ke memori,
    ''' tidak mengubah tbl_datareferensi.
    '''
    ''' Bisa difilter per rentang tanggal untuk verifikasi per periode.
    ''' </summary>
    Public Function VerifikasiSaldoSemua(
            Optional tglAwal As Date? = Nothing,
            Optional tglAkhir As Date? = Nothing) As List(Of HasilAkun)

        EnsureConnectionReady()
        Dim hasil As New List(Of HasilAkun)()

        ' Step 1: Ambil semua akun dari COA beserta saldo tersimpan
        Dim sqlCOA As String =
            "SELECT KODE_AKUN, NAMA_AKUN, TYPE_AKUN, JENIS_AKUN, SUB_AKUN, AKUN_DK, " &
            "       COALESCE(SALDO_AWAL, 0) AS SALDO_AWAL, " &
            "       COALESCE(SALDO_AKHIR, 0) AS SALDO_AKHIR " &
            "FROM tbl_datareferensi ORDER BY KODE_AKUN"

        Dim coa As New Dictionary(Of String, HasilAkun)()
        Using cmd As New MySqlCommand(sqlCOA, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim h As New HasilAkun()
                    h.KodeAkun = rd("KODE_AKUN").ToString()
                    h.NamaAkun = rd("NAMA_AKUN").ToString()
                    h.TypeAkun = rd("TYPE_AKUN").ToString()
                    h.JenisAkun = rd("JENIS_AKUN").ToString()
                    h.SubAkun = rd("SUB_AKUN").ToString()
                    h.AkunDK = rd("AKUN_DK").ToString()
                    h.SaldoAwal = ModuleAngka.ParseDecimal(rd("SALDO_AWAL"))
                    h.SaldoTersimpan = ModuleAngka.ParseDecimal(rd("SALDO_AKHIR"))
                    coa(h.KodeAkun) = h
                End While
            End Using
        End Using

        ' Step 2: Hitung total D dan K per akun dari JurnalUmum
        Dim whereClause As String = ""
        If tglAwal.HasValue AndAlso tglAkhir.HasValue Then
            whereClause = " WHERE TGL_TRANSAKSI >= @tglAwal AND TGL_TRANSAKSI <= @tglAkhir"
        End If

        Dim sqlJurnal As String =
            "SELECT kode_akun, SUM(total_d) AS TOTAL_D, SUM(total_k) AS TOTAL_K FROM (" &
            "  SELECT NOMOR_AKUN_D AS kode_akun, NOMINAL AS total_d, 0 AS total_k " &
            "  FROM JurnalUmum WHERE NOMOR_AKUN_D <> ''" & whereClause &
            "  UNION ALL " &
            "  SELECT NOMOR_AKUN_K AS kode_akun, 0 AS total_d, NOMINAL AS total_k " &
            "  FROM JurnalUmum WHERE NOMOR_AKUN_K <> ''" & whereClause &
            ") x GROUP BY kode_akun"

        ' Untuk subquery dengan WHERE, parameter harus diduplikasi
        Dim sqlJurnalFinal As String
        If tglAwal.HasValue Then
            sqlJurnalFinal =
                "SELECT kode_akun, SUM(total_d) AS TOTAL_D, SUM(total_k) AS TOTAL_K FROM (" &
                "  SELECT NOMOR_AKUN_D AS kode_akun, NOMINAL AS total_d, 0 AS total_k " &
                "  FROM JurnalUmum WHERE NOMOR_AKUN_D <> '' AND TGL_TRANSAKSI >= @tglAwal AND TGL_TRANSAKSI <= @tglAkhir " &
                "  UNION ALL " &
                "  SELECT NOMOR_AKUN_K AS kode_akun, 0 AS total_d, NOMINAL AS total_k " &
                "  FROM JurnalUmum WHERE NOMOR_AKUN_K <> '' AND TGL_TRANSAKSI >= @tglAwal2 AND TGL_TRANSAKSI <= @tglAkhir2 " &
                ") x GROUP BY kode_akun"
        Else
            sqlJurnalFinal =
                "SELECT kode_akun, SUM(total_d) AS TOTAL_D, SUM(total_k) AS TOTAL_K FROM (" &
                "  SELECT NOMOR_AKUN_D AS kode_akun, NOMINAL AS total_d, 0 AS total_k " &
                "  FROM JurnalUmum WHERE NOMOR_AKUN_D <> '' " &
                "  UNION ALL " &
                "  SELECT NOMOR_AKUN_K AS kode_akun, 0 AS total_d, NOMINAL AS total_k " &
                "  FROM JurnalUmum WHERE NOMOR_AKUN_K <> '' " &
                ") x GROUP BY kode_akun"
        End If

        Using cmd As New MySqlCommand(sqlJurnalFinal, conn)
            If tglAwal.HasValue Then
                Dim a As String = tglAwal.Value.ToString("yyyy-MM-dd 00:00:00")
                Dim b As String = tglAkhir.Value.ToString("yyyy-MM-dd 23:59:59")
                cmd.Parameters.AddWithValue("@tglAwal", a)
                cmd.Parameters.AddWithValue("@tglAkhir", b)
                cmd.Parameters.AddWithValue("@tglAwal2", a)
                cmd.Parameters.AddWithValue("@tglAkhir2", b)
            End If
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim kode As String = rd("kode_akun").ToString().Trim()
                    If String.IsNullOrEmpty(kode) Then Continue While
                    If coa.ContainsKey(kode) Then
                        coa(kode).TotalDebet = ModuleAngka.ParseDecimal(rd("TOTAL_D"))
                        coa(kode).TotalKredit = ModuleAngka.ParseDecimal(rd("TOTAL_K"))
                    End If
                End While
            End Using
        End Using

        ' Step 3: Hitung SaldoHitung per akun sesuai AKUN_DK
        ' Akun LABA RUGI dihitung terpisah di akhir
        ' Formula akumulasi: LABA+KREDIT +, LABA+DEBET -, RUGI+DEBET -, RUGI+KREDIT +
        Dim labaKotor As Decimal = 0
        Dim totalBiaya As Decimal = 0
        Dim pendapatanLain As Decimal = 0
        Dim bebanPajak As Decimal = 0

        For Each h As HasilAkun In coa.Values
            If h.TypeAkun = "LABA RUGI" Then
                ' Diisi setelah semua akun L/R dihitung
                Continue For
            End If

            Select Case h.AkunDK.ToUpper()
                Case "DEBET"
                    h.SaldoHitung = h.SaldoAwal + h.TotalDebet - h.TotalKredit
                Case "KREDIT"
                    h.SaldoHitung = h.SaldoAwal - h.TotalDebet + h.TotalKredit
                Case Else
                    h.SaldoHitung = 0
            End Select

            ' Akumulasi untuk Laba Rugi — wajib pakai SUB_AKUN + AKUN_DK
            ' LABA+KREDIT → menambah laba  (Penjualan, Pendapatan Lain, Diskon Beli, Retur Beli)
            ' LABA+DEBET  → mengurangi laba (Retur Jual, Diskon Jual)
            ' RUGI+DEBET  → mengurangi laba (HPP, Biaya Kirim, Beban, Pajak)
            ' RUGI+KREDIT → menambah laba   (Diskon Beli 06.05, Retur Beli 06.06)
            If h.SubAkun = "LABA" Then
                If h.AkunDK = "KREDIT" Then
                    labaKotor += h.SaldoHitung   ' Pendapatan — menambah laba
                Else
                    labaKotor -= h.SaldoHitung   ' Retur/Diskon Jual — mengurangi laba
                End If
            ElseIf h.SubAkun = "RUGI" Then
                If h.AkunDK = "KREDIT" Then
                    ' Kontra-beban (06.05 Diskon Beli, 06.06 Retur Beli) — menambah laba
                    labaKotor += h.SaldoHitung
                ElseIf h.JenisAkun = "BIAYA" Then
                    totalBiaya += h.SaldoHitung
                ElseIf h.JenisAkun = "PAJAK" Then
                    bebanPajak += h.SaldoHitung
                Else
                    ' HPP, Angkut, Penyesuaian Stok — mengurangi laba via labaKotor
                    labaKotor -= h.SaldoHitung
                End If
            End If

            hasil.Add(h)
        Next

        ' Step 4: Hitung akun LABA RUGI (05.01.001)
        ' Laba Bersih = Laba Kotor - Total Biaya + Pendapatan Lain - Beban Pajak
        Dim labaBersih As Decimal = labaKotor - totalBiaya + pendapatanLain - bebanPajak

        If coa.ContainsKey("05.01.001") Then
            Dim hLR As HasilAkun = coa("05.01.001")
            hLR.SaldoHitung = labaBersih
            hasil.Add(hLR)
        End If

        Return hasil
    End Function

    ''' <summary>
    ''' Hitung Laba Rugi dari JurnalUmum secara langsung.
    ''' Bisa difilter per rentang tanggal.
    '''
    ''' Formula (sesuai COA db_kasirlancar):
    '''   Pendapatan Kotor = Penjualan - Retur Jual - Diskon Jual
    '''   HPP              = HPP Pokok + Angkut Beli + Angkut Jual + Peny.Stok - Diskon Beli - Retur Beli
    '''   Laba Kotor       = Pendapatan Kotor - HPP
    '''   Laba Bersih      = Laba Kotor - Biaya Operasional + Pendapatan Lain - Beban Pajak
    ''' </summary>
    Public Function HitungLabaRugi(
            Optional tglAwal As Date? = Nothing,
            Optional tglAkhir As Date? = Nothing) As HasilLabaRugi

        EnsureConnectionReady()
        Dim hasil As New HasilLabaRugi()

        ' Ambil saldo per akun dari hasil verifikasi
        Dim semuaAkun As List(Of HasilAkun) = VerifikasiSaldoSemua(tglAwal, tglAkhir)
        Dim byKode As New Dictionary(Of String, Decimal)()
        For Each h As HasilAkun In semuaAkun
            byKode(h.KodeAkun) = h.SaldoHitung
        Next

        Dim S As Func(Of String, Decimal) = Function(kode) If(byKode.ContainsKey(kode), byKode(kode), 0D)

        ' Pendapatan Kotor
        ' 05.02.001 PENJUALAN (KREDIT) — saldo positif = pendapatan
        ' 05.03.001 RETUR PENJUALAN (DEBET) — saldo positif = mengurangi pendapatan
        ' 05.04.001 DISKON PENJUALAN (DEBET) — saldo positif = mengurangi pendapatan
        hasil.Penjualan = S("05.02.001")
        hasil.ReturPenjualan = S("05.03.001")
        hasil.DiskonPenjualan = S("05.04.001")
        hasil.PendapatanKotor = hasil.Penjualan - hasil.ReturPenjualan - hasil.DiskonPenjualan

        ' HPP
        ' 06.01.001 HPP POKOK (DEBET) — menambah HPP
        ' 06.02.001 ANGKUT BELI (DEBET) — menambah HPP
        ' 06.03.001 ANGKUT JUAL (DEBET) — menambah HPP
        ' 06.04.001 PENY. STOK MINUS (DEBET) — menambah HPP
        ' 06.05.001 DISKON BELI (KREDIT) — mengurangi HPP
        ' 06.06.001 RETUR BELI (KREDIT) — mengurangi HPP
        hasil.HppPokok = S("06.01.001")
        hasil.AngkutBeli = S("06.02.001")
        hasil.AngkutJual = S("06.03.001")
        hasil.PenyesuaianStok = S("06.04.001")
        hasil.DiskonBeli = S("06.05.001")
        hasil.ReturBeli = S("06.06.001")
        hasil.TotalHPP = hasil.HppPokok + hasil.AngkutBeli + hasil.AngkutJual +
                         hasil.PenyesuaianStok - hasil.DiskonBeli - hasil.ReturBeli

        ' Laba Kotor
        hasil.LabaKotor = hasil.PendapatanKotor - hasil.TotalHPP

        ' Biaya Operasional (semua akun JENIS_AKUN='BIAYA', SUB_AKUN='RUGI')
        hasil.TotalBiaya = semuaAkun.
            Where(Function(h) h.JenisAkun = "BIAYA" AndAlso h.SubAkun = "RUGI").
            Sum(Function(h) h.SaldoHitung)

        ' Pendapatan Lain (JENIS_AKUN='PENDAPATAN LAIN')
        hasil.PendapatanLain = semuaAkun.
            Where(Function(h) h.JenisAkun = "PENDAPATAN LAIN").
            Sum(Function(h) If(h.AkunDK = "KREDIT", h.SaldoHitung, -h.SaldoHitung))

        ' Beban Pajak (JENIS_AKUN='PAJAK', SUB_AKUN='RUGI')
        hasil.BebanPajak = semuaAkun.
            Where(Function(h) h.JenisAkun = "PAJAK" AndAlso h.SubAkun = "RUGI").
            Sum(Function(h) h.SaldoHitung)

        ' Laba Bersih
        hasil.LabaBersih = hasil.LabaKotor - hasil.TotalBiaya + hasil.PendapatanLain - hasil.BebanPajak

        ' Bandingkan dengan nilai tersimpan di tbl_datareferensi (05.01.001)
        Using cmd As New MySqlCommand(
            "SELECT COALESCE(SALDO_AKHIR, 0) FROM tbl_datareferensi WHERE KODE_AKUN = '05.01.001'", conn)
            Dim val = cmd.ExecuteScalar()
            hasil.LabaRugiTersimpan = If(val Is Nothing OrElse IsDBNull(val), 0D, Convert.ToDecimal(val))
        End Using

        Return hasil
    End Function

    ''' <summary>
    ''' Ringkasan verifikasi: tampilkan hanya akun yang TIDAK cocok antara
    ''' nilai hitung dan nilai tersimpan.
    ''' Berguna untuk debug — jika list kosong berarti semua saldo valid.
    ''' </summary>
    Public Function CariAkunTidakCocok(
            Optional tglAwal As Date? = Nothing,
            Optional tglAkhir As Date? = Nothing) As List(Of HasilAkun)

        Return VerifikasiSaldoSemua(tglAwal, tglAkhir).
               Where(Function(h) Not h.Cocok).
               ToList()
    End Function

    ''' <summary>
    ''' Verifikasi cepat: apakah neraca seimbang?
    ''' Aset = Pasiva + Modal + Laba Rugi Berjalan
    '''
    ''' Catatan COA db_kasirlancar:
    '''   - Akun PRIVE (04.02.001) ber-JENIS_AKUN='MODAL' tapi AKUN_DK='DEBET' (kontra-modal)
    '''     → masuk ke sisi Aset dalam persamaan neraca
    '''   - Akun AKUM. PENYUSUTAN (02.02.*) ber-JENIS_AKUN='ASET TETAP' tapi AKUN_DK='KREDIT'
    '''     → mengurangi Aset Tetap
    '''   Rumus: Total Aset Bersih = Total Pasiva + Modal + Laba Rugi
    '''   Di mana: Aset Bersih = SUM(akun DEBET di sisi Aset) - SUM(akun KREDIT di sisi Aset)
    ''' </summary>
    Public Function CekNeracaSeimbang() As Boolean
        EnsureConnectionReady()

        Dim sqlNeraca As String =
            "SELECT " &
            "  -- Aset: akun DEBET di JENIS_AKUN ASET* dikurangi akun KREDIT (akumulasi penyusutan) " &
            "  SUM(CASE WHEN JENIS_AKUN LIKE 'ASET%' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) - " &
            "  SUM(CASE WHEN JENIS_AKUN LIKE 'ASET%' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) AS ASET_BERSIH, " &
            "  -- Pasiva: hutang (KREDIT) " &
            "  SUM(CASE WHEN JENIS_AKUN = 'PASIVA' THEN SALDO_AKHIR ELSE 0 END) AS TOTAL_PASIVA, " &
            "  -- Modal: KREDIT dikurangi PRIVE (DEBET) " &
            "  SUM(CASE WHEN JENIS_AKUN = 'MODAL' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) - " &
            "  SUM(CASE WHEN JENIS_AKUN = 'MODAL' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) AS MODAL_BERSIH, " &
            "  -- Laba Rugi Berjalan " &
            "  SUM(CASE WHEN KODE_AKUN = '05.01.001' THEN SALDO_AKHIR ELSE 0 END) AS LABA_RUGI " &
            "FROM tbl_datareferensi"

        Using cmd As New MySqlCommand(sqlNeraca, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Dim asetBersih As Decimal = ModuleAngka.ParseDecimal(rd("ASET_BERSIH"))
                    Dim totalPasiva As Decimal = ModuleAngka.ParseDecimal(rd("TOTAL_PASIVA"))
                    Dim modalBersih As Decimal = ModuleAngka.ParseDecimal(rd("MODAL_BERSIH"))
                    Dim labaRugi As Decimal = ModuleAngka.ParseDecimal(rd("LABA_RUGI"))
                    ' Persamaan neraca: Aset = Pasiva + Modal + Laba Rugi
                    Return Math.Abs(asetBersih - (totalPasiva + modalBersih + labaRugi)) < 1D
                End If
            End Using
        End Using
        Return False
    End Function

End Module
