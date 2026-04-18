using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP1_CursoTec.Infrastructure.Repositories;

public class ProfessorRepository : IProfessorRepository
{
    private readonly ApplicationDbContext _context;

    public ProfessorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Professor?> GetByIdAsync(Guid id)
    {
        return await _context.Professores
            .Include(p => p.Turma)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Professor>> GetAllAsync()
    {
        return await _context.Professores
            .Include(p => p.Turma)
            .ToListAsync();
    }

    public async Task AddAsync(Professor professor)
    {
        await _context.Professores.AddAsync(professor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Professor professor)
    {
        _context.Professores.Update(professor);
        await _context.SaveChangesAsync();
    }

    public void Remove(Professor professor)
    {
        _context.Professores.Remove(professor);
        _context.SaveChanges();
    }
}