using System.ComponentModel.DataAnnotations;

namespace CP1_CursoTec.Application.DTO;

/// <summary>Dados de entrada do login (POST /api/v2/auth/login).</summary>
public class LoginRequest
{
    /// <summary>E-mail cadastrado do aluno.</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Senha em texto (é conferida contra o hash; nunca é guardada nem devolvida).</summary>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Resposta de um login bem-sucedido. O token não carrega a senha.</summary>
/// <param name="AccessToken">JWT para enviar no cabeçalho <c>Authorization: Bearer {token}</c>.</param>
/// <param name="Expiration">Momento (UTC) em que o token expira.</param>
/// <param name="Email">E-mail do usuário autenticado.</param>
/// <param name="Name">Nome do usuário autenticado.</param>
/// <param name="Role">Papel do usuário (Aluno ou Admin).</param>
public record LoginResponse(string AccessToken, DateTime Expiration, string Email, string Name, string Role);