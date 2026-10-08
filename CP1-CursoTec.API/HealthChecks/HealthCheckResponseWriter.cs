using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CP1_CursoTec.HealthChecks;

/// <summary>Relatório de saúde devolvido por <c>GET /health</c>.</summary>
public sealed record HealthReportDto(
    string Status,
    double TotalDurationMs,
    string TraceId,
    IReadOnlyList<HealthCheckEntryDto> Checks);

/// <summary>Resultado de uma verificação individual.</summary>
public sealed record HealthCheckEntryDto(
    string Name,
    string Status,
    double DurationMs,
    string? Description,
    string? Exception);

/// <summary>
/// Escreve o <see cref="HealthReport"/> como JSON (em vez do texto "Healthy" padrão).
/// O detalhe de exceção só é incluído em Development.
/// </summary>
public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();

        var dto = new HealthReportDto(
            report.Status.ToString(),
            Math.Round(report.TotalDuration.TotalMilliseconds, 2),
            context.TraceIdentifier,
            report.Entries
                .Select(entry => new HealthCheckEntryDto(
                    entry.Key,
                    entry.Value.Status.ToString(),
                    Math.Round(entry.Value.Duration.TotalMilliseconds, 2),
                    entry.Value.Description,
                    environment.IsDevelopment() ? entry.Value.Exception?.Message : null))
                .ToList());

        context.Response.ContentType = "application/json; charset=utf-8";
        return context.Response.WriteAsync(JsonSerializer.Serialize(dto, JsonOptions));
    }
}
