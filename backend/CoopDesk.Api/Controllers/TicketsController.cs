using CoopDesk.Application.Dtos;
using CoopDesk.Application.Services;
using CoopDesk.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CoopDesk.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class TicketsController(ITicketService ticketService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TicketSummaryDto>>> Search(
        [FromQuery] TicketStatus? status,
        [FromQuery] TicketPriority? priority,
        [FromQuery] Guid? departmentId,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var tickets = await ticketService.SearchAsync(new TicketQuery(status, priority, departmentId, search), cancellationToken);
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
        var performedBy = Request.Headers.TryGetValue("X-User", out var user) ? user.ToString() : "api";
        var ticket = await ticketService.CreateAsync(request, performedBy, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketDetailDto>> Update(Guid id, UpdateTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await ticketService.UpdateAsync(id, request, cancellationToken);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpPatch("{id:guid}/assignment")]
    public async Task<ActionResult<TicketDetailDto>> Assign(Guid id, AssignTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await ticketService.AssignAsync(id, request, cancellationToken);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<TicketDetailDto>> ChangeStatus(Guid id, ChangeTicketStatusRequest request, CancellationToken cancellationToken)
    {
        var ticket = await ticketService.ChangeStatusAsync(id, request, cancellationToken);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await ticketService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
