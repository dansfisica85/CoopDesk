using CoopDesk.Domain.Entities;

namespace CoopDesk.Application.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
