using CP1_CursoTec.Application.DTO;
using MongoDB.Bson.Serialization.Attributes;

namespace CP1_CursoTec.Infrastructure.Mongo;

/// <summary>
/// Formato do documento na coleção "recommendationLogs" do banco "recommenda".
/// Os nomes em camelCase (userId, query...) são os que aparecem no MongoDB.
/// </summary>
public class RecommendationLogDocument
{
    /// <summary>Identidade do documento (_id), um Guid.</summary>
    [BsonId]
    public Guid Id { get; set; }

    /// <summary>Outro Guid: o usuário. Não é o _id.</summary>
    [BsonElement("userId")]
    public Guid UserId { get; set; }

    [BsonElement("query")]
    public string Query { get; set; } = string.Empty;

    [BsonElement("titles")]
    public List<string> Titles { get; set; } = [];

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    public static RecommendationLogDocument FromDto(RecommendationLogDto dto) => new()
    {
        Id = dto.Id,
        UserId = dto.UserId,
        Query = dto.Query,
        Titles = dto.Titles.ToList(),
        CreatedAt = dto.CreatedAt
    };

    public RecommendationLogDto ToDto() => new(Id, UserId, Query, Titles, CreatedAt);
}