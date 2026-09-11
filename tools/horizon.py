"""Где DFS полезен, а где вреден: скан по горизонту планирования."""
import sys, random, statistics
sys.path.insert(0,'/home/user/AIME/tools')
from dfs_sim import evaluate
from gen_profiles import make_cohort
from sensitivity import ground_truth, spearman

cohort=make_cohort(24); RUNS=200
print(f"{'горизонт':>9} | {'ρ скоринг':>10} {'ρ DFS':>8} | {'скоринг':>8} {'DFS':>8} {'выигрыш':>9}")
print("-"*62)
for H in [6,12,24,36,60,120]:
    for c in cohort: c['loss']=evaluate(c['kk'],c['tm'],H=H)['loss']
    facts={c['id']:[] for c in cohort}
    for r in range(RUNS):
        rng=random.Random(77+r)
        for c in cohort:
            facts[c['id']].append(ground_truth(c['kk'],c['tm'],H,rng,0.25,0.010,0.05))
    for c in cohort: c['fact']=statistics.mean(facts[c['id']])
    f=[c['fact'] for c in cohort]
    rs=spearman([c['static'] for c in cohort],f); rd=spearman([-c['loss'] for c in cohort],f)
    vs=statistics.mean(c['fact'] for c in sorted(cohort,key=lambda c:-c['static'])[:5])
    vd=statistics.mean(c['fact'] for c in sorted(cohort,key=lambda c:c['loss'])[:5])
    print(f"{H:>7} м | {rs:>+10.3f} {rd:>+8.3f} | {vs:>8.2f} {vd:>8.2f} {(vd/vs-1)*100:>+8.1f}%")
