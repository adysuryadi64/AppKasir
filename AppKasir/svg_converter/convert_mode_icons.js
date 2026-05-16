// convert_mode_icons.js - Render hanya dark-mode dan light-mode
const sharp = require('sharp');
const fs    = require('fs');
const path  = require('path');

const svgDir = path.resolve(__dirname, '../Resources/Icons/svg');
const outDir = path.resolve(__dirname, '../Resources/Icons');

const colors = {
    'dark-mode':  '#3949AB',   // indigo - mewakili malam/gelap
    'light-mode': '#F57F17',   // amber  - mewakili siang/terang
};

function colorSvg(svg, color) {
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
    const targets = ['dark-mode', 'light-mode'];
    const sizes   = [16, 20, 24, 64];

    for (const name of targets) {
        const color   = colors[name];
        const svgPath = path.join(svgDir, `${name}.svg`);

        for (const size of sizes) {
            // Konsisten dengan convert.js: ukuran 16 = nama.png (tanpa suffix)
            const suffix = size === 16 ? '' : `_${size}`;
            const out    = path.join(outDir, `${name}${suffix}.png`);
            await renderOne(svgPath, color, size, out);
            console.log(`  OK: ${name}${suffix}.png (${size}x${size})`);
        }
    }
    console.log('\nSelesai!');
}

main().catch(console.error);
