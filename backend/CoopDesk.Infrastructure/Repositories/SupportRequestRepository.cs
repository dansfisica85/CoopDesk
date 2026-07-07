using CoopDesk.Application.Interfaces;
using CoopDesk.Domain.Entities;
using CoopDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoopDesk.Infrastructure.Repositories;

public sealed class SupportRequestRepository(CoopDeskDbContext dbContext) : ISupportRequestRepository
{
    public Task<bool> DepartmentExistsAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return dbContext.Departments
            .AnyAsync(department => department.Id == departmentId && department.IsActive, cancellationToken);
    }

    public Task<Collaborator?> GetRequesterByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return dbContext.Collaborators
            .FirstOrDefaultAsync(collaborator => collaborator.Email == email, cancellationToken);
    }

    public Task AddRequesterAsync(Collaborator requester, CancellationToken cancellationToken = default)
    {
        return dbContext.Collaborators.AddAsync(requester, cancellationToken).AsTask();
    }

    public Task AddTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        return dbContext.Tickets.AddAsync(ticket, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
