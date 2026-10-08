using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Domain.Entities;

namespace CP1_CursoTec.Application.Services;

/// <summary>Casos de uso de escrita de turmas.</summary>
public interface ITurmaService
{
    /// <summary>
    /// Cria uma turma validando que o professor (e o curso, quando informado) existem.
    /// </summary>
    /// <exception cref="Domain.Exceptions.DomainException">Professor não informado ou dados da turma inválidos.</exception>
    /// <exception cref="Domain.Exceptions.ResourceNotFoundException">Professor ou curso inexistente.</exception>
    Task<Turma> CriarAsync(TurmaRequest request);
}
