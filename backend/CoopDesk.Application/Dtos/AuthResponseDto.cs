namespace CoopDesk.Application.Dtos;

public sealed record AuthResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    AuthenticatedUserDto User);
