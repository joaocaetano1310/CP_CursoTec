using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi;

namespace CP1_CursoTec.Extensions;

public class DeprecatedOperationFilter(IApiVersionDescriptionProvider provider) : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Controllers neutros (Professores e Turmas)
        var neutro = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .OfType<ApiVersionNeutralAttribute>().Any();
        if (neutro) return;

        // Se o documento do Swagger (v1, v2) é de uma versão obsoleta, marca a operação como deprecated
        var obsoleto = provider.ApiVersionDescriptions
            .Any(d => d.GroupName == context.DocumentName && d.IsDeprecated);

        if (obsoleto)
            operation.Deprecated = true;
    }
}