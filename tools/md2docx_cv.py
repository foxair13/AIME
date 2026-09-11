# -*- coding: utf-8 -*-
"""Конвертация ATS-резюме из Markdown в .docx.
Только линейные абзацы: без таблиц, колонок, textbox, колонтитулов и графики."""
import sys, re
from docx import Document
from docx.shared import Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH

def build(md_path, out_path):
    lines = open(md_path, encoding='utf-8').read().split('\n')
    doc = Document()
    st = doc.styles['Normal']
    st.font.name = 'Calibri'          # стандартный шрифт, гарантированно извлекаемый
    st.font.size = Pt(10.5)
    for s in doc.sections:
        s.left_margin = s.right_margin = Pt(50)
        s.top_margin = s.bottom_margin = Pt(40)

    def strip_em(t):
        return t.replace('*', '')

    def add(text, size=10.5, bold=False, space_before=0, space_after=3, color=None):
        text = strip_em(text)
        p = doc.add_paragraph()
        p.paragraph_format.space_before = Pt(space_before)
        p.paragraph_format.space_after = Pt(space_after)
        p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        # разбор **жирного** внутри строки
        for part in re.split(r'(\*\*[^*]+\*\*)', text):
            if not part: continue
            r = p.add_run(part[2:-2] if part.startswith('**') else part)
            r.bold = bold or part.startswith('**')
            r.font.size = Pt(size)
            if color: r.font.color.rgb = color
        return p

    i = 0
    while i < len(lines):
        ln = lines[i].rstrip()
        if ln.startswith('# '):
            add(ln[2:], size=20, bold=True, space_after=2)
        elif ln.startswith('### '):
            add(ln[4:], size=11.5, bold=True, space_before=10, space_after=1)
        elif ln.startswith('## '):
            add(ln[3:].upper(), size=12, bold=True, space_before=12, space_after=4)
        elif ln.startswith('- '):
            p = doc.add_paragraph(style='List Bullet')
            p.paragraph_format.space_after = Pt(2)
            for part in re.split(r'(\*\*[^*]+\*\*)', strip_em(ln[2:])):
                if not part: continue
                r = p.add_run(part[2:-2] if part.startswith('**') else part)
                r.bold = part.startswith('**'); r.font.size = Pt(10.5)
        elif ln.startswith('---') or not ln.strip():
            pass
        else:
            add(ln)
        i += 1
    doc.save(out_path)
    print("сохранено:", out_path)

if __name__ == '__main__':
    build(sys.argv[1], sys.argv[2])
