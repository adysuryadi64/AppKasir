# Standar Penulisan Kode VB.NET — AppKasir

> Panduan ini membantu AI agent memahami, menavigasi, dan memodifikasi kode dengan akurat dan efisien.
> Ikuti standar ini saat membuat file baru atau menambahkan kode ke file yang sudah ada.

---

## 1. Struktur File Form

Setiap file form wajib mengikuti urutan section berikut:

```vb
Public Class FormNama

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION A: VARIABEL PRIVATE
    ' ═══════════════════════════════════════════════════════════════════

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION B: PROPERTIES
    ' ═══════════════════════════════════════════════════════════════════

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: FORM LOAD & SETUP
    ' ═══════════════════════════════════════════════════════════════════
#Region "FORM LOAD & SETUP"
    ' Form_Load, Kondisiawal, Setup*, Muat*
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: FUNGSI KALKULASI
    ' ═══════════════════════════════════════════════════════════════════
#Region "FUNGSI KALKULASI"
    ' Hitung*, Update*, Sync* — tidak ada akses DB di sini
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: DGV EVENT HANDLERS
    ' ═══════════════════════════════════════════════════════════════════
#Region "DGV EVENT HANDLERS"
    ' CellEndEdit, CellFormatting, EditingControlShowing, dll
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: BUTTON HANDLERS
    ' ═══════════════════════════════════════════════════════════════════
#Region "BUTTON HANDLERS"
    ' BtnSimpan_Click, BtnBatal_Click, TekanBayar, dll
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: FUNGSI DB — SIMPAN
    ' ═══════════════════════════════════════════════════════════════════
#Region "FUNGSI DB — SIMPAN"
    ' Simpan*, Insert*, Update* ke database
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: FUNGSI DB — AMBIL DATA
    ' ═══════════════════════════════════════════════════════════════════
#Region "FUNGSI DB — AMBIL DATA"
    ' Ambil*, Load*, Muat* dari database
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: JURNAL
    ' ═══════════════════════════════════════════════════════════════════
#Region "JURNAL"
    ' Simpanjurnal, InsertJurnal, dll
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: HELPER & UTILITAS
    ' ═══════════════════════════════════════════════════════════════════
#Region "HELPER & UTILITAS"
    ' Fungsi kecil yang dipakai banyak tempat
#End Region

End Class
```

---

## 2. Aturan Region

- **Satu region = satu tanggung jawab bisnis** — jangan buat region per 5 baris
- **Nama region huruf besar semua** — `#Region "FUNGSI KALKULASI"` bukan `#Region "fungsi kalkulasi"`
- **Gunakan pemisah `' ═══`** di atas setiap region untuk visibilitas saat scroll
- **Jangan nested region** kecuali benar-benar diperlukan

---

## 3. Aturan Penamaan Fungsi

### Pola nama yang konsisten

| Prefix | Artinya | Contoh |
|---|---|---|
| `Hitung*` | Kalkulasi murni, update variabel class | `HitungGrandTotal`, `HitungSisaPembayaran` |
| `Update*` | Update variabel + UI sekaligus | `UpdateSemuaTotal`, `UpdateLabelPembayaran` |
| `Sync*` | Sinkronisasi UI → variabel atau sebaliknya | `SyncKomponenTambahanDariUI` |
| `Simpan*` | Tulis ke database | `SimpanPembelian`, `SimpanPembelianDetail` |
| `Ambil*` | Baca dari database ke form | `AmbilDataPembelian`, `AmbilDaftarBarang` |
| `Isi*` | Isi kontrol UI dari data | `IsiBarangKeRow`, `IsiComboBoxAkun` |
| `Tampil*` | Tampilkan form/panel/list | `TampilkanSemuaSupplier` |
| `Tekan*` | Handler tombol/shortcut | `TekanBayar`, `TekanSimpan`, `Tekanbatal` |
| `Cek*` | Validasi, return Boolean | `CekStok`, `Cekjualrugi` |
| `Reset*` / `Kosong*` | Bersihkan state/UI | `KosongTxtboxcari`, `Kondisiawal` |

### Yang dilarang
- Nama ambigu: `Proses`, `Jalankan`, `DoSomething`
- Nama duplikat dengan fungsi lain yang sudah ada — selalu cek dulu dengan `grepSearch`
- Fungsi yang melakukan dua tanggung jawab berbeda — pisah menjadi dua fungsi

---

## 4. Aturan Variabel Private

### Deklarasi di bagian atas form, dikelompokkan per section

```vb
' ── Section A: Per baris DGV ──────────────────────────────────────
Private _hargaPerBaris As Decimal = 0D
Private _qtyPerBaris As Decimal = 0D

' ── Section B: Agregat ────────────────────────────────────────────
Private _subtotalBarang As Decimal = 0D
Private _totalQty As Decimal = 0D

' ── Section C: Komponen tambahan ──────────────────────────────────
Private _diskonRupiah As Decimal = 0D
Private _ppnRupiah As Decimal = 0D

' ── Section D: Grand total ────────────────────────────────────────
Private _grandTotal As Decimal = 0D

' ── Section E: Pembayaran ─────────────────────────────────────────
Private _bayarTunai As Decimal = 0D
Private _sisaHutang As Decimal = 0D
```

### Aturan penamaan variabel
- Prefix `_` untuk semua variabel Private class-level
- Nama deskriptif: `_grandTotalPembelian` bukan `_gt`
- Komentar singkat di setiap deklarasi: `' Kolom: Totalharga`

---

## 5. Aturan Fungsi Kalkulasi — Satu Sumber Kebenaran

> Prinsip utama: **satu jalur kalkulasi, satu fungsi master**.

### Pola orkestrasi yang benar

```vb
' ✅ BENAR — satu fungsi master yang memanggil semua sub-fungsi secara berurutan
Private Sub UpdateSemuaTotal()
    ' Step 1: Per baris DGV
    For Each row As DataGridViewRow In DgvData.Rows
        If Not row.IsNewRow Then UpdatePerhitunganPerBaris(row)
    Next
    ' Step 2: Agregat
    UpdatePerhitunganAgregat()
    ' Step 3: Sync komponen tambahan dari UI
    SyncKomponenTambahanDariUI()
    ' Step 4: Grand total
    HitungGrandTotalPembelian()
    ' Step 5: Pembayaran
    HitungPembayaran()
    ' Step 6: Jurnal
    UpdatePerhitunganJurnal()
    ' Step 7: Update UI
    UpdateUIHasilPerhitungan()
End Sub
```

### Yang dilarang
- Fungsi kalkulasi yang sama ditulis dua kali dengan nama berbeda
- Fungsi yang menghitung ulang nilai yang sudah ada di variabel class
- Memanggil fungsi yang sama dua kali dalam satu alur (duplikat)

### Cara deteksi duplikasi sebelum menulis fungsi baru
```powershell
# Cari fungsi dengan logika serupa sebelum membuat baru
Select-String -Path "2Trans/FormNama.vb" -Pattern "Private (Sub|Function) Hitung"
```

---

## 6. Aturan Komentar

### Wajib ada di setiap fungsi

```vb
''' <summary>
''' Hitung grand total pembelian: Subtotal - Diskon + PPN + Biaya Kirim.
''' Dipanggil dari UpdateSemuaTotal() dan HitungGrandTotalBeli().
''' Mengisi _grandTotalPembelian dan update TxtGrandTotalPembelian.
''' </summary>
Private Sub HitungGrandTotalPembelian()
```

### Komentar inline untuk logika tidak langsung jelas

```vb
' Pakai _subtotalBarang (sudah dihitung di Step 2) — bukan loop DGV lagi
Dim dasarDiskon As Decimal = _subtotalBarang
```

### Pemisah sub-bagian dalam fungsi panjang

```vb
' ── Validasi input ────────────────────────────────────────────────
If String.IsNullOrEmpty(TxtNama.Text) Then ...

' ── Simpan ke database ────────────────────────────────────────────
Using cmd As New MySqlCommand(...)
```

---

## 7. Aturan Penomoran Step di Fungsi Panjang

Untuk fungsi yang punya urutan langkah penting (simpan transaksi, load edit, dll):

```vb
Private Sub SimpanTransaksi()
    ' ── Step 1: Validasi ──────────────────────────────────────────
    If Not ValidasiSebelumSimpan() Then Exit Sub

    ' ── Step 2: Buka transaksi DB ─────────────────────────────────
    Dim transaction = conn.BeginTransaction()

    ' ── Step 3: Simpan header ─────────────────────────────────────
    SimpanHeader(transaction)

    ' ── Step 4: Simpan detail ─────────────────────────────────────
    SimpanDetail(transaction)

    ' ── Step 5: Jurnal ────────────────────────────────────────────
    Simpanjurnal(transaction, jD, jK)

    ' ── Step 6: Commit ────────────────────────────────────────────
    transaction.Commit()
End Sub
```

---

## 8. Aturan Khusus untuk AI Agent

Hal-hal yang paling membantu AI agent saat bekerja dengan kode ini:

### Yang membantu
- **Region dengan nama deskriptif** → AI bisa langsung `grepSearch` nama region
- **Komentar `' Dipanggil dari X`** di fungsi → AI tahu dampak perubahan tanpa harus trace manual
- **Variabel dengan komentar kolom** → `' Kolom: Totalharga` membantu AI tahu mapping DGV
- **Penomoran step** di fungsi panjang → AI bisa identifikasi posisi tanpa baca seluruh fungsi
- **Nama fungsi konsisten** dengan prefix → AI bisa prediksi nama fungsi yang relevan

### Yang menyulitkan
- Fungsi tanpa komentar → AI harus baca seluruh isi untuk tahu tujuannya
- Variabel nama pendek (`_gt`, `_d`, `_p`) → AI tidak bisa tahu konteksnya
- Logika bisnis yang sama ditulis di dua tempat → AI bisa ubah satu tapi lupa yang lain
- Nama kolom DGV tidak konsisten (`qty` vs `QTY` vs `Qty`) → AI bisa salah nama kolom
- Fungsi yang melakukan terlalu banyak hal → sulit dipahami dan sulit diubah dengan aman

---

## 9. Checklist Sebelum Menulis Kode Baru

- [ ] Sudah `grepSearch` apakah fungsi serupa sudah ada?
- [ ] Nama fungsi mengikuti prefix standar (`Hitung*`, `Update*`, dll)?
- [ ] Variabel Private diberi prefix `_` dan komentar?
- [ ] Fungsi diberi komentar `''' <summary>`?
- [ ] Fungsi masuk ke region yang tepat?
- [ ] Tidak ada duplikasi logika dengan fungsi yang sudah ada?
- [ ] Kalkulasi menggunakan variabel class, bukan loop DGV ulang?
- [ ] Nama kolom DGV sudah diverifikasi case-sensitive dengan designer?
