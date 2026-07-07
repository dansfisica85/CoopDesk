using CoopDesk.Application.Interfaces;
using CoopDesk.Infrastructure.Persistence;
using CoopDesk.Infrastructure.Repositories;
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

        return services;
    }
}
