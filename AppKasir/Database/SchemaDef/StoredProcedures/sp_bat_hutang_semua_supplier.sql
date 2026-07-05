DROP PROCEDURE IF EXISTS `sp_bat_hutang_semua_supplier`;
DELIMITER //
sp_bat_hutang_semua_supplier

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_bat_hutang_semua_supplier`()
BEGIN
    UPDATE tbl_supliyer s
    LEFT JOIN (
        SELECT ID_SUPPLIER, SUM(IFNULL(TAGIHAN, 0)) AS HUTANG
        FROM pembelian
        GROUP BY ID_SUPPLIER
    ) x ON x.ID_SUPPLIER = s.KODE
    SET s.HUTANGAKHIR = IFNULL(x.HUTANG, 0) + IFNULL(s.HUTANGAWAL, 0);
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
