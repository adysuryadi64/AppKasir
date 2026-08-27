Berikut adalah changelog untuk aplikasi kasir (POS) Anda:

- **AppKasir/1Master/FormBarang.vb**
  - Mengimplementasikan penggunaan fungsi `ModuleAngka.ParseDecimal` untuk konversi nilai ke desimal pada berbagai input harga dan stok. Perubahan ini menggantikan `Decimal.Parse(value.ToString())` dan bertujuan untuk meningkatkan ketahanan aplikasi terhadap nilai `DBNull` atau format angka non-standar dari database, sehingga mengurangi potensi kesalahan saat menampilkan data.
  - Perubahan ini berlaku pada pengisian nilai untuk:
    - Harga jual umum (kecil, sedang, besar).
    - Harga jual partai (kecil, sedang, besar).
    - Harga beli (`TxtHrgBeli`, `LblHargaUntukEdit`, `TxtHargaBeliTerakhir`).
    - Stok (`TxtIsiStokToko`, `TxtIsiStokGudang`).
