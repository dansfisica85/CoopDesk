using CoopDesk.Application.Dtos;
using CoopDesk.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoopDesk.Api.Controllers;

[ApiController]
[Route("api/reference-data")]
public sealed class ReferenceDataController(IReferenceDataService referenceDataService) : ControllerBase
{
    [HttpGet("departments")]
    public async Task<ActionResult<IReadOnlyCollection<LookupItemDto>>> ListDepartments(CancellationToken cancellationToken)
    {
        var departments = await referenceDataService.ListDepartmentsAsync(cancellationToken);
        return Ok(departments);
    }

    [HttpGet("collaborators")]
    public async Task<ActionResult<IReadOnlyCollection<LookupItemDto>>> ListCollaborators(CancellationToken cancellationToken)
    {
        var collaborators = await referenceDataService.ListCollaboratorsAsync(cancellationToken);
        return Ok(collaborators);
    }
}
