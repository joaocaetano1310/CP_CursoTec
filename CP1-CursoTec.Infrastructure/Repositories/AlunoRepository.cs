using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP1_CursoTec.Infrastructure.Repositorys;

public class AlunoRepository : IAlunoRepository
{
    private readonly ApplicationDbContext _context;

    public AlunoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Aluno?> GetByIdAsync(Guid id)
    {
        return await _context.Alunos.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Aluno>> GetAllAsync()
    {
        return await _context.Alunos.ToListAsync();
    }

    public async Task AddAsync(Aluno aluno)
    {
        await _context.Alunos.AddAsync(aluno);
        await _context.SaveChangesAsync();
    }

    public void Update(Aluno aluno)
    {
        _context.Alunos.Update(aluno);
        _context.SaveChanges();
    }

    public void Remove(Aluno aluno)
    {
        _context.Alunos.Remove(aluno);
        _context.SaveChanges();
    }
}