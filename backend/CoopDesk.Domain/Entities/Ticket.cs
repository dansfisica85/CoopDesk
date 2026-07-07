using CoopDesk.Domain.Common;
using CoopDesk.Domain.Enums;

namespace CoopDesk.Domain.Entities;

public sealed class Ticket : Entity
{
    private Ticket()
    {
    }

    public Ticket(
        string title,
        string description,
        TicketPriority priority,
        Guid requesterId,
        Guid departmentId,
        string createdBy,
        SupportProblemType problemType = SupportProblemType.Other)
    {
        Title = RequireText(title, nameof(title));
        Description = RequireText(description, nameof(description));
        ProblemType = problemType;
        Priority = priority;
        Status = TicketStatus.Open;
        RequesterId = requesterId;
        DepartmentId = departmentId;
        History.Add(new TicketHistory(Id, null, Status, "Chamado aberto.", createdBy));
    }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public SupportProblemType ProblemType { get; private set; } = SupportProblemType.Other;
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public Guid RequesterId { get; private set; }
    public Collaborator? Requester { get; private set; }
    public Guid? AssignedAgentId { get; private set; }
    public Collaborator? AssignedAgent { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Department? Department { get; private set; }
    public DateTime? DueAtUtc { get; private set; }
    public DateTime? ResolvedAtUtc { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public ICollection<TicketHistory> History { get; private set; } = new List<TicketHistory>();

    public void UpdateDetails(string title, string description, TicketPriority priority, Guid departmentId, DateTime? dueAtUtc, SupportProblemType? problemType = null)
    {
        EnsureEditable();

        Title = RequireText(title, nameof(title));
        Description = RequireText(description, nameof(description));
        ProblemType = problemType ?? ProblemType;
        Priority = priority;
        DepartmentId = departmentId;
        DueAtUtc = dueAtUtc;
        Touch();
    }

    public void AssignTo(Guid? agentId, string performedBy)
    {
        EnsureEditable();

        AssignedAgentId = agentId;
        History.Add(new TicketHistory(Id, Status, Status, agentId.HasValue ? "Responsavel atribuido." : "Responsavel removido.", performedBy));
        Touch();
    }

    public void ChangeStatus(TicketStatus newStatus, string notes, string performedBy)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Canceled)
        {
            throw new InvalidOperationException("Closed or canceled tickets cannot be changed.");
        }

        var previous = Status;
        Status = newStatus;

        if (newStatus == TicketStatus.Resolved)
        {
            ResolvedAtUtc = DateTime.UtcNow;
        }

        if (newStatus == TicketStatus.Closed || newStatus == TicketStatus.Canceled)
        {
            ClosedAtUtc = DateTime.UtcNow;
        }

        History.Add(new TicketHistory(Id, previous, newStatus, notes, performedBy));
        Touch();
    }

    private void EnsureEditable()
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Canceled)
        {
            throw new InvalidOperationException("Closed or canceled tickets cannot be edited.");
        }
    }

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty.", parameterName);
        }

        return value.Trim();
    }
}
