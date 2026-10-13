#!/usr/bin/env python3
"""Generate the solver/analysis oracle used by the C# solver tests (P2).

For every level, with the reference rules of Kit/tools/solver_reference.py:
- solve(): status, solutionLanes, statesExplored, shortestMoves (BFS, lanes tried 0..2);
- exhaustive exploration of the reachable state graph:
    reachableStates  number of distinct positions reachable from the start
    winStates        finished positions
    deadlockStates   non-finished positions with no legal move
    decisionStates   non-terminal positions with at least 2 legal moves
    losingMoves      legal (position, lane) moves from a position that can still win
                     to a position that cannot

Usage (from BloomGarden_Claude/):
    python Tools/python/generate_solver_fixture.py Kit/tools Assets/_BloomGarden/Levels \
        Assets/_BloomGarden/Tests/EditMode/Fixtures/python_solver_reference.json
"""
import json, sys
from collections import deque
from pathlib import Path


def analyze(s, level):
    by_id = s.can_mapping(level)
    start = s.initial(level)
    succ = {}
    queue = deque([start])
    succ[start] = None
    while queue:
        x = queue.popleft()
        moves = []
        for lane in range(3):
            y = s.move(x, lane, level, by_id)
            if y is None:
                continue
            moves.append(y)
            if y not in succ:
                succ[y] = None
                queue.append(y)
        succ[x] = moves

    wins = {x for x in succ if s.finished(x, level)}
    # Backward propagation of "can still win".
    preds = {x: [] for x in succ}
    for x, moves in succ.items():
        for y in moves:
            preds[y].append(x)
    winnable = set(wins)
    queue = deque(wins)
    while queue:
        y = queue.popleft()
        for x in preds[y]:
            if x not in winnable:
                winnable.add(x)
                queue.append(x)

    deadlocks = sum(1 for x, m in succ.items() if x not in wins and not m)
    decisions = sum(1 for x, m in succ.items() if x not in wins and len(m) >= 2)
    losing = sum(1 for x, m in succ.items() if x in winnable for y in m if y not in winnable)
    return {
        'reachableStates': len(succ),
        'winStates': len(wins),
        'deadlockStates': deadlocks,
        'decisionStates': decisions,
        'losingMoves': losing,
    }


def main():
    tools_dir, levels_dir, output = sys.argv[1:4]
    sys.path.insert(0, str(Path(tools_dir).resolve()))
    import solver_reference as s

    records = []
    for path in sorted(Path(levels_dir).glob('BG-*.json')):
        level = json.loads(path.read_text(encoding='utf-8'))
        s.validate(level)
        solved = s.solve(level, max_states=150000)
        record = {
            'level': level['id'],
            'status': solved['status'],
            'solutionLanes': solved.get('solutionLanes', []),
            'statesExplored': solved['statesExplored'],
            'shortestMoves': solved.get('shortestMoves', -1),
        }
        record.update(analyze(s, level))
        records.append(record)

    Path(output).write_text(json.dumps({'rulesVersion': s.RULES, 'levels': records}, indent=1) + '\n', encoding='utf-8')
    for r in records:
        print(r['level'], r['status'], 'moves', r['shortestMoves'], 'explored', r['statesExplored'],
              'reachable', r['reachableStates'], 'deadlocks', r['deadlockStates'],
              'decisions', r['decisionStates'], 'losingMoves', r['losingMoves'])


if __name__ == '__main__':
    main()
