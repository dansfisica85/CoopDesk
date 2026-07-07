using CoopDesk.Application.Dtos;
using CoopDesk.Application.Interfaces;
using CoopDesk.Domain.Entities;
using CoopDesk.Domain.Enums;

namespace CoopDesk.Application.Services;

public sealed class SupportRequestService(ISupportRequestRepository repository) : ISupportRequestService
{
    public async Task<SupportRequestResponseDto> CreateAsync(CreateSupportRequest request, CancellationToken cancellationToken = default)
    {
        var requesterName = RequireText(request.RequesterName, nameof(request.RequesterName), 160);
        var requesterEmail = RequireEmail(request.RequesterEmail);
        var description = RequireText(request.Description, nameof(request.Description), 4000);

        if (!await repository.DepartmentExistsAsync(request.DepartmentId, cancellationToken))
        {
            throw new ArgumentException("Departamento informado nao existe.", nameof(request.DepartmentId));
        }

        var requester = await repository.GetRequesterByEmailAsync(requesterEmail, cancellationToken);
        if (requester is null)
        {
            requester = new Collaborator(requesterName, requesterEmail, request.DepartmentId);
            await repository.AddRequesterAsync(requester, cancellationToken);
        }
        else
        {
            requester.UpdateProfile(requesterName, requesterEmail, request.DepartmentId);
        }

        var problemName = ProblemTypeLabel(request.ProblemType);
        var title = $"{problemName} - {requesterName}";
        var fullDescription = $"""
            Solicitante: {requesterName}
            E-mail: {requesterEmail}
            Tipo de problema: {problemName}

            {description}
            """;

        var ticket = new Ticket(
            title,
            fullDescription,
            PriorityFor(request.ProblemType),
            requester.Id,
            request.DepartmentId,
            requesterEmail,
            request.ProblemType);

        await repository.AddTicketAsync(ticket, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new SupportRequestResponseDto(
            ticket.Id,
            CreateProtocol(ticket.Id),
            ticket.Status,
            ticket.CreatedAtUtc);
    }

    private static TicketPriority PriorityFor(SupportProblemType problemType)
    {
        return problemType switch
        {
            SupportProblemType.Access => TicketPriority.High,
            SupportProblemType.SystemError => TicketPriority.High,
            SupportProblemType.SlowPerformance => TicketPriority.Medium,
            SupportProblemType.RegistrationUpdate => TicketPriority.Medium,
            SupportProblemType.ReportIssue => TicketPriority.Medium,
            _ => TicketPriority.Low
        };
    }

    private static string ProblemTypeLabel(SupportProblemType problemType)
    {
        return problemType switch
        {
            SupportProblemType.Access => "Acesso ao sistema",
            SupportProblemType.SystemError => "Erro no sistema",
            SupportProblemType.SlowPerformance => "Lentidao",
            SupportProblemType.RegistrationUpdate => "Atualizacao cadastral",
            SupportProblemType.ReportIssue => "Relatorio ou consulta",
            _ => "Outro problema"
        };
    }

    private static string CreateProtocol(Guid ticketId)
    {
        return $"CD-{ticketId.ToString("N")[..8].ToUpperInvariant()}";
    }

    private static string RequireText(string value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Valor obrigatorio.", parameterName);
        }

        var trimmed = value.Trim();
        return trimmed.Length > maxLength ? trimmed[..maxLength] : trimmed;
    }

    private static string RequireEmail(string value)
    {
        var email = RequireText(value, nameof(value), 180).ToLowerInvariant();
        return email.Contains('@', StringComparison.Ordinal) ? email : throw new ArgumentException("E-mail invalido.", nameof(value));
    }
}
