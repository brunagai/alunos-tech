INSERT INTO "Cursos" ("Id", "Titulo", "CargaHoraria", "Ativo", "CreatedAt", "UpdatedAt")
VALUES ('44444444-4444-4444-4444-444444444444', 'Curso da captura', 2, TRUE, NOW(), NOW())
ON CONFLICT ("Id") DO NOTHING;
