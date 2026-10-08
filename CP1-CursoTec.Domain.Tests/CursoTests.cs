using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;

namespace CP1_CursoTec.Domain.Tests;

public class CursoTests
{
    [Fact]
    public void Construtor_DadosValidos_CriaCursoComValoresInformados()
    {
        // Arrange
        const string nome = "Desenvolvimento de Sistemas";
        const int cargaHoraria = 1200;
        const string descricao = "Curso técnico";

        // Act
        var curso = new Curso(nome, cargaHoraria, descricao);

        // Assert
        Assert.Equal(nome, curso.Nome);
        Assert.Equal(cargaHoraria, curso.CargaHoraria);
        Assert.Equal(descricao, curso.Descricao);
        Assert.True(curso.Active);
        Assert.NotEqual(Guid.Empty, curso.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_NomeVazioOuEmBranco_LancaDomainException(string? nome)
    {
        // Arrange
        const int cargaHoraria = 800;

        // Act
        var excecao = Assert.Throws<DomainException>(() => new Curso(nome!, cargaHoraria, null));

        // Assert
        Assert.Equal("Nome não pode ser vazio.", excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-800)]
    public void UpdateCargaHoraria_ValorNaoPositivo_LancaDomainException(int cargaHoraria)
    {
        // Arrange
        var curso = new Curso("Redes", 800, null);

        // Act
        var excecao = Assert.Throws<DomainException>(() => curso.UpdateCargaHoraria(cargaHoraria));

        // Assert
        Assert.Equal("A carga horária deve ser maior que zero.", excecao.Message);
        Assert.Equal(800, curso.CargaHoraria); // valor anterior preservado
    }
}
