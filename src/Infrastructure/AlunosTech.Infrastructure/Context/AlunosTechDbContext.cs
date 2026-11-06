using AlunosTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlunosTech.Infrastructure.Context;

public class AlunosTechDbContext : DbContext
{
    public DbSet<Aluno> Alunos => Set<Aluno>();

    public DbSet<Curso> Cursos => Set<Curso>();

    public DbSet<Matricula> Matriculas => Set<Matricula>();

    public DbSet<ChamadoSuporte> ChamadosSuporte => Set<ChamadoSuporte>();

    public AlunosTechDbContext(DbContextOptions<AlunosTechDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aluno>(ConfigurarAluno);
        modelBuilder.Entity<Curso>(ConfigurarCurso);
        modelBuilder.Entity<Matricula>(ConfigurarMatricula);
        modelBuilder.Entity<ChamadoSuporte>(ConfigurarChamadoSuporte);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigurarBase<TEntity>(EntityTypeBuilder<TEntity> entity)
        where TEntity : BaseEntity
    {
        entity.HasKey(item => item.Id);

        entity.Property(item => item.Id)
            .ValueGeneratedOnAdd();

        entity.Property(item => item.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        entity.Property(item => item.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");
    }

    private static void ConfigurarAluno(EntityTypeBuilder<Aluno> entity)
    {
        ConfigurarBase(entity);
        entity.ToTable("Alunos");

        entity.Property(aluno => aluno.Nome)
            .IsRequired()
            .HasMaxLength(Aluno.NomeMaxLength);

        entity.Property(aluno => aluno.Email)
            .IsRequired()
            .HasMaxLength(Aluno.EmailMaxLength);

        entity.Property(aluno => aluno.Cargo)
            .HasMaxLength(Aluno.CargoMaxLength);

        entity.Property(aluno => aluno.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        entity.HasMany(aluno => aluno.Matriculas)
            .WithOne(matricula => matricula.Aluno)
            .HasForeignKey(matricula => matricula.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.Navigation(aluno => aluno.Matriculas)
            .HasField("_matriculas")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        entity.HasMany(aluno => aluno.ChamadosSuporte)
            .WithOne(chamado => chamado.Aluno)
            .HasForeignKey(chamado => chamado.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.Navigation(aluno => aluno.ChamadosSuporte)
            .HasField("_chamadosSuporte")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigurarCurso(EntityTypeBuilder<Curso> entity)
    {
        ConfigurarBase(entity);
        entity.ToTable("Cursos");

        entity.Property(curso => curso.Titulo)
            .IsRequired()
            .HasMaxLength(Curso.TituloMaxLength);

        entity.Property(curso => curso.Descricao)
            .HasMaxLength(Curso.DescricaoMaxLength);

        entity.Property(curso => curso.CargaHoraria)
            .IsRequired();

        entity.Property(curso => curso.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        entity.HasMany(curso => curso.Matriculas)
            .WithOne(matricula => matricula.Curso)
            .HasForeignKey(matricula => matricula.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.Navigation(curso => curso.Matriculas)
            .HasField("_matriculas")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigurarMatricula(EntityTypeBuilder<Matricula> entity)
    {
        ConfigurarBase(entity);
        entity.ToTable("Matriculas");

        entity.Property(matricula => matricula.AlunoId)
            .IsRequired();

        entity.Property(matricula => matricula.CursoId)
            .IsRequired();

        entity.Property(matricula => matricula.Status)
            .IsRequired()
            .HasConversion<int>();

        entity.Property(matricula => matricula.DataMatricula)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        entity.HasIndex(matricula => new { matricula.AlunoId, matricula.CursoId })
            .IsUnique()
            .HasDatabaseName("UX_Matriculas_AlunoId_CursoId");
    }

    private static void ConfigurarChamadoSuporte(EntityTypeBuilder<ChamadoSuporte> entity)
    {
        ConfigurarBase(entity);
        entity.ToTable("ChamadosSuporte");

        entity.Property(chamado => chamado.AlunoId)
            .IsRequired();

        entity.Property(chamado => chamado.Titulo)
            .IsRequired()
            .HasMaxLength(ChamadoSuporte.TituloMaxLength);

        entity.Property(chamado => chamado.Descricao)
            .HasMaxLength(ChamadoSuporte.DescricaoMaxLength);

        entity.Property(chamado => chamado.Status)
            .IsRequired()
            .HasConversion<int>();

        entity.Property(chamado => chamado.Prioridade)
            .IsRequired()
            .HasConversion<int>();
    }
}
