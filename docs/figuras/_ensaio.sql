INSERT INTO "Cursos" ("Id", "Titulo", "Descricao", "CargaHoraria", "Ativo", "CreatedAt", "UpdatedAt")
VALUES ('33333333-3333-3333-3333-333333333333', 'Curso do portal local', NULL, 4, TRUE, NOW(), NOW())
ON CONFLICT ("Id") DO NOTHING;

\d "Alunos"
\d "Cursos"
\d "Matriculas"
\d "ChamadosSuporte"

BEGIN;
INSERT INTO "Alunos" ("Id", "Nome", "Email", "Cargo", "Ativo", "CreatedAt", "UpdatedAt")
VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Ensaio Lote', 'ensaio-lote@alunostech.local', NULL, TRUE, NOW(), NOW())
ON CONFLICT ("Id") DO NOTHING;
CALL sp_MatricularAlunoEmLote(
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    ARRAY['33333333-3333-3333-3333-333333333333']::uuid[]
);
CALL sp_MatricularAlunoEmLote(
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    ARRAY['33333333-3333-3333-3333-333333333333']::uuid[]
);
ROLLBACK;
