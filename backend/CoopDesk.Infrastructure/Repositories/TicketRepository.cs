using CoopDesk.Application.Dtos;
using CoopDesk.Application.Interfaces;
using CoopDesk.Domain.Entities;
using CoopDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoopDesk.Infrastructure.Repositories;

public sealed class TicketRepository(CoopDeskDbContext dbContext) : ITicketRepository
{
    public async Task<IReadOnlyCollection<Ticket>> SearchAsync(TicketQuery query, CancellationToken cancellationToken = default)
    {
        var tickets = dbContext.Tickets
            .AsNoTracking()
            .Include(ticket => ticket.Requester)
            .Include(ticket => ticket.Department)
            .Include(ticket => ticket.AssignedAgent)
            .AsQueryable();

        if (query.Status.HasValue)
        {
            tickets = tickets.Where(ticket => ticket.Status == query.Status.Value);
        }

        if (query.Priority.HasValue)
        {
            tickets = tickets.Where(ticket => ticket.Priority == query.Priority.Value);
        }

        if (query.ProblemType.HasValue)
        {
            tickets = tickets.Where(ticket => ticket.ProblemType == query.ProblemType.Value);
        }

        if (query.DepartmentId.HasValue)
        {
            tickets = tickets.Where(ticket => ticket.DepartmentId == query.DepartmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            tickets = tickets.Where(ticket =>
                ticket.Title.Contains(search) ||
                ticket.Description.Contains(search) ||
                ticket.Requester!.FullName.Contains(search));
        }

        return await tickets
            .OrderByDescending(ticket => ticket.Priority)
            .ThenBy(ticket => ticket.CreatedAtUtc)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Tickets
            .Include(ticket => ticket.Requester)
            .Include(ticket => ticket.Department)
            .Include(ticket => ticket.AssignedAgent)
            .Include(ticket => ticket.History)
            .FirstOrDefaultAsync(ticket => ticket.Id == id, cancellationToken);
    }

    public Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        return dbContext.Tickets.AddAsync(ticket, cancellationToken).AsTask();
    }

    public void Remove(Ticket ticket)
    {
        dbContext.Tickets.Remove(ticket);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
