using CP1_CursoTec.Application.DTO;

namespace CP1_CursoTec.Application.Services;

/// <summary>Casos de uso do histórico de recomendação (CP6, MongoDB).</summary>
public interface IRecommendationLogService
{
    /// <exception cref="Domain.Exceptions.DomainException">query vazia, titles ausente/vazio/com item em branco ou userId vazio.</exception>
    Task<RecommendationLogDto> CriarAsync(CreateRecommendationLogRequest request);

    /// <exception cref="Domain.Exceptions.ResourceNotFoundException">Log inexistente.</exception>
    Task<RecommendationLogDto> ObterPorIdAsync(Guid id);

    /// <exception cref="Domain.Exceptions.DomainException">userId vazio.</exception>
    Task<IReadOnlyList<RecommendationLogDto>> ListarPorUsuarioAsync(Guid userId);

    /// <exception cref="Domain.Exceptions.DomainException">query ou titles inválidos.</exception>
    /// <exception cref="Domain.Exceptions.ResourceNotFoundException">Log inexistente.</exception>
    Task AtualizarAsync(Guid id, UpdateRecommendationLogRequest request);

    /// <exception cref="Domain.Exceptions.ResourceNotFoundException">Log inexistente.</exception>
    Task RemoverAsync(Guid id);
}