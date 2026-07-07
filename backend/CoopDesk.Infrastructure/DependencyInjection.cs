using CoopDesk.Application.Interfaces;
using CoopDesk.Application.Security;
using CoopDesk.Infrastructure.Persistence;
using CoopDesk.Infrastructure.Repositories;
using CoopDesk.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CoopDesk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CoopDeskDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IReferenceDataRepository, ReferenceDataRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHashService, Pbkdf2PasswordHashService>();

        return services;
    }
}
