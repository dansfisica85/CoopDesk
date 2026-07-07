namespace CoopDesk.Application.Dtos;

public sealed record AuthenticatedUserDto(
    Guid Id,
    string FullName,
    string Email,
    string Role);
