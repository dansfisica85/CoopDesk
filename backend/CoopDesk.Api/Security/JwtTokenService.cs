using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoopDesk.Application.Dtos;
using CoopDesk.Application.Security;
using CoopDesk.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CoopDesk.Api.Security;

public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public AuthResponseDto CreateToken(AppUser user)
    {
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer was not configured.");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience was not configured.");
        var signingKey = configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey was not configured.");
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(configuration.GetValue("Jwt:ExpirationMinutes", 120));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var dto = new AuthenticatedUserDto(user.Id, user.FullName, user.Email, user.Role.ToString());
        return new AuthResponseDto(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc, dto);
    }
}
