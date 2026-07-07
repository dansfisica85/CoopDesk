using CoopDesk.Application.Dtos;

namespace CoopDesk.Application.Services;

public interface ISupportRequestService
{
    Task<SupportRequestResponseDto> CreateAsync(CreateSupportRequest request, CancellationToken cancellationToken = default);
}
