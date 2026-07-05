DROP PROCEDURE IF EXISTS `sp_trx_opname_simpan`;
DELIMITER //
sp_trx_opname_simpan

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_trx_opname_simpan`(
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

    IF p_id_opname IS NULL OR p_id_opname = '' THEN
        CALL sp_hlp_faktur_generate('OP', DATE(p_tgl_transaksi),
                                    'stok_opname', 'ID_STOK_OPNAME',
                                    p_id_opname_out);
    ELSE
        SET p_id_opname_out = p_id_opname;
    END IF;

    IF EXISTS (SELECT 1 FROM stok_opname WHERE ID_STOK_OPNAME = p_id_opname_out) THEN
        SET p_error_code    = 'DUPLIKAT_ID_OPNAME';
        SET p_error_message = CONCAT('ID opname ', p_id_opname_out, ' sudah digunakan');
        ROLLBACK;
        LEAVE proc_body;
    END IF;

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

            IF p_lokasi = 'TOKO' THEN
                UPDATE tbl_barang SET OPNAME_TOKO = OPNAME_TOKO + v_stok_selisih WHERE ID_BARANG = v_id_barang;
            ELSE
                UPDATE tbl_barang SET OPNAME_GUDANG = OPNAME_GUDANG + v_stok_selisih WHERE ID_BARANG = v_id_barang;
            END IF;

            CALL sp_hlp_stok_hitung(v_id_barang);

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

            IF v_stok_selisih <> 0 THEN
                BEGIN
                    DECLARE v_kode_rek_brg  VARCHAR(20) DEFAULT '01.04.001';
                    DECLARE v_nama_rek_brg  VARCHAR(50) DEFAULT 'PERSEDIAAN BARANG';
                    DECLARE v_nama_barang_op VARCHAR(100) DEFAULT '';
                    DECLARE v_nilai_selisih DECIMAL(15,2);

                    SELECT COALESCE(KODE_REK_BARANG, '01.04.001'),
                           COALESCE(NAMA_REK_BARANG, 'PERSEDIAAN BARANG')
                    INTO   v_kode_rek_brg, v_nama_rek_brg
                    FROM   tbl_perusahaan LIMIT 1;

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
                        IF(v_stok_selisih < 0, 'PENYESUAIAN STOK MINUS', v_nama_rek_brg),
                        IF(v_stok_selisih < 0, '06.04.001',               v_kode_rek_brg),
                        IF(v_stok_selisih < 0, v_nama_rek_brg,            'PENYESUAIAN STOK MINUS'),
                        IF(v_stok_selisih < 0, v_kode_rek_brg,            '06.04.001'),
                        v_nilai_selisih,
                        'STOK OPNAME',   -- konsisten dengan VB: "STOK OPNAME" bukan "OPNAME"
                        p_lokasi, p_id_user, p_id_komputer
                    );

                    CALL sp_hlp_saldo_akun_update(v_kode_rek_brg);
                    CALL sp_hlp_saldo_akun_update('06.04.001');
                END;
            END IF;

        END LOOP;

        CLOSE cur_items;
    END;

    COMMIT;
    SET p_success = 1;

END proc_body
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
