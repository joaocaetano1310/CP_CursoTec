using System.ComponentModel.DataAnnotations;

namespace CP1_CursoTec.Application.DTO;

/// <summary>Dados de entrada para cadastrar um aluno (também é o usuário que faz login).</summary>
public class AlunoRequest
{
    /// <summary>Nome completo do aluno.</summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>E-mail do aluno (único). É usado no login.</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>CPF do aluno.</summary>
    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [StringLength(14, ErrorMessage = "O CPF deve ter no máximo 14 caracteres.")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>Data de nascimento (formato yyyy-MM-dd). O aluno deve ter pelo menos 17 anos.</summary>
    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateOnly? DataNascimento { get; set; }

    /// <summary>Senha de acesso (mínimo 8 caracteres). Nunca é devolvida pela API.</summary>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    [StringLength(100, ErrorMessage = "A senha deve ter no máximo 100 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

/// <summary>Representação de um aluno retornada pela API (sem senha nem hash).</summary>
public record AlunoResponse(Guid Id, string Nome, string Email, string Cpf, DateOnly DataNascimento, string Papel);