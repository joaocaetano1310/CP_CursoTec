using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Interfaces;

namespace CP1_CursoTec.Application.Services;

/// <summary>
/// Serviço de aplicação de autenticação: busca o aluno pelo e-mail, confere a senha
/// pelo hash do domínio (<c>Aluno.VerifyPassword</c>) e pede o token ao <see cref="ITokenGenerator"/>.
/// Não conhece HTTP nem JWT.
/// </summary>
public class AuthService(
    IAlunoRepository alunoRepository,
    ITokenGenerator tokenGenerator) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return null;

        var aluno = await alunoRepository.GetByEmailAsync(request.Email);

        // Mesmo retorno para "e-mail não existe" e "senha errada": não revela qual dos dois falhou.
        if (aluno is null || !aluno.VerifyPassword(request.Password))
            return null;

        var token = tokenGenerator.Gerar(aluno);

        return new LoginResponse(
            token.AccessToken,
            token.Expiration,
            aluno.Email,
            aluno.Nome,
            aluno.Papel.ToString());
    }
}