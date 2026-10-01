#!/usr/bin/env node
// L'atelier de niveaux de Pop Archer : fabrique, teste et fige les niveaux d'une région.
//
//   node tools/atelier.mjs 1              fabrique la région 1 (recette : tools/regions/region-01.mjs)
//   node tools/atelier.mjs 1 --rounds 6   nombre maximal d'essais par niveau (4 par défaut)
//   node tools/atelier.mjs 1 --jobs 8     parties jouées en même temps (6 par défaut)
//   node tools/atelier.mjs 1 --check      rejoue seulement les niveaux déjà figés
//
// 1. Chaque niveau est tiré au hasard (mais toujours pareil pour une même graine) selon la difficulté voulue :
//    en dents de scie dans chaque monde, avec deux niveaux difficiles (5e et 10e), de plus en plus dur de monde
//    en monde, et la mécanique de la région dans une bonne partie des niveaux.
// 2. Un robot invincible joue chaque niveau dans le vrai jeu (Chromium sans fenêtre, en accéléré).
// 3. Un niveau est gardé si le robot le finit en utilisant au plus 60 % du temps (75 % pour un boss) :
//    il reste de la marge pour un joueur qui doit esquiver. Sinon on en tire un autre, un peu plus facile.
// 4. Le résultat est figé dans levels/region-XX.js : un niveau publié ne change plus.
//    Ensuite, tools/chrono.mjs --levels <niveaux de la région> donne à chaque niveau sa difficulté et son temps.
//
// Il faut Playwright (npm i -g playwright) ; Chromium est celui de Playwright.
import { readFileSync, writeFileSync, rmSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath, pathToFileURL } from 'node:url';
import { execSync } from 'node:child_process';
import { createRequire } from 'node:module';

const GAME = join(dirname(fileURLToPath(import.meta.url)), '..');
const args = process.argv.slice(2);
const opt = (k, d) => { const i = args.indexOf('--' + k); return i < 0 ? d : args[i + 1] === undefined || args[i + 1].startsWith('--') ? true : args[i + 1]; };
const regionId = Number(args.find(a => /^\d+$/.test(a)) || 1);
const ROUNDS = Number(opt('rounds', 4)), JOBS = Number(opt('jobs', 6)), SPEED = Number(opt('speed', 3));
const pad = n => String(n).padStart(2, '0');
const recipe = (await import(pathToFileURL(join(GAME, 'tools', 'regions', `region-${pad(regionId)}.mjs`)))).default;
const OUT = join(GAME, 'levels', `region-${pad(regionId)}.js`);

/* ---------- Règles du jeu utiles à l'atelier (copiées de index.html) ---------- */
const WORLD = 12;
function popsOf(s, v) {
  const kids = v === 'green' ? 3 : 2, childV = v === 'steel' || v === 'bomb' || v === 'sleep' ? null : v;
  return (v === 'steel' ? 2 : 1) + (s > 0 ? kids * popsOf(s - 1, childV) : 0);
}
const clamp = (v, a, b) => v < a ? a : v > b ? b : v;
const r2 = v => Math.round(v * 100) / 100;

/* ---------- Hasard reproductible ---------- */
function rng(seed) {
  let a = seed >>> 0;
  return () => { a = (a + 0x6D2B79F5) >>> 0; let t = a; t = Math.imul(t ^ (t >>> 15), t | 1); t ^= t + Math.imul(t ^ (t >>> 7), t | 61); return ((t ^ (t >>> 14)) >>> 0) / 4294967296; };
}
const hash = (...xs) => xs.reduce((h, x) => Math.imul(h ^ (x * 2654435761 >>> 0), 16777619) >>> 0, 2166136261);

/* ---------- Dispositions d'obstacles (monde de 640 × 360, sol à 338) ---------- */
// Chaque disposition rend { k, y } : k au format compact des fichiers levels/, y = hauteur de départ imposée
// aux bulles (null : hauteur normale). ['s', x, y, w, h, ax, ay, période, phase] ; ['b', x, y, n, ax, ...] ; ['g', x].
const PI = Math.PI;
const slider = (R, y, w) => { const ax = 110 + Math.round(R() * 70), x = clamp(Math.round(170 + R() * (300 - w)), ax + 10, 630 - ax - w); return ['s', x, y, w, 14, ax, 0, r2(4 + R() * 2.5), r2(R() * PI * 2)]; };
const MOVING = {
  slider: R => ({ k: [slider(R, 165 + Math.round(R() * 55), 120 + Math.round(R() * 40))] }),
  twin: R => { const ph = r2(R() * PI); return { k: [['s', 250, 150, 130, 14, 170, 0, 5.5, ph], ['s', 250, 225, 130, 14, 170, 0, 5.5, r2(ph + PI)]] }; },
  elevators: R => { const per = r2(3.5 + R() * 1.5); return { k: [['s', 60, 195, 120, 14, 0, 45, per, 0], ['s', 460, 195, 120, 14, 0, 45, per, r2(PI)]] }; },
  gateSlider: R => ({ k: [['g', 320], ['s', 420, 185, 100, 14, 70, 0, r2(3.5 + R()), 0], ['s', 100, 205, 100, 14, 60, 0, r2(4 + R()), r2(PI)]] }),
  brickSlide: R => ({ k: [['b', 220, 165 + Math.round(R() * 35), 5, 150, 0, r2(6 + R() * 2), r2(R() * PI * 2)]] }),
  ferris: R => ({ k: [0, 1, 2].map(i => ['s', 275, 130 + i * 55, 90, 14, 180, 0, 6, r2(i * PI * 2 / 3)]) }),
  lid: R => ({ k: [['s', 190, 125, 260, 16, 150, 0, r2(6.5 + R() * 1.5), 0]] }),
  shelfSlider: R => ({ k: [['s', 0, 170, 180, 14], ['s', 460, 170, 180, 14], ['s', 265, 230, 110, 14, 150, 0, 4.5, 0]] }),
  brickLift: R => ({ k: [['b', 40, 180, 4, 0, 40, 4.5, 0], ['b', 440, 180, 4, 0, 40, 4.5, r2(PI)]] }),
};
const brickFloor = y => Array.from({ length: 16 }, (_, i) => i % 5 === 0 ? ['s', i * 40, y, 40, 16] : ['b', i * 40, y, 1]);
const STATIC = {
  none: () => ({ k: [] }),
  gate: () => ({ k: [['g', 320]] }),
  gates2: () => ({ k: [['g', 213], ['g', 427]] }),
  shelves: () => ({ k: [['s', 0, 170, 260, 14], ['s', 380, 170, 260, 14]] }),
  stairs: () => ({ k: [['s', 40, 250, 100, 14], ['s', 180, 200, 100, 14], ['s', 320, 150, 100, 14], ['s', 460, 100, 100, 14]] }),
  bricks: () => ({ k: [['b', 160, 190, 8]] }),
  brickFloor: () => ({ k: brickFloor(210), y: 60 }),
  ceiling: () => ({ k: [['s', 0, 200, 640, 14]], y: 262 }),
  pads: () => ({ k: [['g', 320], ['s', 70, 205, 120, 14], ['s', 450, 205, 120, 14]] }),
};
// Pièges du château (région 2). Une disposition peut aussi imposer ses bulles (b), ajouter des champs au niveau
// (extra : spikes, press, spawn, every, weapon) ou n'utiliser qu'une partie de la difficulté.
// Un monde peut ajouter des champs à tous ses niveaux (all : le plafond à pointes de la Salle des pointes).
const pick = (R, xs) => xs[Math.floor(R() * xs.length)];
const plain = R => pick(R, [[], [], [['s', 0, 170, 200, 14], ['s', 440, 170, 200, 14]], [['b', 160, 190, 8]], [['s', 220, 200, 200, 14]]]);
const CASTLE = {
  // plafond à pointes : les bulles qui le touchent éclatent d'un coup
  spikes: R => ({ k: plain(R), extra: { spikes: 1 } }),
  // une seule bulle géante sous les pointes : vite, la renvoyer vers le haut
  giant: (R, d) => ({ k: [], extra: { spikes: 1 }, b: [[d > .55 ? 5 : 4, pick(R, [.2, .28, .72, .8]), R() < .5 ? 1 : -1]] }),
  // des bulles endormies de chaque côté, une grosse qui bouge au milieu
  sleepers: (R, d) => {
    const n = d > .5 ? 3 : 2, s = d > .6 ? 2 : 1, b = [];
    for (let i = 0; i < n; i++) b.push([s, r2(.08 + i * .1), 1, 110 + i * 30, 'sleep'], [s, r2(.92 - i * .1), -1, 110 + i * 30, 'sleep']);
    b.push([d > .5 ? 4 : 3, .5, R() < .5 ? 1 : -1]);
    return { k: [], b, extra: R() < .5 ? { spikes: 1 } : {} };
  },
  // la presse : le plafond à pointes descend avec le chrono
  press: R => ({ k: pick(R, [[], [], [['g', 320]], [['b', 120, 230, 10]]]), extra: { press: 1 } }),
  // pluie de mini-bulles en décalé, sous la presse
  rain: (R, d) => {
    const n = clamp(Math.round(6 + d * 6), 6, 11), b = [];
    for (let i = 0; i < n; i++) b.push([d > .55 && i % 3 === 0 ? 1 : 0, r2(i < n / 2 ? .04 + .36 * (i + .5) / (n / 2) : .6 + .36 * (i - n / 2 + .5) / (n / 2)), i % 2 ? -1 : 1, 70 + ((i * 53) % 150)]);
    return { k: [], b, extra: { press: 1 } };
  },
  // couloir bas : une plateforme sur toute la largeur, les bulles roulent en dessous
  corridor: () => ({ k: [['s', 0, 200, 640, 14]], y: 262 }),
  // le mur qui avance : la première chambre rétrécit, il faut la vider vite
  slide: R => { const x = 430 + Math.round(R() * 60); return { k: [['g', x, x - 150 - Math.round(R() * 50), r2(6 + R() * 4)]] }; },
  // deux portes : la première ne laisse qu'un passage en bas, la seconde s'ouvre en grand sur une grosse bulle
  doors: (R, d) => ({ k: [['g', 213], ['g', 427, 0, 0, 1]], b: [[2, .16, 1], [2, .5, -1], [d > .55 ? 4 : 3, .8, -1]] }),
  // six chambres étroites, une petite bulle (souvent rebondissante) dans chacune
  chambers: (R, d) => ({ k: [107, 213, 320, 427, 533].map(x => ['g', x]),
    b: [0, 1, 2, 3, 4, 5].map(i => [d > .5 && i > 2 ? 2 : 1, r2((i + .5) / 6), i % 2 ? -1 : 1, null, R() < .45 ? 'bouncy' : null]) }),
  // toute une rangée de bulles identiques
  row: (R, d) => { const n = d > .5 ? 7 : 6; return { k: plain(R), b: Array.from({ length: n }, (_, i) => [d > .6 ? 2 : 1, r2(.08 + .84 * i / (n - 1)), i % 2 ? -1 : 1, 120]) }; },
  // en miroir : chaque bulle a sa jumelle de l'autre côté
  mirror: (R, d) => {
    const b = [], n = d > .5 ? 3 : 2;
    for (let i = 0; i < n; i++) { const s = clamp(3 - i, 1, 3) + (d > .65 && !i ? 1 : 0), x = r2(.1 + i * .14), v = R() < .3 ? pick(R, ['gold', 'bouncy', 'steel']) : null; b.push([s, x, 1, null, v], [s, r2(1 - x), -1, null, v]); }
    return { k: [['s', 260, 190, 120, 14]], b };
  },
  // les gargouilles : rien au départ, les bulles arrivent des murs une à une, de plus en plus vite
  gargoyles: (R, d) => {
    const n = clamp(Math.round(4 + d * 6), 4, 10), spawn = [];
    for (let i = 0; i < n; i++) spawn.push([clamp(Math.floor(i * (1.2 + d * 2.2) / n + R() * .6), 0, 3)]);
    return { k: plain(R), b: d > .5 ? [[2, .5, 1]] : [], extra: { spawn, every: r2(3.4 - d * 1.2) } };
  },
  // bulles de pierre : leurs morceaux roulent au sol ; on part avec la flèche collante
  stone: (R, d) => {
    const b = [[d > .55 ? 3 : 2, r2(.25 + R() * .1), 1, null, 'stone']];
    if (d > .4) b.push([2, .78, -1, null, R() < .5 ? 'stone' : null]);
    if (d > .65) b.push([1, .55, 1, 120]);
    return { k: plain(R), b, extra: { weapon: 'sticky' } };
  },
};
const FAMILIES = { ...MOVING, ...STATIC, ...CASTLE };
// dispositions où il faut au moins une bulle de chaque côté d'une barrière
const gatesOf = k => k.filter(o => o[0] === 'g').map(o => o[1] / 640);

/* ---------- Fabrication d'un niveau ---------- */
const POOL = ['gold', 'bouncy', 'steel', 'green', 'ghost', 'bomb', 'black', ...(recipe.pool || [])];
function difficulty(wi, i) {
  const hard = i === 4 || i === 9;
  const d = recipe.base + recipe.perWorld * wi + .22 * (i / 10) + (i === 3 || i === 7 ? -.08 : 0) + (hard ? .1 : 0);
  return { d: clamp(d, .15, .95), hard };
}
function makeLevel(wi, i, attempt, avoid = []) {
  const { d: d0, hard } = difficulty(wi, i);
  if (i === 11) return { ...recipe.bosses[wi] };
  // premier niveau de la région : on découvre la plateforme mobile, sans piège
  if (wi === 0 && i === 0 && recipe.intro) return { ...recipe.intro, _family: 'intro', _d: d0 };
  const R = rng(hash(recipe.id, wi, i, attempt));
  const d = clamp(d0 - attempt * .04, .12, .95);
  // famille d'obstacles : la mécanique de la région de plus en plus souvent, jamais deux fois de suite la même.
  // Si le monde a ses pièges (mech), on les tire le plus souvent, les premiers de la liste deux fois plus ;
  // le premier niveau d'un monde montre toujours son premier piège.
  const mech = recipe.worlds[wi].mech;
  let names;
  if (mech) names = i === 0 ? [mech[0]] : R() < .72 + .04 * wi ? [...mech, ...mech.slice(0, recipe.worlds[wi].fresh || 1)] : Object.keys(STATIC);
  else names = Object.keys(R() < .55 + .06 * wi ? MOVING : STATIC);
  let fi = Math.floor(R() * names.length);
  for (let t = 0; t < names.length && avoid.includes(names[fi]); t++) fi = (fi + 1) % names.length;
  const family = names[fi];
  const lay = FAMILIES[family](R, i === 0 ? Math.min(d, .4) : d);
  if (lay.b) {
    const lvl = { b: lay.b.map(x => { x = x.slice(); while (x.length > 3 && x[x.length - 1] == null) x.pop(); return x; }), ...(lay.k.length ? { k: lay.k } : {}), ...(lay.extra || {}), ...(recipe.worlds[wi].all || {}) };
    if (hard) lvl.hard = 1;
    return { ...lvl, _family: family, _d: r2(d) };
  }
  // bulles
  const count = clamp(Math.round(1.4 + d * 3.4 + R() * 1.4), 1, 6);
  const top = clamp(2 + Math.floor(d * 2.6 + R() * 1.3), 2, 5);
  const b = [];
  let black = false;
  for (let j = 0; j < count; j++) {
    let s = j === 0 ? top : clamp(top - 1 - Math.floor(R() * 2), 1, 4);
    let v = R() < .22 + .35 * d ? POOL[Math.floor(R() * POOL.length)] : null;
    if (v === 'black' && (black || d < .42 || s > 2)) v = 'gold';
    if (v === 'green' && s > 3) v = 'bouncy';
    if (v === 'black') black = true;
    b.push([s, 0, j % 2 ? -1 : 1, lay.y ?? null, v]);
  }
  // plafond de coups selon la difficulté : on rétrécit d'abord les petites bulles, puis on en retire
  const cap = Math.round(24 + 58 * d);
  const total = () => b.reduce((t, x) => t + popsOf(x[0], x[4]), 0);
  while (total() > cap) {
    const rest = b.slice(1).filter(x => x[0] > 1);
    if (rest.length) rest.reduce((m, x) => (x[0] > m[0] ? x : m))[0]--;
    else if (b.length > 1) b.pop();
    else if (b[0][0] > 1) b[0][0]--;
    else break;
  }
  // positions : réparties, avec un peu de jeu, jamais juste au-dessus de l'archer au départ,
  // et au moins une bulle de chaque côté de chaque barrière
  const gates = gatesOf(lay.k), start = gates.length ? gates[0] / 2 : .5;
  b.forEach((x, j) => {
    let f = (j + .5) / b.length + (R() - .5) * .06;
    if (Math.abs(f - start) < .09) f = start + (f < start ? -.1 : .1);
    x[1] = r2(clamp(f, .05, .95));
  });
  for (const g of gates) if (!b.some(x => x[1] > g) && b.length > 1) b[b.length - 1][1] = r2(Math.min(.95, g + .12));
  const lvl = { b: b.map(x => { while (x.length > 3 && x[x.length - 1] == null) x.pop(); return x; }), k: lay.k, ...(lay.extra || {}), ...(recipe.worlds[wi].all || {}) };
  if (hard) lvl.hard = 1;
  return { ...lvl, _family: family, _d: r2(d) };
}

/* ---------- Écriture du fichier de la région ---------- */
function writeRegion(levels, stats) {
  const clean = l => { const o = { ...l }; for (const k of Object.keys(o)) if (k.startsWith('_')) delete o[k]; if (o.k && !o.k.length) delete o.k; return o; };
  const lines = levels.map((l, j) => {
    const n = recipe.first + j, s = stats && stats[n];
    const note = s ? ` // ${n}${l.hard ? ' · difficile' : ''}${l.boss ? ' · boss' : ''} · ${l._family || ''}${l._d != null ? ' d=' + l._d : ''} · robot ${s.used} s / ${s.max} s` : ` // ${n}`;
    return '    ' + JSON.stringify(clean(l)) + ',' + note;
  });
  const ok = stats ? Object.values(stats).filter(s => s.ok).length : 0;
  const { worlds, art, i18n, name } = recipe;
  const txt = `// Pop Archer, région ${recipe.id} : ${name} (niveaux ${recipe.first} à ${recipe.first + levels.length - 1}).
// Fichier fabriqué par tools/atelier.mjs à partir de tools/regions/region-${pad(recipe.id)}.mjs : ne pas le modifier à la main.
// Vérifié : ${ok}/${levels.length} niveaux finis par le robot dans la marge de temps voulue.
(window.POP_REGIONS = window.POP_REGIONS || []).push({
  name: ${JSON.stringify(name)},
  art: ${JSON.stringify(art)},
  i18n: {
${Object.entries(i18n).map(([k, v]) => `    ${JSON.stringify(k)}: ${JSON.stringify(v)},`).join('\n')}
  },
  worlds: [
${worlds.map(({ mech, fresh, all, ...w }) => `    ${JSON.stringify(w)},`).join('\n')}
  ],
  levels: [
${lines.join('\n')}
  ],
});
`;
  writeFileSync(OUT, txt);
}

/* ---------- Le robot : chaque niveau joué dans le vrai jeu ---------- */
// Le robot décide tous les 5 pas de la boucle du jeu (environ 40 ms de jeu, un temps de réaction humain)
// et pas sur une minuterie : ses résultats ne dépendent pas de la charge de la machine.
const HOOK = `window.__t={adv:n=>startRun('adv',n),st:()=>({state,level,timeLeft,timeMax,balls:balls.length,boss:!!boss,won:lastOver==='clear',left:window.__left}),bot:on=>{window.__on=on;}};
window.__bot=()=>{if(!window.__on||(window.__k=(window.__k||0)+1)%5)return;const p=players[0];if(!p)return;p.inv=99;
  let t=balls.filter(b=>!blocks.some(k=>k.t==='g'&&k.x<p.x!==k.x<b.x)).sort((a,c)=>Math.abs(a.x-p.x)-Math.abs(c.x-p.x))[0]||balls[0];
  if(!t&&boss)t={x:boss.x,s:4};if(!t){kb.l=kb.r=false;return;}const dx=t.x-p.x;kb.l=dx<-6;kb.r=dx>6;if(Math.abs(dx)<RAD[t.s]+4)fireQueued=true;};
`;
async function play(levelsToCheck) {
  const require = createRequire(import.meta.url);
  const { chromium } = require(join(execSync('npm root -g').toString().trim(), 'playwright'));
  let html = readFileSync(join(GAME, 'index.html'), 'utf8');
  const a = 'const hot = window.claude && window.claude.hot;';
  const won = 'function levelClear() {';
  for (const x of [a, won, 'acc += Math.min(.1, (now - last) / 1000);', '  while (acc >= STEP) { update(STEP); acc -= STEP; }'])
    if (!html.includes(x)) throw new Error('hook point not found in index.html: ' + x);
  // le temps restant est noté au moment de la victoire : ensuite il est converti en bonus et le chrono tombe à 0,
  // et l'écran de fin peut arriver avant la lecture suivante (machine chargée)
  html = html.replace(a, HOOK + a).replace(won, won + ' window.__left = timeLeft;').replace('acc += Math.min(.1, (now - last) / 1000);', `acc += Math.min(.1, (now - last) / 1000 * ${SPEED});`)
    .replace('  while (acc >= STEP) { update(STEP); acc -= STEP; }', '  while (acc >= STEP) { window.__bot(); update(STEP); acc -= STEP; }');
  const test = join(GAME, '_atelier.html');
  writeFileSync(test, html);
  const browser = await chromium.launch();
  const res = {}, queue = [...levelsToCheck];
  const worker = async () => {
    while (queue.length) {
      const n = queue.shift();
      const ctx = await browser.newContext({ viewport: { width: 1100, height: 620 } });
      const p = await ctx.newPage();
      const errs = [];
      p.on('pageerror', e => errs.push(e.message));
      await p.addInitScript(() => { localStorage.setItem('eclate-bulles:tut', 'true'); localStorage.setItem('eclate-bulles:lang', '"en"'); localStorage.setItem('eclate-bulles:daily', JSON.stringify({ day: new Date().toDateString(), streak: 1 })); });
      let st = { state: 'error', timeMax: 0, timeLeft: 0 };
      try {
        await p.goto(pathToFileURL(test).href);
        await p.waitForTimeout(300);
        await p.evaluate(n => { window.__t.adv(n); window.__t.bot(true); }, n);
        for (let t = 0; t < 400; t++) {
          await p.waitForTimeout(250);
          st = await p.evaluate(() => window.__t.st());
          if (st.won && st.left != null) { st.state = 'clear'; st.timeLeft = st.left; }
          if (st.state === 'clear' || st.state === 'over' || (st.state === 'play' && st.timeLeft <= 0)) break;
        }
      } catch (e) { errs.push(e.message.split('\n')[0]); }
      await ctx.close();
      const used = Math.round(st.timeMax - Math.max(0, st.timeLeft)), boss = n % WORLD === 0;
      // (temps par défaut du jeu, par la formule ; tools/chrono.mjs donne ensuite à chaque niveau son vrai temps)
      const ok = st.state === 'clear' && used <= st.timeMax * (boss ? .75 : .6) && !errs.length;
      res[n] = { ok, used, max: Math.round(st.timeMax), state: st.state, errs };
      console.log(`  ${n}: ${ok ? 'ok ' : 'NON'} ${st.state} ${used}/${Math.round(st.timeMax)} s${errs.length ? ' ' + errs[0] : ''}`);
    }
  };
  await Promise.all(Array.from({ length: JOBS }, worker));
  await browser.close();
  rmSync(test, { force: true });
  return res;
}

/* ---------- Déroulé ---------- */
const count = recipe.worlds.length * WORLD;
const attempts = Array(count).fill(0);
const levels = [];
for (let j = 0; j < count; j++) levels.push(makeLevel(Math.floor(j / WORLD), j % WORLD, 0, j ? [levels[j - 1]._family] : []));
let stats = {};
if (opt('check', false)) {
  // on rejoue les niveaux tels qu'ils sont figés dans le fichier (sans les retirer au sort)
  const window = {};
  new Function('window', readFileSync(OUT, 'utf8'))(window);
  const frozen = window.POP_REGIONS[0].levels, lines = readFileSync(OUT, 'utf8').split('\n').filter(l => /^ {4}\{.*\/\/ \d+/.test(l));
  frozen.forEach((l, j) => {
    const m = / · (\w+) d=([\d.]+) /.exec(lines[j] || '');
    levels[j] = { ...l, ...(m ? { _family: m[1], _d: Number(m[2]) } : {}) };
  });
  stats = await play(Array.from({ length: count }, (_, j) => recipe.first + j));
} else {
  let todo = Array.from({ length: count }, (_, j) => recipe.first + j);
  for (let round = 1; round <= ROUNDS && todo.length; round++) {
    console.log(`Essai ${round} : ${todo.length} niveau(x) à jouer`);
    writeRegion(levels, null);
    Object.assign(stats, await play(todo));
    todo = todo.filter(n => !stats[n].ok);
    if (!todo.length || round === ROUNDS) break;
    // on retire au sort les niveaux ratés (un peu plus faciles) ; les bosses ne changent pas
    // seuls les niveaux ratés changent : les autres restent tels que le robot les a validés
    for (const n of todo) {
      const j = n - recipe.first;
      if (!((j + 1) % WORLD)) continue;
      attempts[j]++;
      levels[j] = makeLevel(Math.floor(j / WORLD), j % WORLD, attempts[j], [levels[j - 1]?._family, levels[j + 1]?._family]);
    }
    todo = todo.filter(n => (n - recipe.first + 1) % WORLD);
  }
}
writeRegion(levels, stats);
const bad = Object.entries(stats).filter(([, s]) => !s.ok).map(([n]) => n);
console.log(`\n${OUT} : ${count - bad.length}/${count} niveaux validés${bad.length ? ' ; à revoir : ' + bad.join(', ') : ''}`);
