DROP PROCEDURE IF EXISTS `sp_val_pembelian_harga_beli_vs_jual`;
DELIMITER //
sp_val_pembelian_harga_beli_vs_jual

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_val_pembelian_harga_beli_vs_jual`(
    IN  p_id_barang           VARCHAR(50),
    IN  p_harga_beli_total    DECIMAL(15,2),
    IN  p_qty_sat             DECIMAL(15,4),
    OUT p_rugi_kritis         TINYINT(1),    -- Rugi di SEMUA level (BLOCK)
    OUT p_rugi_umum           TINYINT(1),    -- Rugi di harga umum
    OUT p_rugi_partai         TINYINT(1),    -- Rugi di harga partai
    OUT p_harga_jual_min      DECIMAL(15,2), -- Harga jual minimum per satuan kecil
    OUT p_harga_jual_umum     DECIMAL(15,2), -- Harga jual umum total
    OUT p_harga_jual_partai   DECIMAL(15,2)  -- Harga jual partai total
)
sp_val_pembelian_harga_beli_vs_jual: BEGIN
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
    
END
utf8mb4
utf8mb4_0900_ai_ci
utf8mb4_unicode_ci
//
DELIMITER ;
