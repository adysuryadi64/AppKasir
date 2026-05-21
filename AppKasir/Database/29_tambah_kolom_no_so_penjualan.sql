-- Menambahkan kolom NO_SO ke tabel penjualan untuk menyimpan referensi Sales Order saat dikonversi menjadi Penjualan
ALTER TABLE `penjualan` ADD COLUMN `NO_SO` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL AFTER `ID_PENJUALAN`;
