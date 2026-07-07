using CoopDesk.Domain.Enums;

namespace CoopDesk.Application.Dtos;

public sealed record CreateSupportRequest(
    string RequesterName,
    string RequesterEmail,
    Guid DepartmentId,
    SupportProblemType ProblemType,
    string Description);
