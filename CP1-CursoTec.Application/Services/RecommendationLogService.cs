using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Exceptions;

namespace CP1_CursoTec.Application.Services;

/// <summary>
/// Serviço de aplicação do histórico de recomendação: valida as regras do contrato
/// e delega a persistência ao <see cref="IRecommendationLogRepository"/>.
/// Não conhece HTTP nem o driver do MongoDB.
/// </summary>
public class RecommendationLogService(IRecommendationLogRepository repository) : IRecommendationLogService
{
    /// <summary>Teto da listagem por usuário (aplicado no banco, não em memória).</summary>
    public const int LimiteListagem = 20;

    private const string NomeRecurso = "RecommendationLog";

    public async Task<RecommendationLogDto> CriarAsync(CreateRecommendationLogRequest request)
    {
        ValidarUserId(request.UserId);
        var query = ValidarQuery(request.Query);
        var titles = ValidarTitles(request.Titles);

        // O id do log é gerado aqui e é diferente do userId.
        var log = new RecommendationLogDto(Guid.NewGuid(), request.UserId, query, titles, DateTime.UtcNow);
        await repository.InsertAsync(log);

        return log;
    }

    public async Task<RecommendationLogDto> ObterPorIdAsync(Guid id)
    {
        return await repository.GetByIdAsync(id)
               ?? throw new ResourceNotFoundException(NomeRecurso, id);
    }

    public Task<IReadOnlyList<RecommendationLogDto>> ListarPorUsuarioAsync(Guid userId)
    {
        ValidarUserId(userId);
        return repository.ListByUserAsync(userId, LimiteListagem);
    }

    public async Task AtualizarAsync(Guid id, UpdateRecommendationLogRequest request)
    {
        var query = ValidarQuery(request.Query);
        var titles = ValidarTitles(request.Titles);

        // Só query e titles mudam; userId, createdAt e _id permanecem.
        var encontrado = await repository.UpdateAsync(id, query, titles);
        if (!encontrado)
            throw new ResourceNotFoundException(NomeRecurso, id);
    }

    public async Task RemoverAsync(Guid id)
    {
        var removido = await repository.DeleteAsync(id);
        if (!removido)
            throw new ResourceNotFoundException(NomeRecurso, id);
    }

    private static void ValidarUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("userId é obrigatório e não pode ser 00000000-0000-0000-0000-000000000000.");
    }

    private static string ValidarQuery(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new DomainException("query não pode ser vazia.");

        return query.Trim();
    }

    private static List<string> ValidarTitles(List<string>? titles)
    {
        if (titles is null || titles.Count == 0)
            throw new DomainException("titles deve ter pelo menos um título.");

        if (titles.Any(string.IsNullOrWhiteSpace))
            throw new DomainException("titles não pode ter item em branco.");

        return titles.Select(t => t.Trim()).ToList();
    }
}