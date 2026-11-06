# -*- coding: utf-8 -*-
"""Desenha as duas figuras de topologia e de modelo a partir do que o cluster executa."""

from PIL import Image, ImageDraw, ImageFont

FONTE = ImageFont.truetype(r"C:\Windows\Fonts\times.ttf", 28)
FONTE_B = ImageFont.truetype(r"C:\Windows\Fonts\timesbd.ttf", 30)
FONTE_P = ImageFont.truetype(r"C:\Windows\Fonts\times.ttf", 24)
PRETO = (0, 0, 0)
BRANCO = (255, 255, 255)
CINZA = (245, 245, 245)


def caixa(draw, xy, titulo, linhas):
    draw.rounded_rectangle(xy, radius=12, fill=BRANCO, outline=PRETO, width=3)
    x1, y1, x2, y2 = xy
    draw.text((x1 + 18, y1 + 14), titulo, font=FONTE_B, fill=PRETO)
    y = y1 + 56
    for linha in linhas:
        draw.text((x1 + 18, y), linha, font=FONTE_P, fill=PRETO)
        y += 32


def seta(draw, a, b):
    draw.line([a, b], fill=PRETO, width=3)
    x, y = b
    ax, ay = a
    if abs(x - ax) >= abs(y - ay):
        sinal = 1 if x > ax else -1
        draw.polygon([(x, y), (x - 14 * sinal, y - 8), (x - 14 * sinal, y + 8)], fill=PRETO)
    else:
        sinal = 1 if y > ay else -1
        draw.polygon([(x, y), (x - 8, y - 14 * sinal), (x + 8, y - 14 * sinal)], fill=PRETO)


def arquitetura(destino):
    im = Image.new("RGB", (1680, 900), BRANCO)
    draw = ImageDraw.Draw(im)
    draw.rounded_rectangle((30, 30, 1100, 870), radius=16, fill=CINZA, outline=PRETO, width=3)
    draw.text((52, 48), "Cluster kind — nó alunos-tech-control-plane", font=FONTE_B, fill=PRETO)
    draw.text((52, 90), "Kubernetes v1.33.1 nesta máquina. Não é nuvem de provedor.", font=FONTE_P, fill=PRETO)
    caixa(draw, (70, 160, 520, 340), "Portal do gestor", ["/gestor/ no navegador"])
    caixa(draw, (620, 160, 1100, 340), "Aplicativo MAUI", ["AlunosTech.Mobile no emulador"])
    caixa(draw, (180, 400, 980, 620), "API", ["127.0.0.1:18080", "localhost:5001/alunos-tech-api:4604e61"])
    caixa(draw, (1220, 400, 1640, 620), "Registro local", ["localhost:5001", "tag 4604e61"])
    caixa(draw, (180, 680, 980, 860), "PostgreSQL 16", ["serviço ClusterIP, sem porta no host"])
    seta(draw, (295, 340), (420, 400))
    seta(draw, (860, 340), (740, 400))
    seta(draw, (580, 620), (580, 680))
    seta(draw, (1220, 510), (980, 510))
    im.save(destino)


def modelo(destino):
    im = Image.new("RGB", (1680, 920), BRANCO)
    draw = ImageDraw.Draw(im)
    caixa(draw, (40, 30, 560, 280), "Alunos", [
        "Id  uuid  PK",
        "Nome, Email, Cargo",
        "Ativo, CreatedAt, UpdatedAt",
    ])
    caixa(draw, (1120, 30, 1640, 280), "Cursos", [
        "Id  uuid  PK",
        "Titulo, Descricao, CargaHoraria",
        "Ativo, CreatedAt, UpdatedAt",
    ])
    caixa(draw, (40, 430, 560, 780), "ChamadosSuporte", [
        "Id  uuid  PK",
        "AlunoId  FK para Alunos",
        "Titulo, Descricao",
        "Status, Prioridade",
    ])
    caixa(draw, (760, 400, 1560, 860), "Matriculas", [
        "Id  uuid  PK",
        "AlunoId  FK para Alunos",
        "CursoId  FK para Cursos",
        "Status, DataMatricula",
        "CreatedAt, UpdatedAt",
        "UX_Matriculas_AlunoId_CursoId",
        "UNIQUE (AlunoId, CursoId)",
    ])
    draw.line([(300, 280), (300, 430)], fill=PRETO, width=3)
    draw.text((312, 330), "1", font=FONTE_B, fill=PRETO)
    draw.text((312, 380), "N", font=FONTE_B, fill=PRETO)
    draw.line([(400, 280), (900, 400)], fill=PRETO, width=3)
    draw.text((520, 300), "1", font=FONTE_B, fill=PRETO)
    draw.text((820, 350), "N", font=FONTE_B, fill=PRETO)
    draw.line([(1380, 280), (1200, 400)], fill=PRETO, width=3)
    draw.text((1340, 300), "1", font=FONTE_B, fill=PRETO)
    draw.text((1220, 350), "N", font=FONTE_B, fill=PRETO)
    im.save(destino)


def recortar(origem, destino, margem=28):
    im = Image.open(origem).convert("RGB")
    pixels = im.load()
    largura, altura = im.size
    topo, fundo, esquerda, direita = altura, 0, largura, 0
    for y in range(altura):
        for x in range(largura):
            r, g, b = pixels[x, y]
            if r < 250 or g < 250 or b < 250:
                topo = min(topo, y)
                fundo = max(fundo, y)
                esquerda = min(esquerda, x)
                direita = max(direita, x)
    caixa_recorte = (
        max(0, esquerda - margem),
        max(0, topo - margem),
        min(largura, direita + margem + 1),
        min(altura, fundo + margem + 1),
    )
    im.crop(caixa_recorte).save(destino)


if __name__ == "__main__":
    from pathlib import Path
    base = Path(__file__).resolve().parent
    arquitetura(base / "arquitetura.png")
    modelo(base / "modelo.png")
    for nome in (
        "portal-login.png",
        "portal-matricula.png",
        "portal-chamado.png",
        "portal-duplicada.png",
        "navegador-colaborador.png",
    ):
        recortar(base / nome, base / nome)
    print("desenhos prontos")
