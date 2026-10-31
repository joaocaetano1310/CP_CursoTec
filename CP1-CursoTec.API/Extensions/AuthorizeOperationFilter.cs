using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CP1_CursoTec.Extensions;

/// <summary>
/// Coloca o cadeado do Swagger só nas operações protegidas: as que têm [Authorize]
/// (no método ou no controller) e não têm [AllowAnonymous].
/// Escrito para o Swashbuckle 10 / Microsoft.OpenApi 2.x.
/// </summary>
public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metodo = context.MethodInfo;

        var atributos = metodo.GetCustomAttributes(true)
            .Concat(metodo.DeclaringType?.GetCustomAttributes(true) ?? []);

        // Endpoints públicos (login, cadastro de aluno) ficam sem cadeado.
        if (atributos.OfType<AllowAnonymousAttribute>().Any())
            return;

        // Só põe o cadeado onde existe [Authorize] ou [Authorize(Roles = ...)].
        if (!atributos.OfType<AuthorizeAttribute>().Any())
            return;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
        });
    }
}