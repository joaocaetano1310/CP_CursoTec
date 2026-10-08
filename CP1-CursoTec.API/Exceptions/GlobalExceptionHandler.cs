using CP1_CursoTec.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CP1_CursoTec.Exceptions;

/// <summary>
/// Tratamento global de exceções: registra o erro e devolve um <see cref="ProblemDetails"/>
/// (application/problem+json). Nunca expõe stack trace nem detalhes do banco.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            ResourceNotFoundException or KeyNotFoundException
                => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflictException or DbUpdateException
                => (StatusCodes.Status409Conflict, "Conflito de dados"),
            DomainException or ArgumentException
                => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            _
                => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Erro não tratado em {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "Falha {Status} em {Method} {Path}: {Message}",
                status, httpContext.Request.Method, httpContext.Request.Path, exception.Message);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = BuildDetail(exception, status),
            Instance = httpContext.Request.Path
        };
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }

    private string BuildDetail(Exception exception, int status)
    {
        // Erros de banco: mensagem fixa, a do provedor pode revelar tabelas e colunas.
        if (exception is DbUpdateException)
            return "Não foi possível salvar: os dados violam uma restrição (por exemplo, valor já cadastrado).";

        // 500: mensagem genérica fora de Desenvolvimento (sem stack trace em nenhum ambiente).
        if (status >= StatusCodes.Status500InternalServerError && !environment.IsDevelopment())
            return "Ocorreu um erro inesperado. Tente novamente mais tarde.";

        return exception.Message;
    }
}
