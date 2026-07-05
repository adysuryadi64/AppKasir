DROP PROCEDURE IF EXISTS `sp_trx_transfer_stok_simpan`;
DELIMITER //
sp_trx_transfer_stok_simpan

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_trx_transfer_stok_simpan`(
    IN  p_id_transfer       VARCHAR(20),   -- Kosong = generate otomatis
    IN  p_jenis_transfer    VARCHAR(50),
    IN  p_uraian            VARCHAR(60),
    IN  p_tgl_transfer      DATETIME,
    IN  p_id_barang_k       VARCHAR(20),
    IN  p_nama_barang_k     VARCHAR(100),
    IN  p_qty_k             DECIMAL(10,2),
    IN  p_satuan_k          VARCHAR(20),
    IN  p_isi_k             DECIMAL(10,2),
    IN  p_qty_sat_k         DECIMAL(10,2),
    IN  p_harga_sat_k       DECIMAL(10,2),
    IN  p_total_harga_k     DECIMAL(15,0),
    IN  p_id_barang_m       VARCHAR(20),
    IN  p_nama_barang_m     VARCHAR(100),
    IN  p_qty_m             DECIMAL(10,2),
    IN  p_satuan_m          VARCHAR(20),
    IN  p_isi_m             DECIMAL(10,2),
    IN  p_qty_sat_m         DECIMAL(10,2),
    IN  p_harga_sat_m       DECIMAL(10,2),
    IN  p_total_harga_m     DECIMAL(15,0),
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

    IF p_id_transfer IS NULL OR p_id_transfer = '' THEN
        CALL sp_hlp_faktur_generate('TF', DATE(p_tgl_transfer),
                                    'transfer_stok', 'ID_TRANSFER',
                                    p_id_transfer_out);
    ELSE
        SET p_id_transfer_out = p_id_transfer;
    END IF;

    IF EXISTS (SELECT 1 FROM transfer_stok WHERE ID_TRANSFER = p_id_transfer_out) THEN
        SET p_error_code    = 'DUPLIKAT_ID_TRANSFER';
        SET p_error_message = CONCAT('ID transfer ', p_id_transfer_out, ' sudah digunakan');
        ROLLBACK;
        LEAVE proc_body;
    END IF;

    INSERT INTO transfer_stok (
        ID_TRANSFER, JENIS_TRANSFER, URAIAN, TANGGAL,
        ID_BARANG_M, NAMA_BARANG_M, QTY_M, SATUAN_M, ISI_M, QTY_SAT_M, HARGA_SAT_M, TOTAL_HARGA_M,
        ID_BARANG_K, NAMA_BARANG_K, QTY_K, SATUAN_K, ISI_K, QTY_SAT_K, HARGA_SAT_K, TOTAL_HARGA_K,
        Selisih, ID_USER, ID_KOMPUTER
    ) VALUES (
        p_id_transfer_out,
        p_lokasi,
        IF(p_lokasi = 'TOKO', 'Transfer stok toko antar barang', 'Transfer stok gudang antar barang'),
        p_tgl_transfer,
        p_id_barang_m, p_nama_barang_m, p_qty_m, p_satuan_m, p_isi_m, p_qty_sat_m, p_harga_sat_m, p_total_harga_m,
        p_id_barang_k, p_nama_barang_k, p_qty_k, p_satuan_k, p_isi_k, p_qty_sat_k, p_harga_sat_k, p_total_harga_k,
        p_total_harga_m - p_total_harga_k, p_id_user, p_id_komputer
    );

    IF p_lokasi = 'TOKO' THEN
        UPDATE tbl_barang SET TRANSFER_STOK_KELUAR_TOKO = TRANSFER_STOK_KELUAR_TOKO + p_qty_sat_k WHERE ID_BARANG = p_id_barang_k;
    ELSE
        UPDATE tbl_barang SET TRANSFER_STOK_KELUAR_GUDANG = TRANSFER_STOK_KELUAR_GUDANG + p_qty_sat_k WHERE ID_BARANG = p_id_barang_k;
    END IF;

    IF p_lokasi = 'TOKO' THEN
        UPDATE tbl_barang SET TRANSFER_STOK_MASUK_TOKO = TRANSFER_STOK_MASUK_TOKO + p_qty_sat_m WHERE ID_BARANG = p_id_barang_m;
    ELSE
        UPDATE tbl_barang SET TRANSFER_STOK_MASUK_GUDANG = TRANSFER_STOK_MASUK_GUDANG + p_qty_sat_m WHERE ID_BARANG = p_id_barang_m;
    END IF;

    CALL sp_hlp_stok_hitung(p_id_barang_k);
    CALL sp_hlp_stok_hitung(p_id_barang_m);

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

    BEGIN
        DECLARE v_selisih        DECIMAL(15,0);
        DECLARE v_kode_rek_barang VARCHAR(20) DEFAULT '01.04.001';
        DECLARE v_nama_rek_barang VARCHAR(50) DEFAULT 'PERSEDIAAN BARANG';

        SET v_selisih = p_total_harga_m - p_total_harga_k;

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

            CALL sp_hlp_saldo_akun_update(IF(v_selisih > 0, v_kode_rek_barang, '06.04.001'));
            CALL sp_hlp_saldo_akun_update(IF(v_selisih > 0, '06.04.001', v_kode_rek_barang));
        END IF;
    END;

    COMMIT;
    SET p_success = 1;

END proc_body
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
