using CoopDesk.Application.Interfaces;
using CoopDesk.Domain.Entities;
using CoopDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoopDesk.Infrastructure.Repositories;

public sealed class UserRepository(CoopDeskDbContext dbContext) : IUserRepository
{
    public Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }
}
