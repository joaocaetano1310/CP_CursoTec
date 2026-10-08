using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CP1_CursoTec.HealthChecks;

/// <summary>
/// Verifica se uma URL externa (por padrão, o site da FIAP) responde.
/// A falha é reportada com o <c>FailureStatus</c> do registro (Degraded no CP4):
/// a API continua servindo tráfego (HTTP 200), mas o relatório alerta a dependência fora do ar.
/// </summary>
public class ExternalUrlHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IHealthCheck
{
    public const string DefaultUrl = "https://www.fiap.com.br";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var url = configuration["HealthChecks:ExternalUrl"] ?? DefaultUrl;

        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(3);

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"{url} respondeu {(int)response.StatusCode}.")
                : new HealthCheckResult(context.Registration.FailureStatus, $"{url} respondeu {(int)response.StatusCode}.");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // Descrição fixa: o detalhe da exceção só aparece em Development (ver HealthCheckResponseWriter).
            return new HealthCheckResult(context.Registration.FailureStatus, $"Não foi possível acessar {url}.", ex);
        }
    }
}
