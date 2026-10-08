using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Application.Services;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CP1_CursoTec.Controllers;

/// <summary>
/// Consulta e criação de turmas. A leitura usa <see cref="ITurmaRepository"/>
/// (carrega curso, professor e alunos); a criação é delegada ao <see cref="ITurmaService"/>.
/// </summary>
[ApiController]
[Route("api/turmas")]
[Produces("application/json")]
public class TurmasController(
    ITurmaRepository turmaRepository,
    ITurmaService turmaService,
    ILogger<TurmasController> logger) : ControllerBase
{
    /// <summary>Lista todas as turmas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TurmaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TurmaResponse>>> GetAll()
    {
        var turmas = await turmaRepository.GetAllAsync();
        return Ok(turmas.Select(ToResponse));
    }

    /// <summary>Busca uma turma pelo id.</summary>
    /// <param name="id">Identificador (GUID) da turma.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TurmaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TurmaResponse>> GetById(Guid id)
    {
        var turma = await turmaRepository.GetByIdAsync(id)
                    ?? throw new ResourceNotFoundException(nameof(Turma), id);

        return Ok(ToResponse(turma));
    }

    /// <summary>Cria uma turma.</summary>
    /// <remarks>
    /// Exemplo de corpo (use ids existentes de professor e, opcionalmente, de curso):
    ///
    ///     {
    ///       "nome": "2TDS-A",
    ///       "dataInicio": "2026-02-02T00:00:00",
    ///       "dataFim": "2026-12-18T00:00:00",
    ///       "professorId": "00000000-0000-0000-0000-000000000000",
    ///       "cursoId": null
    ///     }
    ///
    /// Retorna 404 se o professor (ou o curso informado) não existir e 400 se as datas forem incoerentes.
    /// </remarks>
    /// <param name="request">Dados da turma.</param>
    [HttpPost]
    [ProducesResponseType(typeof(TurmaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TurmaResponse>> Create(TurmaRequest request)
    {
        logger.LogInformation(
            "Iniciando criação de turma {Nome} para o professor {ProfessorId}. TraceId={TraceId}",
            request.Nome, request.ProfessorId, HttpContext.TraceIdentifier);

        var turma = await turmaService.CriarAsync(request);

        logger.LogInformation(
            "Turma {TurmaId} criada com sucesso. TraceId={TraceId}",
            turma.Id, HttpContext.TraceIdentifier);

        return CreatedAtAction(nameof(GetById), new { id = turma.Id }, ToResponse(turma));
    }

    private static TurmaResponse ToResponse(Turma turma) =>
        new(turma.Id,
            turma.Nome_turma,
            turma.DataInicio,
            turma.DataFim,
            turma.ProfessorId,
            turma.Professor?.Nome,
            turma.Curso?.Nome,
            turma.Alunos.Count);
}
