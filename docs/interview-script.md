# Roteiro para entrevista

## Pitch curto

Construi o CoopDesk para simular um cenario comum em empresas: usuarios abrem solicitacoes por um aplicativo Windows, enquanto a equipe de suporte atende tudo por uma central web com API REST e Angular.

## Pontos para destacar

- Modelei o dominio de chamados com entidades, status, prioridade e historico.
- Separei as responsabilidades em Domain, Application, Infrastructure e Api.
- Usei ASP.NET Core para expor endpoints RESTful.
- Implementei autenticacao JWT com usuarios demo e autorizacao por perfil.
- Usei Entity Framework Core com SQL Server e relacionamentos entre chamados, departamentos e colaboradores.
- Mantive um cliente Windows Forms consumindo a API para demonstrar atendimento desktop sem expor o banco ao usuario final.
- Criei um front-end Angular para a experiencia moderna.
- Adicionei testes unitarios na camada de aplicacao.
- Removi uma dependencia OpenAPI do template porque ela estava gerando alerta de vulnerabilidade no build.

## Como conectar com a vaga

Na vaga, eles citam manutencao de Windows Forms, migracao para .NET/.NET Core e Angular, CRUD em SQL Server, boas praticas e code review. Este projeto cobre esses pontos em uma aplicacao pequena, mas com arquitetura parecida com um sistema corporativo real, incluindo solicitacao publica, login administrativo, token JWT, perfis e endpoints protegidos.

## Proximas evolucoes

- Migracoes EF Core versionadas.
- Pipeline CI com build, testes e analise de vulnerabilidades.
- Testes de integracao da API.
- Melhorias de observabilidade com logs estruturados.
- Tela Angular de detalhe do chamado com linha do tempo.
