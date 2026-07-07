using CoopDesk.Domain.Enums;

namespace CoopDesk.Application.Dtos;

public sealed record ChangeTicketStatusRequest(
    TicketStatus Status,
    string Notes,
    string PerformedBy);
