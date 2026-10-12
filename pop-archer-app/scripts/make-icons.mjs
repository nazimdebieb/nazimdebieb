// Génère les icônes Android, l'écran de démarrage et les visuels de la fiche Play Store
// à partir de art/icon-1024.png (Canva) et des images du jeu. Utilise Playwright (Chromium).
// Lancer : node scripts/make-icons.mjs
import { readFileSync, writeFileSync, mkdirSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { execSync } from 'node:child_process';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const { chromium } = await import(join(execSync('npm root -g').toString().trim(), 'playwright', 'index.mjs'));
const res = join(root, 'android/app/src/main/res');
const img = p => 'data:image/' + (p.endsWith('.png') ? 'png' : 'webp') + ';base64,' + readFileSync(p).toString('base64');
const ICON = img(join(root, 'art/icon-1024.png'));
const GAME = join(root, '..', 'pop-archer', 'img');
const BG = '#211130';

const browser = await chromium.launch();
const page = await browser.newPage();
async function render(w, h, draw, args, out) {
  const data = await page.evaluate(async ({ w, h, draw, args }) => {
    const load = src => new Promise(r => { const i = new Image(); i.onload = () => r(i); i.src = src; });
    const c = document.createElement('canvas'); c.width = w; c.height = h;
    const x = c.getContext('2d'); x.imageSmoothingQuality = 'high';
    await new Function('x', 'w', 'h', 'load', 'a', `return (async () => { ${draw} })()`)(x, w, h, load, args);
    return c.toDataURL('image/png');
  }, { w, h, draw, args });
  mkdirSync(dirname(out), { recursive: true });
  writeFileSync(out, Buffer.from(data.split(',')[1], 'base64'));
}

// Icônes classiques (carré arrondi et rond), icône adaptative (avant-plan plein cadre sur fond violet).
const dens = { mdpi: 1, hdpi: 1.5, xhdpi: 2, xxhdpi: 3, xxxhdpi: 4 };
for (const [d, k] of Object.entries(dens)) {
  const s = Math.round(48 * k), f = Math.round(108 * k);
  await render(s, s, `const i = await load(a.icon); x.beginPath(); x.roundRect(0, 0, w, h, w * .18); x.clip(); x.drawImage(i, 0, 0, w, h);`, { icon: ICON }, join(res, `mipmap-${d}/ic_launcher.png`));
  await render(s, s, `const i = await load(a.icon); x.beginPath(); x.arc(w / 2, h / 2, w / 2, 0, Math.PI * 2); x.clip(); x.drawImage(i, 0, 0, w, h);`, { icon: ICON }, join(res, `mipmap-${d}/ic_launcher_round.png`));
  await render(f, f, `const i = await load(a.icon); x.drawImage(i, 0, 0, w, h);`, { icon: ICON }, join(res, `mipmap-${d}/ic_launcher_foreground.png`));
}
// Écran de démarrage : le logo sur le fond du jeu.
const LOGO = img(join(GAME, 'logo.webp'));
const splash = `x.fillStyle = a.bg; x.fillRect(0, 0, w, h); const l = await load(a.logo);
  const s = Math.min(w * .55 / l.width, h * .45 / l.height); x.drawImage(l, (w - l.width * s) / 2, (h - l.height * s) / 2, l.width * s, l.height * s);`;
const sizes = { mdpi: [480, 320], hdpi: [800, 480], xhdpi: [1280, 720], xxhdpi: [1600, 960], xxxhdpi: [1920, 1280] };
for (const [d, [w, h]] of Object.entries(sizes)) {
  await render(w, h, splash, { bg: BG, logo: LOGO }, join(res, `drawable-land-${d}/splash.png`));
  await render(h, w, splash, { bg: BG, logo: LOGO }, join(res, `drawable-port-${d}/splash.png`));
}
await render(480, 320, splash, { bg: BG, logo: LOGO }, join(res, 'drawable/splash.png'));

// Fiche Play Store : icône 512 px et bannière 1024×500.
await render(512, 512, `const i = await load(a.icon); x.drawImage(i, 0, 0, w, h);`, { icon: ICON }, join(root, 'store/icon-512.png'));
await render(1024, 500, `
  const bg = await load(a.bg), logo = await load(a.logo), hero = await load(a.hero);
  const s = Math.max(w / bg.width, h / bg.height); x.drawImage(bg, (w - bg.width * s) / 2, h - bg.height * s, bg.width * s, bg.height * s);
  const g = x.createLinearGradient(0, 0, w, 0); g.addColorStop(0, 'rgba(33,17,48,.55)'); g.addColorStop(.6, 'rgba(33,17,48,0)'); x.fillStyle = g; x.fillRect(0, 0, w, h);
  const ball = (cx, cy, r, c) => { const gr = x.createRadialGradient(cx - r * .35, cy - r * .4, r * .1, cx, cy, r); gr.addColorStop(0, '#ffffff'); gr.addColorStop(.3, c); gr.addColorStop(1, '#40102a');
    x.fillStyle = gr; x.beginPath(); x.arc(cx, cy, r, 0, Math.PI * 2); x.fill(); x.lineWidth = 4; x.strokeStyle = 'rgba(20,10,30,.5)'; x.stroke(); };
  ball(820, 120, 70, '#ff3b55'); ball(930, 250, 42, '#3fbf6f'); ball(700, 70, 30, '#ffc83d'); ball(960, 90, 26, '#b45cff');
  const ls = 470 / logo.width; x.drawImage(logo, 40, 60, logo.width * ls, logo.height * ls);
  const hs = 300 / hero.height; x.drawImage(hero, 640, 470 - hero.height * hs, hero.width * hs, hero.height * hs);`,
  { bg: img(join(GAME, 'forest.webp')), logo: LOGO, hero: img(join(GAME, 'archer-shoot.webp')) }, join(root, 'store/feature-1024x500.png'));
await browser.close();
console.log('Icons, splash screens and store images written.');
