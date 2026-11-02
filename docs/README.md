# Evidências

Esta pasta guarda as capturas e os logs da API em execução (`dotnet run --project CP1-CursoTec.API --launch-profile https`). Os comandos abaixo estão em PowerShell, onde o `curl` precisa ser chamado como `curl.exe`.

## CP3: API REST, Swagger e erros

- `swagger-endpoints.png`: Swagger UI com os endpoints de Cursos, Professores e Turmas.
- `swagger-comentarios-xml.png`: uma ação expandida mostrando os comentários XML e os tipos de resposta.
- `post-curso-201.png`: `POST /api/cursos` retornando 201.
- `erro-404-problemdetails.png`: `GET` de um curso inexistente retornando 404 em `application/problem+json`.
- `erro-400-validacao.png`: `POST` com payload inválido retornando 400.

## CP4: health check, logs e testes

- `health-healthy.json` ou `.png`: `GET /health` com 200 e os checks `self`, `database` e `fiap-site`.
- `health-unhealthy.json` ou `.png`: `GET /health` com 503, com o banco indisponível (perfil `http-db-invalido`).
- `log-post-traceid.txt`: trecho do console de um POST ou PUT, com início, sucesso e `TraceId`.
- `log-erro-traceid.txt`: trecho de uma exceção tratada, com o mesmo `traceId` no `ProblemDetails`.

## Versionamento, paginação e rate limit

| Arquivo | O que mostra |
|---|---|
| `get-v1-cursos.json` | `GET /api/cursos?api-version=1.0`: lista simples (array) |
| `get-v2-cursos.json` | `GET /api/cursos?api-version=2.0`: envelope com `items`, `page`, `pageSize`, `totalItems` e `totalPages` (mesmo recurso) |
| `headers-versoes.txt` | cabeçalhos `api-supported-versions: 2.0` e `api-deprecated-versions: 1.0` |
| `erro-400-paginacao.txt` | 400 com `page=0` e `pageSize=51` (fora das faixas 1 e 1 a 50) |
| `pagina-1.json` e `pagina-2.json` | `pageSize=2` com 5 cursos: `totalItems: 5` e `totalPages: 3` |
| `erro-429-retry-after.txt` | 429 com `Retry-After: 30` e `ProblemDetails`, depois de estourar o limite |
| `health-apos-429.txt` | `GET /health` com 200 logo depois do 429 (a sonda não divide o teto) |
| `dotnet-test.txt` | saída do `dotnet test`: 44 testes, todos passando (testes do CP4 e o da paginação) |

## Como gerar cada evidência

```powershell
# preparação (API rodando em outro terminal)
$base = "https://localhost:7264"

# versionamento e cabeçalhos
curl.exe -k -s -o docs\get-v1-cursos.json "$base/api/cursos?api-version=1.0"
curl.exe -k -s -o docs\get-v2-cursos.json "$base/api/cursos?api-version=2.0"
curl.exe -k -s -D docs\headers-versoes.txt -o NUL "$base/api/cursos?api-version=2.0"

# paginação (espere uns 30 s depois do bloco anterior, por causa do rate limit)
curl.exe -k -s -i -o docs\erro-400-paginacao.txt "$base/api/cursos?page=0&pageSize=51"
curl.exe -k -s -o docs\pagina-1.json "$base/api/cursos?page=1&pageSize=2"
curl.exe -k -s -o docs\pagina-2.json "$base/api/cursos?page=2&pageSize=2"

# rate limit e health logo depois (tudo na mesma janela de 30 s)
1..7 | ForEach-Object { curl.exe -k -s -o NUL -w "%{http_code}`n" "$base/api/cursos" }
curl.exe -k -s -i -o docs\erro-429-retry-after.txt "$base/api/cursos"
curl.exe -k -s -i -o docs\health-apos-429.txt "$base/health"

# testes
dotnet test 2>&1 | Out-File -Encoding utf8 docs\dotnet-test.txt
```

Para as evidências de health do CP4: `curl.exe -k https://localhost:7264/health` com a API normal e, com o perfil `http-db-invalido`, `curl.exe -i http://localhost:5104/health`.

## Roteiro de teste da API

| Teste | Resultado esperado |
|---|---|
| `POST /api/cursos` com nome e carga horária válidos | 201, com corpo e header `Location` |
| `GET /api/cursos?api-version=1.0` | 200 com a lista simples |
| `GET /api/cursos?api-version=2.0` ou sem versão | 200 com o envelope paginado |
| `GET /api/cursos` com `X-Api-Version: 1.0` | 200 com a lista simples |
| `GET /api/cursos?page=0` ou `pageSize=51` | 400 (`ValidationProblemDetails`) |
| `GET /api/cursos?page=1&pageSize=2` e `page=2` | `totalItems` e `totalPages` consistentes |
| mais de 5 `GET /api/cursos` (v2) em 30 s | 429 com `Retry-After` |
| `GET /health` logo depois do 429 | 200 |
| `GET /api/cursos/{id}` com id inexistente | 404 em `ProblemDetails` |
| `POST /api/cursos` com carga horária zero ou sem nome | 400 |
| `POST /api/cursos` com nome repetido | 409 |
| `POST /api/turmas` com `professorId` inexistente | 404 |
| `GET /health` com o perfil `http-db-invalido` | 503 |