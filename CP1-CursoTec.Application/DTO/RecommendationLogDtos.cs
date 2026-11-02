namespace CP1_CursoTec.Application.DTO;

/// <summary>Corpo do POST /api/v2/recommendation-logs.</summary>
public class CreateRecommendationLogRequest
{
    /// <summary>Usuário que recebeu a recomendação (qualquer Guid diferente de 00000000-...).</summary>
    public Guid UserId { get; set; }

    /// <summary>O que o usuário pediu (ex.: "filme de terror curto").</summary>
    public string? Query { get; set; }

    /// <summary>Títulos sugeridos (pelo menos um, nenhum em branco).</summary>
    public List<string>? Titles { get; set; }
}

/// <summary>Corpo do PUT /api/v2/recommendation-logs/{id}. Não envia userId.</summary>
public class UpdateRecommendationLogRequest
{
    /// <summary>Nova consulta.</summary>
    public string? Query { get; set; }

    /// <summary>Nova lista de títulos.</summary>
    public List<string>? Titles { get; set; }
}

/// <summary>Um item do histórico de recomendação, como a API devolve.</summary>
/// <param name="Id">Identificador do documento (_id no MongoDB).</param>
/// <param name="UserId">Usuário que recebeu a recomendação.</param>
/// <param name="Query">O que foi pedido.</param>
/// <param name="Titles">Títulos sugeridos.</param>
/// <param name="CreatedAt">Momento (UTC) em que o log foi criado.</param>
public record RecommendationLogDto(
    Guid Id,
    Guid UserId,
    string Query,
    IReadOnlyList<string> Titles,
    DateTime CreatedAt);