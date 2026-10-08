using CP1_CursoTec.Domain.Commom;
using CP1_CursoTec.Domain.Exceptions;

namespace CP1_CursoTec.Domain.Entities;

public class Turma : BaseEntity
{
    public string Nome_turma { get; private set; } = string.Empty;
    public DateTime DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public Guid ProfessorId { get; private set; }
    public Curso? Curso { get; private set; }
    public Professor? Professor { get; private set; }

    public ICollection<Aula> Aulas { get; private set; } = new List<Aula>();
    public ICollection<Aluno> Alunos { get; private set; } = new List<Aluno>();

    // Para o EF Core
    protected Turma() { }

    public Turma(string nomeTurma, DateTime dataInicio, DateTime? dataFim, Guid professorId, Curso? curso)
    {
        if (professorId == Guid.Empty)
            throw new DomainException("Professor é obrigatório.");

        UpdateNome(nomeTurma);
        DataInicio = dataInicio;
        UpdateDataFim(dataFim);
        ProfessorId = professorId;
        Curso = curso;
    }

    public void UpdateNome(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Nome não pode ser vazio.");

        Nome_turma = newName;
    }

    public void UpdateDataInicio(DateTime newData)
    {
        if (DataFim.HasValue && DataFim.Value < newData)
            throw new DomainException("A data de início não pode ser posterior à data de fim.");

        DataInicio = newData;
    }

    public void UpdateDataFim(DateTime? newData)
    {
        if (newData.HasValue && newData.Value < DataInicio)
            throw new DomainException("A data de fim não pode ser anterior à data de início.");

        DataFim = newData;
    }
}
