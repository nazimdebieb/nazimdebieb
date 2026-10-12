#!/usr/bin/env python3
"""Bloom Garden deterministic reference solver, stdlib only.

Rules 0.1.0: 3 queues, 3 active slots, color-specific watering, exact charge
per bud, visible top bud only, deterministic target row/col ascending.
This is a planning oracle, NOT an implementation to embed in Unity.

Usage:
    python tools/solver_reference.py levels/*.json
    python tools/solver_reference.py --generate 20 --output levels
"""
import argparse, json, random, sys
from collections import Counter, deque
from pathlib import Path

COLORS=('coral','blue','yellow','purple')
RULES='0.1.0'

class LevelError(ValueError): pass


def validate(level):
    if level.get('rulesVersion') != RULES: raise LevelError('Unsupported rulesVersion')
    if level.get('slotCount') != 3: raise LevelError('MVP requires 3 slots')
    w,h=level['grid']['width'],level['grid']['height']
    if not(1 <= w <= 7 and 1 <= h <= 7):raise LevelError('Invalid grid')
    cells=level['grid']['cells']
    if len(cells)!=w*h:raise LevelError('Wrong number of grid cells')
    if len(level.get('lanes',[]))!=3: raise LevelError('Three lanes required')
    buds=Counter(); charge=Counter(); ids=set()
    for cell in cells:
        if not isinstance(cell,list) or len(cell)>3: raise LevelError('Each cell is a list of up to 3 layers')
        for c in cell:
            if c not in COLORS:raise LevelError('Unknown flower color')
            buds[c]+=1
    for lane in level['lanes']:
        for can in lane:
            if not isinstance(can.get('capacity'),int) or can['capacity']<1:raise LevelError('Invalid capacity')
            if can.get('color') not in COLORS:raise LevelError('Invalid watering can color')
            if can['id'] in ids:raise LevelError('Duplicate can ID')
            ids.add(can['id']); charge[can['color']]+=can['capacity']
    if sum(buds.values())==0:raise LevelError('Empty level')
    if buds != charge:raise LevelError(f'Unequal charge to flowers: buds={buds}, charge={charge}')
    return True


def can_mapping(level):
    return {x['id']:(x['color'],x['capacity']) for lane in level['lanes'] for x in lane}


def initial(level):
    # state = (opened count per cell, current index in 3 lanes, ordered active (ID, water_left))
    return (tuple(0 for _ in level['grid']['cells']), (0,0,0), ())


def finished(state,level):
    opened, _, _=state
    return all(k==len(stack) for k,stack in zip(opened,level['grid']['cells']))


def stabilize(state,level,by_id):
    opened, lane_idx, slots=state
    opened=list(opened); slots=list(slots)
    while True:
        progress=False
        for s,(can_id,remaining) in enumerate(slots):
            color,_=by_id[can_id]
            dest=next((idx for idx,stack in enumerate(level['grid']['cells'])
                       if opened[idx]<len(stack) and stack[opened[idx]]==color),None)
            if dest is None: continue
            opened[dest]+=1; remaining-=1
            if remaining==0: slots.pop(s)
            else:slots[s]=(can_id,remaining)
            progress=True; break
        if not progress:break
    return (tuple(opened),lane_idx,tuple(slots))


def move(state, lane, level, by_id):
    opened, lane_idx, slots=state
    if len(slots)>=level['slotCount']:return None
    if not 0 <= lane < 3:return None
    if lane_idx[lane]>=len(level['lanes'][lane]):return None
    can=level['lanes'][lane][lane_idx[lane]]
    counts=list(lane_idx); counts[lane]+=1
    new=(opened,tuple(counts),slots+((can['id'],can['capacity']),))
    return stabilize(new,level,by_id)


def solve(level,max_states=150000):
    validate(level); by_id=can_mapping(level)
    start=initial(level)
    fringe=deque([start]); parents={start:(None,None)}; explored=0
    while fringe:
        state=fringe.popleft(); explored+=1
        if finished(state,level):
            path=[]; p=state
            while parents[p][0] is not None:
                prev,action=parents[p];path.append(action);p=prev
            path.reverse()
            return {'status':'solvable','solutionLanes':path,'statesExplored':explored,
                    'shortestMoves':len(path)}
        if explored>=max_states:return {'status':'unknown','statesExplored':explored}
        for lane in range(3):
            next_state=move(state,lane,level,by_id)
            if next_state is not None and next_state not in parents:
                parents[next_state]=(state,lane)
                fringe.append(next_state)
    return {'status':'unsolvable','statesExplored':explored}


def create_level(seed,index):
    r=random.Random(seed)
    if index<=3: cols=2
    elif index<=10:cols=3
    else: cols=4
    colors=COLORS[:cols]
    w=3 if index<=9 else 4
    h=3 if index<=14 else 4
    cells=[[] for _ in range(w*h)]
    # 6..20 buds, keep 1..2/3 layers per cell; counts by color >=1.
    total=min(7+index//2,19)
    pool=list(colors)+[r.choice(colors) for _ in range(total-len(colors))]
    r.shuffle(pool)
    for color in pool:
        choices=[j for j,c in enumerate(cells) if len(c)<(1 if index<=3 else 2 if index<=12 else 3)]
        cells[r.choice(choices)].append(color)
    # Deliberately random but resources exactly cover all flowers.
    by_color=Counter(pool)
    cans=[]
    for color,num in by_color.items():
        while num>0:
            cap=r.randint(1,min(3,num))
            cans.append({'id':f'{color[0]}{len(cans):02d}','color':color,'capacity':cap})
            num-=cap
    r.shuffle(cans)
    lanes=[[],[],[]]
    for k,can in enumerate(cans):lanes[r.randrange(3)].append(can)
    # Avoid empty initial lanes in larger levels: not required, but improves initial choice.
    if index>5:
        for j in range(3):
            if not lanes[j]:
                richest=max(range(3),key=lambda x:len(lanes[x]))
                if len(lanes[richest])>1:lanes[j].append(lanes[richest].pop())
    return {'id':f'BG-{index:03d}', 'rulesVersion':RULES,'slotCount':3,
            'title':f'Jardin {index:02d}', 'grid':{'width':w,'height':h,'cells':cells},
            'lanes':lanes,'theme':'meadow' if index<=10 else 'pond' if index<=15 else 'zen'}


def generate(n,output):
    p=Path(output);p.mkdir(parents=True,exist_ok=True)
    summary=[]
    for i in range(1,n+1):
        selected=None
        for attempt in range(1500):
            obj=create_level(71230+i*10000+attempt,i)
            result=solve(obj,max_states=50000)
            if result['status']=='solvable' and result['shortestMoves'] >= (2 if i<=4 else 3):
                selected=obj; break
        if selected is None:raise RuntimeError(f'Could not find candidate for level {i}')
        selected['referenceSolution']=result['solutionLanes']
        selected['analysis']={k:v for k,v in result.items() if k not in ('solutionLanes',)}
        (p/f'BG-{i:03d}.json').write_text(json.dumps(selected,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
        summary.append({'id':selected['id'],**result})
    (p/'REFERENCE_SOLUTIONS.json').write_text(json.dumps(summary,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    return summary


def main():
    ap=argparse.ArgumentParser();ap.add_argument('files',nargs='*');ap.add_argument('--generate',type=int)
    ap.add_argument('--output',default='levels');ap.add_argument('--max-states',type=int,default=150000)
    args=ap.parse_args()
    if args.generate:
        records=generate(args.generate,args.output)
        print(f'Generated {len(records)} verified solvable levels in {args.output}')
        print('States explored min/max:',min(x['statesExplored'] for x in records),max(x['statesExplored'] for x in records))
    for name in args.files:
        lv=json.loads(Path(name).read_text(encoding='utf-8'))
        ans=solve(lv,max_states=args.max_states)
        print(lv['id'],ans)
        if ans['status']!='solvable':sys.exit(2)
if __name__=='__main__': main()
