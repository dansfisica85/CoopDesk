using CoopDesk.Application.Dtos;
using CoopDesk.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoopDesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/reference-data")]
public sealed class ReferenceDataController(IReferenceDataService referenceDataService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("departments")]
    public async Task<ActionResult<IReadOnlyCollection<LookupItemDto>>> ListDepartments(CancellationToken cancellationToken)
    {
        var departments = await referenceDataService.ListDepartmentsAsync(cancellationToken);
        return Ok(departments);
    }

    [AllowAnonymous]
    [HttpGet("problem-types")]
    public ActionResult<IReadOnlyCollection<ProblemTypeDto>> ListProblemTypes()
    {
        return Ok(referenceDataService.ListProblemTypes());
    }

    [HttpGet("collaborators")]
    public async Task<ActionResult<IReadOnlyCollection<LookupItemDto>>> ListCollaborators(CancellationToken cancellationToken)
    {
        var collaborators = await referenceDataService.ListCollaboratorsAsync(cancellationToken);
        return Ok(collaborators);
    }
}
