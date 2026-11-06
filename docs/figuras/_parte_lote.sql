\pset pager off
BEGIN;
INSERT INTO "Alunos" ("Id", "Nome", "Email", "Cargo", "Ativo", "CreatedAt", "UpdatedAt")
VALUES ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Ensaio da figura', 'ensaio-figura@alunostech.local', NULL, TRUE, NOW(), NOW());
CALL sp_MatricularAlunoEmLote(
    'cccccccc-cccc-cccc-cccc-cccccccccccc',
    ARRAY['55555555-5555-5555-5555-555555555555']::uuid[]
);
CALL sp_MatricularAlunoEmLote(
    'cccccccc-cccc-cccc-cccc-cccccccccccc',
    ARRAY['55555555-5555-5555-5555-555555555555']::uuid[]
);
ROLLBACK;
