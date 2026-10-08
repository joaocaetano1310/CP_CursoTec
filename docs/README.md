# Evidências da API

Esta pasta guarda as evidências de funcionamento da API (CP3). Coloque aqui as capturas feitas com a API em execução (`dotnet run --project CP1-CursoTec.API --launch-profile https`).

## O que capturar

| Arquivo sugerido | O que mostrar |
|---|---|
| `swagger-endpoints.png` | Tela do Swagger UI (`/swagger`) listando os endpoints de Cursos, Professores e Turmas |
| `swagger-comentarios-xml.png` | Uma ação expandida mostrando o `summary`, o `remarks` e os tipos de resposta (200/201/400/404/409/500) |
| `post-curso-201.png` | `POST /api/cursos` com resposta 201 |
| `get-cursos-200.png` | `GET /api/cursos` com a lista retornada |
| `erro-404-problemdetails.png` | `GET /api/cursos/{id inexistente}` retornando 404 em `application/problem+json` |
| `erro-400-validacao.png` | `POST /api/cursos` com payload inválido retornando 400 |

## Roteiro de teste

| Teste | Resultado esperado |
|---|---|
| `POST /api/cursos` com `{"nome":"Redes","cargaHoraria":800}` | **201** + corpo + header `Location` |
| `GET /api/cursos` | **200** com a lista |
| `GET /api/cursos/{id}` com o id criado | **200** |
| `GET /api/cursos/00000000-0000-0000-0000-000000000001` | **404** em ProblemDetails |
| `POST /api/cursos` com `cargaHoraria: 0` ou sem `nome` | **400** (ProblemDetails de validação) |
| `POST /api/cursos` repetindo o mesmo nome | **409** (ProblemDetails) |
| `POST /api/turmas` com `professorId` inexistente | **404** (ProblemDetails) |

> Anonimize `traceId` e quaisquer dados pessoais antes de publicar as capturas.
