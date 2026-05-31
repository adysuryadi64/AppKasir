Berikut adalah catatan rilis mendetail untuk aplikasi Kasir berdasarkan perubahan kode yang Anda berikan:

---

## Catatan Rilis Aplikasi Kasir - Versi 15.2026.531.9

### Tanggal Rilis: [Isi Tanggal Rilis]

Rilis ini fokus pada peningkatan integritas data, stabilitas, dan konsistensi format tampilan numerik, khususnya pada modul Transfer Barang. Sebuah isu kritis terkait *overflow* nilai harga pada data historis transaksi transfer telah diperbaiki melalui migrasi database dan penyempurnaan kode aplikasi.

---

### **Perbaikan & Peningkatan Stabilitas**

*   **AppKasir/Database/37_migrasi_fix_transfer_stok_harga_salah.sql**
    *   **Deskripsi:** Memperkenalkan skrip migrasi database baru yang krusial untuk memperbaiki masalah integritas data historis pada transaksi transfer stok dan transfer barang.
    *   **Detail Teknis:**
        *   **Isu yang Diperbaiki:** Terjadi *overflow* nilai harga (tersimpan sebagai `99999999.99` atau lebih tinggi) pada kolom harga di tabel `transfer_stok` (`HARGA_SAT_M`, `HARGA_SAT_K`) dan `transfer_barang_detail` (`HARGA`, `HARGA_QTY`, `TOTAL`). Masalah ini disebabkan oleh bug konversi tipe data `DECIMAL` dari MySQL yang salah diinterpretasikan oleh aplikasi saat di-`ToString()` dan kemudian di-`ParseDecimal`, di mana format `.0000` dapat memicu kesalahan interpretasi sebagai pemisah ribuan dalam kultur `id-ID`, menghasilkan nilai yang jauh lebih besar dan menyebabkan *overflow* saat disimpan kembali ke kolom `DECIMAL(10,2)`.
        *   **Dampak:** Nilai harga satuan dan total pada transaksi transfer historis menjadi tidak akurat, yang memengaruhi laporan stok, total transaksi, `JurnalUmum`, dan `HistoryBarang`.
        *   **Solusi:** Skrip migrasi ini secara spesifik mengidentifikasi dan mengoreksi baris data yang terdampak (dimana `HARGA >= 99999999`) di tabel `transfer_stok`, `transfer_barang_detail`, `transfer_barang`, `JurnalUmum`, dan `HistoryBarang`. Koreksi dilakukan dengan mengambil nilai `HARGA_BELI` yang benar dari `tbl_barang` dan mengaplikasikannya kembali. Skrip menggunakan *temporary table* untuk memastikan konsistensi selama proses pembaruan.
    *   **Manfaat:** Memulihkan akurasi data historis transaksi transfer, memastikan laporan keuangan dan stok yang benar.

*   **AppKasir/2Trans/FormTransferBarang.vb**
    *   **Modul:** `FormTransferBarang` (Form Transaksi Transfer Barang)
    *   **Peningkatan Format Tampilan Angka pada Grid dan Input:**
        *   **Deskripsi:** Implementasi format tampilan numerik yang lebih konsisten dan rapi pada `DataGridView` dan `TextBox` di form transfer barang.
        *   **Detail Teknis:**
            *   Menambahkan pemanggilan `ModuleAngka.TerapkanFormatKolomAngka` untuk kolom-kolom harga dan kuantitas (`Hargabeli`, `Qty`, `Isi`, `HargaBeliSat`, `QtySat`, `Totalharga`, `Stok`) di `DgvData` saat inisialisasi form.
            *   Mengganti `ToString("N0")` dengan `ModuleAngka.FormatRupiah` atau `ModuleAngka.FormatRupiahLabel` pada tampilan total (misalnya `TxtTotalRupiah.Text`, `TxtGrandtotal.Text`, `TxtTotalQTY.Text`) dan pada tampilan stok di *autocomplete* barang.
        *   **Manfaat:** Memastikan semua nilai numerik dan mata uang ditampilkan dengan format yang seragam dan mudah dibaca (misalnya, dengan pemisah ribuan, dua desimal, atau awalan "Rp."), meningkatkan pengalaman pengguna.
    *   **Peningkatan Keamanan dan Akurasi Konversi Tipe Data Numerik:**
        *   **Deskripsi:** Mengganti semua konversi tipe data numerik langsung (`CDec`, `CInt`, `Convert.ToDecimal`) dengan fungsi utilitas yang lebih aman dan terstandardisasi dari `ModuleAngka`.
        *   **Detail Teknis:**
            *   Implementasi `ModuleAngka.ParseDecimal` dan `ModuleAngka.ParseInteger` di berbagai titik, termasuk:
                *   Saat memuat harga beli (`Hargabeli`) dan stok (`Stok`) ke `DataGridView`.
                *   Saat melakukan perhitungan `HargaBeliSat`, `QtySat`, dan `Totalharga`.
                *   Saat memperbarui kuantitas dan total harga pada item yang sudah ada di `DataGridView`.
                *   Saat mendapatkan nilai `Qty`, `Isi`, `Harga`, `Stok` dari sel `DataGridView` atau `DataReader`.
            *   Mengganti `CInt(rd(...))` dengan `ModuleAngka.SafeGetValue(Of Integer)(rd, ..., 1)` saat mengambil nilai `ISI_UMUM_KECIL/SEDANG/BESAR` untuk memastikan nilai *default* dan penanganan `DBNull` yang aman.
        *   **Manfaat:** Ini adalah perbaikan fundamental untuk mencegah kesalahan konversi data yang dapat menyebabkan nilai tidak akurat, *runtime error*, atau bahkan *overflow* seperti yang terjadi pada data historis. Fungsi dari `ModuleAngka` dirancang untuk menangani `DBNull`, string kosong, dan perbedaan format angka lintas kultur dengan lebih tangguh.
    *   **Peningkatan Stabilitas Pemrosesan Data Saat Memuat Item Transfer:**
        *   **Deskripsi:** Memperbaiki cara baris baru ditambahkan ke `DgvData` ketika memuat item transfer dari `DataReader`.
        *   **Detail Teknis:** Mengganti pemuatan sel secara iteratif (`For i As Integer... row.Cells(i).Value = rd(i)`) dengan penetapan nilai secara eksplisit per nama kolom (misalnya `row.Cells("Id").Value = rd("ID_BARANG")`) dan langsung menerapkan `ModuleAngka.ParseDecimal` untuk kolom numerik.
        *   **Manfaat:** Membuat proses pengisian `DataGridView` lebih tangguh terhadap perubahan urutan kolom database dan memastikan data numerik di-parse dengan benar sejak awal.

*   **Pembaruan Versi Aplikasi & Mekanisme Pembaruan Otomatis**
    *   **File Terdampak:** `AppKasir/My Project/AssemblyInfo.vb`, `update.xml`
    *   **Deskripsi:** Versi aplikasi telah diperbarui ke `15.2026.531.9`.
    *   **Detail Teknis:** Nomor versi di `AssemblyInfo.vb` dan `update.xml` telah disinkronkan untuk mencerminkan rilis ini.
    *   **Manfaat:** Memungkinkan mekanisme pembaruan otomatis untuk mengunduh dan menerapkan perbaikan penting ini kepada pengguna.

---

Kami sangat merekomendasikan semua pengguna untuk memperbarui ke versi ini sesegera mungkin untuk mendapatkan perbaikan stabilitas dan integritas data yang krusial.

Terima kasih atas dukungan Anda.

Hormat kami,
Tim Pengembang Aplikasi Kasir

---
