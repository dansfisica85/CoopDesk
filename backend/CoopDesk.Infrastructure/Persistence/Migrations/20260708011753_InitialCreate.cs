using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoopDesk.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "support");

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collaborators",
                schema: "support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collaborators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collaborators_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "support",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                schema: "support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProblemType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RequesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Collaborators_AssignedAgentId",
                        column: x => x.AssignedAgentId,
                        principalSchema: "support",
                        principalTable: "Collaborators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Collaborators_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "support",
                        principalTable: "Collaborators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "support",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CollaboratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Collaborators_CollaboratorId",
                        column: x => x.CollaboratorId,
                        principalSchema: "support",
                        principalTable: "Collaborators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketHistories",
                schema: "support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    NewStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketHistories_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "support",
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "support",
                table: "Departments",
                columns: new[] { "Id", "CreatedAtUtc", "IsActive", "Name", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("534652fd-dffa-4d30-b270-ce46b0a46b0d"), new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), true, "Operacoes", null },
                    { new Guid("5d6ef6ef-5f87-47f4-8a8e-e35708e5e101"), new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), true, "Tecnologia", null },
                    { new Guid("f6070747-ed08-4a7f-94d0-ddcc7851cb7f"), new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), true, "Credito", null }
                });

            migrationBuilder.InsertData(
                schema: "support",
                table: "Users",
                columns: new[] { "Id", "CollaboratorId", "CreatedAtUtc", "Email", "FullName", "IsActive", "PasswordHash", "Role", "UpdatedAtUtc" },
                values: new object[] { new Guid("40d30e39-4c09-4108-90f2-64895c8ae701"), null, new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), "admin@coopdesk.local", "Administrador Demo", true, "PBKDF2$100000$BmILyUgJIjH4aZwOXR5ozA==$rOnI6VqfnVp51b9MaK7dA5myVY8P03lUFCicYddbN98=", "Administrator", null });

            migrationBuilder.InsertData(
                schema: "support",
                table: "Collaborators",
                columns: new[] { "Id", "CreatedAtUtc", "DepartmentId", "Email", "FullName", "IsActive", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("0dba1335-4389-4d47-bff5-8f2b27b5d901"), new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("534652fd-dffa-4d30-b270-ce46b0a46b0d"), "ana.souza@coopdesk.local", "Ana Souza", true, null },
                    { new Guid("a471f67e-03ab-4d81-a4cd-53b903a59e7f"), new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("f6070747-ed08-4a7f-94d0-ddcc7851cb7f"), "carla.mendes@coopdesk.local", "Carla Mendes", true, null },
                    { new Guid("fb7c7217-6d70-4a56-857e-bfdb87170c2e"), new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("5d6ef6ef-5f87-47f4-8a8e-e35708e5e101"), "bruno.lima@coopdesk.local", "Bruno Lima", true, null }
                });

            migrationBuilder.InsertData(
                schema: "support",
                table: "Tickets",
                columns: new[] { "Id", "AssignedAgentId", "ClosedAtUtc", "CreatedAtUtc", "DepartmentId", "Description", "DueAtUtc", "Priority", "ProblemType", "RequesterId", "ResolvedAtUtc", "Status", "Title", "UpdatedAtUtc" },
                values: new object[] { new Guid("e4fd358e-b413-4a3e-aa5d-419112995001"), new Guid("fb7c7217-6d70-4a56-857e-bfdb87170c2e"), null, new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("5d6ef6ef-5f87-47f4-8a8e-e35708e5e101"), "Tela legada apresenta timeout ao buscar propostas com muitas parcelas.", new DateTime(2026, 7, 9, 12, 0, 0, 0, DateTimeKind.Utc), "High", "SystemError", new Guid("a471f67e-03ab-4d81-a4cd-53b903a59e7f"), null, "Open", "Falha ao consultar proposta de credito", null });

            migrationBuilder.InsertData(
                schema: "support",
                table: "Users",
                columns: new[] { "Id", "CollaboratorId", "CreatedAtUtc", "Email", "FullName", "IsActive", "PasswordHash", "Role", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("03c7b3fb-c8f2-4a4f-8b6d-070c2f738702"), new Guid("fb7c7217-6d70-4a56-857e-bfdb87170c2e"), new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), "atendente@coopdesk.local", "Bruno Lima", true, "PBKDF2$100000$UToCSteUYZ2ZuMONaNTlvA==$OwHgBWAAwjySiIcl1by4wg4PtfyZGHUllBiwVgct2aA=", "Agent", null },
                    { new Guid("35e626a2-f4cc-4072-a2cd-a1437e255703"), new Guid("a471f67e-03ab-4d81-a4cd-53b903a59e7f"), new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), "solicitante@coopdesk.local", "Carla Mendes", true, "PBKDF2$100000$JMPytRZJ0LrGtuWOXOrv/A==$2X7sRd5DlKsO98uSlJ19UtCBZ4DZAjiBrWdwQxg+JjA=", "Requester", null }
                });

            migrationBuilder.InsertData(
                schema: "support",
                table: "TicketHistories",
                columns: new[] { "Id", "CreatedAtUtc", "NewStatus", "Notes", "OccurredAtUtc", "PerformedBy", "PreviousStatus", "TicketId", "UpdatedAtUtc" },
                values: new object[] { new Guid("ce7a2365-62fc-40fd-b71e-60b809834001"), new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), "Open", "Chamado aberto.", new DateTime(2026, 7, 7, 12, 0, 0, 0, DateTimeKind.Utc), "seed", null, new Guid("e4fd358e-b413-4a3e-aa5d-419112995001"), null });

            migrationBuilder.CreateIndex(
                name: "IX_Collaborators_DepartmentId",
                schema: "support",
                table: "Collaborators",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Collaborators_Email",
                schema: "support",
                table: "Collaborators",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_TicketId",
                schema: "support",
                table: "TicketHistories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedAgentId",
                schema: "support",
                table: "Tickets",
                column: "AssignedAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatedAtUtc",
                schema: "support",
                table: "Tickets",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_DepartmentId",
                schema: "support",
                table: "Tickets",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Priority",
                schema: "support",
                table: "Tickets",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_RequesterId",
                schema: "support",
                table: "Tickets",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Status",
                schema: "support",
                table: "Tickets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CollaboratorId",
                schema: "support",
                table: "Users",
                column: "CollaboratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "support",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketHistories",
                schema: "support");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "support");

            migrationBuilder.DropTable(
                name: "Tickets",
                schema: "support");

            migrationBuilder.DropTable(
                name: "Collaborators",
                schema: "support");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "support");
        }
    }
}
