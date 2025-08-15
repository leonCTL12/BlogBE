using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlogBE.Jwt;

public class JwtTokenFactory
{
    private readonly JwtOptions _options;

    public JwtTokenFactory(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public (string token, DateTime expiresAt) CreateToken(DB.User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_options.ExpirationMinutes);

        var jwt = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            now, expiresAt,
            creds
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token, expiresAt);
    }
}