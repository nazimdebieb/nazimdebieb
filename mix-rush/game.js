// Mix Rush — rendering, input, animation and progression for the browser.
(() => {
  'use strict';
  const E = window.MixEngine;
  const LEVELS = window.MIX_LEVELS;

  const $ = (id) => document.getElementById(id);
  const canvas = $('board');
  const ctx = canvas.getContext('2d');

  // ---------- settings & progress (browser storage is a convenience only) ----------
  const STORE = 'mixrush.v1';
  const progress = loadProgress();
  function loadProgress() {
    const base = { unlocked: 1, stars: [], sound: true, glyphs: true };
    try { return Object.assign(base, JSON.parse(localStorage.getItem(STORE) || '{}')); } catch (e) { return base; }
  }
  function saveProgress() {
    try { localStorage.setItem(STORE, JSON.stringify(progress)); } catch (e) { /* storage blocked */ }
  }

  // ---------- theme tokens ----------
  let T = {};
  function readTokens() {
    const cs = getComputedStyle(document.documentElement);
    const g = (n) => cs.getPropertyValue(n).trim();
    T = {
      bg: g('--bg'), bg2: g('--bg-2'), panel: g('--panel'), ink: g('--ink'), muted: g('--muted'), line: g('--line'),
      accent: g('--accent'), frame: g('--frame'), canvas: g('--canvas'), bowl: g('--bowl'), bowlIn: g('--bowl-in'),
      mystery: g('--mystery'), ice: g('--ice'), ok: g('--ok'), warn: g('--warn'), bad: g('--bad'),
      font: g('--font-ui') || 'system-ui', display: g('--font-display') || 'system-ui',
      paint: { R: g('--paint-r'), Y: g('--paint-y'), B: g('--paint-b'), O: g('--paint-o'), G: g('--paint-g'), V: g('--paint-v'), M: g('--paint-m') },
    };
  }
  readTokens();
  matchMedia('(prefers-color-scheme: dark)').addEventListener?.('change', readTokens);
  new MutationObserver(readTokens).observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme', 'class', 'style'] });

  const NAMES = { R: 'rouge', Y: 'jaune', B: 'bleu', O: 'orange', G: 'vert', V: 'violet', M: 'boue' };

  // ---------- colour helpers ----------
  function hexToRgb(h) {
    h = h.replace('#', '');
    if (h.length === 3) h = h.split('').map((c) => c + c).join('');
    const n = parseInt(h, 16);
    return [(n >> 16) & 255, (n >> 8) & 255, n & 255];
  }
  function mix(a, b, t) {
    const A = hexToRgb(a), B = hexToRgb(b);
    const c = A.map((v, i) => Math.round(v + (B[i] - v) * t));
    return `rgb(${c[0]},${c[1]},${c[2]})`;
  }
  function luminance(h) {
    const [r, g, b] = hexToRgb(h).map((v) => v / 255);
    return 0.2126 * r + 0.7152 * g + 0.0722 * b;
  }

  // Compositional glyphs: red = vertical bar, yellow = horizontal bar, blue = ring.
  // A secondary carries both of its parents' marks, so the symbol teaches the recipe.
  function drawGlyph(c, cx, cy, s, stroke, lw, k = ctx) {
    const ctx = k;
    ctx.save();
    ctx.strokeStyle = stroke;
    ctx.lineWidth = lw || Math.max(1.5, s * 0.14);
    ctx.lineCap = 'round';
    const r = s * 0.5;
    const has = (p) => c !== 'M' && (E.MASK[c] & E.BIT[p]);
    ctx.beginPath();
    if (c === 'M') {
      ctx.moveTo(cx - r * 0.7, cy - r * 0.7); ctx.lineTo(cx + r * 0.7, cy + r * 0.7);
      ctx.moveTo(cx + r * 0.7, cy - r * 0.7); ctx.lineTo(cx - r * 0.7, cy + r * 0.7);
    } else {
      if (has('R')) { ctx.moveTo(cx, cy - r); ctx.lineTo(cx, cy + r); }
      if (has('Y')) { ctx.moveTo(cx - r, cy); ctx.lineTo(cx + r, cy); }
    }
    ctx.stroke();
    if (has('B')) { ctx.beginPath(); ctx.arc(cx, cy, r * 0.62, 0, Math.PI * 2); ctx.stroke(); }
    ctx.restore();
  }

  function roundRect(x, y, w, h, r) {
    ctx.beginPath();
    ctx.moveTo(x + r, y);
    ctx.arcTo(x + w, y, x + w, y + h, r);
    ctx.arcTo(x + w, y + h, x, y + h, r);
    ctx.arcTo(x, y + h, x, y, r);
    ctx.arcTo(x, y, x + w, y, r);
    ctx.closePath();
  }

  // ---------- sound ----------
  let audio = null;
  function tone(freq, dur, type = 'sine', gain = 0.06, slideTo) {
    if (!progress.sound) return;
    try {
      audio = audio || new (window.AudioContext || window.webkitAudioContext)();
      const t = audio.currentTime;
      const o = audio.createOscillator(), g = audio.createGain();
      o.type = type; o.frequency.setValueAtTime(freq, t);
      if (slideTo) o.frequency.exponentialRampToValueAtTime(slideTo, t + dur);
      g.gain.setValueAtTime(gain, t); g.gain.exponentialRampToValueAtTime(0.0001, t + dur);
      o.connect(g).connect(audio.destination); o.start(t); o.stop(t + dur + 0.02);
    } catch (e) { /* audio unavailable */ }
  }
  const sfx = {
    pick: () => tone(660, 0.07, 'sine', 0.05),
    drop: () => tone(320, 0.12, 'sine', 0.07, 170),
    tick: (k) => tone(520 + Math.min(k, 30) * 22, 0.05, 'triangle', 0.035),
    mud: () => tone(120, 0.35, 'sawtooth', 0.04, 70),
    nope: () => tone(180, 0.12, 'square', 0.03, 140),
    win: () => [523, 659, 784, 1046].forEach((f, i) => setTimeout(() => tone(f, 0.22, 'triangle', 0.06), i * 110)),
    lose: () => [392, 330, 262].forEach((f, i) => setTimeout(() => tone(f, 0.25, 'sine', 0.05), i * 140)),
  };
  const buzz = (ms) => { try { navigator.vibrate && navigator.vibrate(ms); } catch (e) { /* no vibration */ } };

  // ---------- game state ----------
  let levelIndex = 0;
  let L = null;          // prepared level
  let st = null;         // engine state
  let rank = [];         // pixel index -> position within its colour list
  let history = [];
  let selected = null;   // column index of the lifted pot
  let mode = 'play';     // 'play' | 'rinse' | 'won' | 'lost'
  let boosters = { undo: 3, rinse: 1 };
  let used = 0;
  let job = null;        // current move animation
  let pendingDrop = null;
  let pixelArrive = {};  // pixel index -> time it gets painted on screen
  let drains = {};       // bowl index -> rinse animation start
  let winAt = 0;
  let splats = [];
  let firstMoveDone = false;
  let lay = null;

  function loadLevel(i) {
    levelIndex = Math.max(0, Math.min(LEVELS.length - 1, i));
    L = E.prepare(LEVELS[levelIndex]);
    st = E.newState(L);
    rank = new Array(L.pixels.length);
    for (const c of E.PAINTABLE) L.byColor[c].forEach((p, k) => (rank[p] = k));
    history = [];
    selected = null;
    mode = 'play';
    boosters = { undo: 3, rinse: 1 };
    used = 0;
    job = null; pendingDrop = null; pixelArrive = {}; drains = {}; splats = []; winAt = 0;
    firstMoveDone = false;
    $('lvl-num').textContent = `Niveau ${levelIndex + 1} / ${LEVELS.length}`;
    $('lvl-name').textContent = L.def.name;
    showHint(L.def.hint);
    updateHud();
    resize();
    snapshot();
  }

  function painted(p) {
    const c = L.pixels[p].c;
    return rank[p] < L.totals[c] - st.rem[c];
  }

  // ---------- layout ----------
  let W = 0, H = 0, DPR = 1;
  function resize() {
    const r = canvas.getBoundingClientRect();
    DPR = Math.min(3, window.devicePixelRatio || 1);
    W = r.width; H = r.height;
    canvas.width = Math.round(W * DPR); canvas.height = Math.round(H * DPR);
    computeLayout();
  }
  window.addEventListener('resize', resize);

  function computeLayout() {
    if (!L) return;
    const pad = 16;
    const inner = W - pad * 2;
    const nCols = L.def.cols.length;
    const nBowls = L.def.bowls;
    const colW = Math.min(84, inner / nCols);
    const potW = Math.min(62, colW - 12, H * 0.085);
    const potH = potW * 1.12;
    const step = potH * 0.5;
    const behind = 3;
    const pileH = potH + step * behind + 18;
    const bowlW = Math.min(86, inner / nBowls - 10, H * 0.12);
    const bowlH = bowlW * 0.7;
    const chipH = 24;
    const bowlBlock = chipH + 8 + bowlH + 14;
    const picAvail = H - pileH - bowlBlock - 24;
    const cell = Math.max(8, Math.floor(Math.min(inner / L.w, picAvail / L.h, 44)));
    const picW = cell * L.w, picH = cell * L.h;
    const picX = (W - picW) / 2, picY = Math.max(10, (picAvail - picH) / 2 + 6);
    const bowlsY = picY + picH + 18 + chipH + 8;
    const bowls = [];
    const gapB = (inner - nBowls * bowlW) / nBowls;
    for (let i = 0; i < nBowls; i++) bowls.push({ x: pad + gapB / 2 + i * (bowlW + gapB), y: bowlsY, w: bowlW, h: bowlH });
    const pileY = bowlsY + bowlH + 20;
    const cols = [];
    const gapC = (inner - nCols * colW) / nCols;
    for (let i = 0; i < nCols; i++) {
      const cx = pad + gapC / 2 + i * (colW + gapC) + colW / 2;
      cols.push({ cx, y: pileY });
    }
    lay = { cell, picX, picY, picW, picH, bowls, cols, potW, potH, step, behind, chipH, bowlSlot: inner / nBowls };
  }

  function pixelCenter(p) {
    const px = L.pixels[p];
    return { x: lay.picX + (px.x + 0.5) * lay.cell, y: lay.picY + (px.y + 0.5) * lay.cell };
  }
  function bowlCenter(bi) {
    const b = lay.bowls[bi];
    return { x: b.x + b.w / 2, y: b.y + b.h * 0.22 };
  }
  function frontRect(ci) {
    const c = lay.cols[ci];
    return { x: c.cx - lay.potW / 2, y: c.y, w: lay.potW, h: lay.potH };
  }

  // ---------- moves ----------
  function busy() { return !!job; }

  function selectCol(ci) {
    if (mode !== 'play') return;
    const pot = E.front(L, st, ci);
    if (!pot) return;
    if (!E.canPick(L, st, ci)) { sfx.nope(); buzz(15); flash(`Ce pot est gelé encore ${E.iceLeft(st, pot)} coup${E.iceLeft(st, pot) > 1 ? 's' : ''}.`); return; }
    selected = selected === ci ? null : ci;
    if (selected !== null) { sfx.pick(); buzz(8); }
  }

  function dropInto(bi) {
    if (selected === null) return;
    const pot = E.front(L, st, selected);
    const pv = E.preview(L, st, pot, bi);
    if (!pv.ok) {
      sfx.nope(); buzz(20);
      flash(pv.why === 'full' ? 'Ce bol est trop plein pour ce pot.' : 'Ce bol est bouché par la boue. Rince-le pour le libérer.');
      return;
    }
    if (busy()) { pendingDrop = { ci: selected, bi }; selected = null; return; }
    doMove(selected, bi);
  }

  function doMove(ci, bi) {
    const from = frontRect(ci);
    const before = E.clone(st);
    const res = E.apply(L, st, ci, bi);
    history.push(before);
    st = res.st;
    selected = null;
    if (!firstMoveDone) { firstMoveDone = true; hideHint(); }
    const now = performance.now();
    const fly = 260;
    const landT = now + fly;
    const pixels = L.byColor[res.color] ? L.byColor[res.color].slice(res.startIdx, res.startIdx + res.paint) : [];
    const stagger = pixels.length > 20 ? 22 : 32;
    const travel = 360;
    const parts = pixels.map((p, k) => ({ p, t0: landT + 120 + k * stagger, dur: travel, color: res.color }));
    parts.forEach((pt) => (pixelArrive[pt.p] = pt.t0 + pt.dur));
    const end = parts.length ? parts[parts.length - 1].t0 + travel + 60 : landT + 180;
    job = { ci, bi, pot: res.pot, from, landT, start: now, parts, end, preBowl: before.bowls[bi], mixMask: E.MASK[res.color], amtBefore: res.amtBefore, kind: res.kind, ticked: 0 };
    setTimeout(() => {
      sfx.drop(); buzz(10);
      if (res.kind === 'mud') { sfx.mud(); buzz([30, 40, 30]); }
    }, fly);
    updateHud();
    snapshot();
  }

  function finishJob() {
    job = null;
    const s = E.status(L, st);
    if (s === 'win') return onWin();
    if (s === 'lose') return onLose();
    if (pendingDrop) {
      const { ci, bi } = pendingDrop; pendingDrop = null;
      if (E.canPick(L, st, ci) && E.preview(L, st, E.front(L, st, ci), bi).ok) doMove(ci, bi);
    }
  }

  function undo() {
    if (busy() || !history.length || boosters.undo <= 0 || mode === 'won') return;
    st = history.pop();
    boosters.undo--; used++;
    pixelArrive = {};
    selected = null;
    mode = 'play';
    closeModal();
    updateHud();
  }

  function toggleRinse() {
    if (busy() || mode === 'won') return;
    if (mode === 'rinse') { mode = 'play'; updateHud(); return; }
    if (boosters.rinse <= 0) { flash('Plus de rinçage pour ce niveau.'); return; }
    if (!st.bowls.some((b) => b.mask)) { flash('Tous les bols sont déjà vides.'); return; }
    mode = 'rinse'; selected = null;
    flash('Touche le bol à rincer.');
    updateHud();
  }

  function rinseBowl(bi) {
    if (!st.bowls[bi].mask) { flash('Ce bol est déjà vide.'); return; }
    history.push(E.clone(st));
    st = E.rinse(st, bi);
    boosters.rinse--; used++;
    drains[bi] = performance.now();
    mode = 'play';
    tone(700, 0.25, 'sine', 0.04, 250);
    updateHud();
    const s = E.status(L, st);
    if (s === 'lose') onLose();
  }

  function onWin() {
    mode = 'won';
    winAt = performance.now();
    sfx.win(); buzz([20, 40, 20, 40, 60]);
    const stars = used === 0 ? 3 : used <= 2 ? 2 : 1;
    progress.stars[levelIndex] = Math.max(progress.stars[levelIndex] || 0, stars);
    progress.unlocked = Math.max(progress.unlocked, Math.min(LEVELS.length, levelIndex + 2));
    saveProgress();
    for (let i = 0; i < 26; i++) {
      const c = E.PAINTABLE[i % 6];
      splats.push({ x: W / 2, y: lay.picY + lay.picH / 2, vx: (Math.random() - 0.5) * 9, vy: -Math.random() * 9 - 3, c, r: 4 + Math.random() * 6, t0: winAt });
    }
    setTimeout(() => showWin(stars), 1100);
  }

  function onLose() {
    mode = 'lost';
    sfx.lose(); buzz([40, 60, 40]);
    const out = L.def.cols.every((c, i) => st.pos[i] >= c.length);
    setTimeout(() => showLose(out), 450);
  }

  // ---------- HUD & dialogs ----------
  function updateHud() {
    $('undo-count').textContent = boosters.undo;
    $('rinse-count').textContent = boosters.rinse;
    $('btn-undo').disabled = !history.length || boosters.undo <= 0;
    $('btn-rinse').disabled = boosters.rinse <= 0;
    $('btn-rinse').classList.toggle('active', mode === 'rinse');
  }

  let flashMsg = null;
  function flash(text) { flashMsg = { text, t: performance.now() }; }

  function showHint(text) {
    if (!text) return hideHint();
    $('hint-text').innerHTML = text.replace(/(Jaune \+ Bleu = Vert|Rouge \+ Jaune = Orange|Rouge \+ Bleu = Violet|L’ordre compte|Nouveau)/g, '<b>$1</b>');
    $('hint').hidden = false;
  }
  function hideHint() { $('hint').hidden = true; }

  function starSvg(on) {
    const fill = on ? 'var(--paint-y)' : 'var(--bg-2)';
    return `<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 2.5l2.9 6 6.6.8-4.9 4.5 1.3 6.5L12 17l-5.9 3.3 1.3-6.5L2.5 9.3l6.6-.8z" fill="${fill}" stroke="var(--ink)" stroke-width="1.2" stroke-linejoin="round"/></svg>`;
  }

  function openModal(html) { $('modal-body').innerHTML = html; $('modal').hidden = false; }
  function closeModal() { $('modal').hidden = true; }

  function showWin(stars) {
    const last = levelIndex === LEVELS.length - 1;
    openModal(`
      <h2>Tableau terminé !</h2>
      <canvas id="win-thumb" width="160" height="160" style="width:160px;height:160px"></canvas>
      <div class="stars" aria-label="${stars} étoile${stars > 1 ? 's' : ''} sur 3">${[1, 2, 3].map((k) => starSvg(k <= stars)).join('')}</div>
      <p>« ${L.def.name} » rejoint ton musée.${used ? ` Boosters utilisés : ${used}.` : ' Sans aucun booster !'}</p>
      <div class="actions">
        ${last ? '<button class="btn primary" id="m-museum">Voir le musée</button>' : '<button class="btn primary" id="m-next">Tableau suivant</button>'}
        <button class="btn" id="m-replay">Rejouer</button>
      </div>`);
    drawThumb($('win-thumb'), LEVELS[levelIndex], true);
    $('m-next') && ($('m-next').onclick = () => { closeModal(); loadLevel(levelIndex + 1); });
    $('m-museum') && ($('m-museum').onclick = () => { closeModal(); openMuseum(); });
    $('m-replay').onclick = () => { closeModal(); loadLevel(levelIndex); };
  }

  function showLose(outOfPots) {
    const canOffer = !outOfPots && st.bowls.some((b) => b.mask);
    openModal(`
      <h2>${outOfPots ? 'Plus de peinture' : 'Plus aucun coup'}</h2>
      <p>${outOfPots ? 'Il ne reste plus de pots et le tableau n’est pas fini. Une partie de la peinture est partie au mauvais endroit.' : 'Aucun pot ne peut entrer dans tes bols. Rince un bol pour continuer, ou recommence.'}</p>
      <div class="actions">
        ${canOffer ? '<button class="btn primary" id="m-offer">Rincer un bol et continuer<small>Offert dans le prototype (pub récompensée dans le jeu final)</small></button>' : ''}
        ${history.length && boosters.undo > 0 ? `<button class="btn" id="m-undo">Annuler le dernier coup (${boosters.undo})</button>` : ''}
        <button class="btn ${canOffer ? '' : 'primary'}" id="m-retry">Recommencer</button>
      </div>`);
    $('m-offer') && ($('m-offer').onclick = () => { closeModal(); boosters.rinse++; mode = 'play'; updateHud(); toggleRinse(); });
    $('m-undo') && ($('m-undo').onclick = () => undo());
    $('m-retry').onclick = () => { closeModal(); loadLevel(levelIndex); };
  }

  function showRecipes() {
    const rows = [['R', 'Y', 'O'], ['Y', 'B', 'G'], ['R', 'B', 'V']];
    openModal(`
      <h2>Recettes</h2>
      <p>Chaque couleur a son symbole. Un mélange garde les symboles de ses deux couleurs.</p>
      <div class="recipes">
        ${rows.map((r) => `<div class="recipe">${sw(r[0])}<span>+</span>${sw(r[1])}<span>=</span>${sw(r[2])}<span>${NAMES[r[2]]}</span></div>`).join('')}
        <div class="recipe">${sw('R')}<span>+</span>${sw('Y')}<span>+</span>${sw('B')}<span>=</span>${sw('M')}<span>boue</span></div>
      </div>
      <p>Un bol peint dès qu’il a une couleur du tableau. L’ordre des pots compte.</p>
      <div class="actions"><button class="btn primary" id="m-close">Compris</button></div>`);
    document.querySelectorAll('canvas[data-sw]').forEach((cv) => {
      const c = cv.dataset.sw;
      const k = cv.getContext('2d');
      const s = 34 * DPR; cv.width = s; cv.height = s;
      k.fillStyle = T.paint[c]; k.beginPath(); k.arc(s / 2, s / 2, s / 2, 0, Math.PI * 2); k.fill();
      drawGlyph(c, s / 2, s / 2, s * 0.5, glyphInk(c), s * 0.07, k);
    });
    $('m-close').onclick = closeModal;
  }
  const sw = (c) => `<canvas data-sw="${c}" style="width:34px;height:34px" aria-label="${NAMES[c]}"></canvas>`;

  function glyphInk(c) { return luminance(T.paint[c]) > 0.55 ? 'rgba(20,24,30,.8)' : 'rgba(255,255,255,.92)'; }

  function drawThumb(cv, def, full) {
    const P = E.prepare(def);
    const k = cv.getContext('2d');
    const size = cv.clientWidth || cv.width;
    cv.width = Math.round(size * DPR); cv.height = Math.round(size * DPR);
    const s = cv.width;
    k.fillStyle = T.frame; k.fillRect(0, 0, s, s);
    const m = s * 0.08;
    const cell = Math.floor((s - m * 2) / Math.max(P.w, P.h));
    const ox = (s - cell * P.w) / 2, oy = (s - cell * P.h) / 2;
    k.fillStyle = T.canvas; k.fillRect(ox - cell * 0.3, oy - cell * 0.3, cell * P.w + cell * 0.6, cell * P.h + cell * 0.6);
    P.pixels.forEach((px) => {
      k.fillStyle = full === true ? T.paint[px.c] : full === 'ghost' ? mix(T.paint[px.c], T.canvas, 0.78) : mix(T.mystery, T.canvas, 0.5);
      k.fillRect(ox + px.x * cell, oy + px.y * cell, cell - 0.5, cell - 0.5);
    });
  }

  // ---------- museum ----------
  function openMuseum() {
    const g = $('gallery');
    g.innerHTML = LEVELS.map((lv, i) => {
      const open = i < progress.unlocked;
      const stars = progress.stars[i] || 0;
      return `<button class="frame" data-i="${i}" ${open ? '' : 'disabled'} aria-label="Niveau ${i + 1}, ${lv.name}${open ? '' : ', verrouillé'}">
        <canvas></canvas>
        <div class="meta"><span class="ttl">${lv.name}</span><span class="num">${i + 1}</span></div>
        <div class="st">${open ? (stars ? '★'.repeat(stars) + '☆'.repeat(3 - stars) : 'À peindre') : 'Verrouillé'}</div>
      </button>`;
    }).join('');
    $('museum').hidden = false;
    g.querySelectorAll('.frame').forEach((b) => {
      const i = +b.dataset.i;
      const done = !!progress.stars[i];
      drawThumb(b.querySelector('canvas'), LEVELS[i], done ? true : i < progress.unlocked ? 'ghost' : false);
      b.onclick = () => { $('museum').hidden = true; loadLevel(i); };
    });
    $('tg-sound').setAttribute('aria-pressed', progress.sound);
    $('tg-glyphs').setAttribute('aria-pressed', progress.glyphs);
  }

  // ---------- rendering ----------
  const ease = (t) => 1 - Math.pow(1 - Math.min(1, Math.max(0, t)), 3);

  function visualBowl(bi, now) {
    const b = st.bowls[bi];
    if (job && job.bi === bi) {
      if (now < job.landT) return job.preBowl;
      const departed = job.parts.filter((p) => p.t0 <= now).length;
      if (departed < job.parts.length) return { mask: job.mixMask, amt: job.amtBefore - departed };
      return b;
    }
    return b;
  }

  function draw(now) {
    const c = ctx;
    c.setTransform(DPR, 0, 0, DPR, 0, 0);
    c.clearRect(0, 0, W, H);
    if (!lay) return;
    drawPicture(now);
    drawBowls(now);
    drawPile(now);
    drawFlight(now);
    drawParticles(now);
    drawSplats(now);
    drawFlash(now);
  }

  function drawPicture(now) {
    const { picX, picY, picW, picH, cell } = lay;
    const m = Math.max(6, cell * 0.35);
    // easel frame + primed canvas
    ctx.fillStyle = 'rgba(0,0,0,.18)';
    roundRect(picX - m + 3, picY - m + 5, picW + m * 2, picH + m * 2, 10); ctx.fill();
    ctx.fillStyle = T.frame;
    roundRect(picX - m, picY - m, picW + m * 2, picH + m * 2, 10); ctx.fill();
    ctx.fillStyle = T.canvas;
    ctx.fillRect(picX - m * 0.45, picY - m * 0.45, picW + m * 0.9, picH + m * 0.9);
    const wonT = mode === 'won' ? now - winAt : -1;
    for (let p = 0; p < L.pixels.length; p++) {
      const px = L.pixels[p];
      const x = picX + px.x * cell, y = picY + px.y * cell;
      const col = T.paint[px.c];
      const arrive = pixelArrive[p] || 0;
      const shown = painted(p) && now >= arrive;
      if (shown) {
        const age = now - arrive;
        const pop = age < 200 ? 1 + 0.25 * Math.sin((age / 200) * Math.PI) : 1;
        const s = (cell - 1) * pop;
        ctx.fillStyle = col;
        roundRect(x + (cell - s) / 2, y + (cell - s) / 2, s, s, Math.min(4, cell * 0.12)); ctx.fill();
        ctx.fillStyle = 'rgba(255,255,255,.16)';
        ctx.fillRect(x + (cell - s) / 2 + 2, y + (cell - s) / 2 + 2, s - 4, Math.max(1, s * 0.22));
        if (wonT >= 0) {
          const wave = wonT / 900 - (px.x + px.y) / (L.w + L.h);
          if (wave > 0 && wave < 0.25) {
            ctx.fillStyle = `rgba(255,255,255,${0.55 * Math.sin((wave / 0.25) * Math.PI)})`;
            ctx.fillRect(x, y, cell - 1, cell - 1);
          }
        }
      } else {
        ctx.fillStyle = mix(col, T.canvas, 0.74);
        roundRect(x + 1, y + 1, cell - 2, cell - 2, Math.min(4, cell * 0.12)); ctx.fill();
        if (progress.glyphs && cell >= 12) drawGlyph(px.c, x + cell / 2, y + cell / 2, cell * 0.46, mix(col, '#1b1f24', 0.25), Math.max(1.2, cell * 0.07));
      }
    }
  }

  function drawBowls(now) {
    const pot = selected !== null ? E.front(L, st, selected) : null;
    lay.bowls.forEach((r, bi) => {
      const b = visualBowl(bi, now);
      const cx = r.x + r.w / 2;
      const rimY = r.y + r.h * 0.22;
      const rx = r.w / 2, ry = r.h * 0.2;
      let pv = null;
      if (pot && mode === 'play') pv = E.preview(L, st, pot, bi);
      const target = pv && pv.ok;
      // shadow
      ctx.fillStyle = 'rgba(0,0,0,.14)';
      ctx.beginPath(); ctx.ellipse(cx, r.y + r.h + 4, rx * 0.7, 5, 0, 0, Math.PI * 2); ctx.fill();
      // body
      ctx.beginPath();
      ctx.moveTo(r.x, rimY);
      ctx.bezierCurveTo(r.x + 2, r.y + r.h * 1.05, r.x + r.w - 2, r.y + r.h * 1.05, r.x + r.w, rimY);
      ctx.closePath();
      ctx.fillStyle = T.bowl; ctx.fill();
      ctx.lineWidth = target || (mode === 'rinse' && st.bowls[bi].mask) ? 3 : 1.5;
      ctx.strokeStyle = target ? (pv.kind === 'paint' ? T.ok : pv.kind === 'base' ? T.ink : pv.kind === 'useless' ? T.warn : T.bad) : mode === 'rinse' && st.bowls[bi].mask ? T.accent : T.line;
      ctx.stroke();
      // rim
      ctx.beginPath(); ctx.ellipse(cx, rimY, rx, ry, 0, 0, Math.PI * 2);
      ctx.fillStyle = T.bowlIn; ctx.fill(); ctx.stroke();
      // liquid
      let mask = b.mask, amt = b.amt;
      const dr = drains[bi];
      let drainK = 1;
      if (dr) { const t = (now - dr) / 400; if (t >= 1) delete drains[bi]; else drainK = 1 - t; }
      if (mask || (dr && drainK < 1)) {
        const color = E.BY_MASK[mask] || 'M';
        const fill = Math.min(1, amt / L.def.cap);
        const k = (0.45 + 0.5 * fill) * (dr ? drainK : 1);
        const splash = job && job.bi === bi && now > job.landT && now < job.landT + 250 ? Math.sin(((now - job.landT) / 250) * Math.PI) * 0.12 : 0;
        ctx.beginPath(); ctx.ellipse(cx, rimY + 1, (rx - 5) * (k + splash), (ry - 2) * (k + splash), 0, 0, Math.PI * 2);
        ctx.fillStyle = T.paint[color]; ctx.fill();
        if (color === 'M') {
          for (let i = 0; i < 3; i++) {
            const ph = (now / 700 + i * 0.37) % 1;
            ctx.beginPath();
            ctx.arc(cx + (i - 1) * rx * 0.35, rimY - ph * ry * 0.8, 1.5 + ph * 3, 0, Math.PI * 2);
            ctx.strokeStyle = 'rgba(255,255,255,.45)'; ctx.lineWidth = 1.2; ctx.stroke();
          }
        }
        if (progress.glyphs && mask) drawGlyph(color, cx, rimY + 1, Math.min(ry * 1.4, 16), glyphInk(color), 2.2);
      }
      // amount on the bowl body
      ctx.fillStyle = T.muted;
      ctx.font = `600 ${Math.round(Math.max(11, r.w * 0.16))}px ${T.font}`;
      ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
      ctx.fillText(b.mask ? `${b.amt}/${L.def.cap}` : 'vide', cx, r.y + r.h * 0.66);
      // preview chip
      if (pv) drawChip(cx, r.y - lay.chipH - 6, pv);
      else if (mode === 'rinse' && st.bowls[bi].mask) drawChip(cx, r.y - lay.chipH - 6, { ok: true, kind: 'rinse' });
    });
  }

  function drawChip(cx, y, pv) {
    let text, color, sw = null;
    if (!pv.ok) { text = pv.why === 'full' ? 'Plein' : 'Bouché'; color = T.bad; }
    else if (pv.kind === 'rinse') { text = 'Rincer'; color = T.accent; }
    else {
      sw = pv.color;
      if (pv.kind === 'paint') { text = `+${pv.paint}`; color = T.ok; }
      else if (pv.kind === 'base') { text = 'Réserve'; color = T.ink; }
      else if (pv.kind === 'useless') { text = 'Inutile'; color = T.warn; }
      else { text = 'Boue !'; color = T.bad; }
    }
    ctx.font = `600 12px ${T.font}`;
    const tw = ctx.measureText(text).width;
    const slot = lay.bowlSlot - 4;
    if (sw && tw + 12 + 20 > slot) sw = null; // too narrow: the text alone
    const h = lay.chipH, swR = sw ? 7 : 0;
    const w = Math.min(slot, tw + 12 + (sw ? swR * 2 + 5 : 0));
    const x = cx - w / 2;
    ctx.fillStyle = T.panel; roundRect(x, y, w, h, h / 2); ctx.fill();
    ctx.strokeStyle = color; ctx.lineWidth = 1.5; ctx.stroke();
    let tx = x + 6;
    if (sw) {
      ctx.beginPath(); ctx.arc(tx + swR, y + h / 2, swR, 0, Math.PI * 2); ctx.fillStyle = T.paint[sw]; ctx.fill();
      if (progress.glyphs) drawGlyph(sw, tx + swR, y + h / 2, 8, glyphInk(sw), 1.5);
      tx += swR * 2 + 5;
    }
    ctx.fillStyle = color; ctx.textAlign = 'left'; ctx.textBaseline = 'middle';
    ctx.fillText(text, tx, y + h / 2 + 0.5, x + w - tx - 4);
  }

  function drawPot(pot, x, y, w, h, opts = {}) {
    const hiddenPot = pot.hidden && !opts.front;
    const col = hiddenPot ? T.mystery : T.paint[pot.c];
    const r = w * 0.18;
    ctx.save();
    ctx.globalAlpha = opts.alpha ?? 1;
    // shadow
    ctx.fillStyle = 'rgba(0,0,0,.16)';
    roundRect(x + 2, y + 4, w, h, r); ctx.fill();
    // body
    ctx.fillStyle = col; roundRect(x, y + h * 0.12, w, h * 0.88, r); ctx.fill();
    // lid
    ctx.fillStyle = mix(col, '#000000', 0.25);
    roundRect(x - w * 0.04, y, w * 1.08, h * 0.2, r * 0.7); ctx.fill();
    // label band
    const ly = y + h * 0.38, lh = h * 0.4;
    ctx.fillStyle = 'rgba(255,255,255,.92)'; roundRect(x + w * 0.1, ly, w * 0.8, lh, lh * 0.3); ctx.fill();
    ctx.fillStyle = '#1b2129';
    ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
    if (opts.ice > 0) {
      // The ice covers the label below.
    } else if (hiddenPot) {
      ctx.font = `700 ${Math.round(lh * 0.7)}px ${T.font}`;
      ctx.fillText('?', x + w / 2, ly + lh / 2 + 1);
    } else {
      const g = progress.glyphs;
      ctx.font = `700 ${Math.round(lh * 0.62)}px ${T.font}`;
      ctx.fillText(String(pot.n), x + w * (g ? 0.64 : 0.5), ly + lh / 2 + 1);
      if (g) drawGlyph(pot.c, x + w * 0.3, ly + lh / 2, lh * 0.55, mix(T.paint[pot.c], '#1b2129', 0.35), Math.max(1.6, w * 0.045));
    }
    if (opts.ice > 0) {
      ctx.fillStyle = 'rgba(214,239,252,.96)'; roundRect(x - 2, y - 2, w + 4, h + 4, r); ctx.fill();
      ctx.strokeStyle = T.ice; ctx.lineWidth = 2; ctx.stroke();
      ctx.fillStyle = '#12405c'; ctx.font = `700 ${Math.round(w * 0.34)}px ${T.font}`;
      ctx.fillText(String(opts.ice), x + w / 2, y + h * 0.58);
      ctx.font = `600 ${Math.round(w * 0.16)}px ${T.font}`;
      ctx.fillText('gelé', x + w / 2, y + h * 0.84);
    }
    if (opts.selected) {
      ctx.strokeStyle = T.accent; ctx.lineWidth = 3;
      roundRect(x - 4, y - 4, w + 8, h + 8, r + 3); ctx.stroke();
    }
    ctx.restore();
  }

  function drawPile(now) {
    const { potW, potH, step, behind } = lay;
    L.def.cols.forEach((col, ci) => {
      const c = lay.cols[ci];
      const start = st.pos[ci];
      // column tray
      ctx.fillStyle = T.bg2;
      roundRect(c.cx - potW / 2 - 6, c.y - 6, potW + 12, potH + step * behind + 14, 14); ctx.fill();
      const visible = col.slice(start, start + behind + 1);
      for (let k = visible.length - 1; k >= 0; k--) {
        const pot = visible[k];
        const isFront = k === 0;
        const s = isFront ? 1 : 0.8;
        const w = potW * s, h = potH * s;
        let y = c.y + (isFront ? 0 : potH * 0.62 + (k - 1) * step);
        const x = c.cx - w / 2;
        const sel = isFront && selected === ci;
        if (sel) y -= 8 + Math.sin(now / 160) * 2;
        const ice = E.iceLeft(st, pot);
        const pickable = isFront && ice === 0;
        const hintPulse = levelIndex === 0 && !firstMoveDone && pickable && selected === null;
        if (hintPulse) {
          ctx.strokeStyle = T.accent; ctx.lineWidth = 2;
          const g = 4 + Math.sin(now / 200) * 3;
          roundRect(x - g, y - g, w + g * 2, h + g * 2, 12); ctx.stroke();
        }
        drawPot(pot, x, y, w, h, { front: isFront, selected: sel, ice, alpha: isFront ? 1 : 0.92 });
      }
      const more = col.length - start - visible.length;
      if (more > 0) {
        ctx.fillStyle = T.muted; ctx.font = `600 12px ${T.font}`; ctx.textAlign = 'center'; ctx.textBaseline = 'top';
        ctx.fillText(`+${more}`, c.cx, c.y + potH * 0.62 + (behind - 1) * step + potH * 0.8 + 2);
      }
      if (start >= col.length) {
        ctx.fillStyle = T.muted; ctx.font = `500 12px ${T.font}`; ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
        ctx.fillText('vide', c.cx, c.y + potH / 2);
      }
    });
  }

  function drawFlight(now) {
    if (!job || now >= job.landT) return;
    const t = ease((now - job.start) / (job.landT - job.start));
    const to = bowlCenter(job.bi);
    const fx = job.from.x + job.from.w / 2, fy = job.from.y + job.from.h / 2;
    const x = fx + (to.x - fx) * t;
    const y = fy + (to.y - fy) * t - Math.sin(t * Math.PI) * 40;
    const s = 1 - 0.45 * t;
    const w = lay.potW * s, h = lay.potH * s;
    ctx.save();
    ctx.translate(x, y); ctx.rotate(t * 0.9);
    drawPot(job.pot, -w / 2, -h / 2, w, h, { front: true });
    ctx.restore();
  }

  function drawParticles(now) {
    if (!job) return;
    const from = bowlCenter(job.bi);
    for (const pt of job.parts) {
      if (now < pt.t0 || now > pt.t0 + pt.dur) continue;
      const t = ease((now - pt.t0) / pt.dur);
      const to = pixelCenter(pt.p);
      const x = from.x + (to.x - from.x) * t;
      const y = from.y + (to.y - from.y) * t - Math.sin(t * Math.PI) * 30;
      ctx.beginPath(); ctx.arc(x, y, Math.max(3, lay.cell * 0.28) * (1 - t * 0.3), 0, Math.PI * 2);
      ctx.fillStyle = T.paint[pt.color]; ctx.fill();
    }
    const arrived = job.parts.filter((p) => now >= p.t0 + p.dur).length;
    while (job.ticked < arrived) { sfx.tick(job.ticked); job.ticked++; }
    if (now >= job.end) finishJob();
  }

  function drawSplats(now) {
    if (!splats.length) return;
    splats = splats.filter((s) => now - s.t0 < 1600);
    for (const s of splats) {
      const t = (now - s.t0) / 16.7;
      const x = s.x + s.vx * t, y = s.y + s.vy * t + 0.18 * t * t;
      ctx.globalAlpha = Math.max(0, 1 - (now - s.t0) / 1600);
      ctx.beginPath(); ctx.arc(x, y, s.r, 0, Math.PI * 2); ctx.fillStyle = T.paint[s.c]; ctx.fill();
      ctx.globalAlpha = 1;
    }
  }

  function drawFlash(now) {
    if (!flashMsg) return;
    const age = now - flashMsg.t;
    if (age > 2200) { flashMsg = null; return; }
    const a = age < 150 ? age / 150 : age > 1900 ? (2200 - age) / 300 : 1;
    ctx.globalAlpha = a;
    ctx.font = `500 14px ${T.font}`;
    const words = flashMsg.text;
    const w = Math.min(W - 32, ctx.measureText(words).width + 28);
    const x = (W - w) / 2, y = lay.picY + lay.picH - 8;
    ctx.fillStyle = T.ink; roundRect(x, y, w, 34, 17); ctx.fill();
    ctx.fillStyle = T.bg; ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
    ctx.fillText(words, W / 2, y + 17, W - 48);
    ctx.globalAlpha = 1;
  }

  function loop(now) {
    draw(now);
    requestAnimationFrame(loop);
  }

  // ---------- input ----------
  function hit(e) {
    const r = canvas.getBoundingClientRect();
    const x = e.clientX - r.left, y = e.clientY - r.top;
    for (let bi = 0; bi < lay.bowls.length; bi++) {
      const b = lay.bowls[bi];
      if (x >= b.x - 6 && x <= b.x + b.w + 6 && y >= b.y - lay.chipH - 10 && y <= b.y + b.h + 12) return { bowl: bi };
    }
    for (let ci = 0; ci < lay.cols.length; ci++) {
      const c = lay.cols[ci];
      if (Math.abs(x - c.cx) <= lay.potW / 2 + 8 && y >= c.y - 14 && y <= c.y + lay.potH + lay.step * lay.behind + 12) return { col: ci };
    }
    return {};
  }

  let dragFrom = null;
  canvas.addEventListener('pointerdown', (e) => {
    if (!lay || mode === 'won' || mode === 'lost') return;
    if (audio && audio.state === 'suspended') audio.resume();
    const h = hit(e);
    if (mode === 'rinse') {
      if (h.bowl !== undefined) rinseBowl(h.bowl);
      else { mode = 'play'; updateHud(); }
      return;
    }
    if (h.col !== undefined) {
      if (selected === h.col) selected = null;
      else selectCol(h.col);
      dragFrom = selected !== null ? { ci: selected, x: e.clientX, y: e.clientY } : null;
    } else if (h.bowl !== undefined) {
      if (selected !== null) dropInto(h.bowl);
      else if (st.bowls[h.bowl].mask) flash(describeBowl(h.bowl));
    } else {
      selected = null;
    }
  });
  canvas.addEventListener('pointerup', (e) => {
    if (!dragFrom) return;
    const moved = Math.hypot(e.clientX - dragFrom.x, e.clientY - dragFrom.y) > 20;
    const h = hit(e);
    if (moved && h.bowl !== undefined && selected === dragFrom.ci) dropInto(h.bowl);
    dragFrom = null;
  });

  function describeBowl(bi) {
    const b = st.bowls[bi];
    const c = E.BY_MASK[b.mask];
    if (c === 'M') return 'De la boue : ce bol est bouché. Rince-le.';
    const useful = st.rem[c] > 0 || E.isBase(st, c);
    return `${b.amt} de ${NAMES[c]} en ${useful ? 'réserve' : 'trop : couleur inutile ici'}.`;
  }

  // ---------- buttons ----------
  $('btn-undo').onclick = undo;
  $('btn-rinse').onclick = toggleRinse;
  $('btn-restart').onclick = () => { if (!busy()) loadLevel(levelIndex); };
  $('btn-recipes').onclick = showRecipes;
  $('btn-museum').onclick = openMuseum;
  $('hint-close').onclick = hideHint;
  $('modal').addEventListener('pointerdown', (e) => { if (e.target === $('modal') && mode === 'play') closeModal(); });
  $('tg-sound').onclick = () => { progress.sound = !progress.sound; saveProgress(); $('tg-sound').setAttribute('aria-pressed', progress.sound); if (progress.sound) sfx.pick(); };
  $('tg-glyphs').onclick = () => { progress.glyphs = !progress.glyphs; saveProgress(); $('tg-glyphs').setAttribute('aria-pressed', progress.glyphs); };
  $('tg-reset').onclick = () => {
    progress.unlocked = 1; progress.stars = []; saveProgress(); openMuseum();
  };

  // ---------- hot update hook of the artifact viewer ----------
  function snapshot() {
    try { window.claude?.hot?.snapshot?.(() => ({ level: levelIndex })); } catch (e) { /* not in a viewer */ }
  }

  function start(data) {
    const saved = data && Number.isInteger(data.level) ? data.level : null;
    const first = saved ?? Math.min(progress.unlocked - 1, LEVELS.length - 1);
    loadLevel(first);
    if (saved === null) openMuseum();
    requestAnimationFrame(loop);
  }
  // Test hook for automated play-throughs (open the page with #debug).
  if (location.hash === '#debug') {
    window.MixDebug = {
      level: () => levelIndex, state: () => st, prepared: () => L, layout: () => lay, mode: () => mode,
      busy: () => busy(), load: (i) => loadLevel(i),
    };
  }

  if (window.claude?.hot?.ready) window.claude.hot.ready(start);
  else start(window.claude?.hot?.data ?? {});

  // Fonts can change measured text; redraw layout once they are ready.
  document.fonts && document.fonts.ready.then(() => { readTokens(); resize(); });
})();
