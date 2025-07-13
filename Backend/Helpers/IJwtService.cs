using System.IdentityModel.Tokens.Jwt;

namespace Backend.Helpers;

public interface IJwtService
{
    string GenerateToken(int id);
    JwtSecurityToken? ValidateToken(string token);
}
