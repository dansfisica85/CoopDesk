# Arquitetura

O CoopDesk foi organizado para demonstrar um produto de suporte com cliente Windows, API segura e central web.

## Camadas

### Domain

Contem entidades e regras centrais:

- `Ticket`
- `TicketHistory`
- `Collaborator`
- `Department`
- `AppUser`
- `TicketPriority`
- `TicketStatus`
- `SupportProblemType`
- `UserRole`

Regras como abertura, atribuicao, mudanca de status, perfis de usuario e bloqueio de edicao em chamados fechados ficam aqui.

### Application

Contem DTOs, interfaces e services. Esta camada nao conhece banco de dados, HTTP ou Windows Forms.

Principais classes:

- `TicketService`
- `AuthService`
- `SupportRequestService`
- `ReferenceDataService`
- `ITicketRepository`
- `IReferenceDataRepository`
- `IUserRepository`
- `ISupportRequestRepository`

### Infrastructure

Implementa persistencia com Entity Framework Core e SQL Server.

Principais classes:

- `CoopDeskDbContext`
- `TicketRepository`
- `ReferenceDataRepository`
- `UserRepository`
- `SupportRequestRepository`
- `Pbkdf2PasswordHashService`

### Api

Exposicao REST em ASP.NET Core:

- `TicketsController`
- `AuthController`
- `SupportRequestsController`
- `ReferenceDataController`
- `HealthController`
- `JwtTokenService`
- `ExceptionHandlingMiddleware`

### Desktop

Cliente Windows Forms que representa o aplicativo instalado no computador do usuario final. Ele carrega setores e tipos de problema pela API, envia solicitacoes publicas e recebe um protocolo, sem acessar diretamente o banco de dados.

### Frontend

Aplicacao Angular para operacao moderna do sistema:

- Dashboard de chamados
- Login com usuarios demo
- Filtros
- Abertura de chamado
- Alteracao de status
- Controle visual por perfil

## Banco de dados

Schema: `support`

Tabelas:

- `Departments`
- `Collaborators`
- `Users`
- `Tickets`
- `TicketHistories`

Enums sao persistidos como texto para facilitar leitura, suporte e troubleshooting em SQL Server.
