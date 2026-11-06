\pset pager off
\echo === TABELAS ===
\dt "Alunos"
\dt "Cursos"
\dt "Matriculas"
\dt "ChamadosSuporte"
\echo === ALUNOS ===
\d "Alunos"
\echo === CURSOS ===
\d "Cursos"
\echo === MATRICULAS ===
\d "Matriculas"
\echo === CHAMADOS ===
\d "ChamadosSuporte"
\echo === LOTE ===
BEGIN;
INSERT INTO "Alunos" ("Id", "Nome", "Email", "Cargo", "Ativo", "CreatedAt", "UpdatedAt")
VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Ensaio da captura', 'ensaio-captura@alunostech.local', NULL, TRUE, NOW(), NOW());
CALL sp_MatricularAlunoEmLote(
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
    ARRAY['55555555-5555-5555-5555-555555555555']::uuid[]
);
CALL sp_MatricularAlunoEmLote(
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
    ARRAY['55555555-5555-5555-5555-555555555555']::uuid[]
);
ROLLBACK;
