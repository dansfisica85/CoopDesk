namespace CoopDesk.Application.Dtos;

public sealed record AssignTicketRequest(
    Guid? AgentId,
    string PerformedBy);
