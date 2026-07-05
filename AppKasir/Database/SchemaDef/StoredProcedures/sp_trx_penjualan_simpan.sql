DROP PROCEDURE IF EXISTS `sp_trx_penjualan_simpan`;
DELIMITER //
sp_trx_penjualan_simpan

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_trx_penjualan_simpan`(
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
    IN  p_bank                  VARCHAR(50),
    IN  p_no_rekening           VARCHAR(30),
    IN  p_nama_rekening         VARCHAR(50),
    IN  p_no_referensi          VARCHAR(100),  -- Kolom DB: NO_REFFERENSI (dua F)
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

    IF p_id_penjualan IS NULL OR p_id_penjualan = '' THEN
        CALL sp_hlp_faktur_generate('PJ', DATE(p_tgl_transaksi),
                                    'penjualan', 'ID_PENJUALAN',
                                    p_id_penjualan_out);
    ELSE
        SET p_id_penjualan_out = p_id_penjualan;
    END IF;

    IF EXISTS (SELECT 1 FROM penjualan WHERE ID_PENJUALAN = p_id_penjualan_out) THEN
        SET p_error_code    = 'DUPLIKAT_FAKTUR';
        SET p_error_message = CONCAT('Nomor faktur ', p_id_penjualan_out, ' sudah digunakan');
        ROLLBACK;
        LEAVE proc_body;
    END IF;


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
        CASE WHEN p_status_transaksi = '' THEN 'Belum Lunas' ELSE p_status_transaksi END,
        'KAS',
        p_kode_akun_kas,
        p_nama_akun_kas,
        CASE WHEN p_kode_akun_transfer = '' THEN '' ELSE p_kode_akun_transfer END, 
        CASE WHEN p_nama_akun_transfer = '' THEN '' ELSE p_nama_akun_transfer END,
        'BANK',
        CASE WHEN p_kode_akun_transfer = '' THEN NULL ELSE p_kode_akun_transfer END,
        CASE WHEN p_nama_akun_transfer = '' THEN NULL ELSE p_nama_akun_transfer END,
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

            IF p_lokasi = 'TOKO' THEN
                UPDATE tbl_barang SET PENJUALAN_TOKO = PENJUALAN_TOKO + v_qty_satuan WHERE ID_BARANG = v_id_barang;
            ELSE
                UPDATE tbl_barang SET PENJUALAN_GUDANG = PENJUALAN_GUDANG + v_qty_satuan WHERE ID_BARANG = v_id_barang;
            END IF;

            CALL sp_hlp_stok_hitung(v_id_barang);
        END LOOP;

        CLOSE cur_items;
    END;

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

    IF p_sisa_tagihan > 0 AND p_id_pelanggan IS NOT NULL AND p_id_pelanggan <> '' THEN
        UPDATE tbl_pelanggan
        SET SISA_HUTANG = SISA_HUTANG + p_sisa_tagihan
        WHERE ID_PELANGGAN = p_id_pelanggan;
    END IF;

    IF p_id_draft IS NOT NULL AND p_id_draft <> '' THEN
        DELETE FROM penjualan_ditahan_detail WHERE FAKTUR_JUAL = p_id_draft;
        DELETE FROM penjualan_ditahan WHERE ID_PENJUALAN = p_id_draft;
    END IF;

    COMMIT;
    SET p_success = 1;

END proc_body
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
