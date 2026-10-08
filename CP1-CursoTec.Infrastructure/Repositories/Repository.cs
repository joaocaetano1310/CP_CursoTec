using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Commom;
using CP1_CursoTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP1_CursoTec.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica de <see cref="IRepository{T}"/> sobre o <see cref="ApplicationDbContext"/>.
/// Apenas persistência: nenhuma regra de negócio mora aqui.
/// </summary>
public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _set = context.Set<T>();

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _set.AsNoTracking().ToListAsync();
    }

    // Com tracking: o resultado costuma ser alterado e salvo em seguida (PUT/DELETE).
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _set.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task AddAsync(T entity)
    {
        await _set.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        return await _set.AsNoTracking().AnyAsync(e => e.Id == id);
    }
}
