using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CP1_CursoTec.Extensions;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var d in provider.ApiVersionDescriptions)
        {
            var descricao = "API do domínio CursoTec.";
            if (d.IsDeprecated)
                descricao += " **ESTA VERSÃO ESTÁ OBSOLETA (deprecated).** Use a v2.";

            options.SwaggerDoc(d.GroupName, new OpenApiInfo
            {
                Title = "CursoTec API",
                Version = d.ApiVersion.ToString(),
                Description = descricao
            });
        }
    }
}