using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP1_CursoTec.Infrastructure.Repositories;

public class TurmaRepository : ITurmaRepository
{
    private readonly ApplicationDbContext _context;

    public TurmaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Turma?> GetByIdAsync(Guid id)
    {
        return await _context.Turmas
            .Include(t => t.Curso)
            .Include(t => t.Professor)
            .Include(t => t.Aulas)
            .Include(t => t.Alunos)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Turma>> GetAllAsync()
    {
        return await _context.Turmas
            .Include(t => t.Curso)
            .Include(t => t.Professor)
            .Include(t => t.Aulas)
            .Include(t => t.Alunos)
            .ToListAsync();
    }

    public async Task AddAsync(Turma turma)
    {
        await _context.Turmas.AddAsync(turma);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Turma turma)
    {
        _context.Turmas.Update(turma);
        await _context.SaveChangesAsync();
    }

    public void Remove(Turma turma)
    {
        _context.Turmas.Remove(turma);
        _context.SaveChanges();
    }
}