namespace CP1_CursoTec.Domain.Entities;

public class Curso
{
    public string Nome { get; private set; }
    public int CargaHoraria { get; private set; }
    public string? Descricao { get; private set; }
    
    public ICollection<Turma> Turmas { get; set; } = new List<Turma>();

    public Curso(string nome, int cargaHoraria, string? descricao)
    {
        Nome = nome;
        CargaHoraria = cargaHoraria;
        Descricao = descricao;
    }
    
    public  Curso(string nome, string descricao)
    {
        UpdateNome(nome);
        UpdateDescricao(descricao);
    }

    public void UpdateNome(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new Exception("Nome não pode ser vazio.");
        
        Nome = newName;
    }
    
    public void UpdateDescricao(string newDescricao)
    {
        if (string.IsNullOrWhiteSpace(newDescricao))
            throw new Exception("Necessita de uma descrição.");
        
        Descricao = newDescricao;
    }
}