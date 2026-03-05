using CP1_CursoTec.Domain.Commom;

namespace CP1_CursoTec.Domain.Entities;

public class Aula : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TurmaId { get; private set; }
    public DateTime Data { get; private set; }
    public TimeOnly HoraInicio{ get; private set; }
    public TimeOnly HoraFim { get; private set; }
    public Turma? Turma { get; private set; }

    public Aula(Guid id, Guid turmaId, DateTime data, TimeOnly horaInicio, TimeOnly horaFim, Turma? turma)
    {
        Id = id;
        TurmaId = turmaId;
        Data = data;
        HoraInicio = horaInicio;
        HoraFim = horaFim;
        Turma = turma;
    }

    public Aula(DateTime data, TimeOnly horaInicio, TimeOnly horaFim)
    {
        UpdateData(data);
        UpdateInicio(horaInicio);
        UpdateFim(horaFim);
    }

    public void UpdateFim(TimeOnly newHoraFim)
    {
        HoraFim = newHoraFim;
    }
    
    public void UpdateInicio(TimeOnly newHoraInicio)
    {
        HoraInicio = newHoraInicio;
    }

    public void UpdateData(DateTime newData)
    {
        Data = newData;
    }
}