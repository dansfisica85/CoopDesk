# CoopDesk

CoopDesk e um sistema de atendimento interno construido como projeto de portfolio para demonstrar competencias de Desenvolvedor FullStack com C# .NET, Windows Forms, ASP.NET Core, Angular, SQL Server, Git, boas praticas de arquitetura e testes.

O projeto simula um cenario corporativo real: uma empresa possui uma aplicacao desktop legada em Windows Forms usada por colaboradores internos, mas precisa evoluir para uma arquitetura moderna baseada em API REST, banco relacional e front-end web em Angular, sem interromper a operacao atual.

## Proposito da aplicacao

O CoopDesk tem como proposito centralizar, registrar, acompanhar e resolver chamados internos de suporte tecnico e operacional.

Em um ambiente corporativo, colaboradores de areas como Credito, Operacoes, Atendimento e Tecnologia precisam reportar problemas em sistemas internos, solicitar ajustes, acompanhar prazos e registrar historico de atendimento. O CoopDesk organiza esse fluxo.

O projeto foi pensado para contar uma historia tecnica clara em entrevista:

1. Existe um sistema desktop legado em Windows Forms.
2. Esse sistema ainda e usado pela operacao.
3. Uma nova API ASP.NET Core passa a concentrar as regras de negocio e acesso ao banco.
4. O WinForms deixa de acessar o banco diretamente e passa a consumir a API.
5. Um front-end Angular oferece uma experiencia moderna para o mesmo dominio.
6. A migracao acontece de forma gradual, reduzindo risco para o negocio.

## O que a aplicacao sera capaz de fazer

### Funcionalidades atuais

- Listar chamados internos.
- Abrir novo chamado.
- Informar titulo, descricao, solicitante, area e prioridade.
- Classificar chamados por prioridade.
- Controlar status do chamado.
- Alterar status para atendimento, resolvido ou fechado.
- Registrar historico de mudancas de status.
- Listar departamentos ativos.
- Listar colaboradores ativos.
- Persistir dados em SQL Server LocalDB.
- Expor operacoes por API REST.
- Consumir a mesma API por Windows Forms.
- Consumir a mesma API por Angular.
- Validar regras de negocio com testes unitarios.

### Funcionalidades planejadas

- Tela detalhada do chamado com linha do tempo.
- Comentarios internos no chamado.
- Anexos.
- SLA por prioridade.
- Autenticacao e autorizacao por perfil.
- Perfis como solicitante, atendente, gestor e administrador.
- Dashboard gerencial com indicadores.
- Relatorios por area, status, prioridade e periodo.
- Auditoria completa de alteracoes.
- Testes de integracao da API.
- Pipeline de CI no GitHub Actions.
- Migrations versionadas do Entity Framework Core.
- Swagger/OpenAPI com pacote seguro.

## Relacao direta com a vaga

A vaga pede experiencia com:

- C#.
- .NET ou .NET Core.
- Windows Forms legado.
- Migracao de desktop para web.
- ASP.NET Core Web API RESTful.
- Angular.
- HTML, CSS e JavaScript/TypeScript.
- SQL Server.
- Git.
- SOLID, DDD e Clean Code.
- Code Review.
- Suporte a usuarios e ferramentas internas.

O CoopDesk foi construido para cobrir esses pontos em uma aplicacao pequena, mas com arquitetura parecida com sistemas corporativos reais.

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
- Middleware de tratamento de excecoes.

### Front-end web

- Angular.
- TypeScript.
- HTML.
- SCSS.
- HttpClient.
- FormsModule.

### Desktop

- Windows Forms.
- C#.
- HttpClient.
- DataGridView.

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
      Program.cs
      appsettings.json

    CoopDesk.Application/
      Dtos/
      Interfaces/
      Services/

    CoopDesk.Domain/
      Common/
      Entities/
      Enums/

    CoopDesk.Infrastructure/
      Persistence/
      Repositories/
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

  tests/
    CoopDesk.Tests/
      TicketServiceTests.cs

  docs/
    architecture.md
    interview-script.md
    visual-studio.md

  CoopDesk.sln
  CoopDesk.slnx
  README.md
```

## Arquitetura

O projeto foi separado em camadas para evitar que a regra de negocio fique presa a detalhes de banco, API, Windows Forms ou Angular.

### CoopDesk.Domain

Contem o nucleo do dominio.

Responsabilidades:

- Representar entidades principais.
- Proteger invariantes de negocio.
- Controlar transicoes de status.
- Registrar historico de chamados.

Principais arquivos:

- `Ticket.cs`
- `TicketHistory.cs`
- `Collaborator.cs`
- `Department.cs`
- `TicketPriority.cs`
- `TicketStatus.cs`

Exemplo de regra:

- Um chamado fechado ou cancelado nao pode ser editado.
- Toda mudanca de status gera historico.
- Todo chamado nasce com status `Open`.

### CoopDesk.Application

Contem os casos de uso da aplicacao.

Responsabilidades:

- Receber DTOs.
- Orquestrar regras do dominio.
- Usar interfaces de repositorio.
- Retornar DTOs para API ou outros clientes.

Principais arquivos:

- `TicketService.cs`
- `ReferenceDataService.cs`
- `ITicketService.cs`
- `ITicketRepository.cs`
- `CreateTicketRequest.cs`
- `TicketSummaryDto.cs`
- `TicketDetailDto.cs`

Essa camada nao conhece Entity Framework, SQL Server, HTTP, Angular ou Windows Forms.

### CoopDesk.Infrastructure

Contem detalhes tecnicos de persistencia.

Responsabilidades:

- Configurar o `DbContext`.
- Mapear entidades para tabelas.
- Configurar relacionamentos.
- Implementar repositories.
- Criar dados iniciais para teste.

Principais arquivos:

- `CoopDeskDbContext.cs`
- `CoopDeskDbContextFactory.cs`
- `TicketRepository.cs`
- `ReferenceDataRepository.cs`

### CoopDesk.Api

Exposicao HTTP do sistema.

Responsabilidades:

- Receber requisicoes REST.
- Validar entrada basica.
- Chamar services da camada Application.
- Retornar respostas HTTP.
- Centralizar tratamento de erros.

Principais controllers:

- `TicketsController`
- `ReferenceDataController`
- `HealthController`

### CoopDesk.Legacy.WinForms

Cliente desktop que simula a aplicacao legada.

Responsabilidades:

- Permitir operacao basica de chamados via Windows Forms.
- Consumir a API por HTTP.
- Demonstrar uma migracao gradual de legado.

Importante: neste projeto, o WinForms nao acessa o banco diretamente. Ele usa a API. Essa escolha mostra uma estrategia de modernizacao segura: manter o desktop, mas mover regra e persistencia para o back-end.

### coopdesk-web

Cliente Angular moderno.

Responsabilidades:

- Exibir dashboard simples de chamados.
- Filtrar chamados.
- Abrir chamado.
- Alterar status.
- Consumir endpoints REST da API.

## Modelo de dados

Schema usado: `support`

Tabelas:

- `Departments`
- `Collaborators`
- `Tickets`
- `TicketHistories`

Relacionamentos:

- Um chamado pertence a um departamento.
- Um chamado possui um solicitante.
- Um chamado pode possuir um responsavel.
- Um chamado possui varios registros de historico.

Enums persistidos como texto:

- `TicketPriority`
- `TicketStatus`

Essa escolha facilita leitura direta no SQL Server durante suporte e troubleshooting.

## Fluxo de trabalho da aplicacao

### Fluxo 1: abertura de chamado

1. Usuario acessa o Angular ou o WinForms.
2. Usuario informa titulo, descricao, prioridade, solicitante e area.
3. Cliente envia `POST /api/tickets`.
4. API recebe `CreateTicketRequest`.
5. `TicketService` cria a entidade `Ticket`.
6. `Ticket` nasce com status `Open`.
7. `Ticket` cria um primeiro item de historico.
8. Repository persiste o chamado no SQL Server.
9. API retorna `201 Created`.
10. Cliente atualiza a listagem.

### Fluxo 2: listagem de chamados

1. Cliente chama `GET /api/tickets`.
2. API recebe filtros opcionais.
3. `TicketRepository` monta a consulta.
4. EF Core busca dados no SQL Server.
5. Application converte entidade para `TicketSummaryDto`.
6. API retorna lista de chamados.
7. Cliente exibe os dados em tabela.

### Fluxo 3: alteracao de status

1. Usuario seleciona um chamado.
2. Usuario escolhe atender, resolver ou fechar.
3. Cliente envia `PATCH /api/tickets/{id}/status`.
4. API recebe `ChangeTicketStatusRequest`.
5. `TicketService` busca o chamado.
6. Entidade `Ticket` aplica a mudanca de status.
7. Entidade cria novo `TicketHistory`.
8. Repository salva a alteracao.
9. Cliente recarrega a lista.

### Fluxo 4: suporte ao legado

1. A equipe continua usando o WinForms.
2. O WinForms passa a consumir a API.
3. A regra de negocio deixa de ficar espalhada no desktop.
4. A equipe pode evoluir o Angular em paralelo.
5. A migracao para web acontece sem parar a operacao.

## Endpoints

### Health check

```http
GET /api/health
```

Retorna o status basico da API.

### Chamados

```http
GET /api/tickets
```

Lista chamados.

Filtros opcionais:

- `status`
- `priority`
- `departmentId`
- `search`

```http
GET /api/tickets/{id}
```

Busca um chamado por ID.

```http
POST /api/tickets
```

Cria um chamado.

Exemplo:

```json
{
  "title": "Erro ao aprovar proposta",
  "description": "Usuario relata falha na etapa final do fluxo de credito.",
  "priority": "High",
  "requesterId": "a471f67e-03ab-4d81-a4cd-53b903a59e7f",
  "departmentId": "5d6ef6ef-5f87-47f4-8a8e-e35708e5e101",
  "dueAtUtc": null
}
```

```http
PUT /api/tickets/{id}
```

Atualiza dados principais do chamado.

```http
PATCH /api/tickets/{id}/assignment
```

Atribui ou remove responsavel.

```http
PATCH /api/tickets/{id}/status
```

Altera status do chamado.

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

Remove chamado.

### Dados de referencia

```http
GET /api/reference-data/departments
```

Lista departamentos.

```http
GET /api/reference-data/collaborators
```

Lista colaboradores.

## Como rodar localmente

### Pre-requisitos

- Visual Studio instalado.
- .NET SDK instalado.
- SQL Server LocalDB.
- Node.js.
- pnpm.

### Banco de dados

Por padrao, a API usa:

```text
Server=(localdb)\mssqllocaldb;Database=CoopDeskDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

A primeira execucao da API cria o banco automaticamente com `EnsureCreated`.

### Rodar API

Na raiz do repositorio:

```powershell
dotnet run --project backend/CoopDesk.Api/CoopDesk.Api.csproj --urls http://localhost:5298
```

Teste:

```powershell
Invoke-RestMethod http://localhost:5298/api/health
```

### Rodar Windows Forms

Abra:

```text
CoopDesk.sln
```

No Visual Studio:

1. Clique com o botao direito em `CoopDesk.Legacy.WinForms`.
2. Clique em `Definir como Projeto de Inicializacao`.
3. Garanta que a API esta rodando em `http://localhost:5298`.
4. Pressione `Ctrl+F5`.

### Rodar Angular

```powershell
cd frontend/coopdesk-web
pnpm install
pnpm start
```

URL:

```text
http://localhost:4200
```

O Angular espera a API em:

```text
http://localhost:5298
```

## Como validar

### Build da solution

```powershell
dotnet build CoopDesk.sln
```

### Testes

```powershell
dotnet test CoopDesk.sln
```

### Build Angular

```powershell
cd frontend/coopdesk-web
pnpm build
```

### Verificar pacotes vulneraveis .NET

```powershell
dotnet list CoopDesk.sln package --vulnerable --include-transitive
```

## Etapas de construcao do projeto

Esta secao documenta o processo de criacao do projeto para facilitar apresentacao em entrevista e revisao no GitHub.

### Etapa 1: leitura da vaga

Foi identificada a necessidade de demonstrar:

- C#.
- Windows Forms.
- ASP.NET Core.
- Angular.
- SQL Server.
- CRUD.
- REST API.
- Git.
- SOLID.
- DDD.
- Clean Code.
- Code Review.
- Suporte a sistemas internos.

### Etapa 2: definicao do dominio

Foi escolhido o dominio de atendimento interno porque ele conversa diretamente com a vaga, que menciona suporte TopDesk e sistemas internos.

Entidades principais:

- Chamado.
- Historico de chamado.
- Colaborador.
- Departamento.

### Etapa 3: criacao da solution

Foi criada uma solution com projetos separados:

- `CoopDesk.Domain`
- `CoopDesk.Application`
- `CoopDesk.Infrastructure`
- `CoopDesk.Api`
- `CoopDesk.Legacy.WinForms`
- `CoopDesk.Tests`

### Etapa 4: criacao do dominio

Foram criadas entidades com regras internas:

- `Ticket`
- `TicketHistory`
- `Collaborator`
- `Department`

Tambem foram criados enums:

- `TicketPriority`
- `TicketStatus`

### Etapa 5: criacao da camada Application

Foram adicionados:

- DTOs de entrada.
- DTOs de saida.
- Interfaces de repositorio.
- Services de caso de uso.

Essa camada e testavel sem banco e sem HTTP.

### Etapa 6: criacao da infraestrutura

Foi adicionado Entity Framework Core com SQL Server.

Foram configurados:

- `DbContext`.
- Tabelas.
- Relacionamentos.
- Indices.
- Seed inicial.
- Repositories.

### Etapa 7: criacao da API

Foram criados controllers REST:

- `TicketsController`
- `ReferenceDataController`
- `HealthController`

Tambem foi criado um middleware centralizado de erros.

### Etapa 8: criacao do WinForms

Foi criado um cliente desktop com:

- Campo para URL da API.
- Tabela de chamados.
- Botoes de atualizacao e mudanca de status.
- Formulario para abertura de chamado.

Esse projeto representa a parte legada.

### Etapa 9: criacao do Angular

Foi criado um front-end com:

- Cards de resumo.
- Filtros.
- Tabela de chamados.
- Formulario de abertura.
- Acoes de mudanca de status.

### Etapa 10: testes

Foram criados testes unitarios para validar:

- Criacao de chamado.
- Alteracao de status.
- Registro de historico.

### Etapa 11: validacao tecnica

Foram executados:

```powershell
dotnet build CoopDesk.sln
dotnet test CoopDesk.sln
pnpm build
dotnet list CoopDesk.sln package --vulnerable --include-transitive
```

## Decisoes tecnicas importantes

### Por que WinForms consome API?

Porque esse desenho representa uma migracao gradual. Em vez de reescrever tudo de uma vez, o sistema legado passa a usar o mesmo back-end da aplicacao web.

### Por que separar em camadas?

Para evitar acoplamento entre regra de negocio, banco, API e interface grafica.

### Por que usar DTOs?

Para nao expor entidades diretamente pela API e controlar contratos de entrada e saida.

### Por que usar repositories?

Para isolar o Entity Framework Core da camada Application.

### Por que Angular?

Porque a vaga pede front-end web com Angular, HTML, CSS e JavaScript/TypeScript.

### Por que remover OpenAPI nesta versao?

O template instalado trouxe pacote OpenAPI com alerta de vulnerabilidade NU1903 nas fontes atuais do NuGet. Para manter o projeto sem alertas, o pacote foi removido temporariamente. Uma evolucao futura e adicionar Swagger/OpenAPI com uma combinacao de pacotes sem vulnerabilidade conhecida.

## Checklist para entrevista

Pontos que podem ser explicados:

- O projeto simula migracao de legado Windows Forms.
- A API concentra regra e persistencia.
- O Angular representa a nova experiencia web.
- O WinForms continua funcional durante a transicao.
- O banco relacional possui tabelas e relacionamentos coerentes.
- A camada Domain protege regras de negocio.
- A camada Application e testavel.
- A infraestrutura usa EF Core e SQL Server.
- Os endpoints seguem estilo REST.
- O projeto tem testes unitarios.
- O README documenta decisao, execucao e evolucao.

## Como subir para o GitHub

Quando o projeto estiver pronto:

```powershell
git init
git add .
git commit -m "Initial CoopDesk portfolio project"
git branch -M main
git remote add origin https://github.com/SEU_USUARIO/coopdesk.git
git push -u origin main
```

Antes de commitar, confira:

```powershell
git status
dotnet build CoopDesk.sln
dotnet test CoopDesk.sln
cd frontend/coopdesk-web
pnpm build
```

Nao subir:

- `bin/`
- `obj/`
- `.vs/`
- `node_modules/`
- `dist/`
- logs locais.

Esses itens ja estao cobertos no `.gitignore`.

## Roadmap

### Versao 1

- CRUD completo de chamados.
- API REST.
- WinForms consumindo API.
- Angular consumindo API.
- SQL Server LocalDB.
- Testes unitarios.

### Versao 2

- Tela de detalhe do chamado.
- Comentarios e timeline.
- Migrations EF Core.
- Swagger seguro.
- Melhor tratamento de validacoes.

### Versao 3

- Autenticacao.
- Autorizacao por perfil.
- Dashboard gerencial.
- Relatorios.
- GitHub Actions.
- Testes de integracao.

## Status atual

Implementado:

- Solution C#.
- API ASP.NET Core.
- Camadas Domain, Application e Infrastructure.
- SQL Server LocalDB.
- Windows Forms.
- Angular.
- Testes unitarios.
- Documentacao inicial.

Validado:

- Build C# sem erros.
- Testes passando.
- Build Angular passando.
- API respondendo em `http://localhost:5298`.
- WinForms carregando chamados pela API.

## Licenca

Projeto criado para fins educacionais e de portfolio.
