-- Alunos Tech — scripts avançados de banco (PostgreSQL 16)
--
-- Pré-requisito: as tabelas já existem no schema public, com identificadores
-- entre aspas para preservar o PascalCase usado pela aplicação.
--
--   "Matriculas" ("AlunoId" uuid, "CursoId" uuid, ...)
--   "ChamadosSuporte" ("AlunoId" uuid, "UpdatedAt" timestamptz, ...)
--
-- A procedure grava a linha inteira: identificador, status ativa (1), data e carimbos.
-- Não depende de valor padrão nas colunas da migration.

-- ---------------------------------------------------------------------------
-- 1. Índices nas chaves estrangeiras
-- Um índice por coluna acelera o filtro e o join quando a consulta usa só
-- um dos lados do relacionamento.
-- ---------------------------------------------------------------------------

CREATE INDEX IF NOT EXISTS "IX_Matriculas_AlunoId"
    ON "Matriculas" ("AlunoId");

CREATE INDEX IF NOT EXISTS "IX_Matriculas_CursoId"
    ON "Matriculas" ("CursoId");

CREATE INDEX IF NOT EXISTS "IX_ChamadosSuporte_AlunoId"
    ON "ChamadosSuporte" ("AlunoId");

-- Garante a regra de negócio sob concorrência: o mesmo aluno não entra
-- duas vezes no mesmo curso. O índice de AlunoId acima continua explícito,
-- como pedido; este índice único é a validação definitiva da duplicidade.
CREATE UNIQUE INDEX IF NOT EXISTS "UX_Matriculas_AlunoId_CursoId"
    ON "Matriculas" ("AlunoId", "CursoId");

-- ---------------------------------------------------------------------------
-- 2. Matrícula em lote
-- Uso:
--   CALL sp_MatricularAlunoEmLote(
--       '00000000-0000-0000-0000-000000000001'::uuid,
--       ARRAY[
--           '00000000-0000-0000-0000-000000000010'::uuid,
--           '00000000-0000-0000-0000-000000000011'::uuid
--       ]
--   );
-- ---------------------------------------------------------------------------

CREATE OR REPLACE PROCEDURE sp_MatricularAlunoEmLote(
    p_AlunoId uuid,
    p_CursoIds uuid[]
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_inseridas integer;
BEGIN
    IF p_AlunoId IS NULL THEN
        RAISE EXCEPTION 'p_AlunoId é obrigatório.';
    END IF;

    IF p_CursoIds IS NULL
        OR NOT EXISTS (
            SELECT 1
            FROM unnest(p_CursoIds) AS curso_id
            WHERE curso_id IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'p_CursoIds deve conter ao menos um curso.';
    END IF;

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
END;
$$;

-- ---------------------------------------------------------------------------
-- 3. Auditoria de atualização em ChamadosSuporte
-- BEFORE UPDATE grava NOW() em UpdatedAt antes de o registro ser persistido.
-- ---------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION fn_chamados_suporte_set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW."UpdatedAt" := NOW();
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS tr_chamados_suporte_set_updated_at ON "ChamadosSuporte";

CREATE TRIGGER tr_chamados_suporte_set_updated_at
    BEFORE UPDATE ON "ChamadosSuporte"
    FOR EACH ROW
    EXECUTE FUNCTION fn_chamados_suporte_set_updated_at();

-- ---------------------------------------------------------------------------
-- 4. Papel de aplicação, sem superusuário e sem DELETE
-- A senha abaixo é só de desenvolvimento, no mesmo espírito do placeholder
-- já usado pela API local. Em produção ela é trocada fora do repositório.
-- ---------------------------------------------------------------------------

DO $role$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'alunos_tech_app') THEN
        CREATE ROLE alunos_tech_app LOGIN PASSWORD 'app_dev_senha';
    ELSE
        ALTER ROLE alunos_tech_app LOGIN PASSWORD 'app_dev_senha';
    END IF;
END
$role$;

GRANT USAGE ON SCHEMA public TO alunos_tech_app;
GRANT SELECT, INSERT, UPDATE ON TABLE
    "Alunos", "Cursos", "Matriculas", "ChamadosSuporte"
    TO alunos_tech_app;
REVOKE DELETE ON TABLE
    "Alunos", "Cursos", "Matriculas", "ChamadosSuporte"
    FROM alunos_tech_app;
GRANT EXECUTE ON PROCEDURE sp_MatricularAlunoEmLote(uuid, uuid[]) TO alunos_tech_app;
