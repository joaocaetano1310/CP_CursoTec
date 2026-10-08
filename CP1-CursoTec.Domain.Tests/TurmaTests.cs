using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;

namespace CP1_CursoTec.Domain.Tests;

public class TurmaTests
{
    private static readonly DateTime Inicio = new(2026, 2, 2);
    private static readonly DateTime Fim = new(2026, 12, 18);

    [Fact]
    public void Construtor_DadosValidos_CriaTurmaComProfessorECurso()
    {
        // Arrange
        var professorId = Guid.NewGuid();
        var curso = new Curso("Redes", 800, null);

        // Act
        var turma = new Turma("2TDS-A", Inicio, Fim, professorId, curso);

        // Assert
        Assert.Equal("2TDS-A", turma.Nome_turma);
        Assert.Equal(Inicio, turma.DataInicio);
        Assert.Equal(Fim, turma.DataFim);
        Assert.Equal(professorId, turma.ProfessorId);
        Assert.Same(curso, turma.Curso);
        Assert.Empty(turma.Alunos);
        Assert.Empty(turma.Aulas);
    }

    [Fact]
    public void Construtor_ProfessorIdVazio_LancaDomainException()
    {
        // Arrange
        var professorIdVazio = Guid.Empty;

        // Act
        var excecao = Assert.Throws<DomainException>(
            () => new Turma("2TDS-A", Inicio, Fim, professorIdVazio, null));

        // Assert
        Assert.Equal("Professor é obrigatório.", excecao.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(365)]
    public void Construtor_DataFimAnteriorAoInicio_LancaDomainException(int diasAntes)
    {
        // Arrange
        var fimInvalido = Inicio.AddDays(-diasAntes);

        // Act
        var excecao = Assert.Throws<DomainException>(
            () => new Turma("2TDS-A", Inicio, fimInvalido, Guid.NewGuid(), null));

        // Assert
        Assert.Equal("A data de fim não pode ser anterior à data de início.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateNome_NomeVazio_LancaDomainException(string nome)
    {
        // Arrange
        var turma = new Turma("2TDS-A", Inicio, null, Guid.NewGuid(), null);

        // Act
        var excecao = Assert.Throws<DomainException>(() => turma.UpdateNome(nome));

        // Assert
        Assert.Equal("Nome não pode ser vazio.", excecao.Message);
        Assert.Equal("2TDS-A", turma.Nome_turma);
    }
}
