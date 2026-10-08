using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CP1_CursoTec.Controllers;

/// <summary>
/// CRUD de cursos. Usa o repositório genérico <see cref="IRepository{T}"/>.
/// </summary>
[ApiController]
[Route("api/cursos")]
[Produces("application/json")]
public class CursosController(IRepository<Curso> repository, ILogger<CursosController> logger) : ControllerBase
{
    /// <summary>Lista todos os cursos.</summary>
    /// <returns>Lista de cursos cadastrados.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CursoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CursoResponse>>> GetAll()
    {
        var cursos = await repository.GetAllAsync();
        return Ok(cursos.Select(ToResponse));
    }

    /// <summary>Busca um curso pelo id.</summary>
    /// <param name="id">Identificador (GUID) do curso.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CursoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CursoResponse>> GetById(Guid id)
    {
        var curso = await repository.GetByIdAsync(id)
                    ?? throw new ResourceNotFoundException(nameof(Curso), id);

        return Ok(ToResponse(curso));
    }

    /// <summary>Cria um curso.</summary>
    /// <remarks>
    /// Exemplo de corpo:
    ///
    ///     {
    ///       "nome": "Desenvolvimento de Sistemas",
    ///       "cargaHoraria": 1200,
    ///       "descricao": "Curso técnico de desenvolvimento de sistemas"
    ///     }
    ///
    /// O nome do curso é único: repetir um nome existente retorna 409.
    /// </remarks>
    /// <param name="request">Dados do curso.</param>
    [HttpPost]
    [ProducesResponseType(typeof(CursoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CursoResponse>> Create(CursoRequest request)
    {
        logger.LogInformation(
            "Iniciando criação de curso {Nome}. TraceId={TraceId}",
            request.Nome, HttpContext.TraceIdentifier);

        var curso = new Curso(request.Nome, request.CargaHoraria, request.Descricao);
        await repository.AddAsync(curso);

        logger.LogInformation(
            "Curso {CursoId} criado com sucesso. TraceId={TraceId}",
            curso.Id, HttpContext.TraceIdentifier);

        return CreatedAtAction(nameof(GetById), new { id = curso.Id }, ToResponse(curso));
    }

    /// <summary>Atualiza um curso existente.</summary>
    /// <param name="id">Identificador (GUID) do curso.</param>
    /// <param name="request">Novos dados do curso.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CursoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CursoResponse>> Update(Guid id, CursoRequest request)
    {
        logger.LogInformation(
            "Iniciando atualização do curso {CursoId}. TraceId={TraceId}",
            id, HttpContext.TraceIdentifier);

        var curso = await repository.GetByIdAsync(id)
                    ?? throw new ResourceNotFoundException(nameof(Curso), id);

        curso.UpdateNome(request.Nome);
        curso.UpdateCargaHoraria(request.CargaHoraria);
        curso.UpdateDescricao(request.Descricao);

        await repository.UpdateAsync(curso);

        logger.LogInformation(
            "Curso {CursoId} atualizado com sucesso. TraceId={TraceId}",
            curso.Id, HttpContext.TraceIdentifier);

        return Ok(ToResponse(curso));
    }

    /// <summary>Remove um curso.</summary>
    /// <remarks>As turmas do curso permanecem, sem curso associado.</remarks>
    /// <param name="id">Identificador (GUID) do curso.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        logger.LogInformation(
            "Iniciando remoção do curso {CursoId}. TraceId={TraceId}",
            id, HttpContext.TraceIdentifier);

        var curso = await repository.GetByIdAsync(id)
                    ?? throw new ResourceNotFoundException(nameof(Curso), id);

        await repository.DeleteAsync(curso);

        logger.LogInformation(
            "Curso {CursoId} removido com sucesso. TraceId={TraceId}",
            id, HttpContext.TraceIdentifier);

        return NoContent();
    }

    private static CursoResponse ToResponse(Curso curso) =>
        new(curso.Id, curso.Nome, curso.CargaHoraria, curso.Descricao);
}
