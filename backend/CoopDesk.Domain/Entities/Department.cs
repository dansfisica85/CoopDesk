using CoopDesk.Domain.Common;

namespace CoopDesk.Domain.Entities;

public sealed class Department : Entity
{
    private Department()
    {
    }

    public Department(string name)
    {
        Name = RequireText(name, nameof(name));
    }

    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public void Rename(string name)
    {
        Name = RequireText(name, nameof(name));
        Touch();
    }

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
