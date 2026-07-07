using CoopDesk.Application.Dtos;

namespace CoopDesk.Application.Services;

public interface ITicketService
{
    Task<IReadOnlyCollection<TicketSummaryDto>> SearchAsync(TicketQuery query, CancellationToken cancellationToken = default);
    Task<TicketDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TicketDetailDto> CreateAsync(CreateTicketRequest request, string performedBy, CancellationToken cancellationToken = default);
    Task<TicketDetailDto?> UpdateAsync(Guid id, UpdateTicketRequest request, CancellationToken cancellationToken = default);
    Task<TicketDetailDto?> AssignAsync(Guid id, AssignTicketRequest request, CancellationToken cancellationToken = default);
    Task<TicketDetailDto?> ChangeStatusAsync(Guid id, ChangeTicketStatusRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
