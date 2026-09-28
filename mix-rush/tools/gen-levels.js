#!/usr/bin/env node
// Builds mix-rush/levels.js from the level specs below.
// For each level it builds a pile from a known solution (so every level is
// winnable without boosters), then plays thousands of simulated games to
// pick the seed whose difficulty is closest to the target.
//   node mix-rush/tools/gen-levels.js
'use strict';
const fs = require('fs');
const path = require('path');
const E = require('../engine.js');

const SPECS = [
  {
    name: 'Le ballon', bowls: 3, cols: 2, cap: 12, prim: [4, 6], sec: [6, 10], surplus: 0, target: 1.0,
    hint: 'Touche un pot, puis un bol. Le bol peint tout seul les pixels de sa couleur.',
    picture: [
      '..RRR..',
      '.RRRRR.',
      '.RRRRR.',
      '.RRRRR.',
      '..RRR..',
      '...R...',
      '...B...',
      '..B....',
      '...B...',
    ],
  },
  {
    name: 'La feuille', bowls: 3, cols: 2, cap: 12, prim: [4, 6], sec: [6, 10], surplus: 0, target: 1.0,
    hint: 'Jaune + Bleu = Vert ! Mets les deux pots dans le même bol.',
    picture: [
      '......GG',
      '....GGGG',
      '..GGGGGG',
      '.GGGGGG.',
      'GGGGGG..',
      'GGGGG...',
      '.GGG....',
      'G.......',
    ],
  },
  {
    name: "L'orange", bowls: 4, cols: 3, cap: 12, prim: [4, 6], sec: [6, 10], surplus: 1, target: 0.9,
    hint: 'Rouge + Jaune = Orange. Attention : les trois couleurs ensemble font de la boue.',
    picture: [
      '....GG..',
      '...GG...',
      '..OOOO..',
      '.OOOOOO.',
      'OOOOOOOO',
      'OOOOOOOO',
      '.OOOOOO.',
      '..OOOO..',
    ],
  },
  {
    name: 'Le raisin', bowls: 4, cols: 3, cap: 12, prim: [4, 6], sec: [6, 10], surplus: 1, target: 0.8,
    hint: 'Rouge + Bleu = Violet. Un bol rempli d’une couleur inutile reste bloqué.',
    picture: [
      '....GG..',
      '...GG...',
      '..VV.VV.',
      '.VVVVVVV',
      '..VVVVV.',
      '.VVV.VV.',
      '..VVVV..',
      '...VV...',
      '....V...',
    ],
  },
  {
    name: 'La tulipe', bowls: 4, cols: 3, cap: 12, prim: [3, 6], sec: [4, 8], surplus: 1, target: 0.6,
    hint: 'L’ordre compte ! Un bol qui peut peindre peint tout de suite. Pour l’orange, pose le jaune en premier.',
    picture: [
      '.R.R.R.',
      '.RRORR.',
      '.RROOR.',
      '.RRORR.',
      '..RRR..',
      '...G...',
      '.G.G.G.',
      '..GGG..',
      '...G...',
    ],
  },
  {
    name: 'Coucher de soleil', bowls: 4, cols: 3, cap: 12, prim: [3, 6], sec: [4, 9], surplus: 2, target: 0.5,
    hint: 'Ici le jaune sert seul : pour l’orange, commence par le rouge.',
    picture: [
      '...YYYY...',
      '..YYYYYY..',
      '.OYYYYYYO.',
      'OOOOOOOOOO',
      'BBBBBBBBBB',
      '.BBBBBBBB.',
      'BBBBBBBBBB',
    ],
  },
  {
    name: 'Le papillon', bowls: 4, cols: 4, cap: 12, prim: [3, 6], sec: [4, 9], surplus: 2, target: 0.45,
    hint: 'Des pots en trop peuvent bloquer ton chemin. Garde un bol « poubelle » si besoin.',
    picture: [
      'VV.....VV',
      'VVV...VVV',
      'VOVV.VVOV',
      'VVVVBVVVV',
      '.VVVBVVV.',
      '..VVBVV..',
      '.VOVBVOV.',
      'VVVV.VVVV',
      '.VV...VV.',
    ],
  },
  {
    name: 'Le perroquet', bowls: 4, cols: 4, cap: 12, prim: [3, 6], sec: [4, 9], surplus: 2, hidden: 0.3, target: 0.35,
    hint: 'Nouveau : les pots mystère « ? » révèlent leur couleur quand ils arrivent devant.',
    picture: [
      '...RRR...',
      '..RRRRR..',
      '..RYRRRR.',
      '..RRRRYY.',
      '..RRRR.Y.',
      '...GGG...',
      '..GGGGG..',
      '..BGGGB..',
      '..BBGBB..',
      '...B.B...',
    ],
  },
  {
    name: "L'arc-en-ciel", bowls: 5, cols: 4, cap: 12, prim: [3, 6], sec: [4, 9], surplus: 3, ice: 3, target: 0.3,
    hint: 'Nouveau : les pots gelés dégèlent après quelques coups. Six couleurs à faire !',
    picture: [
      '..RRRRRR..',
      '.ROOOOOOR.',
      'ROYYYYYYOR',
      'OYGGGGGGYO',
      'YGBBBBBBGY',
      'GBVV..VVBG',
      'BV......VB',
      'V........V',
    ],
  },
  {
    name: 'La nuit étoilée', bowls: 5, cols: 5, cap: 12, prim: [3, 6], sec: [4, 9], surplus: 3, hidden: 0.25, ice: 3, target: 0.25,
    hint: 'Chef-d’œuvre final, d’après Van Gogh. Prends ton temps.',
    picture: [
      'BBBBBBBYYB',
      'BYBBVBBYOY',
      'BBBVVVBBOB',
      'BBVBBBVBBB',
      'YBBBBBBBVB',
      'BBVVBBBVBB',
      'GGBBBYBBBB',
      'GGGBBBBBBV',
      'GGGVVVVVVV',
      'GGGGVVVVVV',
    ],
  },
];

// Small seeded PRNG so levels are reproducible.
function rng(seed) {
  let a = seed >>> 0;
  return () => {
    a |= 0; a = (a + 0x6d2b79f5) | 0;
    let t = Math.imul(a ^ (a >>> 15), 1 | a);
    t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}
const randInt = (r, lo, hi) => lo + Math.floor(r() * (hi - lo + 1));
const pick = (r, arr) => arr[Math.floor(r() * arr.length)];

function split(r, n, lo, hi) {
  const parts = [];
  while (n > 0) {
    let s;
    if (n <= hi) s = n;
    else s = randInt(r, lo, Math.min(hi, n - lo));
    parts.push(s);
    n -= s;
  }
  return parts;
}

const PRIMS_OF = { O: ['R', 'Y'], G: ['Y', 'B'], V: ['R', 'B'] };

// Build one valid sequence of pots by simulating an ideal player.
function buildPlan(spec, L, r) {
  const rem = { ...L.totals };
  const tasks = [];
  for (const c of E.PRIMARIES) for (const n of split(r, rem[c], spec.prim[0], spec.prim[1])) tasks.push({ t: 'P', c, n });
  for (const s of E.SECONDARIES) {
    for (const n of split(r, rem[s], spec.sec[0], spec.sec[1])) {
      if (n < 2) throw new Error('secondary colour ' + s + ' needs at least 2 pixels');
      const a = Math.max(1, Math.min(n - 1, Math.round(n / 2) + randInt(r, -1, 1)));
      tasks.push({ t: 'S', s, a, b: n - a });
    }
  }
  const bowls = Array.from({ length: spec.bowls }, () => ({ mask: 0, amt: 0, task: null, waste: false }));
  const plan = [];
  let surplus = spec.surplus || 0;
  const pending = tasks.slice();
  const colorOf = (m) => E.BY_MASK[m];
  for (let guard = 0; guard < 500; guard++) {
    if (!pending.length && !bowls.some((b) => b.task) && surplus === 0) return plan;
    const empties = bowls.filter((b) => b.mask === 0 && !b.waste);
    const cands = [];
    for (const b of bowls) if (b.task) cands.push({ w: 3, go: () => finish(b) });
    for (const task of pending) {
      if (task.t === 'P' && empties.length && rem[task.c] >= task.n) cands.push({ w: 2, go: () => doPrim(task) });
      if (task.t === 'S' && empties.length >= 1) {
        for (const base of PRIMS_OF[task.s]) {
          if (rem[base] === 0 && rem[task.s] >= task.a + task.b) cands.push({ w: 2, go: () => start(task, base) });
        }
      }
    }
    if (surplus > 0) {
      for (const x of E.PRIMARIES) {
        for (const b of bowls) {
          if (b.task) continue;
          const nm = b.mask | E.BIT[x];
          const col = colorOf(nm);
          const n = randInt(r, spec.prim[0], spec.prim[1]);
          if (b.amt + n > spec.cap) continue;
          if (col !== 'M' && rem[col] > 0) continue;
          if (b.mask === 0 && !b.waste) {
            const usable = bowls.filter((o) => !o.waste && o !== b).length;
            if (usable < 2) continue;
          } else if (!b.waste) continue;
          cands.push({ w: 1.2, go: () => waste(b, x, n) });
        }
      }
    }
    if (!cands.length) return null;
    const total = cands.reduce((s, c) => s + c.w, 0);
    let roll = r() * total;
    for (const c of cands) { roll -= c.w; if (roll <= 0) { c.go(); break; } }
  }
  return null;

  function doPrim(task) {
    pending.splice(pending.indexOf(task), 1);
    rem[task.c] -= task.n;
    plan.push({ c: task.c, n: task.n, bi: bowls.findIndex((o) => o.mask === 0 && !o.waste) });
  }
  function start(task, base) {
    pending.splice(pending.indexOf(task), 1);
    const b = bowls.find((o) => o.mask === 0 && !o.waste);
    const other = PRIMS_OF[task.s].find((p) => p !== base);
    const baseN = base === PRIMS_OF[task.s][0] ? task.a : task.b;
    b.mask = E.BIT[base]; b.amt = baseN; b.task = { task, other, otherN: task.a + task.b - baseN };
    plan.push({ c: base, n: baseN, bi: bowls.indexOf(b) });
  }
  function finish(b) {
    const { task, other, otherN } = b.task;
    rem[task.s] -= task.a + task.b;
    plan.push({ c: other, n: otherN, bi: bowls.indexOf(b) });
    b.mask = 0; b.amt = 0; b.task = null;
  }
  function waste(b, x, n) {
    surplus--;
    b.mask |= E.BIT[x]; b.amt += n; b.waste = true;
    plan.push({ c: x, n, bi: bowls.indexOf(b), surplus: true });
  }
}

function layout(spec, plan, r) {
  const cols = Array.from({ length: spec.cols }, () => []);
  const maxLen = Math.ceil(plan.length / spec.cols) + 1;
  const moves = [];
  plan.forEach((pot, i) => {
    const open = cols.map((c, k) => k).filter((k) => cols[k].length < maxLen);
    const k = pick(r, open);
    const p = { c: pot.c, n: pot.n };
    if (spec.hidden && cols[k].length > 0 && r() < spec.hidden) p.hidden = true;
    cols[k].push(p);
    moves.push({ ci: k, i });
  });
  if (spec.ice) {
    // Freeze a few pots; the ice never outlasts the picks the known solution makes first.
    let placed = 0;
    for (let tries = 0; tries < 200 && placed < spec.ice; tries++) {
      const m = pick(r, moves);
      if (m.i < 4) continue;
      const pot = cols[m.ci][positionIn(cols, moves, m)];
      if (!pot || pot.ice) continue;
      pot.ice = Math.max(3, m.i - randInt(r, 0, 2));
      placed++;
    }
  }
  return { cols, moves };
}

function positionIn(cols, moves, m) {
  // Index of plan move m inside its column.
  return moves.filter((o) => o.ci === m.ci && o.i < m.i).length;
}

function playOut(L, r, smart) {
  let st = E.newState(L);
  for (let step = 0; step < 400; step++) {
    const moves = E.legalMoves(L, st);
    if (E.remaining(st) === 0) return true;
    if (!moves.length) return false;
    let m;
    if (!smart) m = pick(r, moves);
    else {
      let best = -Infinity;
      for (const mv of moves) {
        const pv = mv.pv;
        let s = { paint: 100 + pv.paint * 2 - (pv.amt - pv.paint) * 3, base: 20, useless: -50, mud: -100 }[pv.kind];
        if (pv.kind === 'base' && st.bowls[mv.bi].mask !== 0) s += 5;
        s += r() * 40;
        if (s > best) { best = s; m = mv; }
      }
    }
    st = E.apply(L, st, m.ci, m.bi).st;
  }
  return false;
}

function rates(L, seed, runs) {
  const r = rng(seed * 7919 + 13);
  let smart = 0, rand = 0;
  for (let i = 0; i < runs; i++) {
    if (playOut(L, r, true)) smart++;
    if (playOut(L, r, false)) rand++;
  }
  return { smart: smart / runs, random: rand / runs };
}

const out = [];
SPECS.forEach((spec, idx) => {
  let best = null;
  for (let seed = 1; seed <= 120; seed++) {
    const r = rng(seed * 1000 + idx);
    const L0 = E.prepare({ picture: spec.picture, bowls: spec.bowls, cap: spec.cap, cols: [] });
    const plan = buildPlan(spec, L0, r);
    if (!plan) continue;
    const { cols, moves } = layout(spec, plan, r);
    const def = { name: spec.name, hint: spec.hint, picture: spec.picture, bowls: spec.bowls, cap: spec.cap, cols };
    const L = E.prepare(def);
    // Replay the known solution through the real rules: proves the level is winnable.
    let st = E.newState(L);
    let ok = true;
    for (const m of moves) {
      try { st = E.apply(L, st, m.ci, plan[m.i].bi).st; } catch (e) { ok = false; break; }
    }
    let solvable = ok && E.remaining(st) === 0;
    if (!solvable) {
      const s = E.solve(L, E.newState(L), 200000);
      solvable = !!s.path;
    }
    if (!solvable) continue;
    const rt = rates(L, seed, 300);
    const score = Math.abs(rt.smart - spec.target);
    if (!best || score < best.score) best = { score, def, rt, seed, pots: plan.length };
    if (score < 0.03) break;
  }
  if (!best) throw new Error('no valid layout for level ' + (idx + 1));
  console.log(`Niveau ${idx + 1} ${best.def.name.padEnd(20)} pots=${String(best.pots).padStart(2)} ` +
    `joueur-glouton=${(best.rt.smart * 100).toFixed(0)}% hasard=${(best.rt.random * 100).toFixed(0)}% (cible ${spec.target * 100}%)`);
  out.push(best.def);
});

const file = path.join(__dirname, '..', 'levels.js');
fs.writeFileSync(file,
  '// Generated by tools/gen-levels.js — do not edit by hand.\n' +
  'window.MIX_LEVELS = ' + JSON.stringify(out, null, 1) + ';\n');
console.log('Écrit : ' + path.relative(process.cwd(), file));
