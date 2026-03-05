using CP1_CursoTec.Domain.Commom;

namespace CP1_CursoTec.Domain.Entities;

public class Aluno : BaseEntity
{
    public Guid AlunoId { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Cpf { get; private set; }
    public DateOnly DataNascimento { get; private set; }
    
    public Turma? Turma { get; private set; }
    
    public Aluno(string nome, string email, string cpf, DateOnly datanascimento, Turma? turma)
    {
        Nome = nome;
        Email = email;
        Cpf = cpf;
        DataNascimento = datanascimento;
        Turma = turma;
    }
    
    public Aluno(string name, string email, DateOnly dateBorn)
    {
        UpdateNome(name);
        UpdateEmail(email);
        SetDataNascimento(dateBorn);
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

    public void SetDataNascimento(DateOnly newDate)
    {
        var age = CalculateAge(newDate);
        
        if (age < 17)
            throw new Exception("Usuário deve ter pelo menos 17 anos.");

        DataNascimento = newDate;
    }
    
    public int Age => CalculateAge(DataNascimento);

    private static int CalculateAge(DateOnly date)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - date.Year;
        if (date > today.AddYears(-age)) age--;
        return age;
    }
}