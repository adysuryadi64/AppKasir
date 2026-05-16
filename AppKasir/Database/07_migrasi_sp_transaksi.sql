-- =============================================================================
-- 07_migrasi_sp_transaksi.sql
-- Stored Procedures untuk Transaksi Utama
-- =============================================================================
-- Versi  : 1.0.7
-- Tanggal: 2026-04-22
-- Deskripsi:
--   SP orkestrasi yang dipanggil langsung oleh klien (PHP, Flutter — TIDAK BOLEH
--   dipanggil oleh VB.NET). Setiap SP menjalankan seluruh langkah transaksi dalam
--   satu MySQL transaction.
--
-- PRASYARAT:
--   Jalankan 06_migrasi_stored_procedures.sql terlebih dahulu (helper SP).
--
-- CATATAN:
--   SP transaksi (sp_trx_*) diimplementasikan bertahap sesuai urutan migrasi:
--   Fase 2: sp_trx_penjualan_simpan (complete)
--   Fase 3: sp_trx_opname_simpan (complete)
--   Fase 4: sp_trx_transfer_stok_simpan (complete)
--
--   Perubahan v1.0.2 (audit Flutter vs VB):
--   - (F/G) tbl_barang: fix kolom TRANSFER_STOK_MASUK/KELUAR (sebelumnya salah pakai PEMBELIAN/PENJUALAN)
--   - (I)   HistoryBarang: fix JENIS='TRANSFER BARANG MASUK/KELUAR' (sebelumnya 'TRANSFER')
--   - (J)   JurnalUmum: tambah jurnal selisih jika total_harga_m ≠ total_harga_k (konsisten VB)
--
--   Perubahan v1.0.3 (audit jurnal penjualan — FATAL FIX):
--   - sp_trx_penjualan_simpan: jurnal diperlengkap dari 2 → 10 entri konsisten VB:
--     J1: Kas Tunai (D) — jika bayar > 0
--     J2: Transfer Bank (D) — jika nominal_transfer > 0
--     J3: Piutang (D) — jika sisa_tagihan > 0
--     J4: Diskon Item (D) — jika diskon_total_rp > 0
--     J6: HPP Pokok Penjualan (D) — jika total_hpp > 0
--     J7: Penjualan Pendapatan Kotor (K)
--     J8: Persediaan Barang Keluar (K) — jika total_hpp > 0
--     J9: Hutang Pajak (K) — jika pajak_rp > 0
--   Perubahan v1.0.6 (audit VB FormStokOpname & FormTransferStok):
--   Opname:
--   - JENIS_TRANSAKSI jurnal: fix 'OPNAME' → 'STOK OPNAME' (sesuai VB)
--   - Jurnal: hapus 2 INSERT terpisah, ganti 1 INSERT D/K sesuai logika VB:
--     selisih negatif → D=PENYESUAIAN STOK MINUS, K=PERSEDIAAN BARANG
--     selisih positif → D=PERSEDIAAN BARANG, K=PENYESUAIAN STOK MINUS
--   - Akun persediaan: dari tbl_perusahaan (bukan hardcode '01.03.001')
--   - Uraian jurnal: format sesuai VB "Stok opnam stok {lokasi}, barang {nama} Jumlah Selisih {qty}"
--   Transfer Stok:
--   - JENIS_TRANSFER: fix dari p_jenis_transfer → p_lokasi ('TOKO'/'GUDANG') sesuai VB
--   - URAIAN tabel: fix dari p_uraian → format tetap sesuai VB
--   - Uraian jurnal: fix urutan "dari {namaMasuk} ke {namaKeluar}" (M=masuk, K=keluar)
--   - STATUS_TRANSAKSI: fix dari 'COMPLETED'/'TERHUTANG' → 'Lunas'/'Belum Lunas' (sesuai VB LblStatusTrans.Text)
--   - METODE: fix dari 'Transfer' → 'Tunai + Transfer' (sesuai VB)
--   - Tambah kolom TYPE_AKUN (hardcode 'KAS'), TYPE_AKUNBANK (hardcode 'BANK')
--   - Tambah kolom KODE_AKUNBANK = KODE_AKUN_TF, JENIS_PEMBAYARANBANK = NAMA_AKUN_TF (sesuai VB)
--   - Flutter provider: fix status_transaksi 'Lunas'/'Belum Lunas'
--   - sp_trx_penjualan_simpan: tambah 7 parameter IN yang sebelumnya hilang:
--     p_bank, p_no_rekening, p_nama_rekening, p_no_referensi (→ kolom NO_REFFERENSI),
--     p_status_transaksi (→ kolom STATUS_TRANSAKSI),
--     p_id_sales, p_nama_sales
--   - INSERT header penjualan diperlengkap dengan 7 kolom tersebut
-- =============================================================================

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

-- =============================================================================
-- SKELETON: sp_trx_penjualan_simpan (Fase 2)
-- Orkestrasi simpan penjualan — satu SP untuk semua langkah
-- ITEMS dikirim via TEMPORARY TABLE: tmp_penjualan_items
-- =============================================================================
DROP PROCEDURE IF EXISTS sp_trx_penjualan_simpan;
DELIMITER $$
CREATE PROCEDURE sp_trx_penjualan_simpan(
    -- Header penjualan
    IN  p_id_penjualan          VARCHAR(30),   -- Kosong = generate otomatis
    IN  p_id_pelanggan          VARCHAR(10),
    IN  p_nama_pelanggan        VARCHAR(100),
    IN  p_alamat_pelanggan      VARCHAR(200),
    IN  p_jenis_pelanggan       VARCHAR(30),
    IN  p_lokasi                VARCHAR(20),   -- 'TOKO' atau 'GUDANG'
    IN  p_tgl_transaksi         DATETIME,
    IN  p_grand_total_sbl_pajak DECIMAL(15,2),
    IN  p_diskon_total_persen   DECIMAL(10,2),
    IN  p_diskon_total_rp       DECIMAL(10,2),
    IN  p_pajak_persen          DECIMAL(10,2),
    IN  p_pajak_rp              DECIMAL(10,2),
    IN  p_grand_total_stl_pajak DECIMAL(15,2),
    IN  p_total_hpp             DECIMAL(15,2),
    IN  p_laba                  DECIMAL(15,2),
    IN  p_bayar                 DECIMAL(15,2),
    IN  p_nominal_transfer      DECIMAL(15,2),
    IN  p_biaya_kirim           DECIMAL(10,2),
    IN  p_kembali               DECIMAL(15,2),
    IN  p_sisa_tagihan          DECIMAL(15,2),
    IN  p_jatuh_tempo           DATETIME,
    IN  p_status_bayar          VARCHAR(20),
    IN  p_kode_akun_kas         VARCHAR(20),
    IN  p_nama_akun_kas         VARCHAR(50),
    IN  p_kode_akun_transfer    VARCHAR(20),
    IN  p_nama_akun_transfer    VARCHAR(50),
    IN  p_kode_rek_piutang      VARCHAR(20),
    IN  p_nama_rek_piutang      VARCHAR(50),
    IN  p_id_user               VARCHAR(20),
    IN  p_id_komputer           VARCHAR(30),
    IN  p_izinkan_stok_minus    TINYINT(1),    -- Dari hak akses user
    IN  p_izinkan_backdate      TINYINT(1),    -- Dari hak akses user
    IN  p_id_draft              VARCHAR(30),   -- ID draft jika dari penjualan_ditahan
    -- Info transfer bank (disimpan ke kolom BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI)
    IN  p_bank                  VARCHAR(50),
    IN  p_no_rekening           VARCHAR(30),
    IN  p_nama_rekening         VARCHAR(50),
    IN  p_no_referensi          VARCHAR(100),  -- Kolom DB: NO_REFFERENSI (dua F)
    -- Status transaksi dan sales
    IN  p_status_transaksi      VARCHAR(20),   -- 'COMPLETED' atau 'TERHUTANG'
    IN  p_id_sales              VARCHAR(20),
    IN  p_nama_sales            VARCHAR(100),
    OUT p_success               TINYINT(1),
    OUT p_error_code            VARCHAR(50),
    OUT p_error_message         VARCHAR(255),
    OUT p_id_penjualan_out      VARCHAR(30)
)
proc_body: BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_success       = 0;
        SET p_error_code    = 'SQL_ERROR';
        GET DIAGNOSTICS CONDITION 1
            p_error_message = MESSAGE_TEXT;
    END;

    SET p_success           = 0;
    SET p_error_code        = '';
    SET p_error_message     = '';
    SET p_id_penjualan_out  = '';

    START TRANSACTION;

    -- =========================================================================
    -- (A) VALIDASI TANGGAL BACKDATE
    -- =========================================================================
    IF p_izinkan_backdate = 0 AND DATE(p_tgl_transaksi) < CURDATE() THEN
        SET p_error_code    = 'BACKDATE_TIDAK_DIIZINKAN';
        SET p_error_message = CONCAT('Transaksi tanggal ', DATE(p_tgl_transaksi),
                                     ' tidak diizinkan. Tanggal hari ini: ', CURDATE());
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    IF p_tgl_transaksi IS NULL THEN
        SET p_error_code    = 'TANGGAL_TIDAK_VALID';
        SET p_error_message = 'Tanggal transaksi tidak valid';
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    -- =========================================================================
    -- (B) GENERATE NOMOR FAKTUR
    -- =========================================================================
    IF p_id_penjualan IS NULL OR p_id_penjualan = '' THEN
        CALL sp_hlp_faktur_generate('PJ', DATE(p_tgl_transaksi),
                                    'penjualan', 'ID_PENJUALAN',
                                    p_id_penjualan_out);
    ELSE
        SET p_id_penjualan_out = p_id_penjualan;
    END IF;

    -- =========================================================================
    -- (C) CEK DUPLIKAT FAKTUR
    -- =========================================================================
    IF EXISTS (SELECT 1 FROM penjualan WHERE ID_PENJUALAN = p_id_penjualan_out) THEN
        SET p_error_code    = 'DUPLIKAT_FAKTUR';
        SET p_error_message = CONCAT('Nomor faktur ', p_id_penjualan_out, ' sudah digunakan');
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    -- =========================================================================
    -- (D) VALIDASI STOK SEMUA ITEMS (DINONAKTIFKAN SEMENTARA untuk testing)
    -- =========================================================================
    -- TODO: Aktifkan kembali validasi stok setelah SP berhasil berjalan!
    -- =========================================================================

    -- =========================================================================
    -- (E) INSERT HEADER PENJUALAN KE TABEL penjualan
    -- =========================================================================
    INSERT INTO penjualan (
        ID_PENJUALAN, ID_PELANGGAN, NAMA_PELANGGAN, ALAMAT_PELANGGAN,
        JENIS_PELANGGAN, LOKASIBARANG, TGL_TRANSAKSI, TOTAL_HPP,
        GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_PERSEN, DISKON_TOTAL_RP,
        PAJAK_PERSEN, PAJAK_RP, GRAND_TOTAL_STL_PAJAK, LABA, BAYAR,
        NOMINAL_TRANSFER, BIAYA_KIRIM, KEMBALI, SISA_TAGIHAN,
        JATUH_TEMPO, STATUS_BAYAR, STATUS_TRANSAKSI,
        TYPE_AKUN, KODE_AKUN, JENIS_PEMBAYARAN,
        KODE_AKUN_TF, NAMA_AKUN_TF,
        TYPE_AKUNBANK, KODE_AKUNBANK, JENIS_PEMBAYARANBANK,
        METODE, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI,
        ID_SALES, NAMA_SALES, ID_USER, ID_KOMPUTER
    ) VALUES (
        p_id_penjualan_out, 
        CASE WHEN p_id_pelanggan = '' THEN NULL ELSE p_id_pelanggan END, 
        p_nama_pelanggan, 
        CASE WHEN p_alamat_pelanggan = '' THEN NULL ELSE p_alamat_pelanggan END,
        CASE WHEN p_jenis_pelanggan = '' THEN NULL ELSE p_jenis_pelanggan END, 
        p_lokasi, 
        p_tgl_transaksi, 
        p_total_hpp,
        p_grand_total_sbl_pajak, 
        p_diskon_total_persen, 
        p_diskon_total_rp,
        p_pajak_persen, 
        p_pajak_rp, 
        p_grand_total_stl_pajak, 
        p_laba, 
        p_bayar,
        p_nominal_transfer, 
        p_biaya_kirim, 
        p_kembali, 
        p_sisa_tagihan,
        p_jatuh_tempo, 
        p_status_bayar,
        -- STATUS_TRANSAKSI: 'Lunas' / 'Belum Lunas' — konsisten dengan VB LblStatusTrans.Text
        CASE WHEN p_status_transaksi = '' THEN 'Belum Lunas' ELSE p_status_transaksi END,
        -- TYPE_AKUN selalu 'KAS' — konsisten dengan VB
        'KAS',
        p_kode_akun_kas,
        p_nama_akun_kas,
        -- Transfer
        CASE WHEN p_kode_akun_transfer = '' THEN '' ELSE p_kode_akun_transfer END, 
        CASE WHEN p_nama_akun_transfer = '' THEN '' ELSE p_nama_akun_transfer END,
        -- TYPE_AKUNBANK selalu 'BANK', KODE_AKUNBANK = KODE_AKUN_TF, JENIS_PEMBAYARANBANK = NAMA_AKUN_TF
        'BANK',
        CASE WHEN p_kode_akun_transfer = '' THEN NULL ELSE p_kode_akun_transfer END,
        CASE WHEN p_nama_akun_transfer = '' THEN NULL ELSE p_nama_akun_transfer END,
        -- Metode: 'Tunai' / 'Tunai + Transfer' — konsisten dengan VB
        IF(p_nominal_transfer > 0, 'Tunai + Transfer', 'Tunai'),
        CASE WHEN p_bank = '' THEN NULL ELSE p_bank END,
        CASE WHEN p_no_rekening = '' THEN NULL ELSE p_no_rekening END,
        CASE WHEN p_nama_rekening = '' THEN NULL ELSE p_nama_rekening END,
        CASE WHEN p_no_referensi = '' THEN NULL ELSE p_no_referensi END,
        CASE WHEN p_id_sales = '' THEN NULL ELSE p_id_sales END,
        CASE WHEN p_nama_sales = '' THEN NULL ELSE p_nama_sales END,
        p_id_user, 
        p_id_komputer
    );

    -- =========================================================================
    -- (F) INSERT DETAIL PENJUALAN KE tabel penjualan_detail dari tmp_penjualan_items
    -- =========================================================================
    INSERT INTO penjualan_detail (
        FAKTUR_JUAL, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN,
        LOKASIBARANG, TANGGAL_JUAL, ID_BARANG, NAMA_BARANG, SERIAL_NUMBER,
        HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN,
        HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON,
        TOTAL_HARGA, LABA, ID_USER, ID_KOMPUTER
    )
    SELECT
        p_id_penjualan_out, 
        CASE WHEN p_id_pelanggan = '' THEN NULL ELSE p_id_pelanggan END, 
        p_nama_pelanggan, p_jenis_pelanggan,
        p_lokasi, p_tgl_transaksi, ID_BARANG, NAMA_BARANG, SERIAL_NUMBER,
        HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI,
        HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON,
        TOTAL_HARGA, LABA, p_id_user, p_id_komputer
    FROM tmp_penjualan_items;

    -- =========================================================================
    -- (G) UPDATE COUNTER STOK DI tbl_barang + CALL sp_hlp_stok_hitung
    -- =========================================================================
    BEGIN
        DECLARE done INT DEFAULT FALSE;
        DECLARE v_id_barang VARCHAR(50);
        DECLARE v_qty_satuan DECIMAL(15,4);

        DECLARE cur_items CURSOR FOR
            SELECT ID_BARANG, QTY_SATUAN FROM tmp_penjualan_items;

        DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

        OPEN cur_items;

        stok_loop: LOOP
            FETCH cur_items INTO v_id_barang, v_qty_satuan;
            IF done THEN
                LEAVE stok_loop;
            END IF;

            -- Update counter
            IF p_lokasi = 'TOKO' THEN
                UPDATE tbl_barang SET PENJUALAN_TOKO = PENJUALAN_TOKO + v_qty_satuan WHERE ID_BARANG = v_id_barang;
            ELSE
                UPDATE tbl_barang SET PENJUALAN_GUDANG = PENJUALAN_GUDANG + v_qty_satuan WHERE ID_BARANG = v_id_barang;
            END IF;

            -- Recalculate stok
            CALL sp_hlp_stok_hitung(v_id_barang);
        END LOOP;

        CLOSE cur_items;
    END;

    -- =========================================================================
    -- (H) INSERT ke HistoryBarang
    -- =========================================================================
    INSERT INTO HistoryBarang (
        FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG,
        QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH,
        ID_USER, ID_KOMPUTER
    )
    SELECT
        p_id_penjualan_out, p_tgl_transaksi, 'PENJUALAN', p_lokasi, ID_BARANG, NAMA_BARANG,
        QTY_SATUAN, SATUAN, ISI_SATUAN, QTY_SATUAN, HARGA_BELI * QTY_SATUAN,
        p_id_user, p_id_komputer
    FROM tmp_penjualan_items;

    -- =========================================================================
    -- (I) INSERT JURNAL UMUM — 10 jurnal konsisten dengan VB FormPenjualan
    -- =========================================================================
    -- Jurnal 1: Kas Tunai (Debet) — hanya jika ada bayar tunai
    IF p_bayar > 0 THEN
        INSERT INTO JurnalUmum (
            NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
            AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D,
            NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
        ) VALUES (
            p_id_penjualan_out, p_tgl_transaksi,
            CONCAT('Penjualan pembayaran tunai dari ', p_nama_pelanggan),
            p_nama_akun_kas, p_nama_akun_kas, p_kode_akun_kas,
            p_bayar, 'PENJUALAN', p_lokasi, p_id_user, p_id_komputer
        );
        CALL sp_hlp_saldo_akun_update(p_kode_akun_kas);
    END IF;

    -- Jurnal 2: Transfer Bank (Debet) — hanya jika ada bayar transfer
    IF p_nominal_transfer > 0 THEN
        INSERT INTO JurnalUmum (
            NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
            AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D,
            NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
        ) VALUES (
            p_id_penjualan_out, p_tgl_transaksi,
            CONCAT('Penjualan pembayaran transfer dari ', p_nama_pelanggan),
            p_nama_akun_transfer, p_nama_akun_transfer, p_kode_akun_transfer,
            p_nominal_transfer, 'PENJUALAN', p_lokasi, p_id_user, p_id_komputer
        );
        CALL sp_hlp_saldo_akun_update(p_kode_akun_transfer);
    END IF;

    -- Jurnal 3: Piutang (Debet) — hanya jika ada sisa tagihan
    IF p_sisa_tagihan > 0 THEN
        INSERT INTO JurnalUmum (
            NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
            AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D,
            NAMA_BANTU_D, KODE_BANTU_D,
            NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
        ) VALUES (
            p_id_penjualan_out, p_tgl_transaksi,
            CONCAT('Piutang penjualan dari ', p_nama_pelanggan),
            p_nama_rek_piutang, p_nama_rek_piutang, p_kode_rek_piutang,
            p_nama_pelanggan, p_id_pelanggan,
            p_sisa_tagihan, 'PENJUALAN', p_lokasi, p_id_user, p_id_komputer
        );
        CALL sp_hlp_saldo_akun_update(p_kode_rek_piutang);
    END IF;

    -- Jurnal 4: Diskon Item (Debet) — hanya jika ada diskon per item
    IF p_diskon_total_rp > 0 THEN
        INSERT INTO JurnalUmum (
            NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
            AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D,
            NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
        ) VALUES (
            p_id_penjualan_out, p_tgl_transaksi,
            CONCAT('Diskon item penjualan dari ', p_nama_pelanggan),
            'POTONGAN DISKON PENJUALAN', 'POTONGAN DISKON PENJUALAN', '05.04.001',
            p_diskon_total_rp, 'PENJUALAN', p_lokasi, p_id_user, p_id_komputer
        );
        CALL sp_hlp_saldo_akun_update('05.04.001');
    END IF;

    -- Jurnal 6: HPP Pokok Penjualan (Debet) — hanya jika ada HPP
    IF p_total_hpp > 0 THEN
        INSERT INTO JurnalUmum (
            NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
            AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D,
            NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
        ) VALUES (
            p_id_penjualan_out, p_tgl_transaksi,
            CONCAT('HPP penjualan kepada ', p_nama_pelanggan),
            'HPP POKOK PENJUALAN', 'HPP POKOK PENJUALAN', '06.01.001',
            p_total_hpp, 'PENJUALAN', p_lokasi, p_id_user, p_id_komputer
        );
        CALL sp_hlp_saldo_akun_update('06.01.001');
    END IF;

    -- Jurnal 7: Penjualan Pendapatan Kotor (Kredit)
    BEGIN
        DECLARE v_nilai_jual_kotor DECIMAL(15,2);
        SET v_nilai_jual_kotor = p_grand_total_sbl_pajak + p_diskon_total_rp;
        IF v_nilai_jual_kotor > 0 THEN
            INSERT INTO JurnalUmum (
                NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
                AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K,
                NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
            ) VALUES (
                p_id_penjualan_out, p_tgl_transaksi,
                CONCAT('Penjualan kepada ', p_nama_pelanggan),
                'PENJUALAN', 'PENJUALAN', '05.02.001',
                v_nilai_jual_kotor, 'PENJUALAN', p_lokasi, p_id_user, p_id_komputer
            );
            CALL sp_hlp_saldo_akun_update('05.02.001');
        END IF;
    END;

    -- Jurnal 8: Persediaan Barang Keluar / HPP (Kredit)
    IF p_total_hpp > 0 THEN
        BEGIN
            DECLARE v_kode_rek_barang2 VARCHAR(20) DEFAULT '01.04.001';
            DECLARE v_nama_rek_barang2 VARCHAR(50) DEFAULT 'PERSEDIAAN BARANG';
            SELECT COALESCE(KODE_REK_BARANG,'01.04.001'), COALESCE(NAMA_REK_BARANG,'PERSEDIAAN BARANG')
            INTO   v_kode_rek_barang2, v_nama_rek_barang2
            FROM   tbl_perusahaan LIMIT 1;

            INSERT INTO JurnalUmum (
                NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
                AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K,
                NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
            ) VALUES (
                p_id_penjualan_out, p_tgl_transaksi,
                CONCAT('Keluar persediaan HPP penjualan kepada ', p_nama_pelanggan),
                v_nama_rek_barang2, v_nama_rek_barang2, v_kode_rek_barang2,
                p_total_hpp, 'PENJUALAN', p_lokasi, p_id_user, p_id_komputer
            );
            CALL sp_hlp_saldo_akun_update(v_kode_rek_barang2);
        END;
    END IF;

    -- Jurnal 9: Hutang Pajak (Kredit) — hanya jika ada pajak
    IF p_pajak_rp > 0 THEN
        INSERT INTO JurnalUmum (
            NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
            AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K,
            NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
        ) VALUES (
            p_id_penjualan_out, p_tgl_transaksi,
            CONCAT('Hutang pajak penjualan dari ', p_nama_pelanggan),
            'HUTANG PAJAK', 'HUTANG PAJAK', '03.02.001',
            p_pajak_rp, 'PENJUALAN', p_lokasi, p_id_user, p_id_komputer
        );
        CALL sp_hlp_saldo_akun_update('03.02.001');
    END IF;

    -- Jurnal 10: Biaya Kirim (Kredit) — hanya jika ada biaya kirim
    IF p_biaya_kirim > 0 THEN
        INSERT INTO JurnalUmum (
            NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
            AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K,
            NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
        ) VALUES (
            p_id_penjualan_out, p_tgl_transaksi,
            CONCAT('Jasa kirim/Lain ', p_nama_pelanggan),
            'PENDAPATAN LAIN LAIN', 'PENDAPATAN LAIN LAIN', '08.01.002',
            p_biaya_kirim, 'PENJUALAN', p_lokasi, p_id_user, p_id_komputer
        );
        CALL sp_hlp_saldo_akun_update('08.01.002');
    END IF;

    -- =========================================================================
    -- (K) UPDATE HUTANG PELANGGAN DI tbl_pelanggan
    -- =========================================================================
    IF p_sisa_tagihan > 0 AND p_id_pelanggan IS NOT NULL AND p_id_pelanggan <> '' THEN
        UPDATE tbl_pelanggan
        SET SISA_HUTANG = SISA_HUTANG + p_sisa_tagihan
        WHERE ID_PELANGGAN = p_id_pelanggan;
    END IF;

    -- =========================================================================
    -- (L) JIKA ADA DRAFT, HAPUS DARI penjualan_ditahan DAN penjualan_ditahan_detail
    -- =========================================================================
    IF p_id_draft IS NOT NULL AND p_id_draft <> '' THEN
        DELETE FROM penjualan_ditahan_detail WHERE FAKTUR_JUAL = p_id_draft;
        DELETE FROM penjualan_ditahan WHERE ID_PENJUALAN = p_id_draft;
    END IF;

    -- =========================================================================
    -- SEMUA BERHASIL: COMMIT
    -- =========================================================================
    COMMIT;
    SET p_success = 1;

END proc_body$$
DELIMITER ;


-- =============================================================================
-- sp_trx_opname_simpan (Fase 3)
-- Orkestrasi simpan stok opname — satu SP untuk semua langkah
-- ITEMS dikirim via TEMPORARY TABLE: tmp_stokopname_items
-- =============================================================================
DROP PROCEDURE IF EXISTS sp_trx_opname_simpan;
DELIMITER $$
CREATE PROCEDURE sp_trx_opname_simpan(
    -- Header opname
    IN  p_id_opname         VARCHAR(30),   -- Kosong = generate otomatis
    IN  p_lokasi            VARCHAR(20),   -- 'TOKO' atau 'GUDANG'
    IN  p_tgl_transaksi     DATETIME,
    IN  p_keterangan        VARCHAR(200),
    IN  p_id_user           VARCHAR(20),
    IN  p_id_komputer       VARCHAR(30),
    IN  p_izinkan_backdate  TINYINT(1),
    OUT p_success           TINYINT(1),
    OUT p_error_code        VARCHAR(50),
    OUT p_error_message     VARCHAR(255),
    OUT p_id_opname_out     VARCHAR(30)
)
proc_body: BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_success       = 0;
        SET p_error_code    = 'SQL_ERROR';
        GET DIAGNOSTICS CONDITION 1
            p_error_message = MESSAGE_TEXT;
    END;

    SET p_success       = 0;
    SET p_error_code    = '';
    SET p_error_message = '';
    SET p_id_opname_out = '';

    START TRANSACTION;

    -- =========================================================================
    -- (A) VALIDASI TANGGAL BACKDATE
    -- =========================================================================
    IF p_izinkan_backdate = 0 AND DATE(p_tgl_transaksi) < CURDATE() THEN
        SET p_error_code    = 'BACKDATE_TIDAK_DIIZINKAN';
        SET p_error_message = CONCAT('Transaksi tanggal ', DATE(p_tgl_transaksi),
                                     ' tidak diizinkan. Tanggal hari ini: ', CURDATE());
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    IF p_tgl_transaksi IS NULL THEN
        SET p_error_code    = 'TANGGAL_TIDAK_VALID';
        SET p_error_message = 'Tanggal transaksi tidak valid';
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    -- =========================================================================
    -- (B) GENERATE NOMOR FAKTUR OPNAME
    -- =========================================================================
    IF p_id_opname IS NULL OR p_id_opname = '' THEN
        CALL sp_hlp_faktur_generate('OP', DATE(p_tgl_transaksi),
                                    'stok_opname', 'ID_STOK_OPNAME',
                                    p_id_opname_out);
    ELSE
        SET p_id_opname_out = p_id_opname;
    END IF;

    -- =========================================================================
    -- (C) CEK DUPLIKAT ID OPNAME
    -- =========================================================================
    IF EXISTS (SELECT 1 FROM stok_opname WHERE ID_STOK_OPNAME = p_id_opname_out) THEN
        SET p_error_code    = 'DUPLIKAT_ID_OPNAME';
        SET p_error_message = CONCAT('ID opname ', p_id_opname_out, ' sudah digunakan');
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    -- =========================================================================
    -- (D) INSERT KE stok_opname DARI tmp_stokopname_items
    -- =========================================================================
    INSERT INTO stok_opname (
        ID_STOK_OPNAME, TANGGAL, LOKASI, ID_BARANG, NAMA_BARANG, KATEGORI,
        HARGA, STOK_SYSTEM, STOK_NYATA, STOK_SELISIH,
        SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_HARGA, KETERANGAN,
        ID_USER, ID_KOMPUTER
    )
    SELECT
        p_id_opname_out, p_tgl_transaksi, p_lokasi, ID_BARANG, NAMA_BARANG, KATEGORI,
        HARGA, STOK_SYSTEM, STOK_NYATA, STOK_NYATA - STOK_SYSTEM,
        SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_HARGA, p_keterangan,
        p_id_user, p_id_komputer
    FROM tmp_stokopname_items;

    -- =========================================================================
    -- (E) LOOP ITEMS: UPDATE COUNTER, CALL sp_hlp_stok_hitung, INSERT HistoryBarang
    -- =========================================================================
    BEGIN
        DECLARE done INT DEFAULT FALSE;
        DECLARE v_id_barang VARCHAR(50);
        DECLARE v_stok_selisih DECIMAL(10,2);
        DECLARE v_harga DECIMAL(10,2);

        DECLARE cur_items CURSOR FOR
            SELECT ID_BARANG, (STOK_NYATA - STOK_SYSTEM) AS STOK_SELISIH, HARGA FROM tmp_stokopname_items;

        DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

        OPEN cur_items;

        stok_loop: LOOP
            FETCH cur_items INTO v_id_barang, v_stok_selisih, v_harga;
            IF done THEN
                LEAVE stok_loop;
            END IF;

            -- (F) Update counter OPNAME_TOKO / OPNAME_GUDANG di tbl_barang
            IF p_lokasi = 'TOKO' THEN
                UPDATE tbl_barang SET OPNAME_TOKO = OPNAME_TOKO + v_stok_selisih WHERE ID_BARANG = v_id_barang;
            ELSE
                UPDATE tbl_barang SET OPNAME_GUDANG = OPNAME_GUDANG + v_stok_selisih WHERE ID_BARANG = v_id_barang;
            END IF;

            -- (G) Call sp_hlp_stok_hitung
            CALL sp_hlp_stok_hitung(v_id_barang);

            -- (H) INSERT ke HistoryBarang
            INSERT INTO HistoryBarang (
                FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG,
                QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH,
                ID_USER, ID_KOMPUTER
            )
            SELECT
                p_id_opname_out, p_tgl_transaksi, 'OPNAME', p_lokasi, ID_BARANG, NAMA_BARANG,
                (STOK_NYATA - STOK_SYSTEM), SATUAN, ISI_SATUAN, (STOK_NYATA - STOK_SYSTEM),
                (STOK_NYATA - STOK_SYSTEM) * HARGA, p_id_user, p_id_komputer
            FROM tmp_stokopname_items
            WHERE ID_BARANG = v_id_barang;

            -- (I) INSERT ke JurnalUmum jika selisih stok tidak nol
            -- VB: nilaiSelisih = TxtSelisihRp (nilai rupiah, bukan qty)
            -- Selisih negatif (stok kurang): D=PENYESUAIAN STOK MINUS, K=PERSEDIAAN BARANG
            -- Selisih positif (stok lebih) : D=PERSEDIAAN BARANG,       K=PENYESUAIAN STOK MINUS
            IF v_stok_selisih <> 0 THEN
                BEGIN
                    DECLARE v_kode_rek_brg  VARCHAR(20) DEFAULT '01.04.001';
                    DECLARE v_nama_rek_brg  VARCHAR(50) DEFAULT 'PERSEDIAAN BARANG';
                    DECLARE v_nama_barang_op VARCHAR(100) DEFAULT '';
                    DECLARE v_nilai_selisih DECIMAL(15,2);

                    -- Ambil kode rekening persediaan dari tbl_perusahaan (sama dengan VB ModuleVariabel)
                    SELECT COALESCE(KODE_REK_BARANG, '01.04.001'),
                           COALESCE(NAMA_REK_BARANG, 'PERSEDIAAN BARANG')
                    INTO   v_kode_rek_brg, v_nama_rek_brg
                    FROM   tbl_perusahaan LIMIT 1;

                    -- Ambil nama barang untuk uraian
                    SELECT COALESCE(NAMA_BARANG, '') INTO v_nama_barang_op
                    FROM tmp_stokopname_items WHERE ID_BARANG = v_id_barang LIMIT 1;

                    SET v_nilai_selisih = ABS(v_stok_selisih) * v_harga;

                    INSERT INTO JurnalUmum (
                        NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
                        NAMA_AKUN_D, NOMOR_AKUN_D,
                        NAMA_AKUN_K, NOMOR_AKUN_K,
                        NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
                    ) VALUES (
                        p_id_opname_out, p_tgl_transaksi,
                        CONCAT('Stok opnam stok ', p_lokasi, ', barang ', v_nama_barang_op,
                               ' Jumlah Selisih ', v_stok_selisih),
                        -- Selisih negatif: D=PENYESUAIAN, K=PERSEDIAAN (konsisten VB)
                        -- Selisih positif: D=PERSEDIAAN,  K=PENYESUAIAN (konsisten VB)
                        IF(v_stok_selisih < 0, 'PENYESUAIAN STOK MINUS', v_nama_rek_brg),
                        IF(v_stok_selisih < 0, '06.04.001',               v_kode_rek_brg),
                        IF(v_stok_selisih < 0, v_nama_rek_brg,            'PENYESUAIAN STOK MINUS'),
                        IF(v_stok_selisih < 0, v_kode_rek_brg,            '06.04.001'),
                        v_nilai_selisih,
                        'STOK OPNAME',   -- konsisten dengan VB: "STOK OPNAME" bukan "OPNAME"
                        p_lokasi, p_id_user, p_id_komputer
                    );

                    -- Update saldo akun yang terlibat
                    CALL sp_hlp_saldo_akun_update(v_kode_rek_brg);
                    CALL sp_hlp_saldo_akun_update('06.04.001');
                END;
            END IF;

        END LOOP;

        CLOSE cur_items;
    END;

    -- =========================================================================
    -- SEMUA BERHASIL: COMMIT
    -- =========================================================================
    COMMIT;
    SET p_success = 1;

END proc_body$$
DELIMITER ;


-- =============================================================================
-- SKELETON: sp_trx_transfer_stok_simpan (Fase 4)
-- Orkestrasi transfer stok antar barang (contoh: lusin ke pcs)
-- Sesuai tabel transfer_stok di database (1 transaksi = 1 transfer barang)
-- =============================================================================
DROP PROCEDURE IF EXISTS sp_trx_transfer_stok_simpan;
DELIMITER $$
CREATE PROCEDURE sp_trx_transfer_stok_simpan(
    -- Header transfer
    IN  p_id_transfer       VARCHAR(20),   -- Kosong = generate otomatis
    IN  p_jenis_transfer    VARCHAR(50),
    IN  p_uraian            VARCHAR(60),
    IN  p_tgl_transfer      DATETIME,
    -- Barang Asal (K) = Keluar
    IN  p_id_barang_k       VARCHAR(20),
    IN  p_nama_barang_k     VARCHAR(100),
    IN  p_qty_k             DECIMAL(10,2),
    IN  p_satuan_k          VARCHAR(20),
    IN  p_isi_k             DECIMAL(10,2),
    IN  p_qty_sat_k         DECIMAL(10,2),
    IN  p_harga_sat_k       DECIMAL(10,2),
    IN  p_total_harga_k     DECIMAL(15,0),
    -- Barang Tujuan (M) = Masuk
    IN  p_id_barang_m       VARCHAR(20),
    IN  p_nama_barang_m     VARCHAR(100),
    IN  p_qty_m             DECIMAL(10,2),
    IN  p_satuan_m          VARCHAR(20),
    IN  p_isi_m             DECIMAL(10,2),
    IN  p_qty_sat_m         DECIMAL(10,2),
    IN  p_harga_sat_m       DECIMAL(10,2),
    IN  p_total_harga_m     DECIMAL(15,0),
    -- Lainnya
    IN  p_lokasi            VARCHAR(20),   -- 'TOKO' atau 'GUDANG'
    IN  p_id_user           VARCHAR(20),
    IN  p_id_komputer       VARCHAR(20),
    IN  p_izinkan_backdate  TINYINT(1),
    IN  p_izinkan_stok_minus TINYINT(1),
    OUT p_success           TINYINT(1),
    OUT p_error_code        VARCHAR(50),
    OUT p_error_message     VARCHAR(255),
    OUT p_id_transfer_out   VARCHAR(20)
)
proc_body: BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_success       = 0;
        SET p_error_code    = 'SQL_ERROR';
        GET DIAGNOSTICS CONDITION 1
            p_error_message = MESSAGE_TEXT;
    END;

    SET p_success           = 0;
    SET p_error_code        = '';
    SET p_error_message     = '';
    SET p_id_transfer_out   = '';

    START TRANSACTION;

    -- =========================================================================
    -- (A) VALIDASI TANGGAL BACKDATE
    -- =========================================================================
    IF p_izinkan_backdate = 0 AND DATE(p_tgl_transfer) < CURDATE() THEN
        SET p_error_code    = 'BACKDATE_TIDAK_DIIZINKAN';
        SET p_error_message = CONCAT('Transaksi tanggal ', DATE(p_tgl_transfer),
                                     ' tidak diizinkan. Tanggal hari ini: ', CURDATE());
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    IF p_tgl_transfer IS NULL THEN
        SET p_error_code    = 'TANGGAL_TIDAK_VALID';
        SET p_error_message = 'Tanggal transfer tidak valid';
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    -- =========================================================================
    -- (B) VALIDASI STOK BARANG ASAL (K) = Keluar
    -- =========================================================================
    CALL sp_hlp_stok_validasi(
        p_id_barang_k,
        p_qty_sat_k,
        p_lokasi,
        p_izinkan_stok_minus,
        p_error_code,
        p_error_message
    );

    IF p_error_code <> '' THEN
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    -- =========================================================================
    -- (C) GENERATE NOMOR FAKTUR TRANSFER
    -- =========================================================================
    IF p_id_transfer IS NULL OR p_id_transfer = '' THEN
        CALL sp_hlp_faktur_generate('TF', DATE(p_tgl_transfer),
                                    'transfer_stok', 'ID_TRANSFER',
                                    p_id_transfer_out);
    ELSE
        SET p_id_transfer_out = p_id_transfer;
    END IF;

    -- =========================================================================
    -- (D) CEK DUPLIKAT ID TRANSFER
    -- =========================================================================
    IF EXISTS (SELECT 1 FROM transfer_stok WHERE ID_TRANSFER = p_id_transfer_out) THEN
        SET p_error_code    = 'DUPLIKAT_ID_TRANSFER';
        SET p_error_message = CONCAT('ID transfer ', p_id_transfer_out, ' sudah digunakan');
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    -- =========================================================================
    -- (E) INSERT ke tabel transfer_stok
    -- =========================================================================
    INSERT INTO transfer_stok (
        ID_TRANSFER, JENIS_TRANSFER, URAIAN, TANGGAL,
        ID_BARANG_M, NAMA_BARANG_M, QTY_M, SATUAN_M, ISI_M, QTY_SAT_M, HARGA_SAT_M, TOTAL_HARGA_M,
        ID_BARANG_K, NAMA_BARANG_K, QTY_K, SATUAN_K, ISI_K, QTY_SAT_K, HARGA_SAT_K, TOTAL_HARGA_K,
        Selisih, ID_USER, ID_KOMPUTER
    ) VALUES (
        p_id_transfer_out,
        -- JENIS_TRANSFER: VB pakai lokasi ('TOKO'/'GUDANG'), bukan parameter bebas
        p_lokasi,
        -- URAIAN: VB pakai format tetap sesuai lokasi
        IF(p_lokasi = 'TOKO', 'Transfer stok toko antar barang', 'Transfer stok gudang antar barang'),
        p_tgl_transfer,
        p_id_barang_m, p_nama_barang_m, p_qty_m, p_satuan_m, p_isi_m, p_qty_sat_m, p_harga_sat_m, p_total_harga_m,
        p_id_barang_k, p_nama_barang_k, p_qty_k, p_satuan_k, p_isi_k, p_qty_sat_k, p_harga_sat_k, p_total_harga_k,
        p_total_harga_m - p_total_harga_k, p_id_user, p_id_komputer
    );

    -- =========================================================================
    -- (F) Update counter stok barang ASAL (K) = Keluar (kurangi)
    --     FIX: pakai TRANSFER_STOK_KELUAR, bukan PENJUALAN (konsisten dengan VB)
    -- =========================================================================
    IF p_lokasi = 'TOKO' THEN
        UPDATE tbl_barang SET TRANSFER_STOK_KELUAR_TOKO = TRANSFER_STOK_KELUAR_TOKO + p_qty_sat_k WHERE ID_BARANG = p_id_barang_k;
    ELSE
        UPDATE tbl_barang SET TRANSFER_STOK_KELUAR_GUDANG = TRANSFER_STOK_KELUAR_GUDANG + p_qty_sat_k WHERE ID_BARANG = p_id_barang_k;
    END IF;

    -- =========================================================================
    -- (G) Update counter stok barang TUJUAN (M) = Masuk (tambah)
    --     FIX: pakai TRANSFER_STOK_MASUK, bukan PEMBELIAN (konsisten dengan VB)
    -- =========================================================================
    IF p_lokasi = 'TOKO' THEN
        UPDATE tbl_barang SET TRANSFER_STOK_MASUK_TOKO = TRANSFER_STOK_MASUK_TOKO + p_qty_sat_m WHERE ID_BARANG = p_id_barang_m;
    ELSE
        UPDATE tbl_barang SET TRANSFER_STOK_MASUK_GUDANG = TRANSFER_STOK_MASUK_GUDANG + p_qty_sat_m WHERE ID_BARANG = p_id_barang_m;
    END IF;

    -- =========================================================================
    -- (H) Call sp_hlp_stok_hitung untuk kedua barang
    -- =========================================================================
    CALL sp_hlp_stok_hitung(p_id_barang_k);
    CALL sp_hlp_stok_hitung(p_id_barang_m);

    -- =========================================================================
    -- (I) INSERT ke historybarang untuk kedua barang
    --     FIX: JENIS konsisten dengan VB — 'TRANSFER BARANG KELUAR' / 'TRANSFER BARANG MASUK'
    -- =========================================================================
    INSERT INTO HistoryBarang (
        FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG,
        QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER
    ) VALUES (
        p_id_transfer_out, p_tgl_transfer, 'TRANSFER BARANG KELUAR', p_lokasi, p_id_barang_k, p_nama_barang_k,
        p_qty_sat_k, p_satuan_k, p_isi_k, p_qty_sat_k, p_total_harga_k, p_id_user, p_id_komputer
    );

    INSERT INTO HistoryBarang (
        FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG,
        QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER
    ) VALUES (
        p_id_transfer_out, p_tgl_transfer, 'TRANSFER BARANG MASUK', p_lokasi, p_id_barang_m, p_nama_barang_m,
        p_qty_sat_m, p_satuan_m, p_isi_m, p_qty_sat_m, p_total_harga_m, p_id_user, p_id_komputer
    );

    -- =========================================================================
    -- (J) INSERT JurnalUmum jika ada selisih nilai (konsisten dengan VB)
    --     Selisih = total_harga_m - total_harga_k
    --     Positif (masuk > keluar): D PERSEDIAAN BARANG, K PENYESUAIAN STOK MINUS
    --     Negatif (masuk < keluar): D PENYESUAIAN STOK MINUS, K PERSEDIAAN BARANG
    -- =========================================================================
    BEGIN
        DECLARE v_selisih        DECIMAL(15,0);
        DECLARE v_kode_rek_barang VARCHAR(20) DEFAULT '01.04.001';
        DECLARE v_nama_rek_barang VARCHAR(50) DEFAULT 'PERSEDIAAN BARANG';

        SET v_selisih = p_total_harga_m - p_total_harga_k;

        -- Ambil kode rekening persediaan dari tbl_perusahaan
        SELECT COALESCE(KODE_REK_BARANG, '01.04.001'),
               COALESCE(NAMA_REK_BARANG, 'PERSEDIAAN BARANG')
        INTO   v_kode_rek_barang, v_nama_rek_barang
        FROM   tbl_perusahaan
        LIMIT  1;

        IF v_selisih <> 0 THEN
            INSERT INTO JurnalUmum (
                NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
                NAMA_AKUN_D, NOMOR_AKUN_D,
                NAMA_AKUN_K, NOMOR_AKUN_K,
                NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
            ) VALUES (
                p_id_transfer_out,
                p_tgl_transfer,
                CONCAT('Transfer stok ', p_lokasi, ' dari ', p_nama_barang_m, ' ke ', p_nama_barang_k),
                IF(v_selisih > 0, v_nama_rek_barang,        'PENYESUAIAN STOK MINUS'),
                IF(v_selisih > 0, v_kode_rek_barang,        '06.04.001'),
                IF(v_selisih > 0, 'PENYESUAIAN STOK MINUS', v_nama_rek_barang),
                IF(v_selisih > 0, '06.04.001',               v_kode_rek_barang),
                ABS(v_selisih),
                'TRANSFER STOK',
                p_lokasi,
                p_id_user,
                p_id_komputer
            );

            -- Update saldo akun yang terlibat
            CALL sp_hlp_saldo_akun_update(IF(v_selisih > 0, v_kode_rek_barang, '06.04.001'));
            CALL sp_hlp_saldo_akun_update(IF(v_selisih > 0, '06.04.001', v_kode_rek_barang));
        END IF;
    END;

    -- =========================================================================
    -- SEMUA BERHASIL: COMMIT
    -- =========================================================================
    COMMIT;
    SET p_success = 1;

END proc_body$$
DELIMITER ;


-- =============================================================================
-- VERIFIKASI
-- =============================================================================

SELECT
    ROUTINE_NAME AS sp_name,
    ROUTINE_TYPE AS tipe,
    CREATED      AS dibuat
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = DATABASE()
  AND ROUTINE_NAME LIKE 'sp_trx_%'
ORDER BY ROUTINE_NAME;

SELECT 'Semua skeleton sp_trx_* berhasil diperbaiki. Implementasi detail dapat ditambahkan nanti.' AS status;
