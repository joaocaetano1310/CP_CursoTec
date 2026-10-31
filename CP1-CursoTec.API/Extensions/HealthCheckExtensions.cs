using CP1_CursoTec.HealthChecks;
using CP1_CursoTec.Infrastructure.Data;
using CP1_CursoTec.Infrastructure.Mongo;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CP1_CursoTec.Extensions;

public static class HealthCheckExtensions
{
    /// <summary>
    /// Registra as verificações de saúde:
    /// <list type="bullet">
    /// <item><c>self</c>: processo no ar.</item>
    /// <item><c>database</c>: o <see cref="ApplicationDbContext"/> (SQLite) consegue conectar.</item>
    /// <item><c>mongodb</c>: o cluster do MongoDB (banco "recommenda") responde. Usa o mesmo
    /// <c>IMongoClient</c> singleton registrado por <c>AddRecommendaMongo</c>, que precisa vir antes.</item>
    /// <item><c>fiap-site</c>: URL externa; se cair, o status agregado vira Degraded (HTTP 200 com aviso).</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddCursoTecHealthChecks(this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("Processo no ar."), tags: ["self"])
            .AddDbContextCheck<ApplicationDbContext>("database", tags: ["db"])
            .AddMongoDb(
                databaseNameFactory: _ => MongoServiceCollectionExtensions.NomeBanco,
                name: "mongodb",
                tags: ["db", "mongo"],
                timeout: TimeSpan.FromSeconds(5))
            .AddCheck<ExternalUrlHealthCheck>("fiap-site", failureStatus: HealthStatus.Degraded, tags: ["external"]);

        return services;
    }

    /// <summary>
    /// Mapeia somente <c>GET /health</c> com o relatório completo em JSON (cada verificação aparece separada).
    /// Healthy → 200, Degraded → 200 (continua servindo tráfego), Unhealthy → 503.
    /// O probe é público (sem JWT) e fica fora do limite de taxa.
    /// </summary>
    public static IEndpointRouteBuilder MapCursoTecHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteAsync,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            })
            .AllowAnonymous()
            .DisableRateLimiting();

        return endpoints;
    }
}