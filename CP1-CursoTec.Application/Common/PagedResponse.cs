namespace CP1_CursoTec.Application.Common;

public record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages)
{
    public static PagedResponse<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalItems)
        => new(items, page, pageSize, totalItems,
               (int)Math.Ceiling(totalItems / (double)pageSize));
}