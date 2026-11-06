CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105212032_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261105212032_InitialCreate', '8.0.11');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105212512_AddEntitiesAndMappings') THEN
    CREATE TABLE "Alunos" (
        "Id" uuid NOT NULL,
        "Nome" character varying(150) NOT NULL,
        "Email" character varying(150) NOT NULL,
        "Cargo" character varying(100),
        "Ativo" boolean NOT NULL DEFAULT TRUE,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Alunos" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105212512_AddEntitiesAndMappings') THEN
    CREATE TABLE "Cursos" (
        "Id" uuid NOT NULL,
        "Titulo" character varying(200) NOT NULL,
        "Descricao" character varying(1000),
        "CargaHoraria" integer NOT NULL,
        "Ativo" boolean NOT NULL DEFAULT TRUE,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Cursos" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105212512_AddEntitiesAndMappings') THEN
    CREATE TABLE "ChamadosSuporte" (
        "Id" uuid NOT NULL,
        "AlunoId" uuid NOT NULL,
        "Titulo" character varying(150) NOT NULL,
        "Descricao" character varying(2000),
        "Status" integer NOT NULL,
        "Prioridade" integer NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_ChamadosSuporte" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ChamadosSuporte_Alunos_AlunoId" FOREIGN KEY ("AlunoId") REFERENCES "Alunos" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105212512_AddEntitiesAndMappings') THEN
    CREATE TABLE "Matriculas" (
        "Id" uuid NOT NULL,
        "AlunoId" uuid NOT NULL,
        "CursoId" uuid NOT NULL,
        "Status" integer NOT NULL,
        "DataMatricula" timestamp with time zone NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Matriculas" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Matriculas_Alunos_AlunoId" FOREIGN KEY ("AlunoId") REFERENCES "Alunos" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Matriculas_Cursos_CursoId" FOREIGN KEY ("CursoId") REFERENCES "Cursos" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105212512_AddEntitiesAndMappings') THEN
    CREATE INDEX "IX_ChamadosSuporte_AlunoId" ON "ChamadosSuporte" ("AlunoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105212512_AddEntitiesAndMappings') THEN
    CREATE INDEX "IX_Matriculas_CursoId" ON "Matriculas" ("CursoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105212512_AddEntitiesAndMappings') THEN
    CREATE UNIQUE INDEX "UX_Matriculas_AlunoId_CursoId" ON "Matriculas" ("AlunoId", "CursoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105212512_AddEntitiesAndMappings') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261105212512_AddEntitiesAndMappings', '8.0.11');
    END IF;
END $EF$;
COMMIT;

