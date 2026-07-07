using CoopDesk.Domain.Common;

namespace CoopDesk.Domain.Entities;

public sealed class Collaborator : Entity
{
    private Collaborator()
    {
    }

    public Collaborator(string fullName, string email, Guid departmentId)
    {
        FullName = RequireText(fullName, nameof(fullName));
        Email = RequireText(email, nameof(email)).ToLowerInvariant();
        DepartmentId = departmentId;
    }

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public Guid DepartmentId { get; private set; }
    public Department? Department { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<Ticket> RequestedTickets { get; private set; } = new List<Ticket>();
    public ICollection<Ticket> AssignedTickets { get; private set; } = new List<Ticket>();

    public void UpdateProfile(string fullName, string email, Guid departmentId)
    {
        FullName = RequireText(fullName, nameof(fullName));
        Email = RequireText(email, nameof(email)).ToLowerInvariant();
        DepartmentId = departmentId;
        Touch();
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
