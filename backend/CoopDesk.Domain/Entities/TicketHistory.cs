using CoopDesk.Domain.Common;
using CoopDesk.Domain.Enums;

namespace CoopDesk.Domain.Entities;

public sealed class TicketHistory : Entity
{
    private TicketHistory()
    {
    }

    internal TicketHistory(Guid ticketId, TicketStatus? previousStatus, TicketStatus newStatus, string notes, string performedBy)
    {
        TicketId = ticketId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Notes = string.IsNullOrWhiteSpace(notes) ? "Status atualizado." : notes.Trim();
        PerformedBy = string.IsNullOrWhiteSpace(performedBy) ? "system" : performedBy.Trim();
        OccurredAtUtc = DateTime.UtcNow;
    }

    public Guid TicketId { get; private set; }
    public Ticket? Ticket { get; private set; }
    public TicketStatus? PreviousStatus { get; private set; }
    public TicketStatus NewStatus { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public string PerformedBy { get; private set; } = string.Empty;
    public DateTime OccurredAtUtc { get; private set; }
}
