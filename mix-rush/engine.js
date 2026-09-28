// Mix Rush — pure game rules, shared by the browser game and the Node tools.
// A level has a pixel-art picture, a pile of paint pots in columns (only the
// front pot of a column can be taken), and a few mixing bowls that act as the
// buffer. Pots only come in primaries; bowls mix them (set-based: the colour
// of a bowl is the set of primaries it contains, all three = mud).
(function (root, factory) {
  const api = factory();
  if (typeof module === 'object' && module.exports) module.exports = api;
  else root.MixEngine = api;
})(typeof self !== 'undefined' ? self : this, function () {
  'use strict';

  const BIT = { R: 1, Y: 2, B: 4 };
  const MASK = { R: 1, Y: 2, B: 4, O: 3, G: 6, V: 5, M: 7 };
  const BY_MASK = [null, 'R', 'Y', 'O', 'B', 'V', 'G', 'M'];
  const PRIMARIES = ['R', 'Y', 'B'];
  const SECONDARIES = ['O', 'G', 'V'];
  const PAINTABLE = ['R', 'Y', 'B', 'O', 'G', 'V'];

  // Parse the picture once and index its pixels per colour, in reading order.
  function prepare(def) {
    const h = def.picture.length;
    const w = Math.max(...def.picture.map((r) => r.length));
    const pixels = [];
    const byColor = { R: [], Y: [], B: [], O: [], G: [], V: [] };
    def.picture.forEach((row, y) => {
      for (let x = 0; x < row.length; x++) {
        const c = row[x];
        if (PAINTABLE.includes(c)) {
          byColor[c].push(pixels.length);
          pixels.push({ x, y, c });
        }
      }
    });
    const totals = {};
    PAINTABLE.forEach((c) => (totals[c] = byColor[c].length));
    return { def, w, h, pixels, byColor, totals };
  }

  function newState(L) {
    return {
      pos: L.def.cols.map(() => 0),
      bowls: Array.from({ length: L.def.bowls }, () => ({ mask: 0, amt: 0 })),
      rem: { ...L.totals },
      picks: 0,
    };
  }

  function clone(st) {
    return {
      pos: st.pos.slice(),
      bowls: st.bowls.map((b) => ({ mask: b.mask, amt: b.amt })),
      rem: { ...st.rem },
      picks: st.picks,
    };
  }

  function front(L, st, ci) {
    const col = L.def.cols[ci];
    return st.pos[ci] < col.length ? col[st.pos[ci]] : null;
  }

  function iceLeft(st, pot) {
    return Math.max(0, (pot.ice || 0) - st.picks);
  }

  function canPick(L, st, ci) {
    const pot = front(L, st, ci);
    return !!pot && iceLeft(st, pot) === 0;
  }

  // A primary is worth keeping when a secondary that contains it still needs paint.
  function isBase(st, color) {
    if (!BIT[color]) return false;
    return SECONDARIES.some((s) => (MASK[s] & BIT[color]) && st.rem[s] > 0);
  }

  function classify(st, color, paint) {
    if (color === 'M') return 'mud';
    if (paint > 0) return 'paint';
    return isBase(st, color) ? 'base' : 'useless';
  }

  // What would happen if `pot` went into bowl `bi`.
  function preview(L, st, pot, bi) {
    const b = st.bowls[bi];
    if (b.mask === MASK.M) return { ok: false, why: 'clogged' };
    if (b.amt + pot.n > L.def.cap) return { ok: false, why: 'full' };
    const mask = b.mask | BIT[pot.c];
    const color = BY_MASK[mask];
    const amt = b.amt + pot.n;
    const paint = color === 'M' ? 0 : Math.min(amt, st.rem[color]);
    return { ok: true, mask, color, amt, paint, kind: classify(st, color, paint) };
  }

  function apply(L, st, ci, bi) {
    const pot = front(L, st, ci);
    if (!pot || !canPick(L, st, ci)) throw new Error('pot not available');
    const pv = preview(L, st, pot, bi);
    if (!pv.ok) throw new Error('bowl refuses: ' + pv.why);
    const ns = clone(st);
    ns.pos[ci]++;
    ns.picks++;
    const b = ns.bowls[bi];
    b.mask = pv.mask;
    b.amt = pv.amt;
    let startIdx = 0;
    if (pv.paint > 0) {
      startIdx = L.totals[pv.color] - ns.rem[pv.color];
      ns.rem[pv.color] -= pv.paint;
      b.amt -= pv.paint;
      if (b.amt === 0) b.mask = 0;
    }
    return { st: ns, pot, color: pv.color, paint: pv.paint, startIdx, kind: pv.kind, amtBefore: pv.amt };
  }

  function rinse(st, bi) {
    const ns = clone(st);
    ns.bowls[bi] = { mask: 0, amt: 0 };
    return ns;
  }

  function legalMoves(L, st) {
    const moves = [];
    for (let ci = 0; ci < L.def.cols.length; ci++) {
      if (!canPick(L, st, ci)) continue;
      const pot = front(L, st, ci);
      let triedEmpty = false;
      for (let bi = 0; bi < st.bowls.length; bi++) {
        const b = st.bowls[bi];
        if (b.mask === 0) {
          if (triedEmpty) continue; // empty bowls are interchangeable
          triedEmpty = true;
        }
        const pv = preview(L, st, pot, bi);
        if (pv.ok) moves.push({ ci, bi, pv });
      }
    }
    return moves;
  }

  function remaining(st) {
    return PAINTABLE.reduce((s, c) => s + st.rem[c], 0);
  }

  function status(L, st) {
    if (remaining(st) === 0) return 'win';
    return legalMoves(L, st).length ? 'play' : 'lose';
  }

  // Pixels already painted: the first (total - rem) of each colour's list.
  function isPainted(L, st, pixelIdx) {
    const c = L.pixels[pixelIdx].c;
    const k = L.byColor[c].indexOf(pixelIdx);
    return k < L.totals[c] - st.rem[c];
  }

  // Depth-first search with memo of dead states. Returns a list of moves or null.
  function solve(L, st0, budget = 300000) {
    const dead = new Set();
    let nodes = 0;
    const key = (st) =>
      st.pos.join(',') + '|' +
      st.bowls.map((b) => b.mask + ':' + b.amt).sort().join(',') + '|' +
      PAINTABLE.map((c) => st.rem[c]).join(',');
    const order = { paint: 0, base: 1, useless: 2, mud: 3 };
    function dfs(st, path) {
      if (remaining(st) === 0) return path;
      if (++nodes > budget) return undefined;
      const k = key(st);
      if (dead.has(k)) return null;
      const moves = legalMoves(L, st).sort((a, b) => order[a.pv.kind] - order[b.pv.kind] || b.pv.paint - a.pv.paint);
      for (const m of moves) {
        const r = dfs(apply(L, st, m.ci, m.bi).st, path.concat([[m.ci, m.bi]]));
        if (r === undefined) return undefined;
        if (r) return r;
      }
      dead.add(k);
      return null;
    }
    const r = dfs(st0, []);
    return { path: r === undefined ? null : r, exhausted: r === undefined, nodes };
  }

  return {
    BIT, MASK, BY_MASK, PRIMARIES, SECONDARIES, PAINTABLE,
    prepare, newState, clone, front, iceLeft, canPick, isBase, preview, apply, rinse,
    legalMoves, remaining, status, isPainted, solve,
  };
});
