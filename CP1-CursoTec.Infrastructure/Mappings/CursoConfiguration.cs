using CP1_CursoTec.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CP1_CursoTec.Infrastructure.Mappings;

public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("Cursos");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.CargaHoraria)
            .IsRequired();

        builder.Property(c => c.Descricao)
            .HasMaxLength(500);

        builder.HasIndex(c => c.Nome)
            .IsUnique();
    }
}