# CoopDesk

CoopDesk e um sistema de atendimento interno construido como projeto de portfolio para demonstrar competencias de Desenvolvedor FullStack com C# .NET, Windows Forms, ASP.NET Core, Angular, SQL Server, Git e boas praticas de arquitetura e testes.

O projeto simula um cenario corporativo real: usuarios finais instalam um aplicativo Windows para abrir solicitacoes de suporte, enquanto a equipe de atendimento usa uma central web em Angular para acompanhar, filtrar e tratar os chamados. A API ASP.NET Core fica no meio do fluxo e protege o banco de dados.

Este README documenta somente o que esta implementado e funcionando hoje no repositorio.

## Proposito da aplicacao

O CoopDesk centraliza, registra, acompanha e resolve solicitacoes de suporte tecnico e operacional.

Em um ambiente corporativo, usuarios de areas como Credito, Operacoes, Atendimento e Tecnologia precisam reportar problemas em sistemas internos, solicitar ajustes, acompanhar prazos e registrar historico de atendimento. O CoopDesk organiza esse fluxo com tres pontas que conversam apenas por API:

1. O usuario final abre o aplicativo Windows (`CoopDesk.Legacy.WinForms`).
2. O aplicativo coleta nome, e-mail, setor, tipo de problema e descricao.
3. O app envia a solicitacao para a API ASP.NET Core (`CoopDesk.Api`), sem conhecer o banco.
4. A API grava o chamado no banco relacional (SQL Server).
5. A equipe de suporte acessa a central Angular (`coopdesk-web`) com login.
6. A central lista, filtra e atualiza os chamados por status.
7. O banco e os tokens sensiveis ficam protegidos apenas no servidor da API.

## Funcionalidades atuais

- Autenticacao com e-mail e senha, emitindo token JWT.
- Autorizacao por perfil: `Administrator`, `Agent` e `Requester`.
- Abertura de chamado interno pela central Angular autenticada.
- Abertura de solicitacao publica pelo aplicativo Windows, sem necessidade de login.
- Geracao de protocolo (`CD-XXXXXXXX`) para solicitacoes enviadas pelo app Windows.
- Classificacao de chamados por tipo de problema e por prioridade.
- Controle de status do chamado, com transicao para atendimento, resolvido ou fechado.
- Registro de historico a cada mudanca de status.
- Listagem de chamados com filtros por status, prioridade, tipo de problema, departamento e busca textual.
- Listagem de departamentos e colaboradores ativos.
- Persistencia em SQL Server LocalDB via Entity Framework Core.
- Exposicao de todas as operacoes por API REST.
- Consumo da mesma API por dois clientes distintos: Windows Forms e Angular.
- Configuracao do Angular por variavel de ambiente (`COOPDESK_API_BASE_URL`), pronta para deploy no Vercel.
- CORS configuravel por ambiente na API.
- Testes unitarios cobrindo autenticacao, chamados e solicitacoes publicas.

## Tecnologias usadas

### Back-end

- C#.
- .NET 10.
- ASP.NET Core Web API.
- Entity Framework Core.
- SQL Server LocalDB.
- Injecao de dependencia.
- Controllers REST.
- DTOs.
- Autenticacao JWT Bearer.
- Hash de senha com PBKDF2.
- Middleware de tratamento de excecoes.

### Front-end web

- Angular.
- TypeScript (compilado para JavaScript no navegador).
- HTML.
- SCSS.
- HttpClient.
- FormsModule.
- Login com armazenamento local de token.
- Configuracao publica `config.json` gerada no build para apontar para a API.

### Desktop

- Windows Forms.
- C#.
- HttpClient.
- DataGridView.
- Envio publico de solicitacoes sem acesso direto ao banco.

### Testes

- xUnit.
- Testes unitarios da camada de Application.

### Ferramentas

- Visual Studio.
- .NET CLI.
- pnpm.
- Git.

## Estrutura do repositorio

```text
CoopDesk/
  backend/
	CoopDesk.Api/
	  Controllers/
	  Middleware/
	  Security/
	  Program.cs
	  appsettings.json

	CoopDesk.Application/
	  Dtos/
	  Interfaces/
	  Security/
	  Services/

	CoopDesk.Domain/
	  Common/
	  Entities/
	  Enums/

	CoopDesk.Infrastructure/
	  Persistence/
	  Repositories/
	  Security/
	  DependencyInjection.cs

  desktop/
	CoopDesk.Legacy.WinForms/
	  Form1.cs
	  Form1.Designer.cs
	  Program.cs

  frontend/
	coopdesk-web/
	  src/
		app/
		  app.ts
		  app.html
		  app.scss
		  app.config.ts
		  app.routes.ts

  tests/
	CoopDesk.Tests/
	  AuthServiceTests.cs
	  SupportRequestServiceTests.cs
	  TicketServiceTests.cs

  docs/
	architecture.md
	deployment.md
	visual-studio.md

  CoopDesk.sln
  CoopDesk.slnx
  README.md
```

## Arquitetura em camadas

O projeto foi separado em camadas para evitar que a regra de negocio fique presa a detalhes de banco, API, Windows Forms ou Angular.

### CoopDesk.Domain

Nucleo do dominio. Representa entidades principais, protege invariantes de negocio e controla transicoes de status.

Principais arquivos: `Ticket.cs`, `TicketHistory.cs`, `Collaborator.cs`, `Department.cs`, `AppUser.cs`, `TicketPriority.cs`, `TicketStatus.cs`, `SupportProblemType.cs`, `UserRole.cs`.

Exemplos de regra:

- Um chamado fechado ou cancelado nao pode ser editado.
- Toda mudanca de status gera historico.
- Todo chamado nasce com status `Open`.

### CoopDesk.Application

Casos de uso da aplicacao. Recebe DTOs, orquestra regras do dominio, usa interfaces de repositorio e devolve DTOs para a API ou outros clientes. Esta camada nao conhece Entity Framework, SQL Server, HTTP, Angular ou Windows Forms.

Principais arquivos: `TicketService.cs`, `AuthService.cs`, `SupportRequestService.cs`, `ReferenceDataService.cs` e as respectivas interfaces e DTOs em `Dtos/`.

### CoopDesk.Infrastructure

Detalhes tecnicos de persistencia: configura o `DbContext`, mapeia entidades para tabelas, implementa repositories e o hash de senha.

Principais arquivos: `CoopDeskDbContext.cs`, `CoopDeskDbContextFactory.cs`, `TicketRepository.cs`, `ReferenceDataRepository.cs`, `UserRepository.cs`, `SupportRequestRepository.cs`, `Pbkdf2PasswordHashService.cs`.

### CoopDesk.Api

Exposicao HTTP do sistema. Recebe requisicoes REST, chama services da camada Application, emite e valida tokens JWT, aplica autorizacao por perfil e centraliza tratamento de erros.

Principais controllers: `TicketsController`, `AuthController`, `SupportRequestsController`, `ReferenceDataController`, `HealthController`.

### CoopDesk.Legacy.WinForms

Cliente desktop que simula o aplicativo instalado no computador do usuario final. Carrega setores e tipos de problema da API, envia solicitacoes publicas por HTTP e exibe o protocolo retornado.

Importante: o WinForms nao acessa o banco diretamente e nao recebe token de banco. Ele usa a API, assim como o Angular.

### coopdesk-web (Angular)

Cliente Angular moderno usado pela equipe de atendimento. Exibe dashboard de chamados, autentica o usuario, filtra chamados, abre chamados, altera status e consome os endpoints REST da API.

## Como o C# se relaciona com o Angular/JavaScript

Essa e a pergunta central de arquitetura do CoopDesk: todo o back-end (regra de negocio, banco de dados e API) e escrito em C#, e o front-end de atendimento e escrito em Angular, ou seja, TypeScript compilado para JavaScript no navegador. Os dois lados nunca compartilham codigo-fonte nem se referenciam diretamente: eles conversam exclusivamente por HTTP/JSON.

### Onde fica o codigo C#

| Projeto | Linguagem | Responsabilidade |
| --- | --- | --- |
| `CoopDesk.Domain` | C# | Entidades e regras de negocio (`Ticket`, `TicketHistory`, `AppUser`, enums) |
| `CoopDesk.Application` | C# | Casos de uso, DTOs e interfaces de repositorio |
| `CoopDesk.Infrastructure` | C# | Entity Framework Core, repositories e hash de senha |
| `CoopDesk.Api` | C# | Controllers REST, JWT, middleware e CORS. E o unico ponto de contato com o Angular |
| `CoopDesk.Legacy.WinForms` | C# | Cliente desktop que tambem consome a API REST |
| `CoopDesk.Tests` | C# | Testes unitarios da camada Application |

### Onde fica o codigo Angular/JavaScript

| Pasta | Linguagem | Responsabilidade |
| --- | --- | --- |
| `frontend/coopdesk-web` | TypeScript/Angular, compilado para JavaScript | Central web de atendimento: login, listagem, filtros, abertura e mudanca de status |

### A ponte entre os dois mundos: API REST em JSON

- O Angular nunca referencia DLLs, projetos ou pacotes NuGet do C#. Ele so conhece URLs HTTP expostas pela `CoopDesk.Api`.
- O C# nunca referencia arquivos `.ts`. A API apenas devolve texto no formato JSON.
- Cada DTO em C# (pasta `backend/CoopDesk.Application/Dtos`) tem um formato equivalente representado por uma `interface` TypeScript em `frontend/coopdesk-web/src/app/app.ts`. Exemplos de correspondencia:

  | DTO em C# | Interface em TypeScript |
  | --- | --- |
  | `TicketSummaryDto` | `TicketSummary` |
  | `AuthenticatedUserDto` | `AuthenticatedUser` |
  | `AuthResponseDto` | `LoginResponse` |
  | `LookupItemDto` | `LookupItem` |

- Os enums do dominio C# (`TicketPriority`, `TicketStatus`, `SupportProblemType`, `UserRole`) sao serializados como texto pela API atraves de `JsonStringEnumConverter` e reaparecem no Angular como tipos uniao de string, por exemplo `type TicketStatus = 'Open' | 'InProgress' | 'WaitingBusiness' | 'Resolved' | 'Closed' | 'Canceled'`. Isso garante que os dois lados usem os mesmos valores sem compartilhar codigo.
- **Autenticacao**: o C# (`JwtTokenService`, em `CoopDesk.Api/Security`) gera um token JWT no login. O Angular guarda esse token no `localStorage` (metodo `restoreSession` em `app.ts`) e o reenvia no cabecalho `Authorization: Bearer {token}` a cada chamada (metodo `authHeaders`).
- **CORS**: o `Program.cs` da API define quais origens, por exemplo `http://localhost:4200` ou o dominio publicado no Vercel, podem chamar a API a partir do navegador. Sem essa configuracao, o navegador bloquearia as chamadas do Angular por politica de mesma origem.
- **Descoberta do endereco da API em tempo de execucao**: o Angular nao tem a URL da API fixa no codigo compilado. O script Node `scripts/write-config.mjs` le a variavel de ambiente `COOPDESK_API_BASE_URL` no momento do build e gera `public/config.json`. O metodo `loadRuntimeConfig` em `app.ts` busca esse arquivo assim que a pagina carrega. Isso permite publicar o mesmo build do Angular apontando para APIs diferentes, como local, homologacao ou producao, sem recompilar.
- **Dois clientes, uma unica API**: o `CoopDesk.Legacy.WinForms`, tambem escrito em C#, e apenas mais um consumidor HTTP da mesma API, usando `HttpClient` para chamar `api/reference-data/*` e `api/support-requests`. Ele acessa somente os endpoints publicos, sem token administrativo, enquanto o Angular acessa tambem os endpoints protegidos por JWT. Isso demonstra a mesma API C# sendo reaproveitada por um cliente desktop em C# e por um cliente web em Angular/JavaScript.

## Modelo de dados

Schema usado: `support`.

Tabelas: `Departments`, `Collaborators`, `Users`, `Tickets`, `TicketHistories`.

Relacionamentos:

- Um chamado pertence a um departamento.
- Um chamado possui um solicitante e pode possuir um responsavel.
- Um chamado possui varios registros de historico.
- Um usuario pode estar associado a um colaborador.

Enums persistidos como texto: `TicketPriority`, `TicketStatus`, `SupportProblemType`, `UserRole`. Essa escolha facilita leitura direta no SQL Server durante suporte e troubleshooting.

As senhas dos usuarios demo nao sao gravadas em texto puro. Elas ficam persistidas como hash PBKDF2 no formato:

```text
PBKDF2$iteracoes$saltBase64$hashBase64
```

## Fluxos de trabalho principais

### Fluxo 1: solicitacao publica pelo app Windows

1. Usuario abre o app `CoopDesk Client`.
2. App carrega setores em `GET /api/reference-data/departments`.
3. App carrega tipos de problema em `GET /api/reference-data/problem-types`.
4. Usuario informa nome, e-mail, setor, tipo de problema e descricao.
5. App envia `POST /api/support-requests`.
6. API valida os dados, localiza ou cria o solicitante pelo e-mail e cria o chamado com status `Open`.
7. API retorna protocolo no formato `CD-XXXXXXXX`.

Esse fluxo nao exige login do usuario final e nao expoe credenciais de banco.

### Fluxo 2: autenticacao da central

1. Atendente acessa a central Angular e informa e-mail e senha.
2. Cliente envia `POST /api/auth/login`.
3. `AuthService` localiza o usuario por e-mail e `Pbkdf2PasswordHashService` valida a senha contra o hash salvo no SQL Server.
4. `JwtTokenService` emite um token JWT com nome, e-mail e perfil.
5. O Angular salva o token durante a sessao e passa a enviar `Authorization: Bearer {token}` nas chamadas seguintes.

### Fluxo 3: abertura de chamado pela central

1. Usuario autenticado informa titulo, descricao, prioridade, solicitante e area.
2. Cliente envia `POST /api/tickets` com token JWT.
3. `TicketService` cria a entidade `Ticket` com status `Open` e um primeiro item de historico.
4. O repository persiste o chamado no SQL Server e a API retorna `201 Created`.

### Fluxo 4: listagem de chamados

1. Cliente chama `GET /api/tickets` com token JWT e filtros opcionais.
2. `TicketRepository` monta a consulta e o EF Core busca os dados no SQL Server.
3. A camada Application converte a entidade para `TicketSummaryDto` e a API retorna a lista.

### Fluxo 5: alteracao de status

1. Usuario com perfil `Administrator` ou `Agent` seleciona um chamado e escolhe atender, resolver ou fechar.
2. Cliente envia `PATCH /api/tickets/{id}/status` com token JWT.
3. A entidade `Ticket` aplica a mudanca de status e cria um novo `TicketHistory`.
4. O repository salva a alteracao e o cliente recarrega a lista.

### Fluxo 6: separacao entre cliente e central

1. Usuario final usa o WinForms apenas para solicitar suporte.
2. Equipe de atendimento usa o Angular como central.
3. Ambos falam com a mesma API, e a regra de negocio fica concentrada no back-end, que mantem o banco protegido.

## Endpoints da API

Com excecao de `GET /api/health`, `POST /api/auth/login`, `GET /api/reference-data/departments`, `GET /api/reference-data/problem-types` e `POST /api/support-requests`, os endpoints exigem:

```http
Authorization: Bearer {accessToken}
```

### Health check

```http
GET /api/health
```

### Autenticacao

```http
POST /api/auth/login
```

Exemplo de corpo:

```json
{
  "email": "atendente@coopdesk.local",
  "password": "Demo@12345"
}
```

Resposta resumida:

```json
{
  "accessToken": "jwt...",
  "expiresAtUtc": "2026-07-08T00:40:44Z",
  "user": {
	"fullName": "Bruno Lima",
	"email": "atendente@coopdesk.local",
	"role": "Agent"
  }
}
```

```http
GET /api/auth/me
```

Retorna o usuario autenticado a partir do token.

### Solicitacao publica de suporte

```http
POST /api/support-requests
```

Endpoint usado pelo app Windows distribuido para usuarios finais. Nao exige token JWT e nao recebe segredo de banco.

Exemplo:

```json
{
  "requesterName": "Maria Cliente",
  "requesterEmail": "maria@example.com",
  "departmentId": "5d6ef6ef-5f87-47f4-8a8e-e35708e5e101",
  "problemType": "SystemError",
  "description": "Nao consigo finalizar uma operacao no sistema."
}
```

Resposta:

```json
{
  "ticketId": "c46fc2b8-...",
  "protocol": "CD-C46FC2B8",
  "status": "Open",
  "createdAtUtc": "2026-07-07T23:14:00Z"
}
```

### Chamados

```http
GET /api/tickets
```

Lista chamados. Filtros opcionais: `status`, `priority`, `problemType`, `departmentId`, `search`.

```http
GET /api/tickets/{id}
```

Busca um chamado por ID.

```http
POST /api/tickets
```

Cria um chamado.

```http
PUT /api/tickets/{id}
```

Atualiza dados principais do chamado. Requer perfil `Administrator` ou `Agent`.

```http
PATCH /api/tickets/{id}/assignment
```

Atribui ou remove responsavel. Requer perfil `Administrator` ou `Agent`.

```http
PATCH /api/tickets/{id}/status
```

Altera status do chamado. Requer perfil `Administrator` ou `Agent`.

Exemplo:

```json
{
  "status": "InProgress",
  "notes": "Atendimento iniciado.",
  "performedBy": "angular-web"
}
```

```http
DELETE /api/tickets/{id}
```

Remove chamado. Requer perfil `Administrator` ou `Agent`.

### Dados de referencia

```http
GET /api/reference-data/departments
```

Lista departamentos. Publico, usado pelo app Windows para carregar setores.

```http
GET /api/reference-data/problem-types
```

Lista tipos de problema. Publico, usado pelo app Windows.

```http
GET /api/reference-data/collaborators
```

Lista colaboradores. Requer token JWT porque e usado pela central Angular.

## Como rodar localmente

### Pre-requisitos

- Visual Studio instalado.
- .NET SDK 10 instalado.
- SQL Server LocalDB.
- Node.js.
- pnpm.

### Banco de dados

Nao e necessario instalar um SQL Server completo para testar localmente. O projeto usa SQL Server LocalDB por padrao, que normalmente e instalado junto com o Visual Studio.

Connection string padrao:

```text
Server=(localdb)\mssqllocaldb;Database=CoopDeskDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

A primeira execucao da API cria o banco automaticamente. Para usar SQL Server Express ou uma instancia remota, altere `ConnectionStrings:CoopDesk` em `backend/CoopDesk.Api/appsettings.json`.

### Usuarios demo

Todos os usuarios abaixo usam a senha `Demo@12345`.

| Perfil | E-mail | Permissoes |
| --- | --- | --- |
| Administrador | `admin@coopdesk.local` | Consulta, criacao, edicao, status, atribuicao e exclusao |
| Atendente | `atendente@coopdesk.local` | Consulta, criacao, edicao, status e atribuicao |
| Solicitante | `solicitante@coopdesk.local` | Consulta e abertura de chamados |

### Rodar a API

Na raiz do repositorio:

```powershell
dotnet run --project backend/CoopDesk.Api/CoopDesk.Api.csproj --launch-profile http
```

Teste rapido:

```powershell
Invoke-RestMethod http://localhost:5298/api/health
```

### Rodar o Windows Forms

Abra `CoopDesk.sln` no Visual Studio, defina `CoopDesk.Legacy.WinForms` como projeto de inicializacao, garanta que a API esta rodando em `http://localhost:5298` e pressione `Ctrl+F5`. Tambem e possivel iniciar API e WinForms juntos usando o perfil de multi-startup salvo em `CoopDesk.slnLaunch`.

### Rodar o Angular

```powershell
cd frontend/coopdesk-web
pnpm install
pnpm start
```

Acesse `http://localhost:4200`. O Angular espera a API em `http://localhost:5298` por padrao. Na tela de login, use um dos botoes de perfil demo ou informe manualmente um dos usuarios listados acima.

## Deploy

O Vercel hospeda apenas o Angular, usando a variavel `COOPDESK_API_BASE_URL`. O token de banco nunca deve ir para o Angular ou para o Vercel; ele fica somente na hospedagem da API. Detalhes completos em [`docs/deployment.md`](docs/deployment.md).

## Como validar

### Build da solution

```powershell
dotnet build CoopDesk.sln
```

### Testes

```powershell
dotnet test CoopDesk.sln
```

### Build do Angular

```powershell
cd frontend/coopdesk-web
pnpm build
```

### Verificar pacotes vulneraveis no .NET

```powershell
dotnet list CoopDesk.sln package --vulnerable --include-transitive
```

## Decisoes tecnicas importantes

- **Por que o WinForms consome a API em vez de acessar o banco?** Porque o aplicativo Windows e distribuido para usuarios finais e nao pode expor credenciais de banco. Ele fala com a API, e a API decide como validar, persistir e retornar o protocolo.
- **Por que separar em camadas?** Para evitar acoplamento entre regra de negocio, banco, API, WinForms e Angular.
- **Por que usar DTOs?** Para nao expor entidades diretamente pela API e controlar contratos de entrada e saida com o Angular e o WinForms.
- **Por que usar repositories?** Para isolar o Entity Framework Core da camada Application.
- **Por que JWT?** Porque a central Angular precisa identificar atendentes e aplicar permissoes sem manter sessao de servidor. O app Windows nao recebe JWT administrativo; ele usa apenas o endpoint publico de abertura de solicitacao.
- **Por que PBKDF2?** Porque senha nao deve ser persistida em texto puro. PBKDF2 com salt e uma solucao nativa do .NET e suficiente para demonstrar cuidado com seguranca em um projeto de portfolio.

## Documentacao adicional

- [`docs/architecture.md`](docs/architecture.md): detalhamento das camadas e do banco de dados.
- [`docs/deployment.md`](docs/deployment.md): guia de deploy seguro do Angular e da API.
- [`docs/visual-studio.md`](docs/visual-studio.md): passo a passo para abrir e executar o projeto no Visual Studio.

## Como subir alteracoes para o GitHub

Repositorio publicado em `https://github.com/dansfisica85/CoopDesk`.

Antes de commitar, confira:

```powershell
git status
dotnet build CoopDesk.sln
dotnet test CoopDesk.sln
cd frontend/coopdesk-web
pnpm build
```

Fluxo de publicacao:

```powershell
git add .
git commit -m "Describe the change"
git push
```

Nao devem ser versionados: `bin/`, `obj/`, `.vs/`, `node_modules/`, `dist/` e logs locais. Esses itens ja estao cobertos pelo `.gitignore`.

## Licenca

Projeto criado para fins educacionais e de portfolio.
