using CoopDesk.Domain.Common;
using CoopDesk.Domain.Enums;

namespace CoopDesk.Domain.Entities;

public sealed class AppUser : Entity
{
    private AppUser()
    {
    }

    public AppUser(string fullName, string email, string passwordHash, UserRole role, Guid? collaboratorId = null)
    {
        FullName = RequireText(fullName, nameof(fullName));
        Email = RequireText(email, nameof(email)).ToLowerInvariant();
        PasswordHash = RequireText(passwordHash, nameof(passwordHash));
        Role = role;
        CollaboratorId = collaboratorId;
    }

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public Guid? CollaboratorId { get; private set; }
    public Collaborator? Collaborator { get; private set; }
    public bool IsActive { get; private set; } = true;

    public void Deactivate()
    {
        IsActive = false;
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
