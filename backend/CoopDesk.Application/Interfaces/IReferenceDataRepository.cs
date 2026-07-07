using CoopDesk.Application.Dtos;

namespace CoopDesk.Application.Interfaces;

public interface IReferenceDataRepository
{
    Task<IReadOnlyCollection<LookupItemDto>> ListDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LookupItemDto>> ListCollaboratorsAsync(CancellationToken cancellationToken = default);
}
