Berikut adalah changelog berdasarkan perubahan kode yang diberikan:

*   **`AppKasir/Installer/Publish-Release.ps1`**
    *   Memperbarui batas maksimal panjang pemotongan `diff` dari 12000 karakter menjadi 4000 karakter untuk mengoptimalkan penggunaan token dan mencegah kesalahan API.
    *   Mengimplementasikan perbaikan pada fungsi `ConvertTo-JsonString` untuk menghapus karakter kontrol ASCII (`\x00-\x08`, `\x0B`, `\x0C`, `\x0E-\x1F`) yang tidak di-escape, memastikan output JSON lebih valid dan konsisten.
    *   Melakukan pembersihan kode minor pada fungsi `New-JsonBody` dengan menyederhanakan nama variabel (`$modelJson` menjadi `$m`, `$contentJson` menjadi `$c`) dan format string.
    *   Menghapus langkah validasi JSON sisi klien (`ConvertFrom-Json`) sebelum mengirim permintaan API, karena fungsi `ConvertTo-JsonString` yang diperbarui kini diandalkan untuk menghasilkan JSON yang valid secara otomatis.
*   **`AppKasir/My Project/AssemblyInfo.vb`**
    *   Meningkatkan versi Assembly (`AssemblyVersion` dan `AssemblyFileVersion`) dari `2026.07.12.13` menjadi `2026.07.12.14`.
*   **`update.xml`**
    *   Memperbarui informasi versi pembaruan (`<version>`), URL unduhan (`<url>`), dan tautan changelog (`<changelog>`) dari `2026.07.12.13` menjadi `2026.07.12.14` untuk rilis terbaru.
