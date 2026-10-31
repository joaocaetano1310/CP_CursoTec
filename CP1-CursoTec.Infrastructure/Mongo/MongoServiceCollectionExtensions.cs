using CP1_CursoTec.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace CP1_CursoTec.Infrastructure.Mongo;

/// <summary>Registro do MongoDB na injeção de dependência (CP6).</summary>
public static class MongoServiceCollectionExtensions
{
    public const string NomeBanco = "recommenda";

    /// <summary>
    /// Registra um único <see cref="MongoClient"/> por processo (singleton), o banco "recommenda"
    /// e o repositório do histórico. Falha na subida se a connection string estiver vazia.
    /// </summary>
    /// <param name="services">Coleção de serviços da API.</param>
    /// <param name="connectionString">Valor de ConnectionStrings:RecommendaMongo (mongodb+srv://...).</param>
    public static IServiceCollection AddRecommendaMongo(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "ConnectionStrings:RecommendaMongo não configurada. Em Desenvolvimento, rode na pasta da solução: " +
                "dotnet user-secrets set \"ConnectionStrings:RecommendaMongo\" \"mongodb+srv://...\" --project CP1-CursoTec.API");

        // Driver 3.x: Guid precisa de representação definida antes do primeiro documento gravado.
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase(NomeBanco));

        services.AddScoped<IRecommendationLogRepository, MongoRecommendationLogRepository>();

        return services;
    }
}