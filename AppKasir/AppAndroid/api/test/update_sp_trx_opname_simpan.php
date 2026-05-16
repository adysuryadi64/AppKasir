<?php
require_once __DIR__ . '/../db_connect.php';
require_once __DIR__ . '/../no_browser.php';

try {
    // Drop SP lama
    echo "1. Dropping existing sp_trx_opname_simpan..." . PHP_EOL;
    $conn->exec("DROP PROCEDURE IF EXISTS sp_trx_opname_simpan");

    // Create SP baru
    $sql_sp = "CREATE PROCEDURE sp_trx_opname_simpan(
    IN  p_id_opname         VARCHAR(30),
    IN  p_lokasi            VARCHAR(20),
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
        GET DIAGNOSTICS CONDITION 1 p_error_message = MESSAGE_TEXT;
    END;

    SET p_success           = 0;
    SET p_error_code        = '';
    SET p_error_message     = '';
    SET p_id_opname_out  = '';

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
                INSERT INTO JurnalUmum (
                    NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
                    AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_D,
                    NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
                ) VALUES (
                    p_id_opname_out, p_tgl_transaksi,
                    CONCAT('Penyesuaian stok opname (', IF(v_stok_selisih > 0, 'tambah', 'kurang'), ')'),
                    'PERSEDIAAN BARANG', 'PERSEDIAAN BARANG', '01.03.001',
                    ABS(v_stok_selisih) * v_harga,
                    'OPNAME', p_lokasi, p_id_user, p_id_komputer
                );

                INSERT INTO JurnalUmum (
                    NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN,
                    AKUN_K, NAMA_AKUN_K, NOMOR_AKUN_K,
                    NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER
                ) VALUES (
                    p_id_opname_out, p_tgl_transaksi,
                    CONCAT('Penyesuaian stok opname (', IF(v_stok_selisih > 0, 'tambah', 'kurang'), ')'),
                    'OPNAME', 'OPNAME', '01.03.002',
                    ABS(v_stok_selisih) * v_harga,
                    'OPNAME', p_lokasi, p_id_user, p_id_komputer
                );

                CALL sp_hlp_saldo_akun_update('01.03.001');
                CALL sp_hlp_saldo_akun_update('01.03.002');
            END IF;

        END LOOP;

        CLOSE cur_items;
    END;

    COMMIT;
    SET p_success = 1;

END proc_body";

    echo "2. Creating new sp_trx_opname_simpan..." . PHP_EOL;
    $conn->exec($sql_sp);

    echo "✓ SP sp_trx_opname_simpan berhasil diperbarui!" . PHP_EOL;

} catch (Exception $e) {
    echo "Error: " . $e->getMessage() . PHP_EOL;
    echo "Trace: " . $e->getTraceAsString() . PHP_EOL;
}
?>
