using Microsoft.OpenApi;

namespace CP1_CursoTec.Extensions;

public static class SwaggerExtensions
{
    /// <summary>
    /// Registra o Swagger (Swashbuckle) com título/descrição vindos do appsettings
    /// e comentários XML dos projetos API e Application.
    /// </summary>
    public static IServiceCollection AddCursoTecSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = configuration["Swagger:Title"] ?? "CursoTec API",
                Version = "v1",
                Description = configuration["Swagger:Description"]
            });

            // XML gerado por API e Application (DTOs) ficam na pasta de saída.
            foreach (var xmlFile in Directory.GetFiles(AppContext.BaseDirectory, "CP1-CursoTec.*.xml"))
            {
                options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
            }
        });

        return services;
    }
}
