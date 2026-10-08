using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Exceptions;
using CP1_CursoTec.Extensions;
using CP1_CursoTec.Infrastructure.Data;
using CP1_CursoTec.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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

        builder.Services.AddControllers();
        builder.Services.AddCursoTecSwagger(builder.Configuration);

        // Tratamento global de erros (ProblemDetails)
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        var app = builder.Build();

        // Cria/atualiza o banco aplicando as migrations pendentes
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        }

        // Deve vir antes do Swagger e do MapControllers
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "CursoTec API v1");
                options.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
