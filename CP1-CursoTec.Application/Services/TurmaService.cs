using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using CP1_CursoTec.Domain.Exceptions;

namespace CP1_CursoTec.Application.Services;

/// <summary>
/// Serviço de aplicação de turmas: orquestra os repositórios e as regras do domínio.
/// Não conhece HTTP nem EF Core; depende apenas das interfaces de repositório.
/// </summary>
public class TurmaService(
    ITurmaRepository turmaRepository,
    IRepository<Professor> professorRepository,
    IRepository<Curso> cursoRepository) : ITurmaService
{
    public async Task<Turma> CriarAsync(TurmaRequest request)
    {
        var professorId = request.ProfessorId
                          ?? throw new DomainException("Professor é obrigatório.");

        var professor = await professorRepository.GetByIdAsync(professorId)
                        ?? throw new ResourceNotFoundException(nameof(Professor), professorId);

        Curso? curso = null;
        if (request.CursoId.HasValue)
        {
            curso = await cursoRepository.GetByIdAsync(request.CursoId.Value)
                    ?? throw new ResourceNotFoundException(nameof(Curso), request.CursoId.Value);
        }

        var turma = new Turma(request.Nome, request.DataInicio, request.DataFim, professor.Id, curso);
        await turmaRepository.AddAsync(turma);

        // Recarrega com Professor/Curso/Alunos para devolver o agregado completo.
        return await turmaRepository.GetByIdAsync(turma.Id) ?? turma;
    }
}
