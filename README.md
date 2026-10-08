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
API REST de gestão acadêmica com persistência via Entity Framework Core, repositório genérico, Swagger e tratamento global de erros.
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
CP1-CursoTec.API             → Controllers, DTOs expostos, Swagger, tratamento global de erros, composição (DI)
CP1-CursoTec.Application     → Interfaces de repositório (IRepository<T> e por agregado) e DTOs
CP1-CursoTec.Domain          → Entidades, regras de domínio e exceções de domínio
CP1-CursoTec.Infrastructure  → DbContext, mapeamentos Fluent API, migrations e repositórios
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

* **Swagger UI:** <https://localhost:7264/swagger> (ou <http://localhost:5104/swagger> com o perfil `http`)
* **JSON OpenAPI:** `/swagger/v1/swagger.json`
* O Swagger fica habilitado apenas no ambiente **Development**.

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

# 🧠 Regras de Domínio

Violações lançam `DomainException` (resposta **400**).

* **Aluno:** nome obrigatório, e-mail válido, CPF obrigatório e idade mínima de 17 anos
* **Professor:** nome obrigatório, e-mail válido e especialidade obrigatória
* **Curso:** nome obrigatório e carga horária maior que zero
* **Turma:** nome obrigatório, professor obrigatório e data de fim não anterior à data de início
* **Aula:** turma obrigatória e hora de fim posterior à hora de início

---

# 🗂️ Documentação e evidências

A pasta [`docs/`](docs/README.md) reúne as evidências da API (prints do Swagger e exemplos de resposta `ProblemDetails`).

---

# 🏫 Instituição

Projeto acadêmico desenvolvido para a **FIAP – Faculdade de Informática e Administração Paulista**, no curso de **Análise e Desenvolvimento de Sistemas**.
