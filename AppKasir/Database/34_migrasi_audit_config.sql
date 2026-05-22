-- ============================================================
-- Migrasi 34 — Tabel tbl_audit_config
--
-- Masalah: AuditRetensi dan AuditArsipTerakhir selama ini
-- disimpan di hakaksesuser (tabel hak akses) karena tidak ada
-- tabel config yang tepat. Ini menyebabkan:
--   1. Data tidak relevan bercampur dengan hak akses user
--   2. INSERT ON DUPLICATE KEY UPDATE tidak berfungsi
--      (tidak ada UNIQUE KEY pada UserName+Role) → duplikat tiap hari
--
-- Solusi: buat tabel tbl_audit_config khusus, migrasi data lama,
-- hapus baris lama dari hakaksesuser.
--
-- Aman dijalankan berulang kali (idempoten).
-- ============================================================

-- ── 1. Buat tabel tbl_audit_config ───────────────────────────
CREATE TABLE IF NOT EXISTS `tbl_audit_config` (
    `config_key`   VARCHAR(50)  NOT NULL,
    `config_value` VARCHAR(100) NOT NULL DEFAULT '',
    `updated_at`   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
                                ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`config_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
  COMMENT='Konfigurasi modul audit trail (retensi, tanggal arsip terakhir)';

-- ── 2. Seed nilai default ─────────────────────────────────────
-- INSERT IGNORE: tidak overwrite jika sudah ada
INSERT IGNORE INTO `tbl_audit_config` (`config_key`, `config_value`)
VALUES
    ('AuditRetensi',       '3'),
    ('AuditArsipTerakhir', '');

-- ── 3. Migrasi nilai dari hakaksesuser (jika ada) ─────────────
-- Ambil nilai AuditRetensi terbaru dari hakaksesuser → update tbl_audit_config
SET @retensi = (
    SELECT ModuleName FROM hakaksesuser
    WHERE UserName = 'SYSTEM' AND Role = 'AuditRetensi'
    ORDER BY NO DESC LIMIT 1
);
UPDATE `tbl_audit_config`
SET `config_value` = IFNULL(@retensi, '3')
WHERE `config_key` = 'AuditRetensi'
  AND @retensi IS NOT NULL
  AND @retensi <> '';

-- Ambil nilai AuditArsipTerakhir terbaru (tanggal terbesar)
SET @arsip = (
    SELECT ModuleName FROM hakaksesuser
    WHERE UserName = 'SYSTEM' AND Role = 'AuditArsipTerakhir'
      AND ModuleName REGEXP '^[0-9]{4}-[0-9]{2}-[0-9]{2}$'
    ORDER BY ModuleName DESC LIMIT 1
);
UPDATE `tbl_audit_config`
SET `config_value` = IFNULL(@arsip, '')
WHERE `config_key` = 'AuditArsipTerakhir'
  AND @arsip IS NOT NULL;

-- ── 4. Hapus baris lama dari hakaksesuser ─────────────────────
DELETE FROM `hakaksesuser`
WHERE `UserName` = 'SYSTEM'
  AND `Role` IN ('AuditRetensi', 'AuditArsipTerakhir');

-- ── 5. Verifikasi ─────────────────────────────────────────────
SELECT config_key, config_value, updated_at
FROM tbl_audit_config;

SELECT 'Migrasi 34 audit_config selesai.' AS info;
