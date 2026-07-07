using CoopDesk.Application.Dtos;

namespace CoopDesk.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
