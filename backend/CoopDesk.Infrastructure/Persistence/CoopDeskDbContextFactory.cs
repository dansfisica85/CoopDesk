using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoopDesk.Infrastructure.Persistence;

public sealed class CoopDeskDbContextFactory : IDesignTimeDbContextFactory<CoopDeskDbContext>
{
    public CoopDeskDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<CoopDeskDbContext>();
        builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CoopDeskDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new CoopDeskDbContext(builder.Options);
    }
}
