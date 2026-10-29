using Asp.Versioning;
using CP1_CursoTec.Application.DTO;
using CP1_CursoTec.Application.Services;
using CP1_CursoTec.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CP1_CursoTec.Controllers;

/// <summary>
/// Cadastro e consulta de alunos. O aluno também é o usuário que faz login (CP6):
/// o cadastro é público para que seja possível criar a conta antes de obter o token.
/// A criação e a consulta são delegadas ao <see cref="IAlunoService"/>.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/alunos")]
[Route("api/v{version:apiVersion}/alunos")]
[Produces("application/json")]
public class AlunosController(
    IAlunoService alunoService,
    ILogger<AlunosController> logger) : ControllerBase
{
    /// <summary>Busca um aluno pelo id.</summary>
    /// <param name="id">Identificador (GUID) do aluno.</param>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AlunoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AlunoResponse>> GetById(Guid id)
    {
        var aluno = await alunoService.ObterPorIdAsync(id);
        return Ok(ToResponse(aluno));
    }

    /// <summary>Cadastra um aluno (público, sem token).</summary>
    /// <remarks>
    /// Exemplo de corpo:
    ///
    ///     {
    ///       "nome": "Ana Souza",
    ///       "email": "ana.souza@fiap.com.br",
    ///       "cpf": "123.456.789-09",
    ///       "dataNascimento": "2004-05-20",
    ///       "senha": "senha1234"
    ///     }
    ///
    /// O aluno é sempre criado com o papel <c>Aluno</c>. A senha é guardada apenas como hash
    /// e nunca é devolvida. Retorna 400 se os dados forem inválidos (idade menor que 17,
    /// senha com menos de 8 caracteres, e-mail inválido) ou se o e-mail já estiver cadastrado.
    /// </remarks>
    /// <param name="request">Dados do aluno.</param>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AlunoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AlunoResponse>> Create(AlunoRequest request)
    {
        // Não loga senha nem e-mail completo: só o necessário para rastrear a requisição.
        logger.LogInformation(
            "Iniciando cadastro de aluno {Nome}. TraceId={TraceId}",
            request.Nome, HttpContext.TraceIdentifier);

        var aluno = await alunoService.CriarAsync(request);

        logger.LogInformation(
            "Aluno {AlunoId} cadastrado com sucesso. TraceId={TraceId}",
            aluno.Id, HttpContext.TraceIdentifier);

        return CreatedAtAction(nameof(GetById), new { id = aluno.Id }, ToResponse(aluno));
    }

    private static AlunoResponse ToResponse(Aluno aluno) =>
        new(aluno.Id,
            aluno.Nome,
            aluno.Email,
            aluno.Cpf,
            aluno.DataNascimento,
            aluno.Papel.ToString());
}