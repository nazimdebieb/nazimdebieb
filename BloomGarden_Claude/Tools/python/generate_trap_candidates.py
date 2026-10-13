#!/usr/bin/env python3
"""Generate MVP2 candidate levels WITH real traps (P7-01, decision D-107: kit levels stay unchanged for MVP1).

Uses the generator and rules of Kit/tools/solver_reference.py, then keeps a candidate only if, over the whole
reachable state graph (same analysis as generate_solver_fixture.py):
  - it is solvable, with a shortest solution of at least MIN_MOVES taps;
  - at least MIN_LOSING legal moves turn a winnable position into a lost one (real traps);
  - the start is not a trap-fest: at least MIN_WIN_SHARE of the reachable positions can still win.
Candidates are written to Assets/_BloomGarden/LevelsCandidates/ (NOT in the shipped level catalog) and must be
reviewed by a human before being published (GDD chapter 09).

Usage (from BloomGarden_Claude/):
    python Tools/python/generate_trap_candidates.py Kit/tools Assets/_BloomGarden/LevelsCandidates 30
"""
import json, sys
from pathlib import Path

MIN_MOVES = 7
MIN_LOSING = 2
MIN_WIN_SHARE = 0.35
MAX_ATTEMPTS = 4000


def main():
    tools_dir, out_dir, count = sys.argv[1], Path(sys.argv[2]), int(sys.argv[3])
    sys.path.insert(0, str(Path(tools_dir).resolve()))
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    import solver_reference as s
    from generate_solver_fixture import analyze

    out_dir.mkdir(parents=True, exist_ok=True)
    summary = []
    for n in range(1, count + 1):
        # Difficulty grows with n: reuse the kit generator's later bands (3-4 colors, 2-3 layers, bigger grids).
        band_index = 11 + (n - 1) * 9 // max(1, count - 1)
        chosen = None
        for attempt in range(MAX_ATTEMPTS):
            level = s.create_level(910000 + n * 10007 + attempt, band_index)
            try:
                s.validate(level)
            except s.LevelError:
                continue
            solved = s.solve(level, max_states=60000)
            if solved['status'] != 'solvable' or solved['shortestMoves'] < MIN_MOVES:
                continue
            a = analyze(s, level)
            if a['losingMoves'] < MIN_LOSING:
                continue
            win_share = winnable_share(s, level)
            if win_share < MIN_WIN_SHARE:
                continue
            chosen = (level, solved, a, win_share, attempt)
            break
        if chosen is None:
            print(f'candidate {n}: none found in {MAX_ATTEMPTS} attempts', file=sys.stderr)
            continue
        level, solved, a, win_share, attempt = chosen
        level['id'] = f'BG2-{n:03d}'
        level['title'] = f'Jardin piégé {n:02d}'
        level['theme'] = ('meadow', 'pond', 'zen')[(n - 1) * 3 // count]
        level['referenceSolution'] = solved['solutionLanes']
        level['analysis'] = {'status': 'solvable', 'statesExplored': solved['statesExplored'],
                             'shortestMoves': solved['shortestMoves'], 'reachableStates': a['reachableStates'],
                             'deadlockStates': a['deadlockStates'], 'losingMoves': a['losingMoves'],
                             'winnableShare': round(win_share, 3), 'reviewed': False}
        (out_dir / f"{level['id']}.json").write_text(json.dumps(level, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
        summary.append((level['id'], band_index, attempt, solved['shortestMoves'], a['reachableStates'],
                        a['deadlockStates'], a['losingMoves'], round(win_share, 2)))
        print(level['id'], 'band', band_index, 'moves', solved['shortestMoves'], 'reachable', a['reachableStates'],
              'deadlocks', a['deadlockStates'], 'losingMoves', a['losingMoves'], 'winShare', round(win_share, 2))
    with open(out_dir / 'CANDIDATES.csv', 'w', encoding='utf-8', newline='\n') as f:
        f.write('id,band,attempt,shortestMoves,reachableStates,deadlockStates,losingMoves,winnableShare,reviewed\n')
        for row in summary:
            f.write(','.join(str(x) for x in row) + ',false\n')


def winnable_share(s, level):
    """Share of reachable positions from which a win is still possible."""
    by_id = s.can_mapping(level)
    start = s.initial(level)
    succ, stack = {start: None}, [start]
    while stack:
        x = stack.pop()
        nxt = [y for y in (s.move(x, l, level, by_id) for l in range(3)) if y is not None]
        succ[x] = nxt
        for y in nxt:
            if y not in succ:
                succ[y] = None
                stack.append(y)
    preds = {x: [] for x in succ}
    for x, m in succ.items():
        for y in m:
            preds[y].append(x)
    good = {x for x in succ if s.finished(x, level)}
    frontier = list(good)
    while frontier:
        y = frontier.pop()
        for x in preds[y]:
            if x not in good:
                good.add(x)
                frontier.append(x)
    return len(good) / len(succ)


if __name__ == '__main__':
    main()
