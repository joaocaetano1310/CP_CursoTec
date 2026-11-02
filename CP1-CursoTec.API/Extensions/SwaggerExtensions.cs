using Microsoft.OpenApi;

namespace CP1_CursoTec.Extensions;

public static class SwaggerExtensions
{
    /// <summary>
    /// Registra o Swagger (Swashbuckle) com um documento por versão da API (v1, v2),
    /// o aviso de versão obsoleta, o esquema de segurança Bearer (JWT)
    /// e os comentários XML dos projetos API e Application.
    /// </summary>
    /// <remarks>
    /// Os documentos (v1, v2...) são criados pelo <see cref="ConfigureSwaggerOptions"/>,
    /// registrado UMA única vez aqui. Registrar de novo (no Program.cs, por exemplo)
    /// duplicaria o documento "v1" e quebraria o Swagger.
    /// </remarks>
    public static IServiceCollection AddCursoTecSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            // XML gerado por API e Application (DTOs) ficam na pasta de saída.
            foreach (var xmlFile in Directory.GetFiles(AppContext.BaseDirectory, "CP1-CursoTec.*.xml"))
            {
                options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
            }

            // CP5 — marca as operações obsoletas (v1) no Swagger.
            options.OperationFilter<DeprecatedOperationFilter>();

            // CP6 — botão "Authorize": o usuário cola só o token e o Swagger envia "Bearer {token}".
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Cole apenas o accessToken devolvido por POST /api/v2/auth/login (sem a palavra Bearer)."
            });

            // CP6 — cadeado só nas operações com [Authorize].
            options.OperationFilter<AuthorizeOperationFilter>();
        });

        // Cria um SwaggerDoc para cada versão (v1 obsoleta, v2).
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        return services;
    }
}