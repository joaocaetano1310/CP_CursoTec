using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CP1_CursoTec.Application.Interfaces;
using CP1_CursoTec.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
// Ajuste este using para o namespace que está no topo do seu JwtOptions.cs:
using CP1_CursoTec.API.Options;

namespace CP1_CursoTec.Security;

/// <summary>
/// Implementação do <see cref="ITokenGenerator"/> com JWT assinado em HMAC-SHA256.
/// Lê Issuer, Audience, Key e ExpirationMinutes da seção "Jwt" do appsettings (via <see cref="JwtOptions"/>).
/// O token carrega as claims sub (id), email, name e role — nunca a senha.
/// </summary>
public class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;

        // HS256 exige chave de pelo menos 256 bits (32 bytes). Falha cedo e com mensagem clara.
        if (string.IsNullOrWhiteSpace(_options.Key) || Encoding.UTF8.GetByteCount(_options.Key) < 32)
            throw new InvalidOperationException(
                "Jwt:Key ausente ou com menos de 32 caracteres. Configure a seção \"Jwt\" no appsettings.json.");
    }

    public TokenGerado Gerar(Aluno aluno)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, aluno.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, aluno.Email),
            new("name", aluno.Nome),
            new("role", aluno.Papel.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiracao = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiracao,
            signingCredentials: credenciais);

        var tokenTexto = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenGerado(tokenTexto, expiracao);
    }
}