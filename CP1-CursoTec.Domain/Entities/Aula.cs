using CP1_CursoTec.Domain.Commom;

namespace CP1_CursoTec.Domain.Entities;

public class Aula : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TurmaId { get; private set; }
    public DateTime Data { get; private set; }
    public TimeSpan HoraInicio { get; private set; }
    public TimeSpan HoraFim { get; private set; }
    public Turma? Turma { get; private set; }

    public Aula(Guid id, Guid turmaId, DateTime data, TimeSpan horaInicio, TimeSpan horaFim, Turma? turma)
    {
        Id = id;
        TurmaId = turmaId;
        Data = data;
        HoraInicio = horaInicio;
        HoraFim = horaFim;
        Turma = turma;
    }
    
}