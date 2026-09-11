"""
Порт DynamicFitService на Python 1:1 для прогона синтетических профилей.
Модель: диссертация, гл.2, рис.3 (с.66). Используется только для исследования,
в продакшн-контур не входит.
"""
import math, random

STEP = 0.01

def clamp(v, lo, hi):
    return lo if v < lo else (hi if v > hi else v)

def evaluate(Kk, Tm, Te=0.5, Tob=15.0, Kob=1.0, r=1.0, H=36.0):
    H = 36.0 if H <= 0 else H
    r = 1.0 if r <= 0 else r
    Kk = clamp(Kk, 0.01, 1.0)
    Tm = clamp(Tm, 0.1, 20.0)
    Te = 0.5 if Te <= 0 else Te
    Tob = 15.0 if Tob <= 0 else Tob
    Kob = 1.0 if Kob <= 0 else Kob
    n = int(H / STEP)
    I = y1 = y2 = 0.0
    loss = 0.0; peak = 0.0; ttc = -1.0
    for i in range(n):
        y = Kob * y2
        e = r - y
        I += Kk * e * STEP
        u = Tm * e + I
        y1 += (u - y1) / Te * STEP
        y2 += (y1 - y2) / Tob * STEP
        if e > 0: loss += e * STEP
        if y > peak: peak = y
        if ttc < 0 and y >= 0.95 * r: ttc = i * STEP
    steady = Kob * y2
    ov = (peak - steady) / steady * 100.0 if steady > 0 else 0.0
    return dict(loss=loss, ttc=(H if ttc < 0 else ttc), steady=steady, ov=ov)

def rotation_cost(Kk, Tm, vacancy_months, acting_eff, monthly_value, r=1.0, H=36.0, **kw):
    f = evaluate(Kk, Tm, r=r, H=H, **kw)
    gap = vacancy_months * (r - clamp(acting_eff, 0.0, r))
    return (gap + f['loss']) * monthly_value
