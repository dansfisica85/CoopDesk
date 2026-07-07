using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using CoopDesk.Api.Middleware;
using CoopDesk.Api.Security;
using CoopDesk.Application.Security;
using CoopDesk.Application.Services;
using CoopDesk.Domain.Enums;
using CoopDesk.Infrastructure;
using CoopDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:4200"];

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IReferenceDataService, ReferenceDataService>();
builder.Services.AddScoped<ISupportRequestService, SupportRequestService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer was not configured.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience was not configured.");
var jwtSigningKey = builder.Configuration["Jwt:SigningKey"]
    ?? throw new InvalidOperationException("Jwt:SigningKey was not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SupportTeam", policy =>
    {
        policy.RequireRole(UserRole.Administrator.ToString(), UserRole.Agent.ToString());
    });
});

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

static async Task TryEnsureDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CoopDeskDbContext>();

    try
    {
        await dbContext.Database.EnsureCreatedAsync();
        _ = await dbContext.Users.AsNoTracking().AnyAsync();
        _ = await dbContext.Tickets.AsNoTracking().OrderBy(ticket => ticket.Id).Select(ticket => ticket.ProblemType).FirstOrDefaultAsync();
    }
    catch (Exception exception)
    {
        app.Logger.LogWarning(exception, "Database was not created automatically or has an old schema. Trying to recreate the development database.");

        try
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }
        catch (Exception recreationException)
        {
            app.Logger.LogWarning(recreationException, "Database recreation failed. Check SQL Server LocalDB or update the connection string.");
        }
    }
}
