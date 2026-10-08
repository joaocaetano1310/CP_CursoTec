<h1 align="center">🎓 CP - CursoTec</h1>

<p align="center">
<img src="https://img.shields.io/badge/.NET-10-purple"/>
<img src="https://img.shields.io/badge/C%23-Language-green"/>
<img src="https://img.shields.io/badge/EntityFramework-Core-blue"/>
<img src="https://img.shields.io/badge/Database-SQLite-lightgrey"/>
<img src="https://img.shields.io/badge/API-REST%20%2B%20Swagger-85EA2D"/>
<img src="https://img.shields.io/badge/Architecture-Clean-black"/>
</p>

<p align="center">
API REST de gestão acadêmica com persistência via Entity Framework Core, repositório genérico, Swagger, tratamento global de erros, health check, logs estruturados e testes automatizados.
</p>

---

# 👥 Integrantes

* **João Victor Caetano Alves da Silva** — RM: 562074
* **João Victor Bueno Castelini da Silva** — RM: 564115
* **Raul Rezende Iemini Aguiar** — RM: 564002

---

# 📚 Domínio Escolhido

O domínio escolhido foi **Gestão Acadêmica de Cursos Técnicos**.

O sistema representa a estrutura de uma instituição de ensino, permitindo organizar **alunos, professores, cursos, turmas e aulas**.

## 🧩 Entidades

* **Aluno**
* **Professor**
* **Curso**
* **Turma**
* **Aula**

Todas herdam de `BaseEntity` (chave primária `Id` do tipo **GUID**, `Active` e `CreatedAt`).

## 🔗 Relacionamentos

* **Curso 1:N Turma** (curso opcional na turma)
* **Professor 1:N Turma**
* **Turma 1:N Aula**
* **Turma 1:N Aluno** (um aluno pertence a, no máximo, uma turma)

Diagramas em `CP1-CursoTec.Domain/docs/mer.png` e `CP1-CursoTec.API/docs/schema.png`.

---

# 🗄️ SGBD

**SQLite** (herdado do CP2), acessado com **Entity Framework Core 10**.
A connection string fica em `CP1-CursoTec.API/appsettings.json` (`Data Source=cursotec.db`) e **não contém credenciais**. O arquivo `*.db` não é versionado.

---

# 🏗️ Arquitetura

Padrão **Clean Architecture**, em camadas:

```
CP1-CursoTec.API                → Controllers, Swagger, tratamento global de erros, health checks e logs, composição (DI)
CP1-CursoTec.Application        → Interfaces de repositório (IRepository<T> e por agregado), DTOs e serviços de aplicação
CP1-CursoTec.Domain             → Entidades, regras de domínio e exceções de domínio
CP1-CursoTec.Infrastructure     → DbContext, mapeamentos Fluent API, migrations e repositórios
CP1-CursoTec.Domain.Tests       → Testes xUnit do domínio (sem mock; referencia somente o Domain)
CP1-CursoTec.Application.Tests  → Testes xUnit dos serviços de aplicação (repositórios simulados com Moq)
```

O projeto **API** referencia Application e Infrastructure apenas para composição (injeção de dependência no `Program.cs`). Os controllers **não** recebem `DbContext`: falam com repositórios, e a Infrastructure contém apenas persistência.

---

# ▶️ Como executar

Pré-requisito: **.NET SDK 10**.

```bash
# restaurar e compilar
dotnet build

# executar a API (o banco é criado/atualizado automaticamente pelas migrations)
dotnet run --project CP1-CursoTec.API --launch-profile https
```

| O quê | URL (perfil `https`) | URL (perfil `http`) |
|---|---|---|
| **Swagger UI** | <https://localhost:7264/swagger> | <http://localhost:5104/swagger> |
| **Health check** | <https://localhost:7264/health> | <http://localhost:5104/health> |

* **JSON OpenAPI:** `/swagger/v1/swagger.json`
* O Swagger fica habilitado apenas no ambiente **Development** e não lista o `/health`.
* Se o navegador reclamar do HTTPS, gere e confie no certificado de desenvolvimento: `dotnet dev-certs https --trust`.

## ⚙️ Migrations

A migration inicial (`InitialCreate`) está em `CP1-CursoTec.Infrastructure/Migrations`. Ao subir, a API chama `Database.Migrate()`.
Comandos manuais, se preferir:

```bash
# instalar a ferramenta (uma vez)
dotnet tool install --global dotnet-ef

# aplicar migrations
dotnet ef database update --project CP1-CursoTec.Infrastructure --startup-project CP1-CursoTec.API

# criar uma nova migration
dotnet ef migrations add NomeDaMigration --project CP1-CursoTec.Infrastructure --startup-project CP1-CursoTec.API --output-dir Migrations
```

---

# 🌐 Endpoints

| Recurso | Método | Rota | Sucesso | Erros possíveis |
|---|---|---|---|---|
| Cursos | GET | `/api/cursos` | 200 | 500 |
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

As requisições usam **DTOs** de entrada e saída (`CursoRequest/CursoResponse`, `ProfessorRequest/ProfessorResponse`, `TurmaRequest/TurmaResponse`); as entidades de domínio nunca são expostas.

## 🧪 Exemplos de chamada

```bash
# criar um curso (201)
curl -k -X POST https://localhost:7264/api/cursos \
  -H "Content-Type: application/json" \
  -d '{"nome":"Desenvolvimento de Sistemas","cargaHoraria":1200,"descricao":"Curso técnico"}'

# listar cursos (200)
curl -k https://localhost:7264/api/cursos

# buscar por id (200) — troque pelo id retornado no POST
curl -k https://localhost:7264/api/cursos/{id}

# id inexistente (404 em ProblemDetails)
curl -k https://localhost:7264/api/cursos/00000000-0000-0000-0000-000000000001

# payload inválido (400)
curl -k -X POST https://localhost:7264/api/cursos \
  -H "Content-Type: application/json" \
  -d '{"nome":"","cargaHoraria":0}'

# criar professor e depois uma turma que o referencia
curl -k -X POST https://localhost:7264/api/professores \
  -H "Content-Type: application/json" \
  -d '{"nome":"Maria Souza","email":"maria.souza@cursotec.com","especialidade":"Banco de Dados"}'

curl -k -X POST https://localhost:7264/api/turmas \
  -H "Content-Type: application/json" \
  -d '{"nome":"2TDS-A","dataInicio":"2026-02-02T00:00:00","professorId":"{id-do-professor}"}'
```

O arquivo `CP1-CursoTec.API/CP1-CursoTec.http` traz as mesmas chamadas para uso no Rider/Visual Studio.

---

# 📦 Repositório genérico

* **`IRepository<T>`** (camada Application), com `T : BaseEntity`: `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` e `ExistsByIdAsync`.
* **`Repository<T>`** (camada Infrastructure): usa `DbContext.Set<T>()` e `AsNoTracking` nas leituras de lista e de existência.
* Registro na DI: `services.AddScoped(typeof(IRepository<>), typeof(Repository<>));`
* **Uso real:** `CursosController` (CRUD completo), `ProfessoresController` e a validação de professor/curso no `TurmasController`.
* Os repositórios por agregado do CP2 (`ITurmaRepository`, etc.) foram mantidos: `TurmasController` usa `ITurmaRepository` por precisar carregar curso, professor e alunos (`Include`).

---

# 🛡️ Tratamento global de erros

O `GlobalExceptionHandler` (`CP1-CursoTec.API/Exceptions`) implementa `IExceptionHandler`, registra o erro com `ILogger` e responde com **`ProblemDetails`** (`Content-Type: application/problem+json`). Está registrado com `AddExceptionHandler`, `AddProblemDetails` e `app.UseExceptionHandler()` (antes do Swagger e do `MapControllers`).

| Exceção | Status HTTP |
|---|---|
| `ResourceNotFoundException`, `KeyNotFoundException` | **404** Not Found |
| `DomainException`, `ArgumentException` | **400** Bad Request |
| `ConflictException`, `DbUpdateException` (ex.: valor único duplicado) | **409** Conflict |
| Payload inválido (validação dos DTOs, `[ApiController]`) | **400** Bad Request (`ValidationProblemDetails`) |
| Qualquer outra exceção | **500** Internal Server Error |

Regras de segurança: nunca há stack trace na resposta; erros de banco devolvem mensagem fixa (sem tabelas/colunas); e no 500 a mensagem é genérica fora do ambiente Development.

Formato de uma resposta de erro (404):

```json
{
  "title": "Recurso não encontrado",
  "status": 404,
  "detail": "Curso '00000000-0000-0000-0000-000000000001' não encontrado.",
  "instance": "/api/cursos/00000000-0000-0000-0000-000000000001",
  "traceId": "0HN..."
}
```

---

# ❤️ Health check (`GET /health`)

Único endpoint de integridade, baseado em `Microsoft.Extensions.Diagnostics.HealthChecks`, registrado em `AddCursoTecHealthChecks()` (`CP1-CursoTec.API/Extensions`). Retorna o relatório **completo em JSON** (não apenas o texto `Healthy`).

| Check | O que verifica | Falha vira |
|---|---|---|
| `self` | O processo está no ar | — |
| `database` | `AddDbContextCheck<ApplicationDbContext>`: o `DbContext` do CP2 (**SQLite**) consegue conectar | **Unhealthy** → HTTP 503 |
| `fiap-site` | A URL externa (`https://www.fiap.com.br`, configurável em `HealthChecks:ExternalUrl`) responde em até 3 s | **Degraded** → HTTP 200 com aviso |

**Banco:** abordagem **(A)**, `AddDbContextCheck<TContext>`, alinhada ao `DbContext` do CP2. O pacote é o `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`.

**Status HTTP:** Healthy → **200**, Degraded → **200** (continua servindo tráfego, com aviso), Unhealthy → **503**.

**Impacto da URL externa no status agregado:** o status do `/health` é o **pior** entre os checks. Por isso o `fiap-site` foi registrado com falha *Degraded*: se o site da FIAP cair, o relatório mostra `Degraded` mas a API segue com 200. Se ele fosse registrado como Unhealthy, uma dependência externa fora do ar derrubaria o relatório inteiro (503) mesmo com a API e o banco saudáveis.

Formato da resposta (200, tudo saudável):

```json
{
  "status": "Healthy",
  "totalDurationMs": 41.2,
  "traceId": "0HN...",
  "checks": [
    { "name": "self", "status": "Healthy", "durationMs": 0.1, "description": "Processo no ar." },
    { "name": "database", "status": "Healthy", "durationMs": 12.4 },
    { "name": "fiap-site", "status": "Healthy", "durationMs": 28.7, "description": "https://www.fiap.com.br respondeu 200." }
  ]
}
```

O detalhe de exceção (campo `exception`) só aparece em **Development**.

## Como simular falha do banco (503)

Use o perfil de launch `http-db-invalido`, que aponta a connection string para uma pasta inexistente (sem credenciais):

```bash
dotnet run --project CP1-CursoTec.API --launch-profile http-db-invalido
```

A API sobe (a falha das migrations é apenas registrada no log) e `GET http://localhost:5104/health` responde **503** com `"database": "Unhealthy"`. No Rider, escolha o perfil `http-db-invalido` na lista de execução.

---

# 📝 Observabilidade (logs)

* Logs estruturados com `ILogger<T>` (nativo), com **propriedades nomeadas** e `TraceId = HttpContext.TraceIdentifier`.
* **Fluxos de escrita** (`CursosController`: criar, atualizar e remover; `TurmasController`: criar) registram **início** e **sucesso**:

  ```
  info: Iniciando criação de curso Redes. TraceId=0HN...
  info: Curso 3f1c... criado com sucesso. TraceId=0HN...
  ```
* **`GlobalExceptionHandler`:** toda exceção tratada é logada em nível **Error**, com o mesmo `TraceId` devolvido no `ProblemDetails` (`extensions.traceId`). Assim, quem recebe o erro na resposta consegue achar o detalhe no log.
* Em **Produção** a resposta HTTP continua **sem stack trace**; o detalhe fica só no log.

---

# 🧪 Testes automatizados

Dois projetos **xUnit** na solução:

| Projeto | O que testa | Como |
|---|---|---|
| `CP1-CursoTec.Domain.Tests` | Regras do domínio: `Curso`, `Aluno`, `Professor`, `Turma`, `Aula` | **Sem mock**, padrão AAA, `[Fact]` (caminho feliz) e `[Theory]` + `[InlineData]` (erros que lançam `DomainException`). Referencia **somente** o Domain |
| `CP1-CursoTec.Application.Tests` | `TurmaService` (criação de turma) | Repositórios simulados com **Moq**; professor/curso inexistente lança `ResourceNotFoundException` e `AddAsync` **nunca** é chamado (`Times.Never`); caminho feliz persiste **uma vez** (`Times.Once`). Não sobe API nem banco |

Nomes no estilo `MetodoOuCenario_Condicao_ResultadoEsperado`.

Para executar (a partir da raiz da solução):

```bash
dotnet test
```

Resultado esperado: todos os testes passando. A saída do comando fica registrada em `docs/`.

---

# 🧠 Regras de Domínio

Violações lançam `DomainException` (resposta **400**).

* **Aluno:** nome obrigatório, e-mail válido, CPF obrigatório e idade mínima de 17 anos
* **Professor:** nome obrigatório, e-mail válido e especialidade obrigatória
* **Curso:** nome obrigatório e carga horária maior que zero
* **Turma:** nome obrigatório, professor obrigatório e data de fim não anterior à data de início
* **Aula:** turma obrigatória e hora de fim posterior à hora de início

---

# 🗂️ Documentação e evidências

A pasta [`docs/`](docs/README.md) reúne as evidências: Swagger, respostas `ProblemDetails`, `/health` (saudável e com banco indisponível), trecho de log com `traceId` e a saída do `dotnet test`.

---

# 🏫 Instituição

Projeto acadêmico desenvolvido para a **FIAP – Faculdade de Informática e Administração Paulista**, no curso de **Análise e Desenvolvimento de Sistemas**.
