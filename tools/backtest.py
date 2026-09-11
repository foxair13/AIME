"""
Честная проверка: предсказывает ли DFS фактическую пользу лучше классического скоринга?

ВАЖНО про методологию: "правда" (факт) генерируется НЕ моделью DFS, а отдельным
процессом с эффектами, которых DFS не знает:
  - выгорание при высоком перерегулировании,
  - риск ухода (чем меньше вовлечённость, тем выше),
  - случайный шум среды,
  - нелинейный потолок роли.
Иначе тест был бы круговым.
"""
import sys, random, math, statistics
sys.path.insert(0,'/home/user/AIME/tools')
from dfs_sim import evaluate
from gen_profiles import make_cohort

random.seed(1)

def ground_truth(kk, tm, H=36.0, rng=random):
    """Фактическая накопленная польза за H мес. Дискретная помесячная симуляция."""
    STEP=0.25
    n=int(H/STEP); I=y1=y2=0.0
    total=0.0; burn=0.0; alive=True; left_at=None
    for i in range(n):
        t=i*STEP
        y=y2
        e=1.0-y
        I+=kk*e*STEP
        u=tm*e+I
        y1+=(u-y1)/0.5*STEP
        y2+=(y1-y2)/15.0*STEP
        # --- эффекты, невидимые для DFS ---
        if y>1.15: burn += (y-1.15)*STEP          # перенапряжение -> выгорание
        eff = y*(1.0-min(burn*0.25,0.6))          # выгорание срезает отдачу
        eff *= max(0.0, rng.gauss(1.0,0.05))      # шум среды
        eff = min(eff, 1.35)                      # потолок роли
        if alive:
            # риск ухода: растёт если долго ниже плана ИЛИ сильно выгорел
            hz = 0.004 + (0.012 if y<0.6 else 0.0) + burn*0.01
            if rng.random() < hz*STEP*4: alive=False; left_at=t
        if not alive: eff = 0.0                   # ушёл -> нули до конца
        total += eff*STEP
    return total, left_at

cohort = make_cohort(24)
H=36.0
RUNS=400
facts={c['id']:[] for c in cohort}
quits={c['id']:0 for c in cohort}
for r in range(RUNS):
    rng=random.Random(1000+r)
    for c in cohort:
        v,left=ground_truth(c['kk'],c['tm'],H,rng)
        facts[c['id']].append(v)
        if left is not None: quits[c['id']]+=1

for c in cohort:
    c['fact']=statistics.mean(facts[c['id']])
    c['quit']=quits[c['id']]/RUNS
    e=evaluate(c['kk'],c['tm'],H=H)
    c['loss']=e['loss']; c['ttc']=e['ttc']; c['ov']=e['ov']

def spearman(xs,ys):
    def rank(v):
        s=sorted(range(len(v)),key=lambda i:v[i]); r=[0]*len(v)
        for pos,i in enumerate(s): r[i]=pos
        return r
    rx,ry=rank(xs),rank(ys); n=len(xs)
    d2=sum((rx[i]-ry[i])**2 for i in range(n))
    return 1-6*d2/(n*(n*n-1))

fact=[c['fact'] for c in cohort]
print("=== Корреляция предсказания с ФАКТОМ (Spearman, n=24, 400 прогонов) ===")
print(f" классический скоринг : {spearman([c['static'] for c in cohort],fact):+.3f}")
print(f" DFS (−потери)        : {spearman([-c['loss'] for c in cohort],fact):+.3f}")
print(f" только компетентность: {spearman([c['kk'] for c in cohort],fact):+.3f}")

def topk_value(key,k=5,rev=False):
    s=sorted(cohort,key=lambda c:c[key],reverse=rev)[:k]
    return statistics.mean(c['fact'] for c in s), statistics.mean(c['quit'] for c in s)

vs,qs=topk_value('static',5,rev=True)
vd,qd=topk_value('loss',5,rev=False)
vo,_=topk_value('fact',5,rev=True)
base=statistics.mean(fact)
print(f"\n=== Средняя ФАКТИЧЕСКАЯ польза нанятых топ-5 (эфф.-мес за 36 мес) ===")
print(f" случайный найм       : {base:6.2f}")
print(f" по скорингу          : {vs:6.2f}   (уход {qs*100:.0f}%)")
print(f" по DFS               : {vd:6.2f}   (уход {qd*100:.0f}%)")
print(f" идеал (знали заранее): {vo:6.2f}")
print(f"\n DFS против скоринга  : {(vd/vs-1)*100:+.1f}%")
print(f" DFS против случайного: {(vd/base-1)*100:+.1f}%")
print(f" доля пути к идеалу   : {(vd-vs)/(vo-vs)*100:5.1f}%" if vo>vs else "")
