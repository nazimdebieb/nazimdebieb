// Chronomètre des niveaux : donne à chaque niveau sa difficulté (facile, moyen, difficile, très difficile)
// et son temps, à partir du temps que met un robot pour le finir.
//
//   node tools/chrono.mjs                  mesure tous les niveaux et écrit levels/timing.js
//   node tools/chrono.mjs --levels 157-216 mesure seulement ces niveaux (les autres restent dans le fichier)
//   node tools/chrono.mjs --verify         fait jouer un « joueur moyen » avec les vrais temps et compte les réussites
//   node tools/chrono.mjs --retime         recalcule difficultés et temps sans rejouer (après un changement de DIST ou MARGIN)
//
// 1. Le robot (invincible, il ramasse aussi les pièces proches) joue chaque niveau sans limite de temps :
//    3 fois pour un niveau normal, 5 fois pour un boss, en accéléré mais au rythme de la boucle du jeu.
// 2. Dans chaque monde, les 11 niveaux normaux sont classés du plus court au plus long à finir pour le robot :
//    les plus courts sont faciles, les plus longs très difficiles. La répartition change de monde en monde
//    (DIST) : surtout du facile au début, de plus en plus de difficile et de très difficile ensuite.
// 3. Le temps d'un niveau = temps du robot (médiane) × marge du niveau de difficulté + quelques secondes (MARGIN).
//    Un humain doit esquiver et vise moins vite que le robot : large marge en facile, presque rien en très
//    difficile, où il faudra souvent un boost. Un boss : 3e plus long des 5 essais × 1,45 + 5 s.
// 4. --verify : un robot plus lent (il réfléchit 2,5 fois moins souvent et recule devant les bulles qui
//    tombent sur lui) rejoue avec les temps du fichier ; on attend presque toujours la réussite en facile,
//    souvent en moyen, parfois en difficile, rarement en très difficile.
import { readFileSync, writeFileSync, rmSync, existsSync } from 'fs';
import { dirname, join } from 'path';
import { fileURLToPath, pathToFileURL } from 'url';
import { createRequire } from 'module';
import { execSync } from 'child_process';

const GAME = join(dirname(fileURLToPath(import.meta.url)), '..');
const OUT = join(GAME, 'levels', 'timing.js');
const args = process.argv.slice(2);
const opt = (k, d) => { const i = args.indexOf('--' + k); return i < 0 ? d : args[i + 1] && !args[i + 1].startsWith('--') ? args[i + 1] : true; };
const SPEED = 3, JOBS = +opt('jobs', 4), RUNS = +opt('runs', 3), BOSS_RUNS = +opt('boss-runs', 5), WORLD = 12;
const VERIFY = !!opt('verify', false);

// Répartition des 11 niveaux normaux d'un monde : [faciles, moyens, difficiles, très difficiles]
const DIST = w => w === 0 ? [5, 4, 2, 0] : w === 1 ? [4, 4, 2, 1] : w < 4 ? [3, 4, 3, 1] : [2, 4, 3, 2];
// Marge par niveau de difficulté : temps = robot × a + b
const MARGIN = { 1: [2.3, 5], 2: [1.85, 4], 3: [1.5, 3], 4: [1.25, 2] };
const BOSS_MARGIN = [1.6, 12];

const range = s => s.split(',').flatMap(p => { const [a, b] = p.split('-').map(Number); return b ? Array.from({ length: b - a + 1 }, (_, i) => a + i) : [a]; });

// Robot : il vise la bulle la plus proche (du bon côté des barrières), le boss s'il n'y a plus de bulles,
// et fait un détour par une pièce tombée tout près. En --verify il est plus lent et recule devant les bulles.
const HOOK = `window.__t={go:(n,free)=>{startRun('adv',n);if(free)timeMax=timeLeft=900;},st:()=>({state,level,timeLeft,timeMax,W})};
window.__bot=()=>{const slow=window.__slow;if((window.__k=(window.__k||0)+1)%(slow?12:5))return;const p=players[0];if(!p)return;p.inv=99;
  if(slow){const d=balls.find(b=>b.vy>0&&b.y>FLOOR-150&&Math.abs(b.x-p.x)<RAD[b.s]+34);if(d){kb.l=d.x>p.x;kb.r=d.x<=p.x;return;}}
  const c=items.find(i=>i.type==='coin'&&i.y>FLOOR-20&&Math.abs(i.x-p.x)<90);
  let t=balls.filter(b=>!blocks.some(k=>k.t==='g'&&k.x<p.x!==k.x<b.x)).sort((a,c)=>Math.abs(a.x-p.x)-Math.abs(c.x-p.x))[0]||balls[0];
  if(!t&&boss)t={x:boss.x,s:4};if(c)t={x:c.x,s:0,coin:1};if(!t){kb.l=kb.r=false;return;}
  const dx=t.x-p.x;kb.l=dx<-6;kb.r=dx>6;if(!t.coin&&Math.abs(dx)<RAD[t.s]+(slow?0:4))fireQueued=true;};
`;

async function play(list, free, slow) {
  const require = createRequire(import.meta.url);
  const { chromium } = require(join(execSync('npm root -g').toString().trim(), 'playwright'));
  let html = readFileSync(join(GAME, 'index.html'), 'utf8');
  const hooks = [['const hot = window.claude && window.claude.hot;', HOOK + (slow ? 'window.__slow=1;\n' : '') + 'const hot = window.claude && window.claude.hot;'],
    ['acc += Math.min(.1, (now - last) / 1000);', `acc += Math.min(.1, (now - last) / 1000 * ${SPEED});`],
    ['  while (acc >= STEP) { update(STEP); acc -= STEP; }', '  while (acc >= STEP) { window.__bot(); update(STEP); acc -= STEP; }']];
  for (const [a, b] of hooks) { if (!html.includes(a)) throw new Error('hook point not found in index.html: ' + a); html = html.replace(a, b); }
  const test = join(GAME, '_chrono.html');
  writeFileSync(test, html);
  const browser = await chromium.launch();
  const out = {}, queue = [...list];
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
        await p.evaluate(([n, free]) => window.__t.go(n, free), [n, free]);
        for (let t = 0; t < 1400; t++) {
          await p.waitForTimeout(250);
          st = await p.evaluate(() => window.__t.st());
          if (st.state === 'clear' || st.state === 'over' || (st.state === 'play' && st.timeLeft <= 0)) break;
        }
      } catch (e) { errs.push(e.message.split('\n')[0]); }
      await ctx.close();
      const used = Math.round(st.timeMax - Math.max(0, st.timeLeft));
      (out[n] = out[n] || []).push({ ok: st.state === 'clear' && !errs.length, used, max: Math.round(st.timeMax), err: errs[0] });
      console.log(`  ${n}: ${st.state} ${used}/${Math.round(st.timeMax)} s${errs.length ? ' ' + errs[0] : ''}`);
    }
  };
  await Promise.all(Array.from({ length: JOBS }, worker));
  await browser.close();
  rmSync(test, { force: true });
  return out;
}

// Le fichier : window.POP_TIMING[n] = [temps (s, écran de 640 de large), difficulté 1-4 (0 = boss), temps du robot]
function readTiming() {
  if (!existsSync(OUT)) return {};
  const window = {};
  new Function('window', readFileSync(OUT, 'utf8'))(window);
  return window.POP_TIMING || {};
}
function writeTiming(t) {
  const ns = Object.keys(t).map(Number).sort((a, b) => a - b);
  const NAMES = ['boss', 'facile', 'moyen', 'difficile', 'très difficile'];
  const lines = ns.map(n => `  ${n}: [${t[n].join(', ')}],${' '.repeat(Math.max(1, 16 - `  ${n}: [${t[n].join(', ')}],`.length))}// ${NAMES[t[n][1]]}, robot ${t[n][2]} s`);
  writeFileSync(OUT, `// Fichier fabriqué par tools/chrono.mjs : ne pas modifier à la main (relancer l'outil).
// Pour chaque niveau : [temps en secondes sur un écran de 640 de large, difficulté (1 facile, 2 moyen,
// 3 difficile, 4 très difficile, 0 boss), temps mis par le robot]. Le jeu ajoute du temps sur les écrans larges.
window.POP_TIMING = {
${lines.join('\n')}
};
`);
}

const median = a => { const s = [...a].sort((x, y) => x - y); return s[Math.floor((s.length - 1) / 2)]; };
const p75 = a => { const s = [...a].sort((x, y) => x - y); return s[Math.min(s.length - 1, Math.ceil(s.length * .75) - 1)]; };

const timing = readTiming();

// difficulté (classement dans chaque monde) puis temps, à partir des temps du robot
function retime(robot) {
  const worlds = new Set(Object.keys(robot).map(n => Math.floor((n - 1) / WORLD)));
  const out = {};
  for (const w of worlds) {
    const ns = Array.from({ length: WORLD - 1 }, (_, i) => w * WORLD + i + 1).filter(n => robot[n] != null);
    const order = [...ns].sort((a, b) => robot[a] - robot[b] || a - b), dist = DIST(w);
    let k = 0;
    order.forEach((n, i) => {
      while (k < 3 && i >= dist.slice(0, k + 1).reduce((s, x) => s + x, 0) * ns.length / 11) k++;
      const [a, b] = MARGIN[k + 1];
      out[n] = [Math.max(25, Math.round(robot[n] * a + b)), k + 1, robot[n]];
    });
    const bn = (w + 1) * WORLD;
    if (robot[bn] != null) out[bn] = [Math.round(robot[bn] * BOSS_MARGIN[0] + BOSS_MARGIN[1]), 0, robot[bn]];
  }
  return out;
}

if (opt('retime', false)) {
  // --retime : recalcule difficultés et temps depuis les temps du robot déjà mesurés (après un changement de DIST ou MARGIN)
  writeTiming(retime(Object.fromEntries(Object.entries(timing).map(([n, v]) => [n, v[2]]))));
  console.log(`Écrit ${OUT}`);
} else if (VERIFY) {
  const list = opt('levels', null) ? range(opt('levels')) : Object.keys(timing).map(Number);
  const res = await play(list, false, true);
  const byTier = {};
  for (const n of list) {
    const tier = timing[n] ? timing[n][1] : -1;
    const r = res[n] || [];
    (byTier[tier] = byTier[tier] || []).push(...r.map(x => ({ n, ...x })));
  }
  const NAMES = { 0: 'boss', 1: 'facile', 2: 'moyen', 3: 'difficile', 4: 'très difficile', '-1': 'sans temps' };
  for (const [tier, rs] of Object.entries(byTier)) {
    const ok = rs.filter(r => r.ok).length;
    console.log(`${NAMES[tier]} : ${ok}/${rs.length} réussis (${Math.round(ok / rs.length * 100)} %), temps utilisé moyen ${Math.round(rs.reduce((s, r) => s + r.used / r.max, 0) / rs.length * 100)} %`);
    const fails = rs.filter(r => !r.ok).map(r => r.n);
    if (fails.length) console.log('   échecs : ' + fails.join(', '));
  }
} else {
  const all = opt('levels', null) ? range(opt('levels')) : null;
  if (!all) throw new Error('--levels is required, e.g. --levels 1-156');
  const list = all.flatMap(n => Array(n % WORLD ? RUNS : BOSS_RUNS).fill(n));
  console.log(`Mesure de ${all.length} niveaux (${list.length} parties)`);
  const res = await play(list, true, false);
  const robot = { ...Object.fromEntries(Object.entries(timing).map(([n, v]) => [n, v[2]])) };
  for (const n of all) {
    const ok = (res[n] || []).filter(r => r.ok).map(r => r.used);
    if (!ok.length) { console.log(`  ${n} : le robot n'a jamais fini, niveau ignoré`); continue; }
    robot[n] = n % WORLD ? median(ok) : p75(ok);
  }
  const out = retime(robot);
  writeTiming({ ...timing, ...out });
  console.log(`Écrit ${OUT}`);
}
