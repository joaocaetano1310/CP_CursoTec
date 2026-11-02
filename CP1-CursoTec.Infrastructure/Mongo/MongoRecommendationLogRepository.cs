using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Interfaces;
using MongoDB.Driver;

namespace CP1_CursoTec.Infrastructure.Mongo;

/// <summary>
/// Implementação do <see cref="IRecommendationLogRepository"/> com o driver oficial do MongoDB.
/// É o único lugar da solução que fala com a coleção "recommendationLogs".
/// </summary>
public class MongoRecommendationLogRepository : IRecommendationLogRepository
{
    public const string NomeColecao = "recommendationLogs";

    private readonly IMongoCollection<RecommendationLogDocument> _colecao;

    public MongoRecommendationLogRepository(IMongoDatabase database)
    {
        _colecao = database.GetCollection<RecommendationLogDocument>(NomeColecao);
    }

    public Task InsertAsync(RecommendationLogDto log) =>
        _colecao.InsertOneAsync(RecommendationLogDocument.FromDto(log));

    public async Task<RecommendationLogDto?> GetByIdAsync(Guid id)
    {
        var documento = await _colecao
            .Find(d => d.Id == id)
            .FirstOrDefaultAsync();

        return documento?.ToDto();
    }

    public async Task<IReadOnlyList<RecommendationLogDto>> ListByUserAsync(Guid userId, int limit)
    {
        // Ordena e corta no próprio MongoDB: só "limit" documentos saem do banco.
        var documentos = await _colecao
            .Find(d => d.UserId == userId)
            .SortByDescending(d => d.CreatedAt)
            .Limit(limit)
            .ToListAsync();

        return documentos.Select(d => d.ToDto()).ToList();
    }

    public async Task<bool> UpdateAsync(Guid id, string query, IReadOnlyList<string> titles)
    {
        // Só query e titles. userId, createdAt e _id não são tocados.
        var alteracao = Builders<RecommendationLogDocument>.Update
            .Set(d => d.Query, query)
            .Set(d => d.Titles, titles.ToList());

        var resultado = await _colecao.UpdateOneAsync(d => d.Id == id, alteracao);

        // MatchedCount (e não ModifiedCount): um PUT com os mesmos valores não pode virar 404.
        return resultado.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var resultado = await _colecao.DeleteOneAsync(d => d.Id == id);
        return resultado.DeletedCount > 0;
    }
}