namespace CP1_CursoTec.Application.DTO;

public class CreateProfessorRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
}