namespace CP1_CursoTec.Domain.Exceptions;

/// <summary>
/// Regra de negócio ou invariante de domínio violada. Mapeada para HTTP 400.
/// </summary>
public class DomainException(string message) : Exception(message);

/// <summary>
/// Recurso solicitado não existe. Mapeada para HTTP 404.
/// </summary>
public class ResourceNotFoundException(string recurso, Guid id)
    : Exception($"{recurso} '{id}' não encontrado.");

/// <summary>
/// Conflito com o estado atual dos dados (ex.: valor único duplicado). Mapeada para HTTP 409.
/// </summary>
public class ConflictException(string message) : Exception(message);
