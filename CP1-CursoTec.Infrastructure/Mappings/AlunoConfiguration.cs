using CP1_CursoTec.Domain.Entities;
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

        // Relacionamento: Aluno -> Turma (opcional)
        builder.HasOne(a => a.Turma)
            .WithMany(t => t.Alunos)
            .HasForeignKey("TurmaId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}