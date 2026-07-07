using CoopDesk.Application.Dtos;
using CoopDesk.Application.Interfaces;
using CoopDesk.Application.Services;
using CoopDesk.Domain.Entities;
using CoopDesk.Domain.Enums;

namespace CoopDesk.Tests;

public sealed class TicketServiceTests
{
    [Fact]
    public async Task CreateAsync_creates_open_ticket_with_history()
    {
        var repository = new FakeTicketRepository();
        var service = new TicketService(repository);

        var request = new CreateTicketRequest(
            "Acesso ao sistema legado",
            "Usuario nao consegue acessar a tela de propostas.",
            TicketPriority.High,
            Guid.NewGuid(),
            Guid.NewGuid(),
            null);

        var ticket = await service.CreateAsync(request, "unit-test");

        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.Equal(TicketPriority.High, ticket.Priority);
        Assert.NotEmpty(ticket.History);
    }

    [Fact]
    public async Task ChangeStatusAsync_moves_ticket_to_in_progress()
    {
        var repository = new FakeTicketRepository();
        var service = new TicketService(repository);
        var ticket = new Ticket(
            "Erro em relatorio",
            "Relatorio mensal nao abre para a area de credito.",
            TicketPriority.Medium,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "unit-test");

        await repository.AddAsync(ticket);

        var updated = await service.ChangeStatusAsync(
            ticket.Id,
            new ChangeTicketStatusRequest(TicketStatus.InProgress, "Analise iniciada.", "unit-test"));

        Assert.NotNull(updated);
        Assert.Equal(TicketStatus.InProgress, updated.Status);
        Assert.Contains(updated.History, history => history.NewStatus == TicketStatus.InProgress);
    }

    private sealed class FakeTicketRepository : ITicketRepository
    {
        private readonly List<Ticket> _tickets = [];

        public Task<IReadOnlyCollection<Ticket>> SearchAsync(TicketQuery query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Ticket>>(_tickets);
        }

        public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_tickets.SingleOrDefault(ticket => ticket.Id == id));
        }

        public Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
        {
            _tickets.Add(ticket);
            return Task.CompletedTask;
        }

        public void Remove(Ticket ticket)
        {
            _tickets.Remove(ticket);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
