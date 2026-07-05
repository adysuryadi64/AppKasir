DROP PROCEDURE IF EXISTS `optimize_semua_tabel`;
DELIMITER //
optimize_semua_tabel

CREATE DEFINER=`root`@`localhost` PROCEDURE `optimize_semua_tabel`()
BEGIN
    DECLARE selesai INT DEFAULT 0;
    DECLARE nama_tabel VARCHAR(100);
    DECLARE cur CURSOR FOR
        SELECT table_name
        FROM information_schema.tables
        WHERE table_schema = DATABASE()
          AND table_type = 'BASE TABLE'
        ORDER BY (data_length + index_length) DESC;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET selesai = 1;

    OPEN cur;
    loop_tabel: LOOP
        FETCH cur INTO nama_tabel;
        IF selesai = 1 THEN LEAVE loop_tabel; END IF;
        SET @sql = CONCAT('OPTIMIZE TABLE `', nama_tabel, '`');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END LOOP;
    CLOSE cur;
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
