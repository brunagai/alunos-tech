# APÊNDICE A — CLUSTER LOCAL

O sistema ligado neste trabalho é o cluster kind nesta máquina, alcançado em 127.0.0.1:18080. Não é nuvem de provedor. A Figura 11 mostra as duas sondas em Healthy, o nó alunos-tech-control-plane em Ready e o HPA com 2 réplicas, CPU em 2% do pedido de 70%, mínimo 2 e máximo 6. Uma leitura anterior deste mesmo HPA chegou a 6 réplicas; a figura registra a leitura posterior, já reduzida. O endereço /swagger/index.html respondeu HTTP 404, porque a imagem do cluster não abre a interface de desenvolvimento. A lista de rotas do Swagger não entra aqui.

[[figura:figuras/apendice-sondas.png|16]]
[[legenda:Figura 11 — Sondas, nó e HPA no cluster kind local, não num provedor]]
[[fonte:Fonte: Elaborada pela autora.]]

# APÊNDICE B — TABELAS E O PROCEDIMENTO

A Figura 12 mostra Alunos e Cursos. A Figura 13 mostra Matriculas, com o índice único UX_Matriculas_AlunoId_CursoId sobre o par aluno-curso. A Figura 14 mostra ChamadosSuporte, inclusive o gatilho de atualização.

[[figura:figuras/apendice-alunos-cursos.png|16]]
[[legenda:Figura 12 — Tabelas Alunos e Cursos no PostgreSQL do cluster]]
[[fonte:Fonte: Elaborada pela autora.]]

[[figura:figuras/apendice-matriculas.png|16]]
[[legenda:Figura 13 — Tabela Matriculas e o índice único do par aluno-curso]]
[[fonte:Fonte: Elaborada pela autora.]]

[[figura:figuras/apendice-chamados.png|16]]
[[legenda:Figura 14 — Tabela ChamadosSuporte]]
[[fonte:Fonte: Elaborada pela autora.]]

O procedimento sp_MatricularAlunoEmLote foi chamado duas vezes na mesma transação. A primeira inserção registrou 1 linha e a segunda registrou 0. O ROLLBACK desfez o ensaio, então o aluno de teste não permaneceu.

[[figura:figuras/apendice-lote.png|16]]
[[legenda:Figura 15 — Segunda chamada de sp_MatricularAlunoEmLote com zero linhas]]
[[fonte:Fonte: Elaborada pela autora.]]

# APÊNDICE C — TRECHOS DE CÓDIGO

Os trechos abaixo são recortes dos arquivos que sustentam o token, a matrícula, o lote e a imagem aplicada. Nenhum arquivo entra inteiro.

[[legenda:Quadro 1 — Configuração do JWT]]
```
var chave = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(chave) || chave.Length < 32)
{
    throw new InvalidOperationException("Jwt:SigningKey precisa ter pelo menos 32 caracteres.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
```
[[fonte:Fonte: Elaborada pela autora. Arquivo Program.cs, linhas 24 a 44.]]

[[legenda:Quadro 2 — POST /api/matriculas]]
```
    [HttpPost]
    public async Task<IActionResult> Matricular(
        [FromBody] MatricularAlunoDto dto,
        CancellationToken cancellationToken)
    {
        if (!AcessoAluno.PodeOperar(User, dto.AlunoId))
        {
            return Forbid();
        }

        var id = await _matricularAlunoUseCase.ExecutarAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(Matricular), new { id }, new { id });
    }
```
[[fonte:Fonte: Elaborada pela autora. Arquivo MatriculasController.cs, linhas 21 a 34.]]

[[legenda:Quadro 3 — Inserção de sp_MatricularAlunoEmLote]]
```
    INSERT INTO "Matriculas" (
        "Id", "AlunoId", "CursoId", "Status", "DataMatricula", "CreatedAt", "UpdatedAt"
    )
    SELECT gen_random_uuid(), p_AlunoId, cursos."CursoId", 1, NOW(), NOW(), NOW()
    FROM (
        SELECT DISTINCT curso_id AS "CursoId"
        FROM unnest(p_CursoIds) AS curso_id
        WHERE curso_id IS NOT NULL
    ) AS cursos
    WHERE NOT EXISTS (
        SELECT 1
        FROM "Matriculas" AS matricula
        WHERE matricula."AlunoId" = p_AlunoId
          AND matricula."CursoId" = cursos."CursoId"
    )
    ON CONFLICT ("AlunoId", "CursoId") DO NOTHING;

    GET DIAGNOSTICS v_inseridas = ROW_COUNT;
    RAISE NOTICE 'Matrículas inseridas para o aluno %: %', p_AlunoId, v_inseridas;
```
[[fonte:Fonte: Elaborada pela autora. Arquivo scripts_avancados.sql, linhas 67 a 85.]]

[[legenda:Quadro 4 — Dockerfile.cluster da imagem aplicada]]
```
# Imagem efetivamente publicada no registro local.
# O docker build do Dockerfile principal não conseguiu baixar
# mcr.microsoft.com/dotnet/sdk:8.0.427 nem aspnet:8.0.33 (EOF do daemon).
# Esta imagem parte de aspnet:10.0, já presente na máquina, e executa o
# publish net8.0 com roll-forward de major.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY . .
USER 1654
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_ROLL_FORWARD=Major
EXPOSE 8080
ENTRYPOINT ["dotnet", "AlunosTech.Api.dll"]
```
[[fonte:Fonte: Elaborada pela autora. Arquivo Dockerfile.cluster, linhas 1 a 15.]]
