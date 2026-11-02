using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Application.Services;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;
using Moq;

namespace CP1_CursoTec.Application.Tests;

/// <summary>
/// Testes da regra de paginação do TurmaService, com repositórios simulados (Moq).
/// Nenhum teste sobe a API nem acessa banco de dados.
/// </summary>
public class TurmaServicePaginacaoTests
{
    private readonly Mock<ITurmaRepository> _turmaRepository = new();
    private readonly Mock<IRepository<Professor>> _professorRepository = new();
    private readonly Mock<IRepository<Curso>> _cursoRepository = new();
    private readonly TurmaService _service;

    public TurmaServicePaginacaoTests()
    {
        _service = new TurmaService(
            _turmaRepository.Object,
            _professorRepository.Object,
            _cursoRepository.Object);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    [InlineData(1, 9999)]
    public async Task ListarPaginadoAsync_ParametrosInvalidos_LancaDomainExceptionSemConsultarRepositorio(
        int page, int pageSize)
    {
        // Arrange / Act
        await Assert.ThrowsAsync<DomainException>(
            () => _service.ListarPaginadoAsync(page, pageSize));

        // Assert
        _turmaRepository.Verify(
            r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task ListarPaginadoAsync_IntervaloValido_CalculaTotalPagesEConsultaUmaVez()
    {
        // Arrange
        _turmaRepository
            .Setup(r => r.GetPagedAsync(1, 2))
            .ReturnsAsync(((IReadOnlyList<Turma>)new List<Turma>(), 5));

        // Act
        var resultado = await _service.ListarPaginadoAsync(1, 2);

        // Assert
        Assert.Equal(3, resultado.TotalPages);   // teto de 5 / 2
        Assert.Equal(5, resultado.TotalItems);
        Assert.Equal(1, resultado.Page);
        Assert.Equal(2, resultado.PageSize);
        _turmaRepository.Verify(r => r.GetPagedAsync(1, 2), Times.Once);
    }
}