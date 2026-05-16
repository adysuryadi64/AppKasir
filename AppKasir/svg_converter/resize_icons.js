// resize_icons.js
// Resize FamFamFam Silk 16x16 ke 20x20 untuk button

const sharp = require('sharp');
const fs    = require('fs');
const path  = require('path');

const iconDir = path.resolve(__dirname, '../Resources/Icons');
const SIZE = 20; // 20x20 px untuk button (tinggi 32-37px)

async function main() {
    const pngs = fs.readdirSync(iconDir)
        .filter(f => f.endsWith('.png') && !f.match(/_\d+\.png$/));

    console.log(`Resizing ${pngs.length} icons to ${SIZE}x${SIZE}px...`);
    let ok = 0, skip = 0;

    for (const file of pngs) {
        const base = path.basename(file, '.png');
        const src  = path.join(iconDir, file);
        const out  = path.join(iconDir, `${base}_${SIZE}.png`);

        if (fs.existsSync(out)) { skip++; continue; }

        try {
            await sharp(src)
                .resize(SIZE, SIZE, {
                    kernel: sharp.kernel.nearest,
                    fit: 'contain',
                    background: { r: 0, g: 0, b: 0, alpha: 0 }
                })
                .png()
                .toFile(out);
            ok++;
        } catch (e) {
            console.error(`GAGAL: ${file} - ${e.message}`);
        }
        process.stdout.write(`\r  ${ok + skip}/${pngs.length} - ${base}                    `);
    }

    console.log(`\n\nSelesai! ${ok} dibuat, ${skip} dilewati`);
    console.log(`Total PNG: ${fs.readdirSync(iconDir).filter(f => f.endsWith('.png')).length}`);
}

main().catch(console.error);
