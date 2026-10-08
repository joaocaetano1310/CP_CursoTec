# Evidências (CP3 e CP4)

Esta pasta guarda as evidências de funcionamento. Coloque aqui as capturas feitas com a API em execução
(`dotnet run --project CP1-CursoTec.API --launch-profile https`).

## CP3: API REST, Swagger e erros

| Arquivo sugerido | O que mostrar |
|---|---|
| `swagger-endpoints.png` | Swagger UI (`/swagger`) listando os endpoints de Cursos, Professores e Turmas |
| `swagger-comentarios-xml.png` | Uma ação expandida com `summary`, `remarks` e os tipos de resposta |
| `post-curso-201.png` | `POST /api/cursos` com resposta 201 |
| `erro-404-problemdetails.png` | `GET /api/cursos/{id inexistente}` retornando 404 em `application/problem+json` |
| `erro-400-validacao.png` | `POST /api/cursos` com payload inválido retornando 400 |

## CP4: health check, logs e testes

| Arquivo sugerido | O que mostrar |
|---|---|
| `health-healthy.json` ou `.png` | `GET /health` com **200** e os checks `self`, `database` e `fiap-site` |
| `health-unhealthy.json` ou `.png` | `GET /health` com **503** (`database` Unhealthy), usando o perfil `http-db-invalido` |
| `log-post-traceid.txt` | Trecho do console de um `POST`/`PUT` com as linhas de início e sucesso e o `TraceId` |
| `log-erro-traceid.txt` | Trecho do console de uma exceção tratada (nível Error) e o mesmo `traceId` no `ProblemDetails` |
| `dotnet-test.txt` ou `.png` | Saída do `dotnet test` (ou do Test Explorer) com todos os testes passando |

### Como gerar cada evidência

```bash
# health saudável
curl -k https://localhost:7264/health

# health com banco indisponível (503): rode a API com o perfil abaixo e chame o /health
dotnet run --project CP1-CursoTec.API --launch-profile http-db-invalido
curl -i http://localhost:5104/health

# testes
dotnet test > docs/dotnet-test.txt
```

## Roteiro de teste da API

| Teste | Resultado esperado |
|---|---|
| `POST /api/cursos` com `{"nome":"Redes","cargaHoraria":800}` | **201** + corpo + header `Location` |
| `GET /api/cursos` | **200** com a lista |
| `GET /api/cursos/{id}` com o id criado | **200** |
| `GET /api/cursos/00000000-0000-0000-0000-000000000001` | **404** em ProblemDetails |
| `POST /api/cursos` com `cargaHoraria: 0` ou sem `nome` | **400** (ProblemDetails de validação) |
| `POST /api/cursos` repetindo o mesmo nome | **409** (ProblemDetails) |
| `POST /api/turmas` com `professorId` inexistente | **404** (ProblemDetails) |
| `GET /health` com banco ok | **200** |
| `GET /health` com o perfil `http-db-invalido` | **503** |

> Anonimize `traceId` e quaisquer dados pessoais antes de publicar as capturas.
