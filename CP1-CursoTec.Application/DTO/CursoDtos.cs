using System.ComponentModel.DataAnnotations;

namespace CP1_CursoTec.Application.DTO;

/// <summary>Dados de entrada para criar ou atualizar um curso.</summary>
public class CursoRequest
{
    /// <summary>Nome do curso (único).</summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>Carga horária total em horas (maior que zero).</summary>
    [Range(1, int.MaxValue, ErrorMessage = "A carga horária deve ser maior que zero.")]
    public int CargaHoraria { get; set; }

    /// <summary>Descrição opcional do curso.</summary>
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }
}

/// <summary>Representação de um curso retornada pela API.</summary>
public record CursoResponse(Guid Id, string Nome, int CargaHoraria, string? Descricao);
