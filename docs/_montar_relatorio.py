# -*- coding: utf-8 -*-
"""Gera o DOCX do PIM e grava o sumário com a página impressa medida no PDF."""

import re
import subprocess
from pathlib import Path

from docx import Document
from docx.enum.section import WD_SECTION
from docx.enum.text import WD_ALIGN_PARAGRAPH, WD_TAB_ALIGNMENT, WD_TAB_LEADER
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from docx.shared import Cm, Pt
from pypdf import PdfReader

RAIZ = Path(__file__).resolve().parents[1]
DOCS = RAIZ / "docs"
DOCX = RAIZ / "PIM-VIII-Alunos-Tech-Consolidado.docx"
PDF = RAIZ / "PIM-VIII-Alunos-Tech-Consolidado.pdf"
SOFFICE = Path(r"C:\Program Files\LibreOffice\program\soffice.exe")

ARQUIVOS = [
    "00-capa.md",
    "00-folha-de-rosto.md",
    "00-resumo-abstract.md",
    "01-introducao.md",
    "02-arquitetura-devops.md",
    "01-gerenciamento-agil.md",
    "03-engenharia-dotnet.md",
    "04-banco-de-dados.md",
    "06-clientes-multiplataforma.md",
    "05-conclusao.md",
    "06-referencias.md",
    "07-apendices.md",
]


def fonte(run, tamanho=12, negrito=False, italico=False):
    run.bold = negrito
    run.italic = italico
    run.font.name = "Times New Roman"
    run.font.size = Pt(tamanho)
    rpr = run._element.get_or_add_rPr()
    rfonts = rpr.find(qn("w:rFonts"))
    if rfonts is None:
        rfonts = OxmlElement("w:rFonts")
        rpr.append(rfonts)
    for atributo in ("w:ascii", "w:hAnsi", "w:cs", "w:eastAsia"):
        rfonts.set(qn(atributo), "Times New Roman")


def paragrafo(doc, texto="", *, alinhamento, espacamento, recuo=0, deslocamento=0, antes=0, depois=0, tamanho=12):
    p = doc.add_paragraph()
    pf = p.paragraph_format
    pf.alignment = alinhamento
    pf.line_spacing = espacamento
    pf.space_before = Pt(antes)
    pf.space_after = Pt(depois)
    pf.first_line_indent = Cm(recuo) if recuo else Cm(0)
    if deslocamento:
        pf.left_indent = Cm(deslocamento)
    if texto:
        escrever(p, texto, tamanho=tamanho)
    return p


def escrever(paragrafo_alvo, texto, tamanho=12):
    partes = re.split(r"(\*\*[^*]+\*\*|\*[^*]+\*|`[^`]+`)", texto)
    for parte in partes:
        if not parte:
            continue
        if parte.startswith("**") and parte.endswith("**"):
            run = paragrafo_alvo.add_run(parte[2:-2])
            fonte(run, tamanho, negrito=True)
        elif parte.startswith("*") and parte.endswith("*"):
            run = paragrafo_alvo.add_run(parte[1:-1])
            fonte(run, tamanho, italico=True)
        elif parte.startswith("`") and parte.endswith("`"):
            run = paragrafo_alvo.add_run(parte[1:-1])
            fonte(run, tamanho)
        else:
            run = paragrafo_alvo.add_run(parte)
            fonte(run, tamanho)


def configurar_pagina(secao):
    secao.page_width = Cm(21)
    secao.page_height = Cm(29.7)
    secao.top_margin = Cm(3)
    secao.bottom_margin = Cm(2)
    secao.left_margin = Cm(3)
    secao.right_margin = Cm(2)
    secao.header_distance = Cm(2)
    secao.footer_distance = Cm(1)


def limpar_numeracao(secao):
    sect = secao._sectPr
    for filho in list(sect):
        if filho.tag == qn("w:pgNumType"):
            sect.remove(filho)


def iniciar_em(secao, numero):
    limpar_numeracao(secao)
    pg = OxmlElement("w:pgNumType")
    pg.set(qn("w:start"), str(numero))
    secao._sectPr.append(pg)


def campo_pagina(paragrafo_alvo):
    run = paragrafo_alvo.add_run()
    fonte(run, 12)
    inicio = OxmlElement("w:fldChar")
    inicio.set(qn("w:fldCharType"), "begin")
    instrucao = OxmlElement("w:instrText")
    instrucao.set(qn("xml:space"), "preserve")
    instrucao.text = " PAGE "
    fim = OxmlElement("w:fldChar")
    fim.set(qn("w:fldCharType"), "end")
    run._r.append(inicio)
    run._r.append(instrucao)
    run._r.append(fim)


def cabecalho_visivel(secao, mostrar):
    secao.header.is_linked_to_previous = False
    secao.footer.is_linked_to_previous = False
    cabecalho = secao.header
    paragrafo_cab = cabecalho.paragraphs[0]
    paragrafo_cab.clear()
    paragrafo_cab.alignment = WD_ALIGN_PARAGRAPH.RIGHT
    if mostrar:
        campo_pagina(paragrafo_cab)
    secao.footer.paragraphs[0].clear()


def titulo_h1(texto):
    texto = texto.strip()
    if texto.startswith("APÊNDICE"):
        return texto
    if texto.upper() == "REFERÊNCIAS":
        return "REFERÊNCIAS"
    correspondencia = re.match(r"^(\d+\.\s+)(.+)$", texto)
    if correspondencia:
        return correspondencia.group(1) + correspondencia.group(2).upper()
    return texto.upper()


def ler_blocos():
    blocos = []
    for nome in ARQUIVOS:
        for bruto in (DOCS / nome).read_text(encoding="utf-8").splitlines():
            linha = bruto.rstrip()
            if linha.startswith("<!--"):
                continue
            blocos.append(linha)
        blocos.append("")
    return blocos


def classificar(blocos):
    itens = []
    buffer = []
    cerca = False
    linhas_codigo = []

    def despejar():
        texto = " ".join(parte.strip() for parte in buffer if parte.strip())
        buffer.clear()
        if texto:
            itens.append(("p", texto))

    for linha in blocos:
        if cerca:
            if linha.strip() == "```":
                itens.append(("codigo", "\n".join(linhas_codigo)))
                cerca = False
                linhas_codigo = []
            else:
                linhas_codigo.append(linha)
            continue
        if linha.strip() == "```":
            despejar()
            cerca = True
            continue
        if linha.startswith("[[figura:"):
            despejar()
            itens.append(("figura", linha[len("[[figura:") : -2].strip()))
            continue
        if linha.startswith("[[legenda:"):
            despejar()
            itens.append(("legenda", linha[len("[[legenda:") : -2].strip()))
            continue
        if linha.startswith("[[fonte:"):
            despejar()
            itens.append(("fonte", linha[len("[[fonte:") : -2].strip()))
            continue
        if not linha.strip():
            despejar()
            continue
        if linha.startswith("#"):
            despejar()
            nivel = len(linha) - len(linha.lstrip("#"))
            itens.append((f"h{nivel}", linha[nivel:].strip()))
        else:
            buffer.append(linha)
    despejar()
    return itens


def escrever_codigo(doc, texto):
    for linha in texto.split("\n"):
        p = paragrafo(doc, "", alinhamento=WD_ALIGN_PARAGRAPH.LEFT, espacamento=1.0)
        run = p.add_run(linha if linha else " ")
        fonte(run, tamanho=10)


def montar(paginas, paginas_figuras):
    doc = Document()
    secao = doc.sections[0]
    configurar_pagina(secao)
    limpar_numeracao(secao)
    cabecalho_visivel(secao, False)

    estilo = doc.styles["Normal"]
    estilo.font.name = "Times New Roman"
    estilo.font.size = Pt(12)
    estilo._element.rPr.rFonts.set(qn("w:ascii"), "Times New Roman")
    estilo._element.rPr.rFonts.set(qn("w:hAnsi"), "Times New Roman")

    modo = "capa"
    titulos = []
    figuras = []

    def nova_secao(mostrar_numero, inicio=None):
        nonlocal secao
        secao = doc.add_section(WD_SECTION.NEW_PAGE)
        configurar_pagina(secao)
        if inicio is None:
            limpar_numeracao(secao)
        else:
            iniciar_em(secao, inicio)
        cabecalho_visivel(secao, mostrar_numero)

    for tipo, texto in classificar(ler_blocos()):
        if tipo == "h1" and texto == "Capa":
            modo = "capa"
            continue
        if tipo == "h1" and texto == "Folha de rosto":
            modo = "folha"
            nova_secao(False, 1)
            continue
        if tipo == "h1" and texto == "Resumo":
            modo = "resumo"
            doc.add_page_break()
            p = paragrafo(doc, "RESUMO", alinhamento=WD_ALIGN_PARAGRAPH.CENTER, espacamento=1.0, antes=0, depois=12)
            for run in p.runs:
                run.bold = True
            continue
        if tipo == "h1" and texto == "Abstract":
            modo = "abstract"
            doc.add_page_break()
            p = paragrafo(doc, "ABSTRACT", alinhamento=WD_ALIGN_PARAGRAPH.CENTER, espacamento=1.0, antes=0, depois=12)
            for run in p.runs:
                run.bold = True
            continue
        if tipo == "h1" and texto.startswith("1."):
            modo = "texto"
            nova_secao(True, None)
        if tipo == "h1" and texto.startswith("APÊNDICE"):
            modo = "apendice"
        if tipo.startswith("h") and modo in ("texto", "referencias", "apendice"):
            nivel = int(tipo[1])
            mostrado = titulo_h1(texto) if nivel == 1 else texto
            if mostrado in ("7. CONCLUSÃO", "REFERÊNCIAS") or mostrado.startswith("APÊNDICE"):
                doc.add_page_break()
            if mostrado == "REFERÊNCIAS":
                modo = "referencias"
            titulos.append((nivel, mostrado))
            paragrafo(
                doc,
                mostrado,
                alinhamento=WD_ALIGN_PARAGRAPH.LEFT,
                espacamento=1.5,
                antes=12 if nivel == 1 else 10,
                depois=6,
            )
            for run in doc.paragraphs[-1].runs:
                run.bold = True
            continue
        if tipo == "figura":
            caminho, largura = texto.split("|")
            p = paragrafo(doc, "", alinhamento=WD_ALIGN_PARAGRAPH.CENTER, espacamento=1.0, antes=6, depois=0)
            p.add_run().add_picture(str(DOCS / caminho.strip()), width=Cm(float(largura)))
            continue
        if tipo == "legenda":
            if texto.startswith("Figura "):
                figuras.append(texto)
            p = paragrafo(doc, texto, alinhamento=WD_ALIGN_PARAGRAPH.CENTER, espacamento=1.0, antes=6, depois=0)
            for run in p.runs:
                run.bold = True
            continue
        if tipo == "fonte":
            paragrafo(doc, texto, alinhamento=WD_ALIGN_PARAGRAPH.CENTER, espacamento=1.0, antes=0, depois=8)
            continue
        if tipo == "codigo":
            escrever_codigo(doc, texto)
            continue
        if tipo != "p":
            continue
        if modo == "capa":
            negrito = texto.startswith("**")
            limpo = texto.replace("**", "")
            p = paragrafo(doc, limpo, alinhamento=WD_ALIGN_PARAGRAPH.CENTER, espacamento=1.5, antes=12, depois=0)
            for run in p.runs:
                run.bold = negrito or limpo.startswith("PLATAFORMA")
            continue
        if modo == "folha":
            if texto.startswith("Projeto Integrado Multidisciplinar para") or texto.startswith("Orientador"):
                paragrafo(
                    doc,
                    texto,
                    alinhamento=WD_ALIGN_PARAGRAPH.RIGHT,
                    espacamento=1.0,
                    deslocamento=8,
                    antes=12,
                    depois=0,
                    tamanho=10,
                )
                continue
            negrito = texto.startswith("**")
            limpo = texto.replace("**", "")
            p = paragrafo(doc, limpo, alinhamento=WD_ALIGN_PARAGRAPH.CENTER, espacamento=1.5, antes=12, depois=0)
            for run in p.runs:
                run.bold = negrito or limpo.startswith("PLATAFORMA")
            continue
        if modo in ("resumo", "abstract"):
            if texto.startswith("**Palavras-chave:") or texto.startswith("**Keywords:"):
                paragrafo(doc, texto, alinhamento=WD_ALIGN_PARAGRAPH.JUSTIFY, espacamento=1.0, antes=12, depois=0)
            else:
                paragrafo(doc, texto, alinhamento=WD_ALIGN_PARAGRAPH.JUSTIFY, espacamento=1.0, recuo=1.5)
            continue
        if modo == "referencias":
            item = paragrafo(doc, texto, alinhamento=WD_ALIGN_PARAGRAPH.JUSTIFY, espacamento=1.0, depois=12)
            item.paragraph_format.left_indent = Cm(1.5)
            item.paragraph_format.first_line_indent = Cm(-1.5)
            continue
        paragrafo(doc, texto, alinhamento=WD_ALIGN_PARAGRAPH.JUSTIFY, espacamento=1.5, recuo=1.5)

    introducao = None
    for paragrafo_doc in doc.paragraphs:
        if paragrafo_doc.text.startswith("1. INTRODUÇÃO"):
            introducao = paragrafo_doc
            break
    if introducao is None:
        raise RuntimeError("Introdução não encontrada.")
    quebra_de_secao = introducao._element.getprevious()
    while quebra_de_secao is not None:
        if quebra_de_secao.tag == qn("w:p"):
            propriedades = quebra_de_secao.find(qn("w:pPr"))
            if propriedades is not None and propriedades.find(qn("w:sectPr")) is not None:
                break
        quebra_de_secao = quebra_de_secao.getprevious()
    if quebra_de_secao is None:
        raise RuntimeError("Quebra de seção do pré-texto não encontrada.")

    def inserir_antes(elemento, novo):
        elemento.addprevious(novo)

    quebra = doc.add_paragraph()
    quebra.paragraph_format.space_before = Pt(0)
    quebra.paragraph_format.space_after = Pt(0)
    quebra.paragraph_format.first_line_indent = Cm(0)
    run_quebra = quebra.add_run()
    br = OxmlElement("w:br")
    br.set(qn("w:type"), "page")
    run_quebra._r.append(br)
    inserir_antes(quebra_de_secao, quebra._element)
    inserir_lista(doc, quebra._element, figuras, paginas_figuras)

    titulo = doc.add_paragraph()
    titulo.alignment = WD_ALIGN_PARAGRAPH.CENTER
    titulo.paragraph_format.line_spacing = 1.5
    titulo.paragraph_format.space_after = Pt(12)
    titulo.paragraph_format.first_line_indent = Cm(0)
    run = titulo.add_run("SUMÁRIO")
    fonte(run, 12, negrito=True)
    inserir_antes(quebra_de_secao, titulo._element)

    for nivel, mostrado in titulos:
        if mostrado.startswith("APÊNDICE"):
            continue
        if mostrado in ("RESUMO", "ABSTRACT", "LISTA DE ILUSTRAÇÕES"):
            continue
        pagina = paginas.get(mostrado, "…")
        item = doc.add_paragraph()
        pf = item.paragraph_format
        pf.alignment = WD_ALIGN_PARAGRAPH.LEFT
        pf.line_spacing = 1.5
        pf.space_before = Pt(0)
        pf.space_after = Pt(0)
        pf.first_line_indent = Cm(0)
        pf.left_indent = Cm(0.75 * (nivel - 1))
        pf.tab_stops.add_tab_stop(Cm(16), WD_TAB_ALIGNMENT.RIGHT, WD_TAB_LEADER.DOTS)
        run = item.add_run(f"{mostrado}\t{pagina}")
        fonte(run, 12, negrito=True)
        inserir_antes(quebra_de_secao, item._element)

    ajustes = doc.settings.element
    auto = ajustes.find(qn("w:autoHyphenation"))
    if auto is None:
        auto = OxmlElement("w:autoHyphenation")
        ajustes.append(auto)
    auto.set(qn("w:val"), "false")
    doc.save(DOCX)
    return titulos, figuras


def inserir_lista(doc, antes, figuras, paginas_figuras):
    def colocar(elemento):
        antes.addprevious(elemento)

    quebra = doc.add_paragraph()
    quebra.paragraph_format.space_before = Pt(0)
    quebra.paragraph_format.space_after = Pt(0)
    quebra.paragraph_format.first_line_indent = Cm(0)
    run_quebra = quebra.add_run()
    br = OxmlElement("w:br")
    br.set(qn("w:type"), "page")
    run_quebra._r.append(br)
    colocar(quebra._element)

    titulo = doc.add_paragraph()
    titulo.alignment = WD_ALIGN_PARAGRAPH.CENTER
    titulo.paragraph_format.line_spacing = 1.5
    titulo.paragraph_format.space_before = Pt(0)
    titulo.paragraph_format.space_after = Pt(12)
    titulo.paragraph_format.first_line_indent = Cm(0)
    run = titulo.add_run("LISTA DE ILUSTRAÇÕES")
    fonte(run, 12, negrito=True)
    colocar(titulo._element)

    for legenda in figuras:
        pagina = paginas_figuras.get(legenda, "…")
        item = doc.add_paragraph()
        pf = item.paragraph_format
        pf.alignment = WD_ALIGN_PARAGRAPH.LEFT
        pf.line_spacing = 1.5
        pf.space_before = Pt(0)
        pf.space_after = Pt(0)
        pf.first_line_indent = Cm(0)
        pf.left_indent = Cm(0)
        pf.tab_stops.add_tab_stop(Cm(16), WD_TAB_ALIGNMENT.RIGHT, WD_TAB_LEADER.DOTS)
        run = item.add_run(f"{legenda}\t{pagina}")
        fonte(run, 12)
        colocar(item._element)


def normalizar(texto):
    return re.sub(r"\s+", " ", texto or "")


def medir(titulos, figuras):
    if PDF.exists():
        PDF.unlink()
    subprocess.run(
        [str(SOFFICE), "--headless", "--convert-to", "pdf", "--outdir", str(RAIZ), str(DOCX)],
        check=True,
        cwd=RAIZ,
    )
    leitor = PdfReader(str(PDF))
    paginas_texto = [normalizar(pagina.extract_text()) for pagina in leitor.pages]
    encontradas = {}
    anterior = 1
    for _, titulo in titulos:
        prefixo = normalizar(titulo)[:60]
        ocorrencias = [
            indice + 1
            for indice, texto in enumerate(paginas_texto)
            if prefixo in texto and indice + 1 >= anterior
        ]
        if not ocorrencias:
            raise RuntimeError(f"Título não encontrado no PDF: {titulo}")
        impressa = ocorrencias[-1] - 1
        encontradas[titulo] = impressa
        anterior = ocorrencias[-1]
    paginas_lista = [
        indice + 1
        for indice, texto in enumerate(paginas_texto)
        if "LISTA DE ILUSTRA" in texto
    ]
    if not paginas_lista:
        raise RuntimeError("Lista de ilustrações não encontrada no PDF.")
    limite = paginas_lista[-1]
    encontradas_figuras = {}
    for legenda in figuras:
        prefixo = normalizar(legenda)
        ocorrencias = [
            indice + 1
            for indice, texto in enumerate(paginas_texto)
            if prefixo in texto and indice + 1 > limite
        ]
        if not ocorrencias:
            raise RuntimeError(f"Figura não encontrada no PDF: {legenda}")
        encontradas_figuras[legenda] = ocorrencias[0] - 1
    return encontradas, encontradas_figuras, len(leitor.pages)


def desenvolvimento(paginas):
    inicio = next(pagina for titulo, pagina in paginas.items() if titulo.startswith("2. "))
    fim = next(pagina for titulo, pagina in paginas.items() if titulo.startswith("7. "))
    return fim - inicio, inicio, fim


def main():
    paginas = {}
    paginas_figuras = {}
    titulos = []
    figuras = []
    for _ in range(4):
        titulos, figuras = montar(paginas, paginas_figuras)
        novas, novas_figuras, total = medir(titulos, figuras)
        if novas == paginas and novas_figuras == paginas_figuras:
            faixa, inicio, fim = desenvolvimento(novas)
            print(f"PAGINAS_PDF={total}")
            print(f"DESENVOLVIMENTO={faixa}")
            print(f"CAP2={inicio}")
            print(f"CAP7={fim}")
            for titulo in [item[1] for item in titulos if item[0] == 1]:
                print(f"{novas[titulo]} | {titulo}")
            for legenda in figuras:
                print(f"{novas_figuras[legenda]} | {legenda}")
            if not 15 <= faixa <= 20:
                raise RuntimeError(f"Desenvolvimento fora de 15 a 20 páginas: {faixa}")
            return
        paginas = novas
        paginas_figuras = novas_figuras
    raise RuntimeError("O sumário não estabilizou.")


if __name__ == "__main__":
    main()
