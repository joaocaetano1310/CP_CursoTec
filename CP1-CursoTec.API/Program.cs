using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CP1_CursoTec.API.Options;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Application.Services;
using CP1_CursoTec.Exceptions;
using CP1_CursoTec.Extensions;
using CP1_CursoTec.Infrastructure.Data;
using CP1_CursoTec.Infrastructure.Mongo;
using CP1_CursoTec.Infrastructure.Repositories;
using CP1_CursoTec.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
        builder.Services.AddScoped<IAlunoService, AlunoService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        // CP6 — Autenticação JWT
        // Issuer, Audience, Key e ExpirationMinutes vêm da seção "Jwt" do appsettings.json.
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
        builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
                         ?? throw new InvalidOperationException("Seção \"Jwt\" não encontrada no appsettings.json.");

        if (string.IsNullOrWhiteSpace(jwtOptions.Key) || Encoding.UTF8.GetByteCount(jwtOptions.Key) < 32)
            throw new InvalidOperationException("Jwt:Key ausente ou com menos de 32 caracteres no appsettings.json.");

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Mantém os nomes curtos das claims ("role", "name", "email", "sub").
                // Sem isso o .NET troca "role" por um URI longo e o [Authorize(Roles = ...)] não reconhece o papel.
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),

                    RoleClaimType = "role",
                    NameClaimType = "name"
                };
            });

        builder.Services.AddAuthorization();

        // CP6 — MongoDB (histórico de recomendação), registrado antes dos health checks.
        // A connection string vem de ConnectionStrings:RecommendaMongo (user-secrets em Desenvolvimento).
        // String vazia → a API não sobe e explica como configurar.
        builder.Services.AddRecommendaMongo(builder.Configuration.GetConnectionString("RecommendaMongo"));
        builder.Services.AddScoped<IRecommendationLogService, RecommendationLogService>();

        // Versionamento: gera os cabeçalhos api-supported-versions e api-deprecated-versions.
        // A versão pode vir na URL (/api/v2/...), na query string (?api-version=2.0) ou no cabeçalho X-Api-Version.
        builder.Services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";                    // grupos do Swagger: v1, v2
                options.SubstituteApiVersionInUrl = true;              // mostra /api/v2/... no Swagger
            });

        builder.Services.AddControllers();
        builder.Services.AddCursoTecSwagger(builder.Configuration);   // já faz o AddSwaggerGen

        // Rate limit: responde 429 com Retry-After e corpo em ProblemDetails ao estourar
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Política "listagem" (CP5, cursos v2): 5 requisições a cada 30 segundos, sem fila
            options.AddFixedWindowLimiter("listagem", limiter =>
            {
                limiter.PermitLimit = 5;
                limiter.Window = TimeSpan.FromSeconds(30);
                limiter.QueueLimit = 0;                        // sem fila: estourou, 429 na hora
            });

            // Política "escrita" (CP5, turmas): 10 requisições por minuto por IP, janela fixa
            options.AddPolicy("escrita", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            // Resposta quando o limite estoura: cabeçalho Retry-After + corpo em ProblemDetails
            options.OnRejected = async (context, cancellationToken) =>
            {
                var retryAfter = 60;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry))
                    retryAfter = (int)Math.Ceiling(retry.TotalSeconds);

                context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();

                var problems = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                await problems.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context.HttpContext,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Muitas requisições",
                        Detail = $"Limite de requisições excedido. Tente novamente em {retryAfter} segundo(s).",
                        Instance = context.HttpContext.Request.Path
                    }
                });
            };
        });

        // Verificações de saúde (GET /health) — inclui o MongoDB registrado acima
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

                // v2 primeiro (atual), depois a v1 (obsoleta)
                foreach (var d in provider.ApiVersionDescriptions.Reverse())
                {
                    var nome = d.IsDeprecated ? $"CursoTec API {d.GroupName} (obsoleta)" : $"CursoTec API {d.GroupName}";
                    options.SwaggerEndpoint($"/swagger/{d.GroupName}/swagger.json", nome);
                }
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();       // CP6: lê e valida o token do cabeçalho Authorization
        app.UseAuthorization();        // CP6: aplica [Authorize] / [Authorize(Roles = ...)]
        app.UseRateLimiter();          // depois do UseExceptionHandler e antes do MapControllers
        app.MapControllers();
        app.MapCursoTecHealthChecks();

        app.Run();
    }
}