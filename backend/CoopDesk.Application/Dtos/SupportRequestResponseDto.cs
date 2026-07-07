using CoopDesk.Domain.Enums;

namespace CoopDesk.Application.Dtos;

public sealed record SupportRequestResponseDto(
    Guid TicketId,
    string Protocol,
    TicketStatus Status,
    DateTime CreatedAtUtc);
