Tentu, berikut adalah catatan rilis mendetail untuk aplikasi Kasir berdasarkan perubahan kode yang Anda berikan:

---

## Catatan Rilis Aplikasi Kasir - Versi 15.2026.522.6

**Tanggal Rilis:** 22 Mei 2024

Rilis ini membawa dua fitur utama yang signifikan: Sistem Poin Loyalitas Pelanggan (pondasi database dan arsitektur) dan Manajemen Sales Order (SO) yang komprehensif, serta berbagai perbaikan dan optimalisasi lainnya untuk meningkatkan pengalaman pengguna dan efisiensi operasional.

### â¨ Fitur Baru

*   **Sistem Poin Loyalitas Pelanggan (Pondasi)**
    *   **Deskripsi:** Pondasi fitur Poin Loyalitas Pelanggan telah ditambahkan, memungkinkan toko untuk mulai merancang dan mengimplementasikan program *reward* bagi pelanggan terdaftar. Fitur ini dirancang dengan pendekatan *ledger-based tracking* untuk audit trail yang lengkap, serta mendukung dua mekanisme perolehan poin: per satuan item atau berdasarkan kelipatan nominal total belanja. Penukaran poin akan dilakukan melalui form terpisah, menjaga alur transaksi penjualan utama tetap sederhana.
    *   **Komponen yang Diubah:**
        *   `.kiro/specs/loyalty-point/.config.kiro`, `.kiro/specs/loyalty-point/requirements.md`, `.kiro/specs/loyalty-point/tasks.md`: Penambahan dokumen spesifikasi, persyaratan, dan rencana implementasi mendetail untuk fitur poin loyalitas. Dokumen-dokumen ini mencakup:
            *   Pengenalan dan referensi *best practice* yang diadopsi (misalnya, *ledger-based tracking*, *dual earning mechanism*, *redemption as goods*).
            *   Glosarium teknis untuk istilah-istilah terkait poin (misalnya, `Loyalty_Engine`, `Poin_Ledger`, `Earn_Rate`, `FormMasterPoin`, `FormTukarPoin`).
            *   Delapan persyaratan fungsional lengkap, termasuk konfigurasi sistem poin, perolehan poin, penukaran poin dengan barang, pengaturan harga poin per barang, konsistensi poin saat retur penjualan, tampilan poin pada struk, riwayat poin pelanggan, dan integritas data poin.
            *   Rencana implementasi dengan 9 tugas teknis, mulai dari migrasi database hingga pengujian *end-to-end*, beserta detail file dan modul yang akan terlibat.
        *   `Database/32_loyalty_point_schema.sql`: Penambahan skema database baru untuk mendukung fitur poin loyalitas:
            *   Tabel `poin_config`: Menyimpan konfigurasi global sistem poin (misalnya, `AKTIF`, `MEKANISME`, `POIN_PER_QTY`, `KELIPATAN_NOMINAL`, `MINIMUM_REDEEM`).
            *   Tabel `poin_ledger`: Mencatat setiap transaksi poin (tipe `EARN`, `REDEEM`, `VOID_EARN`) sebagai baris yang *immutable* (tidak dapat diubah) untuk audit trail.
            *   Tabel `poin_barang`: Menyimpan konfigurasi harga poin yang dibutuhkan untuk menukar setiap barang.
            *   Penambahan kolom `SALDO_POIN` (tipe `int(11)`) pada tabel `tbl_pelanggan` untuk menyimpan saldo poin terkini pelanggan secara denormalisasi.
            *   Penambahan indeks pada tabel-tabel baru untuk optimasi performa *query*.

*   **Manajemen Sales Order (SO)**
    *   **Deskripsi:** Implementasi penuh fitur Sales Order yang memungkinkan pembuatan, pengelolaan, dan pelacakan pesanan penjualan dari pelanggan. Sales Order dapat dicetak sebagai "Nota Pesanan" dan nantinya dapat diproses menjadi transaksi penjualan sesungguhnya di FormJual, dengan mekanisme alokasi stok yang terintegrasi.
    *   **Komponen yang Diubah:**
        *   `AppKasir/0Form/FormUtama.vb`, `AppKasir/0Form/FormUtama.designer.vb`, `AppKasir/0Form/FormUtama.resx`:
            *   Penambahan tombol dan menu "Sales Order" di menu utama transaksi.
            *   Integrasi data Sales Order ke *datagrid* utama di FormUtama, termasuk fungsi `DataSalesOrder()` untuk menampilkan dan memuat data SO.
            *   Penambahan menu *context click* "Proses ke Penjualan Kasir" pada baris Sales Order di FormUtama, yang akan membuka FormJual dengan data SO tersebut.
            *   Perbaikan penanganan *overlay* FormCekUpdate di FormUtama untuk mencegah error dan memastikan tampilan yang benar.
        *   `AppKasir/2Trans/FormSalesOrder.vb`, `AppKasir/2Trans/FormSalesOrder.Designer.vb`, `AppKasir/2Trans/FormSalesOrder.resx`: Penambahan modul form Sales Order baru yang lengkap, termasuk:
            *   Pengelolaan header (nomor SO, tanggal, pelanggan, sales, lokasi).
            *   Input detail barang dengan fitur pencarian cepat via *barcode* atau nama, dukungan untuk satuan per jenis pelanggan (Umum/Partai), dan deteksi *auto-level* satuan berdasarkan kuantitas.
            *   Validasi stok *real-time* dan cek harga jual rugi (sesuai konfigurasi).
            *   Fitur simpan, edit, dan hapus Sales Order.
            *   *Tooltip* dinamis pada kolom *datagrid* untuk memandu pengguna.
            *   Integrasi printer untuk mencetak "Nota Pesanan".
        *   `AppKasir/1Master/FormHakUser.vb`: Penambahan hak akses "Sales Order" ke daftar hak akses pengguna.
        *   `AppKasir/0Form/ModuleHapusTransaksi.vb`: Penambahan fungsi `HapusSalesOrder` untuk menghapus data Sales Order dari database secara atomik, termasuk pembalikan alokasi stok.
        *   `AppKasir/Database/28_sales_order_migration.sql`: Penambahan tabel `sales_order` dan `sales_order_detail` dengan struktur lengkap untuk mencatat transaksi Sales Order.
        *   `AppKasir/Database/29_tambah_kolom_no_so_penjualan.sql`: Penambahan kolom `NO_SO` (varchar(30)) pada tabel `penjualan` untuk mencatat referensi Sales Order jika transaksi penjualan berasal dari konversi SO.
        *   `AppKasir/Database/30_sp_hlp_stok_validasi_so.sql`: Penambahan *stored procedure* `sp_hlp_stok_validasi_so` untuk melakukan validasi stok dengan mempertimbangkan alokasi stok dari Sales Order yang sedang diproses.

### ð Peningkatan & Perubahan

*   **Integrasi Sales Order ke Modul Penjualan (`FormJual`)**
    *   **Modul:** `AppKasir/2Trans/FormJual.vb`, `AppKasir/2Trans/FormJual.Designer.vb`, `AppKasir/2Trans/FormJual.resx`
    *   **Perubahan Teknis:**
        *   Penambahan variabel `draftSalesOrderAktif` untuk menyimpan nomor SO yang sedang diproses.
        *   Logika impor detail Sales Order ke *datagrid* FormJual saat SO dipilih dari FormUtama, termasuk penyesuaian tampilan stok dengan menambahkan kembali kuantitas SO yang telah dialokasikan.
        *   Penggunaan *stored procedure* `sp_hlp_stok_validasi_so` untuk validasi stok yang memperhitungkan alokasi SO, mencegah konflik stok saat konversi SO ke penjualan.
        *   Logika penghapusan Sales Order dari database setelah berhasil dikonversi menjadi transaksi penjualan.
        *   Penambahan kolom `NO_SO` pada operasi simpan transaksi penjualan di tabel `penjualan`.

*   **Integrasi Sales Order ke Modul Surat Jalan (`FormSuratJalan`)**
    *   **Modul:** `AppKasir/2Trans/FormSuratJalan.vb`, `AppKasir/2Trans/FormSuratJalan.Designer.vb`, `AppKasir/2Trans/FormSuratJalan.resx`
    *   **Perubahan Teknis:**
        *   Integrasi *query* untuk menampilkan daftar Sales Order aktif (dengan status 'Aktif') bersamaan dengan transaksi penjualan reguler di *datagrid* "Penjualan Siap Kirim".
        *   Penambahan kolom `SUMBER` pada *datagrid* DGVPenjualan untuk membedakan asal transaksi ("Jual" atau "SO").
        *   Pemberian warna latar belakang yang berbeda untuk baris Sales Order agar mudah dibedakan secara visual.
        *   Penambahan kolom `SUMBER_TRANS` pada *datagrid* DGVSuratJalan dan pada proses penyimpanan `surat_jalan_detail` untuk mencatat asal nota.
        *   `Database/31_tambah_kolom_sumber_surat_jalan_detail.sql`: Penambahan kolom `SUMBER` pada tabel `surat_jalan_detail` dengan *default* 'Jual'.

*   **Peningkatan Pencetakan Nota Sales Order**
    *   **Modul:** `AppKasir/6Print/CetakPenjualan/EscPosCetakjualThermalMatrik.vb`, `AppKasir/6Print/CetakPenjualan/FormMonitorRDLC.vb`, `AppKasir/6Print/CetakPenjualan/GdiCetakjualThermalMatrik.vb`, `AppKasir/6Print/CetakPenjualan/ModuleCetakJualInkjet.vb`, `AppKasir/6Print/CetakPenjualan/ModulePrinterJual.vb`
    *   **Perubahan Teknis:**
        *   Fungsionalitas cetak untuk Sales Order (`isSalesOrder` parameter) telah ditambahkan di `ModulePrinterJual`, membedakan judul nota menjadi "Nota SO" atau "Nota Order".
        *   Penyesuaian tata letak dan konten pada format cetak thermal (ESC/POS), GDI+, dan RDLC agar sesuai dengan karakteristik Sales Order (misalnya, menampilkan label "Nota Pesanan / Sales Order", menyertakan referensi nomor SO, tidak menampilkan detail pembayaran/kembalian pada Nota Order).
        *   Penambahan *helper* `DbStrSafe` di `ModulePrinterJual` untuk membaca nilai dari `MySqlDataReader` secara aman, mencegah error jika kolom tidak ada.

*   **Perbaikan Umum & Optimalisasi**
    *   **Modul:** `AppKasir/0Form/FormCekUpdate.vb`
    *   **Perubahan Teknis:** Peningkatan penanganan `Application.Exit()` agar aman dipanggil dari *thread* manapun melalui `Me.Invoke`, mencegah potensi *crash* aplikasi.
    *   **Modul:** `AppKasir/0Form/FormPembelian.vb`
    *   **Perubahan Teknis:** Perbaikan pada opsi cetak pembelian "TAMPILKAN DI MONITOR" untuk memastikan fungsionalitasnya berjalan dengan benar.

### âï¸ Perubahan Internal & Teknis

*   **Dokumentasi Steering:**
    *   **Modul:** `AppKasir/.kiro/steering/akses-database.md`, `AppKasir/.kiro/steering/coa-tbl-datareferensi.md`, `AppKasir/.kiro/steering/no-overwrite-files.md`
    *   **Perubahan Teknis:** Nama *database development* (`db_kasirlancar`) diubah menjadi `db_moroseneng` dalam dokumentasi internal untuk konsistensi.
*   **Update Informasi Proyek:**
    *   **Modul:** `AppKasir/My Project/AssemblyInfo.vb`, `update.xml`
    *   **Perubahan Teknis:** Nomor versi aplikasi diperbarui menjadi `15.2026.522.6`.

---
