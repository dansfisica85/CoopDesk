using CoopDesk.Domain.Enums;

namespace CoopDesk.Application.Dtos;

public sealed record TicketSummaryDto(
    Guid Id,
    string Title,
    TicketPriority Priority,
    TicketStatus Status,
    string RequesterName,
    string DepartmentName,
    string? AssignedAgentName,
    DateTime CreatedAtUtc,
    DateTime? DueAtUtc);
