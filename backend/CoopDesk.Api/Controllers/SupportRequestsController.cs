using CoopDesk.Application.Dtos;
using CoopDesk.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoopDesk.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/support-requests")]
public sealed class SupportRequestsController(ISupportRequestService supportRequestService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SupportRequestResponseDto>> Create(CreateSupportRequest request, CancellationToken cancellationToken)
    {
        var response = await supportRequestService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(TicketsController.GetById), "Tickets", new { id = response.TicketId }, response);
    }
}
