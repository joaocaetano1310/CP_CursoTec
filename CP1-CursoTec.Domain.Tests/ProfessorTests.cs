using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;

namespace CP1_CursoTec.Domain.Tests;

public class ProfessorTests
{
    [Fact]
    public void Construtor_DadosValidos_CriaProfessorComValoresInformados()
    {
        // Arrange
        const string nome = "Maria Souza";
        const string email = "maria.souza@cursotec.com";
        const string especialidade = "Banco de Dados";

        // Act
        var professor = new Professor(nome, email, especialidade);

        // Assert
        Assert.Equal(nome, professor.Nome);
        Assert.Equal(email, professor.Email);
        Assert.Equal(especialidade, professor.Especialidade);
        Assert.Empty(professor.Turmas);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_EspecialidadeVazia_LancaDomainException(string especialidade)
    {
        // Arrange / Act
        var excecao = Assert.Throws<DomainException>(
            () => new Professor("Maria Souza", "maria.souza@cursotec.com", especialidade));

        // Assert
        Assert.Equal("Especialidade requerida.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sem-arroba")]
    public void UpdateEmail_EmailInvalido_LancaDomainException(string email)
    {
        // Arrange
        var professor = new Professor("Maria Souza", "maria.souza@cursotec.com", "Banco de Dados");

        // Act
        var excecao = Assert.Throws<DomainException>(() => professor.UpdateEmail(email));

        // Assert
        Assert.Equal("E-mail inválido.", excecao.Message);
        Assert.Equal("maria.souza@cursotec.com", professor.Email); // valor anterior preservado
    }
}
