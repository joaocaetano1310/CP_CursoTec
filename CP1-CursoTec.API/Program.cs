using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Application.Services;
using CP1_CursoTec.Exceptions;
using CP1_CursoTec.Extensions;
using CP1_CursoTec.Infrastructure.Data;
using CP1_CursoTec.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CP1_CursoTec;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Persistência (SQLite + EF Core)
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Repositório genérico e repositórios por agregado
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
        builder.Services.AddScoped<IAulaRepository, AulaRepository>();
        builder.Services.AddScoped<ICursoRepository, CursoRepository>();
        builder.Services.AddScoped<IProfessorRepository, ProfessorRepository>();
        builder.Services.AddScoped<ITurmaRepository, TurmaRepository>();

        // Serviços de aplicação
        builder.Services.AddScoped<ITurmaService, TurmaService>();

        // Gera os cabeçalhos api-supported-versions e api-deprecated-versions
        builder.Services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;                      
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";                    // grupos do Swagger: v1, v2
            });

        builder.Services.AddControllers();
        builder.Services.AddCursoTecSwagger(builder.Configuration);

        // Rate limit: limita as requisições da listagem de cursos (v2) e responde 429 com Retry-After ao estourar
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Política "listagem": 5 requisições a cada 30 segundos, sem fila
            options.AddFixedWindowLimiter("listagem", limiter =>
            {
                limiter.PermitLimit = 5;
                limiter.Window = TimeSpan.FromSeconds(30);
                limiter.QueueLimit = 0;                        // sem fila: estourou, 429 na hora
            });

            // Resposta quando o limite estoura: cabeçalho Retry-After + corpo em ProblemDetails
            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString();
                }

                var problems = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                await problems.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context.HttpContext,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Muitas requisições",
                        Detail = "Limite de 5 requisições a cada 30 segundos para a listagem de cursos. Aguarde e tente novamente."
                    }
                });
            };
        });

        // Verificações de saúde (GET /health)
        builder.Services.AddCursoTecHealthChecks();

        // Tratamento global de erros (ProblemDetails)
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        var app = builder.Build();

        // Cria/atualiza o banco aplicando as migrations pendentes.
        // Se o banco estiver indisponível, a falha é registrada e a API sobe mesmo assim:
        // quem reporta o problema é o GET /health (503), em vez de a API simplesmente não iniciar.
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Falha ao aplicar as migrations na inicialização. O banco pode estar indisponível.");
            }
        }

        // Deve vir antes do Swagger e do MapControllers
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var d in provider.ApiVersionDescriptions)
                {
                    var nome = d.IsDeprecated ? $"CursoTec API {d.GroupName} (obsoleta)" : $"CursoTec API {d.GroupName}";
                    options.SwaggerEndpoint($"/swagger/{d.GroupName}/swagger.json", nome);
                }
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapControllers();
        app.MapCursoTecHealthChecks();

        app.Run();
    }
}
