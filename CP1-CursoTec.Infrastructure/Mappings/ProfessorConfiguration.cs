using CP1_CursoTec.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CP1_CursoTec.Infrastructure.Mappings;

public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.ToTable("Professores");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(p => p.Email)
            .IsUnique();

        builder.Property(p => p.Especialidade)
            .HasMaxLength(150);

        // Relacionamento: Professor -> Turma (1:1)
        builder.HasOne(p => p.Turma)
            .WithOne(t => t.Professor)
            .HasForeignKey<Turma>(t => t.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}