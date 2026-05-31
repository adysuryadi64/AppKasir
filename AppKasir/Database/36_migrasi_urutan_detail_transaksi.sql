-- ============================================================
-- Migrasi 36 — Tambah kolom URUTAN di tabel detail transaksi
--
-- Tujuan:
--   Menyimpan urutan input item per transaksi agar cetakan
--   receipt mencetak sesuai urutan input, bukan alfabetis.
--
-- Latar belakang:
--   Semua modul cetak (ModulePrinter*.vb) menggunakan
--   ORDER BY NAMA_BARANG yang mengabaikan urutan input user.
--   Dengan kolom URUTAN, cetakan bisa ORDER BY URUTAN.
--
-- Aman dijalankan berulang kali (idempoten).
-- ============================================================

DELIMITER $$
DROP PROCEDURE IF EXISTS AddColumnSafely36$$
CREATE PROCEDURE AddColumnSafely36(
    IN p_table_name  VARCHAR(64),
    IN p_column_name VARCHAR(64),
    IN p_column_def  TEXT
)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = p_table_name
          AND COLUMN_NAME  = p_column_name
    ) THEN
        SET @sql = CONCAT(
            'ALTER TABLE ', p_table_name,
            ' ADD COLUMN ', p_column_name, ' ', p_column_def
        );
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$

DELIMITER ;

CALL AddColumnSafely36('penjualan_detail',         'URUTAN', 'int(11) NOT NULL DEFAULT 0');
CALL AddColumnSafely36('pembelian_detail',         'URUTAN', 'int(11) NOT NULL DEFAULT 0');
CALL AddColumnSafely36('retur_penjualan_detail',    'URUTAN', 'int(11) NOT NULL DEFAULT 0');
CALL AddColumnSafely36('retur_pembelian_detail',    'URUTAN', 'int(11) NOT NULL DEFAULT 0');
CALL AddColumnSafely36('sales_order_detail',        'URUTAN', 'int(11) NOT NULL DEFAULT 0');
CALL AddColumnSafely36('transfer_barang_detail',    'URUTAN', 'int(11) NOT NULL DEFAULT 0');
CALL AddColumnSafely36('transfer_cabang_detail',    'URUTAN', 'int(11) NOT NULL DEFAULT 0');
CALL AddColumnSafely36('stok_opname',               'URUTAN', 'int(11) NOT NULL DEFAULT 0');
CALL AddColumnSafely36('penjualan_ditahan_detail',  'URUTAN', 'int(11) NOT NULL DEFAULT 0');
CALL AddColumnSafely36('pembelian_ditahan_detail',  'URUTAN', 'int(11) NOT NULL DEFAULT 0');

DROP PROCEDURE IF EXISTS AddColumnSafely36;
