using CP1_CursoTec.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CP1_CursoTec.Infrastructure.Mappings;

public class TurmaConfiguration : IEntityTypeConfiguration<Turma>
{
    public void Configure(EntityTypeBuilder<Turma> builder)
    {
        builder.ToTable("Turmas");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Nome_turma)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.DataInicio)
            .IsRequired();

        builder.Property(t => t.DataFim)
            .IsRequired(false);

        builder.HasOne(t => t.Professor)
            .WithMany(p => p.Turmas)
            .HasForeignKey(t => t.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Curso)
            .WithMany(c => c.Turmas)
            .HasForeignKey("CursoId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(t => t.ProfessorId);
    }
}