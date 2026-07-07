# Deploy seguro

Este projeto tem tres partes:

- `CoopDesk.Legacy.WinForms`: aplicativo Windows que o usuario instala para abrir solicitacoes.
- `CoopDesk.Api`: API ASP.NET Core que recebe solicitacoes, autentica a central e grava no banco.
- `coopdesk-web`: central Angular para atendimento/admin.

## Regra de seguranca principal

Token de banco de dados nunca deve ir para:

- aplicativo Windows distribuido para usuarios;
- Angular no Vercel;
- README;
- commit no GitHub.

O app Windows e o Angular conversam apenas com a API. A API e a unica parte que pode guardar segredo de banco.

## Vercel

Use Vercel apenas para o Angular.

Projeto:

```text
frontend/coopdesk-web
```

Build command:

```text
pnpm build
```

Output directory:

```text
dist/coopdesk-web
```

Variavel no Vercel:

```text
COOPDESK_API_BASE_URL=https://sua-api-publica.example.com
```

Essa variavel e publica. Ela apenas informa ao Angular onde esta a API.

## Hospedagem da API

A API precisa ir para uma plataforma que rode ASP.NET Core, por exemplo:

- Azure App Service;
- Render;
- Railway;
- Fly.io;
- VPS Windows/Linux com .NET Runtime.

Variaveis da API:

```text
ConnectionStrings__CoopDesk=<connection-string-do-banco>
Jwt__Issuer=CoopDesk.Api
Jwt__Audience=CoopDesk.Clients
Jwt__SigningKey=<segredo-longo-e-aleatorio>
Jwt__ExpirationMinutes=120
Cors__AllowedOrigins__0=https://seu-front.vercel.app
```

## Banco

O projeto principal usa SQL Server/LocalDB porque a vaga pede SQL Server.

Para demo online, as opcoes mais coerentes sao:

- Azure SQL, mantendo o mesmo provider EF Core SQL Server.
- SQL Server em uma VPS ou servico gerenciado.
- Adaptador futuro para Turso/libSQL, mantendo a API como intermediaria.

## Sobre Turso

Turso pode ser usado como banco online, mas nao deve ser acessado diretamente pelo Angular nem pelo app Windows.

Se Turso for usado, as variaveis devem ficar somente na hospedagem da API:

```text
TURSO_DATABASE_URL=libsql://...
TURSO_AUTH_TOKEN=...
```

Esta versao ainda mantem SQL Server como provider principal por aderencia a vaga. O Turso deve entrar como adaptador de infraestrutura separado, sem expor token ao cliente.
