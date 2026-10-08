using CP1_CursoTec.Domain.Commom;
using CP1_CursoTec.Domain.Exceptions;

namespace CP1_CursoTec.Domain.Entities;

public class Aula : BaseEntity
{
    public Guid TurmaId { get; private set; }
    public DateTime Data { get; private set; }
    public TimeOnly HoraInicio { get; private set; }
    public TimeOnly HoraFim { get; private set; }
    public Turma? Turma { get; private set; }

    // Para o EF Core
    protected Aula() { }

    public Aula(Guid turmaId, DateTime data, TimeOnly horaInicio, TimeOnly horaFim)
    {
        if (turmaId == Guid.Empty)
            throw new DomainException("Turma é obrigatória.");

        if (horaFim <= horaInicio)
            throw new DomainException("A hora de fim deve ser posterior à hora de início.");

        TurmaId = turmaId;
        Data = data;
        HoraInicio = horaInicio;
        HoraFim = horaFim;
    }

    public void UpdateData(DateTime newData)
    {
        Data = newData;
    }

    public void UpdateHorario(TimeOnly newHoraInicio, TimeOnly newHoraFim)
    {
        if (newHoraFim <= newHoraInicio)
            throw new DomainException("A hora de fim deve ser posterior à hora de início.");

        HoraInicio = newHoraInicio;
        HoraFim = newHoraFim;
    }
}
