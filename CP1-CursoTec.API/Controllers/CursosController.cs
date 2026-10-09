using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;
using CP1_CursoTec.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;

namespace CP1_CursoTec.Controllers;

/// <summary>
/// CRUD de cursos. Usa o repositório genérico <see cref="IRepository{T}"/>.
/// listagem em duas versões: v1 (obsoleta, lista simples) e v2 (paginada, com rate limit).
/// </summary>
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Route("api/cursos")]
[Produces("application/json")]
public class CursosController(IRepository<Curso> repository, ILogger<CursosController> logger) : ControllerBase
{
    private const int MaxPageSize = 50;

    /// <summary>Lista todos os cursos (v1, obsoleta: use a v2).</summary>
    /// <returns>Lista de cursos cadastrados.</returns>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(IEnumerable<CursoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CursoResponse>>> GetAll()
    {
        var cursos = await repository.GetAllAsync();
        return Ok(cursos.Select(ToResponse));
    }

    /// <summary>Lista cursos com paginação (v2).</summary>
    /// <remarks>
    /// Limite de 5 requisições a cada 30 segundos. Ao exceder, retorna 429 com o cabeçalho Retry-After.
    /// </remarks>
    /// <param name="page">Número da página, começando em 1 (padrão 1).</param>
    /// <param name="pageSize">Itens por página, de 1 a 50 (padrão 10).</param>
    /// <returns>Página de cursos com totalItems e totalPages.</returns>
    [HttpGet]
    [MapToApiVersion("2.0")]
    [EnableRateLimiting("listagem")]
    [ProducesResponseType(typeof(PagedResponse<CursoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResponse<CursoResponse>>> GetAllPaged(
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, MaxPageSize)] int pageSize = 10)
    {
        var (cursos, totalItems) = await repository.GetPagedAsync(page, pageSize);
        var items = cursos.Select(ToResponse).ToList();

        return Ok(PagedResponse<CursoResponse>.Create(items, page, pageSize, totalItems));
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