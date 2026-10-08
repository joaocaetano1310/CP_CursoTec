using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;

namespace CP1_CursoTec.Domain.Tests;

public class AulaTests
{
    [Fact]
    public void Construtor_HorariosValidos_CriaAula()
    {
        // Arrange
        var turmaId = Guid.NewGuid();
        var data = new DateTime(2026, 3, 10);
        var inicio = new TimeOnly(19, 0);
        var fim = new TimeOnly(22, 30);

        // Act
        var aula = new Aula(turmaId, data, inicio, fim);

        // Assert
        Assert.Equal(turmaId, aula.TurmaId);
        Assert.Equal(data, aula.Data);
        Assert.Equal(inicio, aula.HoraInicio);
        Assert.Equal(fim, aula.HoraFim);
    }

    [Theory]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    [InlineData(23, 0)]
    public void Construtor_HoraFimNaoPosteriorAoInicio_LancaDomainException(int horaInicio, int horaFim)
    {
        // Arrange
        var inicio = new TimeOnly(horaInicio, 0);
        var fim = new TimeOnly(horaFim, 0);

        // Act
        var excecao = Assert.Throws<DomainException>(
            () => new Aula(Guid.NewGuid(), new DateTime(2026, 3, 10), inicio, fim));

        // Assert
        Assert.Equal("A hora de fim deve ser posterior à hora de início.", excecao.Message);
    }

    [Fact]
    public void Construtor_TurmaIdVazio_LancaDomainException()
    {
        // Arrange
        var turmaIdVazio = Guid.Empty;

        // Act
        var excecao = Assert.Throws<DomainException>(
            () => new Aula(turmaIdVazio, new DateTime(2026, 3, 10), new TimeOnly(19, 0), new TimeOnly(22, 0)));

        // Assert
        Assert.Equal("Turma é obrigatória.", excecao.Message);
    }
}
