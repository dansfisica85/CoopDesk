using CoopDesk.Domain.Enums;

namespace CoopDesk.Application.Dtos;

public sealed record UpdateTicketRequest(
    string Title,
    string Description,
    TicketPriority Priority,
    Guid DepartmentId,
    DateTime? DueAtUtc);
