using System.Security.Claims;
using CoopDesk.Application.Dtos;
using CoopDesk.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoopDesk.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        return result is null ? Unauthorized(new { message = "E-mail ou senha invalidos." }) : Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<AuthenticatedUserDto> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        var fullName = User.Identity?.Name ?? email;

        return Guid.TryParse(userId, out var id)
            ? Ok(new AuthenticatedUserDto(id, fullName, email, role))
            : Unauthorized();
    }
}
