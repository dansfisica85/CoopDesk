using System.Security.Claims;
using CoopDesk.Application.Dtos;
using CoopDesk.Application.Services;
using CoopDesk.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoopDesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tickets")]
public sealed class TicketsController(ITicketService ticketService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TicketSummaryDto>>> Search(
        [FromQuery] TicketStatus? status,
        [FromQuery] TicketPriority? priority,
        [FromQuery] SupportProblemType? problemType,
        [FromQuery] Guid? departmentId,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var tickets = await ticketService.SearchAsync(new TicketQuery(status, priority, problemType, departmentId, search), cancellationToken);
        return Ok(tickets);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var ticket = await ticketService.GetByIdAsync(id, cancellationToken);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDetailDto>> Create(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await ticketService.CreateAsync(request, CurrentUserName(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [Authorize(Policy = "SupportTeam")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketDetailDto>> Update(Guid id, UpdateTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await ticketService.UpdateAsync(id, request, cancellationToken);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [Authorize(Policy = "SupportTeam")]
    [HttpPatch("{id:guid}/assignment")]
    public async Task<ActionResult<TicketDetailDto>> Assign(Guid id, AssignTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await ticketService.AssignAsync(id, request with { PerformedBy = CurrentUserName() }, cancellationToken);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [Authorize(Policy = "SupportTeam")]
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<TicketDetailDto>> ChangeStatus(Guid id, ChangeTicketStatusRequest request, CancellationToken cancellationToken)
    {
        var ticket = await ticketService.ChangeStatusAsync(id, request with { PerformedBy = CurrentUserName() }, cancellationToken);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [Authorize(Policy = "SupportTeam")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await ticketService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private string CurrentUserName()
    {
        return User.Identity?.Name
            ?? User.FindFirstValue(ClaimTypes.Email)
            ?? "api";
    }
}
