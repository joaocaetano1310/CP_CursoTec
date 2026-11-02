using CP1_CursoTec.Domain.Entities;

namespace CP1_CursoTec.Application.Interfaces;

public interface IAlunoRepository
{
    Task<Aluno?> GetByIdAsync(Guid id);
    Task<IEnumerable<Aluno>> GetAllAsync();
    Task<Aluno?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(Aluno aluno);
    void Update(Aluno aluno);
    void Remove(Aluno aluno);
}