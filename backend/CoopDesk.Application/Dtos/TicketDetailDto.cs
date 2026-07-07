using CoopDesk.Domain.Enums;

namespace CoopDesk.Application.Dtos;

public sealed record TicketDetailDto(
    Guid Id,
    string Title,
    string Description,
    TicketPriority Priority,
    TicketStatus Status,
    Guid RequesterId,
    string RequesterName,
    Guid DepartmentId,
    string DepartmentName,
    Guid? AssignedAgentId,
    string? AssignedAgentName,
    DateTime CreatedAtUtc,
    DateTime? DueAtUtc,
    DateTime? ResolvedAtUtc,
    DateTime? ClosedAtUtc,
    IReadOnlyCollection<TicketHistoryDto> History);
