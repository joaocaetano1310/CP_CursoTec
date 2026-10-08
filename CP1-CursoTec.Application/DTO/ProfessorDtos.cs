using System.ComponentModel.DataAnnotations;

namespace CP1_CursoTec.Application.DTO;

/// <summary>Dados de entrada para cadastrar um professor.</summary>
public class ProfessorRequest
{
    /// <summary>Nome completo do professor.</summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>E-mail do professor (único).</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Área de especialidade do professor.</summary>
    [Required(ErrorMessage = "A especialidade é obrigatória.")]
    [StringLength(150, ErrorMessage = "A especialidade deve ter no máximo 150 caracteres.")]
    public string Especialidade { get; set; } = string.Empty;
}

/// <summary>Representação de um professor retornada pela API.</summary>
public record ProfessorResponse(Guid Id, string Nome, string Email, string? Especialidade);
