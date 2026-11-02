using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CP1_CursoTec.Infrastructure.Mappings;

public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> builder)
    {
        builder.ToTable("Alunos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(a => a.Email)
            .IsUnique();

        builder.Property(a => a.Cpf)
            .IsRequired()
            .HasMaxLength(14);

        builder.HasIndex(a => a.Cpf)
            .IsUnique();

        builder.Property(a => a.DataNascimento)
            .IsRequired();

        // CP6 — login: só o hash da senha é salvo (formato "iteracoes.salt.hash", ~70 caracteres).
        builder.Property(a => a.SenhaHash)
            .IsRequired()
            .HasMaxLength(200);

        // CP6 — papel do usuário salvo como texto ("Aluno"/"Admin") em vez de número.
        // O valor padrão garante que alunos já existentes no banco fiquem como "Aluno".
        builder.Property(a => a.Papel)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PapelUsuario.Aluno);

        builder.HasOne(a => a.Turma)
            .WithMany(t => t.Alunos)
            .HasForeignKey("TurmaId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}