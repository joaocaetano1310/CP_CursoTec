using System.Threading.RateLimiting;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Application.Services;
using CP1_CursoTec.Exceptions;
using CP1_CursoTec.Extensions;
using CP1_CursoTec.Infrastructure.Data;
using CP1_CursoTec.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

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

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(2, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("X-Api-Version"),
            new UrlSegmentApiVersionReader());
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddControllers();
builder.Services.AddCursoTecSwagger(builder.Configuration);   // já faz o AddSwaggerGen

// Limite de taxa (429 + Retry-After)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Escrita: 10 requisições por minuto por IP (janela fixa)
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

    options.OnRejected = async (context, cancellationToken) =>
    {
        var response = context.HttpContext.Response;

        var retryAfter = 60;
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry))
            retryAfter = (int)Math.Ceiling(retry.TotalSeconds);

        response.StatusCode = StatusCodes.Status429TooManyRequests;
        response.Headers["Retry-After"] = retryAfter.ToString();

        await response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Muitas requisições",
                Detail = $"Limite de 10 requisições por minuto excedido. Tente novamente em {retryAfter} segundo(s).",
                Instance = context.HttpContext.Request.Path
            },
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);
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

        // v2 primeiro (atual), depois a v1 (obsoleta)
        foreach (var description in provider.ApiVersionDescriptions.Reverse())
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.IsDeprecated
                    ? $"CursoTec API {description.GroupName} (obsoleta)"
                    : $"CursoTec API {description.GroupName}");
        }

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRateLimiter();          // depois do UseExceptionHandler e antes do MapControllers
app.MapControllers();
app.MapCursoTecHealthChecks();

app.Run();