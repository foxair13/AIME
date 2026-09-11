"""
Проверка на подгонку: насколько вывод зависит от МОИХ допущений о выгорании и уходе?
Сканируем силу этих эффектов от 0 (их нет вообще) до сильных.
"""
import sys, random, statistics
sys.path.insert(0,'/home/user/AIME/tools')
from dfs_sim import evaluate
from gen_profiles import make_cohort

def ground_truth(kk,tm,H,rng,burn_k,quit_k,noise):
    STEP=0.25; n=int(H/STEP); I=y1=y2=0.0; total=0.0; burn=0.0; alive=True
    for i in range(n):
        y=y2; e=1.0-y
        I+=kk*e*STEP; u=tm*e+I
        y1+=(u-y1)/0.5*STEP; y2+=(y1-y2)/15.0*STEP
        if y>1.15: burn+=(y-1.15)*STEP
        eff=y*(1.0-min(burn*burn_k,0.6))
        if noise>0: eff*=max(0.0,rng.gauss(1.0,noise))
        eff=min(eff,1.35)
        if alive and quit_k>0:
            hz=0.004+(0.012 if y<0.6 else 0.0)+burn*quit_k
            if rng.random()<hz*STEP*4: alive=False
        if not alive: eff=0.0
        total+=eff*STEP
    return total

def spearman(xs,ys):
    def rank(v):
        s=sorted(range(len(v)),key=lambda i:v[i]); r=[0]*len(v)
        for p,i in enumerate(s): r[i]=p
        return r
    rx,ry=rank(xs),rank(ys); n=len(xs)
    return 1-6*sum((rx[i]-ry[i])**2 for i in range(n))/(n*(n*n-1))

cohort=make_cohort(24); H=36.0; RUNS=200
for c in cohort:
    c['loss']=evaluate(c['kk'],c['tm'],H=H)['loss']

print(f"{'выгорание':>10} {'уход':>7} {'шум':>6} | {'ρ скоринг':>10} {'ρ DFS':>8} | {'польза скор':>11} {'польза DFS':>11} {'выигрыш':>9}")
print("-"*84)
for burn_k,quit_k,noise,label in [
    (0.00,0.000,0.00,"нет эффектов"),
    (0.00,0.004,0.05,"только уход"),
    (0.10,0.000,0.05,"слабое выгор."),
    (0.25,0.010,0.05,"базовый сцен."),
    (0.40,0.020,0.10,"сильное"),
]:
    facts={c['id']:[] for c in cohort}
    for r in range(RUNS):
        rng=random.Random(500+r)
        for c in cohort:
            facts[c['id']].append(ground_truth(c['kk'],c['tm'],H,rng,burn_k,quit_k,noise))
    for c in cohort: c['fact']=statistics.mean(facts[c['id']])
    f=[c['fact'] for c in cohort]
    rs=spearman([c['static'] for c in cohort],f)
    rd=spearman([-c['loss'] for c in cohort],f)
    vs=statistics.mean(c['fact'] for c in sorted(cohort,key=lambda c:-c['static'])[:5])
    vd=statistics.mean(c['fact'] for c in sorted(cohort,key=lambda c:c['loss'])[:5])
    print(f"{burn_k:>10.2f} {quit_k:>7.3f} {noise:>6.2f} | {rs:>+10.3f} {rd:>+8.3f} | {vs:>11.2f} {vd:>11.2f} {(vd/vs-1)*100:>+8.1f}%  {label}")
