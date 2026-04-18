using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP1_CursoTec.Infrastructure.Repositorys;

public class AulaRepository : IAulaRepository
{
    private readonly ApplicationDbContext _context;

    public AulaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Aula?> GetByIdAsync(Guid id)
    {
        return await _context.Aulas
            .Include(a => a.Turma)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Aula>> GetAllAsync()
    {
        return await _context.Aulas
            .Include(a => a.Turma)
            .ToListAsync();
    }

    public async Task AddAsync(Aula aula)
    {
        await _context.Aulas.AddAsync(aula);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Aula aula)
    {
        _context.Aulas.Update(aula);
        await _context.SaveChangesAsync();
    }

    public void Remove(Aula aula)
    {
        _context.Aulas.Remove(aula);
        _context.SaveChanges();
    }
}