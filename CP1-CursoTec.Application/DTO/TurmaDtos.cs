using System.ComponentModel.DataAnnotations;

namespace CP1_CursoTec.Application.DTO;

/// <summary>Dados de entrada para criar uma turma.</summary>
public class TurmaRequest
{
    /// <summary>Nome da turma.</summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>Data de início das aulas.</summary>
    public DateTime DataInicio { get; set; }

    /// <summary>Data de término (opcional, não pode ser anterior ao início).</summary>
    public DateTime? DataFim { get; set; }

    /// <summary>Id do professor responsável (obrigatório).</summary>
    [Required(ErrorMessage = "O professor é obrigatório.")]
    public Guid? ProfessorId { get; set; }

    /// <summary>Id do curso ao qual a turma pertence (opcional).</summary>
    public Guid? CursoId { get; set; }
}

/// <summary>Representação de uma turma retornada pela API.</summary>
public record TurmaResponse(
    Guid Id,
    string Nome,
    DateTime DataInicio,
    DateTime? DataFim,
    Guid ProfessorId,
    string? ProfessorNome,
    string? CursoNome,
    int TotalAlunos);
