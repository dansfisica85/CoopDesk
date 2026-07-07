using System.Text.Json.Serialization;
using CoopDesk.Api.Middleware;
using CoopDesk.Application.Services;
using CoopDesk.Infrastructure;
using CoopDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IReferenceDataService, ReferenceDataService>();

var connectionString = builder.Configuration.GetConnectionString("CoopDesk")
    ?? throw new InvalidOperationException("Connection string 'CoopDesk' was not found.");

builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await TryEnsureDatabaseAsync(app);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AngularDev");
app.MapControllers();

await app.RunAsync();

static async Task TryEnsureDatabaseAsync(WebApplication app)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CoopDeskDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
    catch (Exception exception)
    {
        app.Logger.LogWarning(exception, "Database was not created automatically. Check SQL Server LocalDB or update the connection string.");
    }
}
