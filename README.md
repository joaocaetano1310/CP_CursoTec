# 🎓 CursoTec — API de Gestão Acadêmica

![.NET 10](https://img.shields.io/badge/.NET-10-purple)
![C#](https://img.shields.io/badge/C%23-Language-green)
![EF Core](https://img.shields.io/badge/EntityFramework-Core-blue)
![SQLite](https://img.shields.io/badge/Database-SQLite-lightgrey)
![REST + Swagger](https://img.shields.io/badge/API-REST%20%2B%20Swagger-85EA2D)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-black)

API REST de gestão acadêmica de cursos técnicos, feita em **.NET 10** com **Clean Architecture**. Usa Entity Framework Core com SQLite, repositório genérico, Swagger, tratamento global de erros, health check, logs estruturados e testes automatizados (xUnit e Moq).

---

## 📑 Sumário

- [Integrantes](#-integrantes)
- [Domínio](#-domínio)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#️-arquitetura)
- [Como executar](#️-como-executar)
- [Endpoints](#-endpoints)
- [Repositório genérico](#-repositório-genérico)
- [Tratamento global de erros](#️-tratamento-global-de-erros)
- [Health check](#️-health-check)
- [Logs](#-logs)
- [Testes](#-testes)
- [Regras de domínio](#-regras-de-domínio)
- [Evidências](#️-evidências)
- [Instituição](#-instituição)

---

## 👥 Integrantes

- **João Victor Caetano Alves da Silva** — RM 562074
- **João Victor Bueno Castelini da Silva** — RM 564115
- **Raul Rezende Iemini Aguiar** — RM 564002

---

## 📚 Domínio

Gestão acadêmica de cursos técnicos: o sistema organiza **alunos, professores, cursos, turmas e aulas**. Todas as entidades herdam de `BaseEntity` (`Id` GUID, `Active` e `CreatedAt`).

Relacionamentos:

- Curso **1:N** Turma (o curso é opcional na turma)
- Professor **1:N** Turma
- Turma **1:N** Aula
- Turma **1:N** Aluno (um aluno pertence a, no máximo, uma turma)

Os diagramas estão em `CP1-CursoTec.Domain/docs/mer.png` e `CP1-CursoTec.API/docs/schema.png`.

---

## 🧰 Tecnologias

- **.NET 10** (C# com nullable habilitado)
- **Entity Framework Core 10**, com Fluent API e migrations
- **SQLite** (`Data Source=cursotec.db`, configurado no `appsettings.json` da API, sem credenciais; o `*.db` não é versionado)
- **Swashbuckle** (Swagger/OpenAPI)
- **HealthChecks.EntityFrameworkCore** (`Microsoft.Extensions.Diagnostics`)
- **xUnit** e **Moq**

---

## 🏗️ Arquitetura

```
CP1-CursoTec.API                Controllers, Swagger, erros globais, health check, logs e DI
CP1-CursoTec.Application        Interfaces de repositório, DTOs e serviços de aplicação
CP1-CursoTec.Domain             Entidades, regras de domínio e exceções
CP1-CursoTec.Infrastructure     DbContext, mapeamentos, migrations e repositórios
CP1-CursoTec.Domain.Tests       Testes do domínio (sem mock)
CP1-CursoTec.Application.Tests  Testes dos serviços (repositórios com Moq)
```

A API só referencia Application e Infrastructure para montar a injeção de dependência no `Program.cs`. Os controllers não usam o `DbContext`, falam apenas com repositórios, e as entidades nunca são expostas: a entrada e a saída usam **DTOs**.

---

## ▶️ Como executar

Requer o [.NET SDK 10](https://dotnet.microsoft.com/download).

```bash
dotnet build
dotnet run --project CP1-CursoTec.API --launch-profile https
```

As migrations são aplicadas automaticamente na subida (`Database.Migrate()`).

| | https | http |
|---|---|---|
| **Swagger** | https://localhost:7264/swagger | http://localhost:5104/swagger |
| **Health check** | https://localhost:7264/health | http://localhost:5104/health |

O Swagger só fica ativo em Development. Se o navegador reclamar do certificado, rode `dotnet dev-certs https --trust`.

### ⚙️ Migrations

A migration inicial (`InitialCreate`) fica em `CP1-CursoTec.Infrastructure/Migrations`. Para rodar manualmente:

```bash
dotnet tool install --global dotnet-ef

dotnet ef database update --project CP1-CursoTec.Infrastructure --startup-project CP1-CursoTec.API

dotnet ef migrations add NomeDaMigration --project CP1-CursoTec.Infrastructure --startup-project CP1-CursoTec.API --output-dir Migrations
```

---

## 🌐 Endpoints

| Recurso | Método | Rota | Sucesso | Erros |
|---|---|---|---|---|
| Cursos | GET | `/api/cursos` | 200 | 500 |
| Cursos | GET | `/api/cursos/{id}` | 200 | 404 |
	@@ -129,31 +127,25 @@ dotnet ef migrations add NomeDaMigration --project CP1-CursoTec.Infrastructure -
| Turmas | GET | `/api/turmas/{id}` | 200 | 404 |
| Turmas | POST | `/api/turmas` | 201 | 400, 404 |

### 🧪 Exemplos

O arquivo `CP1-CursoTec.API/CP1-CursoTec.http` tem as mesmas chamadas para Rider/Visual Studio.

```bash
# criar curso (201)
curl -k -X POST https://localhost:7264/api/cursos \
  -H "Content-Type: application/json" \
  -d '{"nome":"Desenvolvimento de Sistemas","cargaHoraria":1200,"descricao":"Curso técnico"}'

# payload inválido (400)
curl -k -X POST https://localhost:7264/api/cursos \
  -H "Content-Type: application/json" \
  -d '{"nome":"","cargaHoraria":0}'

# id inexistente (404)
curl -k https://localhost:7264/api/cursos/00000000-0000-0000-0000-000000000001

# criar professor e depois uma turma com ele
curl -k -X POST https://localhost:7264/api/professores \
  -H "Content-Type: application/json" \
  -d '{"nome":"Maria Souza","email":"maria.souza@cursotec.com","especialidade":"Banco de Dados"}'
	@@ -163,35 +155,33 @@ curl -k -X POST https://localhost:7264/api/turmas \
  -d '{"nome":"2TDS-A","dataInicio":"2026-02-02T00:00:00","professorId":"{id-do-professor}"}'
```

---

## 📦 Repositório genérico

`IRepository<T>` (Application, com `T : BaseEntity`) define `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` e `ExistsByIdAsync`. A implementação `Repository<T>` fica na Infrastructure, usa `DbContext.Set<T>()` e `AsNoTracking` nas leituras de lista e de existência. O registro é feito assim:

```csharp
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

Ele é usado no `CursosController` (CRUD completo), no `ProfessoresController` e na validação de professor e curso do `TurmasController`. O `ITurmaRepository`, do CP2, foi mantido porque o `TurmasController` precisa carregar curso, professor e alunos com `Include`.

---

## 🛡️ Tratamento global de erros

O `GlobalExceptionHandler` (`CP1-CursoTec.API/Exceptions`) implementa `IExceptionHandler`, loga o erro e responde com **`ProblemDetails`** (`application/problem+json`). Está registrado com `AddExceptionHandler`, `AddProblemDetails` e `app.UseExceptionHandler()`, antes do Swagger e do `MapControllers`.

| Exceção | Status |
|---|---|
| `ResourceNotFoundException`, `KeyNotFoundException` | 404 |
| `DomainException`, `ArgumentException` | 400 |
| `ConflictException`, `DbUpdateException` (ex.: valor único duplicado) | 409 |
| Payload inválido (validação dos DTOs) | 400 (`ValidationProblemDetails`) |
| Qualquer outra | 500 |

A resposta nunca traz stack trace, erros de banco voltam com mensagem fixa (sem expor tabelas ou colunas) e o 500 tem mensagem genérica fora de Development. Exemplo de 404:

```json
{
	@@ -205,104 +195,66 @@ Formato de uma resposta de erro (404):

---

## ❤️ Health check

`GET /health` retorna o relatório em JSON com o status geral e o resultado de cada verificação:

| Check | O que verifica | Se falhar |
|---|---|---|
| `self` | a API está no ar | — |
| `database` | conexão com o SQLite | Unhealthy (503) |
| `fiap-site` | o site da FIAP responde em até 3 s | Degraded (200) |

O site da FIAP é só *Degraded* de propósito: uma dependência externa fora do ar não deve derrubar o health check se a API e o banco estão bem. Para simular a falha do banco, rode com o perfil `http-db-invalido`. A API sobe (a falha das migrations só vai para o log) e `GET http://localhost:5104/health` responde 503 com `database: Unhealthy`.
```bash
dotnet run --project CP1-CursoTec.API --launch-profile http-db-invalido
```
---

## 📝 Logs

Os logs usam `ILogger<T>` com propriedades nomeadas e `TraceId = HttpContext.TraceIdentifier`. Os fluxos de escrita (criar, atualizar e remover curso; criar turma) registram início e sucesso:

```
info: Iniciando criação de curso Redes. TraceId=0HN...
info: Curso 3f1c... criado com sucesso. TraceId=0HN...
```

Toda exceção tratada pelo `GlobalExceptionHandler` é logada como Error com o mesmo `TraceId` da resposta (`extensions.traceId`), então dá para achar o detalhe no log a partir do erro recebido. Em produção o detalhe fica só no log.

---

## 🧪 Testes

```bash
dotnet test
```

- **`CP1-CursoTec.Domain.Tests`**: regras de `Curso`, `Aluno`, `Professor`, `Turma` e `Aula`, sem mock e referenciando só o Domain. Usa `[Fact]` para o caminho feliz e `[Theory]` com `[InlineData]` para os casos que lançam `DomainException`.
- **`CP1-CursoTec.Application.Tests`**: `TurmaService` com repositórios simulados em Moq. Professor ou curso inexistente lança `ResourceNotFoundException` e `AddAsync` nunca é chamado; no caminho feliz ele é chamado uma vez. Não sobe API nem banco.

Os testes seguem o padrão AAA e os nomes `MetodoOuCenario_Condicao_ResultadoEsperado`. A saída do `dotnet test` está registrada em `docs/`.

---

## 🧠 Regras de domínio

Violações lançam `DomainException`, devolvida pela API como 400.

- **Aluno:** nome obrigatório, e-mail válido, CPF obrigatório e idade mínima de 17 anos
- **Professor:** nome obrigatório, e-mail válido e especialidade obrigatória
- **Curso:** nome obrigatório e carga horária maior que zero
- **Turma:** nome obrigatório, professor obrigatório e data de fim não anterior à de início
- **Aula:** turma obrigatória e hora de fim posterior à de início

---

## 🗂️ Evidências

A pasta [`docs/`](docs/README.md) reúne as evidências: Swagger, respostas `ProblemDetails`, `/health` (saudável e com banco indisponível), trecho de log com `traceId` e a saída do `dotnet test`.

---

## 🏫 Instituição

Projeto acadêmico da **FIAP**, curso de Análise e Desenvolvimento de Sistemas.
