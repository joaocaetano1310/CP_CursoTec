using Asp.Versioning;
using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CP1_CursoTec.Controllers;

/// <summary>
/// Histórico de recomendação (CP6), guardado no MongoDB.
/// Todas as operações exigem token. O controller não fala com o driver:
/// delega ao <see cref="IRecommendationLogService"/>.
/// </summary>
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/recommendation-logs")]
[Produces("application/json")]
[Authorize]
public class RecommendationLogsController(IRecommendationLogService service) : ControllerBase
{
    /// <summary>Cria um log de recomendação.</summary>
    /// <remarks>
    /// Exemplo de corpo:
    ///
    ///     {
    ///       "userId": "11111111-1111-1111-1111-111111111111",
    ///       "query": "filme de terror curto",
    ///       "titles": ["A Bruxa", "Hereditário"]
    ///     }
    ///
    /// Retorna 400 se query estiver vazia, titles ausente/vazio/com item em branco ou userId vazio.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(RecommendationLogDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RecommendationLogDto>> Create(CreateRecommendationLogRequest request)
    {
        var log = await service.CriarAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = log.Id, version = "2.0" }, log);
    }

    /// <summary>Busca um log pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RecommendationLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecommendationLogDto>> GetById(Guid id)
    {
        return Ok(await service.ObterPorIdAsync(id));
    }

    /// <summary>Lista os logs de um usuário (mais novos primeiro, no máximo 20).</summary>
    /// <remarks>Usuário sem logs devolve 200 com array vazio.</remarks>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<RecommendationLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<RecommendationLogDto>>> ListByUser(Guid userId)
    {
        return Ok(await service.ListarPorUsuarioAsync(userId));
    }

    /// <summary>Atualiza query e titles de um log.</summary>
    /// <remarks>
    /// Exemplo de corpo (não envia userId):
    ///
    ///     {
    ///       "query": "filme de terror curto, dublado",
    ///       "titles": ["A Bruxa", "Hereditário", "O Babadook"]
    ///     }
    ///
    /// userId, createdAt e id permanecem.
    /// </remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateRecommendationLogRequest request)
    {
        await service.AtualizarAsync(id, request);
        return NoContent();
    }

    /// <summary>Apaga um log.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await service.RemoverAsync(id);
        return NoContent();
    }
}