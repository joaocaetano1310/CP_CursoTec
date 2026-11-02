# 🎓 CursoTec — API de Gestão Acadêmica

![.NET 10](https://img.shields.io/badge/.NET-10-purple)
![C#](https://img.shields.io/badge/C%23-Language-green)
![EF Core](https://img.shields.io/badge/EntityFramework-Core-blue)
![SQLite](https://img.shields.io/badge/Database-SQLite-lightgrey)
![REST + Swagger](https://img.shields.io/badge/API-REST%20%2B%20Swagger-85EA2D)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-black)

API REST de gestão acadêmica de cursos técnicos, feita em **.NET 10** com **Clean Architecture**. Usa Entity Framework Core com SQLite, repositório genérico, Swagger, versionamento de API, paginação, rate limit, tratamento global de erros, health check, logs estruturados e testes automatizados (xUnit e Moq).

---

## 📑 Sumário

- [Integrantes](#-integrantes)
- [Domínio](#-domínio)
- [SGBD](#️-sgbd)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#️-arquitetura)
- [Como executar](#️-como-executar)
- [Endpoints](#-endpoints)
- [Versionamento](#-versionamento)
- [Paginação](#-paginação)
- [Rate limit](#️-rate-limit)
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

Domínio herdado dos checkpoints anteriores: **gestão acadêmica de cursos técnicos**. O sistema organiza **alunos, professores, cursos, turmas e aulas**. Todas as entidades herdam de `BaseEntity` (`Id` GUID, `Active` e `CreatedAt`).

Relacionamentos:

- Curso **1:N** Turma (o curso é opcional na turma)
- Professor **1:N** Turma
- Turma **1:N** Aula
- Turma **1:N** Aluno (um aluno pertence a, no máximo, uma turma)

Os diagramas estão em `CP1-CursoTec.Domain/docs/mer.png` e `CP1-CursoTec.API/docs/schema.png`.

---

## 🗄️ SGBD

SGBD herdado do CP2: **SQLite**, acessado com **Entity Framework Core 10**. A connection string fica em `CP1-CursoTec.API/appsettings.json` (`Data Source=cursotec.db`), sem credenciais, e o arquivo `*.db` não é versionado.

---

## 🧰 Tecnologias

- **.NET 10** (C# com nullable habilitado)
- **Entity Framework Core 10**, com Fluent API e migrations
- **Asp.Versioning** (versionamento de API) e **Swashbuckle** (Swagger/OpenAPI)
- **Rate limiting** nativo do ASP.NET Core
- **HealthChecks.EntityFrameworkCore** (`Microsoft.Extensions.Diagnostics`)
- **xUnit** e **Moq**

---

## 🏗️ Arquitetura

```
CP1-CursoTec.API                Controllers, Swagger, versionamento, rate limit, erros globais, health check e DI
CP1-CursoTec.Application        Interfaces de repositório, DTOs, PagedResponse (Common) e serviços de aplicação
CP1-CursoTec.Domain             Entidades, regras de domínio e exceções
CP1-CursoTec.Infrastructure     DbContext, mapeamentos, migrations e repositórios
CP1-CursoTec.Domain.Tests       Testes do domínio (sem mock)
CP1-CursoTec.Application.Tests  Testes dos serviços e da paginação (repositórios com Moq)
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

| O quê | URL (perfil `https`) | URL (perfil `http`) |
|---|---|---|
| **Swagger** (grupos v1 e v2) | https://localhost:7264/swagger | http://localhost:5104/swagger |
| **Health check** | https://localhost:7264/health | http://localhost:5104/health |
| **Listagem v1** | https://localhost:7264/api/cursos?api-version=1.0 | http://localhost:5104/api/cursos?api-version=1.0 |
| **Listagem v2** | https://localhost:7264/api/cursos?api-version=2.0 | http://localhost:5104/api/cursos?api-version=2.0 |

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
| Cursos | GET | `/api/cursos` (v1, obsoleta) | 200 (lista simples) | 500 |
| Cursos | GET | `/api/cursos` (v2, paginada, com rate limit) | 200 (envelope) | 400, 429, 500 |
| Cursos | GET | `/api/cursos/{id}` | 200 | 404 |
| Cursos | POST | `/api/cursos` | 201 | 400, 409 |
| Cursos | PUT | `/api/cursos/{id}` | 200 | 400, 404, 409 |
| Cursos | DELETE | `/api/cursos/{id}` | 204 | 404 |
| Professores | GET | `/api/professores` | 200 | 500 |
| Professores | GET | `/api/professores/{id}` | 200 | 404 |
| Professores | POST | `/api/professores` | 201 | 400, 409 |
| Turmas | GET | `/api/turmas` | 200 | 500 |
| Turmas | GET | `/api/turmas/{id}` | 200 | 404 |
| Turmas | POST | `/api/turmas` | 201 | 400, 404 |

### 🧪 Exemplos

O arquivo `CP1-CursoTec.API/CP1-CursoTec.http` tem as mesmas chamadas para Rider/Visual Studio. No PowerShell, use `curl.exe` (o `curl` puro é um apelido de outro comando) e, para enviar JSON, grave o corpo num arquivo e use `-d "@body.json"`.

```bash
# criar curso (201)
curl -k -X POST https://localhost:7264/api/cursos \
  -H "Content-Type: application/json" \
  -d '{"nome":"Desenvolvimento de Sistemas","cargaHoraria":1200,"descricao":"Curso técnico"}'

# listagem v1 e v2 (query e header)
curl -k "https://localhost:7264/api/cursos?api-version=1.0"
curl -k -H "X-Api-Version: 2.0" "https://localhost:7264/api/cursos?page=1&pageSize=2"

# parâmetro de paginação inválido (400)
curl -k -i "https://localhost:7264/api/cursos?page=0&pageSize=51"

# id inexistente (404)
curl -k https://localhost:7264/api/cursos/00000000-0000-0000-0000-000000000001
```

---

## 🔀 Versionamento

A listagem de cursos existe em duas versões. A versão pode ser informada de três formas:

| Como | Exemplo |
|---|---|
| **Query string** | `GET /api/cursos?api-version=1.0` |
| **Header** | `X-Api-Version: 1.0` |
| **Omitida** | assume a versão **2.0** (padrão) |

| Versão | Situação | Resposta |
|---|---|---|
| **1.0** | obsoleta (*deprecated*) | lista simples de cursos |
| **2.0** | atual e padrão | envelope paginado, com rate limit |

As respostas trazem os cabeçalhos `api-supported-versions: 2.0` e `api-deprecated-versions: 1.0`. Os demais endpoints de Cursos aceitam as duas versões, e Professores e Turmas não dependem de versão. O Swagger mostra os dois grupos (v1 e v2), com a v1 marcada como obsoleta.

---

## 📄 Paginação

Vale para a listagem **v2** de cursos (`GET /api/cursos`):

| Parâmetro | Padrão | Regra |
|---|---|---|
| `page` | 1 | mínimo 1 |
| `pageSize` | 10 | de 1 a **50** (teto) |

Valores fora dessa faixa retornam **400** (a API não ajusta sozinha). Uma página além da última retorna 200 com `items` vazio. A ordem é fixa, por `Id`, para as páginas não repetirem nem pularem itens.

```json
{
  "items": [
    { "id": "2922e279-...", "nome": "Curso 1", "cargaHoraria": 100, "descricao": "a" },
    { "id": "2e634d04-...", "nome": "Curso 3", "cargaHoraria": 100, "descricao": "a" }
  ],
  "page": 1,
  "pageSize": 2,
  "totalItems": 5,
  "totalPages": 3
}
```

---

## ⏱️ Rate limit

| | |
|---|---|
| **Endpoint limitado** | `GET /api/cursos` na **v2** (política `listagem`) |
| **Limite** | 5 requisições |
| **Janela** | 30 segundos (janela fixa), sem fila |
| **Ao exceder** | **429 Too Many Requests**, com o cabeçalho `Retry-After` (segundos a aguardar) e corpo `ProblemDetails` |
| **Fora do limite** | a listagem v1, os demais endpoints e o `/health` |

Como o limite só vale para esse endpoint, o `/health` continua respondendo 200 mesmo depois do estouro, e a sonda não divide o teto. Exemplo de resposta:

```
HTTP/1.1 429 Too Many Requests
Content-Type: application/problem+json
Retry-After: 30

{ "title": "Muitas requisições", "status": 429,
  "detail": "Limite de 5 requisições a cada 30 segundos para a listagem de cursos. Aguarde e tente novamente.",
  "traceId": "0HN..." }
```

---

## 📦 Repositório genérico

`IRepository<T>` (Application, com `T : BaseEntity`) define `GetAllAsync`, `GetPagedAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` e `ExistsByIdAsync`. O `GetPagedAsync(page, pageSize)` devolve os itens da página e o total de registros. A implementação `Repository<T>` fica na Infrastructure, usa `DbContext.Set<T>()` e `AsNoTracking` nas leituras. O registro é feito assim:

```csharp
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

Ele é usado no `CursosController` (CRUD completo e listagem paginada), no `ProfessoresController` e na validação de professor e curso do `TurmasController`. O `ITurmaRepository`, do CP2, foi mantido porque o `TurmasController` precisa carregar curso, professor e alunos com `Include`.

---

## 🛡️ Tratamento global de erros

O `GlobalExceptionHandler` (`CP1-CursoTec.API/Exceptions`) implementa `IExceptionHandler`, loga o erro e responde com **`ProblemDetails`** (`application/problem+json`).

| Exceção | Status |
|---|---|
| `ResourceNotFoundException`, `KeyNotFoundException` | 404 |
| `DomainException`, `ArgumentException` | 400 |
| `ConflictException`, `DbUpdateException` (ex.: valor único duplicado) | 409 |
| Payload ou parâmetro inválido (DTOs, `page`, `pageSize`) | 400 (`ValidationProblemDetails`) |
| Qualquer outra | 500 |

A resposta nunca traz stack trace, erros de banco voltam com mensagem fixa e o 500 tem mensagem genérica fora de Development. O 429 do rate limit também responde em `ProblemDetails` (veja a seção de rate limit).

---

## ❤️ Health check

`GET /health` retorna o relatório em JSON com o status geral e o resultado de cada verificação:

| Check | O que verifica | Se falhar |
|---|---|---|
| `self` | a API está no ar | — |
| `database` | conexão com o SQLite | Unhealthy (503) |
| `fiap-site` | o site da FIAP responde em até 3 s | Degraded (200) |

O site da FIAP é só *Degraded* de propósito: uma dependência externa fora do ar não deve derrubar o health check se a API e o banco estão bem. Para simular a falha do banco, rode com o perfil `http-db-invalido`.

---

## 📝 Logs

Os logs usam `ILogger<T>` com propriedades nomeadas e `TraceId = HttpContext.TraceIdentifier`. Os fluxos de escrita (criar, atualizar e remover curso; criar turma) registram início e sucesso:

```
info: Iniciando criação de curso Redes. TraceId=0HN...
info: Curso 3f1c... criado com sucesso. TraceId=0HN...
```

Toda exceção tratada pelo `GlobalExceptionHandler` é logada como Error com o mesmo `TraceId` da resposta (`extensions.traceId`), então dá para achar o detalhe no log a partir do erro recebido.

---

## 🧪 Testes

```bash
dotnet test
```

São 44 testes (34 no Domain e 10 na Application), todos passando.

- **`CP1-CursoTec.Domain.Tests`**: regras de `Curso`, `Aluno`, `Professor`, `Turma` e `Aula`, sem mock e referenciando só o Domain. Usa `[Fact]` para o caminho feliz e `[Theory]` com `[InlineData]` para os casos que lançam `DomainException`.
- **`CP1-CursoTec.Application.Tests`**: `TurmaService` com repositórios simulados em Moq (professor ou curso inexistente lança `ResourceNotFoundException` e `AddAsync` nunca é chamado) e o cálculo de `totalPages` do `PagedResponse` (25, 20, 1 e 0 itens), sem subir API nem banco.

Os testes seguem o padrão AAA e os nomes `MetodoOuCenario_Condicao_ResultadoEsperado`.

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

A pasta [`docs/`](docs/README.md) reúne as evidências: Swagger, erros em `ProblemDetails`, health check (saudável e com banco indisponível), logs com `traceId`, JSON da listagem v1 e v2, cabeçalhos de versão, paginação, 400 de parâmetro inválido, 429 com `Retry-After`, `/health` 200 depois do estouro do limite e a saída do `dotnet test`.

---

## 🏫 Instituição

Projeto acadêmico da **FIAP**, curso de Análise e Desenvolvimento de Sistemas.