using CP1_CursoTec.Domain.Commom;

namespace CP1_CursoTec.Application.Interfaces;

/// <summary>
/// Contrato genérico de persistência para qualquer entidade que herde de <see cref="BaseEntity"/>.
/// </summary>
/// <typeparam name="T">Entidade de domínio (possui <c>Id</c> do tipo <see cref="Guid"/>).</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>Retorna todas as entidades (leitura sem tracking).</summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>Retorna uma página de entidades (leitura sem tracking), em ordem fixa, junto com o total de itens.</summary>
    Task<(IReadOnlyList<T> Items, int TotalItems)> GetPagedAsync(int page, int pageSize);

    /// <summary>Retorna a entidade com o id informado ou <c>null</c> se não existir.</summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>Adiciona uma nova entidade e persiste.</summary>
    Task AddAsync(T entity);

    /// <summary>Atualiza uma entidade existente e persiste.</summary>
    Task UpdateAsync(T entity);

    /// <summary>Remove a entidade e persiste.</summary>
    Task DeleteAsync(T entity);

    /// <summary>Indica se existe uma entidade com o id informado.</summary>
    Task<bool> ExistsByIdAsync(Guid id);
}