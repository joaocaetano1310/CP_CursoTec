using CP1_CursoTec.Application.DTO;

namespace CP1_CursoTec.Application.Interfaces;

/// <summary>
/// Persistência do histórico de recomendação.
/// A Application só conhece este contrato; a implementação com MongoDB fica na Infrastructure.
/// </summary>
public interface IRecommendationLogRepository
{
    Task InsertAsync(RecommendationLogDto log);

    Task<RecommendationLogDto?> GetByIdAsync(Guid id);

    /// <summary>Logs do usuário, do mais novo para o mais antigo, já cortados no banco em <paramref name="limit"/>.</summary>
    Task<IReadOnlyList<RecommendationLogDto>> ListByUserAsync(Guid userId, int limit);

    /// <summary>Altera só query e titles. Devolve false se o id não existir.</summary>
    Task<bool> UpdateAsync(Guid id, string query, IReadOnlyList<string> titles);

    /// <summary>Devolve false se o id não existir.</summary>
    Task<bool> DeleteAsync(Guid id);
}