-- =============================================================================
-- TEST KOMPREHENSIF: sp_hlp_saldo_akun_delta
-- Mencakup semua akun dari FormJual (10 jurnal) + FormPembelian (7 jurnal)
-- Dijalankan 3 kali dengan nilai berbeda untuk memastikan konsistensi
-- =============================================================================
-- AKUN PENJUALAN:
--   J1  01.01.001 KAS DI TOKO          DEBET   — tunai masuk
--   J2  01.02.001 TRANSFER BANK        DEBET   — transfer masuk
--   J3  01.04.002 PIUTANG              DEBET   — penjualan kredit
--   J4  05.04.001 DISKON PENJUALAN     DEBET   — kontra pendapatan
--   J5  05.04.001 DISKON TOTAL         DEBET   — kontra pendapatan
--   J6  06.01.001 HPP POKOK            DEBET   — harga pokok
--   J7  05.02.001 PENJUALAN            KREDIT  — pendapatan
--   J8  01.04.001 PERSEDIAAN BARANG    KREDIT  — keluar persediaan
--   J9  03.02.001 HUTANG PAJAK         KREDIT  — pajak
--   J10 08.01.002 PENDAPATAN LAIN      KREDIT  — biaya kirim
-- AKUN PEMBELIAN (tambahan):
--   P1  01.01.001 KAS DI TOKO          KREDIT  — kas keluar
--   P2  01.02.001 TRANSFER BANK        KREDIT  — transfer keluar
--   P3  02.01.001 HUTANG BELANJA       KREDIT  — hutang timbul
--   P4  06.05.001 DISKON PEMBELIAN     KREDIT  — diskon supplier
--   P5  01.04.001 PERSEDIAAN BARANG    DEBET   — barang masuk
--   P6  01.05.001 PPN MASUKAN          DEBET   — pajak masukan
--   P7  06.02.001 BIAYA KIRIM BELI     DEBET   — angkut pembelian
-- =============================================================================

-- Simpan saldo awal SEMUA akun yang akan ditest
DROP TABLE IF EXISTS _test_saldo_awal;
CREATE TEMPORARY TABLE _test_saldo_awal AS
SELECT KODE_AKUN, AKUN_DK, TYPE_AKUN, SUB_AKUN, S_DEBET, S_KREDIT, SALDO_AKHIR
FROM tbl_datareferensi
WHERE KODE_AKUN IN (
    '01.01.001','01.02.001','01.04.001','01.04.002',
    '02.01.001','03.02.001','05.02.001','05.04.001',
    '06.01.001','06.02.001','06.05.001','08.01.002',
    '01.05.001','05.01.001'
);

SELECT '=== SALDO AWAL ===' AS info;
SELECT KODE_AKUN, AKUN_DK, S_DEBET, S_KREDIT, SALDO_AKHIR FROM _test_saldo_awal ORDER BY KODE_AKUN;

-- =============================================================================
-- PROSEDUR TEST: jalankan delta, simpan hasil, kembalikan, recalculate, bandingkan
-- Dipakai berulang untuk 3 set nilai berbeda
-- =============================================================================

DROP PROCEDURE IF EXISTS _run_test;
DELIMITER $$
CREATE PROCEDURE _run_test(
    IN label VARCHAR(50),
    -- Penjualan
    IN p_tunai DECIMAL(20,0), IN p_transfer DECIMAL(20,0), IN p_piutang DECIMAL(20,0),
    IN p_diskon_item DECIMAL(20,0), IN p_diskon_total DECIMAL(20,0),
    IN p_hpp DECIMAL(20,0), IN p_penjualan DECIMAL(20,0), IN p_persediaan_k DECIMAL(20,0),
    IN p_pajak_jual DECIMAL(20,0), IN p_biaya_kirim_jual DECIMAL(20,0),
    -- Pembelian
    IN p_kas_beli DECIMAL(20,0), IN p_tf_beli DECIMAL(20,0), IN p_hutang_beli DECIMAL(20,0),
    IN p_diskon_beli DECIMAL(20,0), IN p_persediaan_d DECIMAL(20,0),
    IN p_ppn DECIMAL(20,0), IN p_biaya_kirim_beli DECIMAL(20,0)
)
BEGIN
    DECLARE v_pass INT DEFAULT 0;
    DECLARE v_fail INT DEFAULT 0;
    DECLARE v_kode VARCHAR(20);
    DECLARE v_d_delta DECIMAL(20,0); DECLARE v_k_delta DECIMAL(20,0); DECLARE v_s_delta DECIMAL(20,0);
    DECLARE v_d_recalc DECIMAL(20,0); DECLARE v_k_recalc DECIMAL(20,0); DECLARE v_s_recalc DECIMAL(20,0);

    SELECT CONCAT('=== TEST: ', label, ' ===') AS info;

    -- ── BAGIAN A: DELTA ──────────────────────────────────────────────────────
    -- Penjualan
    CALL sp_hlp_saldo_akun_delta('01.01.001', p_tunai, 0);
    CALL sp_hlp_saldo_akun_delta('01.02.001', p_transfer, 0);
    CALL sp_hlp_saldo_akun_delta('01.04.002', p_piutang, 0);
    CALL sp_hlp_saldo_akun_delta('05.04.001', p_diskon_item + p_diskon_total, 0);
    CALL sp_hlp_saldo_akun_delta('06.01.001', p_hpp, 0);
    CALL sp_hlp_saldo_akun_delta('05.02.001', 0, p_penjualan);
    CALL sp_hlp_saldo_akun_delta('01.04.001', 0, p_persediaan_k);
    CALL sp_hlp_saldo_akun_delta('03.02.001', 0, p_pajak_jual);
    CALL sp_hlp_saldo_akun_delta('08.01.002', 0, p_biaya_kirim_jual);
    -- Pembelian
    CALL sp_hlp_saldo_akun_delta('01.01.001', 0, p_kas_beli);
    CALL sp_hlp_saldo_akun_delta('01.02.001', 0, p_tf_beli);
    CALL sp_hlp_saldo_akun_delta('02.01.001', 0, p_hutang_beli);
    CALL sp_hlp_saldo_akun_delta('06.05.001', 0, p_diskon_beli);
    CALL sp_hlp_saldo_akun_delta('01.04.001', p_persediaan_d, 0);
    CALL sp_hlp_saldo_akun_delta('01.05.001', p_ppn, 0);
    CALL sp_hlp_saldo_akun_delta('06.02.001', p_biaya_kirim_beli, 0);

    -- Simpan hasil delta
    DROP TABLE IF EXISTS _hasil_delta;
    CREATE TEMPORARY TABLE _hasil_delta AS
    SELECT KODE_AKUN, S_DEBET, S_KREDIT, SALDO_AKHIR FROM tbl_datareferensi
    WHERE KODE_AKUN IN ('01.01.001','01.02.001','01.04.001','01.04.002','02.01.001',
                        '03.02.001','05.02.001','05.04.001','06.01.001','06.02.001',
                        '06.05.001','08.01.002','01.05.001','05.01.001');

    -- Reversal delta
    CALL sp_hlp_saldo_akun_delta('01.01.001', -p_tunai, p_kas_beli);
    CALL sp_hlp_saldo_akun_delta('01.01.001', 0, 0); -- trigger laba rugi update
    CALL sp_hlp_saldo_akun_delta('01.02.001', -p_transfer, p_tf_beli);
    CALL sp_hlp_saldo_akun_delta('01.04.002', -p_piutang, 0);
    CALL sp_hlp_saldo_akun_delta('05.04.001', -(p_diskon_item + p_diskon_total), 0);
    CALL sp_hlp_saldo_akun_delta('06.01.001', -p_hpp, 0);
    CALL sp_hlp_saldo_akun_delta('05.02.001', 0, -p_penjualan);
    CALL sp_hlp_saldo_akun_delta('01.04.001', p_persediaan_k, -p_persediaan_d);
    CALL sp_hlp_saldo_akun_delta('03.02.001', 0, -p_pajak_jual);
    CALL sp_hlp_saldo_akun_delta('08.01.002', 0, -p_biaya_kirim_jual);
    CALL sp_hlp_saldo_akun_delta('02.01.001', 0, -p_hutang_beli);
    CALL sp_hlp_saldo_akun_delta('06.05.001', 0, -p_diskon_beli);
    CALL sp_hlp_saldo_akun_delta('01.05.001', -p_ppn, 0);
    CALL sp_hlp_saldo_akun_delta('06.02.001', -p_biaya_kirim_beli, 0);

    -- ── BAGIAN B: INSERT JURNAL DUMMY + RECALCULATE ───────────────────────────
    INSERT INTO JurnalUmum (NO_TRANSAKSI,TGL_TRANSAKSI,URAIAN,NAMA_AKUN_D,NOMOR_AKUN_D,NAMA_AKUN_K,NOMOR_AKUN_K,NOMINAL,JENIS_TRANSAKSI,LOKASI,ID_USER,ID_KOMPUTER) VALUES
    ('_TEST_',NOW(),'J1 KAS JUAL',   'KAS',         '01.01.001','',            '',          p_tunai,           'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'J2 TF JUAL',    'BANK',         '01.02.001','',            '',          p_transfer,        'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'J3 PIUTANG',    'PIUTANG',      '01.04.002','',            '',          p_piutang,         'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'J4 DSK ITEM',   'DISKON',       '05.04.001','',            '',          p_diskon_item,     'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'J5 DSK TOT',    'DISKON',       '05.04.001','',            '',          p_diskon_total,    'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'J6 HPP',        'HPP',          '06.01.001','',            '',          p_hpp,             'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'J7 JUAL',       '',             '',          'PENJUALAN',  '05.02.001', p_penjualan,       'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'J8 PERSED K',   '',             '',          'PERSEDIAAN', '01.04.001', p_persediaan_k,    'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'J9 PAJAK',      '',             '',          'PAJAK',      '03.02.001', p_pajak_jual,      'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'J10 KIRIM J',   '',             '',          'PEND LAIN',  '08.01.002', p_biaya_kirim_jual,'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'P1 KAS BELI',   '',             '',          'KAS',        '01.01.001', p_kas_beli,        'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'P2 TF BELI',    '',             '',          'BANK',       '01.02.001', p_tf_beli,         'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'P3 HUTANG',     '',             '',          'HUTANG',     '02.01.001', p_hutang_beli,     'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'P4 DSK BELI',   '',             '',          'DSK BELI',   '06.05.001', p_diskon_beli,     'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'P5 PERSED D',   'PERSEDIAAN',   '01.04.001','',            '',          p_persediaan_d,    'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'P6 PPN',        'PPN',          '01.05.001','',            '',          p_ppn,             'TEST','TOKO','t','t'),
    ('_TEST_',NOW(),'P7 KIRIM B',    'KIRIM BELI',   '06.02.001','',            '',          p_biaya_kirim_beli,'TEST','TOKO','t','t');

    CALL sp_hlp_saldo_akun_update('01.01.001'); CALL sp_hlp_saldo_akun_update('01.02.001');
    CALL sp_hlp_saldo_akun_update('01.04.001'); CALL sp_hlp_saldo_akun_update('01.04.002');
    CALL sp_hlp_saldo_akun_update('02.01.001'); CALL sp_hlp_saldo_akun_update('03.02.001');
    CALL sp_hlp_saldo_akun_update('05.02.001'); CALL sp_hlp_saldo_akun_update('05.04.001');
    CALL sp_hlp_saldo_akun_update('06.01.001'); CALL sp_hlp_saldo_akun_update('06.02.001');
    CALL sp_hlp_saldo_akun_update('06.05.001'); CALL sp_hlp_saldo_akun_update('08.01.002');
    CALL sp_hlp_saldo_akun_update('01.05.001');

    -- ── BAGIAN C: BANDINGKAN ─────────────────────────────────────────────────
    SELECT d.KODE_AKUN,
        IF(d.S_DEBET=r.S_DEBET,'PASS',CONCAT('FAIL d=',d.S_DEBET,' r=',r.S_DEBET)) AS S_DEBET,
        IF(d.S_KREDIT=r.S_KREDIT,'PASS',CONCAT('FAIL d=',d.S_KREDIT,' r=',r.S_KREDIT)) AS S_KREDIT,
        IF(d.SALDO_AKHIR=r.SALDO_AKHIR,'PASS',CONCAT('FAIL d=',d.SALDO_AKHIR,' r=',r.SALDO_AKHIR)) AS SALDO_AKHIR
    FROM _hasil_delta d
    JOIN tbl_datareferensi r ON d.KODE_AKUN=r.KODE_AKUN
    ORDER BY d.KODE_AKUN;

    -- ── CLEANUP ──────────────────────────────────────────────────────────────
    DELETE FROM JurnalUmum WHERE NO_TRANSAKSI='_TEST_';
    CALL sp_hlp_saldo_akun_update('01.01.001'); CALL sp_hlp_saldo_akun_update('01.02.001');
    CALL sp_hlp_saldo_akun_update('01.04.001'); CALL sp_hlp_saldo_akun_update('01.04.002');
    CALL sp_hlp_saldo_akun_update('02.01.001'); CALL sp_hlp_saldo_akun_update('03.02.001');
    CALL sp_hlp_saldo_akun_update('05.02.001'); CALL sp_hlp_saldo_akun_update('05.04.001');
    CALL sp_hlp_saldo_akun_update('06.01.001'); CALL sp_hlp_saldo_akun_update('06.02.001');
    CALL sp_hlp_saldo_akun_update('06.05.001'); CALL sp_hlp_saldo_akun_update('08.01.002');
    CALL sp_hlp_saldo_akun_update('01.05.001');

    DROP TABLE IF EXISTS _hasil_delta;
END$$
DELIMITER ;

-- =============================================================================
-- JALANKAN 3 KALI DENGAN NILAI BERBEDA
-- Format: tunai, transfer, piutang, diskon_item, diskon_total, hpp, penjualan,
--         persediaan_k, pajak_jual, biaya_kirim_jual,
--         kas_beli, tf_beli, hutang_beli, diskon_beli, persediaan_d, ppn, biaya_kirim_beli
-- Syarat seimbang penjualan: tunai+transfer+piutang+diskon_item+diskon_total+hpp = penjualan+persediaan_k+pajak_jual+biaya_kirim_jual
-- Syarat seimbang pembelian: persediaan_d+ppn+biaya_kirim_beli = kas_beli+tf_beli+hutang_beli+diskon_beli
-- =============================================================================

-- TEST 1: Transaksi kecil — penjualan tunai lunas, pembelian tunai lunas
-- Jual: D=5000000+0+0+0+0+3000000=8000000  K=7500000+3000000+300000+200000=11000000 → sesuaikan
-- Jual: tunai=5000000 hpp=3000000 penjualan=7700000 persediaan=3000000 pajak=300000 → D=8000000 K=11000000 ❌
-- Jual seimbang: tunai=5000000 hpp=3000000 → D=8000000; penjualan=4700000 persediaan=3000000 pajak=300000 → K=8000000 ✅
-- Beli: persediaan=4000000 ppn=400000 kirim=100000 → D=4500000; kas=4500000 → K=4500000 ✅
CALL _run_test('TEST1-KECIL-TUNAI-LUNAS',
    5000000,0,0,0,0,3000000,4700000,3000000,300000,0,
    4500000,0,0,0,4000000,400000,100000);

-- TEST 2: Transaksi besar — penjualan split tunai+transfer+piutang+diskon+pajak+kirim, pembelian kredit+diskon+ppn+kirim
-- Jual: tunai=10M tf=5M piutang=3M dsk_item=500K dsk_tot=500K hpp=12M → D=31M
--       jual=18M persed=12M pajak=500K kirim=500K → K=31M ✅
-- Beli: persed=20M ppn=2M kirim=500K → D=22.5M; kas=10M tf=5M hutang=7M dsk=500K → K=22.5M ✅
CALL _run_test('TEST2-BESAR-SPLIT-KREDIT',
    10000000,5000000,3000000,500000,500000,12000000,18000000,12000000,500000,500000,
    10000000,5000000,7000000,500000,20000000,2000000,500000);

-- TEST 3: Edge case — nilai nol di beberapa akun, hanya akun tertentu aktif
-- Jual: tunai=1000000 hpp=800000 → D=1800000; jual=1000000 persed=800000 → K=1800000 ✅
-- Beli: persed=500000 → D=500000; kas=500000 → K=500000 ✅
CALL _run_test('TEST3-EDGE-MINIMAL',
    1000000,0,0,0,0,800000,1000000,800000,0,0,
    500000,0,0,0,500000,0,0);

-- =============================================================================
-- VERIFIKASI FINAL: semua akun kembali ke nilai awal
-- =============================================================================
SELECT '=== VERIFIKASI FINAL: SEMUA KEMBALI KE NILAI AWAL ===' AS info;
SELECT t.KODE_AKUN,
    IF(t.S_DEBET=a.S_DEBET,'PASS',CONCAT('FAIL now=',t.S_DEBET,' awal=',a.S_DEBET)) AS S_DEBET,
    IF(t.S_KREDIT=a.S_KREDIT,'PASS',CONCAT('FAIL now=',t.S_KREDIT,' awal=',a.S_KREDIT)) AS S_KREDIT,
    IF(t.SALDO_AKHIR=a.SALDO_AKHIR,'PASS',CONCAT('FAIL now=',t.SALDO_AKHIR,' awal=',a.SALDO_AKHIR)) AS SALDO_AKHIR
FROM tbl_datareferensi t
JOIN _test_saldo_awal a ON t.KODE_AKUN=a.KODE_AKUN
ORDER BY t.KODE_AKUN;

DROP PROCEDURE IF EXISTS _run_test;
DROP TABLE IF EXISTS _test_saldo_awal;
