using CP1_CursoTec.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CP1_CursoTec.Infrastructure.Mappings;

public class AulaConfiguration : IEntityTypeConfiguration<Aula>
{
    public void Configure(EntityTypeBuilder<Aula> builder)
    {
        builder.ToTable("Aulas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Data)
            .IsRequired();

        builder.Property(a => a.HoraInicio)
            .IsRequired();

        builder.Property(a => a.HoraFim)
            .IsRequired();

        // Relacionamento: Aula -> Turma (obrigatório)
        builder.HasOne(a => a.Turma)
            .WithMany(t => t.Aulas)
            .HasForeignKey(a => a.TurmaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}