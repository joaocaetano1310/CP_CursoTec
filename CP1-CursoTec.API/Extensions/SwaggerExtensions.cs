using Microsoft.OpenApi;

namespace CP1_CursoTec.Extensions;

public static class SwaggerExtensions
{
    /// <summary>
    /// Registra o Swagger (Swashbuckle) com um documento por versão da API (v1, v2)
    /// e comentários XML dos projetos API e Application.
    /// </summary>
    /// <remarks>
    /// Os documentos (v1, v2...) são criados pelo <see cref="ConfigureSwaggerOptions"/>,
    /// a partir das versões que o Asp.Versioning encontra nos controllers.
    /// Por isso não há mais um <c>SwaggerDoc("v1", ...)</c> fixo aqui: ele duplicaria o v1.
    /// </remarks>
    public static IServiceCollection AddCursoTecSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        // Cria um SwaggerDoc para cada versão (v1 obsoleta, v2).
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        services.AddSwaggerGen(options =>
        {
            // XML gerado por API e Application (DTOs) ficam na pasta de saída.
            foreach (var xmlFile in Directory.GetFiles(AppContext.BaseDirectory, "CP1-CursoTec.*.xml"))
            {
                options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
            }
        });

        return services;
    }
}