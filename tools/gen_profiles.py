"""Генерация синтетической когорты кандидатов и прогон через DFS."""
import sys, json, random
sys.path.insert(0, '/home/user/AIME/tools')
from dfs_sim import evaluate, rotation_cost

random.seed(20260912)

# Архетипы: (Kкомпет, Tмотив) + разброс. Статический балл = то, что видит обычный ATS:
# опыт/тесты/образование -> коррелирует с компетентностью, но НЕ с мотивацией.
ARCHETYPES = [
    ("Звезда рынка",        0.92, 1.2),
    ("Опытный, выгоревший", 0.85, 0.8),
    ("Крепкий профи",       0.70, 3.0),
    ("Середняк мотивир.",   0.55, 6.0),
    ("Джун голодный",       0.30, 11.0),
    ("Джун спокойный",      0.32, 2.0),
    ("Переученный теоретик",0.78, 1.5),
    ("Практик без диплома", 0.48, 8.0),
]
NAMES = ["Алексей","Марина","Дмитрий","Ольга","Сергей","Ирина","Павел","Наталья",
         "Виктор","Елена","Роман","Татьяна","Игорь","Юлия","Максим","Анна",
         "Кирилл","Светлана","Артём","Людмила","Денис","Вера","Никита","Галина"]

def make_cohort(n=24):
    out = []
    for i in range(n):
        name = NAMES[i % len(NAMES)]
        arch, kk, tm = ARCHETYPES[i % len(ARCHETYPES)]
        kk = max(0.05, min(1.0, kk * random.gauss(1, 0.10)))
        tm = max(0.2, min(20.0, tm * random.gauss(1, 0.15)))
        # Классический скоринг: резюме/тесты видят компетентность и почти не видят драйв.
        static = 100 * (0.85 * kk + 0.15 * min(tm / 12.0, 1.0)) * random.gauss(1, 0.05)
        out.append(dict(id=i+1, name=f"{name} ({arch})", kk=round(kk,3),
                        tm=round(tm,2), static=round(static,1)))
    return out

if __name__ == "__main__":
    cohort = make_cohort(24)
    H = 36.0
    for c in cohort:
        r = evaluate(c['kk'], c['tm'], H=H)
        c.update(loss=round(r['loss'],3), ttc=round(r['ttc'],2),
                 ov=round(r['ov'],1), steady=round(r['steady'],3),
                 cost=round(rotation_cost(c['kk'], c['tm'], 2.0, 0.3, 12000.0, H=H)))
    json.dump(cohort, open('/home/user/AIME/tools/cohort.json','w'),
              ensure_ascii=False, indent=1)

    by_static = sorted(cohort, key=lambda x: -x['static'])
    by_dfs    = sorted(cohort, key=lambda x: x['loss'])

    print(f"{'#':>2} {'ПО КЛАССИЧЕСКОМУ СКОРИНГУ':<34}{'балл':>6} | {'ПО DFS (потери)':<34}{'J':>6} {'t_вх':>6} {'выброс':>7}")
    print("-"*112)
    for i,(a,b) in enumerate(zip(by_static, by_dfs), 1):
        print(f"{i:>2} {a['name']:<34}{a['static']:>6.1f} | {b['name']:<34}{b['loss']:>6.2f} {b['ttc']:>6.1f} {b['ov']:>6.1f}%")

    top_s = {c['id'] for c in by_static[:5]}
    top_d = {c['id'] for c in by_dfs[:5]}
    print(f"\nПересечение топ-5: {len(top_s & top_d)} из 5")
    print("Взял бы скоринг, отверг DFS:", [c['name'] for c in by_static[:5] if c['id'] not in top_d])
    print("Взял бы DFS, отверг скоринг:", [c['name'] for c in by_dfs[:5] if c['id'] not in top_s])
