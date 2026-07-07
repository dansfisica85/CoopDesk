using CoopDesk.Domain.Enums;

namespace CoopDesk.Application.Dtos;

public sealed record TicketHistoryDto(
    Guid Id,
    TicketStatus? PreviousStatus,
    TicketStatus NewStatus,
    string Notes,
    string PerformedBy,
    DateTime OccurredAtUtc);
