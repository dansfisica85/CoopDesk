using CoopDesk.Application.Dtos;
using CoopDesk.Application.Interfaces;
using CoopDesk.Application.Security;

namespace CoopDesk.Application.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<AuthResponseDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        return passwordHashService.Verify(request.Password, user.PasswordHash)
            ? jwtTokenService.CreateToken(user)
            : null;
    }
}
