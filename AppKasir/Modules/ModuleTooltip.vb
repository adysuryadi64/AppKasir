Friend Module ModuleTooltip


    ''' <summary>Entry point — dipanggil sekali saat FormUtama_Load.</summary>
    Public Sub AturTooltip(frm As FormUtama)
        AturTooltipTombol(frm)
        AturTooltipMenu(frm)
    End Sub

    ' ==================== TOMBOL PANEL ====================
    Private Sub AturTooltipTombol(frm As FormUtama)
        With frm.ToolTip1
            .IsBalloon = True
            .ToolTipIcon = ToolTipIcon.Info
            .ToolTipTitle = "Keterangan Menu"

            ' --- MASTER ---
            .SetToolTip(frm.BtnToko,
                "🏢 DATA PERUSAHAAN / TOKO" & Environment.NewLine &
                "Kelola profil lengkap perusahaan:" & Environment.NewLine &
                "• Identitas: nama, alamat, kota, kontak, pemilik" & Environment.NewLine &
                "• Foto: logo nota, foto toko, foto gudang" & Environment.NewLine &
                "• Footer nota: 3 baris teks di bawah struk cetak" & Environment.NewLine &
                "• Akun jurnal default per transaksi: rekening barang," & Environment.NewLine &
                "  kas beli/jual toko & gudang, hutang, piutang, retur," & Environment.NewLine &
                "  bon karyawan, gaji, bayar hutang/piutang, transfer bank" & Environment.NewLine &
                "• Kode cloud untuk sinkronisasi antar cabang" & Environment.NewLine &
                "⚠️ Wajib diisi pertama kali sebelum menggunakan aplikasi.")

            .SetToolTip(frm.BtnBarang,
                "📦 DATA BARANG" & Environment.NewLine &
                "Kelola master barang dengan field lengkap:" & Environment.NewLine &
                "• Identitas: kode, nama, kategori, merk, supplier" & Environment.NewLine &
                "• Satuan umum (3 level): kecil/sedang/besar + isi + harga jual + barcode" & Environment.NewLine &
                "• Satuan partai (3 level): kecil/sedang/besar + isi + harga jual" & Environment.NewLine &
                "• Harga beli (HPP) dan harga beli terakhir" & Environment.NewLine &
                "• Stok awal, stok min, stok max, point member" & Environment.NewLine &
                "• Status aktif/nonaktif (nonaktif tidak muncul di transaksi)" & Environment.NewLine &
                "• Tambah/kurang stok manual langsung dari form ini" & Environment.NewLine &
                "⚠️ Wajib diisi sebelum transaksi pembelian/penjualan.")

            .SetToolTip(frm.BTnPelanggan,
                "🧑‍💼 DATA PELANGGAN" & Environment.NewLine &
                "Kelola master pelanggan:" & Environment.NewLine &
                "• Identitas: kode (otomatis), nama, alamat, no. telp" & Environment.NewLine &
                "• Jenis pelanggan dan jangka waktu piutang (hari)" & Environment.NewLine &
                "• Hutang awal (saldo piutang pembukaan) dan hutang akhir (saldo berjalan)" & Environment.NewLine &
                "• Status aktif/nonaktif" & Environment.NewLine &
                "Saldo piutang dihitung otomatis dari transaksi penjualan kredit" & Environment.NewLine &
                "dan pembayaran piutang yang sudah dicatat.")

            .SetToolTip(frm.BtnSupliyer,
                "🏬 DATA SUPPLIER" & Environment.NewLine &
                "Kelola master supplier:" & Environment.NewLine &
                "• Identitas: kode (otomatis), nama, alamat, no. HP" & Environment.NewLine &
                "• Jangka waktu hutang (hari jatuh tempo)" & Environment.NewLine &
                "• Hutang awal (saldo hutang pembukaan) dan hutang akhir (saldo berjalan)" & Environment.NewLine &
                "• Status aktif/nonaktif" & Environment.NewLine &
                "Saldo hutang dihitung otomatis dari transaksi pembelian kredit" & Environment.NewLine &
                "dan pembayaran hutang yang sudah dicatat.")

            .SetToolTip(frm.BtnTabelRef,
                "📚 TABEL REFERENSI AKUN NERACA" & Environment.NewLine &
                "Kelola Chart of Accounts (daftar akun keuangan):" & Environment.NewLine &
                "• Kode akun (otomatis per tipe), nama akun" & Environment.NewLine &
                "• Jenis akun: Aset Lancar, Aset Tetap, Pasiva, Modal, HPP, Biaya, Pendapatan Lain, Pajak" & Environment.NewLine &
                "• Tipe akun: KAS, BANK, PIUTANG, HUTANG, EKUITAS, BIAYA, dll" & Environment.NewLine &
                "• Sub akun: Aktiva, Pasiva, Laba, Rugi, Laba Rugi" & Environment.NewLine &
                "• Posisi normal: Debet atau Kredit" & Environment.NewLine &
                "• Saldo awal (saldo pembukaan)" & Environment.NewLine &
                "⚠️ Wajib diisi sebelum transaksi. Akun di sini dipakai sebagai" & Environment.NewLine &
                "rekening jurnal otomatis di semua transaksi (lihat Data Perusahaan).")

            .SetToolTip(frm.BtnArmada,
                "🚚 DATA ARMADA PENGIRIMAN" & Environment.NewLine &
                "Kelola kendaraan pengiriman:" & Environment.NewLine &
                "• Kode (otomatis), nomor polisi, jenis kendaraan" & Environment.NewLine &
                "Data armada dipilih saat membuat Surat Jalan." & Environment.NewLine &
                "Supir dan helper diambil dari Data Karyawan.")

            .SetToolTip(frm.BtnKaryawan,
                "👨‍🔧 DATA KARYAWAN" & Environment.NewLine &
                "Kelola master karyawan:" & Environment.NewLine &
                "• Kode (otomatis), nama, jabatan, tanggal masuk" & Environment.NewLine &
                "• Gaji pokok (dipakai sebagai dasar perhitungan gaji bulanan)" & Environment.NewLine &
                "• Saldo bon berjalan (dihitung otomatis dari transaksi bon)" & Environment.NewLine &
                "• Status aktif/nonaktif" & Environment.NewLine &
                "Data karyawan dipakai di modul Gaji, Bon, dan Surat Jalan (helper).")

            .SetToolTip(frm.BtnUser,
                "👤 DATA USER (PENGGUNA SISTEM)" & Environment.NewLine &
                "Kelola akun pengguna aplikasi:" & Environment.NewLine &
                "• Kode (otomatis), nama lengkap, username, password" & Environment.NewLine &
                "• Level: Owner / Master / Admin / Kasir / Gudang" & Environment.NewLine &
                "• Status aktif/nonaktif (nonaktif tidak bisa login)" & Environment.NewLine &
                "Level menentukan menu yang bisa diakses sesuai pengaturan" & Environment.NewLine &
                "di Hak Akses User. Setiap transaksi mencatat ID user yang login.")

            .SetToolTip(frm.BtnHakAksesUser,
                "🔐 HAK AKSES USER" & Environment.NewLine &
                "Atur izin per level user (Owner/Master/Admin/Kasir/Gudang)." & Environment.NewLine &
                "Izin per modul: Baca, Tambah, Edit, Hapus." & Environment.NewLine &
                "Modul yang bisa diatur:" & Environment.NewLine &
                "• Master: Toko, Barang, Harga Beli, Tambah/Kurang Stok, Pelanggan, Supplier, dll" & Environment.NewLine &
                "• Transaksi: Pembelian, Penjualan, Retur, Bayar Hutang/Piutang, Stok Opname, dll" & Environment.NewLine &
                "• Jurnal, Karyawan, Laporan, Utility, Posting" & Environment.NewLine &
                "Menu QueryDB, Hapus Transaksi hanya tampil untuk level Master." & Environment.NewLine &
                "Perubahan berlaku saat user login berikutnya.")

            .SetToolTip(frm.BtnKirimCabang,
                "🏪 DATA CABANG" & Environment.NewLine &
                "Kelola data cabang/toko: kode, nama, alamat, kota, HP, dan pemilik." & Environment.NewLine &
                "Data cabang disinkronisasi ke cloud untuk mendukung" & Environment.NewLine &
                "fitur Transfer Antar Cabang antar lokasi yang berbeda.")

            .SetToolTip(frm.BtnGeneralSetting,
                "⚙️ PENGATURAN UMUM (GENERAL SETTING)" & Environment.NewLine &
                "Atur perilaku transaksi per modul (Ya/Tidak):" & Environment.NewLine &
                "• Pembelian: fokus input, edit harga beli, tampilkan harga jual," & Environment.NewLine &
                "  update HPP (average/last), boleh beli tanpa supplier, isi nominal" & Environment.NewLine &
                "• Penjualan: fokus input, edit harga jual, jual stok minus," & Environment.NewLine &
                "  tampilkan stok, diskon per item, isi nominal otomatis" & Environment.NewLine &
                "• Retur beli/jual: fokus input, satuan, boleh minus, wajib alasan" & Environment.NewLine &
                "• Transfer stok: fokus input, satuan, boleh minus" & Environment.NewLine &
                "• Stok opname: fokus input, satuan, boleh minus" & Environment.NewLine &
                "• Umum: boleh input transaksi dengan tanggal lampau" & Environment.NewLine &
                "Setting disimpan ke database dan di-cache per sesi login.")

            ' --- TRANSAKSI ---
            .SetToolTip(frm.BtnBelanja,
                "📦 PEMBELIAN" & Environment.NewLine &
                "Catat pembelian barang dari supplier. Nomor otomatis: PB-YYMMDD-XXXX." & Environment.NewLine &
                "• Pilih supplier, input barang via ketik/scan barcode" & Environment.NewLine &
                "• Multi-satuan (kecil/sedang/besar), edit harga beli (sesuai setting)" & Environment.NewLine &
                "• Bayar: tunai (kas), transfer (bank), atau hutang (kredit)" & Environment.NewLine &
                "• Tahan transaksi (draft) → lanjutkan nanti via Pembelian Ditahan" & Environment.NewLine &
                "• HPP diupdate otomatis (metode average atau last sesuai setting)" & Environment.NewLine &
                "• Stok bertambah dan jurnal akuntansi dibuat otomatis saat simpan.")

            .SetToolTip(frm.BtnPenjualan,
                "🧾 PENJUALAN" & Environment.NewLine &
                "Catat penjualan ke pelanggan. Nomor otomatis: PJ-YYMMDD-XXXX." & Environment.NewLine &
                "• Pilih pelanggan, input barang via ketik/scan barcode" & Environment.NewLine &
                "• Diskon per item atau diskon total, PPN, ongkos kirim" & Environment.NewLine &
                "• Bayar: tunai, transfer bank, QRIS, atau piutang (kredit)" & Environment.NewLine &
                "• Hitung kembalian otomatis, assign sales/kasir" & Environment.NewLine &
                "• Tahan transaksi (draft) → lanjutkan nanti via Penjualan Ditahan" & Environment.NewLine &
                "• Stok berkurang dan jurnal akuntansi dibuat otomatis saat simpan." & Environment.NewLine &
                "• Edit pembayaran tersedia via klik kanan di daftar transaksi.")

            .SetToolTip(frm.BtnRetuBelanja,
                "🔁 RETUR PEMBELIAN" & Environment.NewLine &
                "Kembalikan barang ke supplier. Nomor otomatis: RP-YYMMDD-XXXX." & Environment.NewLine &
                "• Pilih supplier → cari nota pembelian asal via nomor nota" & Environment.NewLine &
                "• Pilih barang dan qty yang diretur, isi alasan (sesuai setting)" & Environment.NewLine &
                "• Pengembalian dana: tunai (kas), bank, atau kurangi hutang" & Environment.NewLine &
                "• Stok berkurang, hutang ke supplier dikurangi jika pembelian kredit" & Environment.NewLine &
                "• Jurnal akuntansi dibuat otomatis saat simpan.")

            .SetToolTip(frm.BtnReturPenjualan,
                "🔄 RETUR PENJUALAN" & Environment.NewLine &
                "Terima barang kembalian dari pelanggan. Nomor otomatis: RP-YYMMDD-XXXX." & Environment.NewLine &
                "• Cari nota penjualan asal → tampil daftar barang yang bisa diretur" & Environment.NewLine &
                "• Pilih barang dan qty, isi alasan retur (sesuai setting)" & Environment.NewLine &
                "• Tampil info: total jual, sudah dibayar, sisa tagihan" & Environment.NewLine &
                "• Stok bertambah, piutang pelanggan dikurangi jika penjualan kredit" & Environment.NewLine &
                "• Jurnal akuntansi dibuat otomatis saat simpan.")

            .SetToolTip(frm.BtnBayarHutang,
                "💸 BAYAR HUTANG KE SUPPLIER" & Environment.NewLine &
                "Catat pembayaran hutang ke supplier. Nomor otomatis: BH-YYMMDD-XXXX." & Environment.NewLine &
                "• Pilih supplier → tampil semua nota pembelian kredit yang belum lunas" & Environment.NewLine &
                "• Centang nota yang dibayar, isi nominal per nota" & Environment.NewLine &
                "• Metode bayar: kas, bank, atau ekuitas" & Environment.NewLine &
                "• Status nota otomatis jadi 'Lunas' saat hutang terlunasi penuh" & Environment.NewLine &
                "• Jurnal: Hutang Belanja (D) → Kas/Bank (K) dibuat otomatis.")

            .SetToolTip(frm.BtnBayarPiutang,
                "💰 TERIMA BAYAR PIUTANG PELANGGAN" & Environment.NewLine &
                "Catat penerimaan pembayaran dari pelanggan. Nomor: BP-YYMMDD-XXXX." & Environment.NewLine &
                "• Pilih pelanggan → tampil semua nota penjualan kredit yang belum lunas" & Environment.NewLine &
                "• Centang nota yang dibayar, isi nominal per nota" & Environment.NewLine &
                "• Metode terima: kas, bank, atau ekuitas" & Environment.NewLine &
                "• Status nota otomatis jadi 'Lunas' saat piutang terlunasi penuh" & Environment.NewLine &
                "• Jurnal: Kas/Bank (D) → Piutang Pelanggan (K) dibuat otomatis.")

            .SetToolTip(frm.BtnPindahStok,
                "📤 TRANSFER STOK ANTAR BARANG" & Environment.NewLine &
                "Konversi stok dari satu kode barang ke kode barang lain di lokasi yang sama." & Environment.NewLine &
                "• Input barang masuk (penerima) dan barang keluar (sumber)" & Environment.NewLine &
                "• Isi qty, satuan, harga masing-masing, dan uraian keterangan" & Environment.NewLine &
                "• Tampilan panel berbeda antara TOKO dan GUDANG" & Environment.NewLine &
                "• Contoh: pecah 1 karton → satuan kecil, atau gabung satuan kecil → besar" & Environment.NewLine &
                "• Stok barang sumber berkurang, stok barang tujuan bertambah" & Environment.NewLine &
                "• Selisih nilai dicatat ke jurnal akuntansi otomatis.")

            .SetToolTip(frm.BtnTransferBarang,
                "🔁 TRANSFER BARANG ANTAR LOKASI" & Environment.NewLine &
                "Pindahkan stok barang antara TOKO dan GUDANG. Nomor: TB-YYMMDD-XXXX." & Environment.NewLine &
                "• Arah transfer ditentukan otomatis dari lokasi login saat ini" & Environment.NewLine &
                "  (dari TOKO → ke GUDANG, atau dari GUDANG → ke TOKO)" & Environment.NewLine &
                "• Input barang via ketik/scan barcode, pilih satuan dan qty" & Environment.NewLine &
                "• Bisa input banyak barang sekaligus dalam satu nota" & Environment.NewLine &
                "• Stok lokasi asal berkurang, stok lokasi tujuan bertambah" & Environment.NewLine &
                "• History pergerakan stok tercatat untuk audit trail.")

            .SetToolTip(frm.BtnStokOpname,
                "📊 STOK OPNAME" & Environment.NewLine &
                "Sesuaikan stok sistem dengan hasil hitung fisik. Nomor: SO-YYMMDD-XXXX." & Environment.NewLine &
                "• Cari barang via nama/barcode, tampil stok sistem saat ini" & Environment.NewLine &
                "• Input stok nyata hasil hitung fisik" & Environment.NewLine &
                "• Selisih (nyata - sistem) dihitung dan dicatat otomatis" & Environment.NewLine &
                "• Stok sistem diperbarui sesuai stok nyata yang diinput" & Environment.NewLine &
                "• Jurnal penyesuaian persediaan dibuat otomatis saat simpan.")

            .SetToolTip(frm.BtnSuratJalan,
                "🚚 SURAT JALAN PENGIRIMAN" & Environment.NewLine &
                "Buat dokumen pengiriman barang ke pelanggan. Nomor: SJ-YYMMDD-XXXX." & Environment.NewLine &
                "• Pilih nota penjualan yang akan dikirim (bisa lebih dari satu)" & Environment.NewLine &
                "• Pilih armada (kendaraan), supir, dan helper dari data master" & Environment.NewLine &
                "• Tampil total pelanggan dan total nilai pengiriman" & Environment.NewLine &
                "• Cetak surat jalan sebagai dokumen resmi pengiriman." & Environment.NewLine &
                "• Surat jalan tidak mempengaruhi stok (stok sudah berkurang saat penjualan).")

            .SetToolTip(frm.BtnKirimCabang,
                "🏪 TRANSFER ANTAR CABANG" & Environment.NewLine &
                "Kirim atau terima barang antar cabang/toko. Nomor: TC-YYMMDD-XXXX." & Environment.NewLine &
                "• Mode KIRIM: input barang + qty → stok cabang asal berkurang," & Environment.NewLine &
                "  data dikirim ke server untuk diterima cabang tujuan" & Environment.NewLine &
                "• Mode TERIMA: tampil daftar kiriman masuk dari cabang lain," & Environment.NewLine &
                "  konfirmasi penerimaan → stok cabang tujuan bertambah" & Environment.NewLine &
                "• Mendukung import manual jika koneksi server tidak tersedia" & Environment.NewLine &
                "• History transfer tersimpan untuk audit trail antar cabang.")

            .SetToolTip(frm.BtnTukarPoin,
                "🎁 TUKAR POIN DENGAN BARANG" & Environment.NewLine &
                "Form penukaran poin loyalitas pelanggan dengan barang pilihan." & Environment.NewLine &
                "• Pilih pelanggan → saldo poin tampil otomatis" & Environment.NewLine &
                "• Pilih barang dan qty yang ingin ditukar" & Environment.NewLine &
                "• Total poin dibutuhkan dan sisa poin dihitung real-time" & Environment.NewLine &
                "• Konfirmasi penukaran → stok barang berkurang, saldo poin berkurang" & Environment.NewLine &
                "• Cetak bukti penukaran setelah transaksi berhasil." & Environment.NewLine &
                "• Tombol konfirmasi otomatis disabled jika saldo tidak mencukupi.")
        End With
    End Sub
    Private Sub AturTooltipMenu(frm As FormUtama)

        ' ==================== FILE ====================
        frm.FileToolStripMenuItem.ToolTipText = "Login, logout, registrasi aktivasi, dan keluar aplikasi."

        frm.LoginToolStripMenuItem.ToolTipText =
            "Buka form login untuk masuk ke aplikasi." & Environment.NewLine &
            "Jika form login sudah terbuka, tidak akan dibuka lagi." & Environment.NewLine &
            "Setelah login berhasil, pilih lokasi (TOKO/GUDANG)," & Environment.NewLine &
            "lalu proses loading: muat pengaturan, hak akses, saldo, dan notifikasi jatuh tempo."

        frm.LogOutToolStripMenuItem.ToolTipText =
            "Keluar dari sesi aktif tanpa menutup aplikasi." & Environment.NewLine &
            "Proses: tutup semua form → bersihkan cache hak akses → hentikan timer" & Environment.NewLine &
            "→ kunci semua menu → tampilkan form login kembali." & Environment.NewLine &
            "Setelah login ulang, cache hak akses dan loading dijalankan kembali."

        frm.RegristerToolStripMenuItem.ToolTipText =
            "Buka form aktivasi lisensi aplikasi." & Environment.NewLine &
            "Serial number digenerate otomatis dari hardware komputer ini" & Environment.NewLine &
            "(motherboard + processor + volume serial drive C)." & Environment.NewLine &
            "Masukkan kode aktivasi yang sesuai untuk mengaktifkan lisensi penuh." & Environment.NewLine &
            "Hubungi 082 335 314 336 / ADI untuk mendapatkan kode aktivasi."

        frm.KeluarToolStripMenuItem.ToolTipText =
            "Tutup aplikasi sepenuhnya." & Environment.NewLine &
            "Akan ditawarkan backup database terlebih dahulu sebelum keluar." & Environment.NewLine &
            "Jika backup dipilih, file ZIP akan dibuat otomatis di folder aplikasi."

        ' ==================== MASTER ====================
        frm.MenuMaster.ToolTipText = "Data induk: perusahaan, barang, pelanggan, supplier, user, hak akses, dll."

        ' ==================== TRANSAKSI ====================
        frm.MenuTransaksi.ToolTipText = "Semua transaksi operasional: beli, jual, retur, bayar hutang/piutang, stok opname, surat jalan."

        ' ==================== JURNAL ====================
        frm.MenuJurnal.ToolTipText =
            "Pencatatan jurnal keuangan manual." & vbCrLf &
            "Tipe: Pemasukan, Pengeluaran, Biaya, Setor ke Bos, Bayar Bon Pribadi, Pindah Rekening." & vbCrLf &
            "Setiap entri menggunakan sistem double-entry (Debet-Kredit)." & vbCrLf &
            "Nomor transaksi otomatis per tipe (MS/KL/BY/SB/BB/PR-YYMMDD-XXXX)."

        ' ==================== KARYAWAN ====================
        frm.MenuKaryawan.ToolTipText = "Penggajian, bon karyawan, dan laporan terkait karyawan."

        frm.MasterGajiToolStripMenuItem.ToolTipText =
            "Atur komponen gaji karyawan: gaji pokok, tunjangan, potongan." & vbCrLf &
            "Data ini menjadi dasar perhitungan gaji bulanan."

        frm.GajiKaryawanToolStripMenuItem.ToolTipText =
            "Proses pembayaran gaji bulanan karyawan." & vbCrLf &
            "Hitung gaji bersih (gaji pokok + tunjangan - potongan - bon)." & vbCrLf &
            "Cetak slip gaji dan catat ke jurnal pengeluaran."

        frm.BonKaryawanToolStripMenuItem.ToolTipText =
            "Catat uang muka / pinjaman sementara karyawan (bon)." & vbCrLf &
            "Bon akan dipotong otomatis dari gaji bulan berikutnya." & vbCrLf &
            "Riwayat bon dan status pelunasan tersimpan per karyawan."

        frm.BayarBonDiluarGajiToolStripMenuItem.ToolTipText =
            "Bayar/lunasi bon karyawan di luar siklus pemotongan gaji." & vbCrLf &
            "Digunakan jika karyawan melunasi bon secara tunai langsung."

        frm.LaporanGajiToolStripMenuItem.ToolTipText =
            "Laporan rekapitulasi penggajian per periode." & vbCrLf &
            "Tampilkan total gaji, tunjangan, potongan, dan gaji bersih per karyawan."

        frm.LaporanBonToolStripMenuItem.ToolTipText =
            "Laporan rekap bon seluruh karyawan." & vbCrLf &
            "Tampilkan total bon, yang sudah dibayar, dan sisa bon per periode."

        frm.LaporanBonPerKaryawanToolStripMenuItem.ToolTipText =
            "Laporan detail bon per karyawan." & vbCrLf &
            "Riwayat pengambilan bon, pembayaran, dan saldo bon tersisa."

        ' ==================== LAPORAN ====================
        frm.MenuLaporan.ToolTipText = "Semua laporan keuangan, stok, penjualan, pembelian, hutang, piutang, dan analisis bisnis."

        ' Keuangan
        frm.MutasiSaldoToolStripMenuItem.ToolTipText =
            "Laporan mutasi saldo kas dan bank per periode." & vbCrLf &
            "Menampilkan saldo awal, total debet, total kredit, dan saldo akhir" & vbCrLf &
            "untuk setiap akun kas/bank berdasarkan data di JurnalUmum."

        frm.MutasiBarangToolStripMenuItem.ToolTipText =
            "Laporan mutasi stok per item barang dengan saldo berjalan." & vbCrLf &
            "Pilih barang dan periode → tampil saldo awal, setiap transaksi masuk/keluar," & vbCrLf &
            "dan saldo akhir. Mendukung filter per lokasi (Toko/Gudang)." & vbCrLf &
            "Data diambil dari tabel HistoryBarang."

        frm.JurnalUmumToolStripMenuItem.ToolTipText =
            "Cetak laporan jurnal umum (semua entri akuntansi) per periode." & vbCrLf &
            "Kolom: No. Transaksi, Tanggal, Uraian, Akun Debet, Akun Kredit, Nominal." & vbCrLf &
            "Filter berdasarkan rentang tanggal atau bulan/tahun."

        frm.NeracaToolStripMenuItem.ToolTipText =
            "Laporan Neraca (Balance Sheet) dan Laba Rugi (Income Statement)." & vbCrLf &
            "Dihitung dari saldo awal akun + total debet/kredit di JurnalUmum." & vbCrLf &
            "Neraca: Aktiva = Pasiva + Modal. Laba Rugi: Pendapatan - Biaya." & vbCrLf &
            "Filter per tanggal atau bulan. Proses hitung bisa memakan waktu."

        frm.BukuBesarToolStripMenuItem.ToolTipText =
            "Laporan buku besar: riwayat transaksi kronologis per akun." & vbCrLf &
            "Tampilkan debet, kredit, dan saldo berjalan untuk akun yang dipilih."

        frm.BukuBesarPembantuToolStripMenuItem.ToolTipText =
            "Laporan buku besar pembantu untuk akun tertentu." & vbCrLf &
            "Contoh: detail hutang per supplier, detail piutang per pelanggan."

        ' Pembelian
        frm.PembelianToolStripMenuItem1.ToolTipText =
            "Laporan rekap pembelian per nota." & vbCrLf &
            "Tampilkan: nota, supplier, total, pembayaran, hutang, status."

        frm.PembelianDetailToolStripMenuItem.ToolTipText =
            "Laporan pembelian beserta detail barang per nota." & vbCrLf &
            "Tampilkan setiap item barang yang dibeli beserta qty, harga, dan total."

        frm.PembelianBarangToolStripMenuItem.ToolTipText =
            "Laporan pembelian dikelompokkan per item barang." & vbCrLf &
            "Berguna untuk analisis barang apa yang paling banyak dibeli."

        frm.PembelianDihutangToolStripMenuItem.ToolTipText =
            "Laporan pembelian yang dibayar dengan hutang (kredit ke supplier)." & vbCrLf &
            "Filter: semua / lunas / belum lunas."

        ' Penjualan
        frm.RekapPenjualanByNotaToolStripMenuItem.ToolTipText =
            "Rekap penjualan dikelompokkan per nota transaksi." & vbCrLf &
            "Tampilkan: nota, pelanggan, total, bayar, kembalian, piutang, status."

        frm.RekapPenjualanToolStripMenuItem.ToolTipText =
            "Rekap penjualan dikelompokkan per item barang." & vbCrLf &
            "Berguna untuk analisis barang apa yang paling banyak terjual."

        frm.PenjualanToolStripMenuItem1.ToolTipText =
            "Laporan penjualan per nota." & vbCrLf &
            "Tampilkan: nota, pelanggan, total, jenis bayar, status."

        frm.PenjualanDetailToolStripMenuItem.ToolTipText =
            "Laporan penjualan beserta detail barang per nota." & vbCrLf &
            "Tampilkan setiap item yang terjual beserta qty, harga, diskon, dan total."

        frm.PenjualanBarangToolStripMenuItem.ToolTipText =
            "Laporan penjualan dikelompokkan per item barang." & vbCrLf &
            "Berguna untuk analisis produk terlaris dan omset per barang."

        frm.PenjualanTerhutangToolStripMenuItem.ToolTipText =
            "Laporan penjualan kredit yang belum lunas (piutang)." & vbCrLf &
            "Filter: semua / lunas / belum lunas."

        frm.PenjualanSalesToolStripMenuItem.ToolTipText =
            "Laporan penjualan dikelompokkan per sales/kasir." & vbCrLf &
            "Berguna untuk evaluasi performa kasir atau sales."

        frm.PenjualanQtyToolStripMenuItem.ToolTipText =
            "Laporan penjualan berdasarkan jumlah qty terjual per barang." & vbCrLf &
            "Berguna untuk analisis volume penjualan."

        frm.PenjualanPPNNonPPNToolStripMenuItem.ToolTipText =
            "Laporan penjualan dipisah antara transaksi kena PPN dan non-PPN." & vbCrLf &
            "Berguna untuk pelaporan pajak keluaran (PPN)."

        ' Retur
        frm.ReturPembelianToolStripMenuItem1.ToolTipText =
            "Laporan retur pembelian per nota." & vbCrLf &
            "Tampilkan: nota retur, supplier, nota beli asal, total retur."

        frm.ReturPembelianDetailToolStripMenuItem.ToolTipText =
            "Laporan retur pembelian beserta detail barang yang diretur."

        frm.ReturPembelianBarangToolStripMenuItem.ToolTipText =
            "Laporan retur pembelian dikelompokkan per item barang." & vbCrLf &
            "Berguna untuk analisis barang yang sering diretur ke supplier."

        frm.ReturPenjualanToolStripMenuItem1.ToolTipText =
            "Laporan retur penjualan per nota." & vbCrLf &
            "Tampilkan: nota retur, pelanggan, nota jual asal, total retur."

        frm.ReturPenjualanDetailToolStripMenuItem.ToolTipText =
            "Laporan retur penjualan beserta detail barang yang dikembalikan."

        frm.ReturPenjualanBarangToolStripMenuItem.ToolTipText =
            "Laporan retur penjualan dikelompokkan per item barang." & vbCrLf &
            "Berguna untuk analisis barang yang sering dikembalikan pelanggan."

        ' Hutang
        frm.ByTanggalBelanjaToolStripMenuItem.ToolTipText =
            "Laporan hutang ke supplier, difilter berdasarkan tanggal pembelian." & vbCrLf &
            "Tampilkan: nota beli, supplier, total hutang, sudah dibayar, sisa hutang." & vbCrLf &
            "Filter: semua / lunas / belum lunas, per supplier."

        frm.ByTanggalPelunasanToolStripMenuItem.ToolTipText =
            "Laporan hutang ke supplier, difilter berdasarkan tanggal pelunasan." & vbCrLf &
            "Berguna untuk melihat hutang yang dilunasi dalam periode tertentu."

        frm.ByTanggalJatuhTempoToolStripMenuItem.ToolTipText =
            "Laporan hutang ke supplier, difilter berdasarkan tanggal jatuh tempo." & vbCrLf &
            "Berguna untuk monitoring hutang yang akan atau sudah jatuh tempo."

        ' Piutang
        frm.ByTanggalPenjualanToolStripMenuItem.ToolTipText =
            "Laporan piutang pelanggan, difilter berdasarkan tanggal penjualan." & vbCrLf &
            "Tampilkan: nota jual, pelanggan, total piutang, sudah dibayar, sisa." & vbCrLf &
            "Filter: semua / lunas / belum lunas, per pelanggan."

        frm.ByTanggalPelunasanToolStripMenuItem1.ToolTipText =
            "Laporan piutang pelanggan, difilter berdasarkan tanggal pelunasan." & vbCrLf &
            "Berguna untuk melihat piutang yang dilunasi dalam periode tertentu."

        frm.ByTanggalJatuhTempoToolStripMenuItem1.ToolTipText =
            "Laporan piutang pelanggan, difilter berdasarkan tanggal jatuh tempo." & vbCrLf &
            "Berguna untuk monitoring piutang yang akan atau sudah jatuh tempo."

        frm.RekapBayarHutangToolStripMenuItem.ToolTipText =
            "Rekap pembayaran hutang ke supplier per periode." & vbCrLf &
            "Tampilkan semua transaksi bayar hutang beserta nominalnya."

        frm.RekapBayarPiutangToolStripMenuItem.ToolTipText =
            "Rekap penerimaan pembayaran piutang dari pelanggan per periode." & vbCrLf &
            "Tampilkan semua transaksi terima bayar piutang beserta nominalnya."

        ' Kas Penjualan
        frm.KasPenjualanToolStripMenuItem.ToolTipText =
            "Laporan kas penjualan harian untuk rekonsiliasi kasir." & vbCrLf &
            "Filter per kasir, tanggal, atau bulan. Pisah tunai/non-tunai/piutang." & vbCrLf &
            "Tampilkan: total penjualan, total diterima, total piutang."

        ' Stok
        frm.TransferStokToolStripMenuItem.ToolTipText =
            "Laporan transfer stok antar barang (konversi satuan)." & vbCrLf &
            "Tampilkan: nota, barang masuk, barang keluar, qty, selisih nilai."

        frm.TransferBarangToolStripMenuItem1.ToolTipText =
            "Laporan transfer barang antar lokasi (Toko-Gudang) per nota." & vbCrLf &
            "Tampilkan: nota, lokasi, total item, total qty, total nilai."

        frm.TransferBarangDetailToolStripMenuItem.ToolTipText =
            "Laporan transfer barang antar lokasi beserta detail item per nota."

        frm.StokOpnameToolStripMenuItem.ToolTipText =
            "Laporan stok opname: hasil penyesuaian stok fisik vs sistem." & vbCrLf &
            "Tampilkan: barang, stok sistem, stok nyata, selisih, lokasi."

        frm.StokBarangToolStripMenuItem1.ToolTipText =
            "Laporan kondisi stok barang saat ini." & vbCrLf &
            "Filter: semua barang / stok ada / stok kosong / stok minus." & vbCrLf &
            "Tampilkan: kode, nama, kategori, stok toko, stok gudang, nilai HPP."

        frm.KartuStokToolStripMenuItem1.ToolTipText =
            "Kartu stok per item barang: riwayat lengkap keluar masuk stok." & vbCrLf &
            "Pilih barang (cari nama/barcode), pilih lokasi dan periode." & vbCrLf &
            "Tampilkan: saldo awal, setiap transaksi masuk/keluar, saldo berjalan." & vbCrLf &
            "Data diambil dari tabel HistoryBarang."

        frm.StokBarangTerlarisToolStripMenuItem.ToolTipText =
            "Daftar barang terlaris berdasarkan total qty terjual dalam periode." & vbCrLf &
            "Berguna untuk perencanaan pembelian dan analisis produk unggulan."

        frm.StokBarangTakBergerakToolStripMenuItem.ToolTipText =
            "Daftar barang yang tidak ada pergerakan stok (tidak terjual/dibeli)" & vbCrLf &
            "dalam periode tertentu. Berguna untuk evaluasi dead stock."

        frm.StokMinimumToolStripMenuItem1.ToolTipText =
            "Daftar barang yang stoknya di bawah batas minimum yang ditetapkan." & vbCrLf &
            "Berguna sebagai pengingat untuk segera melakukan pembelian ulang."

        ' Grafik & History
        frm.GrafikToolStripMenuItem.ToolTipText =
            "Laporan grafik laba/rugi dalam bentuk chart visual." & vbCrLf &
            "Tampilkan trend penjualan, pembelian, dan laba per periode."

        frm.HistoryToolStripMenuItem.ToolTipText =
            "Audit trail: riwayat semua pergerakan stok barang." & vbCrLf &
            "Tampilkan: faktur, tanggal, jenis transaksi, lokasi, qty, user." & vbCrLf &
            "Berguna untuk investigasi selisih stok atau audit internal."

        ' Ranking & Omset
        frm.RankingSupplierToolStripMenuItem.ToolTipText =
            "Ranking supplier berdasarkan total nilai pembelian dalam periode." & vbCrLf &
            "Berguna untuk negosiasi diskon dengan supplier terbesar."

        frm.RankingKasirUserPenjualanToolStripMenuItem.ToolTipText =
            "Ranking kasir/user berdasarkan total nilai penjualan yang diproses." & vbCrLf &
            "Berguna untuk evaluasi performa dan insentif kasir."

        frm.RankingBarangTerbanyakDibeliToolStripMenuItem.ToolTipText =
            "Ranking barang berdasarkan total qty yang dibeli dari supplier." & vbCrLf &
            "Berguna untuk analisis kebutuhan stok dan negosiasi harga."

        frm.RankingPelangganPiutangTerbesarToolStripMenuItem.ToolTipText =
            "Ranking pelanggan berdasarkan total sisa piutang terbesar." & vbCrLf &
            "Berguna untuk prioritas penagihan piutang."

        frm.RankingSupplierHutangTerbesarToolStripMenuItem.ToolTipText =
            "Ranking supplier berdasarkan total sisa hutang terbesar." & vbCrLf &
            "Berguna untuk prioritas pembayaran hutang."

        frm.OmsetPerPelangganToolStripMenuItem.ToolTipText =
            "Laporan omset penjualan dikelompokkan per pelanggan." & vbCrLf &
            "Berguna untuk analisis pelanggan terbesar dan program loyalitas."

        frm.OmsetPerKategoriToolStripMenuItem.ToolTipText =
            "Laporan omset penjualan dikelompokkan per kategori barang." & vbCrLf &
            "Berguna untuk analisis kategori produk yang paling menguntungkan."

        ' ==================== POSTING ====================
        frm.MenuPosting.ToolTipText = "Sinkronisasi data transaksi ke stok dan jurnal akuntansi."

        frm.PostingTokoToolStripMenuItem.ToolTipText =
            "Posting transaksi lokasi TOKO." & vbCrLf &
            "Proses: reset stok toko → rebuild dari HistoryBarang → hitung ulang stok." & vbCrLf &
            "Juga menghitung ulang: saldo neraca, bon, piutang, hutang, jatuh tempo." & vbCrLf &
            "Lakukan rutin agar stok dan laporan selalu akurat."

        frm.PostingGudangToolStripMenuItem.ToolTipText =
            "Posting transaksi lokasi GUDANG." & vbCrLf &
            "Proses: reset stok gudang → rebuild dari HistoryBarang → hitung ulang stok." & vbCrLf &
            "Juga menghitung ulang: saldo neraca, bon, piutang, hutang, jatuh tempo."

        frm.PostingSemuaToolStripMenuItem.ToolTipText =
            "Posting semua transaksi (Toko + Gudang) sekaligus." & vbCrLf &
            "Proses lebih lama tapi memastikan semua data tersinkronisasi penuh."

        ' ==================== UTILITY ====================
        frm.MenuUtility.ToolTipText = "Pengaturan sistem, backup/restore database, printer, dan tools administrator."

        frm.PilihanSaatMasukAplikasiToolStripMenuItem.ToolTipText =
            "Atur tampilan dan aksi default saat aplikasi pertama dibuka." & vbCrLf &
            "Contoh: langsung buka form penjualan atau tampilkan menu utama."

        frm.DatabaseToolStripMenuItem.ToolTipText =
            "Konfigurasi koneksi database MySQL." & vbCrLf &
            "Atur: server/IP, port, username, password, dan nama database." & vbCrLf &
            "Perubahan memerlukan restart aplikasi."

        frm.FormatSqlToolStripMenuItem.ToolTipText =
            "Backup database ke file format SQL (.sql)." & vbCrLf &
            "File SQL bisa dibuka dan diedit dengan text editor."

        frm.FormatZipToolStripMenuItem.ToolTipText =
            "Backup database ke file format ZIP (.zip)." & vbCrLf &
            "Ukuran file lebih kecil, cocok untuk penyimpanan dan transfer."

        frm.FormatSqlToolStripMenuItem1.ToolTipText =
            "Restore database dari file backup format SQL (.sql)." & vbCrLf &
            "PERHATIAN: Data yang ada akan ditimpa oleh data dari file backup."

        frm.FormatZipToolStripMenuItem1.ToolTipText =
            "Restore database dari file backup format ZIP (.zip)." & vbCrLf &
            "PERHATIAN: Data yang ada akan ditimpa oleh data dari file backup."

        frm.PerbaikiDatabaseToolStripMenuItem.ToolTipText =
            "Tools untuk memperbaiki kerusakan atau inkonsistensi database." & vbCrLf &
            "Jalankan jika ada error saat membuka atau menyimpan data."

        frm.UpdateTabelDatabaseToolStripMenuItem.ToolTipText =
            "Perbarui struktur tabel database ke versi terbaru." & vbCrLf &
            "Diperlukan setelah update aplikasi yang mengubah skema database."

        frm.QueryDatabaseToolStripMenuItem.ToolTipText =
            "Eksekusi perintah SQL langsung ke database." & vbCrLf &
            "Hanya untuk administrator/developer. Gunakan dengan hati-hati."

        frm.MigrasiDatabaseToolStripMenuItem.ToolTipText =
            "Pindahkan data dari database lama ke database baru." & vbCrLf &
            "Digunakan saat ganti server atau upgrade versi database."

        frm.SettingPrinterToolStripMenuItem.ToolTipText =
            "Konfigurasi printer untuk setiap jenis dokumen cetak." & vbCrLf &
            "Atur: printer default, cetak otomatis/manual, untuk setiap modul" & vbCrLf &
            "(Pembelian, Penjualan, Retur, Bayar Hutang/Piutang, Surat Jalan, dll)."

        frm.HapusTransaksiTokoToolStripMenuItem.ToolTipText =
            "Hapus (TRUNCATE) semua data transaksi lokasi TOKO dari database." & vbCrLf &
            "Termasuk: penjualan, pembelian, retur, jurnal, history barang, dll." & vbCrLf &
            "PERHATIAN: Operasi ini TIDAK DAPAT DIBATALKAN! Backup dulu sebelum lanjut." & vbCrLf &
            "Hanya tersedia untuk level Master."

        frm.HapusTransaksiGudangToolStripMenuItem.ToolTipText =
            "Hapus (TRUNCATE) semua data transaksi lokasi GUDANG dari database." & vbCrLf &
            "PERHATIAN: Operasi ini TIDAK DAPAT DIBATALKAN! Backup dulu sebelum lanjut." & vbCrLf &
            "Hanya tersedia untuk level Master."

        frm.PeriksaUpdateAplikasiToolStripMenuItem.ToolTipText =
            "Cek ketersediaan versi terbaru aplikasi." & vbCrLf &
            "Jika ada update, download dan install versi terbaru."

        frm.CekIpKomputerToolStripMenuItem.ToolTipText =
            "Tampilkan alamat IP komputer ini di jaringan lokal." & vbCrLf &
            "Berguna untuk konfigurasi koneksi multi-komputer."

        ' ==================== WINDOW ====================
        frm.WindowToolStripMenuItem.ToolTipText = "Atur tata letak jendela MDI yang sedang terbuka."
        frm.CascadeToolStripMenuItem.ToolTipText = "Tata semua jendela secara bertumpuk (cascade)."
        frm.TitleHorizontalToolStripMenuItem.ToolTipText = "Tata semua jendela secara horizontal berdampingan."
        frm.TitelVerticalToolStripMenuItem.ToolTipText = "Tata semua jendela secara vertikal berdampingan."
        frm.ArrangeIconsToolStripMenuItem.ToolTipText = "Rapikan ikon jendela yang sedang diminimize."
        frm.CloseAllToolStripMenuItem.ToolTipText = "Tutup semua jendela MDI yang sedang terbuka."

        ' ==================== HELP ====================
        frm.HelpToolStripMenuItem.ToolTipText =
            "Informasi bantuan dan kontak pengembang." & vbCrLf &
            "Hubungi: 082 335 314 336 / ADI untuk bantuan teknis."
    End Sub

End Module
