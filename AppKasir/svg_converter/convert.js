// convert.js - Render Fluent UI SVG ke PNG dengan warna per kategori
// Input : Resources/Icons/svg/*.svg
// Output: Resources/Icons/*.png (berbagai ukuran)

const sharp = require('sharp');
const fs    = require('fs');
const path  = require('path');

const svgDir = path.resolve(__dirname, '../Resources/Icons/svg');
const outDir = path.resolve(__dirname, '../Resources/Icons');

// Warna per icon (untuk bg terang - aplikasi tidak pakai dark mode)
const colors = {
    // Aksi positif - hijau
    'simpan':           '#1B5E20', 'simpan-semua':    '#2E7D32',
    'tambah':           '#1565C0', 'baru':            '#1565C0',
    'pilih':            '#2E7D32', 'login':           '#1565C0',
    'register':         '#6A1B9A', 'pendapatan':      '#2E7D32',
    // Aksi negatif - merah
    'hapus':            '#B71C1C', 'hapus-barang':    '#B71C1C',
    'hapus-transaksi':  '#B71C1C', 'keluar':          '#C62828',
    'batal':            '#E53935', 'tutup':           '#757575',
    'logout':           '#C62828', 'potongan':        '#C62828',
    // Navigasi/netral - biru
    'edit':             '#1565C0', 'reset':           '#E65100',
    'refresh':          '#0277BD', 'refresh-cabang':  '#0277BD',
    'cari':             '#0277BD', 'preview':         '#6A1B9A',
    'tampilkan':        '#0277BD', 'sebelumnya':      '#757575',
    'copy':             '#37474F', 'filter':          '#6A1B9A',
    // Cetak
    'cetak':            '#1A237E', 'cetak-nota':      '#1A237E',
    'setting-printer':  '#37474F', 'print-barcode':   '#1A237E',
    'export-pdf':       '#B71C1C', 'export-excel':    '#1B5E20',
    'export-manual':    '#1565C0', 'simpan-nota':     '#1B5E20',
    'cetak-bahan':      '#1A237E',
    // Keuangan
    'bayar':            '#1B5E20', 'bayar-hutang':    '#B71C1C',
    'bayar-piutang':    '#1B5E20', 'bayar-bon':       '#E65100',
    'hutang':           '#B71C1C', 'piutang':         '#1B5E20',
    'kas-penjualan':    '#1B5E20', 'master-gaji':     '#1B5E20',
    'gaji-karyawan':    '#1B5E20', 'laporan-gaji':    '#1B5E20',
    'bon-karyawan':     '#E65100', 'laporan-bon':     '#E65100',
    'laporan-kas':      '#1B5E20',
    // Transaksi
    'pembelian':        '#1565C0', 'penjualan':       '#0277BD',
    'retur-beli':       '#E65100', 'retur-jual':      '#E65100',
    'transfer-stok':    '#6A1B9A', 'transfer-barang': '#6A1B9A',
    'stok-opname':      '#00695C', 'stok-opname-lap': '#00695C',
    'surat-jalan':      '#37474F', 'kirim-cabang':    '#1565C0',
    'kirim':            '#1565C0', 'tahan':           '#E65100',
    'panggil':          '#0277BD', 'ambil-penjualan': '#0277BD',
    // Laporan
    'grafik':           '#6A1B9A', 'neraca':          '#00695C',
    'jurnal':           '#37474F', 'jurnal-umum':     '#37474F',
    'buku-besar':       '#37474F', 'buku-besar-pembantu':'#37474F',
    'mutasi-saldo':     '#0277BD', 'mutasi-barang':   '#0277BD',
    'stok-barang':      '#1565C0', 'kartu-stok':      '#1565C0',
    'stok-minimum':     '#E65100', 'stok-terlaris':   '#F57F17',
    'stok-tak-bergerak':'#757575', 'ranking':         '#F57F17',
    'omset':            '#1B5E20', 'rekap-penjualan': '#0277BD',
    'laporan-ppn':      '#37474F', 'log-audit':       '#37474F',
    // Master
    'barang':           '#5D4037', 'pelanggan':       '#1565C0',
    'supplier':         '#E65100', 'user':            '#1565C0',
    'user-status':      '#1565C0', 'hak-akses':       '#B71C1C',
    'tabel-coa':        '#37474F', 'armada':          '#37474F',
    'karyawan':         '#1565C0', 'cabang':          '#5D4037',
    'general-setting':  '#37474F', 'company':         '#5D4037',
    'toko':             '#5D4037', 'gudang':          '#5D4037',
    // Database/Utility
    'backup':           '#1565C0', 'restore':         '#0277BD',
    'perbaiki-db':      '#E65100', 'update-tabel':    '#E65100',
    'query-db':         '#37474F', 'migrasi-db':      '#6A1B9A',
    'migrasi':          '#6A1B9A', 'cek-update':      '#0277BD',
    'cek-ip':           '#0277BD', 'pilihan-masuk':   '#37474F',
    'set-default':      '#F57F17', 'buat-database':   '#1B5E20',
    'cek-database':     '#0277BD', 'jalankan-query':  '#1565C0',
    'analyze':          '#0277BD', 'checksum':        '#1B5E20',
    'check-tables':     '#0277BD', 'cleanup':         '#E65100',
    'duplikat':         '#B71C1C', 'debug':           '#B71C1C',
    // Sync
    'upload-sync':      '#1565C0', 'download-sync':   '#1B5E20',
    'sync-semua':       '#0277BD', 'lihat-log':       '#37474F',
    'cek-koneksi':      '#0277BD', 'posting':         '#6A1B9A',
    // Notifikasi
    'notifikasi':       '#E65100', 'notif-stok':      '#E65100',
    'notif-jatuh-tempo':'#B71C1C', 'lihat-daftar':    '#0277BD',
    // Auth
    'register':         '#6A1B9A',
    // Status bar
    'server':           '#37474F', 'pc':              '#37474F',
    'versi':            '#757575', 'tanggal':         '#1565C0',
    'jam':              '#37474F',
    // Master tambah
    'tambah-kategori':  '#1565C0', 'tambah-merk':     '#1565C0',
    'tambah-satuan':    '#1565C0', 'tambah-supplier': '#E65100',
    'tambah-pelanggan': '#1565C0', 'tambah-karyawan': '#1565C0',
    // Barcode
    'barcode':          '#37474F',
    // Mode toggle
    'dark-mode':        '#3949AB', 'light-mode':      '#F57F17',
    // Misc
    'help':             '#0277BD', 'cascade':         '#37474F',
    'close-all':        '#B71C1C', 'hitung':          '#0277BD',
    'proses':           '#0277BD', 'generate':        '#6A1B9A',
    'sopir':            '#37474F', 'armada-kendaraan':'#37474F',
    'mac-address':      '#37474F', 'add-item':        '#1565C0',
    'add-stok-opname':  '#00695C', 'edit-harga':      '#1565C0',
    'username':         '#1565C0', 'password':        '#37474F',
    'tampil-password':  '#757575', 'laporan-stok':    '#1565C0',
    'laporan-transfer': '#6A1B9A', 'laporan-retur':   '#E65100',
    'laporan-pembelian':'#1565C0', 'laporan-penjualan':'#0277BD',
    'laporan-omset':    '#1B5E20', 'laporan-ranking': '#F57F17',
};

function getColor(name) {
    return colors[name] || '#37474F';
}

function colorSvg(svg, color) {
    // Fluent UI SVG pakai currentColor atau fill di path
    return svg
        .replace(/currentColor/g, color)
        .replace(/fill="[^"]*"/g, `fill="${color}"`)
        .replace(/<svg([^>]*)>/, (m, attrs) => {
            if (!attrs.includes('fill=')) return `<svg${attrs} fill="${color}">`;
            return m;
        });
}

async function renderOne(svgPath, color, size, outPath) {
    const svg = fs.readFileSync(svgPath, 'utf8');
    const colored = colorSvg(svg, color);
    await sharp(Buffer.from(colored), { density: Math.round(size * 8) })
        .resize(size, size, { fit: 'contain', background: { r:0,g:0,b:0,alpha:0 } })
        .png()
        .toFile(outPath);
}

async function main() {
    const svgs = fs.readdirSync(svgDir).filter(f => f.endsWith('.svg'));
    console.log(`Rendering ${svgs.length} SVG files...`);

    // Ukuran yang dibutuhkan:
    // 16 = MenuStrip, ContextMenu, StatusBar label
    // 20 = Button normal (tinggi 28-37px)
    // 24 = Button besar (tinggi 38-50px)
    // 64 = Button extra besar (FormMasuk 133px)
    const sizes = [16, 20, 24, 64];

    let ok = 0, fail = 0;
    for (const file of svgs) {
        const base  = path.basename(file, '.svg');
        const color = getColor(base);
        const src   = path.join(svgDir, file);
        let allOk   = true;

        for (const size of sizes) {
            const suffix = size === 16 ? '' : `_${size}`;
            const out = path.join(outDir, `${base}${suffix}.png`);
            try {
                await renderOne(src, color, size, out);
            } catch(e) {
                console.error(`\nGAGAL: ${base} ${size}px - ${e.message}`);
                allOk = false;
            }
        }
        if (allOk) ok++;
        else fail++;
        process.stdout.write(`\r  ${ok+fail}/${svgs.length} - ${base}                    `);
    }

    console.log(`\n\nSelesai! ${ok} OK, ${fail} gagal`);
    console.log(`Total PNG: ${fs.readdirSync(outDir).filter(f=>f.endsWith('.png')).length}`);
    console.log(`\nUkuran:`);
    console.log(`  nama.png    = 16x16 (MenuStrip, ContextMenu)`);
    console.log(`  nama_20.png = 20x20 (Button normal)`);
    console.log(`  nama_24.png = 24x24 (Button besar)`);
    console.log(`  nama_64.png = 64x64 (Button extra besar)`);
}

main().catch(console.error);
