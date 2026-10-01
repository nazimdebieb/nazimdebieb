// Copie le jeu (../pop-archer) dans www/, le dossier que Capacitor met dans l'appli.
// Les polices Google sont remplacées par les copies locales de fonts/, pour jouer sans connexion.
import { cpSync, rmSync, mkdirSync, readFileSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const game = join(root, '..', 'pop-archer');
const www = join(root, 'www');
rmSync(www, { recursive: true, force: true });
mkdirSync(www);
cpSync(join(game, 'img'), join(www, 'img'), { recursive: true });
cpSync(join(game, 'levels'), join(www, 'levels'), { recursive: true });
cpSync(join(root, 'fonts'), join(www, 'fonts'), { recursive: true });

let html = readFileSync(join(game, 'index.html'), 'utf8');
const fontLinks = /<link rel="preconnect" href="https:\/\/fonts\.googleapis\.com">\n<link rel="preconnect" href="https:\/\/fonts\.gstatic\.com" crossorigin>\n<link rel="stylesheet" href="https:\/\/fonts\.googleapis\.com[^"]*">/;
if (!fontLinks.test(html)) throw new Error('Google Fonts links not found in pop-archer/index.html');
html = html.replace(fontLinks, '<link rel="stylesheet" href="fonts/fonts.css">');
writeFileSync(join(www, 'index.html'), html);
console.log('Game copied to www/ (fonts bundled)');
