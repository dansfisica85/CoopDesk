using CoopDesk.Application.Dtos;

namespace CoopDesk.Application.Services;

public interface IReferenceDataService
{
    Task<IReadOnlyCollection<LookupItemDto>> ListDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LookupItemDto>> ListCollaboratorsAsync(CancellationToken cancellationToken = default);
}
