using CoopDesk.Application.Dtos;
using CoopDesk.Application.Interfaces;

namespace CoopDesk.Application.Services;

public sealed class ReferenceDataService(IReferenceDataRepository referenceDataRepository) : IReferenceDataService
{
    public Task<IReadOnlyCollection<LookupItemDto>> ListDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        return referenceDataRepository.ListDepartmentsAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<LookupItemDto>> ListCollaboratorsAsync(CancellationToken cancellationToken = default)
    {
        return referenceDataRepository.ListCollaboratorsAsync(cancellationToken);
    }
}
