namespace CoopDesk.Application.Security;

public interface IPasswordHashService
{
    bool Verify(string password, string passwordHash);
}
