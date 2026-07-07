using CoopDesk.Application.Dtos;
using CoopDesk.Application.Interfaces;
using CoopDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoopDesk.Infrastructure.Repositories;

public sealed class ReferenceDataRepository(CoopDeskDbContext dbContext) : IReferenceDataRepository
{
    public async Task<IReadOnlyCollection<LookupItemDto>> ListDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Departments
            .AsNoTracking()
            .Where(department => department.IsActive)
            .OrderBy(department => department.Name)
            .Select(department => new LookupItemDto(department.Id, department.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LookupItemDto>> ListCollaboratorsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Collaborators
            .AsNoTracking()
            .Where(collaborator => collaborator.IsActive)
            .OrderBy(collaborator => collaborator.FullName)
            .Select(collaborator => new LookupItemDto(collaborator.Id, collaborator.FullName))
            .ToListAsync(cancellationToken);
    }
}
