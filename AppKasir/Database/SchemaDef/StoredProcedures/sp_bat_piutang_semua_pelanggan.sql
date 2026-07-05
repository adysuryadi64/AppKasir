DROP PROCEDURE IF EXISTS `sp_bat_piutang_semua_pelanggan`;
DELIMITER //
sp_bat_piutang_semua_pelanggan

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_bat_piutang_semua_pelanggan`()
BEGIN
    UPDATE tbl_pelanggan p
    LEFT JOIN (
        SELECT ID_PELANGGAN, SUM(IFNULL(SISA_TAGIHAN, 0)) AS HUTANG
        FROM penjualan
        GROUP BY ID_PELANGGAN
    ) x ON x.ID_PELANGGAN = p.KODE
    SET p.HUTANGAKHIR = IFNULL(x.HUTANG, 0) + IFNULL(p.HUTANGAWAL, 0);
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
