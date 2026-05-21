Tentu, berikut adalah catatan rilis mendetail untuk aplikasi Kasir berdasarkan perubahan kode yang Anda berikan:

---

# Catatan Rilis Aplikasi Kasir

**Versi: 15.2026.522.8**
Tanggal Rilis: [Isi Tanggal Rilis di Sini]

Catatan rilis ini merinci perubahan dan peningkatan yang diterapkan pada aplikasi Kasir, berfokus pada fungsionalitas terkait penggajian dan pembaruan internal.

---

### Peningkatan Fungsionalitas & Pengalaman Pengguna

*   **Modul Gaji (4Gaji)**
    *   **Formulir Bon (File: `AppKasir/4Gaji/FormBon.vb`)**
        *   **Peningkatan Kontrol `DtpTanggal`:** Pada modul Bon, kontrol `DateTimePicker` (`DtpTanggal`) yang digunakan untuk memilih tanggal bon kini *selalu diaktifkan (enabled)*. Penambahan baris kode `DtpTanggal.Enabled = True` pada saat `FormBon` dimuat (`FormBon_Load`) dan saat mereset kontrol (`ResetControls`) memastikan bahwa pengguna dapat selalu mengubah tanggal bon, mengabaikan potensi pembatasan atau pengaturan hak akses sebelumnya yang mungkin menonaktifkan kontrol tersebut.
    *   **Formulir Gaji (File: `AppKasir/4Gaji/FormGaji.vb`)**
        *   **Peningkatan Kontrol `DtpTanggal`:** Serupa dengan formulir Bon, pada modul Gaji, kontrol `DateTimePicker` (`DtpTanggal`) untuk tanggal penggajian juga kini *selalu diaktifkan (enabled)*. Penambahan `DtpTanggal.Enabled = True` pada saat `FormGaji` dimuat (`FormGaji_Load`) memberikan fleksibilitas penuh kepada pengguna untuk mengatur tanggal penggajian tanpa terhalang oleh konfigurasi izin masa lalu.

### Pembaruan Internal & Infrastruktur

*   **Informasi Perakitan Aplikasi (File: `AppKasir/My Project/AssemblyInfo.vb`)**
    *   **Peningkatan Versi Aplikasi:** Versi internal aplikasi (`AssemblyVersion` dan `AssemblyFileVersion`) telah ditingkatkan dari `15.2026.522.6` menjadi `15.2026.522.8`. Ini merupakan pembaruan versi standar yang mengindikasikan adanya rilis baru.
*   **Konfigurasi Pembaruan Otomatis (File: `update.xml`)**
    *   **Penyesuaian Metadata Pembaruan:** File konfigurasi `update.xml`, yang digunakan oleh sistem pembaruan otomatis aplikasi, telah diperbarui. Atribut `<version>`, `<url>`, dan `<changelog>` kini menunjuk ke versi aplikasi `15.2026.522.8` yang baru, beserta tautan unduhan (`AppKasir_Update.zip`) dan URL changelog yang relevan di GitHub. Ini memastikan bahwa sistem pembaruan otomatis akan mengarahkan pengguna ke rilis terbaru ini.

---
