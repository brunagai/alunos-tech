INSERT INTO "Cursos" ("Id", "Titulo", "CargaHoraria", "Ativo", "CreatedAt", "UpdatedAt")
VALUES ('55555555-5555-5555-5555-555555555555', 'Curso do aplicativo', 2, TRUE, NOW(), NOW())
ON CONFLICT ("Id") DO NOTHING;
