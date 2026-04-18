<h1 align="center">🎓 CP2 - CursoTec</h1>

<p align="center">
<img src="https://img.shields.io/badge/.NET-10-purple"/>
<img src="https://img.shields.io/badge/C%23-Language-green"/>
<img src="https://img.shields.io/badge/EntityFramework-Core-blue"/>
<img src="https://img.shields.io/badge/Database-SQLite-lightgrey"/>
<img src="https://img.shields.io/badge/Architecture-Clean-black"/>
</p>

<p align="center">
Sistema backend de gestão acadêmica com persistência de dados utilizando Entity Framework Core.
</p>

---

# 👥 Integrantes

* **João Victor Caetano Alves da Silva** — RM: 562074
* **João Victor Bueno Castelini da Silva** — RM: 564115
* **Raul Rezende Iemini Aguiar** — RM: 564002

---

# 📚 Domínio Escolhido

O domínio escolhido foi **Gestão Acadêmica de Cursos Técnicos**.

O sistema representa a estrutura de uma instituição de ensino, permitindo organizar:

* Alunos
* Professores
* Cursos
* Turmas
* Salas

---

# 🧩 Entidades Modeladas

* **Aluno**
* **Professor**
* **Curso**
* **Turma**
* **Aula**

Todas as entidades possuem chave primária (**PK**) baseada em **GUID**, conforme padrão adotado no projeto.

---

# 🔗 Relacionamentos

* **Curso 1:N Turma**
* **Professor 1:N Turma**
* **Sala 1:N Turma (opcional em Turma)**
* **Turma 1:N Aula**
* **Aluno N:N Turma (via Matricula)**

---

# 🏗️ Arquitetura

O projeto segue o padrão **Clean Architecture**, organizado em camadas:

```
CP1-CursoTec

CP1-CursoTec.API
CP1-CursoTec.Application
CP1-CursoTec.Domain
CP1-CursoTec.Infrastructure
```

### 📦 Responsibilities

* **API** → Configuração da aplicação e injeção de dependência
* **Application** → Interfaces de repositório
* **Domain** → Entidades e regras de negócio
* **Infrastructure** → Persistência, EF Core, Mappings e Repositories

---

# 🗄️ Persistência de Dados

A persistência foi implementada utilizando:

* **Entity Framework Core**
* **SQLite**

## 🔹 DbContext

Classe responsável por representar o banco de dados:

* `ApplicationDbContext`

## 🔹 Mapeamento (Fluent API)

As entidades foram configuradas utilizando:

* `IEntityTypeConfiguration<T>`
* Definição de:

  * Tabelas
  * Chaves primárias
  * Relacionamentos
  * Índices

## 🔹 Repositórios

Implementação do padrão Repository:

* Interfaces na camada **Application**
* Implementações na camada **Infrastructure**

Exemplo:

* `IAlunoRepository`
* `AlunoRepository`

---

# ⚙️ Migrations

O banco de dados é gerado através de migrations do EF Core.

### Criar migration:

```bash
dotnet ef migrations add InitialCreate
```

### Atualizar banco:

```bash
dotnet ef database update
```

---

# 🔌 Injeção de Dependência

Os serviços foram registrados no `Program.cs`:

```csharp
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
```

---

# 🧠 Regras de Domínio

### 👨‍🎓 Aluno

* Nome obrigatório
* E-mail válido
* Idade mínima de 17 anos

### 👨‍🏫 Professor

* Nome obrigatório
* E-mail válido
* Especialidade obrigatória

### 📚 Curso

* Nome obrigatório
* Carga horária definida

---

# 📌 Observações

Este projeto implementa:

✔ Persistência com EF Core
✔ Mapeamento com Fluent API
✔ Repositórios
✔ Clean Architecture

Não inclui:

❌ Regras de negócio complexas na Infrastructure
❌ Dados sensíveis no repositório

---

# 🏫 Instituição

Projeto acadêmico desenvolvido para a **FIAP – Faculdade de Informática e Administração Paulista**, no curso de **Análise e Desenvolvimento de Sistemas**.
