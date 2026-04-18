using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP1_CursoTec.Infrastructure.Repositorys;

public class CursoRepository : ICursoRepository
{
    private readonly ApplicationDbContext _context;

    public CursoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Curso?> GetByIdAsync(Guid id)
    {
        return await _context.Cursos
            .Include(c => c.Turmas)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Curso>> GetAllAsync()
    {
        return await _context.Cursos
            .Include(c => c.Turmas)
            .ToListAsync();
    }

    public async Task AddAsync(Curso curso)
    {
        await _context.Cursos.AddAsync(curso);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Curso curso)
    {
        _context.Cursos.Update(curso);
        await _context.SaveChangesAsync();
    }

    public void Remove(Curso curso)
    {
        _context.Cursos.Remove(curso);
        _context.SaveChanges();
    }
}