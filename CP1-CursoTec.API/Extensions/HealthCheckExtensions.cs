using CP1_CursoTec.HealthChecks;
using CP1_CursoTec.Infrastructure.Data;
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
    /// <item><c>fiap-site</c>: URL externa; se cair, o status agregado vira Degraded (HTTP 200 com aviso).</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddCursoTecHealthChecks(this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("Processo no ar."), tags: ["self"])
            .AddDbContextCheck<ApplicationDbContext>("database", tags: ["db"])
            .AddCheck<ExternalUrlHealthCheck>("fiap-site", failureStatus: HealthStatus.Degraded, tags: ["external"]);

        return services;
    }

    /// <summary>
    /// Mapeia somente <c>GET /health</c> com o relatório completo em JSON.
    /// Healthy → 200, Degraded → 200 (continua servindo tráfego), Unhealthy → 503.
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
        });

        return endpoints;
    }
}
