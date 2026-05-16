# Analisis Bug: ModuleVerifikasiJurnal.vb & FormLapNeracaLR.vb

Berdasarkan pembacaan kode VB dan test SQL yang dibuat.

---

## ModuleVerifikasiJurnal.vb

### ✅ Yang Sudah Benar

1. **Formula saldo per akun** — `DEBET: SaldoAwal + TotalD - TotalK` dan `KREDIT: SaldoAwal - TotalD + TotalK` sudah identik dengan `HITUNGSEMUASALDO`.

2. **Formula Laba Rugi** — `SUM(KREDIT_LABA) - SUM(DEBET_LABA) - SUM(RUGI)` sudah benar dan konsisten dengan `HITUNGSEMUASALDO` Step 3.

3. **Akun 05.01.001** — dihitung dari akun L/R, tidak dari JurnalUmum. Sudah benar.

4. **Toleransi 1 rupiah** di `HasilAkun.Cocok` — wajar untuk menghindari selisih pembulatan.

---

### ❌ Bug yang Ditemukan

#### BUG-1: `CekKeseimbanganJurnal` — query pertama tidak dipakai

```vb
' Query pertama ini dieksekusi tapi hasilnya langsung ditimpa oleh query kedua
Using cmd As New MySqlCommand(sql, conn)   ' ← hasil TotalDebet dari sini
    ...
    hasil.TotalDebet = Convert.ToDecimal(cmd.ExecuteScalar())
End Using

' Query kedua menimpa TotalDebet yang baru saja diisi
Using cmd As New MySqlCommand(sqlK, conn)
    ...
    hasil.TotalDebet = ModuleAngka.ParseDecimal(rd("TOTAL_D"))   ' ← timpa
    hasil.TotalKredit = ModuleAngka.ParseDecimal(rd("TOTAL_K"))
End Using
```

**Dampak:** Query pertama (SUM NOMINAL) dieksekusi sia-sia, membuang 1 round-trip ke DB.  
**Fix:** Hapus query pertama, hanya pakai query kedua yang sudah benar.

---

#### BUG-2: `VerifikasiSaldoSemua` — akumulasi Laba Rugi di loop tidak konsisten dengan `HitungLabaRugi`

Di loop `For Each h As HasilAkun In coa.Values`:

```vb
If h.SubAkun = "LABA" Then
    If h.AkunDK = "KREDIT" Then
        labaKotor += h.SaldoHitung   ' Pendapatan
    Else
        labaKotor -= h.SaldoHitung   ' HPP/Retur/Diskon
    End If
ElseIf h.SubAkun = "RUGI" Then
    If h.JenisAkun = "BIAYA" Then
        totalBiaya += h.SaldoHitung
    ElseIf h.JenisAkun = "PENDAPATAN LAIN" Then
        ' ← Komentar sendiri bilang "Di COA aktual 08.01.* masuk SUB_AKUN='LABA'"
        ' Jadi cabang ini TIDAK PERNAH dieksekusi untuk pendapatan lain
        pendapatanLain += ...
    ElseIf h.JenisAkun = "PAJAK" Then
        bebanPajak += h.SaldoHitung
    Else
        totalBiaya += h.SaldoHitung   ' ← fallback: semua RUGI lain masuk biaya
    End If
End If
```

**Masalah:** Variabel `labaKotor` di sini sebenarnya adalah `labaBersih` (sudah termasuk pendapatan lain dari SUB_AKUN='LABA'). Nama variabel menyesatkan tapi logikanya benar selama COA konsisten.

**Masalah nyata:** Jika ada akun `SUB_AKUN='RUGI'` dengan `JENIS_AKUN='PENDAPATAN LAIN'` (tidak sesuai COA aktual), cabang itu tidak akan pernah dieksekusi karena komentar sendiri bilang pendapatan lain ada di `SUB_AKUN='LABA'`. Cabang ini dead code.

**Fix:** Hapus cabang `ElseIf h.JenisAkun = "PENDAPATAN LAIN"` di dalam `SUB_AKUN = "RUGI"` — tidak akan pernah dieksekusi.

---

#### BUG-3: `VerifikasiSaldoSemua` — akun LABA RUGI tidak masuk ke `hasil` list

```vb
For Each h As HasilAkun In coa.Values
    If h.TypeAkun = "LABA RUGI" Then
        Continue For   ' ← skip, tidak di-add ke hasil
    End If
    ...
    hasil.Add(h)   ' ← akun biasa di-add
Next

' Setelah loop, akun LABA RUGI di-add
If coa.ContainsKey("05.01.001") Then
    Dim hLR As HasilAkun = coa("05.01.001")
    hLR.SaldoHitung = labaBersih
    hasil.Add(hLR)   ' ← hanya 05.01.001 yang di-add
End If
```

**Masalah:** Jika ada lebih dari 1 akun dengan `TYPE_AKUN = 'LABA RUGI'` (selain 05.01.001), akun tersebut di-skip dan tidak masuk ke `hasil`. Tapi di COA aktual hanya ada 1 akun LABA RUGI, jadi **tidak berdampak saat ini**.

---

#### BUG-4: `HitungLabaRugi` — memanggil `VerifikasiSaldoSemua` dua kali jika dipanggil dari luar

`HitungLabaRugi` memanggil `VerifikasiSaldoSemua` secara internal. Jika pemanggil sudah punya hasil `VerifikasiSaldoSemua`, memanggil `HitungLabaRugi` akan query DB lagi dari awal. Tidak ada caching.

**Dampak:** Performa — 2x query ke DB untuk data yang sama.  
**Fix:** Tambahkan overload yang menerima `List(Of HasilAkun)` sebagai parameter.

---

#### BUG-5: `CekNeracaSeimbang` — membaca dari `tbl_datareferensi.SALDO_AKHIR`

```vb
' Membaca SALDO_AKHIR yang tersimpan, bukan menghitung ulang dari JurnalUmum
Using cmd As New MySqlCommand(sqlNeraca, conn)
```

**Masalah:** Jika `HITUNGSEMUASALDO` belum dijalankan (saldo stale), `CekNeracaSeimbang` akan return `True` meski jurnal tidak seimbang. Fungsi ini **bukan verifikasi independen** — hanya cek konsistensi data tersimpan.

**Ini bukan bug** selama dokumentasinya jelas bahwa fungsi ini cek data tersimpan, bukan hitung ulang. Tapi nama `CekNeracaSeimbang` menyiratkan verifikasi independen.

**Rekomendasi:** Tambahkan komentar: *"Fungsi ini membaca SALDO_AKHIR tersimpan. Jalankan HITUNGSEMUASALDO dulu sebelum memanggil fungsi ini."*

---

## FormLapNeracaLR.vb

### ✅ Yang Sudah Benar

1. **Formula Laba Rugi Step 3** di `HITUNGSEMUASALDO`:
   ```vb
   "SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) - " &
   "SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) - " &
   "SUM(CASE WHEN SUB_AKUN='RUGI' THEN SALDO_AKHIR ELSE 0 END)"
   ```
   Sudah benar — memisahkan KREDIT dan DEBET di SUB_AKUN='LABA'.

2. **`SiapkanTempDatareferensi`** — INSERT dengan kolom eksplisit sudah aman dari kolom extra.

3. **`HITUNGSALDOAWAL` dan `HITUNGSALDOAKHIR`** — menggunakan `temp_datareferensi`, tidak mengubah `tbl_datareferensi`. Sudah benar.

---

### ❌ Bug yang Ditemukan

#### BUG-6: `HITUNGSEMUASALDO` Step 4 — formula SALDO_AKHIR LABA RUGI salah

```vb
' Step 4: Update SALDO_AKHIR LABA RUGI
Using cmd As New MySqlCommand(
    "UPDATE tbl_datareferensi SET SALDO_AKHIR = SALDO_SEBELUMNYA + (-(S_DEBET)) + S_KREDIT " &
    "WHERE TYPE_AKUN = 'LABA RUGI'", conn)
```

**Masalah:** Di Step 3, `SALDO_SEBELUMNYA` sudah diisi dengan nilai `labaRugi` (hasil kalkulasi L/R). Lalu Step 4 menambahkan `-(S_DEBET) + S_KREDIT` lagi.

Tapi `S_DEBET` dan `S_KREDIT` untuk akun LABA RUGI diisi di Step 3 dengan:
```vb
debetLabaRugi = SUM(S_DEBET dari akun LABA/RUGI)
kreditLabaRugi = SUM(S_KREDIT dari akun LABA/RUGI)
```

Jadi `SALDO_AKHIR = labaRugi + (-debetLabaRugi) + kreditLabaRugi` — ini **double counting**.

**Nilai yang benar:** `SALDO_AKHIR = labaRugi` (langsung dari Step 3).

**Verifikasi:** Jalankan test SQL dan cek apakah `05.01.001.SALDO_AKHIR = LABA_RUGI_HITUNG` di bagian verifikasi.

**Fix:**
```vb
' Step 4: SALDO_AKHIR LABA RUGI = nilai yang sudah dihitung di Step 3
Using cmd As New MySqlCommand(
    "UPDATE tbl_datareferensi SET SALDO_AKHIR = SALDO_SEBELUMNYA " &
    "WHERE TYPE_AKUN = 'LABA RUGI'", conn)
```

---

#### BUG-7: `HITUNGSALDOAWAL` Step 4 — formula sama dengan BUG-6

```vb
Dim hitungSaldolabarugi As String = "UPDATE temp_datareferensi " &
    "SET SALDO_SEBELUMNYA = @DebetLabaRugi - @KreditLabaRugi " &
    "WHERE TYPE_AKUN = 'LABA RUGI'"
```

Di sini `kreditLabaRugi = 0` (sudah di-comment "Tidak dipakai lagi"), jadi:
`SALDO_SEBELUMNYA = debetLabaRugi - 0 = debetLabaRugi`

Dan `debetLabaRugi` sudah berisi nilai laba bersih yang benar. Jadi **Step 4 di HITUNGSALDOAWAL tidak ada masalah** — `kreditLabaRugi = 0` membuat formula jadi benar secara kebetulan.

---

#### BUG-8: `HITUNGSALDOAKHIR` Step 4 — sama dengan HITUNGSALDOAWAL, aman

Sama dengan BUG-7 — `kreditLabaRugi = 0`, jadi `SALDO_AKHIR = debetLabaRugi` yang sudah benar.

---

#### BUG-9: `HITUNGSEMUASALDO` — tidak ada `DROP TEMPORARY TABLE` sebelum `CREATE`

```vb
Using cmdDrop As New MySqlCommand("DROP TEMPORARY TABLE IF EXISTS tmp_semua_saldo", conn)
    cmdDrop.ExecuteNonQuery()
End Using
Using cmdTmp As New MySqlCommand("CREATE TEMPORARY TABLE tmp_semua_saldo ...", conn)
```

Ini sudah benar — ada DROP IF EXISTS sebelum CREATE. ✅

---

#### BUG-10: `HITUNGSEMUASALDO` — `tmp_semua_saldo` tidak di-DROP setelah selesai

Temporary table otomatis hilang saat koneksi ditutup, tapi jika `HITUNGSEMUASALDO` dipanggil dua kali dalam satu sesi tanpa menutup koneksi, `CREATE TEMPORARY TABLE` akan error karena tabel sudah ada.

**Fix sudah ada:** `DROP TEMPORARY TABLE IF EXISTS tmp_semua_saldo` di awal. ✅

---

## Ringkasan

| # | File | Severity | Status |
|---|------|----------|--------|
| BUG-1 | ModuleVerifikasiJurnal | Low (performa) | Query pertama sia-sia |
| BUG-2 | ModuleVerifikasiJurnal | Low (dead code) | Cabang PENDAPATAN LAIN di RUGI tidak pernah dieksekusi |
| BUG-3 | ModuleVerifikasiJurnal | Low (edge case) | Akun LABA RUGI selain 05.01.001 tidak masuk hasil |
| BUG-4 | ModuleVerifikasiJurnal | Low (performa) | Double query jika HitungLabaRugi dipanggil setelah VerifikasiSaldoSemua |
| BUG-5 | ModuleVerifikasiJurnal | Info | CekNeracaSeimbang bukan verifikasi independen |
| **BUG-6** | **FormLapNeracaLR** | **HIGH** | **HITUNGSEMUASALDO Step 4: SALDO_AKHIR LABA RUGI double counting** |
| BUG-7 | FormLapNeracaLR | Aman | HITUNGSALDOAWAL: kreditLabaRugi=0 membuat formula benar |
| BUG-8 | FormLapNeracaLR | Aman | HITUNGSALDOAKHIR: sama dengan BUG-7, aman |

---

## Fix Prioritas Tinggi: BUG-6

```vb
' SEBELUM (salah — double counting):
"UPDATE tbl_datareferensi SET SALDO_AKHIR = SALDO_SEBELUMNYA + (-(S_DEBET)) + S_KREDIT " &
"WHERE TYPE_AKUN = 'LABA RUGI'"

' SESUDAH (benar — SALDO_SEBELUMNYA sudah berisi labaRugi dari Step 3):
"UPDATE tbl_datareferensi SET SALDO_AKHIR = SALDO_SEBELUMNYA " &
"WHERE TYPE_AKUN = 'LABA RUGI'"
```

Verifikasi dengan menjalankan `Tests/Test-Pembelian-Penjualan-Real.sql` dan cek bagian:
```
--- 5. Akun 05.01.001 LABA RUGI BERJALAN ---
```
Jika hasilnya `FAIL`, berarti BUG-6 terkonfirmasi.
