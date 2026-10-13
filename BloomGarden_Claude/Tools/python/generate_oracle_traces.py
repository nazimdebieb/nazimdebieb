#!/usr/bin/env python3
"""Generate state traces from the Python reference solver, used as an oracle by the C# tests.

For every level: the referenceSolution playout plus N seeded random playouts of legal moves.
After each move the full logical state is recorded (bloomed count per cell, lane heads,
active cans oldest first with remaining water, finished/stuck flags).

Usage (from BloomGarden_Claude/):
    python Tools/python/generate_oracle_traces.py Kit/tools Assets/_BloomGarden/Levels \
        Assets/_BloomGarden/Tests/EditMode/Fixtures/python_oracle_traces.json
"""
import json, random, sys
from pathlib import Path

def main():
    tools_dir, levels_dir, output = sys.argv[1:4]
    playouts = int(sys.argv[4]) if len(sys.argv) > 4 else 8
    sys.path.insert(0, str(Path(tools_dir).resolve()))
    import solver_reference as s

    traces = []
    for path in sorted(Path(levels_dir).glob('BG-*.json')):
        level = json.loads(path.read_text(encoding='utf-8'))
        s.validate(level)
        by_id = s.can_mapping(level)
        runs = [('reference', list(level['referenceSolution']))]
        rng = random.Random(f"{level['id']}-oracle")
        for k in range(playouts):
            runs.append((f'random-{k}', None))
        for name, fixed in runs:
            state = s.initial(level)
            steps = []
            i = 0
            while True:
                legal = [l for l in range(3) if s.move(state, l, level, by_id) is not None]
                if s.finished(state, level) or not legal:
                    break
                if fixed is not None:
                    if i >= len(fixed):
                        break
                    lane = fixed[i]
                else:
                    lane = rng.choice(legal)
                state = s.move(state, lane, level, by_id)
                assert state is not None
                i += 1
                opened, heads, slots = state
                finished = s.finished(state, level)
                stuck = (not finished) and all(s.move(state, l, level, by_id) is None for l in range(3))
                steps.append({'lane': lane, 'bloomed': list(opened), 'laneHeads': list(heads),
                              'active': [[cid, rem] for cid, rem in slots],
                              'finished': finished, 'stuck': stuck})
            traces.append({'level': level['id'], 'run': name, 'steps': steps})
    Path(output).write_text(json.dumps({'source': 'Kit/tools/solver_reference.py', 'rulesVersion': s.RULES,
                                        'traces': traces}, separators=(',', ':')) + '\n', encoding='utf-8')
    total = sum(len(t['steps']) for t in traces)
    won = sum(1 for t in traces if t['steps'] and t['steps'][-1]['finished'])
    stuck = sum(1 for t in traces if t['steps'] and t['steps'][-1]['stuck'])
    print(f'{len(traces)} traces, {total} moves, {won} wins, {stuck} stuck -> {output}')

if __name__ == '__main__':
    main()
