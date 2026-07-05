DROP PROCEDURE IF EXISTS `sp_bat_bon_semua_karyawan`;
DELIMITER //
sp_bat_bon_semua_karyawan

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_bat_bon_semua_karyawan`()
BEGIN
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
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
