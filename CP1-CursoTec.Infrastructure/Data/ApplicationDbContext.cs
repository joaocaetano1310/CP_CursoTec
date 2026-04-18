using Microsoft.EntityFrameworkCore;
using CP1_CursoTec.Domain.Entities;

namespace CP1_CursoTec.Infrastructure.Data;

// 1. IMPORTANTE: A classe PRECISA ser 'public' e herdar de 'DbContext'
public class ApplicationDbContext : DbContext
{
    // 2. O construtor precisa receber as opções e passar para a classe base (base)
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // 3. Os DbSets representam suas tabelas
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Aula> Aulas { get; set; }
    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Professor> Professores { get; set; }
    public DbSet<Turma> Turmas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}