using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace CP1_CursoTec.Controllers;

/// <summary>
/// Consulta e cadastro de professores. Usa o repositório genérico <see cref="IRepository{T}"/>.
/// </summary>
[ApiController]
[ApiVersionNeutral]
[Route("api/professores")]
[Produces("application/json")]
public class ProfessoresController(IRepository<Professor> repository) : ControllerBase
{
    /// <summary>Lista todos os professores.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProfessorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProfessorResponse>>> GetAll()
    {
        var professores = await repository.GetAllAsync();
        return Ok(professores.Select(ToResponse));
    }

    /// <summary>Busca um professor pelo id.</summary>
    /// <param name="id">Identificador (GUID) do professor.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProfessorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProfessorResponse>> GetById(Guid id)
    {
        var professor = await repository.GetByIdAsync(id)
                        ?? throw new ResourceNotFoundException(nameof(Professor), id);

        return Ok(ToResponse(professor));
    }

    /// <summary>Cadastra um professor.</summary>
    /// <remarks>
    /// Exemplo de corpo:
    ///
    ///     {
    ///       "nome": "Maria Souza",
    ///       "email": "maria.souza@cursotec.com",
    ///       "especialidade": "Banco de Dados"
    ///     }
    ///
    /// O e-mail é único: repetir um e-mail existente retorna 409.
    /// </remarks>
    /// <param name="request">Dados do professor.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ProfessorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProfessorResponse>> Create(ProfessorRequest request)
    {
        var professor = new Professor(request.Nome, request.Email, request.Especialidade);
        await repository.AddAsync(professor);

        return CreatedAtAction(nameof(GetById), new { id = professor.Id }, ToResponse(professor));
    }

    private static ProfessorResponse ToResponse(Professor professor) =>
        new(professor.Id, professor.Nome, professor.Email, professor.Especialidade);
}
