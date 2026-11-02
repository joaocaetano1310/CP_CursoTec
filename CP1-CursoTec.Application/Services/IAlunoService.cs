using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Domain.Entities;

namespace CP1_CursoTec.Application.Services;

/// <summary>Casos de uso de alunos (o aluno também é o usuário que faz login).</summary>
public interface IAlunoService
{
    /// <summary>
    /// Cadastra um aluno com senha. O papel é sempre <c>Aluno</c>; não é aceito pelo corpo da requisição.
    /// </summary>
    /// <exception cref="Domain.Exceptions.DomainException">Dados inválidos (idade, e-mail, CPF, senha) ou e-mail já cadastrado.</exception>
    Task<Aluno> CriarAsync(AlunoRequest request);

    /// <summary>Busca um aluno pelo id.</summary>
    /// <exception cref="Domain.Exceptions.ResourceNotFoundException">Aluno inexistente.</exception>
    Task<Aluno> ObterPorIdAsync(Guid id);
}