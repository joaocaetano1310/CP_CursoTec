using CP1_CursoTec.Application.DTO;

namespace CP1_CursoTec.Application.Services;

/// <summary>Caso de uso de autenticação (login por e-mail e senha).</summary>
public interface IAuthService
{
    /// <summary>
    /// Confere e-mail e senha e, se estiverem corretos, devolve o token.
    /// Devolve <c>null</c> quando o e-mail não existe OU a senha está errada,
    /// sem diferenciar os dois casos.
    /// </summary>
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}