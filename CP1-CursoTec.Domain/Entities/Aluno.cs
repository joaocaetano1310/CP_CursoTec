using CP1_CursoTec.Domain.Commom;
using CP1_CursoTec.Domain.Exceptions;
using CP1_CursoTec.Domain.Enums;

namespace CP1_CursoTec.Domain.Entities;

public class Aluno : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public DateOnly DataNascimento { get; private set; }
    public Turma? Turma { get; private set; }

    public string SenhaHash { get; private set; } = string.Empty;

    public PapelUsuario Papel { get; private set; } = PapelUsuario.Aluno;

    // Para o EF Core
    protected Aluno() { }

    public Aluno(string nome, string email, string cpf, DateOnly dataNascimento,
                 string senha, PapelUsuario papel = PapelUsuario.Aluno)
    {
        UpdateNome(nome);
        UpdateEmail(email);
        UpdateCpf(cpf);
        SetDataNascimento(dataNascimento);
        DefinirSenha(senha);
        Papel = papel;
    }

    public void UpdateNome(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Nome não pode ser vazio.");

        Nome = newName;
    }

    public void UpdateEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains('@'))
            throw new DomainException("E-mail inválido.");

        Email = newEmail;
    }

    public void UpdateCpf(string newCpf)
    {
        if (string.IsNullOrWhiteSpace(newCpf))
            throw new DomainException("CPF é obrigatório.");

        Cpf = newCpf;
    }

    public void SetDataNascimento(DateOnly newDate)
    {
        var age = CalculateAge(newDate);

        if (age < 17)
            throw new DomainException("Usuário deve ter pelo menos 17 anos.");

        DataNascimento = newDate;
    }

    public void AtribuirTurma(Turma? turma)
    {
        Turma = turma;
    }

    // Regra de senha fica no Domain: mínimo de 8 caracteres.
    // A senha nunca é guardada em texto; só o hash (PBKDF2) vai para SenhaHash.
    public void DefinirSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
            throw new DomainException("Senha deve ter pelo menos 8 caracteres.");

        SenhaHash = PasswordHasher.Hash(senha);
    }

    // Usado no login: confere a senha digitada contra o hash salvo.
    public bool VerifyPassword(string senha) =>
        !string.IsNullOrEmpty(senha)
        && !string.IsNullOrEmpty(SenhaHash)
        && PasswordHasher.Verify(senha, SenhaHash);

    public void DefinirPapel(PapelUsuario papel) => Papel = papel;

    public int Age => CalculateAge(DataNascimento);

    private static int CalculateAge(DateOnly date)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - date.Year;
        if (date > today.AddYears(-age)) age--;
        return age;
    }
}