using CoopDesk.Application.Dtos;
using CoopDesk.Application.Interfaces;
using CoopDesk.Application.Services;
using CoopDesk.Domain.Entities;
using CoopDesk.Domain.Enums;

namespace CoopDesk.Tests;

public sealed class SupportRequestServiceTests
{
    [Fact]
    public async Task CreateAsync_creates_ticket_and_protocol_for_public_request()
    {
        var departmentId = Guid.NewGuid();
        var repository = new FakeSupportRequestRepository(departmentId);
        var service = new SupportRequestService(repository);

        var response = await service.CreateAsync(new CreateSupportRequest(
            "Maria Cliente",
            "maria@example.com",
            departmentId,
            SupportProblemType.SystemError,
            "Nao consigo finalizar uma operacao no sistema."));

        Assert.StartsWith("CD-", response.Protocol);
        Assert.Equal(TicketStatus.Open, response.Status);
        Assert.Single(repository.Tickets);
        Assert.Contains("maria@example.com", repository.Tickets.Single().Description);
    }

    private sealed class FakeSupportRequestRepository(Guid validDepartmentId) : ISupportRequestRepository
    {
        private readonly List<Collaborator> _requesters = [];

        public List<Ticket> Tickets { get; } = [];

        public Task<bool> DepartmentExistsAsync(Guid departmentId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(departmentId == validDepartmentId);
        }

        public Task<Collaborator?> GetRequesterByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_requesters.SingleOrDefault(requester => requester.Email == email));
        }

        public Task AddRequesterAsync(Collaborator requester, CancellationToken cancellationToken = default)
        {
            _requesters.Add(requester);
            return Task.CompletedTask;
        }

        public Task AddTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
        {
            Tickets.Add(ticket);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
