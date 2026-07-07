# Abrindo no Visual Studio

## Arquivo recomendado

Abra:

```text
CoopDesk.sln
```

Esse arquivo foi criado no formato classico de solution para evitar incompatibilidade com instalacoes do Visual Studio que ainda nao lidam bem com `.slnx`.

## Projetos principais

- `CoopDesk.Api`: API ASP.NET Core.
- `CoopDesk.Legacy.WinForms`: aplicacao desktop legado.
- `CoopDesk.Tests`: testes unitarios.

## Como executar API + WinForms

1. Clique com o botao direito na solution.
2. Va em `Configure Startup Projects`.
3. Selecione `Multiple startup projects`.
4. Marque `Start` para:
   - `CoopDesk.Api`
   - `CoopDesk.Legacy.WinForms`
5. Inicie com `F5`.

## Angular

O Angular fica em:

```text
frontend/coopdesk-web
```

Se voce tiver Node.js instalado no Windows:

```powershell
cd frontend/coopdesk-web
pnpm install
pnpm start
```

URL:

```text
http://localhost:4200
```

## Observacao

A API usa SQL Server LocalDB por padrao. Ao iniciar a API em ambiente de desenvolvimento, o banco `CoopDeskDb` e dados iniciais sao criados automaticamente.
