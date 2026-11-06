INSERT INTO "Alunos" ("Id", "Nome", "Email", "Cargo", "Ativo", "CreatedAt", "UpdatedAt")
VALUES (
    '11111111-1111-1111-1111-111111111111',
    'Colaborador Local',
    'colaborador@alunostech.local',
    'Analista',
    true,
    NOW(),
    NOW()
)
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Cursos" ("Id", "Titulo", "Descricao", "CargaHoraria", "Ativo", "CreatedAt", "UpdatedAt")
VALUES (
    '22222222-2222-2222-2222-222222222222',
    'Curso local do cluster',
    'Curso usado na chamada do aplicativo.',
    8,
    true,
    NOW(),
    NOW()
)
ON CONFLICT ("Id") DO NOTHING;
