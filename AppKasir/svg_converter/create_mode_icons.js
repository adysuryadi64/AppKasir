// create_mode_icons.js - Generate dark-mode and light-mode icons
const sharp = require('sharp');
const path = require('path');
const fs = require('fs');

const outDir = path.resolve(__dirname, '../Resources/Icons');

// SVG for dark mode (moon icon)
const darkModeSvg = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="64" height="64">
  <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" fill="#1a1a2e" stroke="#4a4a8a" stroke-width="1.5"/>
  <circle cx="18" cy="5" r="1.2" fill="#FFD700"/>
  <circle cx="20" cy="9" r="0.8" fill="#FFD700"/>
  <circle cx="16" cy="3" r="0.8" fill="#FFD700"/>
</svg>`;

// SVG for light mode (sun icon)
const lightModeSvg = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="64" height="64">
  <circle cx="12" cy="12" r="5" fill="#FFD700" stroke="#FFA500" stroke-width="1"/>
  <line x1="12" y1="2" x2="12" y2="5" stroke="#FFA500" stroke-width="2" stroke-linecap="round"/>
  <line x1="12" y1="19" x2="12" y2="22" stroke="#FFA500" stroke-width="2" stroke-linecap="round"/>
  <line x1="2" y1="12" x2="5" y2="12" stroke="#FFA500" stroke-width="2" stroke-linecap="round"/>
  <line x1="19" y1="12" x2="22" y2="12" stroke="#FFA500" stroke-width="2" stroke-linecap="round"/>
  <line x1="4.22" y1="4.22" x2="6.34" y2="6.34" stroke="#FFA500" stroke-width="2" stroke-linecap="round"/>
  <line x1="17.66" y1="17.66" x2="19.78" y2="19.78" stroke="#FFA500" stroke-width="2" stroke-linecap="round"/>
  <line x1="19.78" y1="4.22" x2="17.66" y2="6.34" stroke="#FFA500" stroke-width="2" stroke-linecap="round"/>
  <line x1="6.34" y1="17.66" x2="4.22" y2="19.78" stroke="#FFA500" stroke-width="2" stroke-linecap="round"/>
</svg>`;

async function createIcon(svgContent, baseName) {
    const svgBuf = Buffer.from(svgContent);
    const sizes = [16, 20];
    for (const size of sizes) {
        const outFile = path.join(outDir, `${baseName}_${size}.png`);
        await sharp(svgBuf)
            .resize(size, size)
            .png()
            .toFile(outFile);
        console.log(`Created: ${baseName}_${size}.png`);
    }
    // also base size 64
    const outFile64 = path.join(outDir, `${baseName}.png`);
    await sharp(svgBuf).resize(64, 64).png().toFile(outFile64);
    console.log(`Created: ${baseName}.png (64x64)`);
}

async function main() {
    await createIcon(darkModeSvg, 'dark-mode');
    await createIcon(lightModeSvg, 'light-mode');
    console.log('Done!');
}

main().catch(console.error);
