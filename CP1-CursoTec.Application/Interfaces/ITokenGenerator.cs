using CP1_CursoTec.Domain.Entities;

namespace CP1_CursoTec.Application.Interfaces;

/// <summary>
/// Gera o token de acesso de um aluno autenticado.
/// A Application só conhece este contrato; a implementação com JWT fica na API.
/// </summary>
public interface ITokenGenerator
{
    TokenGerado Gerar(Aluno aluno);
}

/// <summary>Token gerado e o momento (UTC) em que ele expira.</summary>
public record TokenGerado(string AccessToken, DateTime Expiration);