using CoopDesk.Application.Dtos;
using CoopDesk.Application.Interfaces;
using CoopDesk.Application.Security;
using CoopDesk.Application.Services;
using CoopDesk.Domain.Entities;
using CoopDesk.Domain.Enums;

namespace CoopDesk.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_returns_token_when_credentials_are_valid()
    {
        var user = new AppUser("Atendente Demo", "atendente@coopdesk.local", "hash", UserRole.Agent);
        var service = new AuthService(
            new FakeUserRepository(user),
            new FakePasswordHashService(isValid: true),
            new FakeJwtTokenService());

        var result = await service.LoginAsync(new LoginRequest("atendente@coopdesk.local", "Demo@12345"));

        Assert.NotNull(result);
        Assert.Equal("token-demo", result.AccessToken);
        Assert.Equal("Agent", result.User.Role);
    }

    [Fact]
    public async Task LoginAsync_returns_null_when_password_is_invalid()
    {
        var user = new AppUser("Atendente Demo", "atendente@coopdesk.local", "hash", UserRole.Agent);
        var service = new AuthService(
            new FakeUserRepository(user),
            new FakePasswordHashService(isValid: false),
            new FakeJwtTokenService());

        var result = await service.LoginAsync(new LoginRequest("atendente@coopdesk.local", "errada"));

        Assert.Null(result);
    }

    private sealed class FakeUserRepository(AppUser? user) : IUserRepository
    {
        public Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(user?.Email == email ? user : null);
        }
    }

    private sealed class FakePasswordHashService(bool isValid) : IPasswordHashService
    {
        public bool Verify(string password, string passwordHash)
        {
            return isValid;
        }
    }

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        public AuthResponseDto CreateToken(AppUser user)
        {
            var dto = new AuthenticatedUserDto(user.Id, user.FullName, user.Email, user.Role.ToString());
            return new AuthResponseDto("token-demo", DateTime.UtcNow.AddHours(1), dto);
        }
    }
}
