using Xunit;
using CP1_CursoTec.Application.Common;

public class PagedResponseTests
{
    [Theory]
    [InlineData(25, 10, 3)]
    [InlineData(20, 10, 2)]
    [InlineData(1, 50, 1)]
    [InlineData(0, 10, 0)]
    public void Create_TotalItems_CalculaTotalPages(int totalItems, int pageSize, int totalPagesEsperado)
    {
        // Arrange + Act
        var resposta = PagedResponse<string>.Create(new List<string>(), 1, pageSize, totalItems);

        // Assert
        Assert.Equal(totalPagesEsperado, resposta.TotalPages);
        Assert.Equal(totalItems, resposta.TotalItems);
    }
}