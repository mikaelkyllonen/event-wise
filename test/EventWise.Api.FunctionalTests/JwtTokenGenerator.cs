using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace EventWise.Api.FunctionalTests;

public static class JwtTokenGenerator
{
    public static string Issuer { get; } = "test-issuer";
    public static string Audience { get; } = "test-audience";
    public static string SigningKey { get; } = Guid.NewGuid().ToString();

    public static string GenerateToken(Guid? subject = null)
    {
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: [new Claim(ClaimTypes.NameIdentifier, subject.HasValue ? subject.ToString()! : UserData.DefaultUserGuid.ToString()), new Claim(ClaimTypes.Role, "user")],
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey)),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}