#!/usr/bin/env python3
"""Exhaustive count of reachable / deadlock / win states per level (reference rules).
Usage: python Tools/python/count_deadends.py Kit/tools Assets/_BloomGarden/Levels"""
import json,sys
from pathlib import Path
sys.path.insert(0,sys.argv[1]); import solver_reference as s
for p in sorted(Path(sys.argv[2]).glob('BG-*.json')):
    lv=json.loads(p.read_text()); by=s.can_mapping(lv); st=s.initial(lv)
    seen={st}; stack=[st]; dead=0; won=0
    while stack:
        x=stack.pop(); nx=[s.move(x,l,lv,by) for l in range(3)]; nx=[y for y in nx if y is not None]
        if s.finished(x,lv): won+=1
        elif not nx: dead+=1
        for y in nx:
            if y not in seen: seen.add(y); stack.append(y)
    print(lv['id'],'states',len(seen),'deadlocks',dead,'wins',won)
