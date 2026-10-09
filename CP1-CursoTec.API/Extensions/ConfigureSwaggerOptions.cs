using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi;

namespace CP1_CursoTec.Extensions;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration config)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo
            {
                Title = config["Swagger:Title"] ?? "CursoTec API",
                Version = description.ApiVersion.ToString(),
                Description = config["Swagger:Description"]
            };

            if (description.IsDeprecated)
                info.Description += " ATENÇÃO: esta versão está obsoleta (deprecated). Use a v2.";

            options.SwaggerDoc(description.GroupName, info);
        }
    }
}