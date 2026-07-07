using CoopDesk.Domain.Enums;

namespace CoopDesk.Application.Dtos;

public sealed record CreateTicketRequest(
    string Title,
    string Description,
    SupportProblemType ProblemType,
    TicketPriority Priority,
    Guid RequesterId,
    Guid DepartmentId,
    DateTime? DueAtUtc);
