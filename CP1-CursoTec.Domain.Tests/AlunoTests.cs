using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Enums;
using CP1_CursoTec.Domain.Exceptions;

// ATENÇÃO: mantenha aqui o mesmo namespace que já existia no topo do seu arquivo original.
// namespace CP1_CursoTec.Domain.Tests;

public class AlunoTests
{
    private const string NomeValido = "Ana Souza";
    private const string EmailValido = "ana.souza@fiap.com.br";
    private const string CpfValido = "123.456.789-09";
    private const string SenhaValida = "senha1234";

    private static DateOnly NascimentoComIdade(int anos) =>
        DateOnly.FromDateTime(DateTime.Today).AddYears(-anos);

    [Fact]
    public void Construtor_DadosValidos_CriaAlunoComValoresInformados()
    {
        // Arrange
        var nascimento = NascimentoComIdade(20);

        // Act
        var aluno = new Aluno(NomeValido, EmailValido, CpfValido, nascimento, SenhaValida);

        // Assert
        Assert.Equal(NomeValido, aluno.Nome);
        Assert.Equal(EmailValido, aluno.Email);
        Assert.Equal(CpfValido, aluno.Cpf);
        Assert.Equal(nascimento, aluno.DataNascimento);
        Assert.Equal(20, aluno.Age);
        Assert.Null(aluno.Turma);
    }

    [Fact]
    public void SetDataNascimento_IdadeExatamenteNoMinimo_Aceita()
    {
        // Arrange
        var aluno = new Aluno(NomeValido, EmailValido, CpfValido, NascimentoComIdade(25), SenhaValida);
        var nascimento = NascimentoComIdade(17);

        // Act
        aluno.SetDataNascimento(nascimento);

        // Assert
        Assert.Equal(nascimento, aluno.DataNascimento);
        Assert.Equal(17, aluno.Age);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(16)]
    public void Construtor_IdadeMenorQue17_LancaDomainException(int idade)
    {
        // Arrange
        var nascimento = NascimentoComIdade(idade);

        // Act
        var excecao = Assert.Throws<DomainException>(
            () => new Aluno(NomeValido, EmailValido, CpfValido, nascimento, SenhaValida));

        // Assert
        Assert.Equal("Usuário deve ter pelo menos 17 anos.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("email-sem-arroba")]
    public void Construtor_EmailInvalido_LancaDomainException(string email)
    {
        // Arrange
        var nascimento = NascimentoComIdade(20);

        // Act
        var excecao = Assert.Throws<DomainException>(
            () => new Aluno(NomeValido, email, CpfValido, nascimento, SenhaValida));

        // Assert
        Assert.Equal("E-mail inválido.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_CpfVazio_LancaDomainException(string cpf)
    {
        // Arrange
        var nascimento = NascimentoComIdade(20);

        // Act
        var excecao = Assert.Throws<DomainException>(
            () => new Aluno(NomeValido, EmailValido, cpf, nascimento, SenhaValida));

        // Assert
        Assert.Equal("CPF é obrigatório.", excecao.Message);
    }

    // ===== Testes novos do CP6: senha e papel =====

    [Fact]
    public void Construtor_SemPapelInformado_CriaComoAluno()
    {
        // Act
        var aluno = new Aluno(NomeValido, EmailValido, CpfValido, NascimentoComIdade(20), SenhaValida);

        // Assert
        Assert.Equal(PapelUsuario.Aluno, aluno.Papel);
    }

    [Fact]
    public void Construtor_SenhaNaoFicaEmTextoPuro()
    {
        // Act
        var aluno = new Aluno(NomeValido, EmailValido, CpfValido, NascimentoComIdade(20), SenhaValida);

        // Assert
        Assert.False(string.IsNullOrEmpty(aluno.SenhaHash));
        Assert.NotEqual(SenhaValida, aluno.SenhaHash);
    }

    [Fact]
    public void VerifyPassword_SenhaCorreta_RetornaTrue()
    {
        // Arrange
        var aluno = new Aluno(NomeValido, EmailValido, CpfValido, NascimentoComIdade(20), SenhaValida);

        // Act
        var resultado = aluno.VerifyPassword(SenhaValida);

        // Assert
        Assert.True(resultado);
    }

    [Theory]
    [InlineData("outrasenha")]
    [InlineData("SENHA1234")]
    [InlineData("")]
    public void VerifyPassword_SenhaErrada_RetornaFalse(string tentativa)
    {
        // Arrange
        var aluno = new Aluno(NomeValido, EmailValido, CpfValido, NascimentoComIdade(20), SenhaValida);

        // Act
        var resultado = aluno.VerifyPassword(tentativa);

        // Assert
        Assert.False(resultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("1234567")]
    public void Construtor_SenhaInvalida_LancaDomainException(string senha)
    {
        // Arrange
        var nascimento = NascimentoComIdade(20);

        // Act
        var excecao = Assert.Throws<DomainException>(
            () => new Aluno(NomeValido, EmailValido, CpfValido, nascimento, senha));

        // Assert
        Assert.Equal("Senha deve ter pelo menos 8 caracteres.", excecao.Message);
    }
}