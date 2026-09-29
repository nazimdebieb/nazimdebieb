// Orbite — circular brick breaker without a paddle.
// The ball orbits the core; a tap releases it straight outward. It bounces off
// bricks and the arena wall, then the core's gravity pulls it back into orbit.
// Rings of bricks creep toward the core; if one reaches the danger ring, game over.
(() => {
  'use strict';
  const $ = (id) => document.getElementById(id);
  const cv = $('game');
  const ctx = cv.getContext('2d');

  // ---------- world constants (world units: arena radius = 180) ----------
  const ARENA = 180;
  const ORBIT = 36;
  const BALL = 5.5;
  const RING = 17;
  const RGAP = 3;
  const DANGER = ORBIT + BALL + 5;
  const OMEGA = 2.7;           // orbit speed, rad/s
  const LAUNCH = 340;          // launch speed, units/s
  const GRAVITY = 140;         // pull toward the core, units/s²
  const GHOST_AFTER = 6;       // seconds of flight before the ball ignores bricks
  const MAX_BALLS = 4;

  const COLORS = {
    hp1: '#5ef0b8', hp2: '#ffd166', hp3: '#ff6b8b',
    bomb: '#ff8a3d', plus: '#6fb6ff', slow: '#9be7ff', shield: '#6f6a8a',
    core: '#ffb23f', danger: '#ff4d6d', ball: '#ffffff', text: '#f4efff', muted: '#a99bd0',
  };

  // ---------- storage (a per-viewer convenience) ----------
  const store = {
    get(k, d) { try { const v = localStorage.getItem('orbite.' + k); return v === null ? d : JSON.parse(v); } catch (e) { return d; } },
    set(k, v) { try { localStorage.setItem('orbite.' + k, JSON.stringify(v)); } catch (e) { /* blocked */ } },
  };
  let best = store.get('best', 0);
  let sound = store.get('sound', true);

  // ---------- sound ----------
  let ac = null;
  function tone(f, dur, type = 'sine', g = 0.05, to) {
    if (!sound) return;
    try {
      ac = ac || new (window.AudioContext || window.webkitAudioContext)();
      const t = ac.currentTime, o = ac.createOscillator(), gn = ac.createGain();
      o.type = type; o.frequency.setValueAtTime(f, t);
      if (to) o.frequency.exponentialRampToValueAtTime(to, t + dur);
      gn.gain.setValueAtTime(g, t); gn.gain.exponentialRampToValueAtTime(0.0001, t + dur);
      o.connect(gn).connect(ac.destination); o.start(t); o.stop(t + dur + 0.02);
    } catch (e) { /* no audio */ }
  }
  const sfx = {
    launch: () => tone(240, 0.12, 'triangle', 0.05, 520),
    hit: (n) => tone(330 * Math.pow(1.0595, Math.min(n, 24) * 2), 0.07, 'square', 0.035),
    wall: () => tone(140, 0.05, 'sine', 0.03),
    catch: () => tone(660, 0.08, 'sine', 0.03, 880),
    boom: () => { tone(90, 0.35, 'sawtooth', 0.06, 40); tone(200, 0.2, 'square', 0.03, 60); },
    bonus: () => [660, 880, 1100].forEach((f, i) => setTimeout(() => tone(f, 0.09, 'triangle', 0.05), i * 60)),
    over: () => [392, 311, 233, 175].forEach((f, i) => setTimeout(() => tone(f, 0.28, 'sawtooth', 0.04), i * 150)),
  };
  const buzz = (p) => { try { navigator.vibrate && navigator.vibrate(p); } catch (e) { /* none */ } };

  // ---------- screen ----------
  let W = 0, H = 0, DPR = 1, CX = 0, CY = 0, U = 1;
  function resize() {
    DPR = Math.min(3, window.devicePixelRatio || 1);
    W = window.innerWidth; H = window.innerHeight;
    cv.width = Math.round(W * DPR); cv.height = Math.round(H * DPR);
    const top = 96, bottom = 40;
    const R = Math.max(120, Math.min(W / 2 - 10, (H - top - bottom) / 2));
    U = R / ARENA;
    CX = W / 2;
    CY = top + (H - top - bottom) / 2;
  }
  window.addEventListener('resize', resize);
  resize();

  // ---------- game state ----------
  let g = null;
  let state = 'menu'; // menu | play | pause | over
  let last = performance.now();

  function newGame() {
    g = {
      score: 0, wave: 0, t: 0,
      balls: [orbitBall(0)],
      bricks: [], parts: [], pops: [],
      shake: 0, slowUntil: 0, continued: false, flashDanger: 0,
      broken: 0, bestCombo: 0, cleared: 0,
    };
    for (let i = 0; i < 3; i++) spawnRing(104 + i * (RING + RGAP), true);
    updateHud();
  }

  function orbitBall(a) { return { mode: 'orbit', a, x: 0, y: 0, vx: 0, vy: 0, t: 0, hits: 0, trail: [] }; }

  function speed() {
    const base = Math.min(8, 1.2 + g.wave * 0.2);
    return g.t < g.slowUntil ? base * 0.35 : base;
  }

  const rnd = Math.random;
  function spawnRing(r0, initial) {
    if (!initial) g.wave++;
    const w = g.wave;
    const k = 12 + Math.min(8, Math.floor(w / 2));
    const span = (Math.PI * 2) / k;
    const off = rnd() * span;
    const gap = 0.045;
    const holes = Math.max(0.06, 0.2 - w * 0.01);
    for (let i = 0; i < k; i++) {
      if (rnd() < holes) continue;
      let type = 'n', hp = 1;
      const pShield = w >= 7 ? Math.min(0.12, (w - 6) * 0.015) : 0;
      const pBomb = w >= 1 ? 0.07 : 0;
      const pPlus = g.balls.length < MAX_BALLS ? 0.03 : 0;
      const pSlow = w >= 3 ? 0.03 : 0;
      let r = rnd();
      if ((r -= pShield) < 0) { type = 'shield'; hp = Infinity; }
      else if ((r -= pBomb) < 0) type = 'bomb';
      else if ((r -= pPlus) < 0) type = 'plus';
      else if ((r -= pSlow) < 0) type = 'slow';
      if (type === 'n') {
        const q = rnd();
        if (q < Math.min(0.22, (w - 4) * 0.02)) hp = 3;
        else if (q < Math.min(0.55, 0.05 + w * 0.04)) hp = 2;
      }
      g.bricks.push({ r0, r1: r0 + RING, a0: off + i * span + gap / 2, a1: off + (i + 1) * span - gap / 2, hp, max: hp, type, flash: 0 });
    }
    if (!initial) updateHud();
  }

  // ---------- helpers ----------
  const TAU = Math.PI * 2;
  function angDiff(a, b) { let d = (a - b) % TAU; if (d > Math.PI) d -= TAU; if (d < -Math.PI) d += TAU; return d; }
  function brickMid(b) { return (b.a0 + b.a1) / 2; }
  function inBrick(b, x, y, pad) {
    const r = Math.hypot(x, y);
    if (r < b.r0 - pad || r > b.r1 + pad) return false;
    const half = (b.a1 - b.a0) / 2 + pad / Math.max(r, 1);
    return Math.abs(angDiff(Math.atan2(y, x), brickMid(b))) <= half;
  }

  // ---------- actions ----------
  function launch() {
    let any = false;
    for (const b of g.balls) {
      if (b.mode !== 'orbit') continue;
      b.mode = 'fly'; b.t = 0; b.hits = 0;
      b.x = Math.cos(b.a) * ORBIT; b.y = Math.sin(b.a) * ORBIT;
      b.vx = Math.cos(b.a) * LAUNCH; b.vy = Math.sin(b.a) * LAUNCH;
      any = true;
    }
    if (any) { sfx.launch(); buzz(8); }
  }

  function hitBrick(br, ball) {
    br.flash = 0.12;
    if (br.type === 'shield') { sfx.wall(); return; }
    ball.hits++;
    br.hp--;
    sfx.hit(ball.hits);
    if (br.hp <= 0) breakBrick(br, ball.hits);
  }

  function breakBrick(br, combo) {
    const i = g.bricks.indexOf(br);
    if (i < 0) return;
    g.bricks.splice(i, 1);
    g.broken++;
    const pts = 10 * Math.max(1, combo);
    g.score += pts;
    const mid = brickMid(br), rm = (br.r0 + br.r1) / 2;
    const x = Math.cos(mid) * rm, y = Math.sin(mid) * rm;
    const col = brickColor(br);
    for (let k = 0; k < 7; k++) {
      const a = rnd() * TAU, s = 30 + rnd() * 90;
      g.parts.push({ x, y, vx: Math.cos(a) * s + Math.cos(mid) * 40, vy: Math.sin(a) * s + Math.sin(mid) * 40, life: 0.6 + rnd() * 0.4, t: 0, col, size: 2 + rnd() * 3 });
    }
    pop(x, y, '+' + pts, combo >= 3 ? COLORS.core : COLORS.text, combo >= 3 ? 1.25 : 1);
    if (br.type === 'bomb') explode(x, y, combo);
    if (br.type === 'plus' && g.balls.length < MAX_BALLS) {
      const orbiting = g.balls.find((b) => b.mode === 'orbit');
      g.balls.push(orbitBall((orbiting ? orbiting.a : 0) + Math.PI));
      pop(x, y - 14, '+1 bille', COLORS.plus, 1.1);
      sfx.bonus(); buzz([10, 30, 10]);
    }
    if (br.type === 'slow') {
      g.slowUntil = g.t + 7;
      pop(x, y - 14, 'Ralenti', COLORS.slow, 1.1);
      sfx.bonus();
    }
    if (!g.bricks.some((b) => b.type !== 'shield')) {
      g.score += 250; g.cleared++;
      pop(0, 0, 'Anneaux nettoyés +250', COLORS.core, 1.3);
      sfx.bonus();
    }
    updateHud();
  }

  function explode(x, y, combo) {
    g.shake = 0.35; sfx.boom(); buzz([30, 20, 40]);
    for (let k = 0; k < 24; k++) {
      const a = rnd() * TAU, s = 60 + rnd() * 160;
      g.parts.push({ x, y, vx: Math.cos(a) * s, vy: Math.sin(a) * s, life: 0.5 + rnd() * 0.4, t: 0, col: k % 2 ? COLORS.bomb : COLORS.core, size: 2 + rnd() * 3 });
    }
    const victims = g.bricks.filter((b) => {
      if (b.type === 'shield') return false;
      const m = brickMid(b), rm = (b.r0 + b.r1) / 2;
      return Math.hypot(Math.cos(m) * rm - x, Math.sin(m) * rm - y) < 46;
    });
    victims.forEach((b) => { b.hp = 0; breakBrick(b, combo + 1); });
  }

  function pop(x, y, text, col, scale) { g.pops.push({ x, y, text, col, scale, t: 0 }); }

  // ---------- simulation ----------
  function step(dt) {
    g.t += dt;
    // Bricks creep inward.
    const v = speed() * dt;
    let minR = Infinity, maxR = 0;
    for (const b of g.bricks) {
      b.r0 -= v; b.r1 -= v;
      if (b.flash > 0) b.flash -= dt;
      if (b.r0 < minR) minR = b.r0;
      if (b.r1 > maxR) maxR = b.r1;
    }
    if (!g.bricks.length) spawnRing(ARENA - RING - 4);
    else if (maxR <= ARENA - RING - RGAP - 4) spawnRing(maxR + RGAP);
    g.flashDanger = minR < DANGER + 25 ? 1 - (minR - DANGER) / 25 : 0;
    if (minR <= DANGER) return gameOver();

    // Balls.
    for (const b of g.balls) {
      if (b.mode === 'orbit') { b.a += OMEGA * dt; b.trail.length = 0; continue; }
      b.t += dt;
      const n = Math.max(1, Math.ceil((Math.hypot(b.vx, b.vy) * dt) / 2.5));
      const h = dt / n;
      for (let s = 0; s < n && b.mode === 'fly'; s++) moveBall(b, h);
      b.trail.push([b.x, b.y]);
      if (b.trail.length > 10) b.trail.shift();
    }

    for (const p of g.parts) { p.t += dt; p.x += p.vx * dt; p.y += p.vy * dt; p.vx *= 0.96; p.vy *= 0.96; }
    g.parts = g.parts.filter((p) => p.t < p.life);
    for (const p of g.pops) p.t += dt;
    g.pops = g.pops.filter((p) => p.t < 0.9);
    if (g.shake > 0) g.shake -= dt;
  }

  function moveBall(b, h) {
    const r = Math.hypot(b.x, b.y) || 1;
    const pull = GRAVITY * (1 + b.t * 0.7);
    b.vx -= (b.x / r) * pull * h;
    b.vy -= (b.y / r) * pull * h;
    const px = b.x, py = b.y;
    b.x += b.vx * h; b.y += b.vy * h;
    const nr = Math.hypot(b.x, b.y);
    // Back into orbit.
    if (nr <= ORBIT && b.x * b.vx + b.y * b.vy < 0) {
      b.mode = 'orbit';
      b.a = Math.atan2(b.y, b.x);
      if (b.hits >= 3) pop(Math.cos(b.a) * (ORBIT + 22), Math.sin(b.a) * (ORBIT + 22), `Combo x${b.hits}`, COLORS.core, 1.2);
      g.bestCombo = Math.max(g.bestCombo, b.hits);
      sfx.catch();
      return;
    }
    // Arena wall.
    if (nr > ARENA - BALL) {
      const nx = b.x / nr, ny = b.y / nr;
      const d = b.vx * nx + b.vy * ny;
      if (d > 0) { b.vx -= 2 * d * nx; b.vy -= 2 * d * ny; sfx.wall(); }
      b.x = nx * (ARENA - BALL); b.y = ny * (ARENA - BALL);
      return;
    }
    if (b.t > GHOST_AFTER) return;
    for (const br of g.bricks) {
      if (!inBrick(br, b.x, b.y, BALL)) continue;
      const pr = Math.hypot(px, py);
      let nx, ny;
      if (pr < br.r0 - BALL || pr > br.r1 + BALL) { nx = px / pr; ny = py / pr; }   // hit an arc face
      else { const a = Math.atan2(py, px); nx = -Math.sin(a); ny = Math.cos(a); }    // hit a side face
      const d = b.vx * nx + b.vy * ny;
      b.vx -= 2 * d * nx; b.vy -= 2 * d * ny;
      b.x = px; b.y = py;
      hitBrick(br, b);
      break;
    }
  }

  // ---------- rendering ----------
  function brickColor(b) {
    if (b.type === 'bomb') return COLORS.bomb;
    if (b.type === 'plus') return COLORS.plus;
    if (b.type === 'slow') return COLORS.slow;
    if (b.type === 'shield') return COLORS.shield;
    return b.hp >= 3 ? COLORS.hp3 : b.hp === 2 ? COLORS.hp2 : COLORS.hp1;
  }

  function sector(b) {
    ctx.beginPath();
    ctx.arc(0, 0, b.r1 * U, b.a0, b.a1);
    ctx.arc(0, 0, Math.max(0, b.r0) * U, b.a1, b.a0, true);
    ctx.closePath();
  }

  function draw(now) {
    ctx.setTransform(DPR, 0, 0, DPR, 0, 0);
    // background
    const bg = ctx.createRadialGradient(CX, CY, 0, CX, CY, Math.max(W, H) * 0.7);
    bg.addColorStop(0, '#2a1a58'); bg.addColorStop(1, '#150d2b');
    ctx.fillStyle = bg; ctx.fillRect(0, 0, W, H);
    drawStars(now);
    if (!g) return;

    let sx = 0, sy = 0;
    if (g.shake > 0) { sx = (rnd() - 0.5) * 10 * g.shake; sy = (rnd() - 0.5) * 10 * g.shake; }
    ctx.translate(CX + sx, CY + sy);

    // arena rim
    ctx.beginPath(); ctx.arc(0, 0, ARENA * U, 0, TAU);
    ctx.strokeStyle = 'rgba(169,155,208,.35)'; ctx.lineWidth = 2; ctx.stroke();

    // danger ring
    const pulse = 0.5 + 0.5 * Math.sin(now / 120);
    ctx.beginPath(); ctx.arc(0, 0, DANGER * U, 0, TAU);
    ctx.setLineDash([4, 6]);
    ctx.strokeStyle = g.flashDanger > 0 ? `rgba(255,77,109,${0.35 + 0.65 * g.flashDanger * pulse})` : 'rgba(255,77,109,.28)';
    ctx.lineWidth = g.flashDanger > 0 ? 2.5 : 1.5; ctx.stroke(); ctx.setLineDash([]);

    // aim lines from orbiting balls
    for (const b of g.balls) {
      if (b.mode !== 'orbit') continue;
      const c = Math.cos(b.a), s = Math.sin(b.a);
      const grad = ctx.createLinearGradient(c * ORBIT * U, s * ORBIT * U, c * ARENA * U, s * ARENA * U);
      grad.addColorStop(0, 'rgba(255,255,255,.35)'); grad.addColorStop(1, 'rgba(255,255,255,0)');
      ctx.strokeStyle = grad; ctx.lineWidth = 2; ctx.setLineDash([3, 7]);
      ctx.beginPath(); ctx.moveTo(c * (ORBIT + 8) * U, s * (ORBIT + 8) * U); ctx.lineTo(c * ARENA * U, s * ARENA * U); ctx.stroke();
      ctx.setLineDash([]);
    }

    // bricks
    for (const b of g.bricks) {
      sector(b);
      const col = brickColor(b);
      ctx.fillStyle = b.flash > 0 ? '#ffffff' : col;
      ctx.fill();
      ctx.strokeStyle = 'rgba(21,13,43,.55)'; ctx.lineWidth = 1.5; ctx.stroke();
      const mid = brickMid(b), rm = ((b.r0 + b.r1) / 2) * U;
      const x = Math.cos(mid) * rm, y = Math.sin(mid) * rm;
      ctx.fillStyle = '#1d1238';
      ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
      if (b.type === 'n' && b.hp >= 2) { ctx.font = `800 ${Math.round(10 * U + 2)}px Outfit, system-ui`; ctx.fillText(String(b.hp), x, y + 1); }
      else if (b.type === 'plus') { ctx.font = `800 ${Math.round(9 * U + 2)}px Outfit, system-ui`; ctx.fillText('+1', x, y + 1); }
      else if (b.type === 'bomb') drawBurst(x, y, 5 * U + 1);
      else if (b.type === 'slow') drawFlake(x, y, 5 * U + 1);
      else if (b.type === 'shield') {
        ctx.save(); sector(b); ctx.clip();
        ctx.strokeStyle = 'rgba(255,255,255,.18)'; ctx.lineWidth = 2;
        for (let k = -3; k <= 3; k++) { ctx.beginPath(); ctx.moveTo(x - 20 + k * 8, y - 20); ctx.lineTo(x + 20 + k * 8, y + 20); ctx.stroke(); }
        ctx.restore();
      }
    }

    // orbit path + core
    ctx.beginPath(); ctx.arc(0, 0, ORBIT * U, 0, TAU);
    ctx.strokeStyle = 'rgba(255,178,63,.25)'; ctx.lineWidth = 1.5; ctx.stroke();
    const cr = (ORBIT * 0.5 + Math.sin(now / 300) * 1.2) * U;
    const glow = ctx.createRadialGradient(0, 0, 0, 0, 0, cr * 2.4);
    glow.addColorStop(0, 'rgba(255,178,63,.55)'); glow.addColorStop(1, 'rgba(255,178,63,0)');
    ctx.fillStyle = glow; ctx.beginPath(); ctx.arc(0, 0, cr * 2.4, 0, TAU); ctx.fill();
    ctx.fillStyle = COLORS.core; ctx.beginPath(); ctx.arc(0, 0, cr, 0, TAU); ctx.fill();
    ctx.fillStyle = '#fff3d6'; ctx.beginPath(); ctx.arc(-cr * 0.3, -cr * 0.3, cr * 0.35, 0, TAU); ctx.fill();

    // particles
    for (const p of g.parts) {
      ctx.globalAlpha = Math.max(0, 1 - p.t / p.life);
      ctx.fillStyle = p.col;
      ctx.fillRect(p.x * U - p.size / 2, p.y * U - p.size / 2, p.size, p.size);
    }
    ctx.globalAlpha = 1;

    // balls
    for (const b of g.balls) {
      let x, y;
      if (b.mode === 'orbit') { x = Math.cos(b.a) * ORBIT; y = Math.sin(b.a) * ORBIT; }
      else {
        x = b.x; y = b.y;
        b.trail.forEach(([tx, ty], i) => {
          ctx.globalAlpha = (i / b.trail.length) * 0.4;
          ctx.fillStyle = b.t > GHOST_AFTER ? COLORS.muted : '#ffffff';
          ctx.beginPath(); ctx.arc(tx * U, ty * U, BALL * U * (0.4 + (0.6 * i) / b.trail.length), 0, TAU); ctx.fill();
        });
        ctx.globalAlpha = 1;
      }
      ctx.shadowColor = '#ffffff'; ctx.shadowBlur = 12;
      ctx.fillStyle = b.mode === 'fly' && b.t > GHOST_AFTER ? COLORS.muted : COLORS.ball;
      ctx.beginPath(); ctx.arc(x * U, y * U, BALL * U, 0, TAU); ctx.fill();
      ctx.shadowBlur = 0;
    }

    // score pop-ups
    for (const p of g.pops) {
      const k = p.t / 0.9;
      ctx.globalAlpha = 1 - k * k;
      ctx.fillStyle = p.col;
      ctx.font = `800 ${Math.round(14 * p.scale)}px Outfit, system-ui`;
      ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
      ctx.fillText(p.text, p.x * U, p.y * U - k * 26);
    }
    ctx.globalAlpha = 1;

    if (state === 'play' && g.t < 4 && g.broken === 0) {
      ctx.fillStyle = `rgba(244,239,255,${0.6 + 0.4 * Math.sin(now / 250)})`;
      ctx.font = '600 16px Outfit, system-ui'; ctx.textAlign = 'center';
      ctx.fillText('Touche l’écran pour lâcher la bille', 0, (ARENA + 26) * U);
    }
    if (g.t < g.slowUntil && state === 'play') {
      ctx.fillStyle = COLORS.slow; ctx.font = '600 14px Outfit, system-ui'; ctx.textAlign = 'center';
      ctx.fillText(`Ralenti ${Math.ceil(g.slowUntil - g.t)} s`, 0, (ARENA + 24) * U);
    }
  }

  function drawBurst(x, y, r) {
    ctx.strokeStyle = '#1d1238'; ctx.lineWidth = 2; ctx.beginPath();
    for (let k = 0; k < 4; k++) { const a = (k * Math.PI) / 4; ctx.moveTo(x - Math.cos(a) * r, y - Math.sin(a) * r); ctx.lineTo(x + Math.cos(a) * r, y + Math.sin(a) * r); }
    ctx.stroke();
  }
  function drawFlake(x, y, r) {
    ctx.strokeStyle = '#1d1238'; ctx.lineWidth = 1.8; ctx.beginPath();
    for (let k = 0; k < 3; k++) { const a = (k * Math.PI) / 3 + Math.PI / 2; ctx.moveTo(x - Math.cos(a) * r, y - Math.sin(a) * r); ctx.lineTo(x + Math.cos(a) * r, y + Math.sin(a) * r); }
    ctx.stroke();
  }

  const stars = Array.from({ length: 70 }, () => ({ x: rnd(), y: rnd(), s: rnd() * 1.4 + 0.3, p: rnd() * TAU }));
  function drawStars(now) {
    for (const s of stars) {
      ctx.globalAlpha = 0.25 + 0.35 * (0.5 + 0.5 * Math.sin(now / 900 + s.p));
      ctx.fillStyle = '#ffffff';
      ctx.fillRect(s.x * W, s.y * H, s.s, s.s);
    }
    ctx.globalAlpha = 1;
  }

  // ---------- flow ----------
  function updateHud() {
    if (!g) return;
    $('score').textContent = g.score;
    $('wave').textContent = Math.max(1, g.wave);
    $('best').textContent = Math.max(best, g.score);
  }

  function start() {
    newGame();
    state = 'play';
    $('menu').hidden = true; $('over').hidden = true; $('paused').hidden = true; $('hud').hidden = false;
  }

  function gameOver() {
    state = 'over';
    sfx.over(); buzz([60, 40, 90]);
    const isBest = g.score > best;
    if (isBest) { best = g.score; store.set('best', best); }
    $('over-score').textContent = g.score;
    $('over-newbest').hidden = !isBest;
    $('over-detail').textContent = `Vague ${Math.max(1, g.wave)} · ${g.broken} briques · meilleur combo x${Math.max(1, g.bestCombo)}`;
    $('btn-continue').hidden = g.continued;
    setTimeout(() => { if (state === 'over') $('over').hidden = false; }, 600);
    updateHud();
  }

  function continueGame() {
    g.continued = true;
    for (const b of g.bricks) { b.r0 += 70; b.r1 += 70; }
    g.bricks = g.bricks.filter((b) => b.r1 <= ARENA - 2);
    for (const b of g.balls) if (b.mode === 'fly') { b.mode = 'orbit'; b.a = Math.atan2(b.y, b.x); }
    state = 'play';
    $('over').hidden = true;
  }

  function pause(on) {
    if (on && state === 'play') { state = 'pause'; $('paused').hidden = false; }
    else if (!on && state === 'pause') { state = 'play'; $('paused').hidden = true; last = performance.now(); }
  }

  function frame(now) {
    const dt = Math.min(0.033, (now - last) / 1000);
    last = now;
    if (state === 'play') step(dt);
    else if (g) { for (const p of g.parts) p.t += dt; }
    draw(now);
    requestAnimationFrame(frame);
  }

  // ---------- input ----------
  cv.addEventListener('pointerdown', (e) => {
    e.preventDefault();
    if (ac && ac.state === 'suspended') ac.resume();
    if (state === 'play') launch();
  });
  window.addEventListener('keydown', (e) => {
    if (e.code === 'Space' || e.code === 'Enter') {
      if (state === 'play') { e.preventDefault(); launch(); }
    } else if (e.code === 'Escape' || e.code === 'KeyP') pause(state === 'play');
  });
  document.addEventListener('visibilitychange', () => { if (document.hidden) pause(true); });

  $('btn-play').onclick = start;
  $('btn-retry').onclick = start;
  $('btn-continue').onclick = continueGame;
  $('btn-pause').onclick = () => pause(true);
  $('btn-resume').onclick = () => pause(false);
  $('btn-quit').onclick = () => { state = 'menu'; g = null; $('paused').hidden = true; $('hud').hidden = true; showMenu(); };
  const soundLabel = () => ($('btn-sound').textContent = `Son : ${sound ? 'activé' : 'coupé'}`);
  $('btn-sound').onclick = () => { sound = !sound; store.set('sound', sound); soundLabel(); };
  soundLabel();

  function showMenu() {
    $('menu').hidden = false;
    $('menu-best').textContent = best ? `Ton record : ${best}` : '';
  }

  if (location.hash === '#debug') {
    window.OrbiteDebug = { game: () => g, state: () => state, launch, start, step: (dt) => step(dt), setSlow: (on) => { g.slowUntil = on ? g.t + 999 : 0; } };
  }

  showMenu();
  requestAnimationFrame(frame);
})();
