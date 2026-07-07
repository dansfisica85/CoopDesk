using CoopDesk.Domain.Enums;

namespace CoopDesk.Application.Dtos;

public sealed record TicketQuery(
    TicketStatus? Status,
    TicketPriority? Priority,
    SupportProblemType? ProblemType,
    Guid? DepartmentId,
    string? Search);
