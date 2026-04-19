using CP1_CursoTec.Domain.Commom;

namespace CP1_CursoTec.Domain.Entities;

public class Professor : BaseEntity
{
    public Guid ProfessorId { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string? Especialidade { get; private set; }

    public ICollection<Turma> Turmas { get; private set; } = new List<Turma>();

    protected Professor() { }

    public Professor(string nome, string email, string? especialidade)
    {
        Nome = nome;
        Email = email;
        Especialidade = especialidade;
    }

    public void UpdateNome(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new Exception("Nome não pode ser vazio.");

        Nome = newName;
    }

    public void UpdateEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains("@"))
            throw new Exception("E-mail inválido.");

        Email = newEmail;
    }

    public void UpdateEspecialidade(string newEspecialidade)
    {
        if (string.IsNullOrWhiteSpace(newEspecialidade))
            throw new Exception("Especialidade requerida.");

        Especialidade = newEspecialidade;
    }
}