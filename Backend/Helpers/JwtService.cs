using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Helpers;

public class JwtService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtService(IConfiguration configuration)
    {
        _key = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing in configuration.");
        _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is missing in configuration.");
        _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience is missing in configuration.");
    }

    public string GenerateToken(int id)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var jwtHeader = new JwtHeader(signingCredentials);
        var jwtPayload = new JwtPayload(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1)
        );

        var securityToken = new JwtSecurityToken(jwtHeader, jwtPayload);
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public JwtSecurityToken? ValidateToken(string jwtToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);

            tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidateIssuer = true,
                ValidAudience = _audience,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return validatedToken as JwtSecurityToken;
        }
        catch
        {
            return null;
        }
    }
}