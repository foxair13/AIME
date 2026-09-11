# -*- coding: utf-8 -*-
import sys; sys.path.insert(0,'/home/user/AIME/tools')
from dfs_sim import evaluate, rotation_cost
STEP=0.01
def useful(kk,tm,H):
    n=int(H/STEP); I=y1=y2=0.0; tot=0.0
    for i in range(n):
        y=y2; e=1.0-y
        I+=kk*e*STEP; u=tm*e+I
        y1+=(u-y1)/0.5*STEP; y2+=(y1-y2)/15.0*STEP
        tot+=min(y,1.0)*STEP
    return tot

SAL=4000.0  # условная полная стоимость senior/мес для работодателя, USD
print("=== Взгляд работодателя: окупаемость найма (Kк=0.88, Tм=4.0) ===")
print("Допущение: найм окупается, когда накопленная польза покрывает разгон + стоимость подбора (1.5 мес).")
HIRE=1.5
for H in [3,7,9,12,15,24,36]:
    u=useful(0.88,4.0,H)
    net=u-HIRE
    print(f" срок {H:>3}м: польза {u:>5.1f} эфф-мес, минус подбор -> {net:>5.1f}  "
          f"{'УБЫТОК' if net< H*0.5 else 'окупился'}  (${net*SAL:>9,.0f} чист. ценности)")

print("\n=== Что даёт удержание: цена ухода на 8-м месяце вместо 36-го ===")
short=useful(0.88,4.0,8)-HIRE; long=useful(0.88,4.0,36)-HIRE
print(f" ушёл в 8 мес : {short:5.1f} эфф-мес  (${short*SAL:>9,.0f})")
print(f" ушёл в 36 мес: {long:5.1f} эфф-мес  (${long*SAL:>9,.0f})")
print(f" разница      : {long-short:5.1f} эфф-мес  (${(long-short)*SAL:>9,.0f}) — вот что теряет работодатель")

print("\n=== DFS-профиль для роли, где разгон не нужен (консалтинг/аудит, H=3-6 мес) ===")
for name,(kk,tm) in {"эксперт-консультант":(0.88,4.0),"обычный найм в штат":(0.88,4.0)}.items():
    pass
r3=evaluate(0.88,4.0,H=3); r6=evaluate(0.88,4.0,H=6)
print(f" H=3м: потери {r3['loss']:.2f}, t_вх {r3['ttc']:.1f}м -> роль короче разгона: найм нерационален")
print(f" H=6м: потери {r6['loss']:.2f}, t_вх {r6['ttc']:.1f}м -> начинает окупаться")
