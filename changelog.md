```markdown
## Catatan Rilis Teknis Aplikasi Kasir - Versi 15.2026.522.15

### 1. Perubahan Versi Aplikasi
- **File:** `My Project/AssemblyInfo.vb`
- **Deskripsi:** 
  - Versi aplikasi diperbarui menjadi `15.2026.522.15` (sebagian dari versããåé² dari 15.2026.522.13).
  - Cambian versi file juga disesuaikan sehingga sesuai dengan versi utama.  
  - Perubahan ini memungkinkan identifikasi yang lebih jelas dan menumpuk update yang lebih besar.

---

### 2. Perubahan FormUtama (File: `FormUtama.designer.vb`)
- **File:** `AppKasir/0Form/FormUtama.designer.vb`
- **Deskripsi:**  
  - **Tambaah Tombol Baru:**
    - `BtnSalesOrder` (Tombol Sales Order) ditambahkan di posisi (112,5) dengan gaya penampilan yang lebih rapi.  
    - Stil visual termasuk WARNA gambar (hijau keemasan), font "Arial Narrow"eran bold, dan warna teks (biru tua).
  - **Perubahan Posisi Tombol:**
    - Tombol `BtnKirimCabang` dilengkapi ke posisi (898,5) dari (806,5).
    - Tombol lainnya seperti `BtnSuratJalan`, `BtnTransferBarang`, `BtnStokOpname`, `BtnBayarPiutang`, `BtnPindahStok`, `BtnRetuBelanja`, `BtnPenjualan`, `BtnReturPenjualan`, dan `BtnBayarHutang` juga dipindahkan dengan koordinat baru untuk memajukan layout tampilan.
  - **Optimalisasi Tata Letak:**
    - Semua tombol tetap dalam baris superi dengan penempatan horizontal yang lebih terdistribusikan.

---

### 3. Perubahan File Resurs (File: `FormUtama.resx`)
- **File:** `AppKasir/0Form/FormUtama.resx`
- **Deskripsi:**  
  - **Ubah Nama Icons Menu:**
    - `MenuUtility.Image` baru menjadi icon untuk menu umum (diseberangi `MenuLaporan.Image`).
    - Beberapa ikon menu lainnya diupdate, contoh:
      - `BackupDatabaseToolStripMenuItem.Image` menggantikan `RestoreDatabaseToolStripMenuItem.Image`.
      - `MenuKaryawan.Image`, `WindowToolStripMenuItem.Image`, dan lainnya diupdate dengan data base64 baru.
  - **Menambahkan Icon Baru:**
    - `MenuLaporan.Image` ditambahkan sebagai ikon baru untuk menu genera la laporsi.
  - **Perbaikan Data Icon:**
    - Beberapa ikon memiliki string ren fungus base64 yang lebih recent untuk mendukung desain GUI yang lebih konsisten.

---

### 4. Perubahan AssemblyInfo (File: `AssemblyInfo.vb`)
- **File:** `AppKasir/My Project/AssemblyInfo.vb`
- **Deskripsi:**  
  - Versi assembly diperbarui menjadi `15.2026.522.15`.
  - Versi file versi juga diubah sesuai versi utama.
  - Perubahan ini mencerminkan juga update install aplikasi dalam file `update.xml`.

---

### 5. Perubahan File Update (File: `update.xml`)
- **File:** `update.xml`
- **Deskripsi:**  
  - Versi aplikasi diperbarui menjadi `15.2026.522.15`.
  - URL dan link changelog untuk download update juga diupdate menjadi versi terkini.
  - Permintaan compulsori update tetap tidak aktif (`<mandatory>false</mandatory>`).

---

### Kesimpulan
Update ini mencakup peningkatan visual UIç°ä¸­ tombol, reposisi elemen untuk memajukan efisiensi, perbaikan ikon menup, serta penyesuaian versi aplikasi untuk mendukung fitur atau optimasi berikutnya.
```

