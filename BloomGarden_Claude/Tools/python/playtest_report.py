#!/usr/bin/env python3
"""Bloom Garden playtest report (P6-05): turns the logs sent by testers into tables of wins, quits and frictions.

Each tester copies their data from the game (Options > Playtest data > Copy) and sends it; save each one as a
text file (one per tester, any name) in a folder, then run:

    python Tools/python/playtest_report.py Docs/P6/donnees > Docs/P6/rapport.md

Only real data is reported: no file -> no table. Five testers are enough to find problems, not to draw
statistics (GDD chapter 16): the report gives counts and medians, never percentages on tiny groups.
"""
import json, statistics, sys
from collections import defaultdict
from pathlib import Path

LEVEL_COUNT = 20


def read_events(path):
    events = []
    for line in Path(path).read_text(encoding='utf-8', errors='replace').splitlines():
        line = line.strip()
        if not line.startswith('{'):
            continue  # messaging apps may add text around the pasted data
        try:
            events.append(json.loads(line))
        except json.JSONDecodeError:
            pass
    return events


def median(values, scale=1.0, fmt='{:.0f}'):
    return fmt.format(statistics.median(values) / scale) if values else '-'


def build_report(folder):
    files = sorted(p for p in Path(folder).iterdir() if p.is_file() and p.suffix.lower() in ('.txt', '.jsonl', '.json'))
    testers = {p.stem: read_events(p) for p in files}
    testers = {k: v for k, v in testers.items() if v}
    out = ['# Rapport de playtest Bloom Garden', '']
    if not testers:
        out.append('Aucune donnée de testeur trouvée dans `%s`. Rien à rapporter.' % folder)
        return '\n'.join(out) + '\n'

    sessions = {e.get('sid') for evs in testers.values() for e in evs}
    out += [f'Sources : **{len(testers)} testeur(s)**, {len(sessions)} session(s) de jeu, '
            f'{sum(len(v) for v in testers.values())} événements. Petit groupe : chiffres à lire comme des signaux, pas des statistiques.', '']

    # ---- per level
    lv = defaultdict(lambda: {'who_started': set(), 'who_won': set(), 'starts': 0, 'wins': 0, 'abandons': 0,
                              'deadlocks': 0, 'undos': 0, 'restarts': 0, 'win_ms': [], 'win_steps': [], 'win_undos': []})
    first_tap = {}
    hints = defaultdict(int)
    settings = defaultdict(int)
    abandon_reasons = defaultdict(int)
    for tester, evs in testers.items():
        level1_start = None
        for e in evs:
            name, level = e.get('e'), e.get('level_id')
            if name == 'settings_changed':
                settings[f"{e.get('setting_name')}={e.get('value')}"] += 1
            if not level:
                continue
            s = lv[level]
            if name == 'level_started':
                s['starts'] += 1
                s['who_started'].add(tester)
                if level == 'BG-001' and level1_start is None:
                    level1_start = e
            elif name == 'can_selected' and level == 'BG-001' and tester not in first_tap and level1_start is not None:
                first_tap[tester] = e.get('ms', 0)
            elif name == 'level_completed':
                s['wins'] += 1
                s['who_won'].add(tester)
                s['win_ms'].append(e.get('duration_ms', 0))
                s['win_steps'].append(e.get('steps', 0))
                s['win_undos'].append(e.get('undos', 0))
            elif name == 'level_abandoned':
                s['abandons'] += 1
                abandon_reasons[e.get('reason', '?')] += 1
            elif name == 'deadlock_detected':
                s['deadlocks'] += 1
            elif name == 'undo_used':
                s['undos'] += 1
            elif name == 'restart_used':
                s['restarts'] += 1
            elif name == 'tutorial_hint_shown':
                hints[(level, e.get('hint'))] += 1

    out += ['## Par niveau', '',
            '| Niveau | Testeurs ayant commencé | Testeurs ayant gagné | Parties | Victoires | Abandons | Blocages | Annulations | Recommencer | Temps médian (victoire) | Coups médians | Annulations médianes |',
            '|---|---|---|---|---|---|---|---|---|---|---|---|']
    for level in sorted(lv):
        s = lv[level]
        out.append(f"| {level} | {len(s['who_started'])} | {len(s['who_won'])} | {s['starts']} | {s['wins']} | {s['abandons']} | "
                   f"{s['deadlocks']} | {s['undos']} | {s['restarts']} | {median(s['win_ms'], 1000, '{:.0f} s')} | "
                   f"{median(s['win_steps'])} | {median(s['win_undos'])} |")
    out.append('')

    # ---- where testers stopped
    out += ['## Où chaque testeur s\'est arrêté', '', '| Testeur | Dernier niveau gagné | Niveaux gagnés | Abandons | Temps avant le 1er tap (niveau 1) |', '|---|---|---|---|---|']
    for tester, evs in testers.items():
        won = sorted({e['level_id'] for e in evs if e.get('e') == 'level_completed'})
        quits = sum(1 for e in evs if e.get('e') == 'level_abandoned')
        tap = f"{first_tap[tester] / 1000:.1f} s" if tester in first_tap else '-'
        out.append(f"| {tester} | {won[-1] if won else '-'} | {len(won)} | {quits} | {tap} |")
    out.append('')

    # ---- signals
    out += ['## Signaux à examiner (à confirmer avec les observations écrites)', '']
    signals = []
    for level in sorted(lv):
        s = lv[level]
        stuck = s['who_started'] - s['who_won']
        if stuck:
            signals.append(f"- {level} : {len(stuck)} testeur(s) l'ont commencé sans le gagner ({', '.join(sorted(stuck))}).")
        if s['deadlocks']:
            signals.append(f"- {level} : {s['deadlocks']} blocage(s).")
        if s['restarts'] >= 2:
            signals.append(f"- {level} : « Recommencer » utilisé {s['restarts']} fois.")
    if first_tap:
        slow = [t for t, ms in first_tap.items() if ms > 15000]
        if slow:
            signals.append(f"- Premier tap du niveau 1 après plus de 15 s pour : {', '.join(slow)} (tutoriel à revoir ?).")
    out += signals or ['- Aucun signal automatique ; se fier aux observations.']
    out.append('')
    if abandon_reasons:
        out += ['Raisons d\'abandon : ' + ', '.join(f'{k} = {v}' for k, v in sorted(abandon_reasons.items())), '']
    if settings:
        out += ['Réglages modifiés : ' + ', '.join(f'{k} ({v})' for k, v in sorted(settings.items())), '']
    return '\n'.join(out) + '\n'


if __name__ == '__main__':
    if len(sys.argv) != 2:
        sys.exit('usage: playtest_report.py <folder of tester files>')
    sys.stdout.reconfigure(encoding='utf-8')
    print(build_report(sys.argv[1]), end='')
