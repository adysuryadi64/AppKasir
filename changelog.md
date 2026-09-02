Berikut adalah changelog untuk aplikasi kasir Anda:

*   **AppKasir/1Master/TambahBarang.vb**
    *   Menerapkan format `ToString("0.####")` untuk menampilkan nilai harga beli (umum kecil, umum sedang, umum besar, partai kecil, partai sedang, partai besar) pada `TxtHargaBeli` di berbagai fungsi, memastikan presisi hingga empat angka desimal.
    *   Menambahkan validasi di mana jika `hargaJual` (harga jual) adalah 0, perhitungan laba (dalam rupiah dan persentase) serta label harga jual untuk `UmumKecil` akan disetel ke "0" dan proses akan dihentikan untuk mencegah perhitungan yang tidak valid.
    *   Memperbarui kondisi perhitungan persentase laba (`TxtLabaPersenUmumKecil.Text`) agar hanya memeriksa `_hargaBeli` (harga beli) untuk menghindari pembagian dengan nol.
    *   (Potongan `diff` menunjukkan penambahan validasi serupa untuk `UmumSedang` jika `hargaJual` adalah 0, tetapi detail lengkapnya tidak tersedia.)
