using CoopDesk.Application.Dtos;
using CoopDesk.Domain.Entities;

namespace CoopDesk.Application.Interfaces;

public interface ITicketRepository
{
    Task<IReadOnlyCollection<Ticket>> SearchAsync(TicketQuery query, CancellationToken cancellationToken = default);
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
    void Remove(Ticket ticket);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
