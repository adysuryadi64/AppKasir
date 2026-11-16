-- phpMyAdmin SQL Dump
-- version 4.9.1
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Waktu pembuatan: 02 Okt 2026 pada 11.58
-- Versi server: 8.0.17
-- Versi PHP: 7.3.10

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `db_kasir`
--

DELIMITER $$
--
-- Prosedur
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_bat_bon_semua_karyawan` ()  BEGIN
    UPDATE tbl_karyawan k
    LEFT JOIN (
        SELECT KODE, SUM(NOMINAL) AS TotalBon
        FROM bon_karyawan
        WHERE JENIS = 'BON'
        GROUP BY KODE
    ) b ON b.KODE = k.KODE
    LEFT JOIN (
        SELECT KODE, SUM(NOMINAL) AS TotalBayar
        FROM bon_karyawan
        WHERE JENIS = 'BAYAR'
        GROUP BY KODE
    ) p ON p.KODE = k.KODE
    SET k.TOTALBON    = IFNULL(b.TotalBon, 0),
        k.TOTALBAYAR  = IFNULL(p.TotalBayar, 0),
        k.SALDOAKHIR  = IFNULL(k.SALDOAWAL, 0) + IFNULL(b.TotalBon, 0) - IFNULL(p.TotalBayar, 0);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_bat_hutang_semua_supplier` ()  BEGIN
    UPDATE tbl_supliyer s
    LEFT JOIN (
        SELECT ID_SUPPLIER, SUM(IFNULL(TAGIHAN, 0)) AS HUTANG
        FROM pembelian
        GROUP BY ID_SUPPLIER
    ) x ON x.ID_SUPPLIER = s.KODE
    SET s.HUTANGAKHIR = IFNULL(x.HUTANG, 0) + IFNULL(s.HUTANGAWAL, 0);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_bat_piutang_semua_pelanggan` ()  BEGIN
    UPDATE tbl_pelanggan p
    LEFT JOIN (
        SELECT ID_PELANGGAN, SUM(IFNULL(SISA_TAGIHAN, 0)) AS HUTANG
        FROM penjualan
        GROUP BY ID_PELANGGAN
    ) x ON x.ID_PELANGGAN = p.KODE
    SET p.HUTANGAKHIR = IFNULL(x.HUTANG, 0) + IFNULL(p.HUTANGAWAL, 0);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_bat_saldo_semua_akun` ()  BEGIN
    DECLARE v_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_debet_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_kredit_laba_rugi DECIMAL(18,2) DEFAULT 0;

    UPDATE tbl_datareferensi r
    LEFT JOIN (
        SELECT NOMOR_AKUN_D, SUM(NOMINAL) AS total_debet
        FROM JurnalUmum
        GROUP BY NOMOR_AKUN_D
    ) d ON d.NOMOR_AKUN_D = r.KODE_AKUN
    LEFT JOIN (
        SELECT NOMOR_AKUN_K, SUM(NOMINAL) AS total_kredit
        FROM JurnalUmum
        GROUP BY NOMOR_AKUN_K
    ) k ON k.NOMOR_AKUN_K = r.KODE_AKUN
    SET
        r.S_DEBET = IFNULL(d.total_debet, 0),
        r.S_KREDIT = IFNULL(k.total_kredit, 0),
        r.SALDO_SEBELUMNYA = r.SALDO_AWAL,
        r.SALDO_AKHIR = CASE
            WHEN r.AKUN_DK = 'DEBET'  THEN IFNULL(r.SALDO_AWAL, 0) + IFNULL(d.total_debet, 0) - IFNULL(k.total_kredit, 0)
            WHEN r.AKUN_DK = 'KREDIT' THEN IFNULL(r.SALDO_AWAL, 0) - IFNULL(d.total_debet, 0) + IFNULL(k.total_kredit, 0)
            ELSE 0
        END
    WHERE r.TYPE_AKUN <> 'LABA RUGI';

    SELECT
        SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) -
        SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) -
        SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) +
        SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) AS LABA_RUGI,
        SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_DEBET ELSE 0 END) AS TOTAL_DEBET,
        SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_KREDIT ELSE 0 END) AS TOTAL_KREDIT
    INTO v_laba_rugi, v_total_debet_laba_rugi, v_total_kredit_laba_rugi
    FROM tbl_datareferensi
    WHERE SUB_AKUN IN ('LABA','RUGI');

    UPDATE tbl_datareferensi
    SET
        SALDO_SEBELUMNYA = v_laba_rugi,
        S_DEBET = v_total_debet_laba_rugi,
        S_KREDIT = v_total_kredit_laba_rugi
    WHERE TYPE_AKUN = 'LABA RUGI';

    UPDATE tbl_datareferensi
    SET SALDO_AKHIR = SALDO_SEBELUMNYA + (-(S_DEBET)) + S_KREDIT
    WHERE TYPE_AKUN = 'LABA RUGI';
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_bat_stok_gudang` ()  BEGIN
    UPDATE tbl_barang SET
        STOK_GUDANG = COALESCE(AWAL_GUDANG, 0)
            + COALESCE(TAMBAH_GUDANG, 0)            - COALESCE(KURANG_GUDANG, 0)
            + COALESCE(PEMBELIAN_GUDANG, 0)         - COALESCE(PENJUALAN_GUDANG, 0)
            - COALESCE(RETUR_BELI_GUDANG, 0)        + COALESCE(RETUR_JUAL_GUDANG, 0)
            + COALESCE(OPNAME_GUDANG, 0)
            + COALESCE(TRANSFER_STOK_MASUK_GUDANG, 0)    - COALESCE(TRANSFER_STOK_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_GUDANG, 0);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_bat_stok_semua_barang` ()  BEGIN
    UPDATE tbl_barang SET
        STOK_TOKO = COALESCE(AWAL_TOKO, 0)
            + COALESCE(TAMBAH_TOKO, 0)              - COALESCE(KURANG_TOKO, 0)
            + COALESCE(PEMBELIAN_TOKO, 0)           - COALESCE(PENJUALAN_TOKO, 0)
            - COALESCE(RETUR_BELI_TOKO, 0)          + COALESCE(RETUR_JUAL_TOKO, 0)
            + COALESCE(OPNAME_TOKO, 0)
            + COALESCE(TRANSFER_STOK_MASUK_TOKO, 0)    - COALESCE(TRANSFER_STOK_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_TOKO, 0),
        STOK_GUDANG = COALESCE(AWAL_GUDANG, 0)
            + COALESCE(TAMBAH_GUDANG, 0)            - COALESCE(KURANG_GUDANG, 0)
            + COALESCE(PEMBELIAN_GUDANG, 0)         - COALESCE(PENJUALAN_GUDANG, 0)
            - COALESCE(RETUR_BELI_GUDANG, 0)        + COALESCE(RETUR_JUAL_GUDANG, 0)
            + COALESCE(OPNAME_GUDANG, 0)
            + COALESCE(TRANSFER_STOK_MASUK_GUDANG, 0)    - COALESCE(TRANSFER_STOK_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_GUDANG, 0);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_bat_stok_toko` ()  BEGIN
    UPDATE tbl_barang SET
        STOK_TOKO = COALESCE(AWAL_TOKO, 0)
            + COALESCE(TAMBAH_TOKO, 0)              - COALESCE(KURANG_TOKO, 0)
            + COALESCE(PEMBELIAN_TOKO, 0)           - COALESCE(PENJUALAN_TOKO, 0)
            - COALESCE(RETUR_BELI_TOKO, 0)          + COALESCE(RETUR_JUAL_TOKO, 0)
            + COALESCE(OPNAME_TOKO, 0)
            + COALESCE(TRANSFER_STOK_MASUK_TOKO, 0)    - COALESCE(TRANSFER_STOK_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_TOKO, 0);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_faktur_generate` (IN `p_prefix` VARCHAR(5), IN `p_tanggal` DATE, IN `p_tabel` VARCHAR(50), IN `p_kolom_id` VARCHAR(50), OUT `p_nomor_faktur` VARCHAR(30))  BEGIN

    DECLARE v_tgl_kode   VARCHAR(6);   -- YYMMDD, contoh: '260401'
    DECLARE v_prefix_tgl VARCHAR(12);  -- PREFIX-YYMMDD, contoh: 'PJ-260401'
    DECLARE v_max_val    VARCHAR(30) DEFAULT NULL;
    DECLARE v_urut       INT DEFAULT 0;
    DECLARE v_prefix_len INT;

    SET v_tgl_kode   = DATE_FORMAT(p_tanggal, '%y%m%d');
    SET v_prefix_tgl = CONCAT(p_prefix, '-', v_tgl_kode);
    SET v_prefix_len = LENGTH(v_prefix_tgl);

    SET @sql_max = CONCAT(
        'SELECT MAX(', p_kolom_id, ') INTO @v_max ',
        'FROM ', p_tabel, ' ',
        'WHERE ', p_kolom_id, ' LIKE ? ',
        'FOR UPDATE'
    );
    SET @prefix_like = CONCAT(v_prefix_tgl, '%');

    PREPARE stmt_max FROM @sql_max;
    EXECUTE stmt_max USING @prefix_like;
    DEALLOCATE PREPARE stmt_max;

    SET v_max_val = @v_max;

    IF v_max_val IS NOT NULL AND LEFT(v_max_val, v_prefix_len) = v_prefix_tgl THEN
        SET v_urut = CAST(RIGHT(v_max_val, 4) AS UNSIGNED) + 1;
    ELSE
        SET v_urut = 1;
    END IF;

    SET p_nomor_faktur = CONCAT(v_prefix_tgl, LPAD(v_urut, 4, '0'));
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_saldo_akun_delta` (IN `p_kode_akun` VARCHAR(20), IN `p_delta_debet` DECIMAL(20,0), IN `p_delta_kredit` DECIMAL(20,0))  BEGIN
    DECLARE v_laba_rugi             DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_debet_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_kredit_laba_rugi DECIMAL(18,2) DEFAULT 0;

    IF p_kode_akun IS NOT NULL AND p_kode_akun <> '' THEN

        UPDATE tbl_datareferensi
        SET
            S_DEBET   = S_DEBET   + p_delta_debet,
            S_KREDIT  = S_KREDIT  + p_delta_kredit,
            SALDO_AKHIR = CASE
                WHEN AKUN_DK = 'DEBET'  THEN SALDO_AKHIR + p_delta_debet  - p_delta_kredit
                WHEN AKUN_DK = 'KREDIT' THEN SALDO_AKHIR - p_delta_debet  + p_delta_kredit
                ELSE SALDO_AKHIR
            END
        WHERE KODE_AKUN = p_kode_akun;

        SELECT
            SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) -
            SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) -
            SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) +
            SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) AS LABA_RUGI,
            SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_DEBET  ELSE 0 END) AS TOTAL_DEBET,
            SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_KREDIT ELSE 0 END) AS TOTAL_KREDIT
        INTO v_laba_rugi, v_total_debet_laba_rugi, v_total_kredit_laba_rugi
        FROM tbl_datareferensi
        WHERE SUB_AKUN IN ('LABA','RUGI');

        UPDATE tbl_datareferensi
        SET
            SALDO_SEBELUMNYA = v_laba_rugi,
            S_DEBET          = v_total_debet_laba_rugi,
            S_KREDIT         = v_total_kredit_laba_rugi
        WHERE TYPE_AKUN = 'LABA RUGI';

        UPDATE tbl_datareferensi
        SET SALDO_AKHIR = SALDO_SEBELUMNYA + (-(S_DEBET)) + S_KREDIT
        WHERE TYPE_AKUN = 'LABA RUGI';

    END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_saldo_akun_update` (IN `p_kode_akun` VARCHAR(20))  BEGIN
    DECLARE v_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_debet_laba_rugi DECIMAL(18,2) DEFAULT 0;
    DECLARE v_total_kredit_laba_rugi DECIMAL(18,2) DEFAULT 0;

    IF p_kode_akun IS NOT NULL AND p_kode_akun <> '' THEN
        UPDATE tbl_datareferensi r
        LEFT JOIN (
            SELECT NOMOR_AKUN_D AS kode, SUM(NOMINAL) AS total
            FROM JurnalUmum FORCE INDEX(idx_covering_akun_d)
            WHERE NOMOR_AKUN_D = p_kode_akun
            GROUP BY NOMOR_AKUN_D
        ) d ON d.kode = r.KODE_AKUN
        LEFT JOIN (
            SELECT NOMOR_AKUN_K AS kode, SUM(NOMINAL) AS total
            FROM JurnalUmum FORCE INDEX(idx_covering_akun_k)
            WHERE NOMOR_AKUN_K = p_kode_akun
            GROUP BY NOMOR_AKUN_K
        ) k ON k.kode = r.KODE_AKUN
        SET
            r.S_DEBET   = IFNULL(d.total, 0),
            r.S_KREDIT  = IFNULL(k.total, 0),
            r.SALDO_AKHIR = CASE
                WHEN r.AKUN_DK = 'DEBET'  THEN IFNULL(r.SALDO_AWAL, 0) + IFNULL(d.total, 0) - IFNULL(k.total, 0)
                WHEN r.AKUN_DK = 'KREDIT' THEN IFNULL(r.SALDO_AWAL, 0) - IFNULL(d.total, 0) + IFNULL(k.total, 0)
                ELSE 0
            END
        WHERE r.KODE_AKUN = p_kode_akun;

        SELECT
            SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) -
            SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) -
            SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END) +
            SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END) AS LABA_RUGI,
            SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_DEBET  ELSE 0 END) AS TOTAL_DEBET,
            SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_KREDIT ELSE 0 END) AS TOTAL_KREDIT
        INTO v_laba_rugi, v_total_debet_laba_rugi, v_total_kredit_laba_rugi
        FROM tbl_datareferensi
        WHERE SUB_AKUN IN ('LABA','RUGI');

        UPDATE tbl_datareferensi
        SET SALDO_SEBELUMNYA = v_laba_rugi,
            S_DEBET          = v_total_debet_laba_rugi,
            S_KREDIT         = v_total_kredit_laba_rugi
        WHERE TYPE_AKUN = 'LABA RUGI';

        UPDATE tbl_datareferensi
        SET SALDO_AKHIR = SALDO_SEBELUMNYA + (-(S_DEBET)) + S_KREDIT
        WHERE TYPE_AKUN = 'LABA RUGI';
    END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_saldo_kas_validasi` (IN `p_kode_akun` VARCHAR(20), IN `p_nominal_keluar` DECIMAL(15,2), OUT `p_error_code` VARCHAR(50), OUT `p_error_message` VARCHAR(255))  BEGIN
    DECLARE v_saldo     DECIMAL(20,0) DEFAULT 0;
    DECLARE v_nama      VARCHAR(100)  DEFAULT '';

    SET p_error_code    = '';
    SET p_error_message = '';

    SELECT COALESCE(SALDO_AKHIR, 0), COALESCE(NAMA_AKUN, '')
    INTO v_saldo, v_nama
    FROM tbl_datareferensi
    WHERE KODE_AKUN = p_kode_akun
    FOR UPDATE;

    IF v_saldo < p_nominal_keluar THEN
        SET p_error_code    = 'SALDO_KAS_KURANG';
        SET p_error_message = CONCAT('Saldo akun "', v_nama, '" tidak cukup. ',
                                     'Tersedia: Rp ', FORMAT(v_saldo, 0, 'id_ID'), ', ',
                                     'Dibutuhkan: Rp ', FORMAT(p_nominal_keluar, 0, 'id_ID'));
    END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_stok_ambil` (IN `p_kode_barang` VARCHAR(50), OUT `p_stok_toko` DECIMAL(10,2), OUT `p_stok_gudang` DECIMAL(10,2), OUT `p_nama_barang` VARCHAR(200))  BEGIN
    SET p_stok_toko   = 0;
    SET p_stok_gudang = 0;
    SET p_nama_barang = '';

    SELECT
        COALESCE(STOK_TOKO,   0),
        COALESCE(STOK_GUDANG, 0),
        COALESCE(NAMA_BARANG, '')
    INTO
        p_stok_toko,
        p_stok_gudang,
        p_nama_barang
    FROM tbl_barang
    WHERE ID_BARANG = p_kode_barang
    LIMIT 1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_stok_ambil_edit` (IN `p_kode_barang` VARCHAR(50), IN `p_faktur_lama` VARCHAR(30), IN `p_lokasi` VARCHAR(10), OUT `p_stok_toko` DECIMAL(10,2), OUT `p_stok_gudang` DECIMAL(10,2), OUT `p_nama_barang` VARCHAR(200))  BEGIN
    DECLARE v_stok_toko   DECIMAL(10,2) DEFAULT 0;
    DECLARE v_stok_gudang DECIMAL(10,2) DEFAULT 0;
    DECLARE v_nama        VARCHAR(200)  DEFAULT '';
    DECLARE v_qty_lama    DECIMAL(10,2) DEFAULT 0;

    SET p_stok_toko   = 0;
    SET p_stok_gudang = 0;
    SET p_nama_barang = '';

    SELECT
        COALESCE(STOK_TOKO,   0),
        COALESCE(STOK_GUDANG, 0),
        COALESCE(NAMA_BARANG, '')
    INTO
        v_stok_toko,
        v_stok_gudang,
        v_nama
    FROM tbl_barang
    WHERE ID_BARANG = p_kode_barang
    LIMIT 1;

    SELECT COALESCE(SUM(QTY_SATUAN), 0)
    INTO v_qty_lama
    FROM penjualan_detail
    WHERE FAKTUR_JUAL = p_faktur_lama
      AND ID_BARANG   = p_kode_barang
      AND LOKASIBARANG = p_lokasi;

    SET p_stok_toko   = IF(p_lokasi = 'TOKO',   v_stok_toko   + v_qty_lama, v_stok_toko);
    SET p_stok_gudang = IF(p_lokasi = 'GUDANG',  v_stok_gudang + v_qty_lama, v_stok_gudang);
    SET p_nama_barang = v_nama;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_stok_ambil_edit_retur_beli` (IN `p_kode_barang` VARCHAR(50), IN `p_faktur_lama` VARCHAR(30), IN `p_lokasi` VARCHAR(10), OUT `p_stok_toko` DECIMAL(10,2), OUT `p_stok_gudang` DECIMAL(10,2), OUT `p_nama_barang` VARCHAR(200))  BEGIN
    DECLARE v_stok_toko   DECIMAL(10,2) DEFAULT 0;
    DECLARE v_stok_gudang DECIMAL(10,2) DEFAULT 0;
    DECLARE v_nama        VARCHAR(200)  DEFAULT '';
    DECLARE v_qty_lama    DECIMAL(10,2) DEFAULT 0;
    DECLARE v_lokasi_faktur VARCHAR(20) DEFAULT '';

    SET p_stok_toko   = 0;
    SET p_stok_gudang = 0;
    SET p_nama_barang = '';

    SELECT
        COALESCE(STOK_TOKO,   0),
        COALESCE(STOK_GUDANG, 0),
        COALESCE(NAMA_BARANG, '')
    INTO
        v_stok_toko,
        v_stok_gudang,
        v_nama
    FROM tbl_barang
    WHERE ID_BARANG = p_kode_barang
    LIMIT 1;

    SELECT COALESCE(LOKASI, '')
    INTO v_lokasi_faktur
    FROM retur_pembelian r
    INNER JOIN pembelian p ON r.ID_PEMBELIAN = p.ID_PEMBELIAN
    WHERE r.ID_RETUR_PEMBELIAN = p_faktur_lama
    LIMIT 1;

    SELECT COALESCE(SUM(QTY_SAT), 0)
    INTO v_qty_lama
    FROM retur_pembelian_detail
    WHERE ID_RETUR_PEMBELIAN = p_faktur_lama
      AND ID_BARANG   = p_kode_barang;

    IF v_lokasi_faktur = p_lokasi THEN
        SET p_stok_toko   = IF(p_lokasi = 'TOKO',   v_stok_toko   - v_qty_lama, v_stok_toko);
        SET p_stok_gudang = IF(p_lokasi = 'GUDANG',  v_stok_gudang - v_qty_lama, v_stok_gudang);
    ELSE
        SET p_stok_toko   = v_stok_toko;
        SET p_stok_gudang = v_stok_gudang;
    END IF;
    SET p_nama_barang = v_nama;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_stok_hitung` (IN `p_kode_barang` VARCHAR(50))  BEGIN
    UPDATE tbl_barang SET
        STOK_TOKO = COALESCE(AWAL_TOKO, 0)
            + COALESCE(TAMBAH_TOKO, 0)              - COALESCE(KURANG_TOKO, 0)
            + COALESCE(PEMBELIAN_TOKO, 0)           - COALESCE(PENJUALAN_TOKO, 0)
            - COALESCE(RETUR_BELI_TOKO, 0)          + COALESCE(RETUR_JUAL_TOKO, 0)
            + COALESCE(OPNAME_TOKO, 0)
            + COALESCE(TRANSFER_STOK_MASUK_TOKO, 0)    - COALESCE(TRANSFER_STOK_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_TOKO, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_TOKO, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_TOKO, 0),
        STOK_GUDANG = COALESCE(AWAL_GUDANG, 0)
            + COALESCE(TAMBAH_GUDANG, 0)            - COALESCE(KURANG_GUDANG, 0)
            + COALESCE(PEMBELIAN_GUDANG, 0)         - COALESCE(PENJUALAN_GUDANG, 0)
            - COALESCE(RETUR_BELI_GUDANG, 0)        + COALESCE(RETUR_JUAL_GUDANG, 0)
            + COALESCE(OPNAME_GUDANG, 0)
            + COALESCE(TRANSFER_STOK_MASUK_GUDANG, 0)    - COALESCE(TRANSFER_STOK_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_BARANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_BARANG_KELUAR_GUDANG, 0)
            + COALESCE(TRANSFER_CABANG_MASUK_GUDANG, 0)  - COALESCE(TRANSFER_CABANG_KELUAR_GUDANG, 0)
    WHERE ID_BARANG = p_kode_barang;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_stok_validasi` (IN `p_kode_barang` VARCHAR(50), IN `p_qty_dibutuhkan` DECIMAL(15,4), IN `p_lokasi` VARCHAR(10), IN `p_izinkan_minus` TINYINT(1), OUT `p_error_code` VARCHAR(50), OUT `p_error_message` VARCHAR(255))  BEGIN
    DECLARE v_stok      DECIMAL(15,4) DEFAULT 0;
    DECLARE v_nama      VARCHAR(200)  DEFAULT '';

    SET p_error_code    = '';
    SET p_error_message = '';

    IF p_lokasi = 'GUDANG' THEN
        SELECT COALESCE(STOK_GUDANG, 0), COALESCE(NAMA_BARANG, '')
        INTO v_stok, v_nama
        FROM tbl_barang
        WHERE ID_BARANG = p_kode_barang
        FOR UPDATE;
    ELSE
        SELECT COALESCE(STOK_TOKO, 0), COALESCE(NAMA_BARANG, '')
        INTO v_stok, v_nama
        FROM tbl_barang
        WHERE ID_BARANG = p_kode_barang
        FOR UPDATE;
    END IF;

    IF p_izinkan_minus = 0 AND v_stok < p_qty_dibutuhkan THEN
        SET p_error_code    = 'STOK_KURANG';
        SET p_error_message = CONCAT('Stok barang "', v_nama, '" tidak cukup. ',
                                     'Tersedia: ', TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM CAST(v_stok AS CHAR))),
                                     ', Dibutuhkan: ', TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM CAST(p_qty_dibutuhkan AS CHAR))));
    END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_hlp_stok_validasi_so` (IN `p_kode_barang` VARCHAR(50), IN `p_qty_dibutuhkan` DECIMAL(15,4), IN `p_faktur_so` VARCHAR(30), IN `p_lokasi` VARCHAR(10), IN `p_izinkan_minus` TINYINT(1), OUT `p_error_code` VARCHAR(50), OUT `p_error_message` VARCHAR(255))  BEGIN
    DECLARE v_stok      DECIMAL(15,4) DEFAULT 0;
    DECLARE v_nama      VARCHAR(200)  DEFAULT '';
    DECLARE v_qty_so    DECIMAL(15,4) DEFAULT 0;
    DECLARE v_stok_ef   DECIMAL(15,4) DEFAULT 0;

    SET p_error_code    = '';
    SET p_error_message = '';

    IF p_lokasi = 'GUDANG' THEN
        SELECT COALESCE(STOK_GUDANG, 0), COALESCE(NAMA_BARANG, '')
        INTO v_stok, v_nama
        FROM tbl_barang
        WHERE ID_BARANG = p_kode_barang
        FOR UPDATE;
    ELSE
        SELECT COALESCE(STOK_TOKO, 0), COALESCE(NAMA_BARANG, '')
        INTO v_stok, v_nama
        FROM tbl_barang
        WHERE ID_BARANG = p_kode_barang
        FOR UPDATE;
    END IF;

    SELECT COALESCE(SUM(QTY_SATUAN), 0)
    INTO v_qty_so
    FROM sales_order_detail
    WHERE FAKTUR_JUAL = p_faktur_so
      AND ID_BARANG   = p_kode_barang
      AND LOKASIBARANG = p_lokasi;

    SET v_stok_ef = v_stok + v_qty_so;

    IF p_izinkan_minus = 0 AND v_stok_ef < p_qty_dibutuhkan THEN
        SET p_error_code    = 'STOK_KURANG';
        SET p_error_message = CONCAT('Stok barang "', v_nama, '" tidak cukup. ',
                                     'Tersedia Efektif (Stok + Reservasi SO): ', 
                                     TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM CAST(v_stok_ef AS CHAR))),
                                     ', Dibutuhkan: ', 
                                     TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM CAST(p_qty_dibutuhkan AS CHAR))));
    END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_trx_opname_simpan` (IN `p_id_opname` VARCHAR(30), IN `p_lokasi` VARCHAR(20), IN `p_tgl_transaksi` DATETIME, IN `p_keterangan` VARCHAR(200), IN `p_id_user` VARCHAR(20), IN `p_id_komputer` VARCHAR(30), IN `p_izinkan_backdate` TINYINT(1), OUT `p_success` TINYINT(1), OUT `p_error_code` VARCHAR(50), OUT `p_error_message` VARCHAR(255), OUT `p_id_opname_out` VARCHAR(30))  proc_body: BEGIN
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

END proc_body$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_trx_penjualan_simpan` (IN `p_id_penjualan` VARCHAR(30), IN `p_id_pelanggan` VARCHAR(10), IN `p_nama_pelanggan` VARCHAR(100), IN `p_alamat_pelanggan` VARCHAR(200), IN `p_jenis_pelanggan` VARCHAR(30), IN `p_lokasi` VARCHAR(20), IN `p_tgl_transaksi` DATETIME, IN `p_grand_total_sbl_pajak` DECIMAL(15,2), IN `p_diskon_total_persen` DECIMAL(10,2), IN `p_diskon_total_rp` DECIMAL(10,2), IN `p_pajak_persen` DECIMAL(10,2), IN `p_pajak_rp` DECIMAL(10,2), IN `p_grand_total_stl_pajak` DECIMAL(15,2), IN `p_total_hpp` DECIMAL(15,2), IN `p_laba` DECIMAL(15,2), IN `p_bayar` DECIMAL(15,2), IN `p_nominal_transfer` DECIMAL(15,2), IN `p_biaya_kirim` DECIMAL(10,2), IN `p_kembali` DECIMAL(15,2), IN `p_sisa_tagihan` DECIMAL(15,2), IN `p_jatuh_tempo` DATETIME, IN `p_status_bayar` VARCHAR(20), IN `p_kode_akun_kas` VARCHAR(20), IN `p_nama_akun_kas` VARCHAR(50), IN `p_kode_akun_transfer` VARCHAR(20), IN `p_nama_akun_transfer` VARCHAR(50), IN `p_kode_rek_piutang` VARCHAR(20), IN `p_nama_rek_piutang` VARCHAR(50), IN `p_id_user` VARCHAR(20), IN `p_id_komputer` VARCHAR(30), IN `p_izinkan_stok_minus` TINYINT(1), IN `p_izinkan_backdate` TINYINT(1), IN `p_id_draft` VARCHAR(30), IN `p_bank` VARCHAR(50), IN `p_no_rekening` VARCHAR(30), IN `p_nama_rekening` VARCHAR(50), IN `p_no_referensi` VARCHAR(100), IN `p_status_transaksi` VARCHAR(20), IN `p_id_sales` VARCHAR(20), IN `p_nama_sales` VARCHAR(100), OUT `p_success` TINYINT(1), OUT `p_error_code` VARCHAR(50), OUT `p_error_message` VARCHAR(255), OUT `p_id_penjualan_out` VARCHAR(30))  proc_body: BEGIN
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

END proc_body$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_trx_transfer_stok_simpan` (IN `p_id_transfer` VARCHAR(20), IN `p_jenis_transfer` VARCHAR(50), IN `p_uraian` VARCHAR(60), IN `p_tgl_transfer` DATETIME, IN `p_id_barang_k` VARCHAR(20), IN `p_nama_barang_k` VARCHAR(100), IN `p_qty_k` DECIMAL(10,2), IN `p_satuan_k` VARCHAR(20), IN `p_isi_k` DECIMAL(10,2), IN `p_qty_sat_k` DECIMAL(10,2), IN `p_harga_sat_k` DECIMAL(10,2), IN `p_total_harga_k` DECIMAL(15,0), IN `p_id_barang_m` VARCHAR(20), IN `p_nama_barang_m` VARCHAR(100), IN `p_qty_m` DECIMAL(10,2), IN `p_satuan_m` VARCHAR(20), IN `p_isi_m` DECIMAL(10,2), IN `p_qty_sat_m` DECIMAL(10,2), IN `p_harga_sat_m` DECIMAL(10,2), IN `p_total_harga_m` DECIMAL(15,0), IN `p_lokasi` VARCHAR(20), IN `p_id_user` VARCHAR(20), IN `p_id_komputer` VARCHAR(20), IN `p_izinkan_backdate` TINYINT(1), IN `p_izinkan_stok_minus` TINYINT(1), OUT `p_success` TINYINT(1), OUT `p_error_code` VARCHAR(50), OUT `p_error_message` VARCHAR(255), OUT `p_id_transfer_out` VARCHAR(20))  proc_body: BEGIN
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

END proc_body$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_val_pembelian_harga_beli_vs_jual` (IN `p_id_barang` VARCHAR(50), IN `p_harga_beli_total` DECIMAL(15,2), IN `p_qty_sat` DECIMAL(15,4), OUT `p_rugi_kritis` TINYINT(1), OUT `p_rugi_umum` TINYINT(1), OUT `p_rugi_partai` TINYINT(1), OUT `p_harga_jual_min` DECIMAL(15,2), OUT `p_harga_jual_umum` DECIMAL(15,2), OUT `p_harga_jual_partai` DECIMAL(15,2))  sp_val_pembelian_harga_beli_vs_jual:BEGIN
    DECLARE v_harga_beli_per_kecil    DECIMAL(15,4) DEFAULT 0;
    DECLARE v_harga_jual_umum_kecil   DECIMAL(15,2) DEFAULT 0;
    DECLARE v_harga_jual_partai_kecil DECIMAL(15,2) DEFAULT 0;
    DECLARE v_ada_harga_umum          TINYINT(1) DEFAULT 0;
    DECLARE v_ada_harga_partai        TINYINT(1) DEFAULT 0;
    
    SET p_rugi_kritis = 0;
    SET p_rugi_umum = 0;
    SET p_rugi_partai = 0;
    SET p_harga_jual_min = 0;
    SET p_harga_jual_umum = 0;
    SET p_harga_jual_partai = 0;
    
    SELECT COALESCE(HARGA_JUAL_UMUM_KECIL, 0),
           COALESCE(HARGA_JUAL_PARTAI_KECIL, 0)
    INTO v_harga_jual_umum_kecil, v_harga_jual_partai_kecil
    FROM tbl_barang
    WHERE ID_BARANG = p_id_barang
    LIMIT 1;
    
    SET v_ada_harga_umum = IF(v_harga_jual_umum_kecil > 0, 1, 0);
    SET v_ada_harga_partai = IF(v_harga_jual_partai_kecil > 0, 1, 0);
    
    IF v_ada_harga_umum = 0 AND v_ada_harga_partai = 0 THEN
        LEAVE sp_val_pembelian_harga_beli_vs_jual;
    END IF;
    
    SET v_harga_beli_per_kecil = p_harga_beli_total / p_qty_sat;
    
    SET p_harga_jual_umum = ROUND(v_harga_jual_umum_kecil * p_qty_sat, 0);
    SET p_harga_jual_partai = ROUND(v_harga_jual_partai_kecil * p_qty_sat, 0);
    
    IF v_ada_harga_umum = 1 AND v_ada_harga_partai = 1 THEN
        SET p_harga_jual_min = LEAST(v_harga_jual_umum_kecil, v_harga_jual_partai_kecil);
    ELSEIF v_ada_harga_umum = 1 THEN
        SET p_harga_jual_min = v_harga_jual_umum_kecil;
    ELSE
        SET p_harga_jual_min = v_harga_jual_partai_kecil;
    END IF;
    
    IF v_ada_harga_umum = 1 AND v_harga_beli_per_kecil > v_harga_jual_umum_kecil THEN
        SET p_rugi_umum = 1;
    END IF;
    
    IF v_ada_harga_partai = 1 AND v_harga_beli_per_kecil > v_harga_jual_partai_kecil THEN
        SET p_rugi_partai = 1;
    END IF;
    
    IF (v_ada_harga_umum = 1 AND p_rugi_umum = 1 AND (v_ada_harga_partai = 0 OR p_rugi_partai = 1))
       OR (v_ada_harga_partai = 1 AND p_rugi_partai = 1 AND (v_ada_harga_umum = 0 OR p_rugi_umum = 1)) THEN
        SET p_rugi_kritis = 1;
    END IF;
    
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Struktur dari tabel `bon_karyawan`
--

CREATE TABLE `bon_karyawan` (
  `FAKTUR` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AWAL_BON` decimal(15,0) DEFAULT '0',
  `NOMINAL` decimal(15,0) DEFAULT '0',
  `AKHIR_BON` decimal(15,0) DEFAULT '0',
  `KETERANGAN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `gaji_karyawan`
--

CREATE TABLE `gaji_karyawan` (
  `NOMOR` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `BULAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `TANGGALAWAL` datetime DEFAULT NULL,
  `TANGGALAKHIR` datetime DEFAULT NULL,
  `KODE` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `POKOK` decimal(15,0) DEFAULT '0',
  `OMSET_JUAL` decimal(20,0) DEFAULT '0',
  `KOMISI_JUAL` decimal(15,0) DEFAULT '0',
  `SUPIR` int(11) DEFAULT '0',
  `SUPIR_RP` decimal(15,0) DEFAULT '0',
  `HELPER` int(11) DEFAULT '0',
  `HELPER_RP` decimal(15,0) DEFAULT '0',
  `LEMBUR` decimal(15,0) DEFAULT '0',
  `LEMBUR_RP` decimal(15,0) DEFAULT '0',
  `TUNJANGAN` decimal(15,0) DEFAULT '0',
  `TRANSP` decimal(15,0) DEFAULT '0',
  `TRANSPORT` decimal(15,0) DEFAULT '0',
  `UANG_MKN` decimal(15,0) DEFAULT '0',
  `UANG_MAKAN` decimal(15,0) DEFAULT '0',
  `SALDO_BON` decimal(15,0) DEFAULT '0',
  `POT_BON` decimal(15,0) DEFAULT '0',
  `ANGSURAN` decimal(15,0) DEFAULT '0',
  `NILAI_POTONGAN_ABSEN` decimal(15,0) DEFAULT '0',
  `ABSEN` decimal(15,0) DEFAULT '0',
  `ABSEN_RP` decimal(15,0) DEFAULT '0',
  `ABSEN_KHUSUS` decimal(15,0) DEFAULT '0',
  `ABSEN_KHUSUS_RP` decimal(15,0) DEFAULT '0',
  `TERLAMBAT` decimal(15,0) DEFAULT '0',
  `TERLAMBAT_RP` decimal(15,0) DEFAULT '0',
  `POT_LAIN` decimal(15,0) DEFAULT '0',
  `PENDAPATAN` decimal(15,0) DEFAULT '0',
  `POTONGAN` decimal(15,0) DEFAULT '0',
  `TERIMA` decimal(15,0) DEFAULT '0',
  `REKENING` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `hakaksesuser`
--

CREATE TABLE `hakaksesuser` (
  `NO` int(11) NOT NULL,
  `UserName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Role` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ModuleName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CanRead` tinyint(1) DEFAULT '0',
  `CanAdd` tinyint(1) DEFAULT '0',
  `CanEdit` tinyint(1) DEFAULT '0',
  `CanDelete` tinyint(1) DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data untuk tabel `hakaksesuser`
--

INSERT INTO `hakaksesuser` (`NO`, `UserName`, `Role`, `ModuleName`, `CanRead`, `CanAdd`, `CanEdit`, `CanDelete`, `created_at`, `sync_id`) VALUES
(1, 'Master', 'Master Data', 'MASTER', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61ce70-3cc8-11f1-85fc-a8a159b7c591'),
(2, 'Master', 'Master Data', 'Toko', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61ddbb-3cc8-11f1-85fc-a8a159b7c591'),
(3, 'Master', 'Master Data', 'Barang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61e2ce-3cc8-11f1-85fc-a8a159b7c591'),
(4, 'Master', 'Master Data', 'Harga Beli', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61e5d2-3cc8-11f1-85fc-a8a159b7c591'),
(5, 'Master', 'Master Data', 'Tambah Stok', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61e8b4-3cc8-11f1-85fc-a8a159b7c591'),
(6, 'Master', 'Master Data', 'Kurang Stok', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61eb46-3cc8-11f1-85fc-a8a159b7c591'),
(7, 'Master', 'Master Data', 'Perbaiki Data Barang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61edf7-3cc8-11f1-85fc-a8a159b7c591'),
(8, 'Master', 'Master Data', 'Perbaiki isi satuan', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61f0ba-3cc8-11f1-85fc-a8a159b7c591'),
(9, 'Master', 'Master Data', 'Supplier', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61f36b-3cc8-11f1-85fc-a8a159b7c591'),
(10, 'Master', 'Master Data', 'Pelanggan', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61f600-3cc8-11f1-85fc-a8a159b7c591'),
(11, 'Master', 'Master Data', 'Tabel Referensi', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce61f8a0-3cc8-11f1-85fc-a8a159b7c591'),
(12, 'Master', 'Master Data', 'Armada', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62067d-3cc8-11f1-85fc-a8a159b7c591'),
(13, 'Master', 'Master Data', 'Karyawan', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62097c-3cc8-11f1-85fc-a8a159b7c591'),
(14, 'Master', 'Master Data', 'User', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce620c2e-3cc8-11f1-85fc-a8a159b7c591'),
(15, 'Master', 'Master Data', 'Hak Akses', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce620ea8-3cc8-11f1-85fc-a8a159b7c591'),
(16, 'Admin', 'Master Data', 'MASTER', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce621117-3cc8-11f1-85fc-a8a159b7c591'),
(17, 'Admin', 'Master Data', 'Toko', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce621391-3cc8-11f1-85fc-a8a159b7c591'),
(18, 'Admin', 'Master Data', 'Barang', 1, 1, 1, 0, '2026-04-20 21:49:39', 'ce62161f-3cc8-11f1-85fc-a8a159b7c591'),
(19, 'Admin', 'Master Data', 'Harga Beli', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce622526-3cc8-11f1-85fc-a8a159b7c591'),
(20, 'Admin', 'Master Data', 'Tambah Stok', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce622924-3cc8-11f1-85fc-a8a159b7c591'),
(21, 'Admin', 'Master Data', 'Kurang Stok', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce622e84-3cc8-11f1-85fc-a8a159b7c591'),
(22, 'Admin', 'Master Data', 'Perbaiki Data Barang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6231fd-3cc8-11f1-85fc-a8a159b7c591'),
(23, 'Admin', 'Master Data', 'Perbaiki isi satuan', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62405a-3cc8-11f1-85fc-a8a159b7c591'),
(24, 'Admin', 'Master Data', 'Supplier', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce6243d4-3cc8-11f1-85fc-a8a159b7c591'),
(25, 'Admin', 'Master Data', 'Pelanggan', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62471e-3cc8-11f1-85fc-a8a159b7c591'),
(26, 'Admin', 'Master Data', 'Tabel Referensi', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62552b-3cc8-11f1-85fc-a8a159b7c591'),
(27, 'Admin', 'Master Data', 'Armada', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6258cb-3cc8-11f1-85fc-a8a159b7c591'),
(28, 'Admin', 'Master Data', 'Karyawan', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce625cd1-3cc8-11f1-85fc-a8a159b7c591'),
(29, 'Admin', 'Master Data', 'User', 1, 0, 1, 0, '2026-04-20 21:49:39', 'ce625fb5-3cc8-11f1-85fc-a8a159b7c591'),
(30, 'Admin', 'Master Data', 'Hak Akses', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce626412-3cc8-11f1-85fc-a8a159b7c591'),
(31, 'Kasir', 'Master Data', 'MASTER', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62649e-3cc8-11f1-85fc-a8a159b7c591'),
(32, 'Kasir', 'Master Data', 'Toko', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62651b-3cc8-11f1-85fc-a8a159b7c591'),
(33, 'Kasir', 'Master Data', 'Barang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62658f-3cc8-11f1-85fc-a8a159b7c591'),
(34, 'Kasir', 'Master Data', 'Harga Beli', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce626609-3cc8-11f1-85fc-a8a159b7c591'),
(35, 'Kasir', 'Master Data', 'Tambah Stok', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62668a-3cc8-11f1-85fc-a8a159b7c591'),
(36, 'Kasir', 'Master Data', 'Kurang Stok', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce626702-3cc8-11f1-85fc-a8a159b7c591'),
(37, 'Kasir', 'Master Data', 'Perbaiki Data Barang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce626779-3cc8-11f1-85fc-a8a159b7c591'),
(38, 'Kasir', 'Master Data', 'Perbaiki isi satuan', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6267f8-3cc8-11f1-85fc-a8a159b7c591'),
(39, 'Kasir', 'Master Data', 'Supplier', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62686d-3cc8-11f1-85fc-a8a159b7c591'),
(40, 'Kasir', 'Master Data', 'Pelanggan', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6268bd-3cc8-11f1-85fc-a8a159b7c591'),
(41, 'Kasir', 'Master Data', 'Tabel Referensi', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62690b-3cc8-11f1-85fc-a8a159b7c591'),
(42, 'Kasir', 'Master Data', 'Armada', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62695e-3cc8-11f1-85fc-a8a159b7c591'),
(43, 'Kasir', 'Master Data', 'Karyawan', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6269ac-3cc8-11f1-85fc-a8a159b7c591'),
(44, 'Kasir', 'Master Data', 'User', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6269f9-3cc8-11f1-85fc-a8a159b7c591'),
(45, 'Kasir', 'Master Data', 'Hak Akses', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce626a47-3cc8-11f1-85fc-a8a159b7c591'),
(46, 'Gudang', 'Master Data', 'MASTER', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce626aa4-3cc8-11f1-85fc-a8a159b7c591'),
(47, 'Gudang', 'Master Data', 'Toko', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce626af1-3cc8-11f1-85fc-a8a159b7c591'),
(48, 'Gudang', 'Master Data', 'Barang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce626b3c-3cc8-11f1-85fc-a8a159b7c591'),
(49, 'Gudang', 'Master Data', 'Harga Beli', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce626b86-3cc8-11f1-85fc-a8a159b7c591'),
(50, 'Gudang', 'Master Data', 'Tambah Stok', 1, 0, 1, 0, '2026-04-20 21:49:39', 'ce626bd2-3cc8-11f1-85fc-a8a159b7c591'),
(51, 'Gudang', 'Master Data', 'Kurang Stok', 1, 0, 1, 0, '2026-04-20 21:49:39', 'ce626c20-3cc8-11f1-85fc-a8a159b7c591'),
(52, 'Gudang', 'Master Data', 'Perbaiki Data Barang', 1, 0, 1, 0, '2026-04-20 21:49:39', 'ce626c6a-3cc8-11f1-85fc-a8a159b7c591'),
(53, 'Gudang', 'Master Data', 'Perbaiki isi satuan', 1, 0, 1, 0, '2026-04-20 21:49:39', 'ce626cb7-3cc8-11f1-85fc-a8a159b7c591'),
(54, 'Gudang', 'Master Data', 'Supplier', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce626d07-3cc8-11f1-85fc-a8a159b7c591'),
(55, 'Gudang', 'Master Data', 'Pelanggan', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce626d52-3cc8-11f1-85fc-a8a159b7c591'),
(56, 'Gudang', 'Master Data', 'Tabel Referensi', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce626d9d-3cc8-11f1-85fc-a8a159b7c591'),
(57, 'Gudang', 'Master Data', 'Armada', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce626de7-3cc8-11f1-85fc-a8a159b7c591'),
(58, 'Gudang', 'Master Data', 'Karyawan', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce626e32-3cc8-11f1-85fc-a8a159b7c591'),
(59, 'Gudang', 'Master Data', 'User', 1, 0, 1, 0, '2026-04-20 21:49:39', 'ce626e7c-3cc8-11f1-85fc-a8a159b7c591'),
(60, 'Gudang', 'Master Data', 'Hak Akses', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62706f-3cc8-11f1-85fc-a8a159b7c591'),
(61, 'Master', 'Master Data', 'TRANSAKSI', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce6271a8-3cc8-11f1-85fc-a8a159b7c591'),
(62, 'Master', 'Master Data', 'Pembelian', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce6272dc-3cc8-11f1-85fc-a8a159b7c591'),
(63, 'Master', 'Master Data', 'Penjualan', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62740e-3cc8-11f1-85fc-a8a159b7c591'),
(64, 'Master', 'Master Data', 'Retur Pembelian', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce6276a7-3cc8-11f1-85fc-a8a159b7c591'),
(65, 'Master', 'Master Data', 'Retur Penjualan', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce627880-3cc8-11f1-85fc-a8a159b7c591'),
(66, 'Master', 'Master Data', 'Bayar Hutang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce627a5f-3cc8-11f1-85fc-a8a159b7c591'),
(67, 'Master', 'Master Data', 'Bayar Piutang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce627dc1-3cc8-11f1-85fc-a8a159b7c591'),
(68, 'Master', 'Master Data', 'Transfer Stok', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce627fb7-3cc8-11f1-85fc-a8a159b7c591'),
(69, 'Master', 'Master Data', 'Transfer Barang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce628055-3cc8-11f1-85fc-a8a159b7c591'),
(70, 'Master', 'Master Data', 'Stok Opname', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce6280a5-3cc8-11f1-85fc-a8a159b7c591'),
(71, 'Master', 'Master Data', 'Surat Jalan', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce6280f5-3cc8-11f1-85fc-a8a159b7c591'),
(72, 'Admin', 'Master Data', 'TRANSAKSI', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628185-3cc8-11f1-85fc-a8a159b7c591'),
(73, 'Admin', 'Master Data', 'Pembelian', 1, 1, 1, 0, '2026-04-20 21:49:39', 'ce6281d2-3cc8-11f1-85fc-a8a159b7c591'),
(74, 'Admin', 'Master Data', 'Penjualan', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628225-3cc8-11f1-85fc-a8a159b7c591'),
(75, 'Admin', 'Master Data', 'Retur Pembelian', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628270-3cc8-11f1-85fc-a8a159b7c591'),
(76, 'Admin', 'Master Data', 'Retur Penjualan', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce6282bc-3cc8-11f1-85fc-a8a159b7c591'),
(77, 'Admin', 'Master Data', 'Bayar Hutang', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628307-3cc8-11f1-85fc-a8a159b7c591'),
(78, 'Admin', 'Master Data', 'Bayar Piutang', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628354-3cc8-11f1-85fc-a8a159b7c591'),
(79, 'Admin', 'Master Data', 'Transfer Stok', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce62839f-3cc8-11f1-85fc-a8a159b7c591'),
(80, 'Admin', 'Master Data', 'Transfer Barang', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce6283ec-3cc8-11f1-85fc-a8a159b7c591'),
(81, 'Admin', 'Master Data', 'Stok Opname', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce62843f-3cc8-11f1-85fc-a8a159b7c591'),
(82, 'Admin', 'Master Data', 'Surat Jalan', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce62848b-3cc8-11f1-85fc-a8a159b7c591'),
(83, 'Kasir', 'Master Data', 'TRANSAKSI', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce6284da-3cc8-11f1-85fc-a8a159b7c591'),
(84, 'Kasir', 'Master Data', 'Pembelian', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628526-3cc8-11f1-85fc-a8a159b7c591'),
(85, 'Kasir', 'Master Data', 'Penjualan', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628572-3cc8-11f1-85fc-a8a159b7c591'),
(86, 'Kasir', 'Master Data', 'Retur Pembelian', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce6285c0-3cc8-11f1-85fc-a8a159b7c591'),
(87, 'Kasir', 'Master Data', 'Retur Penjualan', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce62860b-3cc8-11f1-85fc-a8a159b7c591'),
(88, 'Kasir', 'Master Data', 'Bayar Hutang', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce62865c-3cc8-11f1-85fc-a8a159b7c591'),
(89, 'Kasir', 'Master Data', 'Bayar Piutang', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce6286a7-3cc8-11f1-85fc-a8a159b7c591'),
(90, 'Kasir', 'Master Data', 'Transfer Stok', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce6286f2-3cc8-11f1-85fc-a8a159b7c591'),
(91, 'Kasir', 'Master Data', 'Transfer Barang', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce62873b-3cc8-11f1-85fc-a8a159b7c591'),
(92, 'Kasir', 'Master Data', 'Stok Opname', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce628787-3cc8-11f1-85fc-a8a159b7c591'),
(93, 'Kasir', 'Master Data', 'Surat Jalan', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6287db-3cc8-11f1-85fc-a8a159b7c591'),
(94, 'Gudang', 'Master Data', 'TRANSAKSI', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628825-3cc8-11f1-85fc-a8a159b7c591'),
(95, 'Gudang', 'Master Data', 'Pembelian', 1, 1, 1, 0, '2026-04-20 21:49:39', 'ce628872-3cc8-11f1-85fc-a8a159b7c591'),
(96, 'Gudang', 'Master Data', 'Penjualan', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce6288bd-3cc8-11f1-85fc-a8a159b7c591'),
(97, 'Gudang', 'Master Data', 'Retur Pembelian', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628907-3cc8-11f1-85fc-a8a159b7c591'),
(98, 'Gudang', 'Master Data', 'Retur Penjualan', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628955-3cc8-11f1-85fc-a8a159b7c591'),
(99, 'Gudang', 'Master Data', 'Bayar Hutang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6289a1-3cc8-11f1-85fc-a8a159b7c591'),
(100, 'Gudang', 'Master Data', 'Bayar Piutang', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce6289f0-3cc8-11f1-85fc-a8a159b7c591'),
(101, 'Gudang', 'Master Data', 'Transfer Stok', 1, 1, 1, 0, '2026-04-20 21:49:39', 'ce628a86-3cc8-11f1-85fc-a8a159b7c591'),
(102, 'Gudang', 'Master Data', 'Transfer Barang', 1, 1, 1, 0, '2026-04-20 21:49:39', 'ce628b43-3cc8-11f1-85fc-a8a159b7c591'),
(103, 'Gudang', 'Master Data', 'Stok Opname', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628dac-3cc8-11f1-85fc-a8a159b7c591'),
(104, 'Gudang', 'Master Data', 'Surat Jalan', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce628e57-3cc8-11f1-85fc-a8a159b7c591'),
(105, 'Master', 'Master Data', 'JURNAL', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce628f02-3cc8-11f1-85fc-a8a159b7c591'),
(106, 'Admin', 'Master Data', 'JURNAL', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce628fac-3cc8-11f1-85fc-a8a159b7c591'),
(107, 'Kasir', 'Master Data', 'JURNAL', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce629058-3cc8-11f1-85fc-a8a159b7c591'),
(108, 'Gudang', 'Master Data', 'JURNAL', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce629104-3cc8-11f1-85fc-a8a159b7c591'),
(109, 'Master', 'Master Data', 'MENUKARYAWAN', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62919f-3cc8-11f1-85fc-a8a159b7c591'),
(110, 'Master', 'Master Data', 'BON', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce629260-3cc8-11f1-85fc-a8a159b7c591'),
(111, 'Master', 'Master Data', 'BAYAR', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce629326-3cc8-11f1-85fc-a8a159b7c591'),
(112, 'Master', 'Master Data', 'LAP BON', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce6293c7-3cc8-11f1-85fc-a8a159b7c591'),
(113, 'Master', 'Master Data', 'GAJI', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce6294a3-3cc8-11f1-85fc-a8a159b7c591'),
(114, 'Master', 'Master Data', 'LAP GAJI', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62959b-3cc8-11f1-85fc-a8a159b7c591'),
(115, 'Admin', 'Master Data', 'MENUKARYAWAN', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce6296bb-3cc8-11f1-85fc-a8a159b7c591'),
(116, 'Admin', 'Master Data', 'BON', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce629805-3cc8-11f1-85fc-a8a159b7c591'),
(117, 'Admin', 'Master Data', 'BAYAR', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce629916-3cc8-11f1-85fc-a8a159b7c591'),
(118, 'Admin', 'Master Data', 'LAP BON', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce6299c8-3cc8-11f1-85fc-a8a159b7c591'),
(119, 'Admin', 'Master Data', 'GAJI', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce629a45-3cc8-11f1-85fc-a8a159b7c591'),
(120, 'Admin', 'Master Data', 'LAP GAJI', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce629abd-3cc8-11f1-85fc-a8a159b7c591'),
(121, 'Kasir', 'Master Data', 'MENUKARYAWAN', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce629b31-3cc8-11f1-85fc-a8a159b7c591'),
(122, 'Kasir', 'Master Data', 'BON', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce629ba7-3cc8-11f1-85fc-a8a159b7c591'),
(123, 'Kasir', 'Master Data', 'BAYAR', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce629c1a-3cc8-11f1-85fc-a8a159b7c591'),
(124, 'Kasir', 'Master Data', 'LAP BON', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce629c8f-3cc8-11f1-85fc-a8a159b7c591'),
(125, 'Kasir', 'Master Data', 'GAJI', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce629d01-3cc8-11f1-85fc-a8a159b7c591'),
(126, 'Kasir', 'Master Data', 'LAP GAJI', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce629d74-3cc8-11f1-85fc-a8a159b7c591'),
(127, 'Gudang', 'Master Data', 'MENUKARYAWAN', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce629de6-3cc8-11f1-85fc-a8a159b7c591'),
(128, 'Gudang', 'Master Data', 'BON', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce629e58-3cc8-11f1-85fc-a8a159b7c591'),
(129, 'Gudang', 'Master Data', 'BAYAR', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce629ec9-3cc8-11f1-85fc-a8a159b7c591'),
(130, 'Gudang', 'Master Data', 'LAP BON', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce629f3a-3cc8-11f1-85fc-a8a159b7c591'),
(131, 'Gudang', 'Master Data', 'GAJI', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce629fad-3cc8-11f1-85fc-a8a159b7c591'),
(132, 'Gudang', 'Master Data', 'LAP GAJI', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce629fff-3cc8-11f1-85fc-a8a159b7c591'),
(133, 'Master', 'Master Data', 'LAPORAN', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a053-3cc8-11f1-85fc-a8a159b7c591'),
(134, 'Master', 'Master Data', 'Mutasi saldo', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a0a2-3cc8-11f1-85fc-a8a159b7c591'),
(135, 'Master', 'Master Data', 'Mutasi barang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a0f4-3cc8-11f1-85fc-a8a159b7c591'),
(136, 'Master', 'Master Data', 'Neraca', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a144-3cc8-11f1-85fc-a8a159b7c591'),
(137, 'Master', 'Master Data', 'Lap Pembelian', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a191-3cc8-11f1-85fc-a8a159b7c591'),
(138, 'Master', 'Master Data', 'Lap Penjualan', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a203-3cc8-11f1-85fc-a8a159b7c591'),
(139, 'Master', 'Master Data', 'Piutang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a278-3cc8-11f1-85fc-a8a159b7c591'),
(140, 'Master', 'Master Data', 'Hutang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a304-3cc8-11f1-85fc-a8a159b7c591'),
(141, 'Master', 'Master Data', 'Kas Penjualan', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a38b-3cc8-11f1-85fc-a8a159b7c591'),
(142, 'Master', 'Master Data', 'Stok Barang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a3ef-3cc8-11f1-85fc-a8a159b7c591'),
(143, 'Master', 'Master Data', 'Grafik', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62a450-3cc8-11f1-85fc-a8a159b7c591'),
(144, 'Admin', 'Master Data', 'LAPORAN', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a4ae-3cc8-11f1-85fc-a8a159b7c591'),
(145, 'Admin', 'Master Data', 'Mutasi saldo', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a50d-3cc8-11f1-85fc-a8a159b7c591'),
(146, 'Admin', 'Master Data', 'Mutasi barang', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a56d-3cc8-11f1-85fc-a8a159b7c591'),
(147, 'Admin', 'Master Data', 'Neraca', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a5d0-3cc8-11f1-85fc-a8a159b7c591'),
(148, 'Admin', 'Master Data', 'Lap Pembelian', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a62f-3cc8-11f1-85fc-a8a159b7c591'),
(149, 'Admin', 'Master Data', 'Lap Penjualan', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a68c-3cc8-11f1-85fc-a8a159b7c591'),
(150, 'Admin', 'Master Data', 'Piutang', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a6ec-3cc8-11f1-85fc-a8a159b7c591'),
(151, 'Admin', 'Master Data', 'Hutang', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a749-3cc8-11f1-85fc-a8a159b7c591'),
(152, 'Admin', 'Master Data', 'Kas Penjualan', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a7aa-3cc8-11f1-85fc-a8a159b7c591'),
(153, 'Admin', 'Master Data', 'Stok Barang', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a80f-3cc8-11f1-85fc-a8a159b7c591'),
(154, 'Admin', 'Master Data', 'Grafik', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a86f-3cc8-11f1-85fc-a8a159b7c591'),
(155, 'Kasir', 'Master Data', 'LAPORAN', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a8d2-3cc8-11f1-85fc-a8a159b7c591'),
(156, 'Kasir', 'Master Data', 'Mutasi saldo', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a930-3cc8-11f1-85fc-a8a159b7c591'),
(157, 'Kasir', 'Master Data', 'Mutasi barang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a98e-3cc8-11f1-85fc-a8a159b7c591'),
(158, 'Kasir', 'Master Data', 'Neraca', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62a9ed-3cc8-11f1-85fc-a8a159b7c591'),
(159, 'Kasir', 'Master Data', 'Lap Pembelian', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62aa53-3cc8-11f1-85fc-a8a159b7c591'),
(160, 'Kasir', 'Master Data', 'Lap Penjualan', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62aaa2-3cc8-11f1-85fc-a8a159b7c591'),
(161, 'Kasir', 'Master Data', 'Piutang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62aaf3-3cc8-11f1-85fc-a8a159b7c591'),
(162, 'Kasir', 'Master Data', 'Hutang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62ab3e-3cc8-11f1-85fc-a8a159b7c591'),
(163, 'Kasir', 'Master Data', 'Kas Penjualan', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62ab8d-3cc8-11f1-85fc-a8a159b7c591'),
(164, 'Kasir', 'Master Data', 'Stok Barang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62abe7-3cc8-11f1-85fc-a8a159b7c591'),
(165, 'Kasir', 'Master Data', 'Grafik', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62ac3a-3cc8-11f1-85fc-a8a159b7c591'),
(166, 'Gudang', 'Master Data', 'LAPORAN', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62ac86-3cc8-11f1-85fc-a8a159b7c591'),
(167, 'Gudang', 'Master Data', 'Mutasi saldo', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62acd3-3cc8-11f1-85fc-a8a159b7c591'),
(168, 'Gudang', 'Master Data', 'Mutasi barang', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62ad22-3cc8-11f1-85fc-a8a159b7c591'),
(169, 'Gudang', 'Master Data', 'Neraca', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62ad72-3cc8-11f1-85fc-a8a159b7c591'),
(170, 'Gudang', 'Master Data', 'Lap Pembelian', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62adc0-3cc8-11f1-85fc-a8a159b7c591'),
(171, 'Gudang', 'Master Data', 'Lap Penjualan', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62ae16-3cc8-11f1-85fc-a8a159b7c591'),
(172, 'Gudang', 'Master Data', 'Piutang', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62ae91-3cc8-11f1-85fc-a8a159b7c591'),
(173, 'Gudang', 'Master Data', 'Hutang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62af0a-3cc8-11f1-85fc-a8a159b7c591'),
(174, 'Gudang', 'Master Data', 'Kas Penjualan', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62af84-3cc8-11f1-85fc-a8a159b7c591'),
(175, 'Gudang', 'Master Data', 'Stok Barang', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b000-3cc8-11f1-85fc-a8a159b7c591'),
(176, 'Gudang', 'Master Data', 'Grafik', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b09c-3cc8-11f1-85fc-a8a159b7c591'),
(177, 'Master', 'Master Data', 'UTILITY', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62b114-3cc8-11f1-85fc-a8a159b7c591'),
(178, 'Master', 'Master Data', 'Database', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62b18f-3cc8-11f1-85fc-a8a159b7c591'),
(179, 'Master', 'Master Data', 'Backup Database', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62b20d-3cc8-11f1-85fc-a8a159b7c591'),
(180, 'Master', 'Master Data', 'Perbaiki Database', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62b289-3cc8-11f1-85fc-a8a159b7c591'),
(181, 'Master', 'Master Data', 'History', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62b309-3cc8-11f1-85fc-a8a159b7c591'),
(182, 'Master', 'Master Data', 'Setting Printer', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62b396-3cc8-11f1-85fc-a8a159b7c591'),
(183, 'Admin', 'Master Data', 'UTILITY', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b415-3cc8-11f1-85fc-a8a159b7c591'),
(184, 'Admin', 'Master Data', 'Database', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b4a0-3cc8-11f1-85fc-a8a159b7c591'),
(185, 'Admin', 'Master Data', 'Backup Database', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b524-3cc8-11f1-85fc-a8a159b7c591'),
(186, 'Admin', 'Master Data', 'Perbaiki Database', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b5a4-3cc8-11f1-85fc-a8a159b7c591'),
(187, 'Admin', 'Master Data', 'History', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b634-3cc8-11f1-85fc-a8a159b7c591'),
(188, 'Admin', 'Master Data', 'Setting Printer', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b6b2-3cc8-11f1-85fc-a8a159b7c591'),
(189, 'Kasir', 'Master Data', 'UTILITY', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b72b-3cc8-11f1-85fc-a8a159b7c591'),
(190, 'Kasir', 'Master Data', 'Database', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b7a7-3cc8-11f1-85fc-a8a159b7c591'),
(191, 'Kasir', 'Master Data', 'Backup Database', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b824-3cc8-11f1-85fc-a8a159b7c591'),
(192, 'Kasir', 'Master Data', 'Perbaiki Database', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b8aa-3cc8-11f1-85fc-a8a159b7c591'),
(193, 'Kasir', 'Master Data', 'History', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b923-3cc8-11f1-85fc-a8a159b7c591'),
(194, 'Kasir', 'Master Data', 'Setting Printer', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62b997-3cc8-11f1-85fc-a8a159b7c591'),
(195, 'Gudang', 'Master Data', 'UTILITY', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62ba36-3cc8-11f1-85fc-a8a159b7c591'),
(196, 'Gudang', 'Master Data', 'Database', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62bc2b-3cc8-11f1-85fc-a8a159b7c591'),
(197, 'Gudang', 'Master Data', 'Backup Database', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62bcda-3cc8-11f1-85fc-a8a159b7c591'),
(198, 'Gudang', 'Master Data', 'Perbaiki Database', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62bf1b-3cc8-11f1-85fc-a8a159b7c591'),
(199, 'Gudang', 'Master Data', 'History', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62bfb5-3cc8-11f1-85fc-a8a159b7c591'),
(200, 'Gudang', 'Master Data', 'Setting Printer', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce62c04c-3cc8-11f1-85fc-a8a159b7c591'),
(216, 'Master', 'Master Data', 'Export Barang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62c0e6-3cc8-11f1-85fc-a8a159b7c591'),
(217, 'Master', 'Master Data', 'Import Barang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce62c181-3cc8-11f1-85fc-a8a159b7c591'),
(218, 'Admin', 'Master Data', 'Export Barang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62c21b-3cc8-11f1-85fc-a8a159b7c591'),
(219, 'Admin', 'Master Data', 'Import Barang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62c2c0-3cc8-11f1-85fc-a8a159b7c591'),
(220, 'Kasir', 'Master Data', 'Export Barang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62c357-3cc8-11f1-85fc-a8a159b7c591'),
(221, 'Kasir', 'Master Data', 'Import Barang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce62c3f1-3cc8-11f1-85fc-a8a159b7c591'),
(222, 'Gudang', 'Master Data', 'Export Barang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce6329f4-3cc8-11f1-85fc-a8a159b7c591'),
(223, 'Gudang', 'Master Data', 'Import Barang', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce632d9f-3cc8-11f1-85fc-a8a159b7c591'),
(224, 'Master', 'Master Data', 'MASTER GAJI', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce632fe3-3cc8-11f1-85fc-a8a159b7c591'),
(225, 'Master', 'Master Data', 'LAP BON KAR', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce6331ba-3cc8-11f1-85fc-a8a159b7c591'),
(226, 'Admin', 'Master Data', 'MASTER GAJI', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6332fd-3cc8-11f1-85fc-a8a159b7c591'),
(227, 'Admin', 'Master Data', 'LAP BON KAR', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce63341d-3cc8-11f1-85fc-a8a159b7c591'),
(228, 'Kasir', 'Master Data', 'MASTER GAJI', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce633524-3cc8-11f1-85fc-a8a159b7c591'),
(229, 'Kasir', 'Master Data', 'LAP BON KAR', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce63362e-3cc8-11f1-85fc-a8a159b7c591'),
(230, 'Gudang', 'Master Data', 'MASTER GAJI', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce633730-3cc8-11f1-85fc-a8a159b7c591'),
(231, 'Gudang', 'Master Data', 'LAP BON KAR', 1, 1, 0, 0, '2026-04-20 21:49:39', 'ce633840-3cc8-11f1-85fc-a8a159b7c591'),
(232, 'Master', 'Master Data', 'Jurnal Umum', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce63393d-3cc8-11f1-85fc-a8a159b7c591'),
(233, 'Master', 'Master Data', 'Buku Besar', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce633a4c-3cc8-11f1-85fc-a8a159b7c591'),
(234, 'Master', 'Master Data', 'Buku Besar Pembantu', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce633b55-3cc8-11f1-85fc-a8a159b7c591'),
(235, 'Master', 'Master Data', 'Jual PPnNonPPn', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce633c5a-3cc8-11f1-85fc-a8a159b7c591'),
(236, 'Master', 'Master Data', 'Retur Beli', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce633d85-3cc8-11f1-85fc-a8a159b7c591'),
(237, 'Master', 'Master Data', 'Retur Jual', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce633f13-3cc8-11f1-85fc-a8a159b7c591'),
(238, 'Admin', 'Master Data', 'Jurnal Umum', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce6340b0-3cc8-11f1-85fc-a8a159b7c591'),
(239, 'Admin', 'Master Data', 'Buku Besar', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6341f1-3cc8-11f1-85fc-a8a159b7c591'),
(240, 'Admin', 'Master Data', 'Buku Besar Pembantu', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce63430d-3cc8-11f1-85fc-a8a159b7c591'),
(241, 'Admin', 'Master Data', 'Jual PPnNonPPn', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce634432-3cc8-11f1-85fc-a8a159b7c591'),
(242, 'Admin', 'Master Data', 'Retur Beli', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce634572-3cc8-11f1-85fc-a8a159b7c591'),
(243, 'Admin', 'Master Data', 'Retur Jual', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce634697-3cc8-11f1-85fc-a8a159b7c591'),
(244, 'Kasir', 'Master Data', 'Jurnal Umum', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6347b6-3cc8-11f1-85fc-a8a159b7c591'),
(245, 'Kasir', 'Master Data', 'Buku Besar', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6348c4-3cc8-11f1-85fc-a8a159b7c591'),
(246, 'Kasir', 'Master Data', 'Buku Besar Pembantu', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6349d2-3cc8-11f1-85fc-a8a159b7c591'),
(247, 'Kasir', 'Master Data', 'Jual PPnNonPPn', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce634b67-3cc8-11f1-85fc-a8a159b7c591'),
(248, 'Kasir', 'Master Data', 'Retur Beli', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce634c78-3cc8-11f1-85fc-a8a159b7c591'),
(249, 'Kasir', 'Master Data', 'Retur Jual', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce634d8a-3cc8-11f1-85fc-a8a159b7c591'),
(250, 'Gudang', 'Master Data', 'Jurnal Umum', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce634e97-3cc8-11f1-85fc-a8a159b7c591'),
(251, 'Gudang', 'Master Data', 'Buku Besar', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce634fa2-3cc8-11f1-85fc-a8a159b7c591'),
(252, 'Gudang', 'Master Data', 'Buku Besar Pembantu', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce6350ad-3cc8-11f1-85fc-a8a159b7c591'),
(253, 'Gudang', 'Master Data', 'Jual PPnNonPPn', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce63583f-3cc8-11f1-85fc-a8a159b7c591'),
(254, 'Gudang', 'Master Data', 'Retur Beli', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce63596a-3cc8-11f1-85fc-a8a159b7c591'),
(255, 'Gudang', 'Master Data', 'Retur Jual', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce635a7a-3cc8-11f1-85fc-a8a159b7c591'),
(256, 'Master', 'Master Data', 'Restore Database', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce635c53-3cc8-11f1-85fc-a8a159b7c591'),
(257, 'Master', 'Master Data', 'Query', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce635ef2-3cc8-11f1-85fc-a8a159b7c591'),
(258, 'Admin', 'Master Data', 'Restore Database', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce63604b-3cc8-11f1-85fc-a8a159b7c591'),
(259, 'Admin', 'Master Data', 'Query', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce63617b-3cc8-11f1-85fc-a8a159b7c591'),
(260, 'Kasir', 'Master Data', 'Restore Database', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce63ba65-3cc8-11f1-85fc-a8a159b7c591'),
(261, 'Kasir', 'Master Data', 'Query', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce63bc5b-3cc8-11f1-85fc-a8a159b7c591'),
(262, 'Gudang', 'Master Data', 'Restore Database', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce63bd71-3cc8-11f1-85fc-a8a159b7c591'),
(263, 'Gudang', 'Master Data', 'Query', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce63c0cd-3cc8-11f1-85fc-a8a159b7c591'),
(264, 'Master', 'Master Data', 'POSTING', 1, 1, 1, 1, '2026-04-20 21:49:39', 'ce63c6a3-3cc8-11f1-85fc-a8a159b7c591'),
(265, 'Admin', 'Master Data', 'POSTING', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce63c823-3cc8-11f1-85fc-a8a159b7c591'),
(266, 'Kasir', 'Master Data', 'POSTING', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce63c951-3cc8-11f1-85fc-a8a159b7c591'),
(267, 'Gudang', 'Master Data', 'POSTING', 1, 0, 0, 0, '2026-04-20 21:49:39', 'ce63dce9-3cc8-11f1-85fc-a8a159b7c591'),
(1130, 'Semua', 'Mode pencarian saat membuka pembelian', 'Pencarian', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65aaa4-3cc8-11f1-85fc-a8a159b7c591'),
(1131, 'Semua', 'Izinkan pembelian kode barang dengan satuan berbeda', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65abc2-3cc8-11f1-85fc-a8a159b7c591'),
(1132, 'Semua', 'Tampilkan peringatan jika harga jual di bawah harga beli', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65accb-3cc8-11f1-85fc-a8a159b7c591'),
(1133, 'Semua', 'Edit harga beli juga ubah harga jual', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65adcb-3cc8-11f1-85fc-a8a159b7c591'),
(1134, 'Semua', 'Metode update harga beli saat pembelian', 'Metode Average (Rata - Rata)', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65aecf-3cc8-11f1-85fc-a8a159b7c591'),
(1135, 'Semua', 'Izinkan user mengubah harga beli', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65afdc-3cc8-11f1-85fc-a8a159b7c591'),
(1136, 'Semua', 'Perhitungan rata-rata harga dari stok mana', 'Toko dan Gudang', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65b0eb-3cc8-11f1-85fc-a8a159b7c591'),
(1137, 'Semua', 'Izinkan pembelian tanpa supplier', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65b1ee-3cc8-11f1-85fc-a8a159b7c591'),
(1138, 'Semua', 'Perbolehkan input pembelian tanggal lampau', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65b2ee-3cc8-11f1-85fc-a8a159b7c591'),
(1139, 'Semua', 'Langsung isi nominal total pembelian', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65b7ec-3cc8-11f1-85fc-a8a159b7c591'),
(1140, 'Semua', 'Izinkan pembelian dengan nominal 0', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65b8fa-3cc8-11f1-85fc-a8a159b7c591'),
(1141, 'Semua', 'Mode pencarian saat membuka penjualan', 'Pencarian', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65ba77-3cc8-11f1-85fc-a8a159b7c591'),
(1142, 'Semua', 'Izinkan penjualan kode barang dengan satuan berbeda', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65bd04-3cc8-11f1-85fc-a8a159b7c591'),
(1143, 'Semua', 'Izinkan user mengubah harga jual', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65bfa9-3cc8-11f1-85fc-a8a159b7c591'),
(1144, 'Semua', 'Izinkan jual barang di bawah harga beli', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65c23f-3cc8-11f1-85fc-a8a159b7c591'),
(1145, 'Semua', 'Izinkan penjualan meski stok minus', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65c687-3cc8-11f1-85fc-a8a159b7c591'),
(1146, 'Semua', 'Tampilkan informasi stok saat penjualan', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65c940-3cc8-11f1-85fc-a8a159b7c591'),
(1147, 'Semua', 'Izinkan diskon per item', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65cc5c-3cc8-11f1-85fc-a8a159b7c591'),
(1148, 'Semua', 'Perbolehkan input penjualan tanggal lampau', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65d0e0-3cc8-11f1-85fc-a8a159b7c591'),
(1149, 'Semua', 'Langsung isi nominal total penjualan', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65d455-3cc8-11f1-85fc-a8a159b7c591'),
(1150, 'Semua', 'Izinkan penjualan dengan nominal 0', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65d7bf-3cc8-11f1-85fc-a8a159b7c591'),
(1151, 'Semua', 'Izinkan edit harga jual di data master barang', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65dbc9-3cc8-11f1-85fc-a8a159b7c591'),
(1152, 'Semua', 'Mode pencarian saat membuka transfer barang', 'Pencarian', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65ded3-3cc8-11f1-85fc-a8a159b7c591'),
(1153, 'Semua', 'Izinkan transfer barang dengan satuan berbeda', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65e2fe-3cc8-11f1-85fc-a8a159b7c591'),
(1154, 'Semua', 'Izinkan transfer barang meski stok kurang', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65e5ee-3cc8-11f1-85fc-a8a159b7c591'),
(1155, 'Semua', 'Mode pencarian saat membuka retur pembelian', 'Pencarian', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65e811-3cc8-11f1-85fc-a8a159b7c591'),
(1156, 'Semua', 'Izinkan retur barang dengan satuan berbeda', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65e976-3cc8-11f1-85fc-a8a159b7c591'),
(1157, 'Semua', 'Izinkan retur meski stok menjadi minus', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65eb76-3cc8-11f1-85fc-a8a159b7c591'),
(1158, 'Semua', 'Wajib input alasan retur pembelian', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65ed58-3cc8-11f1-85fc-a8a159b7c591'),
(1159, 'Semua', 'Mode pencarian saat membuka retur penjualan', 'Pencarian', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65ef23-3cc8-11f1-85fc-a8a159b7c591'),
(1160, 'Semua', 'Izinkan retur barang dengan satuan berbeda dari penjualan', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65f10e-3cc8-11f1-85fc-a8a159b7c591'),
(1161, 'Semua', 'Izinkan retur walaupun stok akhir menjadi minus', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65f284-3cc8-11f1-85fc-a8a159b7c591'),
(1162, 'Semua', 'Wajib isi alasan saat retur penjualan', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65f3e8-3cc8-11f1-85fc-a8a159b7c591'),
(1163, 'Semua', 'Mode pencarian saat membuka form transfer stok', 'Pencarian', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65f552-3cc8-11f1-85fc-a8a159b7c591'),
(1164, 'Semua', 'Izinkan transfer walaupun stok tidak cukup', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65f6b4-3cc8-11f1-85fc-a8a159b7c591'),
(1234122, 'Semua', 'Tanyakan apakah ada pilihan untuk memilih kertas cetak', 'Tidak', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65f812-3cc8-11f1-85fc-a8a159b7c591'),
(1267524, 'Semua', 'Semua transaksi boleh menggunakan tanggal lampau', 'Iya', 0, 0, 0, 0, '2026-04-20 21:49:39', 'ce65f90a-3cc8-11f1-85fc-a8a159b7c591'),
(2262102, 'Semua', 'Izinkan transaksi keluar barang meski stok jadi minus', 'Iya', 0, 0, 0, 0, '2026-04-21 05:13:09', NULL),
(2262103, 'Semua', 'Mode pencarian saat membuka transaksi', 'Kolom data', 0, 0, 0, 0, '2026-04-21 05:13:09', NULL),
(2262104, 'Semua', 'Izinkan kode barang dengan satuan berbeda', 'Tidak', 0, 0, 0, 0, '2026-04-21 05:13:09', NULL),
(2262105, 'Semua', 'Langsung isi nominal total transaksi', 'Tidak', 0, 0, 0, 0, '2026-04-21 05:13:09', NULL),
(2262106, 'Semua', 'Tampilkan informasi stok saat transaksi', 'Iya', 0, 0, 0, 0, '2026-04-21 05:13:09', NULL),
(2262107, 'Semua', 'Navigasi setelah cari nama barang di grid', 'Kolom Nama Bawahnya', 0, 0, 0, 0, '2026-04-21 05:13:09', NULL),
(2272243, 'Semua', 'Sembunyikan pencarian di atas data grid', 'Tidak', 0, 0, 0, 0, '2026-04-26 13:31:18', NULL),
(2273063, 'Semua', 'Auto pilih satuan berdasarkan jumlah qty', 'Iya', 0, 0, 0, 0, '2026-04-28 19:38:09', NULL),
(2273070, 'SYSTEM', 'Batas qty satuan besar (qty >=:)', '6', 0, 0, 0, 0, '2026-04-28 20:35:54', NULL),
(2273686, 'Semua', 'Apakah Mau Backup Sebelum Tutup Aplikasi', 'Tidak', 0, 0, 0, 0, '2026-07-05 11:57:21', NULL),
(2273687, 'Semua', 'Batas qty satuan sedang (qty >=:)', '3', 0, 0, 0, 0, '2026-07-05 11:57:34', NULL),
(2274754, 'Owner', 'MASTER', 'MASTER', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274755, 'Owner', 'MASTER', 'Toko', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274756, 'Owner', 'MASTER', 'Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274757, 'Owner', 'MASTER', 'Harga Beli', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274758, 'Owner', 'MASTER', 'Tambah Stok', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274759, 'Owner', 'MASTER', 'Kurang Stok', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274760, 'Owner', 'MASTER', 'Export Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274761, 'Owner', 'MASTER', 'Import Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274762, 'Owner', 'MASTER', 'Perbaiki Data Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274763, 'Owner', 'MASTER', 'Perbaiki isi satuan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274764, 'Owner', 'MASTER', 'Pelanggan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274765, 'Owner', 'MASTER', 'Supplier', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274766, 'Owner', 'MASTER', 'Tabel Referensi', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274767, 'Owner', 'MASTER', 'Armada', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274768, 'Owner', 'MASTER', 'Karyawan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274769, 'Owner', 'MASTER', 'User', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274770, 'Owner', 'MASTER', 'Hak Akses', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274771, 'Owner', 'MASTER', 'Cabang Master', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274772, 'Owner', 'MASTER', 'Master Poin', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274773, 'Master', 'MASTER', 'MASTER', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274774, 'Master', 'MASTER', 'Toko', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274775, 'Master', 'MASTER', 'Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274776, 'Master', 'MASTER', 'Harga Beli', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274777, 'Master', 'MASTER', 'Tambah Stok', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274778, 'Master', 'MASTER', 'Kurang Stok', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274779, 'Master', 'MASTER', 'Export Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274780, 'Master', 'MASTER', 'Import Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274781, 'Master', 'MASTER', 'Perbaiki Data Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274782, 'Master', 'MASTER', 'Perbaiki isi satuan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274783, 'Master', 'MASTER', 'Pelanggan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274784, 'Master', 'MASTER', 'Supplier', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274785, 'Master', 'MASTER', 'Tabel Referensi', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274786, 'Master', 'MASTER', 'Armada', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274787, 'Master', 'MASTER', 'Karyawan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274788, 'Master', 'MASTER', 'User', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274789, 'Master', 'MASTER', 'Hak Akses', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274790, 'Master', 'MASTER', 'Cabang Master', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274791, 'Master', 'MASTER', 'Master Poin', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274792, 'Admin', 'MASTER', 'MASTER', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274793, 'Admin', 'MASTER', 'Toko', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274794, 'Admin', 'MASTER', 'Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274795, 'Admin', 'MASTER', 'Harga Beli', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274796, 'Admin', 'MASTER', 'Tambah Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274797, 'Admin', 'MASTER', 'Kurang Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274798, 'Admin', 'MASTER', 'Export Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274799, 'Admin', 'MASTER', 'Import Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274800, 'Admin', 'MASTER', 'Perbaiki Data Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274801, 'Admin', 'MASTER', 'Perbaiki isi satuan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274802, 'Admin', 'MASTER', 'Pelanggan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274803, 'Admin', 'MASTER', 'Supplier', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274804, 'Admin', 'MASTER', 'Tabel Referensi', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274805, 'Admin', 'MASTER', 'Armada', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274806, 'Admin', 'MASTER', 'Karyawan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274807, 'Admin', 'MASTER', 'User', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274808, 'Admin', 'MASTER', 'Hak Akses', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274809, 'Admin', 'MASTER', 'Cabang Master', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274810, 'Admin', 'MASTER', 'Master Poin', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274811, 'Kasir', 'MASTER', 'MASTER', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274812, 'Kasir', 'MASTER', 'Toko', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274813, 'Kasir', 'MASTER', 'Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274814, 'Kasir', 'MASTER', 'Harga Beli', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274815, 'Kasir', 'MASTER', 'Tambah Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274816, 'Kasir', 'MASTER', 'Kurang Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274817, 'Kasir', 'MASTER', 'Export Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274818, 'Kasir', 'MASTER', 'Import Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274819, 'Kasir', 'MASTER', 'Perbaiki Data Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274820, 'Kasir', 'MASTER', 'Perbaiki isi satuan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274821, 'Kasir', 'MASTER', 'Pelanggan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274822, 'Kasir', 'MASTER', 'Supplier', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274823, 'Kasir', 'MASTER', 'Tabel Referensi', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274824, 'Kasir', 'MASTER', 'Armada', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274825, 'Kasir', 'MASTER', 'Karyawan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274826, 'Kasir', 'MASTER', 'User', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274827, 'Kasir', 'MASTER', 'Hak Akses', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274828, 'Kasir', 'MASTER', 'Cabang Master', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274829, 'Kasir', 'MASTER', 'Master Poin', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274830, 'Gudang', 'MASTER', 'MASTER', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274831, 'Gudang', 'MASTER', 'Toko', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274832, 'Gudang', 'MASTER', 'Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274833, 'Gudang', 'MASTER', 'Harga Beli', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274834, 'Gudang', 'MASTER', 'Tambah Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274835, 'Gudang', 'MASTER', 'Kurang Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274836, 'Gudang', 'MASTER', 'Export Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274837, 'Gudang', 'MASTER', 'Import Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274838, 'Gudang', 'MASTER', 'Perbaiki Data Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274839, 'Gudang', 'MASTER', 'Perbaiki isi satuan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274840, 'Gudang', 'MASTER', 'Pelanggan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274841, 'Gudang', 'MASTER', 'Supplier', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274842, 'Gudang', 'MASTER', 'Tabel Referensi', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274843, 'Gudang', 'MASTER', 'Armada', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274844, 'Gudang', 'MASTER', 'Karyawan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274845, 'Gudang', 'MASTER', 'User', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274846, 'Gudang', 'MASTER', 'Hak Akses', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274847, 'Gudang', 'MASTER', 'Cabang Master', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274848, 'Gudang', 'MASTER', 'Master Poin', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274849, 'Owner', 'TRANSAKSI', 'TRANSAKSI', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274850, 'Owner', 'TRANSAKSI', 'Pembelian', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274851, 'Owner', 'TRANSAKSI', 'Penjualan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274852, 'Owner', 'TRANSAKSI', 'Retur Pembelian', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274853, 'Owner', 'TRANSAKSI', 'Retur Penjualan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274854, 'Owner', 'TRANSAKSI', 'Bayar Hutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274855, 'Owner', 'TRANSAKSI', 'Bayar Piutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274856, 'Owner', 'TRANSAKSI', 'Transfer Stok', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274857, 'Owner', 'TRANSAKSI', 'Transfer Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274858, 'Owner', 'TRANSAKSI', 'Stok Opname', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274859, 'Owner', 'TRANSAKSI', 'Surat Jalan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274860, 'Owner', 'TRANSAKSI', 'Transfer Cabang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274861, 'Owner', 'TRANSAKSI', 'Sales Order', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274862, 'Owner', 'TRANSAKSI', 'Tukar Poin', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274863, 'Master', 'TRANSAKSI', 'TRANSAKSI', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274864, 'Master', 'TRANSAKSI', 'Pembelian', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274865, 'Master', 'TRANSAKSI', 'Penjualan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274866, 'Master', 'TRANSAKSI', 'Retur Pembelian', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274867, 'Master', 'TRANSAKSI', 'Retur Penjualan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274868, 'Master', 'TRANSAKSI', 'Bayar Hutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274869, 'Master', 'TRANSAKSI', 'Bayar Piutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274870, 'Master', 'TRANSAKSI', 'Transfer Stok', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274871, 'Master', 'TRANSAKSI', 'Transfer Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274872, 'Master', 'TRANSAKSI', 'Stok Opname', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274873, 'Master', 'TRANSAKSI', 'Surat Jalan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274874, 'Master', 'TRANSAKSI', 'Transfer Cabang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274875, 'Master', 'TRANSAKSI', 'Sales Order', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274876, 'Master', 'TRANSAKSI', 'Tukar Poin', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274877, 'Admin', 'TRANSAKSI', 'TRANSAKSI', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274878, 'Admin', 'TRANSAKSI', 'Pembelian', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274879, 'Admin', 'TRANSAKSI', 'Penjualan', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274880, 'Admin', 'TRANSAKSI', 'Retur Pembelian', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274881, 'Admin', 'TRANSAKSI', 'Retur Penjualan', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274882, 'Admin', 'TRANSAKSI', 'Bayar Hutang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274883, 'Admin', 'TRANSAKSI', 'Bayar Piutang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274884, 'Admin', 'TRANSAKSI', 'Transfer Stok', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274885, 'Admin', 'TRANSAKSI', 'Transfer Barang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274886, 'Admin', 'TRANSAKSI', 'Stok Opname', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274887, 'Admin', 'TRANSAKSI', 'Surat Jalan', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274888, 'Admin', 'TRANSAKSI', 'Transfer Cabang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274889, 'Admin', 'TRANSAKSI', 'Sales Order', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274890, 'Admin', 'TRANSAKSI', 'Tukar Poin', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274891, 'Kasir', 'TRANSAKSI', 'TRANSAKSI', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274892, 'Kasir', 'TRANSAKSI', 'Pembelian', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274893, 'Kasir', 'TRANSAKSI', 'Penjualan', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274894, 'Kasir', 'TRANSAKSI', 'Retur Pembelian', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274895, 'Kasir', 'TRANSAKSI', 'Retur Penjualan', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274896, 'Kasir', 'TRANSAKSI', 'Bayar Hutang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274897, 'Kasir', 'TRANSAKSI', 'Bayar Piutang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274898, 'Kasir', 'TRANSAKSI', 'Transfer Stok', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274899, 'Kasir', 'TRANSAKSI', 'Transfer Barang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274900, 'Kasir', 'TRANSAKSI', 'Stok Opname', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274901, 'Kasir', 'TRANSAKSI', 'Surat Jalan', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274902, 'Kasir', 'TRANSAKSI', 'Transfer Cabang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274903, 'Kasir', 'TRANSAKSI', 'Sales Order', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274904, 'Kasir', 'TRANSAKSI', 'Tukar Poin', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274905, 'Gudang', 'TRANSAKSI', 'TRANSAKSI', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274906, 'Gudang', 'TRANSAKSI', 'Pembelian', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274907, 'Gudang', 'TRANSAKSI', 'Penjualan', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274908, 'Gudang', 'TRANSAKSI', 'Retur Pembelian', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274909, 'Gudang', 'TRANSAKSI', 'Retur Penjualan', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274910, 'Gudang', 'TRANSAKSI', 'Bayar Hutang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL);
INSERT INTO `hakaksesuser` (`NO`, `UserName`, `Role`, `ModuleName`, `CanRead`, `CanAdd`, `CanEdit`, `CanDelete`, `created_at`, `sync_id`) VALUES
(2274911, 'Gudang', 'TRANSAKSI', 'Bayar Piutang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274912, 'Gudang', 'TRANSAKSI', 'Transfer Stok', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274913, 'Gudang', 'TRANSAKSI', 'Transfer Barang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274914, 'Gudang', 'TRANSAKSI', 'Stok Opname', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274915, 'Gudang', 'TRANSAKSI', 'Surat Jalan', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274916, 'Gudang', 'TRANSAKSI', 'Transfer Cabang', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274917, 'Gudang', 'TRANSAKSI', 'Sales Order', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274918, 'Gudang', 'TRANSAKSI', 'Tukar Poin', 1, 1, 0, 0, '2026-10-02 18:56:23', NULL),
(2274919, 'Owner', 'JURNAL', 'JURNAL', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274920, 'Master', 'JURNAL', 'JURNAL', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274921, 'Admin', 'JURNAL', 'JURNAL', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274922, 'Kasir', 'JURNAL', 'JURNAL', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274923, 'Gudang', 'JURNAL', 'JURNAL', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274924, 'Owner', 'MENUKARYAWAN', 'MENUKARYAWAN', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274925, 'Owner', 'MENUKARYAWAN', 'Master gaji', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274926, 'Owner', 'MENUKARYAWAN', 'Bon', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274927, 'Owner', 'MENUKARYAWAN', 'Bayar', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274928, 'Owner', 'MENUKARYAWAN', 'Lap bon', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274929, 'Owner', 'MENUKARYAWAN', 'Lap bon karyawan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274930, 'Owner', 'MENUKARYAWAN', 'Gaji', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274931, 'Owner', 'MENUKARYAWAN', 'Lap Gaji', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274932, 'Master', 'MENUKARYAWAN', 'MENUKARYAWAN', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274933, 'Master', 'MENUKARYAWAN', 'Master gaji', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274934, 'Master', 'MENUKARYAWAN', 'Bon', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274935, 'Master', 'MENUKARYAWAN', 'Bayar', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274936, 'Master', 'MENUKARYAWAN', 'Lap bon', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274937, 'Master', 'MENUKARYAWAN', 'Lap bon karyawan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274938, 'Master', 'MENUKARYAWAN', 'Gaji', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274939, 'Master', 'MENUKARYAWAN', 'Lap Gaji', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274940, 'Admin', 'MENUKARYAWAN', 'MENUKARYAWAN', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274941, 'Admin', 'MENUKARYAWAN', 'Master gaji', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274942, 'Admin', 'MENUKARYAWAN', 'Bon', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274943, 'Admin', 'MENUKARYAWAN', 'Bayar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274944, 'Admin', 'MENUKARYAWAN', 'Lap bon', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274945, 'Admin', 'MENUKARYAWAN', 'Lap bon karyawan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274946, 'Admin', 'MENUKARYAWAN', 'Gaji', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274947, 'Admin', 'MENUKARYAWAN', 'Lap Gaji', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274948, 'Kasir', 'MENUKARYAWAN', 'MENUKARYAWAN', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274949, 'Kasir', 'MENUKARYAWAN', 'Master gaji', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274950, 'Kasir', 'MENUKARYAWAN', 'Bon', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274951, 'Kasir', 'MENUKARYAWAN', 'Bayar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274952, 'Kasir', 'MENUKARYAWAN', 'Lap bon', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274953, 'Kasir', 'MENUKARYAWAN', 'Lap bon karyawan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274954, 'Kasir', 'MENUKARYAWAN', 'Gaji', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274955, 'Kasir', 'MENUKARYAWAN', 'Lap Gaji', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274956, 'Gudang', 'MENUKARYAWAN', 'MENUKARYAWAN', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274957, 'Gudang', 'MENUKARYAWAN', 'Master gaji', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274958, 'Gudang', 'MENUKARYAWAN', 'Bon', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274959, 'Gudang', 'MENUKARYAWAN', 'Bayar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274960, 'Gudang', 'MENUKARYAWAN', 'Lap bon', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274961, 'Gudang', 'MENUKARYAWAN', 'Lap bon karyawan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274962, 'Gudang', 'MENUKARYAWAN', 'Gaji', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274963, 'Gudang', 'MENUKARYAWAN', 'Lap Gaji', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2274964, 'Owner', 'LAPORAN', 'LAPORAN', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274965, 'Owner', 'LAPORAN', 'Mutasi saldo', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274966, 'Owner', 'LAPORAN', 'Mutasi barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274967, 'Owner', 'LAPORAN', 'Jurnal Umum', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274968, 'Owner', 'LAPORAN', 'Neraca', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274969, 'Owner', 'LAPORAN', 'Buku Besar', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274970, 'Owner', 'LAPORAN', 'Buku Besar Pembantu', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274971, 'Owner', 'LAPORAN', 'Lap Pembelian', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274972, 'Owner', 'LAPORAN', 'Lap Pembelian Detail', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274973, 'Owner', 'LAPORAN', 'Lap Pembelian Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274974, 'Owner', 'LAPORAN', 'Lap Pembelian Hutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274975, 'Owner', 'LAPORAN', 'Rekap Penjualan Nota', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274976, 'Owner', 'LAPORAN', 'Rekap Penjualan Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274977, 'Owner', 'LAPORAN', 'Lap Penjualan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274978, 'Owner', 'LAPORAN', 'Lap Penjualan Detail', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274979, 'Owner', 'LAPORAN', 'Lap Penjualan Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274980, 'Owner', 'LAPORAN', 'Lap Penjualan Hutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274981, 'Owner', 'LAPORAN', 'Lap Penjualan Sales', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274982, 'Owner', 'LAPORAN', 'Lap Penjualan Qty', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274983, 'Owner', 'LAPORAN', 'Jual PPnNonPPn', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274984, 'Owner', 'LAPORAN', 'Retur Beli', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274985, 'Owner', 'LAPORAN', 'Retur Beli Detail', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274986, 'Owner', 'LAPORAN', 'Retur Beli Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274987, 'Owner', 'LAPORAN', 'Retur Jual', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274988, 'Owner', 'LAPORAN', 'Retur Jual Detail', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274989, 'Owner', 'LAPORAN', 'Retur Jual Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274990, 'Owner', 'LAPORAN', 'Hutang By Pembelian', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274991, 'Owner', 'LAPORAN', 'Hutang By Pelunasan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274992, 'Owner', 'LAPORAN', 'Hutang By Jatuh Tempo', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274993, 'Owner', 'LAPORAN', 'Rekap Bayar Hutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274994, 'Owner', 'LAPORAN', 'Piutang By Penjualan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274995, 'Owner', 'LAPORAN', 'Piutang By Pelunasan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274996, 'Owner', 'LAPORAN', 'Piutang By Jatuh Tempo', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274997, 'Owner', 'LAPORAN', 'Rekap Bayar Piutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274998, 'Owner', 'LAPORAN', 'Kas Penjualan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2274999, 'Owner', 'LAPORAN', 'Lap Transfer Stok', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275000, 'Owner', 'LAPORAN', 'Lap Transfer Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275001, 'Owner', 'LAPORAN', 'Lap Transfer Barang Detail', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275002, 'Owner', 'LAPORAN', 'Lap Stok Opname', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275003, 'Owner', 'LAPORAN', 'Stok Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275004, 'Owner', 'LAPORAN', 'Kartu Stok', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275005, 'Owner', 'LAPORAN', 'Barang Terlaris', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275006, 'Owner', 'LAPORAN', 'Barang Tidak Bergerak', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275007, 'Owner', 'LAPORAN', 'Stok Minimum', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275008, 'Owner', 'LAPORAN', 'Stok Masa Lampau', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275009, 'Owner', 'LAPORAN', 'Ranking Supplier', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275010, 'Owner', 'LAPORAN', 'Ranking Kasir', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275011, 'Owner', 'LAPORAN', 'Ranking Barang Terbanyak Dibeli', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275012, 'Owner', 'LAPORAN', 'Ranking Pelanggan Piutang Terbesar', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275013, 'Owner', 'LAPORAN', 'Ranking Supplier Hutang Terbesar', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275014, 'Owner', 'LAPORAN', 'Omset Per Pelanggan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275015, 'Owner', 'LAPORAN', 'Omset Per Kategori', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275016, 'Owner', 'LAPORAN', 'Grafik', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275017, 'Owner', 'LAPORAN', 'History', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275018, 'Master', 'LAPORAN', 'LAPORAN', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275019, 'Master', 'LAPORAN', 'Mutasi saldo', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275020, 'Master', 'LAPORAN', 'Mutasi barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275021, 'Master', 'LAPORAN', 'Jurnal Umum', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275022, 'Master', 'LAPORAN', 'Neraca', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275023, 'Master', 'LAPORAN', 'Buku Besar', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275024, 'Master', 'LAPORAN', 'Buku Besar Pembantu', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275025, 'Master', 'LAPORAN', 'Lap Pembelian', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275026, 'Master', 'LAPORAN', 'Lap Pembelian Detail', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275027, 'Master', 'LAPORAN', 'Lap Pembelian Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275028, 'Master', 'LAPORAN', 'Lap Pembelian Hutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275029, 'Master', 'LAPORAN', 'Rekap Penjualan Nota', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275030, 'Master', 'LAPORAN', 'Rekap Penjualan Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275031, 'Master', 'LAPORAN', 'Lap Penjualan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275032, 'Master', 'LAPORAN', 'Lap Penjualan Detail', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275033, 'Master', 'LAPORAN', 'Lap Penjualan Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275034, 'Master', 'LAPORAN', 'Lap Penjualan Hutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275035, 'Master', 'LAPORAN', 'Lap Penjualan Sales', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275036, 'Master', 'LAPORAN', 'Lap Penjualan Qty', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275037, 'Master', 'LAPORAN', 'Jual PPnNonPPn', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275038, 'Master', 'LAPORAN', 'Retur Beli', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275039, 'Master', 'LAPORAN', 'Retur Beli Detail', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275040, 'Master', 'LAPORAN', 'Retur Beli Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275041, 'Master', 'LAPORAN', 'Retur Jual', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275042, 'Master', 'LAPORAN', 'Retur Jual Detail', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275043, 'Master', 'LAPORAN', 'Retur Jual Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275044, 'Master', 'LAPORAN', 'Hutang By Pembelian', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275045, 'Master', 'LAPORAN', 'Hutang By Pelunasan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275046, 'Master', 'LAPORAN', 'Hutang By Jatuh Tempo', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275047, 'Master', 'LAPORAN', 'Rekap Bayar Hutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275048, 'Master', 'LAPORAN', 'Piutang By Penjualan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275049, 'Master', 'LAPORAN', 'Piutang By Pelunasan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275050, 'Master', 'LAPORAN', 'Piutang By Jatuh Tempo', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275051, 'Master', 'LAPORAN', 'Rekap Bayar Piutang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275052, 'Master', 'LAPORAN', 'Kas Penjualan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275053, 'Master', 'LAPORAN', 'Lap Transfer Stok', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275054, 'Master', 'LAPORAN', 'Lap Transfer Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275055, 'Master', 'LAPORAN', 'Lap Transfer Barang Detail', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275056, 'Master', 'LAPORAN', 'Lap Stok Opname', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275057, 'Master', 'LAPORAN', 'Stok Barang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275058, 'Master', 'LAPORAN', 'Kartu Stok', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275059, 'Master', 'LAPORAN', 'Barang Terlaris', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275060, 'Master', 'LAPORAN', 'Barang Tidak Bergerak', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275061, 'Master', 'LAPORAN', 'Stok Minimum', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275062, 'Master', 'LAPORAN', 'Stok Masa Lampau', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275063, 'Master', 'LAPORAN', 'Ranking Supplier', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275064, 'Master', 'LAPORAN', 'Ranking Kasir', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275065, 'Master', 'LAPORAN', 'Ranking Barang Terbanyak Dibeli', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275066, 'Master', 'LAPORAN', 'Ranking Pelanggan Piutang Terbesar', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275067, 'Master', 'LAPORAN', 'Ranking Supplier Hutang Terbesar', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275068, 'Master', 'LAPORAN', 'Omset Per Pelanggan', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275069, 'Master', 'LAPORAN', 'Omset Per Kategori', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275070, 'Master', 'LAPORAN', 'Grafik', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275071, 'Master', 'LAPORAN', 'History', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275072, 'Admin', 'LAPORAN', 'LAPORAN', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275073, 'Admin', 'LAPORAN', 'Mutasi saldo', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275074, 'Admin', 'LAPORAN', 'Mutasi barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275075, 'Admin', 'LAPORAN', 'Jurnal Umum', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275076, 'Admin', 'LAPORAN', 'Neraca', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275077, 'Admin', 'LAPORAN', 'Buku Besar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275078, 'Admin', 'LAPORAN', 'Buku Besar Pembantu', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275079, 'Admin', 'LAPORAN', 'Lap Pembelian', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275080, 'Admin', 'LAPORAN', 'Lap Pembelian Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275081, 'Admin', 'LAPORAN', 'Lap Pembelian Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275082, 'Admin', 'LAPORAN', 'Lap Pembelian Hutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275083, 'Admin', 'LAPORAN', 'Rekap Penjualan Nota', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275084, 'Admin', 'LAPORAN', 'Rekap Penjualan Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275085, 'Admin', 'LAPORAN', 'Lap Penjualan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275086, 'Admin', 'LAPORAN', 'Lap Penjualan Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275087, 'Admin', 'LAPORAN', 'Lap Penjualan Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275088, 'Admin', 'LAPORAN', 'Lap Penjualan Hutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275089, 'Admin', 'LAPORAN', 'Lap Penjualan Sales', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275090, 'Admin', 'LAPORAN', 'Lap Penjualan Qty', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275091, 'Admin', 'LAPORAN', 'Jual PPnNonPPn', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275092, 'Admin', 'LAPORAN', 'Retur Beli', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275093, 'Admin', 'LAPORAN', 'Retur Beli Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275094, 'Admin', 'LAPORAN', 'Retur Beli Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275095, 'Admin', 'LAPORAN', 'Retur Jual', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275096, 'Admin', 'LAPORAN', 'Retur Jual Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275097, 'Admin', 'LAPORAN', 'Retur Jual Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275098, 'Admin', 'LAPORAN', 'Hutang By Pembelian', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275099, 'Admin', 'LAPORAN', 'Hutang By Pelunasan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275100, 'Admin', 'LAPORAN', 'Hutang By Jatuh Tempo', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275101, 'Admin', 'LAPORAN', 'Rekap Bayar Hutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275102, 'Admin', 'LAPORAN', 'Piutang By Penjualan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275103, 'Admin', 'LAPORAN', 'Piutang By Pelunasan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275104, 'Admin', 'LAPORAN', 'Piutang By Jatuh Tempo', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275105, 'Admin', 'LAPORAN', 'Rekap Bayar Piutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275106, 'Admin', 'LAPORAN', 'Kas Penjualan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275107, 'Admin', 'LAPORAN', 'Lap Transfer Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275108, 'Admin', 'LAPORAN', 'Lap Transfer Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275109, 'Admin', 'LAPORAN', 'Lap Transfer Barang Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275110, 'Admin', 'LAPORAN', 'Lap Stok Opname', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275111, 'Admin', 'LAPORAN', 'Stok Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275112, 'Admin', 'LAPORAN', 'Kartu Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275113, 'Admin', 'LAPORAN', 'Barang Terlaris', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275114, 'Admin', 'LAPORAN', 'Barang Tidak Bergerak', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275115, 'Admin', 'LAPORAN', 'Stok Minimum', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275116, 'Admin', 'LAPORAN', 'Stok Masa Lampau', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275117, 'Admin', 'LAPORAN', 'Ranking Supplier', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275118, 'Admin', 'LAPORAN', 'Ranking Kasir', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275119, 'Admin', 'LAPORAN', 'Ranking Barang Terbanyak Dibeli', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275120, 'Admin', 'LAPORAN', 'Ranking Pelanggan Piutang Terbesar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275121, 'Admin', 'LAPORAN', 'Ranking Supplier Hutang Terbesar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275122, 'Admin', 'LAPORAN', 'Omset Per Pelanggan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275123, 'Admin', 'LAPORAN', 'Omset Per Kategori', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275124, 'Admin', 'LAPORAN', 'Grafik', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275125, 'Admin', 'LAPORAN', 'History', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275126, 'Kasir', 'LAPORAN', 'LAPORAN', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275127, 'Kasir', 'LAPORAN', 'Mutasi saldo', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275128, 'Kasir', 'LAPORAN', 'Mutasi barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275129, 'Kasir', 'LAPORAN', 'Jurnal Umum', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275130, 'Kasir', 'LAPORAN', 'Neraca', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275131, 'Kasir', 'LAPORAN', 'Buku Besar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275132, 'Kasir', 'LAPORAN', 'Buku Besar Pembantu', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275133, 'Kasir', 'LAPORAN', 'Lap Pembelian', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275134, 'Kasir', 'LAPORAN', 'Lap Pembelian Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275135, 'Kasir', 'LAPORAN', 'Lap Pembelian Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275136, 'Kasir', 'LAPORAN', 'Lap Pembelian Hutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275137, 'Kasir', 'LAPORAN', 'Rekap Penjualan Nota', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275138, 'Kasir', 'LAPORAN', 'Rekap Penjualan Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275139, 'Kasir', 'LAPORAN', 'Lap Penjualan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275140, 'Kasir', 'LAPORAN', 'Lap Penjualan Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275141, 'Kasir', 'LAPORAN', 'Lap Penjualan Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275142, 'Kasir', 'LAPORAN', 'Lap Penjualan Hutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275143, 'Kasir', 'LAPORAN', 'Lap Penjualan Sales', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275144, 'Kasir', 'LAPORAN', 'Lap Penjualan Qty', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275145, 'Kasir', 'LAPORAN', 'Jual PPnNonPPn', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275146, 'Kasir', 'LAPORAN', 'Retur Beli', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275147, 'Kasir', 'LAPORAN', 'Retur Beli Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275148, 'Kasir', 'LAPORAN', 'Retur Beli Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275149, 'Kasir', 'LAPORAN', 'Retur Jual', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275150, 'Kasir', 'LAPORAN', 'Retur Jual Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275151, 'Kasir', 'LAPORAN', 'Retur Jual Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275152, 'Kasir', 'LAPORAN', 'Hutang By Pembelian', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275153, 'Kasir', 'LAPORAN', 'Hutang By Pelunasan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275154, 'Kasir', 'LAPORAN', 'Hutang By Jatuh Tempo', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275155, 'Kasir', 'LAPORAN', 'Rekap Bayar Hutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275156, 'Kasir', 'LAPORAN', 'Piutang By Penjualan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275157, 'Kasir', 'LAPORAN', 'Piutang By Pelunasan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275158, 'Kasir', 'LAPORAN', 'Piutang By Jatuh Tempo', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275159, 'Kasir', 'LAPORAN', 'Rekap Bayar Piutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275160, 'Kasir', 'LAPORAN', 'Kas Penjualan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275161, 'Kasir', 'LAPORAN', 'Lap Transfer Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275162, 'Kasir', 'LAPORAN', 'Lap Transfer Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275163, 'Kasir', 'LAPORAN', 'Lap Transfer Barang Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275164, 'Kasir', 'LAPORAN', 'Lap Stok Opname', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275165, 'Kasir', 'LAPORAN', 'Stok Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275166, 'Kasir', 'LAPORAN', 'Kartu Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275167, 'Kasir', 'LAPORAN', 'Barang Terlaris', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275168, 'Kasir', 'LAPORAN', 'Barang Tidak Bergerak', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275169, 'Kasir', 'LAPORAN', 'Stok Minimum', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275170, 'Kasir', 'LAPORAN', 'Stok Masa Lampau', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275171, 'Kasir', 'LAPORAN', 'Ranking Supplier', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275172, 'Kasir', 'LAPORAN', 'Ranking Kasir', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275173, 'Kasir', 'LAPORAN', 'Ranking Barang Terbanyak Dibeli', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275174, 'Kasir', 'LAPORAN', 'Ranking Pelanggan Piutang Terbesar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275175, 'Kasir', 'LAPORAN', 'Ranking Supplier Hutang Terbesar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275176, 'Kasir', 'LAPORAN', 'Omset Per Pelanggan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275177, 'Kasir', 'LAPORAN', 'Omset Per Kategori', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275178, 'Kasir', 'LAPORAN', 'Grafik', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275179, 'Kasir', 'LAPORAN', 'History', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275180, 'Gudang', 'LAPORAN', 'LAPORAN', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275181, 'Gudang', 'LAPORAN', 'Mutasi saldo', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275182, 'Gudang', 'LAPORAN', 'Mutasi barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275183, 'Gudang', 'LAPORAN', 'Jurnal Umum', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275184, 'Gudang', 'LAPORAN', 'Neraca', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275185, 'Gudang', 'LAPORAN', 'Buku Besar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275186, 'Gudang', 'LAPORAN', 'Buku Besar Pembantu', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275187, 'Gudang', 'LAPORAN', 'Lap Pembelian', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275188, 'Gudang', 'LAPORAN', 'Lap Pembelian Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275189, 'Gudang', 'LAPORAN', 'Lap Pembelian Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275190, 'Gudang', 'LAPORAN', 'Lap Pembelian Hutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275191, 'Gudang', 'LAPORAN', 'Rekap Penjualan Nota', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275192, 'Gudang', 'LAPORAN', 'Rekap Penjualan Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275193, 'Gudang', 'LAPORAN', 'Lap Penjualan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275194, 'Gudang', 'LAPORAN', 'Lap Penjualan Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275195, 'Gudang', 'LAPORAN', 'Lap Penjualan Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275196, 'Gudang', 'LAPORAN', 'Lap Penjualan Hutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275197, 'Gudang', 'LAPORAN', 'Lap Penjualan Sales', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275198, 'Gudang', 'LAPORAN', 'Lap Penjualan Qty', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275199, 'Gudang', 'LAPORAN', 'Jual PPnNonPPn', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275200, 'Gudang', 'LAPORAN', 'Retur Beli', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275201, 'Gudang', 'LAPORAN', 'Retur Beli Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275202, 'Gudang', 'LAPORAN', 'Retur Beli Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275203, 'Gudang', 'LAPORAN', 'Retur Jual', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275204, 'Gudang', 'LAPORAN', 'Retur Jual Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275205, 'Gudang', 'LAPORAN', 'Retur Jual Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275206, 'Gudang', 'LAPORAN', 'Hutang By Pembelian', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275207, 'Gudang', 'LAPORAN', 'Hutang By Pelunasan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275208, 'Gudang', 'LAPORAN', 'Hutang By Jatuh Tempo', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275209, 'Gudang', 'LAPORAN', 'Rekap Bayar Hutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275210, 'Gudang', 'LAPORAN', 'Piutang By Penjualan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275211, 'Gudang', 'LAPORAN', 'Piutang By Pelunasan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275212, 'Gudang', 'LAPORAN', 'Piutang By Jatuh Tempo', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275213, 'Gudang', 'LAPORAN', 'Rekap Bayar Piutang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275214, 'Gudang', 'LAPORAN', 'Kas Penjualan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275215, 'Gudang', 'LAPORAN', 'Lap Transfer Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275216, 'Gudang', 'LAPORAN', 'Lap Transfer Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275217, 'Gudang', 'LAPORAN', 'Lap Transfer Barang Detail', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275218, 'Gudang', 'LAPORAN', 'Lap Stok Opname', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275219, 'Gudang', 'LAPORAN', 'Stok Barang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275220, 'Gudang', 'LAPORAN', 'Kartu Stok', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275221, 'Gudang', 'LAPORAN', 'Barang Terlaris', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275222, 'Gudang', 'LAPORAN', 'Barang Tidak Bergerak', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275223, 'Gudang', 'LAPORAN', 'Stok Minimum', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275224, 'Gudang', 'LAPORAN', 'Stok Masa Lampau', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275225, 'Gudang', 'LAPORAN', 'Ranking Supplier', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275226, 'Gudang', 'LAPORAN', 'Ranking Kasir', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275227, 'Gudang', 'LAPORAN', 'Ranking Barang Terbanyak Dibeli', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275228, 'Gudang', 'LAPORAN', 'Ranking Pelanggan Piutang Terbesar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275229, 'Gudang', 'LAPORAN', 'Ranking Supplier Hutang Terbesar', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275230, 'Gudang', 'LAPORAN', 'Omset Per Pelanggan', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275231, 'Gudang', 'LAPORAN', 'Omset Per Kategori', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275232, 'Gudang', 'LAPORAN', 'Grafik', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275233, 'Gudang', 'LAPORAN', 'History', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275234, 'Owner', 'UTILITY', 'UTILITY', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275235, 'Owner', 'UTILITY', 'Database', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275236, 'Owner', 'UTILITY', 'Backup Database', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275237, 'Owner', 'UTILITY', 'Restore Database', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275238, 'Owner', 'UTILITY', 'Perbaiki Database', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275239, 'Owner', 'UTILITY', 'Setting Printer', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275240, 'Master', 'UTILITY', 'UTILITY', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275241, 'Master', 'UTILITY', 'Database', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275242, 'Master', 'UTILITY', 'Backup Database', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275243, 'Master', 'UTILITY', 'Restore Database', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275244, 'Master', 'UTILITY', 'Perbaiki Database', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275245, 'Master', 'UTILITY', 'Setting Printer', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275246, 'Admin', 'UTILITY', 'UTILITY', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275247, 'Admin', 'UTILITY', 'Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275248, 'Admin', 'UTILITY', 'Backup Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275249, 'Admin', 'UTILITY', 'Restore Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275250, 'Admin', 'UTILITY', 'Perbaiki Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275251, 'Admin', 'UTILITY', 'Setting Printer', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275252, 'Kasir', 'UTILITY', 'UTILITY', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275253, 'Kasir', 'UTILITY', 'Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275254, 'Kasir', 'UTILITY', 'Backup Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275255, 'Kasir', 'UTILITY', 'Restore Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275256, 'Kasir', 'UTILITY', 'Perbaiki Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275257, 'Kasir', 'UTILITY', 'Setting Printer', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275258, 'Gudang', 'UTILITY', 'UTILITY', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275259, 'Gudang', 'UTILITY', 'Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275260, 'Gudang', 'UTILITY', 'Backup Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275261, 'Gudang', 'UTILITY', 'Restore Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275262, 'Gudang', 'UTILITY', 'Perbaiki Database', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275263, 'Gudang', 'UTILITY', 'Setting Printer', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275264, 'Owner', 'POSTING', 'POSTING', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275265, 'Owner', 'POSTING', 'Posting Toko', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275266, 'Owner', 'POSTING', 'Posting Gudang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275267, 'Owner', 'POSTING', 'Posting Semua', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275268, 'Master', 'POSTING', 'POSTING', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275269, 'Master', 'POSTING', 'Posting Toko', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275270, 'Master', 'POSTING', 'Posting Gudang', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275271, 'Master', 'POSTING', 'Posting Semua', 1, 1, 1, 1, '2026-10-02 18:56:23', NULL),
(2275272, 'Admin', 'POSTING', 'POSTING', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275273, 'Admin', 'POSTING', 'Posting Toko', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275274, 'Admin', 'POSTING', 'Posting Gudang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275275, 'Admin', 'POSTING', 'Posting Semua', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275276, 'Kasir', 'POSTING', 'POSTING', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275277, 'Kasir', 'POSTING', 'Posting Toko', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275278, 'Kasir', 'POSTING', 'Posting Gudang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275279, 'Kasir', 'POSTING', 'Posting Semua', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275280, 'Gudang', 'POSTING', 'POSTING', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275281, 'Gudang', 'POSTING', 'Posting Toko', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275282, 'Gudang', 'POSTING', 'Posting Gudang', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL),
(2275283, 'Gudang', 'POSTING', 'Posting Semua', 0, 0, 0, 0, '2026-10-02 18:56:23', NULL);

-- --------------------------------------------------------

--
-- Struktur dari tabel `history`
--

CREATE TABLE `history` (
  `NO` int(11) NOT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `Aksi` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `historybarang`
--

CREATE TABLE `historybarang` (
  `NO` int(11) NOT NULL,
  `FAKTUR` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `JENIS` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) DEFAULT '0',
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `TOTAL_RUPIAH` decimal(15,0) DEFAULT '0',
  `ID_USER` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `hutang`
--

CREATE TABLE `hutang` (
  `NOBAYARHUTANG` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODESUPLIYER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMASUPLIYER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGLPEMBAYARAN` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TOTALHUTANG` decimal(15,0) DEFAULT '0',
  `NOMINALBAYAR` decimal(15,0) DEFAULT '0',
  `SISAHUTANG` decimal(15,0) DEFAULT '0',
  `ID_USER_BAYAR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER_BAYAR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `hutang_detail`
--

CREATE TABLE `hutang_detail` (
  `ID_BAYAR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_BAYAR` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BELI` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'BAYAR',
  `TANGGAL_BELI` datetime DEFAULT NULL,
  `TOTAL_HUTANG` decimal(15,0) DEFAULT '0',
  `DIBAYAR` decimal(15,0) DEFAULT '0',
  `RETUR` decimal(15,0) DEFAULT '0',
  `HUTANG` decimal(15,0) DEFAULT '0',
  `JATUH_TEMPO` datetime DEFAULT NULL,
  `PEMBAYARAN` decimal(15,0) DEFAULT '0',
  `STATUS` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `jurnalumum`
--

CREATE TABLE `jurnalumum` (
  `NO` int(11) NOT NULL,
  `NO_TRANSAKSI` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_TRANSAKSI` datetime DEFAULT NULL,
  `NO_NOTA` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URAIAN` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_D` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_AKUN_D` varchar(40) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOMOR_AKUN_D` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_K` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_AKUN_K` varchar(40) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOMOR_AKUN_K` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BANTU_D` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_BANTU_D` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BANTU_K` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_BANTU_K` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOMINAL` decimal(20,0) DEFAULT '0',
  `JENIS_TRANSAKSI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data untuk tabel `jurnalumum`
--

INSERT INTO `jurnalumum` (`NO`, `NO_TRANSAKSI`, `TGL_TRANSAKSI`, `NO_NOTA`, `URAIAN`, `AKUN_D`, `NAMA_AKUN_D`, `NOMOR_AKUN_D`, `AKUN_K`, `NAMA_AKUN_K`, `NOMOR_AKUN_K`, `NAMA_BANTU_D`, `KODE_BANTU_D`, `NAMA_BANTU_K`, `KODE_BANTU_K`, `NOMINAL`, `JENIS_TRANSAKSI`, `LOKASI`, `ID_USER`, `ID_KOMPUTER`, `created_at`, `sync_id`) VALUES
(1, 'ADJ-20261002-001', '2026-10-02 18:53:51', 'PENYESUAIAN', 'Koreksi persediaan ke nilai fisik stok', NULL, '', '', NULL, 'PERSEDIAAN BARANG', '01.04.001', NULL, NULL, NULL, NULL, NULL, 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI', '2026-10-02 18:53:51', NULL),
(2, 'ADJ-20261002-002', '2026-10-02 18:53:51', 'PENYESUAIAN', 'Koreksi penyesuaian stok historis skema lama', NULL, 'PENYESUAIAN STOK MINUS', '06.04.001', NULL, '', '', NULL, NULL, NULL, NULL, '0', 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI', '2026-10-02 18:53:51', NULL),
(3, 'ADJ-20261002-003', '2026-10-02 18:53:51', 'PENYESUAIAN', 'Koreksi HPP skema jurnal lama', NULL, 'HPP POKOK PENJUALAN', '06.01.001', NULL, '', '', NULL, NULL, NULL, NULL, '0', 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI', '2026-10-02 18:53:51', NULL),
(4, 'ADJ-20261002-004', '2026-10-02 18:53:51', 'PENYESUAIAN', 'Koreksi prive akibat skema jurnal lama', NULL, 'PRIVE PEMILIK', '04.02.001', NULL, '', '', NULL, NULL, NULL, NULL, '0', 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI', '2026-10-02 18:53:51', NULL),
(5, 'ADJ-20261002-001', '2026-10-02 18:53:51', 'PENYESUAIAN', 'Koreksi persediaan barang ke nilai fisik stok opname', NULL, 'PENYESUAIAN STOK MINUS', '06.04.001', NULL, 'PERSEDIAAN BARANG', '01.04.001', NULL, NULL, NULL, NULL, NULL, 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI', '2026-10-02 18:53:51', NULL),
(6, 'ADJ-20261002-002', '2026-10-02 18:53:51', 'PENYESUAIAN', 'Koreksi modal akibat distorsi stok opname historis skema lama', NULL, 'MODAL', '04.01.001', NULL, 'PENYESUAIAN STOK MINUS', '06.04.001', NULL, NULL, NULL, NULL, NULL, 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI', '2026-10-02 18:53:51', NULL),
(7, 'ADJ-20261002-003', '2026-10-02 18:53:51', 'PENYESUAIAN', 'Koreksi modal akibat distorsi HPP skema jurnal lama', NULL, 'MODAL', '04.01.001', NULL, 'HPP POKOK PENJUALAN', '06.01.001', NULL, NULL, NULL, NULL, '0', 'PINDAH REKENING', 'SEMUA', 'SYSTEM', 'MIGRASI', '2026-10-02 18:53:51', NULL);

-- --------------------------------------------------------

--
-- Struktur dari tabel `pembelian`
--

CREATE TABLE `pembelian` (
  `ID_PEMBELIAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ID_SUPPLIER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_PEMBELIAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_BELI` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_BAYAR` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_AKUN_TF` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `NAMA_AKUN_TF` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `GRAND_TOTAL_BELI` decimal(15,0) DEFAULT '0',
  `DISKON_SUPPLIER` decimal(15,2) NOT NULL DEFAULT '0.00',
  `PPN_MASUKAN` decimal(15,2) NOT NULL DEFAULT '0.00',
  `BIAYA_KIRIM` decimal(15,2) NOT NULL DEFAULT '0.00',
  `BIAYA_LAIN` decimal(15,2) NOT NULL DEFAULT '0.00',
  `KODE_AKUN_BIAYA_LAIN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `NAMA_AKUN_BIAYA_LAIN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `TOTAL_BARANG` decimal(15,2) DEFAULT '0.00',
  `PEMBAYARAN` decimal(15,0) DEFAULT '0',
  `NOMINAL_TRANSFER` decimal(15,2) DEFAULT '0.00',
  `TGL_RETUR` datetime DEFAULT NULL,
  `RETUR` decimal(15,0) DEFAULT '0',
  `TAGIHAN` decimal(15,0) DEFAULT '0',
  `JATUH_TEMPO` datetime DEFAULT NULL,
  `TGL_BAYAR` datetime DEFAULT NULL,
  `NOMINALBAYAR` decimal(15,0) DEFAULT '0',
  `STATUS_JUAL` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `STATUS_TRANSAKSI_BELI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `pembelian_detail`
--

CREATE TABLE `pembelian_detail` (
  `NO` int(11) NOT NULL,
  `FAKTUR_BELI` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_BELI` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_MASUK` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_SUPLIYER` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA_BELI` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `HARGA_AVERAGE` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `HARGA_BELI_SEBELUMNYA` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) NOT NULL DEFAULT '1',
  `HARGA_BELI_SATUAN` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `QTY_SAT` decimal(15,4) DEFAULT '0.0000',
  `TOTAL` decimal(15,0) DEFAULT '0',
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URUTAN` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `pembelian_ditahan`
--

CREATE TABLE `pembelian_ditahan` (
  `ID_PEMBELIAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ID_SUPPLIER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_PEMBELIAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_BELI` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_BAYAR` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GRAND_TOTAL_BELI` decimal(15,0) DEFAULT NULL,
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `TOTAL_BARANG` decimal(15,2) DEFAULT '0.00',
  `TGL_RETUR` datetime DEFAULT NULL,
  `RETUR` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `pembelian_ditahan_detail`
--

CREATE TABLE `pembelian_ditahan_detail` (
  `FAKTUR_BELI` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_BELI` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_MASUK` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_SUPLIYER` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA_BELI` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `HARGA_AVERAGE` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `HARGA_BELI_SEBELUMNYA` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) NOT NULL DEFAULT '1',
  `HARGA_BELI_SATUAN` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `QTY_SAT` decimal(15,4) DEFAULT '0.0000',
  `TOTAL` decimal(15,2) DEFAULT '0.00',
  `URUTAN` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `penjualan`
--

CREATE TABLE `penjualan` (
  `ID_PENJUALAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NO_SO` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PELANGGAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT_PELANGGAN` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASIBARANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_TRANSAKSI` datetime DEFAULT NULL,
  `TOTAL_HPP` decimal(15,2) DEFAULT '0.00',
  `GRAND_TOTAL_SBL_PAJAK` decimal(15,2) DEFAULT '0.00',
  `DISKON_TOTAL_PERSEN` decimal(10,2) DEFAULT '0.00',
  `DISKON_TOTAL_RP` decimal(15,2) DEFAULT '0.00',
  `PAJAK_PERSEN` decimal(10,2) DEFAULT '0.00',
  `PAJAK_RP` decimal(15,2) DEFAULT '0.00',
  `GRAND_TOTAL_STL_PAJAK` decimal(15,2) DEFAULT '0.00',
  `LABA` decimal(15,2) DEFAULT '0.00',
  `BAYAR` decimal(15,2) DEFAULT '0.00',
  `NOMINAL_TRANSFER` decimal(15,2) DEFAULT '0.00',
  `BIAYA_KIRIM` decimal(15,2) DEFAULT '0.00',
  `KEMBALI` decimal(15,2) DEFAULT '0.00',
  `TGL_RETUR` datetime DEFAULT NULL,
  `NILAI_RETUR` decimal(15,2) DEFAULT '0.00',
  `TGL_PEMBAYARAN` datetime DEFAULT NULL,
  `NOMINALBAYARPIUTANG` decimal(15,2) DEFAULT '0.00',
  `SISA_TAGIHAN` decimal(15,2) DEFAULT '0.00',
  `JATUH_TEMPO` datetime DEFAULT NULL,
  `STATUS_BAYAR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `STATUS_TRANSAKSI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TYPE_AKUN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_AKUN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PEMBAYARAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_AKUN_TF` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `NAMA_AKUN_TF` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `TYPE_AKUNBANK` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_AKUNBANK` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PEMBAYARANBANK` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `METODE` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BANK` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NO_REKENING` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REKENING` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NO_REFFERENSI` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_SALES` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SALES` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `penjualan_detail`
--

CREATE TABLE `penjualan_detail` (
  `FAKTUR_JUAL` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PELANGGAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASIBARANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_JUAL` datetime DEFAULT NULL,
  `ID_BARANG` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SERIAL_NUMBER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `HARGA_BELI` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) DEFAULT '0',
  `HARGA_BELI_SATUAN` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `HARGA_JUAL` decimal(15,0) DEFAULT '0',
  `QTY_SATUAN` decimal(15,4) DEFAULT '0.0000',
  `DISKON_PERSEN` decimal(10,2) DEFAULT '0.00',
  `DISKON_RP` decimal(15,2) DEFAULT '0.00',
  `TOTAL_DISKON` decimal(15,2) DEFAULT '0.00',
  `TOTAL_HARGA` decimal(15,0) DEFAULT '0',
  `LABA` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URUTAN` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `penjualan_ditahan`
--

CREATE TABLE `penjualan_ditahan` (
  `FAKTUR_JUAL` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PELANGGAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_JUAL` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GRAN_TOTAL` decimal(15,0) DEFAULT '0',
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `TOTAL_ITEM` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `penjualan_ditahan_detail`
--

CREATE TABLE `penjualan_ditahan_detail` (
  `FAKTUR_JUAL` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SERIAL_NUMBER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `HARGA_BELI` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` smallint(6) DEFAULT '0',
  `HARGA_BELI_SATUAN` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `HARGA_JUAL` decimal(15,2) DEFAULT '0.00',
  `QTY_SATUAN` decimal(15,4) DEFAULT '0.0000',
  `DISKON_PERSEN` decimal(10,2) DEFAULT '0.00',
  `DISKON_RP` decimal(15,2) DEFAULT '0.00',
  `TOTAL_DISKON` decimal(15,2) DEFAULT '0.00',
  `TOTAL_HARGA` decimal(15,0) DEFAULT '0',
  `TOKO` decimal(15,4) DEFAULT '0.0000',
  `GUDANG` decimal(15,4) DEFAULT '0.0000',
  `STOK` decimal(15,4) DEFAULT '0.0000',
  `SISA` decimal(15,4) DEFAULT '0.0000',
  `URUTAN` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `piutang`
--

CREATE TABLE `piutang` (
  `ID_BAYAR_PIUTANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_PELANGGAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_BAYAR` datetime DEFAULT NULL,
  `TOTAL_PIUTANG` decimal(15,0) DEFAULT '0',
  `NOMINAL_BAYAR` decimal(15,0) DEFAULT '0',
  `SISA_PIUTANG` decimal(15,0) DEFAULT '0',
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER_BAYAR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER_BAYAR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `piutang_detail`
--

CREATE TABLE `piutang_detail` (
  `ID_BAYAR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_BAYAR` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_JUAL` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'BAYAR',
  `TANGGAL_JUAL` datetime DEFAULT NULL,
  `PIUTANG` decimal(15,0) DEFAULT '0',
  `DIBAYAR` decimal(15,0) DEFAULT '0',
  `RETUR` decimal(15,0) DEFAULT '0',
  `HUTANG` decimal(15,0) DEFAULT '0',
  `JATUH_TEMPO` datetime DEFAULT NULL,
  `PEMBAYARAN` decimal(15,0) DEFAULT '0',
  `STATUS` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `poin_barang`
--

CREATE TABLE `poin_barang` (
  `ID_BARANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'FK ke tbl_barang.ID_BARANG',
  `HARGA_POIN` int(11) NOT NULL DEFAULT '0' COMMENT 'Jumlah poin yang dibutuhkan untuk 1 unit barang ini',
  `AKTIF` tinyint(4) NOT NULL DEFAULT '1' COMMENT '1=Tersedia untuk ditukar, 0=Tidak tersedia',
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1',
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `poin_config`
--

CREATE TABLE `poin_config` (
  `ID` int(11) NOT NULL,
  `AKTIF` tinyint(4) NOT NULL DEFAULT '0' COMMENT '0=Tidak Aktif, 1=Aktif',
  `MEKANISME` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PER_ITEM' COMMENT 'PER_ITEM atau PER_NOMINAL',
  `POIN_PER_QTY` decimal(10,2) NOT NULL DEFAULT '1.00' COMMENT 'Poin per 1 qty satuan item (dipakai saat PER_ITEM)',
  `KELIPATAN_NOMINAL` decimal(15,0) NOT NULL DEFAULT '10000' COMMENT 'Nilai Rp per 1 poin (dipakai saat PER_NOMINAL)',
  `MINIMUM_REDEEM` int(11) NOT NULL,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Konfigurasi aturan earn poin & data sinkronisasi';

--
-- Dumping data untuk tabel `poin_config`
--

INSERT INTO `poin_config` (`ID`, `AKTIF`, `MEKANISME`, `POIN_PER_QTY`, `KELIPATAN_NOMINAL`, `MINIMUM_REDEEM`, `sync_id`) VALUES
(1, 0, 'PER_ITEM', '1.00', '10000', 0, NULL);

-- --------------------------------------------------------

--
-- Struktur dari tabel `poin_ledger`
--

CREATE TABLE `poin_ledger` (
  `ID` int(11) NOT NULL,
  `KODE_PELANGGAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'FK ke tbl_pelanggan.KODE',
  `TIPE` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'EARN | REDEEM | VOID_EARN',
  `JUMLAH_POIN` int(11) NOT NULL DEFAULT '0' COMMENT 'Positif=tambah, Negatif=kurang',
  `NO_REFERENSI` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Nomor faktur penjualan atau nomor penukaran TP-xxx',
  `KETERANGAN` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `retur_pembelian`
--

CREATE TABLE `retur_pembelian` (
  `ID_RETUR_PEMBELIAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_RETUR_BELI` datetime DEFAULT NULL,
  `ID_SUPPLIER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPPLIER` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT_SUPPLIER` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KONTAK_SUPPLIER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PEMBELIAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_PEMBELIAN` datetime DEFAULT NULL,
  `STATUS_PEMBELIAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PENYIMPANAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BAYAR_PEMBELIAN` decimal(15,0) DEFAULT '0',
  `SISA_PEMBELIAN` decimal(15,0) DEFAULT NULL,
  `TOTAL_BARANG` int(11) DEFAULT '0',
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `TOTAL_RUPIAH` decimal(15,0) DEFAULT '0',
  `JENIS_PENGEMBALIAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REKENING` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REKENING` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REKENING_TRANSFER` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Nama akun untuk pembayaran transfer',
  `KODE_REKENING_TRANSFER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Kode akun untuk pembayaran transfer',
  `NOMINAL_TUNAI` decimal(15,2) DEFAULT '0.00' COMMENT 'Nominal pembayaran tunai',
  `NOMINAL_TRANSFER` decimal(15,2) DEFAULT '0.00' COMMENT 'Nominal pembayaran transfer',
  `ALASAN_RETUR` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `retur_pembelian_detail`
--

CREATE TABLE `retur_pembelian_detail` (
  `ID_RETUR_PEMBELIAN` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_RETUR_BELI` datetime DEFAULT NULL,
  `ID_SUPLIYER` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA_BELI` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) DEFAULT '0',
  `HARGA_BELI_SATUAN` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `QTY_SAT` decimal(15,4) DEFAULT '0.0000',
  `TOTAL` decimal(15,0) DEFAULT '0',
  `PENYIMPANAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URUTAN` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `retur_penjualan`
--

CREATE TABLE `retur_penjualan` (
  `ID_RETUR_PENJUALAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_RETUR_JUAL` datetime DEFAULT NULL,
  `ID_PELANGGAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT_PELANGGAN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KONTAK_PELANGGAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PENJUALAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_PENJUALAN` datetime DEFAULT NULL,
  `STATUS_PENJUALAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PENYIMPANAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BAYAR_PENJUALAN` decimal(15,0) DEFAULT '0',
  `HUTANG_PENJUALAN` decimal(15,0) DEFAULT NULL,
  `TOTAL_BARANG` int(11) DEFAULT '0',
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `TOTAL_RUPIAH` decimal(15,0) DEFAULT '0',
  `NAMA_REKENING` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REKENING` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALASAN_RETUR` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `retur_penjualan_detail`
--

CREATE TABLE `retur_penjualan_detail` (
  `ID_RETUR_PENJUALAN` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_RETUR_JUAL` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PELANGGAN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA_BELI` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) NOT NULL DEFAULT '1',
  `QTY_SATUAN` decimal(15,4) DEFAULT '0.0000',
  `HARGA_BELI_SATUAN` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `HARGA_JUAL` decimal(15,2) DEFAULT '0.00',
  `TOTAL_DISKON` decimal(15,2) DEFAULT '0.00',
  `TOTAL_HARGA` decimal(15,0) DEFAULT '0',
  `LABA` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URUTAN` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `sales_order`
--

CREATE TABLE `sales_order` (
  `ID_PENJUALAN` varchar(30) NOT NULL,
  `ID_PELANGGAN` varchar(10) DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(100) DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(30) DEFAULT NULL,
  `ALAMAT_PELANGGAN` varchar(200) DEFAULT NULL,
  `LOKASIBARANG` varchar(20) DEFAULT NULL,
  `ID_SALES` varchar(20) DEFAULT NULL,
  `NAMA_SALES` varchar(100) DEFAULT NULL,
  `TGL_TRANSAKSI` datetime DEFAULT NULL,
  `TOTAL_HPP` decimal(15,2) DEFAULT '0.00',
  `GRAND_TOTAL_SBL_PAJAK` decimal(15,2) DEFAULT '0.00',
  `DISKON_TOTAL_PERSEN` decimal(10,2) DEFAULT '0.00',
  `DISKON_TOTAL_RP` decimal(15,2) DEFAULT '0.00',
  `PAJAK_PERSEN` decimal(10,2) DEFAULT '0.00',
  `PAJAK_RP` decimal(15,2) DEFAULT '0.00',
  `GRAND_TOTAL_STL_PAJAK` decimal(15,2) DEFAULT '0.00',
  `LABA` decimal(15,2) DEFAULT '0.00',
  `STATUS_TRANSAKSI` varchar(20) DEFAULT 'Aktif',
  `CATATAN` text,
  `ID_USER` varchar(20) DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) DEFAULT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Struktur dari tabel `sales_order_detail`
--

CREATE TABLE `sales_order_detail` (
  `FAKTUR_JUAL` varchar(15) DEFAULT NULL,
  `ID_PELANGGAN` varchar(30) DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(100) DEFAULT NULL,
  `JENIS_PELANGGAN` varchar(10) DEFAULT NULL,
  `LOKASIBARANG` varchar(20) DEFAULT NULL,
  `TANGGAL_JUAL` datetime DEFAULT NULL,
  `ID_BARANG` varchar(15) DEFAULT NULL,
  `NAMA_BARANG` varchar(100) DEFAULT NULL,
  `SERIAL_NUMBER` varchar(50) DEFAULT NULL,
  `HARGA_BELI` decimal(15,4) DEFAULT '0.0000',
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(10) DEFAULT NULL,
  `ISI_SATUAN` int(11) DEFAULT '1',
  `HARGA_BELI_SATUAN` decimal(15,4) DEFAULT '0.0000',
  `HARGA_JUAL` decimal(15,2) DEFAULT '0.00',
  `QTY_SATUAN` decimal(15,4) DEFAULT '0.0000',
  `DISKON_PERSEN` decimal(10,2) DEFAULT '0.00',
  `DISKON_RP` decimal(15,2) DEFAULT '0.00',
  `TOTAL_DISKON` decimal(15,2) DEFAULT '0.00',
  `TOTAL_Harga` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(20) DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) DEFAULT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) DEFAULT NULL,
  `URUTAN` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Struktur dari tabel `stoktambahkurang`
--

CREATE TABLE `stoktambahkurang` (
  `FAKTUR` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `JENIS` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) DEFAULT '0',
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `stok_opname`
--

CREATE TABLE `stok_opname` (
  `ID_STOK_OPNAME` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KATEGORI` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA` decimal(15,2) DEFAULT '0.00',
  `STOK_SYSTEM` decimal(15,4) DEFAULT '0.0000',
  `STOK_NYATA` decimal(15,4) DEFAULT '0.0000',
  `STOK_SELISIH` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` smallint(6) DEFAULT '0',
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `TOTAL_HARGA` decimal(15,0) DEFAULT '0',
  `KETERANGAN` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URUTAN` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `surat_jalan`
--

CREATE TABLE `surat_jalan` (
  `NOTA` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_PENGIRIMAN` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TOTAL_PELANGGAN` int(11) DEFAULT '0',
  `TOTAL_RUPIAH` decimal(20,0) DEFAULT '0',
  `KODE_ARMADA` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ARMADA` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_ARMADA` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_SUPIR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SUPIR` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_HELPER1` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HELPER1` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_HELPER2` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HELPER2` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_USER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `surat_jalan_detail`
--

CREATE TABLE `surat_jalan_detail` (
  `NOTA` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_KIRIM` datetime DEFAULT NULL,
  `LOKASISIMPAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTA_BELANJA` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_PELANGGAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_PELANGGAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT_PELANGGAN` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_BELANJA` datetime DEFAULT NULL,
  `NILAI_BELANJA` decimal(15,0) DEFAULT '0',
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SUMBER` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Jual',
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `sync_config`
--

CREATE TABLE `sync_config` (
  `kunci` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nilai` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data untuk tabel `sync_config`
--

INSERT INTO `sync_config` (`kunci`, `nilai`) VALUES
('device_id', '66a2646a-7552-4fa1-a11b-14d4045836db'),
('kode_cabang', 'TK-000001-66A2'),
('kode_toko', 'TOKO1'),
('last_sync_armada', '2000-01-01 00:00:00'),
('last_sync_barang', '2000-01-01 00:00:00'),
('last_sync_cabang', '2000-01-01 00:00:00'),
('last_sync_kategori', '2000-01-01 00:00:00'),
('last_sync_merk', '2000-01-01 00:00:00'),
('last_sync_pelanggan', '2000-01-01 00:00:00'),
('last_sync_satuan', '2000-01-01 00:00:00'),
('last_sync_supliyer', '2000-01-01 00:00:00'),
('last_sync_transfer', '2000-01-01 00:00:00');

-- --------------------------------------------------------

--
-- Struktur dari tabel `sync_log`
--

CREATE TABLE `sync_log` (
  `id` int(11) NOT NULL,
  `waktu` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `jenis` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'UPLOAD/DOWNLOAD/CONFLICT/ERROR',
  `tabel` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_lokal` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `pesan` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `sync_queue`
--

CREATE TABLE `sync_queue` (
  `id` int(11) NOT NULL,
  `aksi` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'INSERT / UPDATE',
  `tabel` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_lokal` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'PK lokal (ID_BARANG dll)',
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `payload` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'JSON data',
  `status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending' COMMENT 'pending/done/failed',
  `retry_count` tinyint(4) NOT NULL DEFAULT '0',
  `last_error` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_armada`
--

CREATE TABLE `tbl_armada` (
  `KODE` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NOPOL` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `JENIS` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_audit_config`
--

CREATE TABLE `tbl_audit_config` (
  `config_key` varchar(50) NOT NULL,
  `config_value` varchar(100) NOT NULL DEFAULT '',
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Konfigurasi modul audit trail (retensi, tanggal arsip terakhir)';

--
-- Dumping data untuk tabel `tbl_audit_config`
--

INSERT INTO `tbl_audit_config` (`config_key`, `config_value`) VALUES
('AuditArsipTerakhir', '2026-10-02'),
('AuditRetensi', '3');

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_audit_trail`
--

CREATE TABLE `tbl_audit_trail` (
  `id_audit` int(11) NOT NULL,
  `waktu_aksi` datetime NOT NULL,
  `jenis_aksi` char(12) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'HAPUS | EDIT | TAMBAH_STOK | KURANG_STOK',
  `jenis_trans` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Penjualan | Pembelian | Master User | dll',
  `identifier` varchar(35) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'no_faktur atau PREFIX:nilai untuk master',
  `id_user` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `lokasi` char(6) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'TOKO atau GUDANG',
  `komputer` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ket` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci COMMENT '[KRITIS]/[MENENGAH] | faktur | tgl | nominal | nama | status'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Audit trail edit dan hapus transaksi serta data master';

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_audit_trail_arsip`
--

CREATE TABLE `tbl_audit_trail_arsip` (
  `id_audit` int(11) NOT NULL,
  `waktu_aksi` datetime NOT NULL,
  `jenis_aksi` char(12) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'HAPUS | EDIT | TAMBAH_STOK | KURANG_STOK',
  `jenis_trans` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Penjualan | Pembelian | Master User | dll',
  `identifier` varchar(35) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'no_faktur atau PREFIX:nilai untuk master',
  `id_user` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `lokasi` char(6) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'TOKO atau GUDANG',
  `komputer` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ket` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci COMMENT '[KRITIS]/[MENENGAH] | faktur | tgl | nominal | nama | status'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Audit trail edit dan hapus transaksi serta data master';

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_barang`
--

CREATE TABLE `tbl_barang` (
  `ID_BARANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ID_BARANG_BANTU` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA_BARANG_BANTU` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_KATEGORI` varchar(4) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_KATEGORI` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_SUPLIYER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_SUPLIYER` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_MERK` varchar(4) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_MERK` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SERIAL_NUMBER` tinyint(1) DEFAULT NULL,
  `JENIS_SATUAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA_BELI` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `HARGA_BELI_TERAKHIR` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `HPP_UMUM_KECIL` decimal(15,2) DEFAULT '0.00',
  `HPP_UMUM_SEDANG` decimal(15,2) DEFAULT '0.00',
  `HPP_UMUM_BESAR` decimal(15,2) DEFAULT '0.00',
  `HARGA_BELI_UMUM_KECIL` decimal(15,2) DEFAULT '0.00',
  `HARGA_BELI_UMUM_SEDANG` decimal(15,2) DEFAULT '0.00',
  `HARGA_BELI_UMUM_BESAR` decimal(15,2) DEFAULT '0.00',
  `HPP_PARTAI_KECIL` decimal(15,2) DEFAULT '0.00',
  `HPP_PARTAI_SEDANG` decimal(15,2) DEFAULT '0.00',
  `HPP_PARTAI_BESAR` decimal(15,2) DEFAULT '0.00',
  `HARGA_BELI_PARTAI_KECIL` decimal(15,2) DEFAULT '0.00',
  `HARGA_BELI_UPARTAI_SEDANG` decimal(15,2) DEFAULT '0.00',
  `HARGA_BELI_PARTAI_BESAR` decimal(15,2) DEFAULT '0.00',
  `BARCODE_KECIL` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BARCODE_SEDANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BARCODE_BESAR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_UMUM_KECIL` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_UMUM_SEDANG` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_UMUM_BESAR` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_UMUM_KECIL` int(11) DEFAULT '0',
  `ISI_UMUM_SEDANG` int(11) DEFAULT '0',
  `ISI_UMUM_BESAR` int(11) DEFAULT '0',
  `HARGA_JUAL_UMUM_KECIL` decimal(15,2) DEFAULT '0.00',
  `HARGA_JUAL_UMUM_SEDANG` decimal(15,2) DEFAULT '0.00',
  `HARGA_JUAL_UMUM_BESAR` decimal(15,2) DEFAULT '0.00',
  `SATUAN_PARTAI_KECIL` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_PARTAI_SEDANG` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_PARTAI_BESAR` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_PARTAI_KECIL` int(11) DEFAULT '0',
  `ISI_PARTAI_SEDANG` int(11) DEFAULT '0',
  `ISI_PARTAI_BESAR` int(11) DEFAULT '0',
  `HARGA_JUAL_PARTAI_KECIL` decimal(15,2) DEFAULT '0.00',
  `HARGA_JUAL_PARTAI_SEDANG` decimal(15,2) DEFAULT '0.00',
  `HARGA_JUAL_PARTAI_BESAR` decimal(15,2) DEFAULT '0.00',
  `AWAL_TOKO` decimal(15,4) DEFAULT '0.0000',
  `STOK_AWAL_TOKO` decimal(15,4) DEFAULT '0.0000',
  `TAMBAH_TOKO` decimal(15,4) DEFAULT '0.0000',
  `KURANG_TOKO` decimal(15,4) DEFAULT '0.0000',
  `PEMBELIAN_TOKO` decimal(15,4) DEFAULT '0.0000',
  `PENJUALAN_TOKO` decimal(15,4) DEFAULT '0.0000',
  `RETUR_BELI_TOKO` decimal(15,4) DEFAULT '0.0000',
  `RETUR_JUAL_TOKO` decimal(15,4) DEFAULT '0.0000',
  `OPNAME_TOKO` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_STOK_MASUK_TOKO` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_STOK_KELUAR_TOKO` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_BARANG_MASUK_TOKO` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_BARANG_KELUAR_TOKO` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_CABANG_MASUK_TOKO` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_CABANG_KELUAR_TOKO` decimal(15,4) DEFAULT '0.0000',
  `STOK_TOKO` decimal(15,4) DEFAULT '0.0000',
  `AWAL_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `STOK_AWAL_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `TAMBAH_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `KURANG_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `PEMBELIAN_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `PENJUALAN_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `RETUR_BELI_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `RETUR_JUAL_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `OPNAME_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_STOK_MASUK_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_STOK_KELUAR_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_BARANG_MASUK_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_BARANG_KELUAR_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_CABANG_MASUK_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `TRANSFER_CABANG_KELUAR_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `STOK_GUDANG` decimal(15,4) DEFAULT '0.0000',
  `SATUAN_STOK` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SATUAN_ISI_STOK` int(11) DEFAULT '0',
  `STOK_MIN` decimal(15,4) DEFAULT '0.0000',
  `STOK_MAX` decimal(15,4) DEFAULT '0.0000',
  `LOKASI_RAK_TOKO` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI_RAK_GUDANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `POINT_MEMBER` decimal(10,2) DEFAULT '0.00',
  `POINT_KARYAWAN` decimal(10,2) DEFAULT '0.00',
  `KOMISI_SALES_RP` decimal(15,2) DEFAULT '0.00',
  `KOMISI_SALES_PERSEN` decimal(10,2) DEFAULT '0.00',
  `STATUS` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Aktif',
  `IS_PAKET` tinyint(1) NOT NULL DEFAULT '0' COMMENT '0=Barang biasa, 1=Paket rakitan/bundle',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_cabang`
--

CREATE TABLE `tbl_cabang` (
  `kode_cabang` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nama_cabang` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `alamat` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `kota` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `hp` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `pemilik` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `sumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'manual',
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '0',
  `version` int(11) NOT NULL DEFAULT '1',
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_datareferensi`
--

CREATE TABLE `tbl_datareferensi` (
  `STATUS` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Terkunci = tidak bisa dihapus user | NULL = bebas dihapus',
  `JENIS_AKUN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Filter laporan: ASET LANCAR | ASET TETAP | PASIVA | MODAL | PENJUALAN | HPP | BIAYA | PENDAPATAN LAIN | PAJAK',
  `TYPE_AKUN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Prefix urutan kode akun, dipakai filter dropdown FormTabelReferensi',
  `KODE_AKUN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Primary key format XX.XX.XXX',
  `NAMA_AKUN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Nama lengkap akun',
  `SUB_AKUN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'AKTIVA | PASIVA | LABA RUGI | LABA (pendapatan+kontra-beban) | RUGI (beban+kontra-pendapatan)',
  `AKUN_DK` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Saldo normal: DEBET | KREDIT',
  `AKUN_NRLR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Posisi di laporan: NERACA | LABA RUGI',
  `KETERANGAN` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci COMMENT 'Penjelasan akuntansi lengkap fungsi akun',
  `SALDO_AWAL` decimal(20,0) DEFAULT '0' COMMENT 'Saldo awal periode (titik awal kalkulasi, tidak pernah diubah oleh laporan)',
  `SALDO_SEBELUMNYA` decimal(20,0) DEFAULT '0' COMMENT 'Kolom warisan (legacy) — nilainya selalu = SALDO_AWAL setelah posting resmi. Kalkulasi periode pakai temp_datareferensi',
  `S_DEBET` decimal(20,0) DEFAULT '0' COMMENT 'Total mutasi debet periode ini dari JurnalUmum',
  `S_KREDIT` decimal(20,0) DEFAULT '0' COMMENT 'Total mutasi kredit periode ini dari JurnalUmum',
  `SALDO_AKHIR` decimal(20,0) DEFAULT '0' COMMENT 'Saldo akhir = SALDO_SEBELUMNYA +/- mutasi sesuai AKUN_DK',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data untuk tabel `tbl_datareferensi`
--

INSERT INTO `tbl_datareferensi` (`STATUS`, `JENIS_AKUN`, `TYPE_AKUN`, `KODE_AKUN`, `NAMA_AKUN`, `SUB_AKUN`, `AKUN_DK`, `AKUN_NRLR`, `KETERANGAN`, `SALDO_AWAL`, `SALDO_SEBELUMNYA`, `S_DEBET`, `S_KREDIT`, `SALDO_AKHIR`, `created_at`, `sync_id`) VALUES
('Terkunci', 'ASET LANCAR', 'KAS', '01.01.001', 'KAS DI TOKO', 'AKTIVA', 'DEBET', 'NERACA', 'Mencatat uang tunai fisik yang berada di mesin kasir/laci toko. Digunakan untuk transaksi penjualan tunai harian dan pengeluaran kecil di toko. Rekonsiliasi fisik dilakukan setiap tutup kasir.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce960f8a-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'ASET LANCAR', 'KAS', '01.01.002', 'KAS DI GUDANG', 'AKTIVA', 'DEBET', 'NERACA', 'Uang tunai kecil (petty cash) yang disimpan di gudang untuk operasional harian seperti pembelian bahan pembantu, bayar upah lepas, dsb. Sistem imprest atau fluktuasi.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce961adf-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET LANCAR', 'KAS', '01.01.003', 'KAS KIRIMAN TOKO', 'AKTIVA', 'DEBET', 'NERACA', 'Uang tunai yang sudah dikeluarkan dari bank atau kas gudang tetapi belum diterima secara fisik oleh toko. Digunakan saat arus kas antar lokasi. Harus segera direkonsiliasi.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce961e5c-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET LANCAR', 'KAS', '01.01.004', 'KAS KIRIMAN GUDANG', 'AKTIVA', 'DEBET', 'NERACA', 'Uang tunai yang sedang dalam perjalanan menuju gudang (misal dari toko atau bank). Akun transit, saldo harus nol setelah periode tertentu.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce962122-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET LANCAR', 'BANK', '01.02.001', 'TRANSFER BANK', 'AKTIVA', 'DEBET', 'NERACA', 'Seluruh rekening bank perusahaan (giro, tabungan, deposito). Mencatat semua penerimaan dan pengeluaran via transfer, kliring, atau setoran tunai. Rekonsiliasi bank setiap bulan wajib.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce9623b1-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'ASET LANCAR', 'PIUTANG', '01.03.001', 'PIUTANG USAHA', 'AKTIVA', 'DEBET', 'NERACA', 'Tagihan penjualan kredit kepada pelanggan/reseller yang jatuh tempo 12 bulan. Disajikan neto setelah cadangan kerugian piutang. Umur piutang dipantau rutin.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce962625-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'ASET LANCAR', 'PIUTANG', '01.03.002', 'PIUTANG KARYAWAN', 'AKTIVA', 'DEBET', 'NERACA', 'Uang yang dipinjam karyawan (kas bon) yang akan dipotong dari gaji atau dicicil. Jangka pendek. Tidak boleh dikapitalisasi sebagai beban.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce962aa7-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'ASET LANCAR', 'A LANCAR', '01.04.001', 'PERSEDIAAN BARANG', 'AKTIVA', 'DEBET', 'NERACA', 'Nilai barang dagang yang tersedia di gudang/toko (harga perolehan). Sistem pencatatan perpetual atau periodik. Dicatat sesuai PSAK 14 (persediaan).', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce962e89-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'ASET LANCAR', 'A LANCAR', '01.04.002', 'TAGIHAN / SALDO PIUTANG', 'AKTIVA', 'DEBET', 'NERACA', 'Tagihan lain-lain yang bersifat jangka pendek selain piutang usaha & karyawan. Contoh: uang muka pembelian, klaim asuransi, deposit sewa.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce963114-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET LANCAR', 'A LANCAR', '01.04.003', 'PERLENGKAPAN KANTOR', 'AKTIVA', 'DEBET', 'NERACA', 'Stok perlengkapan kantor (kertas, tinta, alat tulis, dll) yang belum terpakai. Saat dipakai, diakui sebagai beban perlengkapan.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce963363-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET LANCAR', 'PAJAK AL', '01.05.001', 'PPN MASUKAN', 'AKTIVA', 'DEBET', 'NERACA', 'PPN yang dibayar saat pembelian barang/jasa kena pajak. Dapat dikreditkan (di-offset) dengan PPN Keluaran. Disajikan sebagai aset pajak.', '0', '0', '0', '0', '0', '2026-04-20 22:53:57', NULL),
('NULL', 'ASET TETAP', 'A TETAP', '02.01.001', 'TANAH', 'AKTIVA', 'DEBET', 'NERACA', 'Aset tetap berupa tanah (harga perolehan termasuk biaya perolehan hak). Tidak disusutkan. Disajikan sebesar harga perolehan.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce9635b5-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET TETAP', 'A TETAP', '02.01.002', 'GEDUNG', 'AKTIVA', 'DEBET', 'NERACA', 'Aset tetap berupa bangunan/gudang/toko (harga perolehan). Disusutkan metode garis lurus sesuai taksiran masa manfaat (umumnya 20 tahun).', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce963836-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET TETAP', 'A TETAP', '02.01.003', 'INVENTARIS', 'AKTIVA', 'DEBET', 'NERACA', 'Aset tetap berupa perabotan, meja kursi, lemari, AC, komputer, mesin kantor (nilai signifikan). Disusutkan 4-8 tahun.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce963a73-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET TETAP', 'A TETAP', '02.01.004', 'KENDARAAN', 'AKTIVA', 'DEBET', 'NERACA', 'Aset tetap berupa kendaraan operasional (mobil, motor, truck). Disusutkan 4-8 tahun. BPKB atas nama perusahaan.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce963cc3-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET TETAP', 'AKM PENY.', '02.02.002', 'AKUM. PENY. GEDUNG', 'AKTIVA', 'KREDIT', 'NERACA', 'Akun kontra aset (pengurang nilai gedung). Mencatat akumulasi beban penyusutan gedung sejak perolehan. Saldo kredit.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce9642a2-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET TETAP', 'AKM PENY.', '02.02.003', 'AKUM. PENY. INVENTARIS', 'AKTIVA', 'KREDIT', 'NERACA', 'Akun kontra aset (pengurang nilai inventaris). Mencatat akumulasi beban penyusutan inventaris. Saldo kredit.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce9644fa-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'ASET TETAP', 'AKM PENY.', '02.02.004', 'AKUM. PENY. KENDARAAN', 'AKTIVA', 'KREDIT', 'NERACA', 'Akun kontra aset (pengurang nilai kendaraan). Mencatat akumulasi beban penyusutan kendaraan. Saldo kredit.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce96473b-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'PASIVA', 'HUTANG', '03.01.001', 'HUTANG BELANJA', 'PASIVA', 'KREDIT', 'NERACA', 'Hutang usaha kepada supplier atas pembelian barang dagang secara kredit (jatuh tempo 12 bulan). Dicatat saat faktur diterima.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce964980-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'PASIVA', 'HUTANG', '03.01.002', 'HUTANG USAHA', 'PASIVA', 'KREDIT', 'NERACA', 'Hutang lain-lain terkait operasional usaha selain pembelian barang dagang. Contoh: hutang jasa perbaikan, hutang konsultan.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce964bf3-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'PASIVA', 'HUTANG', '03.01.003', 'HUTANG LAIN LAIN', 'PASIVA', 'KREDIT', 'NERACA', 'Hutang non-operasional jangka pendek. Contoh: hutang kepada pihak ketiga bukan pemasok utama, uang muka pelanggan.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce964e3b-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'PASIVA', 'BEBAN', '03.02.001', 'HUTANG PAJAK', 'PASIVA', 'KREDIT', 'NERACA', 'Hutang Pajak Penghasilan (PPh) yang masih harus disetor: Pasal 21, 22, 23, 25, 29 (badan). KAP sesuai DJP.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce965078-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'PASIVA', 'BEBAN', '03.02.002', 'HUTANG BANK JANGKA PENDEK', 'PASIVA', 'KREDIT', 'NERACA', 'Pinjaman bank dengan jatuh tempo kurang dari 1 tahun (termasuk cicilan pokok utang jangka panjang yang jatuh tempo tahun berjalan).', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce9652e7-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'PASIVA', 'BEBAN', '03.02.003', 'HUTANG BANK JANGKA PANJANG', 'PASIVA', 'KREDIT', 'NERACA', 'Pinjaman bank dengan jatuh tempo lebih dari 1 tahun. Disajikan setelah dikurangi bagian yang jatuh tempo dalam 1 tahun.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce96552e-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'PASIVA', 'PAJAK', '03.02.004', 'PPN KELUARAN', 'PASIVA', 'KREDIT', 'NERACA', 'PPN yang dipungut dari pembeli saat penjualan barang/jasa kena pajak. Harus disetor ke negara selisihnya dengan PPN Masukan.', '0', '0', '0', '0', '0', '2026-04-20 22:53:57', NULL),
('NULL', 'PASIVA', 'SOSIAL', '03.03.001', 'DANA KESEJAHTERAAN', 'PASIVA', 'KREDIT', 'NERACA', 'Dana sosial karyawan (koperasi, iuran kegiatan, sumbangan yang dipotong dari gaji atau kas perusahaan). Bersifat hutang kepada karyawan.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce96574a-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'MODAL', 'EKUITAS', '04.01.001', 'MODAL', 'PASIVA', 'KREDIT', 'NERACA', 'Modal dasar yang disetor oleh pemilik perusahaan (setoran awal dan setoran tambahan yang disahkan). Akun permanen.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce96596f-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'MODAL', 'EKUITAS', '04.01.002', 'MODAL PEMILIK', 'PASIVA', 'KREDIT', 'NERACA', 'Penyesuaian modal pemilik (misal revaluasi, tambahan investasi non-tunai, atau koreksi modal). Tidak untuk transaksi laba/rugi.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce965bb8-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'MODAL', 'EKUITAS', '04.01.003', 'REKENING KORAN PUSAT', 'PASIVA', 'KREDIT', 'NERACA', 'Akun penampung untuk transfer aset internal antar cabang/toko tanpa melibatkan kas. Saldo normal kredit. Pada konsolidasi, saldo antar cabang saling menghapus.', '0', '0', '0', '0', '0', '2026-10-02 18:53:23', NULL),
('Terkunci', 'MODAL', 'PRIVE', '04.02.001', 'PRIVE PEMILIK', 'PASIVA', 'DEBET', 'NERACA', 'Pengambilan uang atau aset perusahaan untuk keperluan pribadi pemilik. Mengurangi ekuitas. Saldo normal debet. Ditutup ke modal akhir tahun.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce965dfd-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'MODAL', 'LABA RUGI', '05.01.001', 'LABA RUGI BERJALAN', 'LABA RUGI', 'KREDIT', 'NERACA', 'Akun penampung laba/rugi tahun berjalan. Saldo kredit = laba, debet = rugi. Ditutup ke modal pada akhir periode.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce966025-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'PENJUALAN', 'PEND. KOTOR', '05.02.001', 'PENJUALAN', 'LABA', 'KREDIT', 'LABA RUGI', 'Pendapatan kotor dari penjualan barang dagang kepada pelanggan (belum dikurangi retur, diskon, dan potongan). Akun nominal.', '0', '0', '0', '0', '0', '2026-08-07 17:35:20', NULL),
('Terkunci', 'PENJUALAN', 'RETUR PEND.', '05.03.001', 'RETUR PENJUALAN', 'LABA', 'DEBET', 'LABA RUGI', 'Pengembalian barang oleh pelanggan karena cacat atau tidak sesuai. Mengurangi penjualan kotor. Saldo normal debet.', '0', '0', '0', '0', '0', '2026-10-02 18:53:23', NULL),
('Terkunci', 'PENJUALAN', 'DISKON PEND.', '05.04.001', 'POTONGAN DISKON PENJUALAN', 'LABA', 'DEBET', 'LABA RUGI', 'Potongan harga tunai yang diberikan kepada pelanggan. Mengurangi penjualan. Akun kontra pendapatan.', '0', '0', '0', '0', '0', '2026-10-02 18:53:23', NULL),
('Terkunci', 'HPP', 'HPP POKOK', '06.01.001', 'HPP POKOK PENJUALAN', 'RUGI', 'DEBET', 'LABA RUGI', 'Harga perolehan barang yang terjual (COGS). Dihitung dari persediaan awal + pembelian bersih - persediaan akhir. Akun nominal debet.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce9662f9-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'HPP', 'ANGKUT BELI', '06.02.001', 'BIAYA KIRIM PEMBELIAN', 'RUGI', 'DEBET', 'LABA RUGI', 'Biaya angkut pembelian (freight in) untuk mendatangkan barang ke gudang. Menambah nilai persediaan / HPP.', '0', '0', '0', '0', '0', '2026-04-20 22:53:57', NULL),
('NULL', 'HPP', 'ANGKUT JUAL', '06.03.001', 'BIAYA KIRIM PENJUALAN', 'RUGI', 'DEBET', 'LABA RUGI', 'Beban ongkos kirim (freight out) yang ditanggung perusahaan untuk mengirim barang ke pelanggan.', '0', '0', '0', '0', '0', '2026-04-20 22:53:57', NULL),
('Terkunci', 'HPP', 'PENY. STOK', '06.04.001', 'PENYESUAIAN STOK MINUS', 'RUGI', 'DEBET', 'LABA RUGI', 'Pencatatan selisih kurang (rugi) saat stok opname: barang hilang, rusak, expired.', '0', '0', '0', '0', '0', '2026-04-20 22:53:57', NULL),
('NULL', 'HPP', 'PENY. STOK', '06.04.002', 'PENYESUAIAN HARGA POKOK', 'RUGI', 'DEBET', 'LABA RUGI', 'Pencatatan selisih nilai persediaan akibat perubahan harga pokok barang (harga terbaru atau average cost). Digunakan untuk menjaga neraca tetap seimbang ketika harga pokok barang diupdate saat pembelian.', '0', '0', '0', '0', '0', '2026-04-21 20:03:25', NULL),
('NULL', 'HPP', 'DISKON BELI', '06.05.001', 'POTONGAN DISKON PEMBELIAN', 'RUGI', 'KREDIT', 'LABA RUGI', 'Diskon yang diperoleh dari supplier karena pembayaran lebih awal. Mengurangi HPP. Akun kontra-HPP (kredit).', '0', '0', '0', '0', '0', '2026-04-20 22:53:57', NULL),
('NULL', 'HPP', 'RETUR BELI', '06.06.001', 'RETUR PEMBELIAN', 'RUGI', 'KREDIT', 'LABA RUGI', 'Pengembalian barang ke supplier karena cacat atau tidak sesuai. Mengurangi nilai pembelian/HPP. Saldo normal kredit.', '0', '0', '0', '0', '0', '2026-04-20 22:53:57', NULL),
('Terkunci', 'BIAYA', 'BIAYA', '07.01.001', 'BEBAN GAJI KARYAWAN', 'RUGI', 'DEBET', 'LABA RUGI', 'Biaya gaji, upah, tunjangan, bonus, dan THR karyawan tetap & harian.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce96662b-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'BIAYA', 'BIAYA', '07.01.002', 'BEBAN PERLENGKAPAN ATK', 'RUGI', 'DEBET', 'LABA RUGI', 'Biaya pemakaian alat tulis kantor (ATK) yang sudah habis pakai dalam periode berjalan.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce9669c0-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'BIAYA', 'BIAYA', '07.01.003', 'BEBAN LISTRIK & AIR', 'RUGI', 'DEBET', 'LABA RUGI', 'Biaya utilitas: listrik (PLN), air (PDAM), telepon, internet untuk operasional kantor/toko/gudang.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce966d46-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'BIAYA', 'BIAYA', '07.01.004', 'BEBAN BBM DAN ONGKOS KIRIM', 'RUGI', 'DEBET', 'LABA RUGI', 'Beban BBM kendaraan operasional dan ongkos kirim yang tidak terkait langsung dengan penjualan.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce96709e-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'BIAYA', 'BIAYA', '07.01.005', 'BEBAN PEMELIHARAAN GEDUNG', 'RUGI', 'DEBET', 'LABA RUGI', 'Biaya perbaikan, perawatan, cat, perpipaan, kebersihan gedung. Bukan biaya penyusutan.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce9672f6-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'BIAYA', 'BIAYA', '07.01.007', 'BEBAN PENYUSUTAN GEDUNG', 'RUGI', 'DEBET', 'LABA RUGI', 'Beban penyusutan aset tetap gedung per periode (metode garis lurus).', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce967778-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'BIAYA', 'BIAYA', '07.01.008', 'BEBAN PENYUSUTAN INVENTARIS', 'RUGI', 'DEBET', 'LABA RUGI', 'Beban penyusutan inventaris/furniture per periode.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce9679ad-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'BIAYA', 'BIAYA', '07.01.009', 'BEBAN PENYUSUTAN KENDARAAN', 'RUGI', 'DEBET', 'LABA RUGI', 'Beban penyusutan kendaraan operasional per periode.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce967bee-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'BIAYA', 'BIAYA', '07.01.011', 'BEBAN ADM DAN BUNGA BANK', 'RUGI', 'DEBET', 'LABA RUGI', 'Biaya administrasi bank (fee bulanan, transfer, materai) dan bunga pinjaman bank.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce96807c-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'BIAYA', 'BIAYA', '07.01.012', 'BEBAN ADM DAN UMUM LAINNYA', 'RUGI', 'DEBET', 'LABA RUGI', 'Akun untuk beban operasional kecil lain-lain yang tidak material atau tidak sering terjadi.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce9682af-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'PENDAPATAN LAIN', 'PEND. BUNGA', '08.01.001', 'PENDAPATAN BUNGA BANK', 'LABA', 'KREDIT', 'LABA RUGI', 'Pendapatan bunga dari saldo rekening giro, tabungan, atau deposito.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce96851e-3cc8-11f1-85fc-a8a159b7c591'),
('Terkunci', 'PENDAPATAN LAIN', 'PEND. LAIN', '08.01.002', 'PENDAPATAN LAIN LAIN', 'LABA', 'KREDIT', 'LABA RUGI', 'Pendapatan non-operasional: laba penjualan aset tetap, klaim asuransi, komplain supplier, hibah, dll.', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce968882-3cc8-11f1-85fc-a8a159b7c591'),
('NULL', 'PAJAK', 'B PAJAK', '09.01.001', 'PAJAK PENGHASILAN', 'RUGI', 'DEBET', 'LABA RUGI', 'Beban pajak penghasilan badan (PPh Badan) terutang untuk tahun berjalan (PPh 25/29).', '0', '0', '0', '0', '0', '2026-04-20 21:49:39', 'ce968bdb-3cc8-11f1-85fc-a8a159b7c591');

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_gaji`
--

CREATE TABLE `tbl_gaji` (
  `KODE` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `HARI_KERJA` smallint(6) DEFAULT '0',
  `PROSENTASE_KOMISI` decimal(10,2) DEFAULT '0.00',
  `BONUS_SUPIR` decimal(15,2) DEFAULT '0.00',
  `BONUS_HELPER` decimal(15,2) DEFAULT '0.00',
  `BONUS_TRANSPORT` decimal(15,2) DEFAULT '0.00',
  `BONUS_MAKAN` decimal(15,2) DEFAULT '0.00',
  `BONUS_LEMBUR` decimal(15,2) DEFAULT '0.00',
  `JENIS_POTONGAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `POTONGAN_ABSEN` decimal(15,2) DEFAULT '0.00',
  `POTONGAN_ABSEN_KHUSUS` decimal(15,2) DEFAULT '0.00',
  `POTONGAN_TERLAMBAT` decimal(15,2) DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_karyawan`
--

CREATE TABLE `tbl_karyawan` (
  `KODE` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JABATAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGLMASUK` datetime DEFAULT NULL,
  `GAJI` decimal(15,0) DEFAULT '0',
  `SALDOAWAL` decimal(15,0) DEFAULT '0',
  `TOTALBON` decimal(15,0) DEFAULT '0',
  `TOTALBAYAR` decimal(15,0) DEFAULT '0',
  `SALDOAKHIR` decimal(15,0) DEFAULT '0',
  `Status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Aktif',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_kategori`
--

CREATE TABLE `tbl_kategori` (
  `KODE` varchar(4) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `JENIS` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data untuk tabel `tbl_kategori`
--

INSERT INTO `tbl_kategori` (`KODE`, `NAMA`, `JENIS`, `created_at`, `sync_id`, `id_cloud`, `updated_by`, `is_dirty`, `version`) VALUES
('ACC', 'Accesosies', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('AJI', 'Ajinomoto', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('AMI', 'Air Mineral', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('ARN', 'Arnots', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('ASE', 'Asesoris', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('ATK', 'Alat Tulis Kantor', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('BAB', 'Babycare', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('BIS', 'Biscuit', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('CBA', 'Cussons Baby', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('CHA', 'Chandi', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('CIC', 'Campina Ice Cream', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('CLA', 'Cap Lang', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('COC', 'Cocacola', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('DEK', 'Deka', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('DEO', 'Deodorant', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('DLO', 'Deo Lotion', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('DRI', 'Drink', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('EPR', 'Eratel Prima', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('FFL', 'Frisian Flag', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('FOO', 'Food', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('GAR', 'Garnier', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('GFO', 'Garuda Food', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('GOD', 'Go-day', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('HAI', 'Haircare', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('HAN', 'Hanasui', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('IMP', 'Implora', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('IND', 'Indofood', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('JBA', 'Johnson Baby', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('KAL', 'Kalbe', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('KAO', 'Kao', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('KAP', 'Kapal Api', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('KIN', 'Kino', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('KOK', 'Kokola', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('KOS', 'Kosmetik', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('LAG', 'Lagie', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('LAT', 'Latulipe', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('LLA', 'Lain Lain', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('MAY', 'Mayora', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('MIN', 'Minuman', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('MIN1', 'Mondeles International', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('MOR', 'Morinaga', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('MRA', 'Mustika Ratu', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('NAY', 'Nayla', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('NES', 'Nestle', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('NIN', 'Nabati Indonesia', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('NOO', 'Noodle', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('NUT', 'Nutrifood', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('OBA', 'Obat', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('OBA1', 'Obat-obatan', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('PAL', 'Palen', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('PAN', 'Panamex', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('PAR', 'Parfum', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('PER', 'Permen', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('PG', 'P&g', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('PIX', 'Pixy', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('POP', 'Popok', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('ROK', 'Rokok', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('SEM', 'Sembako', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('SEM1', 'Semir', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('SHA', 'Shampo', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('SKI', 'Skincare', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('SLA', 'Sari Laut', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('SME', 'Sinar Mentari', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('SNA', 'Snack', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('SOS', 'Sosro', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('SRO', 'Sari Roti', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('STT', 'Sttc', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('SUS', 'Susu', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('TEM', 'Tempo', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('TIS', 'Tissu', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('TOI', 'Toiletris', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('TOO', 'Toothpaste', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('TRI', 'Trios', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('ULT', 'Ultrajaya', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('UNI', 'Unibis', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('UNI1', 'Uni-charm', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('UNI2', 'Unilever', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('UNI3', 'Unirama', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('VIT', 'Vitalis', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('WCO', 'Wardah Cosmetic', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('WIN', 'Wingscare', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('WIN1', 'Wingsfood', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('WIS', 'Wismilak', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1),
('YCO', 'You Cosmetik', 'Barang Dagangan', '2026-04-28 19:54:05', NULL, NULL, NULL, 1, 1);

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_merk`
--

CREATE TABLE `tbl_merk` (
  `KODE` varchar(4) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KETERANGAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_pelanggan`
--

CREATE TABLE `tbl_pelanggan` (
  `KODE` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NO_TELP` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JANGKAPIUTANG` smallint(6) DEFAULT '0',
  `HUTANGAWAL` decimal(15,0) DEFAULT '0',
  `TOTALHUTANG` decimal(15,0) DEFAULT '0',
  `TOTALBAYAR` decimal(15,0) DEFAULT '0',
  `HUTANGAKHIR` decimal(15,0) DEFAULT '0',
  `SALDO_POIN` int(11) NOT NULL DEFAULT '0' COMMENT 'Saldo poin loyalitas terkini',
  `Status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Aktif',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_perusahaan`
--

CREATE TABLE `tbl_perusahaan` (
  `KODE` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `KODE_CLOUD` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_CLOUD` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT_CLOUD` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KOTA` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HP` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PEMILIK` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FOOTER1` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FOOTER2` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FOOTER3` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SYSTEM_TUTUP_BULAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL_TUTUP_BULAN` smallint(6) DEFAULT '0',
  `NAMA_REK_BARANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BARANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LAWAN_NAMA_REK_BARANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LAWAN_KODE_REK_BARANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_BELI_TOKO` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BELI_TOKO` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_BELI_GUDANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BELI_GUDANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_JUAL_TOKO` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_JUAL_TOKO` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_JUAL_GUDANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_JUAL_GUDANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_HUTANG_BELI` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_HUTANG_BELI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_PIUTANG_JUAL` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_PIUTANG_JUAL` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_RETUR_PEMBELIAN_TOKO` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_RETUR_PEMBELIAN_TOKO` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_RETUR_PENJUALAN_TOKO` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_RETUR_PENJUALAN_TOKO` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_RETUR_PEMBELIAN_GUDANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_RETUR_PEMBELIAN_GUDANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_RETUR_PENJUALAN_GUDANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_RETUR_PENJUALAN_GUDANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_BON_KARYAWAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BON_KARYAWAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_GAJI_KARYAWAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_GAJI_KARYAWAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_BAYAR_HUTANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BAYAR_HUTANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_BAYAR_PIUTANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_BAYAR_PIUTANG` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_REK_TRANSFER_JUAL` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_REK_TRANSFER_JUAL` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data untuk tabel `tbl_perusahaan`
--

INSERT INTO `tbl_perusahaan` (`KODE`, `KODE_CLOUD`, `NAMA_CLOUD`, `ALAMAT_CLOUD`, `NAMA`, `ALAMAT`, `KOTA`, `HP`, `PEMILIK`, `FOOTER1`, `FOOTER2`, `FOOTER3`, `SYSTEM_TUTUP_BULAN`, `TANGGAL_TUTUP_BULAN`, `NAMA_REK_BARANG`, `KODE_REK_BARANG`, `LAWAN_NAMA_REK_BARANG`, `LAWAN_KODE_REK_BARANG`, `NAMA_REK_BELI_TOKO`, `KODE_REK_BELI_TOKO`, `NAMA_REK_BELI_GUDANG`, `KODE_REK_BELI_GUDANG`, `NAMA_REK_JUAL_TOKO`, `KODE_REK_JUAL_TOKO`, `NAMA_REK_JUAL_GUDANG`, `KODE_REK_JUAL_GUDANG`, `NAMA_REK_HUTANG_BELI`, `KODE_REK_HUTANG_BELI`, `NAMA_REK_PIUTANG_JUAL`, `KODE_REK_PIUTANG_JUAL`, `NAMA_REK_RETUR_PEMBELIAN_TOKO`, `KODE_REK_RETUR_PEMBELIAN_TOKO`, `NAMA_REK_RETUR_PENJUALAN_TOKO`, `KODE_REK_RETUR_PENJUALAN_TOKO`, `NAMA_REK_RETUR_PEMBELIAN_GUDANG`, `KODE_REK_RETUR_PEMBELIAN_GUDANG`, `NAMA_REK_RETUR_PENJUALAN_GUDANG`, `KODE_REK_RETUR_PENJUALAN_GUDANG`, `NAMA_REK_BON_KARYAWAN`, `KODE_REK_BON_KARYAWAN`, `NAMA_REK_GAJI_KARYAWAN`, `KODE_REK_GAJI_KARYAWAN`, `NAMA_REK_BAYAR_HUTANG`, `KODE_REK_BAYAR_HUTANG`, `NAMA_REK_BAYAR_PIUTANG`, `KODE_REK_BAYAR_PIUTANG`, `NAMA_REK_TRANSFER_JUAL`, `KODE_REK_TRANSFER_JUAL`) VALUES
('TK-000001', 'TK-000001-66A2', 'Toko Saya - Cabang Utama', 'Jl. Contoh No. 1, Kota', 'TOKO SAYA YA', 'Jl. Contoh No. 1, Rt 01/Rw 01', 'Kota', '0812-3456-7890', 'NAMA PEMILIK', 'Terima Kasih Telah Berbelanja', 'Barang Yang Sudah Dibeli Tidak Dapat Dikembalikan', 'Garansi Barang Elektronik 1 Minggu Setelah Pembelian', 'Berdasar bulan kalender', 1, 'PERSEDIAAN BARANG', '01.04.001', 'MODAL', '04.01.001', 'KAS DI TOKO', '01.01.001', 'KAS DI GUDANG', '01.01.002', 'KAS DI TOKO', '01.01.001', 'KAS DI GUDANG', '01.01.002', 'HUTANG BELANJA', '03.01.001', 'TAGIHAN / SALDO PIUTANG', '01.04.002', 'KAS DI TOKO', '01.01.001', 'KAS DI TOKO', '01.01.001', 'KAS DI GUDANG', '01.01.002', 'KAS DI GUDANG', '01.01.002', 'KAS DI TOKO', '01.01.001', 'KAS DI TOKO', '01.01.001', 'KAS DI TOKO', '01.01.001', 'KAS DI TOKO', '01.01.001', 'TRANSFER BANK', '01.02.001');

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_proses_rakitan`
--

CREATE TABLE `tbl_proses_rakitan` (
  `id` int(11) NOT NULL,
  `no_rakitan` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `tanggal` datetime NOT NULL,
  `kode_rakitan` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nama_rakitan` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `qty_hasil` decimal(10,2) NOT NULL DEFAULT '1.00',
  `jenis` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Rakit' COMMENT 'Rakit atau Bongkar',
  `lokasi` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'TOKO',
  `id_ref` int(11) DEFAULT NULL COMMENT 'Untuk Bongkar: id transaksi Rakit asal',
  `keterangan` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `id_user` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `id_komputer` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_proses_rakitan_detail`
--

CREATE TABLE `tbl_proses_rakitan_detail` (
  `id` int(11) NOT NULL,
  `id_proses` int(11) NOT NULL,
  `kode_komponen` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nama_komponen` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `qty` decimal(10,2) NOT NULL DEFAULT '0.00',
  `satuan` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_rakitan_bom`
--

CREATE TABLE `tbl_rakitan_bom` (
  `id` int(11) NOT NULL,
  `kode_rakitan` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ID_BARANG paket rakitan',
  `kode_komponen` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ID_BARANG komponen',
  `nama_komponen` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `qty` decimal(10,2) NOT NULL DEFAULT '1.00',
  `satuan` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `urutan` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_satuan`
--

CREATE TABLE `tbl_satuan` (
  `KODE` varchar(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI` int(11) DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data untuk tabel `tbl_satuan`
--

INSERT INTO `tbl_satuan` (`KODE`, `NAMA`, `ISI`, `created_at`, `sync_id`, `id_cloud`, `updated_by`, `is_dirty`, `version`) VALUES
('/10', '/10', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('/101', '/102', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('/102', '/103', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BAG', 'BAG', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BAG1', 'BAG2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BAG2', 'BAG3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BAL', 'BAL', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BAL1', 'BAL2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BAL2', 'BAL3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BOS', 'BOSS', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BOS1', 'BOSS2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BOS2', 'BOSS3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BOX', 'BOX', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BOX1', 'BOX2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BOX2', 'BOX3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BX', 'BX', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BX1', 'BX2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('BX2', 'BX3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('IKA', 'IKAT', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('IKA1', 'IKAT2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('IKA2', 'IKAT3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KAR', 'KARTON', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KAR1', 'KARTON2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KAR2', 'KARTON3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KG', 'KG', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KG1', 'KG2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KG2', 'KG3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KLG', 'KLG', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KLG1', 'KLG2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KLG2', 'KLG3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KRA', 'KRAT', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KRA1', 'KRAT2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KRA2', 'KRAT3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KTK', 'KTK', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KTK1', 'KTK2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('KTK2', 'KTK3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('LB', 'LB', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('LB1', 'LB2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('LB2', 'LB3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('LMB', 'LEMBAR', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('LMB2', 'LEMBAR2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('LMB3', 'LEMBAR3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('LUS', 'LUSIN', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('LUS1', 'LUSIN2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('LUS2', 'LUSIN3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('ONS', 'ONS', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('ONS1', 'ONS2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('ONS2', 'ONS3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PAK', 'PAK', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PAK1', 'PAK2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PAK2', 'PAK3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PCS', 'PCS', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PCS1', 'PCS2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PCS2', 'PCS3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PK', 'PK', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PK1', 'PK2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PK2', 'PK3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PSG', 'PSG', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PSG1', 'PSG2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('PSG2', 'PSG3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('RCG', 'RCG', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('RCG1', 'RCG2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('RCG2', 'RCG3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('ROL', 'ROLL', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('ROL1', 'ROLL2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('ROL2', 'ROLL3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('SAK', 'SAK', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('SAK1', 'SAK2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('SAK2', 'SAK3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('SLO', 'SLOP', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('SLO1', 'SLOP2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('SLO2', 'SLOP3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('TPL', 'TPL', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('TPL1', 'TPL2', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1),
('TPL2', 'TPL3', 1, '2026-04-28 19:32:49', NULL, NULL, NULL, 1, 1);

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_supliyer`
--

CREATE TABLE `tbl_supliyer` (
  `KODE` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ALAMAT` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HP` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JANGKAHUTANG` int(11) DEFAULT '0',
  `HUTANGAWAL` decimal(15,0) DEFAULT '0',
  `TOTALHUTANG` decimal(15,0) DEFAULT '0',
  `TOTALBAYAR` decimal(15,0) DEFAULT '0',
  `HUTANGAKHIR` decimal(15,0) DEFAULT '0',
  `Status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Aktif',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_by` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_dirty` tinyint(4) NOT NULL DEFAULT '1',
  `version` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tbl_user`
--

CREATE TABLE `tbl_user` (
  `KODE_USER` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA_USER` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `USER_NAME` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `PWD` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LVL` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Aktif',
  `login_session_key` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email_status` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `password_expire_date` datetime DEFAULT NULL,
  `password_reset_key` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tempbukubesarpembantu`
--

CREATE TABLE `tempbukubesarpembantu` (
  `NOMOR` int(11) DEFAULT '0',
  `JENISTRANSAKSI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTRANSAKSI` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGLTRANSAKSI` datetime DEFAULT NULL,
  `NONOTA` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URAIAN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DEBET` decimal(20,0) DEFAULT '0',
  `KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tempjurnalumum`
--

CREATE TABLE `tempjurnalumum` (
  `NOMOR` int(11) DEFAULT '0',
  `JENISTRANSAKSI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NOTRANSAKSI` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGLTRANSAKSI` datetime DEFAULT NULL,
  `NONOTA` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URAIAN` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DEBET` decimal(20,0) DEFAULT '0',
  `KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `temp_bbpembantu`
--

CREATE TABLE `temp_bbpembantu` (
  `ID` int(11) NOT NULL,
  `NOMOR` int(11) DEFAULT '0',
  `TANGGAL` datetime DEFAULT NULL,
  `NOTA` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ENTITAS` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KETERANGAN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DEBET` decimal(15,0) DEFAULT '0',
  `KREDIT` decimal(15,0) DEFAULT '0',
  `SALDO` decimal(15,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `temp_bon_karyawan`
--

CREATE TABLE `temp_bon_karyawan` (
  `NO` int(11) NOT NULL DEFAULT '0',
  `NOMOR` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `JENIS` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KETERANGAN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DEBET` decimal(15,0) DEFAULT '0',
  `KREDIT` decimal(15,0) DEFAULT '0',
  `SALDO` decimal(15,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `temp_datareferensi`
--

CREATE TABLE `temp_datareferensi` (
  `STATUS` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_AKUN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TYPE_AKUN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `KODE_AKUN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA_AKUN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SUB_AKUN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_DK` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_NRLR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KETERANGAN` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `SALDO_AWAL` decimal(20,0) DEFAULT '0',
  `SALDO_SEBELUMNYA` decimal(20,0) DEFAULT '0',
  `S_DEBET` decimal(20,0) DEFAULT '0',
  `S_KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO_AKHIR` decimal(20,0) DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `temp_jurnal`
--

CREATE TABLE `temp_jurnal` (
  `TYPE_AKUN` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODE_AKUN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_AKUN` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SUB_AKUN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_DK` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AKUN_NLRL` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SALDO_AWAL` decimal(20,0) DEFAULT '0',
  `DEBET` decimal(20,0) DEFAULT '0',
  `KREDIT` decimal(20,0) DEFAULT '0',
  `SALDO_AKHIR` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `temp_labarugi`
--

CREATE TABLE `temp_labarugi` (
  `TANGGAL` datetime DEFAULT NULL,
  `BULAN` int(11) DEFAULT '0',
  `TOTAL` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `temp_loading`
--

CREATE TABLE `temp_loading` (
  `TANGGAL` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `temp_mutasi_barang`
--

CREATE TABLE `temp_mutasi_barang` (
  `FAKTUR` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `JENIS` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY_MASUK` decimal(15,4) DEFAULT '0.0000',
  `QTY_KELUAR` decimal(15,4) DEFAULT '0.0000',
  `SALDO` decimal(15,4) DEFAULT '0.0000',
  `ID_USER` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `temp_supliyerbayar`
--

CREATE TABLE `temp_supliyerbayar` (
  `NO` smallint(6) DEFAULT NULL,
  `KODE` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BAYAR` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `temp_supliyerhutang`
--

CREATE TABLE `temp_supliyerhutang` (
  `NO` smallint(6) DEFAULT NULL,
  `KODE` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HUTANG` decimal(20,0) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `transfer_barang`
--

CREATE TABLE `transfer_barang` (
  `ID_TRANSFER` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_TRANSFER` datetime DEFAULT NULL,
  `LOKASI` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `TOTAL_BARANG` int(11) DEFAULT '0',
  `TOTAL_RUPIAH` decimal(15,0) DEFAULT NULL,
  `ID_USER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `transfer_barang_detail`
--

CREATE TABLE `transfer_barang_detail` (
  `ID_TRANSFER` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TGL_TRANSFER` datetime DEFAULT NULL,
  `LOKASI` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(60) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA` decimal(15,2) DEFAULT '0.00',
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) NOT NULL DEFAULT '1',
  `HARGA_QTY` decimal(15,0) DEFAULT '0',
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `TOTAL` decimal(15,0) DEFAULT '0',
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URUTAN` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `transfer_cabang`
--

CREATE TABLE `transfer_cabang` (
  `ID_TRANSFER` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_TRANSFER` datetime DEFAULT NULL,
  `LOKASI` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI_ASAL` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DARI_CABANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KE_CABANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MODE_KIRIM` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'OFFLINE_EXPORT',
  `STATUS_TRANSFER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `ID_CLOUD_TRANSFER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FILE_MANUAL` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `TOTAL_BARANG` int(11) DEFAULT '0',
  `TOTAL_RUPIAH` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `transfer_cabang_detail`
--

CREATE TABLE `transfer_cabang_detail` (
  `ID_TRANSFER` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TGL_TRANSFER` datetime DEFAULT NULL,
  `LOKASI` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LOKASI_ASAL` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NAMA_BARANG` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HARGA` decimal(15,2) DEFAULT '0.00',
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_SATUAN` int(11) NOT NULL DEFAULT '1',
  `HARGA_QTY` decimal(15,2) DEFAULT '0.00',
  `TOTAL_QTY` decimal(15,4) DEFAULT '0.0000',
  `DITERIMA_QTY` decimal(15,4) DEFAULT '0.0000',
  `STATUS_ITEM` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `TOTAL` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URUTAN` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `transfer_keluar_offline`
--

CREATE TABLE `transfer_keluar_offline` (
  `id` int(11) NOT NULL,
  `id_transfer` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `dari_cabang` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ke_cabang` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `kode_barang` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `nama_barang` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `qty` decimal(15,4) DEFAULT '0.0000',
  `satuan` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `isi_satuan` int(11) DEFAULT '1',
  `qty_satuan` decimal(15,4) DEFAULT '0.0000',
  `keterangan` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `transfer_masuk_manual`
--

CREATE TABLE `transfer_masuk_manual` (
  `id` int(11) NOT NULL,
  `id_transfer` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `sumber_transfer` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'MANUAL',
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `dari_cabang` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ke_cabang` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `STATUS_TERIMA` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `kode_barang` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nama_barang` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `qty` decimal(15,4) DEFAULT '0.0000',
  `satuan` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `isi_satuan` int(11) DEFAULT '1',
  `qty_satuan` decimal(15,4) DEFAULT '0.0000',
  `harga_beli` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `keterangan` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tgl_kirim` datetime DEFAULT NULL,
  `tgl_terima` datetime DEFAULT NULL,
  `id_user_terima` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `catatan_terima` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status_transfer` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `transfer_stok`
--

CREATE TABLE `transfer_stok` (
  `ID_TRANSFER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENIS_TRANSFER` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `URAIAN` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `ID_BARANG_M` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG_M` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY_M` decimal(15,4) DEFAULT '0.0000',
  `SATUAN_M` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_M` decimal(15,4) DEFAULT '0.0000',
  `QTY_SAT_M` decimal(15,4) DEFAULT '0.0000',
  `HARGA_SAT_M` decimal(15,2) DEFAULT '0.00',
  `TOTAL_HARGA_M` decimal(15,0) DEFAULT '0',
  `ID_BARANG_K` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG_K` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY_K` decimal(15,4) DEFAULT '0.0000',
  `SATUAN_K` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISI_K` decimal(15,4) DEFAULT '0.0000',
  `QTY_SAT_K` decimal(15,4) DEFAULT '0.0000',
  `HARGA_SAT_K` decimal(15,2) DEFAULT '0.00',
  `TOTAL_HARGA_K` decimal(15,0) DEFAULT '0',
  `Selisih` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_KOMPUTER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sync_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `transfer_terima_pending`
--

CREATE TABLE `transfer_terima_pending` (
  `id` int(11) NOT NULL,
  `id_cloud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `kode_barang` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_user` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tgl_terima` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur dari tabel `tukarbarang`
--

CREATE TABLE `tukarbarang` (
  `ID_TUKAR` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_PENJUALAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TANGGAL` datetime DEFAULT NULL,
  `DESKRIPSI` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KODEPEL` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMAPEL` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `JENISPEL` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ID_BARANG` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NAMA_BARANG` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `QTY` decimal(15,4) DEFAULT '0.0000',
  `SATUAN` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ISISATUAN` int(11) DEFAULT NULL,
  `QTYSATUAN` decimal(15,4) DEFAULT '0.0000',
  `HARGASATUAN` decimal(15,2) DEFAULT '0.00',
  `DISKON` decimal(15,2) DEFAULT '0.00',
  `TOTALHARGA` decimal(15,2) DEFAULT '0.00',
  `SELISIH` decimal(15,2) DEFAULT '0.00',
  `ID_USER` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Indexes for dumped tables
--

--
-- Indeks untuk tabel `bon_karyawan`
--
ALTER TABLE `bon_karyawan`
  ADD PRIMARY KEY (`FAKTUR`),
  ADD UNIQUE KEY `uq_sync_id_bon_karyawan` (`sync_id`),
  ADD KEY `Bon_karyawan_ID_USER` (`ID_USER`),
  ADD KEY `Bon_karyawan_Nama` (`NAMA`),
  ADD KEY `idx_tanggal_bon` (`TANGGAL`),
  ADD KEY `idx_tanggal_jenis_bon` (`TANGGAL`,`JENIS`),
  ADD KEY `idx_kode_tanggal_bon` (`KODE`,`TANGGAL`),
  ADD KEY `idx_kode_jenis_tanggal_bon` (`KODE`,`JENIS`,`TANGGAL`),
  ADD KEY `idx_faktur_bon` (`FAKTUR`),
  ADD KEY `idx_jenis_bon` (`JENIS`),
  ADD KEY `idx_kode_jenis_bon` (`KODE`,`JENIS`),
  ADD KEY `idx_kode_tanggal_jenis_bon` (`KODE`,`TANGGAL`,`JENIS`);

--
-- Indeks untuk tabel `gaji_karyawan`
--
ALTER TABLE `gaji_karyawan`
  ADD PRIMARY KEY (`NOMOR`),
  ADD UNIQUE KEY `uq_sync_id_gaji_karyawan` (`sync_id`),
  ADD KEY `idx_nomor_gaji` (`NOMOR`),
  ADD KEY `idx_bulan_gaji` (`BULAN`),
  ADD KEY `idx_kode_gaji` (`KODE`);

--
-- Indeks untuk tabel `hakaksesuser`
--
ALTER TABLE `hakaksesuser`
  ADD PRIMARY KEY (`NO`),
  ADD UNIQUE KEY `uq_sync_id_hakaksesuser` (`sync_id`),
  ADD KEY `idx_username_hakakses` (`UserName`),
  ADD KEY `idx_username_role_hakakses` (`UserName`,`Role`),
  ADD KEY `idx_username_module_hakakses` (`UserName`,`ModuleName`),
  ADD KEY `idx_updated_at_hakakses` (`updated_at`),
  ADD KEY `idx_role_hakakses` (`Role`);

--
-- Indeks untuk tabel `history`
--
ALTER TABLE `history`
  ADD PRIMARY KEY (`NO`),
  ADD UNIQUE KEY `uq_sync_id_history` (`sync_id`),
  ADD KEY `idx_tanggal_history` (`TANGGAL`);

--
-- Indeks untuk tabel `historybarang`
--
ALTER TABLE `historybarang`
  ADD PRIMARY KEY (`NO`),
  ADD UNIQUE KEY `uq_sync_id_historybarang` (`sync_id`),
  ADD KEY `HistoryBarang_ID_USER` (`ID_USER`),
  ADD KEY `HistoryBarang_JENIS` (`JENIS`),
  ADD KEY `HistoryBarang_TANGGAL` (`TANGGAL`),
  ADD KEY `idx_faktur_history` (`FAKTUR`),
  ADD KEY `idx_lokasi_jenis_barang_qty` (`LOKASI`,`JENIS`,`ID_BARANG`,`TOTAL_QTY`),
  ADD KEY `idx_barang_jenis_tgl` (`ID_BARANG`,`JENIS`,`TANGGAL`),
  ADD KEY `idx_barang_jenis_tgl_lokasi` (`ID_BARANG`,`JENIS`,`TANGGAL`,`LOKASI`),
  ADD KEY `idx_barang_lokasi_tgl` (`ID_BARANG`,`LOKASI`,`TANGGAL`);

--
-- Indeks untuk tabel `hutang`
--
ALTER TABLE `hutang`
  ADD UNIQUE KEY `uq_sync_id_hutang` (`sync_id`),
  ADD KEY `idx_tgl_pembayaran_hutang` (`TGLPEMBAYARAN`),
  ADD KEY `idx_nobayarhutang` (`NOBAYARHUTANG`),
  ADD KEY `idx_tgl_supplier_hutang` (`TGLPEMBAYARAN`,`NAMASUPLIYER`),
  ADD KEY `idx_namasupliyer_hutang` (`NAMASUPLIYER`),
  ADD KEY `idx_tgl_lokasi_hutang` (`TGLPEMBAYARAN`,`LOKASI`);

--
-- Indeks untuk tabel `hutang_detail`
--
ALTER TABLE `hutang_detail`
  ADD UNIQUE KEY `uq_sync_id_hutang_detail` (`sync_id`),
  ADD KEY `Hutang_Detail_ID_JUAL` (`ID_BELI`),
  ADD KEY `Hutang_Detail_ID_USER_BAYAR` (`ID_USER`),
  ADD KEY `idx_id_bayar_hutang` (`ID_BAYAR`),
  ADD KEY `idx_hutang_detail_jenis_beli` (`JENIS`,`ID_BELI`);

--
-- Indeks untuk tabel `jurnalumum`
--
ALTER TABLE `jurnalumum`
  ADD PRIMARY KEY (`NO`),
  ADD UNIQUE KEY `uq_sync_id_jurnalumum` (`sync_id`),
  ADD KEY `idx_no_transaksi_jurnal` (`NO_TRANSAKSI`),
  ADD KEY `idx_tgl_jenis_transaksi` (`TGL_TRANSAKSI`,`JENIS_TRANSAKSI`),
  ADD KEY `idx_nomor_akun_d_jurnal` (`NOMOR_AKUN_D`,`TGL_TRANSAKSI`),
  ADD KEY `idx_nomor_akun_k_jurnal` (`NOMOR_AKUN_K`,`TGL_TRANSAKSI`),
  ADD KEY `idx_tgl_id_user_jurnal` (`TGL_TRANSAKSI`,`ID_USER`),
  ADD KEY `idx_lokasi_jurnal` (`LOKASI`),
  ADD KEY `idx_covering_akun_d` (`TGL_TRANSAKSI`,`NOMOR_AKUN_D`,`NOMINAL`),
  ADD KEY `idx_covering_akun_k` (`TGL_TRANSAKSI`,`NOMOR_AKUN_K`,`NOMINAL`),
  ADD KEY `idx_updated_at_jurnal` (`updated_at`);

--
-- Indeks untuk tabel `pembelian`
--
ALTER TABLE `pembelian`
  ADD PRIMARY KEY (`ID_PEMBELIAN`),
  ADD UNIQUE KEY `uq_sync_id_pembelian` (`sync_id`),
  ADD KEY `idx_tgl_beli` (`TGL_BELI`),
  ADD KEY `idx_supplier_tagihan` (`ID_SUPPLIER`,`TAGIHAN`),
  ADD KEY `idx_id_supplier` (`ID_SUPPLIER`),
  ADD KEY `idx_status_transaksi_beli` (`STATUS_TRANSAKSI_BELI`),
  ADD KEY `idx_jenis_bayar` (`JENIS_BAYAR`),
  ADD KEY `idx_id_user_pembelian` (`ID_USER`),
  ADD KEY `idx_updated_at_beli` (`updated_at`),
  ADD KEY `idx_tgl_jenis_bayar` (`TGL_BELI`,`JENIS_BAYAR`),
  ADD KEY `idx_supplier_status` (`ID_SUPPLIER`,`STATUS_TRANSAKSI_BELI`),
  ADD KEY `idx_jatuh_tempo_status_beli` (`JATUH_TEMPO`,`STATUS_TRANSAKSI_BELI`),
  ADD KEY `idx_tgl_bayar_beli` (`TGL_BAYAR`),
  ADD KEY `idx_status_jual_beli` (`STATUS_JUAL`),
  ADD KEY `idx_status_lokasi_beli` (`STATUS_TRANSAKSI_BELI`,`LOKASI`),
  ADD KEY `idx_tgl_lokasi_beli` (`TGL_BELI`,`LOKASI`);

--
-- Indeks untuk tabel `pembelian_detail`
--
ALTER TABLE `pembelian_detail`
  ADD PRIMARY KEY (`NO`),
  ADD UNIQUE KEY `uq_sync_id_pembelian_detail` (`sync_id`),
  ADD KEY `idx_faktur_beli` (`FAKTUR_BELI`),
  ADD KEY `idx_faktur_beli_barang` (`FAKTUR_BELI`,`ID_BARANG`),
  ADD KEY `idx_tanggal_masuk_beli` (`TANGGAL_MASUK`),
  ADD KEY `idx_tgl_masuk_barang` (`TANGGAL_MASUK`,`ID_BARANG`),
  ADD KEY `idx_id_barang_tgl_masuk` (`ID_BARANG`,`TANGGAL_MASUK`),
  ADD KEY `idx_id_barang_beli` (`ID_BARANG`);

--
-- Indeks untuk tabel `pembelian_ditahan`
--
ALTER TABLE `pembelian_ditahan`
  ADD PRIMARY KEY (`ID_PEMBELIAN`),
  ADD KEY `idx_id_pembelian_ditahan` (`ID_PEMBELIAN`),
  ADD KEY `idx_lokasi_pembelian_ditahan` (`LOKASI`);

--
-- Indeks untuk tabel `penjualan`
--
ALTER TABLE `penjualan`
  ADD PRIMARY KEY (`ID_PENJUALAN`),
  ADD UNIQUE KEY `uq_sync_id_penjualan` (`sync_id`),
  ADD KEY `idx_tgl_transaksi` (`TGL_TRANSAKSI`),
  ADD KEY `idx_pelanggan_tagihan` (`ID_PELANGGAN`,`SISA_TAGIHAN`),
  ADD KEY `idx_id_pelanggan` (`ID_PELANGGAN`),
  ADD KEY `idx_status_transaksi` (`STATUS_TRANSAKSI`),
  ADD KEY `idx_lokasibarang` (`LOKASIBARANG`),
  ADD KEY `idx_id_user_penjualan` (`ID_USER`),
  ADD KEY `idx_updated_at_jual` (`updated_at`),
  ADD KEY `idx_tgl_kode_akun_jual` (`TGL_TRANSAKSI`,`KODE_AKUN`),
  ADD KEY `idx_tgl_kode_akun_tf` (`TGL_TRANSAKSI`,`KODE_AKUN_TF`),
  ADD KEY `idx_pelanggan_status` (`ID_PELANGGAN`,`STATUS_TRANSAKSI`),
  ADD KEY `idx_lokasi_tanggal` (`LOKASIBARANG`,`TGL_TRANSAKSI`),
  ADD KEY `idx_pelanggan_tgl_jual` (`ID_PELANGGAN`,`TGL_TRANSAKSI`),
  ADD KEY `idx_lokasi_tgl_pelanggan` (`LOKASIBARANG`,`TGL_TRANSAKSI`,`ID_PELANGGAN`),
  ADD KEY `idx_jatuh_tempo_status_jual` (`JATUH_TEMPO`,`STATUS_TRANSAKSI`),
  ADD KEY `idx_id_sales_tgl_jual` (`ID_SALES`,`TGL_TRANSAKSI`),
  ADD KEY `idx_type_akun_jual` (`TYPE_AKUN`),
  ADD KEY `idx_tgl_type_akun_jual` (`TGL_TRANSAKSI`,`TYPE_AKUN`),
  ADD KEY `idx_id_tgl_lokasi` (`ID_PENJUALAN`,`TGL_TRANSAKSI`,`LOKASIBARANG`);

--
-- Indeks untuk tabel `penjualan_detail`
--
ALTER TABLE `penjualan_detail`
  ADD UNIQUE KEY `uq_sync_id_penjualan_detail` (`sync_id`),
  ADD KEY `idx_faktur_jual` (`FAKTUR_JUAL`),
  ADD KEY `idx_faktur_barang` (`FAKTUR_JUAL`,`ID_BARANG`),
  ADD KEY `idx_tgl_lokasi_jual` (`TANGGAL_JUAL`,`LOKASIBARANG`),
  ADD KEY `idx_pelanggan_tgl_jual` (`ID_PELANGGAN`,`TANGGAL_JUAL`),
  ADD KEY `idx_id_barang_detail_jual` (`ID_BARANG`),
  ADD KEY `idx_tgl_pelanggan_user` (`TANGGAL_JUAL`,`NAMA_PELANGGAN`,`ID_USER`),
  ADD KEY `idx_barang_faktur` (`ID_BARANG`,`FAKTUR_JUAL`),
  ADD KEY `idx_tgl_lokasi_barang` (`TANGGAL_JUAL`,`LOKASIBARANG`,`ID_BARANG`);

--
-- Indeks untuk tabel `penjualan_ditahan`
--
ALTER TABLE `penjualan_ditahan`
  ADD KEY `idx_faktur_jual_ditahan` (`FAKTUR_JUAL`);

--
-- Indeks untuk tabel `penjualan_ditahan_detail`
--
ALTER TABLE `penjualan_ditahan_detail`
  ADD KEY `idx_faktur_jual_ditahan_detail` (`FAKTUR_JUAL`),
  ADD KEY `idx_id_barang_ditahan_detail` (`ID_BARANG`);

--
-- Indeks untuk tabel `piutang`
--
ALTER TABLE `piutang`
  ADD UNIQUE KEY `uq_sync_id_piutang` (`sync_id`),
  ADD KEY `idx_tgl_bayar_piutang` (`TGL_BAYAR`),
  ADD KEY `idx_id_bayar_piutang` (`ID_BAYAR_PIUTANG`),
  ADD KEY `idx_tgl_pelanggan_piutang` (`TGL_BAYAR`,`NAMA_PELANGGAN`),
  ADD KEY `idx_nama_pelanggan_piutang` (`NAMA_PELANGGAN`),
  ADD KEY `idx_tgl_lokasi_piutang` (`TGL_BAYAR`,`LOKASI`);

--
-- Indeks untuk tabel `piutang_detail`
--
ALTER TABLE `piutang_detail`
  ADD UNIQUE KEY `uq_sync_id_piutang_detail` (`sync_id`),
  ADD KEY `idx_id_bayar_piutang_detail` (`ID_BAYAR`),
  ADD KEY `idx_id_jual` (`ID_JUAL`),
  ADD KEY `idx_piutang_detail_jenis_jual` (`JENIS`,`ID_JUAL`);

--
-- Indeks untuk tabel `poin_barang`
--
ALTER TABLE `poin_barang`
  ADD PRIMARY KEY (`ID_BARANG`),
  ADD UNIQUE KEY `uq_sync_id_poin_barang` (`sync_id`),
  ADD KEY `idx_poin_barang_aktif` (`AKTIF`),
  ADD KEY `idx_poin_barang_is_dirty` (`is_dirty`),
  ADD KEY `idx_poin_barang_id_cloud` (`id_cloud`),
  ADD KEY `idx_poin_barang_updated_at` (`updated_at`);

--
-- Indeks untuk tabel `poin_config`
--
ALTER TABLE `poin_config`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `uq_sync_id_poin_config` (`sync_id`),
  ADD KEY `idx_poin_config_updated_at` (`updated_at`);

--
-- Indeks untuk tabel `poin_ledger`
--
ALTER TABLE `poin_ledger`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `uq_sync_id_poin_ledger` (`sync_id`),
  ADD KEY `idx_poin_ledger_pelanggan` (`KODE_PELANGGAN`),
  ADD KEY `idx_poin_ledger_referensi` (`NO_REFERENSI`),
  ADD KEY `idx_poin_ledger_created_at` (`created_at`),
  ADD KEY `idx_poin_ledger_updated_at` (`updated_at`);

--
-- Indeks untuk tabel `retur_pembelian`
--
ALTER TABLE `retur_pembelian`
  ADD UNIQUE KEY `uq_sync_id_retur_pembelian` (`sync_id`),
  ADD KEY `retur_pembelian_ID_KOMPUTER` (`ID_KOMPUTER`),
  ADD KEY `retur_pembelian_ID_PEMBELIAN` (`ID_PEMBELIAN`),
  ADD KEY `retur_pembelian_ID_RETUR_PEMBELIAN` (`ID_RETUR_PEMBELIAN`),
  ADD KEY `retur_pembelian_ID_SUPPLIER` (`ID_SUPPLIER`),
  ADD KEY `idx_tgl_retur_beli` (`TGL_RETUR_BELI`),
  ADD KEY `idx_kode_rekening_retur_beli` (`KODE_REKENING`),
  ADD KEY `idx_id_user_retur_beli` (`ID_USER`);

--
-- Indeks untuk tabel `retur_pembelian_detail`
--
ALTER TABLE `retur_pembelian_detail`
  ADD UNIQUE KEY `uq_sync_id_retur_pembelian_detail` (`sync_id`),
  ADD KEY `idx_id_retur_pembelian` (`ID_RETUR_PEMBELIAN`),
  ADD KEY `idx_tgl_retur_beli_detail` (`TGL_RETUR_BELI`),
  ADD KEY `idx_tgl_supplier_retur_beli` (`TGL_RETUR_BELI`,`NAMA_SUPLIYER`),
  ADD KEY `idx_penyimpanan_retur_beli_detail` (`PENYIMPANAN`);

--
-- Indeks untuk tabel `retur_penjualan`
--
ALTER TABLE `retur_penjualan`
  ADD UNIQUE KEY `uq_sync_id_retur_penjualan` (`sync_id`),
  ADD KEY `idx_tgl_retur_jual` (`TGL_RETUR_JUAL`),
  ADD KEY `idx_id_penjualan_retur` (`ID_PENJUALAN`),
  ADD KEY `idx_id_retur_penjualan_header` (`ID_RETUR_PENJUALAN`),
  ADD KEY `idx_kode_rekening_retur_jual` (`KODE_REKENING`),
  ADD KEY `idx_id_user_retur_jual` (`ID_USER`);

--
-- Indeks untuk tabel `retur_penjualan_detail`
--
ALTER TABLE `retur_penjualan_detail`
  ADD UNIQUE KEY `uq_sync_id_retur_penjualan_detail` (`sync_id`),
  ADD KEY `idx_id_retur_penjualan` (`ID_RETUR_PENJUALAN`),
  ADD KEY `idx_retur_jual_barang` (`ID_RETUR_PENJUALAN`,`ID_BARANG`),
  ADD KEY `idx_tgl_retur_jual_detail` (`TGL_RETUR_JUAL`),
  ADD KEY `idx_tgl_pelanggan_retur_jual` (`TGL_RETUR_JUAL`,`NAMA_PELANGGAN`),
  ADD KEY `idx_lokasi_retur_jual_detail` (`LOKASI`);

--
-- Indeks untuk tabel `sales_order`
--
ALTER TABLE `sales_order`
  ADD PRIMARY KEY (`ID_PENJUALAN`),
  ADD UNIQUE KEY `sync_id_unique` (`sync_id`);

--
-- Indeks untuk tabel `sales_order_detail`
--
ALTER TABLE `sales_order_detail`
  ADD UNIQUE KEY `sync_id_unique` (`sync_id`),
  ADD KEY `FK_SALES_ORDER` (`FAKTUR_JUAL`);

--
-- Indeks untuk tabel `stoktambahkurang`
--
ALTER TABLE `stoktambahkurang`
  ADD UNIQUE KEY `uq_sync_id_stoktambahkurang` (`sync_id`),
  ADD KEY `idx_tanggal_stok_tk` (`TANGGAL`),
  ADD KEY `idx_lokasi_stok_tk` (`LOKASI`),
  ADD KEY `idx_id_barang_stok_tk` (`ID_BARANG`),
  ADD KEY `idx_faktur_stok_tk` (`FAKTUR`);

--
-- Indeks untuk tabel `stok_opname`
--
ALTER TABLE `stok_opname`
  ADD UNIQUE KEY `uq_sync_id_stok_opname` (`sync_id`),
  ADD KEY `idx_tanggal_opname` (`TANGGAL`),
  ADD KEY `idx_id_stok_opname` (`ID_STOK_OPNAME`),
  ADD KEY `idx_id_barang_opname` (`ID_BARANG`),
  ADD KEY `idx_barang_tanggal` (`ID_BARANG`,`TANGGAL`),
  ADD KEY `idx_tanggal_lokasi_opname` (`TANGGAL`,`LOKASI`),
  ADD KEY `idx_lokasi_tanggal_opname` (`LOKASI`,`TANGGAL`);

--
-- Indeks untuk tabel `surat_jalan`
--
ALTER TABLE `surat_jalan`
  ADD PRIMARY KEY (`NOTA`),
  ADD UNIQUE KEY `uq_sync_id_surat_jalan` (`sync_id`),
  ADD KEY `idx_tgl_pengiriman` (`TGL_PENGIRIMAN`),
  ADD KEY `idx_nota_sj` (`NOTA`),
  ADD KEY `idx_kode_supir_tgl` (`KODE_SUPIR`,`TGL_PENGIRIMAN`),
  ADD KEY `idx_kode_helper1_tgl` (`KODE_HELPER1`,`TGL_PENGIRIMAN`),
  ADD KEY `idx_kode_helper2_tgl` (`KODE_HELPER2`,`TGL_PENGIRIMAN`);

--
-- Indeks untuk tabel `surat_jalan_detail`
--
ALTER TABLE `surat_jalan_detail`
  ADD UNIQUE KEY `uq_sync_id_surat_jalan_detail` (`sync_id`),
  ADD KEY `idx_nota_sj_detail` (`NOTA`);

--
-- Indeks untuk tabel `sync_config`
--
ALTER TABLE `sync_config`
  ADD PRIMARY KEY (`kunci`);

--
-- Indeks untuk tabel `sync_log`
--
ALTER TABLE `sync_log`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_waktu_log` (`waktu`),
  ADD KEY `idx_jenis_log` (`jenis`);

--
-- Indeks untuk tabel `sync_queue`
--
ALTER TABLE `sync_queue`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_status_queue` (`status`),
  ADD KEY `idx_tabel_queue` (`tabel`);

--
-- Indeks untuk tabel `tbl_armada`
--
ALTER TABLE `tbl_armada`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_armada` (`sync_id`),
  ADD KEY `idx_nopol_armada` (`NOPOL`),
  ADD KEY `idx_updated_at_armada` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`);

--
-- Indeks untuk tabel `tbl_audit_config`
--
ALTER TABLE `tbl_audit_config`
  ADD PRIMARY KEY (`config_key`);

--
-- Indeks untuk tabel `tbl_audit_trail`
--
ALTER TABLE `tbl_audit_trail`
  ADD PRIMARY KEY (`id_audit`),
  ADD KEY `idx_audit_waktu` (`waktu_aksi`),
  ADD KEY `idx_audit_user` (`id_user`),
  ADD KEY `idx_audit_id` (`identifier`);

--
-- Indeks untuk tabel `tbl_audit_trail_arsip`
--
ALTER TABLE `tbl_audit_trail_arsip`
  ADD PRIMARY KEY (`id_audit`),
  ADD KEY `idx_audit_waktu` (`waktu_aksi`),
  ADD KEY `idx_audit_user` (`id_user`),
  ADD KEY `idx_audit_id` (`identifier`);

--
-- Indeks untuk tabel `tbl_barang`
--
ALTER TABLE `tbl_barang`
  ADD PRIMARY KEY (`ID_BARANG`),
  ADD UNIQUE KEY `uq_sync_id_tbl_barang` (`sync_id`),
  ADD KEY `idx_nama_barang` (`NAMA_BARANG`),
  ADD KEY `idx_barcode_kecil` (`BARCODE_KECIL`),
  ADD KEY `idx_barcode_sedang` (`BARCODE_SEDANG`),
  ADD KEY `idx_barcode_besar` (`BARCODE_BESAR`),
  ADD KEY `idx_updated_at_barang` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_stok_minimum` (`STOK_MIN`,`STOK_TOKO`,`STOK_GUDANG`),
  ADD KEY `idx_kategori_barang` (`NAMA_KATEGORI`),
  ADD KEY `idx_kode_kategori_barang` (`KODE_KATEGORI`),
  ADD KEY `idx_status_barang` (`STATUS`),
  ADD KEY `idx_status_nama_barang` (`STATUS`,`NAMA_BARANG`),
  ADD KEY `idx_stok_toko_id_nama_harga` (`STOK_TOKO`,`ID_BARANG`,`NAMA_BARANG`,`HARGA_BELI`),
  ADD KEY `idx_stok_gudang_id_nama_harga` (`STOK_GUDANG`,`ID_BARANG`,`NAMA_BARANG`,`HARGA_BELI`),
  ADD KEY `idx_barang_is_paket` (`IS_PAKET`);
ALTER TABLE `tbl_barang` ADD FULLTEXT KEY `ft_nama_barang` (`NAMA_BARANG`);

--
-- Indeks untuk tabel `tbl_cabang`
--
ALTER TABLE `tbl_cabang`
  ADD PRIMARY KEY (`kode_cabang`),
  ADD UNIQUE KEY `uq_sync_id_tbl_cabang` (`sync_id`),
  ADD KEY `idx_nama_cabang` (`nama_cabang`),
  ADD KEY `idx_sumber_cabang` (`sumber`),
  ADD KEY `idx_updated_at_cabang` (`updated_at`),
  ADD KEY `idx_id_cloud_cabang` (`id_cloud`);

--
-- Indeks untuk tabel `tbl_datareferensi`
--
ALTER TABLE `tbl_datareferensi`
  ADD PRIMARY KEY (`KODE_AKUN`),
  ADD UNIQUE KEY `uq_sync_id_tbl_datareferensi` (`sync_id`),
  ADD KEY `idx_nama_akun` (`NAMA_AKUN`),
  ADD KEY `idx_type_akun` (`TYPE_AKUN`),
  ADD KEY `idx_kode_akun_ref` (`KODE_AKUN`),
  ADD KEY `idx_sub_akun` (`SUB_AKUN`);

--
-- Indeks untuk tabel `tbl_gaji`
--
ALTER TABLE `tbl_gaji`
  ADD PRIMARY KEY (`KODE`);

--
-- Indeks untuk tabel `tbl_karyawan`
--
ALTER TABLE `tbl_karyawan`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_karyawan` (`sync_id`),
  ADD KEY `idx_nama_karyawan` (`NAMA`),
  ADD KEY `idx_status_nama` (`Status`,`NAMA`),
  ADD KEY `idx_kode_karyawan` (`KODE`),
  ADD KEY `idx_saldo_akhir_karyawan` (`SALDOAKHIR`);

--
-- Indeks untuk tabel `tbl_kategori`
--
ALTER TABLE `tbl_kategori`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_kategori` (`sync_id`),
  ADD KEY `idx_updated_at_kategori` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_nama_kategori` (`NAMA`),
  ADD KEY `idx_kode_kategori` (`KODE`);

--
-- Indeks untuk tabel `tbl_merk`
--
ALTER TABLE `tbl_merk`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_merk` (`sync_id`),
  ADD KEY `idx_updated_at_merk` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_nama_merk` (`NAMA`),
  ADD KEY `idx_kode_merk` (`KODE`);

--
-- Indeks untuk tabel `tbl_pelanggan`
--
ALTER TABLE `tbl_pelanggan`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_pelanggan` (`sync_id`),
  ADD KEY `idx_nama_pelanggan` (`NAMA`),
  ADD KEY `idx_updated_at_pelanggan` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_status_pelanggan` (`Status`),
  ADD KEY `idx_status_nama_pelanggan` (`Status`,`NAMA`);

--
-- Indeks untuk tabel `tbl_perusahaan`
--
ALTER TABLE `tbl_perusahaan`
  ADD PRIMARY KEY (`KODE`);

--
-- Indeks untuk tabel `tbl_proses_rakitan`
--
ALTER TABLE `tbl_proses_rakitan`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `no_rakitan` (`no_rakitan`);

--
-- Indeks untuk tabel `tbl_proses_rakitan_detail`
--
ALTER TABLE `tbl_proses_rakitan_detail`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_id_proses` (`id_proses`);

--
-- Indeks untuk tabel `tbl_rakitan_bom`
--
ALTER TABLE `tbl_rakitan_bom`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_bom` (`kode_rakitan`,`kode_komponen`);

--
-- Indeks untuk tabel `tbl_satuan`
--
ALTER TABLE `tbl_satuan`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_satuan` (`sync_id`),
  ADD KEY `idx_nama_satuan` (`NAMA`),
  ADD KEY `idx_updated_at_satuan` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_kode_satuan` (`KODE`),
  ADD KEY `idx_isi_satuan` (`ISI`);

--
-- Indeks untuk tabel `tbl_supliyer`
--
ALTER TABLE `tbl_supliyer`
  ADD PRIMARY KEY (`KODE`),
  ADD UNIQUE KEY `uq_sync_id_tbl_supliyer` (`sync_id`),
  ADD KEY `idx_nama_supliyer` (`NAMA`),
  ADD KEY `idx_status_nama` (`Status`,`NAMA`),
  ADD KEY `idx_updated_at_supliyer` (`updated_at`),
  ADD KEY `idx_is_dirty` (`is_dirty`),
  ADD KEY `idx_id_cloud` (`id_cloud`);

--
-- Indeks untuk tabel `tbl_user`
--
ALTER TABLE `tbl_user`
  ADD PRIMARY KEY (`KODE_USER`),
  ADD UNIQUE KEY `uq_sync_id_tbl_user` (`sync_id`),
  ADD KEY `idx_status_user` (`status`),
  ADD KEY `idx_username_user` (`USER_NAME`),
  ADD KEY `idx_username_pwd_status` (`USER_NAME`,`PWD`,`status`);

--
-- Indeks untuk tabel `temp_bbpembantu`
--
ALTER TABLE `temp_bbpembantu`
  ADD PRIMARY KEY (`ID`);

--
-- Indeks untuk tabel `temp_bon_karyawan`
--
ALTER TABLE `temp_bon_karyawan`
  ADD PRIMARY KEY (`NO`);

--
-- Indeks untuk tabel `temp_datareferensi`
--
ALTER TABLE `temp_datareferensi`
  ADD PRIMARY KEY (`KODE_AKUN`),
  ADD UNIQUE KEY `sync_id` (`sync_id`);

--
-- Indeks untuk tabel `transfer_barang`
--
ALTER TABLE `transfer_barang`
  ADD PRIMARY KEY (`ID_TRANSFER`),
  ADD UNIQUE KEY `uq_sync_id_transfer_barang` (`sync_id`),
  ADD KEY `idx_tgl_transfer_barang` (`TGL_TRANSFER`);

--
-- Indeks untuk tabel `transfer_barang_detail`
--
ALTER TABLE `transfer_barang_detail`
  ADD UNIQUE KEY `uq_sync_id_transfer_barang_detail` (`sync_id`),
  ADD KEY `idx_id_transfer_barang_detail` (`ID_TRANSFER`),
  ADD KEY `idx_id_barang_transfer` (`ID_BARANG`),
  ADD KEY `idx_transfer_barang_id` (`ID_TRANSFER`,`ID_BARANG`),
  ADD KEY `idx_tgl_transfer_detail` (`TGL_TRANSFER`);

--
-- Indeks untuk tabel `transfer_cabang`
--
ALTER TABLE `transfer_cabang`
  ADD PRIMARY KEY (`ID_TRANSFER`),
  ADD UNIQUE KEY `uq_sync_id_transfer_cabang` (`sync_id`),
  ADD KEY `idx_tgl_transfer_cabang` (`TGL_TRANSFER`),
  ADD KEY `idx_status_transfer_cabang` (`STATUS_TRANSFER`),
  ADD KEY `idx_mode_kirim_cabang` (`MODE_KIRIM`),
  ADD KEY `idx_dari_ke_cabang` (`DARI_CABANG`,`KE_CABANG`),
  ADD KEY `idx_ke_status_cabang` (`KE_CABANG`,`STATUS_TRANSFER`),
  ADD KEY `idx_cloud_transfer_cabang` (`ID_CLOUD_TRANSFER`),
  ADD KEY `idx_lokasi_transfer_cabang` (`LOKASI`);

--
-- Indeks untuk tabel `transfer_cabang_detail`
--
ALTER TABLE `transfer_cabang_detail`
  ADD UNIQUE KEY `uq_sync_id_transfer_cabang_detail` (`sync_id`),
  ADD KEY `idx_id_transfer_cabang_detail` (`ID_TRANSFER`),
  ADD KEY `idx_id_barang_transfer_cabang` (`ID_BARANG`),
  ADD KEY `idx_transfer_cabang_id_barang` (`ID_TRANSFER`,`ID_BARANG`),
  ADD KEY `idx_tgl_transfer_cabang_detail` (`TGL_TRANSFER`),
  ADD KEY `idx_status_item_transfer_cabang` (`STATUS_ITEM`),
  ADD KEY `idx_lokasi_transfer_cabang_detail` (`LOKASI`);

--
-- Indeks untuk tabel `transfer_keluar_offline`
--
ALTER TABLE `transfer_keluar_offline`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_status` (`status`),
  ADD KEY `idx_id_transfer` (`id_transfer`);

--
-- Indeks untuk tabel `transfer_masuk_manual`
--
ALTER TABLE `transfer_masuk_manual`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uk_id_cloud` (`id_cloud`),
  ADD KEY `idx_status` (`status_transfer`),
  ADD KEY `idx_id_cloud` (`id_cloud`),
  ADD KEY `idx_id_transfer_masuk_manual` (`id_transfer`),
  ADD KEY `idx_ke_status_transfer_masuk_manual` (`ke_cabang`,`status_transfer`),
  ADD KEY `idx_kode_barang_transfer_masuk_manual` (`kode_barang`);

--
-- Indeks untuk tabel `transfer_stok`
--
ALTER TABLE `transfer_stok`
  ADD UNIQUE KEY `uq_sync_id_transfer_stok` (`sync_id`),
  ADD KEY `idx_tanggal_transfer_stok` (`TANGGAL`),
  ADD KEY `idx_id_transfer_stok` (`ID_TRANSFER`),
  ADD KEY `idx_jenis_transfer` (`JENIS_TRANSFER`);

--
-- Indeks untuk tabel `transfer_terima_pending`
--
ALTER TABLE `transfer_terima_pending`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uk_cloud_kode` (`id_cloud`,`kode_barang`);

--
-- AUTO_INCREMENT untuk tabel yang dibuang
--

--
-- AUTO_INCREMENT untuk tabel `hakaksesuser`
--
ALTER TABLE `hakaksesuser`
  MODIFY `NO` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2275284;

--
-- AUTO_INCREMENT untuk tabel `history`
--
ALTER TABLE `history`
  MODIFY `NO` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `historybarang`
--
ALTER TABLE `historybarang`
  MODIFY `NO` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `jurnalumum`
--
ALTER TABLE `jurnalumum`
  MODIFY `NO` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT untuk tabel `pembelian_detail`
--
ALTER TABLE `pembelian_detail`
  MODIFY `NO` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `poin_config`
--
ALTER TABLE `poin_config`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT untuk tabel `poin_ledger`
--
ALTER TABLE `poin_ledger`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `sync_log`
--
ALTER TABLE `sync_log`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `sync_queue`
--
ALTER TABLE `sync_queue`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `tbl_audit_trail`
--
ALTER TABLE `tbl_audit_trail`
  MODIFY `id_audit` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `tbl_audit_trail_arsip`
--
ALTER TABLE `tbl_audit_trail_arsip`
  MODIFY `id_audit` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `tbl_proses_rakitan`
--
ALTER TABLE `tbl_proses_rakitan`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `tbl_proses_rakitan_detail`
--
ALTER TABLE `tbl_proses_rakitan_detail`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `tbl_rakitan_bom`
--
ALTER TABLE `tbl_rakitan_bom`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `temp_bbpembantu`
--
ALTER TABLE `temp_bbpembantu`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `transfer_keluar_offline`
--
ALTER TABLE `transfer_keluar_offline`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `transfer_masuk_manual`
--
ALTER TABLE `transfer_masuk_manual`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT untuk tabel `transfer_terima_pending`
--
ALTER TABLE `transfer_terima_pending`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
