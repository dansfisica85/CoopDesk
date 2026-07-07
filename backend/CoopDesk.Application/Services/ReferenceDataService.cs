using CoopDesk.Application.Dtos;
using CoopDesk.Application.Interfaces;
using CoopDesk.Domain.Enums;

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

    public IReadOnlyCollection<ProblemTypeDto> ListProblemTypes()
    {
        return Enum
            .GetValues<SupportProblemType>()
            .Select(problemType => new ProblemTypeDto(problemType.ToString(), Label(problemType)))
            .ToList();
    }

    private static string Label(SupportProblemType problemType)
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
}
