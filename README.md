# 🎓 CursoTec — API de Gestão Acadêmica

![.NET 10](https://img.shields.io/badge/.NET-10-purple)
![C#](https://img.shields.io/badge/C%23-Language-green)
![EF Core](https://img.shields.io/badge/EntityFramework-Core-blue)
![SQLite](https://img.shields.io/badge/Database-SQLite-lightgrey)
![MongoDB](https://img.shields.io/badge/Database-MongoDB-47A248)
![JWT](https://img.shields.io/badge/Auth-JWT-orange)
![REST + Swagger](https://img.shields.io/badge/API-REST%20%2B%20Swagger-85EA2D)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-black)

API REST de gestão acadêmica de cursos técnicos, feita em **.NET 10** com **Clean Architecture**. Usa Entity Framework Core com SQLite, repositório genérico, Swagger, versionamento de API, paginação, rate limit, tratamento global de erros, health check, logs estruturados e testes automatizados (xUnit e Moq).

No **CP6**, a API ganhou **autenticação JWT** (login por e-mail e senha, endpoints protegidos com 401 e 403) e um segundo banco, o **MongoDB**, que guarda o histórico de recomendação como documento, ao lado do SQLite.

---

## 📑 Sumário

- [Integrantes](#-integrantes)
- [Domínio](#-domínio)
- [SGBD](#️-sgbd)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#️-arquitetura)
- [Como executar](#️-como-executar)
- [Endpoints](#-endpoints)
- [Autenticação JWT (CP6)](#-autenticação-jwt-cp6)
- [MongoDB — histórico de recomendação (CP6)](#-mongodb--histórico-de-recomendação-cp6)
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

No CP6, o **`Aluno`** passou a ser também o **usuário que faz login**: ganhou senha (guardada só como hash) e papel (`Aluno` ou `Admin`).

Os diagramas estão em `CP1-CursoTec.Domain/docs/mer.png` e `CP1-CursoTec.API/docs/schema.png`.

---

## 🗄️ SGBD

A API usa dois bancos, cada um com seu papel:

| Banco | O que guarda | Acesso |
|---|---|---|
| **SQLite** (herdado do CP2) | alunos, professores, cursos, turmas e aulas | Entity Framework Core 10 |
| **MongoDB** (CP6, Atlas) | histórico de recomendação (coleção `recommendationLogs`, banco `recommenda`) | `MongoDB.Driver` 3.x |

A connection string do SQLite fica em `CP1-CursoTec.API/appsettings.json` (`Data Source=cursotec.db`), sem credenciais, e o arquivo `*.db` não é versionado. A do MongoDB **não fica no repositório**: vem dos *user-secrets* (veja [Como executar](#️-como-executar)).

---

## 🧰 Tecnologias

- **.NET 10** (C# com nullable habilitado)
- **Entity Framework Core 10**, com Fluent API e migrations
- **Asp.Versioning** (versionamento de API) e **Swashbuckle** (Swagger/OpenAPI)
- **Rate limiting** nativo do ASP.NET Core
- **HealthChecks.EntityFrameworkCore** (`Microsoft.Extensions.Diagnostics`) e **AspNetCore.HealthChecks.MongoDb**
- **Microsoft.AspNetCore.Authentication.JwtBearer** (JWT assinado em HMAC-SHA256)
- **MongoDB.Driver** 3.x
- **xUnit** e **Moq**

---

## 🏗️ Arquitetura

```
CP1-CursoTec.API                Controllers, Swagger, versionamento, rate limit, erros globais, health check, JWT e DI
CP1-CursoTec.Application        Interfaces de repositório, DTOs, PagedResponse (Common), serviços de aplicação e contratos do login e do histórico
CP1-CursoTec.Domain             Entidades, regras de domínio (inclusive senha e papel do aluno) e exceções
CP1-CursoTec.Infrastructure     DbContext, mapeamentos, migrations, repositórios e o repositório MongoDB (pasta Mongo)
CP1-CursoTec.Domain.Tests       Testes do domínio (sem mock)
CP1-CursoTec.Application.Tests  Testes dos serviços, da paginação e do login (repositórios simulados)
```

A API só referencia Application e Infrastructure para montar a injeção de dependência no `Program.cs`. Os controllers não usam o `DbContext` nem o driver do MongoDB: falam apenas com serviços e repositórios. As entidades nunca são expostas: a entrada e a saída usam **DTOs**.

Divisão do CP6 entre as camadas:

| Camada | O que entrou |
|---|---|
| **Domain** | senha com hash (`PasswordHasher`), `Aluno.VerifyPassword`, papel (`PapelUsuario`) e regra de senha mínima |
| **Application** | `AuthService` e `ITokenGenerator` (contrato do login); DTOs, `IRecommendationLogRepository` e `RecommendationLogService` (contrato e validações do histórico). **Sem** `MongoDB.Driver` |
| **Infrastructure** | `MongoRecommendationLogRepository` e o registro do `MongoClient` |
| **API** | `JwtTokenGenerator`, configuração do `JwtBearer`, Swagger com Bearer, `AuthController` e `RecommendationLogsController` |

---

## ▶️ Como executar

Requer o [.NET SDK 10](https://dotnet.microsoft.com/download) e um cluster MongoDB (o gratuito do [MongoDB Atlas](https://www.mongodb.com/atlas) serve).

**1. Configure a connection string do MongoDB** (uma vez por máquina). Ela não fica no repositório: o `appsettings.json` tem a chave `ConnectionStrings:RecommendaMongo` **vazia**, e o valor real vai nos *user-secrets*:

```bash
dotnet user-secrets set "ConnectionStrings:RecommendaMongo" "mongodb+srv://USUARIO:SENHA@SEU-CLUSTER.mongodb.net/?retryWrites=true&w=majority" --project CP1-CursoTec.API
```

Em outro ambiente, use a variável `ConnectionStrings__RecommendaMongo`. Se a string estiver vazia, a API **não sobe** e mostra no console como configurar. No Atlas, libere o IP da máquina em *Network Access*.

**2. Rode a API:**

```bash
dotnet build
dotnet run --project CP1-CursoTec.API --launch-profile https
```

As migrations do SQLite são aplicadas automaticamente na subida (`Database.Migrate()`).

| O quê | URL (perfil `https`) | URL (perfil `http`) |
|---|---|---|
| **Swagger** (grupos v1 e v2) | https://localhost:7264/swagger | http://localhost:5104/swagger |
| **Health check** | https://localhost:7264/health | http://localhost:5104/health |
| **Listagem v1** | https://localhost:7264/api/cursos?api-version=1.0 | http://localhost:5104/api/cursos?api-version=1.0 |
| **Listagem v2** | https://localhost:7264/api/cursos?api-version=2.0 | http://localhost:5104/api/cursos?api-version=2.0 |
| **Login** | https://localhost:7264/api/v2/auth/login | http://localhost:5104/api/v2/auth/login |

O Swagger só fica ativo em Development. Se o navegador reclamar do certificado, rode `dotnet dev-certs https --trust`.

### ⚙️ Migrations

As migrations ficam em `CP1-CursoTec.Infrastructure/Migrations`: a inicial (`InitialCreate`) e a do CP6 (`AddSenhaPapelAluno`, que adiciona `SenhaHash` e `Papel` à tabela `Alunos`). Para rodar manualmente:

```bash
dotnet tool install --global dotnet-ef

dotnet ef database update --project CP1-CursoTec.Infrastructure --startup-project CP1-CursoTec.API

dotnet ef migrations add NomeDaMigration --project CP1-CursoTec.Infrastructure --startup-project CP1-CursoTec.API --output-dir Migrations
```

---

## 🌐 Endpoints

A coluna **Acesso** indica o que o endpoint exige no CP6: **público**, **token** (qualquer usuário logado) ou **Admin** (token com papel `Admin`).

| Recurso | Método | Rota | Acesso | Sucesso | Erros |
|---|---|---|---|---|---|
| Auth | POST | `/api/v2/auth/login` | público | 200 (token) | 400, 401 |
| Auth | GET | `/api/v2/auth/me` | token | 200 (claims) | 401 |
| Alunos | POST | `/api/v2/alunos` | público | 201 | 400 |
| Alunos | GET | `/api/v2/alunos/{id}` | público | 200 | 404 |
| Cursos | GET | `/api/cursos` (v1, obsoleta) | público | 200 (lista simples) | 500 |
| Cursos | GET | `/api/cursos` (v2, paginada, com rate limit) | público | 200 (envelope) | 400, 429, 500 |
| Cursos | GET | `/api/cursos/{id}` | público | 200 | 404 |
| Cursos | POST | `/api/cursos` | público | 201 | 400, 409 |
| Cursos | PUT | `/api/cursos/{id}` | público | 200 | 400, 404, 409 |
| Cursos | DELETE | `/api/cursos/{id}` | público | 204 | 404 |
| Professores | GET | `/api/professores` | público | 200 | 500 |
| Professores | GET | `/api/professores/{id}` | público | 200 | 404 |
| Professores | POST | `/api/professores` | **Admin** | 201 | 400, 401, 403, 409 |
| Turmas | GET | `/api/v1/turmas` (obsoleta) | público | 200 (lista simples) | 500 |
| Turmas | GET | `/api/v2/turmas` (paginada) | público | 200 (envelope) | 400, 500 |
| Turmas | GET | `/api/turmas/{id}` | público | 200 | 404 |
| Turmas | POST | `/api/v2/turmas` | **token** | 201 | 400, 401, 404 |
| Histórico | POST | `/api/v2/recommendation-logs` | **token** | 201 | 400, 401 |
| Histórico | GET | `/api/v2/recommendation-logs/{id}` | **token** | 200 | 401, 404 |
| Histórico | GET | `/api/v2/recommendation-logs/user/{userId}` | **token** | 200 | 400, 401 |
| Histórico | PUT | `/api/v2/recommendation-logs/{id}` | **token** | 204 | 400, 401, 404 |
| Histórico | DELETE | `/api/v2/recommendation-logs/{id}` | **token** | 204 | 401, 404 |
| Saúde | GET | `/health` | público, fora do rate limit | 200 | 503 |

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

## 🔐 Autenticação JWT (CP6)

### Quem faz login

O usuário do enunciado é o **`Aluno`**. Ele é cadastrado em `POST /api/v2/alunos` (público), sempre com o papel `Aluno`; o papel não é aceito no corpo da requisição, então não há cadastro público de `Admin`.

A senha **nunca é salva em texto**: o Domain guarda só o hash (PBKDF2-SHA256 com salt aleatório) e a confere em `Aluno.VerifyPassword`. O token também não carrega a senha.

### Onde a API lê a configuração

Os valores ficam na seção `Jwt` do `appsettings.json` do projeto `CP1-CursoTec.API`, lidos pela classe `JwtOptions`:

```json
"Jwt": {
  "Issuer": "CP1-CursoTec.API",
  "Audience": "CP1-CursoTec.Clients",
  "Key": "chave-de-aula-...",
  "ExpirationMinutes": 120
}
```

| Chave | Uso |
|---|---|
| `Jwt:Issuer` | emissor gravado no token e validado em cada requisição |
| `Jwt:Audience` | público gravado no token e validado em cada requisição |
| `Jwt:Key` | chave de assinatura HMAC-SHA256 (mínimo de 32 caracteres) |
| `Jwt:ExpirationMinutes` | validade do token (120 minutos) |

A `Key` versionada é **apenas uma chave de aula**, não uma senha de produção. Em outro ambiente, os valores podem ser sobrescritos pelas variáveis `Jwt__Issuer`, `Jwt__Audience` e `Jwt__Key`. Se a seção não existir ou a chave tiver menos de 32 caracteres, a API não sobe e informa o motivo.

### Token e validação

O token é assinado com **HMAC-SHA256**, expira em 120 minutos e carrega as claims `sub` (id do aluno), `email`, `name` e `role`. O `JwtBearer` valida emissor, público, validade e assinatura, com `RoleClaimType = "role"` e `MapInboundClaims = false`, para que a claim `role` mantenha esse nome e o `[Authorize(Roles = ...)]` a reconheça.

No pipeline, `UseAuthentication()` e `UseAuthorization()` ficam depois de `UseHttpsRedirection()` e antes de `MapControllers()`.

### Papéis e endpoints protegidos

| Endpoint | Exige | Sem token | Token de outro papel |
|---|---|---|---|
| `GET /api/v2/auth/me` | qualquer usuário logado | 401 | — |
| `POST /api/v2/turmas` | qualquer usuário logado | 401 | — |
| `POST /api/professores` | papel **`Admin`** | 401 | **403** |
| rotas de `/api/v2/recommendation-logs` | qualquer usuário logado | 401 | — |

### Roteiro de correção

**1. Criar o usuário** (público):
```http
POST /api/v2/alunos
Content-Type: application/json

{
  "nome": "Ana Souza",
  "email": "ana@example.com",
  "cpf": "123.456.789-09",
  "dataNascimento": "2004-05-20",
  "senha": "senha1234"
}
```
→ **201**, sem senha nem hash na resposta.

**2. Login com senha errada:**
```http
POST /api/v2/auth/login
Content-Type: application/json

{ "email": "ana@example.com", "password": "errada123" }
```
→ **401** com `ProblemDetails` ("E-mail ou senha inválidos."). A resposta é a mesma para e-mail inexistente: não diz qual dos dois falhou.

**3. Login correto:**
```http
POST /api/v2/auth/login
Content-Type: application/json

{ "email": "ana@example.com", "password": "senha1234" }
```
→ **200** com `accessToken`, `expiration`, `email`, `name` e `role`.

**4. `me` sem token:** `GET /api/v2/auth/me` → **401**.

**5. `me` com token:**
```http
GET /api/v2/auth/me
Authorization: Bearer {accessToken}
```
→ **200** com a lista de claims do token.

**6. Escrita sem token:** `POST /api/v2/turmas` → **401**.

**7. Papel errado (403):** com o token do passo 3, que tem `role: Aluno`:
```http
POST /api/professores
Authorization: Bearer {accessToken}
Content-Type: application/json

{ "nome": "Maria Souza", "email": "maria@example.com", "especialidade": "Banco de Dados" }
```
→ **403**, porque o endpoint exige `Admin`.

### Pelo Swagger

1. Escolha a definição **v2** no canto superior direito.
2. Faça o login em `POST /api/v2/auth/login` e copie o valor de `accessToken`.
3. Clique em **Authorize** e cole **só o token**, sem a palavra `Bearer`. O Swagger envia `Authorization: Bearer {token}` sozinho.
4. As operações protegidas aparecem com cadeado; o login e o cadastro de aluno não têm cadeado.

---

## 🍃 MongoDB — histórico de recomendação (CP6)

O **histórico de recomendação** (o que a API sugeriu para um usuário: a consulta e a lista de títulos) é guardado no MongoDB, na coleção `recommendationLogs` do banco `recommenda`. Ele **não** virou tabela no SQLite, e nada do banco relacional foi migrado para o Mongo.

### Registro na injeção de dependência

Feito em `AddRecommendaMongo` (Infrastructure), antes dos health checks:

- um único `MongoClient` por processo (**singleton**);
- `IMongoDatabase` singleton, apontando para o banco `recommenda`;
- `GuidSerializer(GuidRepresentation.Standard)` registrado antes do primeiro `Guid` gravado (driver 3.x);
- connection string vazia → falha explícita na subida, explicando como configurar o *user-secret*.

O controller não fala com o driver: quem grava é o `MongoRecommendationLogRepository`, na Infrastructure.

### Documento

```json
{
  "_id": "6f1c0a2e-1b4a-4c3d-9e10-0a1b2c3d4e5f",
  "userId": "11111111-1111-1111-1111-111111111111",
  "query": "filme de terror curto",
  "titles": ["A Bruxa", "Hereditário"],
  "createdAt": "2026-10-19T01:00:00Z"
}
```

`_id` é um `Guid` gerado pela API e marcado como identidade do documento. `userId` é outro `Guid` e não precisa existir no banco relacional.

### Contrato HTTP (todas as rotas exigem token)

| Operação | HTTP | Sucesso | Não encontrado |
|---|---|---|---|
| Criar | `POST /api/v2/recommendation-logs` | **201** com o item, inclusive o `id` | — |
| Ler um | `GET /api/v2/recommendation-logs/{id}` | **200** | **404** |
| Listar por usuário | `GET /api/v2/recommendation-logs/user/{userId}` | **200** com array (pode ser vazio) | — |
| Atualizar | `PUT /api/v2/recommendation-logs/{id}` | **204** | **404** |
| Apagar | `DELETE /api/v2/recommendation-logs/{id}` | **204** | **404** |

- O `PUT` altera **só** `query` e `titles` (`UpdateOne`); `userId`, `createdAt` e `_id` permanecem.
- A listagem vem ordenada por `createdAt` **decrescente** e é cortada **no próprio banco** (máximo de 20), sem carregar a coleção inteira.

**Validações na Application** (400 com a regra na mensagem):
- `query` vazia;
- `titles` ausente, vazio ou com item em branco;
- `userId` vazio (`00000000-0000-0000-0000-000000000000`) no POST e na listagem.

### Roteiro de correção

Com o token do login (botão **Authorize** no Swagger):

1. `POST /api/v2/recommendation-logs` **sem** token → **401**.
2. Mesmo POST **com** token:
   ```json
   {
     "userId": "11111111-1111-1111-1111-111111111111",
     "query": "filme de terror curto",
     "titles": ["A Bruxa", "Hereditário"]
   }
   ```
   → **201**, com um `id` diferente do `userId`.
3. `GET /api/v2/recommendation-logs/{id}` → **200**, o mesmo documento.
4. `GET /api/v2/recommendation-logs/user/11111111-1111-1111-1111-111111111111` → **200**, com esse log na lista.
5. `PUT /api/v2/recommendation-logs/{id}`:
   ```json
   {
     "query": "filme de terror curto, dublado",
     "titles": ["A Bruxa", "Hereditário", "O Babadook"]
   }
   ```
   → **204**. O GET seguinte mostra `query` e `titles` novos, com o mesmo `userId` e `createdAt`.
6. `DELETE /api/v2/recommendation-logs/{id}` → **204**. O GET seguinte → **404**.
7. `POST` com `"titles": []` → **400**.

---

## 🔀 Versionamento

A versão pode ser informada de quatro formas:

| Como | Exemplo |
|---|---|
| **Segmento da URL** | `GET /api/v2/turmas` |
| **Query string** | `GET /api/cursos?api-version=1.0` |
| **Header** | `X-Api-Version: 1.0` |
| **Omitida** | assume a versão **2.0** (padrão) |

| Versão | Situação | Listagem de cursos e turmas |
|---|---|---|
| **1.0** | obsoleta (*deprecated*) | lista simples |
| **2.0** | atual e padrão | envelope paginado (cursos com rate limit) |

As respostas trazem os cabeçalhos `api-supported-versions` e `api-deprecated-versions`. Professores não dependem de versão (`[ApiVersionNeutral]`). Os endpoints do CP6 (auth e histórico) existem só na **v2**. O Swagger mostra os dois grupos (v1 e v2), com a v1 marcada como obsoleta.

---

## 📄 Paginação

| Listagem | Parâmetros | Teto de `pageSize` |
|---|---|---|
| **Cursos v2** (`GET /api/cursos`) | `page` (padrão 1), `pageSize` (padrão 10) | **50** |
| **Turmas v2** (`GET /api/v2/turmas`) | `page` (padrão 1), `pageSize` (padrão 20) | **100** |

Valores fora da faixa retornam **400** (a API não ajusta sozinha). Uma página além da última retorna 200 com `items` vazio. A ordem é fixa, para as páginas não repetirem nem pularem itens.

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

O `/health` é mapeado com `DisableRateLimiting()`, então continua respondendo mesmo depois do estouro, e a sonda não divide o teto. Exemplo de resposta:

```
HTTP/1.1 429 Too Many Requests
Content-Type: application/problem+json
Retry-After: 30

{ "title": "Muitas requisições", "status": 429,
  "detail": "Limite de requisições excedido. Tente novamente em 30 segundo(s).",
  "traceId": "0HN..." }
```

---

## 📦 Repositório genérico

`IRepository<T>` (Application, com `T : BaseEntity`) define `GetAllAsync`, `GetPagedAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` e `ExistsByIdAsync`. O `GetPagedAsync(page, pageSize)` devolve os itens da página e o total de registros. A implementação `Repository<T>` fica na Infrastructure, usa `DbContext.Set<T>()` e `AsNoTracking` nas leituras. O registro é feito assim:

```csharp
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

Ele é usado no `CursosController` (CRUD completo e listagem paginada), no `ProfessoresController` e na validação de professor e curso do `TurmaService`. O `ITurmaRepository`, do CP2, foi mantido porque turmas precisam carregar curso, professor e alunos com `Include`. O `IAlunoRepository` ganhou no CP6 a busca por e-mail, usada no login e para recusar e-mail repetido no cadastro.

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

A resposta nunca traz stack trace, erros de banco voltam com mensagem fixa e o 500 tem mensagem genérica fora de Development. O 429 do rate limit e o 401 do login também respondem em `ProblemDetails`.

---

## ❤️ Health check

`GET /health` é **público** (sem JWT) e fica **fora do rate limit**. Retorna o relatório em JSON com o status geral e o resultado de cada verificação, **separadas**:

| Check | O que verifica | Se falhar |
|---|---|---|
| `self` | a API está no ar | — |
| `database` | conexão com o SQLite | Unhealthy (503) |
| `mongodb` | o cluster MongoDB responde (banco `recommenda`, timeout de 5 s) | Unhealthy (503) |
| `fiap-site` | o site da FIAP responde em até 3 s | Degraded (200) |

O site da FIAP é só *Degraded* de propósito: uma dependência externa fora do ar não deve derrubar o health check se a API e os bancos estão bem. Para simular a falha do SQLite, rode com o perfil `http-db-invalido`. Como cada item aparece separado, dá para ler o estado do MongoDB mesmo quando o SQLite está fora.

---

## 📝 Logs

Os logs usam `ILogger<T>` com propriedades nomeadas e `TraceId = HttpContext.TraceIdentifier`. Os fluxos de escrita (criar, atualizar e remover curso; criar turma; cadastrar aluno) registram início e sucesso:

```
info: Iniciando criação de curso Redes. TraceId=0HN...
info: Curso 3f1c... criado com sucesso. TraceId=0HN...
```

O login registra só o resultado (sucesso ou recusa), **sem** e-mail nem senha.

Toda exceção tratada pelo `GlobalExceptionHandler` é logada como Error com o mesmo `TraceId` da resposta (`extensions.traceId`), então dá para achar o detalhe no log a partir do erro recebido.

---

## 🧪 Testes

```bash
dotnet test
```

Os testes do CP4 e do CP5 continuam na solução, e o CP6 acrescentou novos:

- **`CP1-CursoTec.Domain.Tests`**: regras de `Curso`, `Aluno`, `Professor`, `Turma` e `Aula`, sem mock e referenciando só o Domain. No CP6, o `AlunoTests` passou a cobrir a senha (hash diferente do texto, `VerifyPassword` certa e errada, senha curta lança `DomainException`) e o papel padrão `Aluno`.
- **`CP1-CursoTec.Application.Tests`**: `TurmaService` com repositórios simulados em Moq (professor ou curso inexistente lança `ResourceNotFoundException` e `AddAsync` nunca é chamado), paginação de turmas, cálculo de `totalPages` do `PagedResponse` e, no CP6, o **`AuthServiceTests`**: senha certa devolve token com `email` e `role`; senha errada ou e-mail inexistente devolvem `null`. Tudo sem subir API nem banco.

Os testes usam `[Fact]` para o caminho feliz e `[Theory]` com `[InlineData]` para os casos de erro, seguem o padrão AAA e os nomes `MetodoOuCenario_Condicao_ResultadoEsperado`.

---

## 🧠 Regras de domínio

Violações lançam `DomainException`, devolvida pela API como 400.

- **Aluno:** nome obrigatório, e-mail válido, CPF obrigatório, idade mínima de 17 anos, senha com pelo menos 8 caracteres (guardada só como hash) e papel `Aluno` por padrão
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
