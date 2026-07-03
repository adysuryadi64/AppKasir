Imports System.Text
Imports System.Globalization

''' <summary>
''' Modul helper untuk membangun HTML laporan Laba Rugi dari temp_datareferensi.
''' Pola query identik dengan FormLapNeracaLR.TampilkanLabaRugi() —
''' satu query per JENIS_AKUN, tanda nilai dihitung di SQL via SUB_AKUN + AKUN_DK.
''' </summary>
Module ModuleLabaRugiHTML

    Private ReadOnly _id As New CultureInfo("id-ID")

    ''' <summary>
    ''' Bangun HTML laporan L/R lengkap.
    ''' Dipanggil dari FormLapLabaRugi setelah temp_datareferensi sudah dihitung.
    ''' </summary>
    Public Function BangunHTML(namaToko As String,
                               periode  As String,
                               awal     As String,
                               rubah    As String,
                               user     As String) As String

        ' ── Baca template ─────────────────────────────────────────────
        ' File di-copy ke root output (tanpa subfolder) via <Link> di vbproj
        Dim templatePath As String = System.IO.Path.Combine(
            Application.StartupPath, "TemplateLabaRugi.html")

        ' Fallback 1: subfolder output (jika <Link> belum aktif)
        If Not System.IO.File.Exists(templatePath) Then
            templatePath = System.IO.Path.Combine(
                Application.StartupPath, "5Lap", "LabaRugiHTML", "TemplateLabaRugi.html")
        End If

        ' Fallback 2: lokasi source relatif terhadap EXE saat debug
        If Not System.IO.File.Exists(templatePath) Then
            Dim exeDir As String = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location)
            templatePath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(exeDir, "..", "..", "..", "5Lap", "LabaRugiHTML", "TemplateLabaRugi.html"))
        End If

        Dim template As String = If(System.IO.File.Exists(templatePath),
            System.IO.File.ReadAllText(templatePath, Encoding.UTF8),
            "<html><body>{{BODY}}</body></html>")

        ' ── Definisi section — urutan, label, query SQL ───────────────
        ' Tanda nilai identik dengan FormLapNeracaLR:
        '   PENJUALAN : LABA+KREDIT=+, LABA+DEBET=-
        '   HPP       : RUGI+DEBET=+, RUGI+KREDIT=-
        '   BIAYA     : AKUN_DK KREDIT=-, DEBET=+
        '   PEND.LAIN : AKUN_DK DEBET=-, KREDIT=+
        '   PAJAK     : AKUN_DK KREDIT=-, DEBET=+
        Dim sections As New List(Of SectionDef) From {
            New SectionDef With {
                .Label    = "A.  PENJUALAN",
                .LabelTotal = "PENJUALAN BERSIH",
                .Urutan   = 1,
                .Sql      = "SELECT KODE_AKUN, NAMA_AKUN, " &
                            "  CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN  SALDO_SEBELUMNYA " &
                            "       WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN -SALDO_SEBELUMNYA " &
                            "       ELSE SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
                            "  CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN  (SALDO_AKHIR-SALDO_SEBELUMNYA) " &
                            "       WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN -(SALDO_AKHIR-SALDO_SEBELUMNYA) " &
                            "       ELSE (SALDO_AKHIR-SALDO_SEBELUMNYA) END AS Perubahan, " &
                            "  CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN  SALDO_AKHIR " &
                            "       WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN -SALDO_AKHIR " &
                            "       ELSE SALDO_AKHIR END AS SALDO_AKHIR " &
                            "FROM temp_datareferensi WHERE JENIS_AKUN='PENJUALAN' ORDER BY KODE_AKUN"
            },
            New SectionDef With {
                .Label    = "B.  HARGA POKOK PENJUALAN",
                .LabelTotal = "HARGA POKOK PENJUALAN",
                .Urutan   = 2,
                .Sql      = "SELECT KODE_AKUN, NAMA_AKUN, " &
                            "  CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN -SALDO_SEBELUMNYA " &
                            "       ELSE SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
                            "  CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN -(SALDO_AKHIR-SALDO_SEBELUMNYA) " &
                            "       ELSE (SALDO_AKHIR-SALDO_SEBELUMNYA) END AS Perubahan, " &
                            "  CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN -SALDO_AKHIR " &
                            "       ELSE SALDO_AKHIR END AS SALDO_AKHIR " &
                            "FROM temp_datareferensi WHERE JENIS_AKUN='HPP' ORDER BY KODE_AKUN"
            },
            New SectionDef With {
                .Label    = "C.  BIAYA ADMINISTRASI DAN UMUM",
                .LabelTotal = "TOTAL BIAYA OPERASIONAL",
                .Urutan   = 3,
                .Sql      = "SELECT KODE_AKUN, NAMA_AKUN, " &
                            "  CASE WHEN AKUN_DK='KREDIT' THEN -SALDO_SEBELUMNYA ELSE SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
                            "  CASE WHEN AKUN_DK='KREDIT' THEN -(SALDO_AKHIR-SALDO_SEBELUMNYA) ELSE (SALDO_AKHIR-SALDO_SEBELUMNYA) END AS Perubahan, " &
                            "  CASE WHEN AKUN_DK='KREDIT' THEN -SALDO_AKHIR ELSE SALDO_AKHIR END AS SALDO_AKHIR " &
                            "FROM temp_datareferensi WHERE JENIS_AKUN='BIAYA' ORDER BY KODE_AKUN"
            },
            New SectionDef With {
                .Label    = "D.  PENDAPATAN DILUAR USAHA",
                .LabelTotal = "TOTAL PENDAPATAN LAIN",
                .Urutan   = 4,
                .Sql      = "SELECT KODE_AKUN, NAMA_AKUN, " &
                            "  CASE WHEN AKUN_DK='DEBET' THEN -SALDO_SEBELUMNYA ELSE SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
                            "  CASE WHEN AKUN_DK='DEBET' THEN -(SALDO_AKHIR-SALDO_SEBELUMNYA) ELSE (SALDO_AKHIR-SALDO_SEBELUMNYA) END AS Perubahan, " &
                            "  CASE WHEN AKUN_DK='DEBET' THEN -SALDO_AKHIR ELSE SALDO_AKHIR END AS SALDO_AKHIR " &
                            "FROM temp_datareferensi WHERE JENIS_AKUN='PENDAPATAN LAIN' ORDER BY KODE_AKUN"
            },
            New SectionDef With {
                .Label    = "E.  BEBAN PAJAK",
                .LabelTotal = "TOTAL BEBAN PAJAK",
                .Urutan   = 5,
                .Sql      = "SELECT KODE_AKUN, NAMA_AKUN, " &
                            "  CASE WHEN AKUN_DK='KREDIT' THEN -SALDO_SEBELUMNYA ELSE SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
                            "  CASE WHEN AKUN_DK='KREDIT' THEN -(SALDO_AKHIR-SALDO_SEBELUMNYA) ELSE (SALDO_AKHIR-SALDO_SEBELUMNYA) END AS Perubahan, " &
                            "  CASE WHEN AKUN_DK='KREDIT' THEN -SALDO_AKHIR ELSE SALDO_AKHIR END AS SALDO_AKHIR " &
                            "FROM temp_datareferensi WHERE JENIS_AKUN='PAJAK' ORDER BY KODE_AKUN"
            }
        }

        ' ── Bangun BODY HTML ──────────────────────────────────────────
        Dim body As New StringBuilder()

        ' Akumulasi total per section untuk kalkulasi laba
        Dim totalPerSection(5) As Decimal  ' index 1-5 sesuai Urutan

        EnsureConnectionReady()

        For Each sec As SectionDef In sections
            ' Query section ini
            Dim rows As New List(Of AkunRow)
            Using cmd As New MySqlCommand(sec.Sql, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        rows.Add(New AkunRow With {
                            .Kode        = rd("KODE_AKUN").ToString(),
                            .Nama        = rd("NAMA_AKUN").ToString(),
                            .SaldoAwal   = ModuleAngka.ParseDecimal(rd("SALDO_SEBELUMNYA")),
                            .Perubahan   = ModuleAngka.ParseDecimal(rd("Perubahan")),
                            .SaldoAkhir  = ModuleAngka.ParseDecimal(rd("SALDO_AKHIR"))
                        })
                    End While
                End Using
            End Using

            ' Hitung total section
            Dim sumAwal      As Decimal = rows.Sum(Function(r) r.SaldoAwal)
            Dim sumPerubahan As Decimal = rows.Sum(Function(r) r.Perubahan)
            Dim sumAkhir     As Decimal = rows.Sum(Function(r) r.SaldoAkhir)
            totalPerSection(sec.Urutan) = sumPerubahan

            ' ── Section header ────────────────────────────────────────
            body.AppendLine($"<tr class=""section-header""><td colspan=""6"">{sec.Label}</td></tr>")

            ' ── Baris per akun ────────────────────────────────────────
            For Each r As AkunRow In rows
                body.AppendLine("<tr class=""row-akun"">")
                body.AppendLine($"  <td class=""kode"">{r.Kode}</td>")
                body.AppendLine($"  <td class=""nama"">{r.Nama}</td>")
                body.AppendLine($"  <td class=""angka"">{Fmt(r.SaldoAwal)}</td>")
                body.AppendLine($"  <td class=""{CssAngka(r.Perubahan)}"">{Fmt(r.Perubahan)}</td>")
                body.AppendLine($"  <td class=""{CssAngka(r.SaldoAkhir)}"">{Fmt(r.SaldoAkhir)}</td>")
                body.AppendLine("  <td></td>")
                body.AppendLine("</tr>")
            Next

            ' ── Baris subtotal ────────────────────────────────────────
            body.AppendLine("<tr class=""row-subtotal"">")
            body.AppendLine($"  <td></td>")
            body.AppendLine($"  <td class=""nama"" style=""text-align:right;"">Jumlah {sec.Label}</td>")
            body.AppendLine($"  <td class=""angka"">{Fmt(sumAwal)}</td>")
            body.AppendLine($"  <td class=""angka"">{Fmt(sumPerubahan)}</td>")
            body.AppendLine($"  <td class=""{CssAngka(sumAkhir)}"">{Fmt(sumAkhir)}</td>")
            body.AppendLine("  <td></td>")
            body.AppendLine("</tr>")

            ' ── Baris total kanan ─────────────────────────────────────
            body.AppendLine("<tr class=""row-total-section"">")
            body.AppendLine($"  <td colspan=""5"" class=""total-label"">{sec.LabelTotal}</td>")
            body.AppendLine($"  <td class=""{CssTotalNilai(sumPerubahan)}"">{Fmt(sumPerubahan)}</td>")
            body.AppendLine("</tr>")

            ' ── Baris laba antara section ─────────────────────────────
            Dim laba As Decimal = 0D
            Dim labelLaba As String = ""
            Select Case sec.Urutan
                Case 2  ' Setelah HPP
                    laba = totalPerSection(1) - totalPerSection(2)
                    labelLaba = "LABA KOTOR"
                Case 3  ' Setelah BIAYA
                    laba = totalPerSection(1) - totalPerSection(2) - totalPerSection(3)
                    labelLaba = "LABA BERSIH SEBELUM PAJAK"
                Case 4  ' Setelah PENDAPATAN LAIN
                    laba = totalPerSection(1) - totalPerSection(2) - totalPerSection(3) + totalPerSection(4)
                    labelLaba = "LABA RUGI SEBELUM PAJAK"
            End Select
            If labelLaba <> "" Then
                body.AppendLine($"<tr class=""row-laba"">")
                body.AppendLine($"  <td colspan=""5"" class=""laba-label"">{labelLaba}</td>")
                body.AppendLine($"  <td class=""{CssLabaNilai(laba)}"">{Fmt(laba)}</td>")
                body.AppendLine("</tr>")
            End If
        Next

        ' ── Baris LABA BERSIH AKHIR ───────────────────────────────────
        Dim labaBersih As Decimal = totalPerSection(1) - totalPerSection(2) -
                                    totalPerSection(3) + totalPerSection(4) -
                                    totalPerSection(5)
        body.AppendLine("<tr class=""row-laba-akhir"">")
        body.AppendLine($"  <td colspan=""5"" class=""laba-akhir-label"">LABA / RUGI BERSIH PERIODE INI</td>")
        body.AppendLine($"  <td class=""laba-akhir-nilai"">{Fmt(labaBersih)}</td>")
        body.AppendLine("</tr>")

        ' ── Ganti placeholder ─────────────────────────────────────────
        Return template.
            Replace("{{NAMATOKO}}", namaToko).
            Replace("{{PERIODE}}",  periode).
            Replace("{{AWAL}}",     awal).
            Replace("{{RUBAH}}",    rubah).
            Replace("{{USER}}",     user).
            Replace("{{WAKTU}}",    Now.ToString("dd/MM/yyyy HH:mm", _id)).
            Replace("{{BODY}}",     body.ToString())
    End Function

    ' ── Helper classes ────────────────────────────────────────────────
    Private Class SectionDef
        Public Property Label       As String
        Public Property LabelTotal  As String
        Public Property Urutan      As Integer
        Public Property Sql         As String
    End Class

    Private Class AkunRow
        Public Property Kode       As String
        Public Property Nama       As String
        Public Property SaldoAwal  As Decimal
        Public Property Perubahan  As Decimal
        Public Property SaldoAkhir As Decimal
    End Class

    ' ── Helper format ─────────────────────────────────────────────────
    Private Function Fmt(nilai As Decimal) As String
        If nilai = 0 Then Return "-"
        Return nilai.ToString("#,##0", _id)
    End Function

    Private Function CssAngka(nilai As Decimal) As String
        Return If(nilai < 0, "angka-negatif", "angka")
    End Function

    Private Function CssTotalNilai(nilai As Decimal) As String
        Return If(nilai < 0, "total-nilai-negatif", "total-nilai")
    End Function

    Private Function CssLabaNilai(nilai As Decimal) As String
        Return If(nilai < 0, "laba-nilai-rugi", "laba-nilai")
    End Function

End Module
