using CP1_CursoTec.Domain.Commom;
using CP1_CursoTec.Domain.Exceptions;

namespace CP1_CursoTec.Domain.Entities;

public class Professor : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Especialidade { get; private set; }

    public ICollection<Turma> Turmas { get; private set; } = new List<Turma>();

    // Para o EF Core
    protected Professor() { }

    public Professor(string nome, string email, string especialidade)
    {
        UpdateNome(nome);
        UpdateEmail(email);
        UpdateEspecialidade(especialidade);
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

    public void UpdateEspecialidade(string newEspecialidade)
    {
        if (string.IsNullOrWhiteSpace(newEspecialidade))
            throw new DomainException("Especialidade requerida.");

        Especialidade = newEspecialidade;
    }
}
