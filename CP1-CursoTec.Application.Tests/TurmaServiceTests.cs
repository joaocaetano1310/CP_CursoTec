using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Application.Services;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;
using Moq;

namespace CP1_CursoTec.Application.Tests;

/// <summary>
/// Testes do TurmaService com as interfaces de repositório simuladas (Moq).
/// Nenhum teste sobe a API nem acessa banco de dados.
/// </summary>
public class TurmaServiceTests
{
    private readonly Mock<ITurmaRepository> _turmaRepository = new();
    private readonly Mock<IRepository<Professor>> _professorRepository = new();
    private readonly Mock<IRepository<Curso>> _cursoRepository = new();
    private readonly TurmaService _service;

    public TurmaServiceTests()
    {
        _service = new TurmaService(
            _turmaRepository.Object,
            _professorRepository.Object,
            _cursoRepository.Object);
    }

    private static TurmaRequest CriarRequest(Guid? professorId, Guid? cursoId = null) => new()
    {
        Nome = "2TDS-A",
        DataInicio = new DateTime(2026, 2, 2),
        DataFim = new DateTime(2026, 12, 18),
        ProfessorId = professorId,
        CursoId = cursoId
    };

    private static Professor CriarProfessor() =>
        new("Maria Souza", "maria.souza@cursotec.com", "Banco de Dados");

    [Fact]
    public async Task CriarAsync_ProfessorInexistente_LancaResourceNotFoundENaoPersiste()
    {
        // Arrange
        var professorId = Guid.NewGuid();
        _professorRepository
            .Setup(r => r.GetByIdAsync(professorId))
            .ReturnsAsync((Professor?)null);

        // Act
        var excecao = await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _service.CriarAsync(CriarRequest(professorId)));

        // Assert
        Assert.Contains(professorId.ToString(), excecao.Message);
        _turmaRepository.Verify(r => r.AddAsync(It.IsAny<Turma>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_CursoInformadoInexistente_LancaResourceNotFoundENaoPersiste()
    {
        // Arrange
        var professor = CriarProfessor();
        var cursoId = Guid.NewGuid();
        _professorRepository
            .Setup(r => r.GetByIdAsync(professor.Id))
            .ReturnsAsync(professor);
        _cursoRepository
            .Setup(r => r.GetByIdAsync(cursoId))
            .ReturnsAsync((Curso?)null);

        // Act
        var excecao = await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _service.CriarAsync(CriarRequest(professor.Id, cursoId)));

        // Assert
        Assert.Contains(cursoId.ToString(), excecao.Message);
        _turmaRepository.Verify(r => r.AddAsync(It.IsAny<Turma>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_ProfessorNaoInformado_LancaDomainExceptionSemConsultarNemPersistir()
    {
        // Arrange
        var request = CriarRequest(professorId: null);

        // Act
        var excecao = await Assert.ThrowsAsync<DomainException>(() => _service.CriarAsync(request));

        // Assert
        Assert.Equal("Professor é obrigatório.", excecao.Message);
        _professorRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _turmaRepository.Verify(r => r.AddAsync(It.IsAny<Turma>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DataFimAnteriorAoInicio_LancaDomainExceptionENaoPersiste()
    {
        // Arrange
        var professor = CriarProfessor();
        _professorRepository
            .Setup(r => r.GetByIdAsync(professor.Id))
            .ReturnsAsync(professor);

        var request = CriarRequest(professor.Id);
        request.DataFim = request.DataInicio.AddDays(-1);

        // Act
        await Assert.ThrowsAsync<DomainException>(() => _service.CriarAsync(request));

        // Assert
        _turmaRepository.Verify(r => r.AddAsync(It.IsAny<Turma>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_PersisteUmaVezERetornaTurma()
    {
        // Arrange
        var professor = CriarProfessor();
        _professorRepository
            .Setup(r => r.GetByIdAsync(professor.Id))
            .ReturnsAsync(professor);
        _turmaRepository
            .Setup(r => r.AddAsync(It.IsAny<Turma>()))
            .Returns(Task.CompletedTask);

        // Act
        var turma = await _service.CriarAsync(CriarRequest(professor.Id));

        // Assert
        Assert.Equal("2TDS-A", turma.Nome_turma);
        Assert.Equal(professor.Id, turma.ProfessorId);
        Assert.Null(turma.Curso);
        _turmaRepository.Verify(
            r => r.AddAsync(It.Is<Turma>(t => t.ProfessorId == professor.Id && t.Nome_turma == "2TDS-A")),
            Times.Once);
        _cursoRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DadosValidosComCurso_AssociaCursoEPersisteUmaVez()
    {
        // Arrange
        var professor = CriarProfessor();
        var curso = new Curso("Desenvolvimento de Sistemas", 1200, null);
        _professorRepository
            .Setup(r => r.GetByIdAsync(professor.Id))
            .ReturnsAsync(professor);
        _cursoRepository
            .Setup(r => r.GetByIdAsync(curso.Id))
            .ReturnsAsync(curso);

        // Act
        var turma = await _service.CriarAsync(CriarRequest(professor.Id, curso.Id));

        // Assert
        Assert.Same(curso, turma.Curso);
        _turmaRepository.Verify(r => r.AddAsync(It.IsAny<Turma>()), Times.Once);
    }
}
