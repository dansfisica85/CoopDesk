using CoopDesk.Domain.Entities;
using CoopDesk.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CoopDesk.Infrastructure.Persistence;

public sealed class CoopDeskDbContext(DbContextOptions<CoopDeskDbContext> options) : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Collaborator> Collaborators => Set<Collaborator>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("support");

        ConfigureDepartments(modelBuilder);
        ConfigureCollaborators(modelBuilder);
        ConfigureUsers(modelBuilder);
        ConfigureTickets(modelBuilder);
        ConfigureTicketHistories(modelBuilder);
        Seed(modelBuilder);
    }

    private static void ConfigureDepartments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(builder =>
        {
            builder.ToTable("Departments");
            builder.HasKey(department => department.Id);
            builder.Property(department => department.Name).HasMaxLength(120).IsRequired();
            builder.Property(department => department.IsActive).IsRequired();
            builder.Property(department => department.CreatedAtUtc).IsRequired();
        });
    }

    private static void ConfigureCollaborators(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Collaborator>(builder =>
        {
            builder.ToTable("Collaborators");
            builder.HasKey(collaborator => collaborator.Id);
            builder.Property(collaborator => collaborator.FullName).HasMaxLength(160).IsRequired();
            builder.Property(collaborator => collaborator.Email).HasMaxLength(180).IsRequired();
            builder.HasIndex(collaborator => collaborator.Email).IsUnique();
            builder.Property(collaborator => collaborator.IsActive).IsRequired();
            builder.Property(collaborator => collaborator.CreatedAtUtc).IsRequired();

            builder
                .HasOne(collaborator => collaborator.Department)
                .WithMany()
                .HasForeignKey(collaborator => collaborator.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(user => user.Id);
            builder.Property(user => user.FullName).HasMaxLength(160).IsRequired();
            builder.Property(user => user.Email).HasMaxLength(180).IsRequired();
            builder.HasIndex(user => user.Email).IsUnique();
            builder.Property(user => user.PasswordHash).HasMaxLength(220).IsRequired();
            builder.Property(user => user.Role).HasConversion<string>().HasMaxLength(32).IsRequired();
            builder.Property(user => user.IsActive).IsRequired();
            builder.Property(user => user.CreatedAtUtc).IsRequired();

            builder
                .HasOne(user => user.Collaborator)
                .WithMany()
                .HasForeignKey(user => user.CollaboratorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureTickets(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(builder =>
        {
            builder.ToTable("Tickets");
            builder.HasKey(ticket => ticket.Id);
            builder.Property(ticket => ticket.Title).HasMaxLength(180).IsRequired();
            builder.Property(ticket => ticket.Description).HasMaxLength(4000).IsRequired();
            builder.Property(ticket => ticket.Priority).HasConversion<string>().HasMaxLength(24).IsRequired();
            builder.Property(ticket => ticket.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            builder.Property(ticket => ticket.CreatedAtUtc).IsRequired();
            builder.Property(ticket => ticket.DueAtUtc);
            builder.HasIndex(ticket => ticket.Status);
            builder.HasIndex(ticket => ticket.Priority);
            builder.HasIndex(ticket => ticket.CreatedAtUtc);

            builder
                .HasOne(ticket => ticket.Requester)
                .WithMany(collaborator => collaborator.RequestedTickets)
                .HasForeignKey(ticket => ticket.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(ticket => ticket.AssignedAgent)
                .WithMany(collaborator => collaborator.AssignedTickets)
                .HasForeignKey(ticket => ticket.AssignedAgentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(ticket => ticket.Department)
                .WithMany()
                .HasForeignKey(ticket => ticket.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(ticket => ticket.History)
                .WithOne(history => history.Ticket)
                .HasForeignKey(history => history.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTicketHistories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketHistory>(builder =>
        {
            builder.ToTable("TicketHistories");
            builder.HasKey(history => history.Id);
            builder.Property(history => history.PreviousStatus).HasConversion<string>().HasMaxLength(32);
            builder.Property(history => history.NewStatus).HasConversion<string>().HasMaxLength(32).IsRequired();
            builder.Property(history => history.Notes).HasMaxLength(1000).IsRequired();
            builder.Property(history => history.PerformedBy).HasMaxLength(120).IsRequired();
            builder.Property(history => history.OccurredAtUtc).IsRequired();
            builder.Property(history => history.CreatedAtUtc).IsRequired();
        });
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        var technologyId = Guid.Parse("5d6ef6ef-5f87-47f4-8a8e-e35708e5e101");
        var operationsId = Guid.Parse("534652fd-dffa-4d30-b270-ce46b0a46b0d");
        var creditId = Guid.Parse("f6070747-ed08-4a7f-94d0-ddcc7851cb7f");

        var anaId = Guid.Parse("0dba1335-4389-4d47-bff5-8f2b27b5d901");
        var brunoId = Guid.Parse("fb7c7217-6d70-4a56-857e-bfdb87170c2e");
        var carlaId = Guid.Parse("a471f67e-03ab-4d81-a4cd-53b903a59e7f");

        var adminUserId = Guid.Parse("40d30e39-4c09-4108-90f2-64895c8ae701");
        var agentUserId = Guid.Parse("03c7b3fb-c8f2-4a4f-8b6d-070c2f738702");
        var requesterUserId = Guid.Parse("35e626a2-f4cc-4072-a2cd-a1437e255703");

        var ticketId = Guid.Parse("e4fd358e-b413-4a3e-aa5d-419112995001");
        var createdAt = new DateTime(2026, 7, 7, 12, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Department>().HasData(
            new { Id = technologyId, Name = "Tecnologia", IsActive = true, CreatedAtUtc = createdAt, UpdatedAtUtc = (DateTime?)null },
            new { Id = operationsId, Name = "Operacoes", IsActive = true, CreatedAtUtc = createdAt, UpdatedAtUtc = (DateTime?)null },
            new { Id = creditId, Name = "Credito", IsActive = true, CreatedAtUtc = createdAt, UpdatedAtUtc = (DateTime?)null });

        modelBuilder.Entity<Collaborator>().HasData(
            new { Id = anaId, FullName = "Ana Souza", Email = "ana.souza@coopdesk.local", DepartmentId = operationsId, IsActive = true, CreatedAtUtc = createdAt, UpdatedAtUtc = (DateTime?)null },
            new { Id = brunoId, FullName = "Bruno Lima", Email = "bruno.lima@coopdesk.local", DepartmentId = technologyId, IsActive = true, CreatedAtUtc = createdAt, UpdatedAtUtc = (DateTime?)null },
            new { Id = carlaId, FullName = "Carla Mendes", Email = "carla.mendes@coopdesk.local", DepartmentId = creditId, IsActive = true, CreatedAtUtc = createdAt, UpdatedAtUtc = (DateTime?)null });

        modelBuilder.Entity<AppUser>().HasData(
            new
            {
                Id = adminUserId,
                FullName = "Administrador Demo",
                Email = "admin@coopdesk.local",
                PasswordHash = "PBKDF2$100000$BmILyUgJIjH4aZwOXR5ozA==$rOnI6VqfnVp51b9MaK7dA5myVY8P03lUFCicYddbN98=",
                Role = UserRole.Administrator,
                CollaboratorId = (Guid?)null,
                IsActive = true,
                CreatedAtUtc = createdAt,
                UpdatedAtUtc = (DateTime?)null
            },
            new
            {
                Id = agentUserId,
                FullName = "Bruno Lima",
                Email = "atendente@coopdesk.local",
                PasswordHash = "PBKDF2$100000$UToCSteUYZ2ZuMONaNTlvA==$OwHgBWAAwjySiIcl1by4wg4PtfyZGHUllBiwVgct2aA=",
                Role = UserRole.Agent,
                CollaboratorId = (Guid?)brunoId,
                IsActive = true,
                CreatedAtUtc = createdAt,
                UpdatedAtUtc = (DateTime?)null
            },
            new
            {
                Id = requesterUserId,
                FullName = "Carla Mendes",
                Email = "solicitante@coopdesk.local",
                PasswordHash = "PBKDF2$100000$JMPytRZJ0LrGtuWOXOrv/A==$2X7sRd5DlKsO98uSlJ19UtCBZ4DZAjiBrWdwQxg+JjA=",
                Role = UserRole.Requester,
                CollaboratorId = (Guid?)carlaId,
                IsActive = true,
                CreatedAtUtc = createdAt,
                UpdatedAtUtc = (DateTime?)null
            });

        modelBuilder.Entity<Ticket>().HasData(new
        {
            Id = ticketId,
            Title = "Falha ao consultar proposta de credito",
            Description = "Tela legada apresenta timeout ao buscar propostas com muitas parcelas.",
            Priority = TicketPriority.High,
            Status = TicketStatus.Open,
            RequesterId = carlaId,
            AssignedAgentId = brunoId,
            DepartmentId = technologyId,
            DueAtUtc = createdAt.AddDays(2),
            ResolvedAtUtc = (DateTime?)null,
            ClosedAtUtc = (DateTime?)null,
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = (DateTime?)null
        });

        modelBuilder.Entity<TicketHistory>().HasData(new
        {
            Id = Guid.Parse("ce7a2365-62fc-40fd-b71e-60b809834001"),
            TicketId = ticketId,
            PreviousStatus = (TicketStatus?)null,
            NewStatus = TicketStatus.Open,
            Notes = "Chamado aberto.",
            PerformedBy = "seed",
            OccurredAtUtc = createdAt,
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = (DateTime?)null
        });
    }
}
