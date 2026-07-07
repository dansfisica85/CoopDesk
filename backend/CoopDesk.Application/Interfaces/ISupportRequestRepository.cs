using CoopDesk.Domain.Entities;

namespace CoopDesk.Application.Interfaces;

public interface ISupportRequestRepository
{
    Task<bool> DepartmentExistsAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<Collaborator?> GetRequesterByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddRequesterAsync(Collaborator requester, CancellationToken cancellationToken = default);
    Task AddTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
