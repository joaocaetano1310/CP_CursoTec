using CP1_CursoTec.Domain.Entities;

namespace CP1_CursoTec.Application.Interfaces;

public interface ITurmaRepository
{
    Task<Turma?> GetByIdAsync(Guid id);
    Task<IEnumerable<Turma>> GetAllAsync();
    Task AddAsync(Turma turma);
    Task UpdateAsync(Turma turma);
    
    Task<(IReadOnlyList<Turma> Items, int TotalItems)> GetPagedAsync(int page, int pageSize);
    
    void Remove(Turma turma);
}