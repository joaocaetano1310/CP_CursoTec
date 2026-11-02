namespace CP1_CursoTec.Application.DTO;

public record PagedResult<T>(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    IReadOnlyList<T> Items)
{
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}