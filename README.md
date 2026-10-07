# Codeflix Backend

API de administração de um catálogo de vídeos, desenvolvida como projeto de estudo em C# e .NET 6. O código explora Clean Architecture e DDD, com persistência em MySQL via Entity Framework Core.

## Arquitetura

| Diretório | Responsabilidade |
|---|---|
| `src/FC.Codeflix.Catalog.Domain` | Entidades e regras de domínio |
| `src/FC.Codeflix.Catalog.Application` | Casos de uso e contratos |
| `src/FC.Codeflix.Catalog.Infra.Data.EF` | Persistência com Entity Framework Core |
| `src/FC.Codeflix.Catalog.Api` | Endpoints HTTP e configuração da aplicação |
| `tests/` | Projetos de testes unitários, de integração e de ponta a ponta |

## Ambiente de desenvolvimento

Requisitos: SDK do .NET 6 e Docker com Compose para o banco de dados. O projeto mantém a versão de .NET usada no estudo.

```sh
git clone https://github.com/felipesbcabral/codeflix-backend.git
cd codeflix-backend
dotnet restore FC.Codeflix.Catalog.sln
docker compose up -d
```

O Compose da raiz inicia o MySQL. A API é executada separadamente. Para executá-la no host, altere apenas `Server=catalogdb` para `Server=localhost` em `ConnectionStrings:CatalogDb`, no arquivo `src/FC.Codeflix.Catalog.Api/appsettings.Development.json`. Preserve os demais parâmetros; o Compose publica a porta 3306. Confira também a configuração de migrations da camada de persistência antes de usar os endpoints.

```sh
dotnet run --project src/FC.Codeflix.Catalog.Api
```

O perfil de desenvolvimento define o Swagger em [https://localhost:7042/swagger](https://localhost:7042/swagger). Também é possível abrir `FC.Codeflix.Catalog.sln` no Visual Studio 2022.

## Testes

Para executar os testes unitários:

```sh
dotnet test tests/FC.Codeflix.Catalog.UnitTests
```

Há projetos separados de integração e de ponta a ponta. Eles dependem de banco e configuração de ambiente; consulte os arquivos em `tests/` antes de executá-los.

## Tecnologias

C#, .NET 6, Entity Framework Core, MySQL, MediatR e Docker.

## Contato

[Felipe Cabral](https://github.com/felipesbcabral) · [LinkedIn](https://www.linkedin.com/in/felipesbcabral/)
