# Arquitetura

O CoopDesk foi organizado para demonstrar uma migracao incremental de legado desktop para web.

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
- `UserRole`

Regras como abertura, atribuicao, mudanca de status, perfis de usuario e bloqueio de edicao em chamados fechados ficam aqui.

### Application

Contem DTOs, interfaces e services. Esta camada nao conhece banco de dados, HTTP ou Windows Forms.

Principais classes:

- `TicketService`
- `AuthService`
- `ReferenceDataService`
- `ITicketRepository`
- `IReferenceDataRepository`
- `IUserRepository`

### Infrastructure

Implementa persistencia com Entity Framework Core e SQL Server.

Principais classes:

- `CoopDeskDbContext`
- `TicketRepository`
- `ReferenceDataRepository`
- `UserRepository`
- `Pbkdf2PasswordHashService`

### Api

Exposicao REST em ASP.NET Core:

- `TicketsController`
- `AuthController`
- `ReferenceDataController`
- `HealthController`
- `JwtTokenService`
- `ExceptionHandlingMiddleware`

### Desktop

Cliente Windows Forms que representa a aplicacao legado. Ele autentica na API, envia token JWT nas chamadas HTTP e simula uma migracao gradual em vez de um corte total.

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
