using CoopDesk.Application.Dtos;
using CoopDesk.Domain.Entities;

namespace CoopDesk.Application.Security;

public interface IJwtTokenService
{
    AuthResponseDto CreateToken(AppUser user);
}
