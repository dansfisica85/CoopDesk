using CoopDesk.Application.Dtos;
using CoopDesk.Application.Interfaces;
using CoopDesk.Domain.Entities;

namespace CoopDesk.Application.Services;

public sealed class TicketService(ITicketRepository ticketRepository) : ITicketService
{
    public async Task<IReadOnlyCollection<TicketSummaryDto>> SearchAsync(TicketQuery query, CancellationToken cancellationToken = default)
    {
        var tickets = await ticketRepository.SearchAsync(query, cancellationToken);
        return tickets.Select(MapSummary).ToList();
    }

    public async Task<TicketDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await ticketRepository.GetByIdAsync(id, cancellationToken);
        return ticket is null ? null : MapDetail(ticket);
    }

    public async Task<TicketDetailDto> CreateAsync(CreateTicketRequest request, string performedBy, CancellationToken cancellationToken = default)
    {
        var ticket = new Ticket(
            request.Title,
            request.Description,
            request.Priority,
            request.RequesterId,
            request.DepartmentId,
            performedBy,
            request.ProblemType);

        if (request.DueAtUtc.HasValue)
        {
            ticket.UpdateDetails(request.Title, request.Description, request.Priority, request.DepartmentId, request.DueAtUtc, request.ProblemType);
        }

        await ticketRepository.AddAsync(ticket, cancellationToken);
        await ticketRepository.SaveChangesAsync(cancellationToken);

        var created = await ticketRepository.GetByIdAsync(ticket.Id, cancellationToken);
        return MapDetail(created ?? ticket);
    }

    public async Task<TicketDetailDto?> UpdateAsync(Guid id, UpdateTicketRequest request, CancellationToken cancellationToken = default)
    {
        var ticket = await ticketRepository.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        ticket.UpdateDetails(request.Title, request.Description, request.Priority, request.DepartmentId, request.DueAtUtc);
        await ticketRepository.SaveChangesAsync(cancellationToken);
        return MapDetail(ticket);
    }

    public async Task<TicketDetailDto?> AssignAsync(Guid id, AssignTicketRequest request, CancellationToken cancellationToken = default)
    {
        var ticket = await ticketRepository.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        ticket.AssignTo(request.AgentId, request.PerformedBy);
        await ticketRepository.SaveChangesAsync(cancellationToken);
        return MapDetail(ticket);
    }

    public async Task<TicketDetailDto?> ChangeStatusAsync(Guid id, ChangeTicketStatusRequest request, CancellationToken cancellationToken = default)
    {
        var ticket = await ticketRepository.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        ticket.ChangeStatus(request.Status, request.Notes, request.PerformedBy);
        await ticketRepository.SaveChangesAsync(cancellationToken);
        return MapDetail(ticket);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await ticketRepository.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return false;
        }

        ticketRepository.Remove(ticket);
        await ticketRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TicketSummaryDto MapSummary(Ticket ticket)
    {
        return new TicketSummaryDto(
            ticket.Id,
            ticket.Title,
            ticket.ProblemType,
            ticket.Priority,
            ticket.Status,
            ticket.Requester?.FullName ?? "Nao informado",
            ticket.Department?.Name ?? "Nao informado",
            ticket.AssignedAgent?.FullName,
            ticket.CreatedAtUtc,
            ticket.DueAtUtc);
    }

    private static TicketDetailDto MapDetail(Ticket ticket)
    {
        return new TicketDetailDto(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.ProblemType,
            ticket.Priority,
            ticket.Status,
            ticket.RequesterId,
            ticket.Requester?.FullName ?? "Nao informado",
            ticket.DepartmentId,
            ticket.Department?.Name ?? "Nao informado",
            ticket.AssignedAgentId,
            ticket.AssignedAgent?.FullName,
            ticket.CreatedAtUtc,
            ticket.DueAtUtc,
            ticket.ResolvedAtUtc,
            ticket.ClosedAtUtc,
            ticket.History
                .OrderByDescending(history => history.OccurredAtUtc)
                .Select(history => new TicketHistoryDto(
                    history.Id,
                    history.PreviousStatus,
                    history.NewStatus,
                    history.Notes,
                    history.PerformedBy,
                    history.OccurredAtUtc))
                .ToList());
    }
}
