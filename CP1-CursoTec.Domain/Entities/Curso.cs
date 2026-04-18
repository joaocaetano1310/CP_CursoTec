using CP1_CursoTec.Domain.Commom;

namespace CP1_CursoTec.Domain.Entities;

public class Curso : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public int CargaHoraria { get; private set; }
    public string? Descricao { get; private set; }

    public ICollection<Turma> Turmas { get; private set; } = new List<Turma>();

    protected Curso() { }

    public Curso(string nome, int cargaHoraria, string? descricao)
    {
        UpdateNome(nome);
        UpdateCargaHoraria(cargaHoraria);
        Descricao = descricao;
    }

    public void UpdateNome(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new Exception("Nome não pode ser vazio.");

        Nome = newName;
    }

    public void UpdateCargaHoraria(int cargaHoraria)
    {
        if (cargaHoraria <= 0)
            throw new Exception("A carga horária deve ser maior que zero.");

        CargaHoraria = cargaHoraria;
    }

    public void UpdateDescricao(string? newDescricao)
    {
        Descricao = newDescricao;
    }
}
