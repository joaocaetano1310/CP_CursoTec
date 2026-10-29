using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;

namespace CP1_CursoTec.Application.Services;

/// <summary>
/// Serviço de aplicação de alunos: orquestra o repositório e as regras do domínio.
/// As regras de nome, e-mail, CPF, idade e senha ficam na entidade <see cref="Aluno"/>.
/// Não conhece HTTP nem EF Core; depende apenas da interface de repositório.
/// </summary>
public class AlunoService(IAlunoRepository alunoRepository) : IAlunoService
{
    public async Task<Aluno> CriarAsync(AlunoRequest request)
    {
        var dataNascimento = request.DataNascimento
                             ?? throw new DomainException("A data de nascimento é obrigatória.");

        if (await alunoRepository.EmailExistsAsync(request.Email))
            throw new DomainException("Já existe um aluno cadastrado com este e-mail.");

        // O construtor valida tudo e transforma a senha em hash (a senha em texto não é guardada).
        // Papel não é passado: todo aluno cadastrado pela API nasce como PapelUsuario.Aluno.
        var aluno = new Aluno(
            request.Nome.Trim(),
            request.Email.Trim(),
            request.Cpf.Trim(),
            dataNascimento,
            request.Senha);

        await alunoRepository.AddAsync(aluno);

        return aluno;
    }

    public async Task<Aluno> ObterPorIdAsync(Guid id)
    {
        return await alunoRepository.GetByIdAsync(id)
               ?? throw new ResourceNotFoundException(nameof(Aluno), id);
    }
}