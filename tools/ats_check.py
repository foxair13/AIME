# -*- coding: utf-8 -*-
"""Имитация того, что делает ATS-парсер с документом: извлечение дат, должностей, стажа."""
import re, sys, datetime

def parse_dates(text):
    """ATS ищет диапазоны дат. Считаем, сколько форматов встречается."""
    pats = {
        "DD.MM.YYYY":  r'\b\d{2}\.\d{2}\.\d{4}\b',
        "MM.YYYY":     r'(?<!\d\.)\b\d{2}\.\d{4}\b',
        "YYYY":        r'(?<![\d.])\b(?:19|20)\d{2}\b(?![\d.])',
        "Месяц YYYY":  r'\b(?:январ|феврал|март|апрел|ма[йя]|июн|июл|август|сентябр|октябр|ноябр|декабр)\w*\s+\d{4}',
    }
    return {k: len(re.findall(v, text, re.I)) for k, v in pats.items()}

def find_pii(text):
    flags = []
    checks = [
        ("дата рождения",      r'Дата рождения'),
        ("возраст",            r'возраст\s*\d+'),
        ("семейное положение", r'Семейное положение'),
        ("адрес прописки",     r'Адрес регистрации'),
        ("родственники",       r'Близкие родственники|Степень родства'),
        ("воинская обязанность", r'воинской обязанности|воинское звание'),
        ("здоровье родных",    r'Инвалид|умер'),
        ("зарплатные ожидания", r'уровню заработной платы|б\.р'),
        ("гражданство",        r'Гражданство'),
        ("фото",               r'фото'),
        ("водительские права", r'права категории|стаж вождения'),
    ]
    for name, pat in checks:
        if re.search(pat, text, re.I): flags.append(name)
    return flags

def structure(text):
    lines = text.split('\n')
    tabbed = sum(1 for l in lines if '\t' in l)
    return dict(lines=len(lines), tab_lines=tabbed,
                tab_ratio=round(tabbed/max(len(lines),1)*100),
                empty=sum(1 for l in lines if not l.strip()))

STD_HEADERS = ["опыт работы","профессиональный опыт","образование","навыки",
               "ключевые навыки","контакты","о себе","профиль"]
def headers(text):
    low = text.lower()
    return [h for h in STD_HEADERS if h in low]

JD_KEYWORDS = [".net","c#","asp.net","entity framework","postgresql","docker",
    "kubernetes","microservices","ddd","solid","rest","api","ci/cd","git",
    "linq","sql","react","signalr","unit test","agile","архитект"]
def kw(text):
    low = text.lower()
    hit = [k for k in JD_KEYWORDS if k in low]
    return hit, [k for k in JD_KEYWORDS if k not in low]

raw = open(sys.argv[1], encoding='utf-8').read()
print("="*70)
print("ИМИТАЦИЯ ATS-ПАРСИНГА:", sys.argv[1])
print("="*70)
s = structure(raw)
print(f"\n[СТРУКТУРА] строк: {s['lines']}, из них с табуляцией (таблицы): "
      f"{s['tab_lines']} ({s['tab_ratio']}%)")
print("  -> ATS читает таблицу построчно слева направо; подписи и значения слипаются." if s['tab_ratio']>15
      else "  -> структура линейная, приемлемо")
d = parse_dates(raw)
print(f"\n[ДАТЫ] найденные форматы: {d}")
used = [k for k,v in d.items() if v>0]
print(f"  -> форматов одновременно: {len(used)}. "
      + ("КОНФЛИКТ: парсер не сможет однозначно построить таймлайн." if len(used)>2 else "ок"))
p = find_pii(raw)
print(f"\n[ЛИШНИЕ ПЕРСОНАЛЬНЫЕ ДАННЫЕ] {len(p)} категорий:")
for x in p: print(f"   - {x}")
h = headers(raw)
print(f"\n[СТАНДАРТНЫЕ ЗАГОЛОВКИ] найдено {len(h)}/{len(STD_HEADERS)}: {h}")
hit, miss = kw(raw)
print(f"\n[КЛЮЧЕВЫЕ СЛОВА под вакансию Senior .NET] {len(hit)}/{len(JD_KEYWORDS)}")
print(f"   отсутствуют: {miss}")
