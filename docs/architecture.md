# Arquitetura

O CoopDesk foi organizado para demonstrar uma migracao incremental de legado desktop para web.

## Camadas

### Domain

Contem entidades e regras centrais:

- `Ticket`
- `TicketHistory`
- `Collaborator`
- `Department`
- `TicketPriority`
- `TicketStatus`

Regras como abertura, atribuicao, mudanca de status e bloqueio de edicao em chamados fechados ficam aqui.

### Application

Contem DTOs, interfaces e services. Esta camada nao conhece banco de dados, HTTP ou Windows Forms.

Principais classes:

- `TicketService`
- `ReferenceDataService`
- `ITicketRepository`
- `IReferenceDataRepository`

### Infrastructure

Implementa persistencia com Entity Framework Core e SQL Server.

Principais classes:

- `CoopDeskDbContext`
- `TicketRepository`
- `ReferenceDataRepository`

### Api

Exposicao REST em ASP.NET Core:

- `TicketsController`
- `ReferenceDataController`
- `HealthController`
- `ExceptionHandlingMiddleware`

### Desktop

Cliente Windows Forms que representa a aplicacao legado. Ele consome a API por HTTP, simulando uma migracao gradual em vez de um corte total.

### Frontend

Aplicacao Angular para operacao moderna do sistema:

- Dashboard de chamados
- Filtros
- Abertura de chamado
- Alteracao de status

## Banco de dados

Schema: `support`

Tabelas:

- `Departments`
- `Collaborators`
- `Tickets`
- `TicketHistories`

Enums sao persistidos como texto para facilitar leitura, suporte e troubleshooting em SQL Server.
