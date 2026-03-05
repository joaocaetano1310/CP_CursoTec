<h1 align="center">
  🎓 CP1 - CursoTec API
</h1>

<p align="center">
  <img src="https://img.shields.io/badge/.NET%2010-5C2D91?style=for-the-badge&logo=.net&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=Swagger&logoColor=black" alt="Swagger" />
  <img src="https://img.shields.io/badge/Clean%20Architecture-000000?style=for-the-badge&logo=awsorganizations&logoColor=white" alt="Clean Architecture" />
</p>

<p align="center">
  <strong>Sistema backend de gestão académica concebido para administrar alunos, professores e cursos técnicos.</strong>
</p>

<hr>

## 👥 Integrantes
* **João Victor Caetano Alves da Silva** — RM: 562074
* **João Victor Bueno Castelini da Silva** — RM: 564115
* **Raul Rezende Iemini Aguiar** — RM: 564002
## 📖 Sobre o Projeto

O **CursoTec** é uma Web API desenvolvida em **.NET 10** cujo objetivo é fornecer a infraestrutura de backend para uma instituição de ensino técnico. O sistema permite o registo e validação de regras de negócio essenciais para garantir a integridade dos dados da instituição.

A aplicação foi desenhada com foco em **Clean Architecture** (Arquitetura Limpa), promovendo um código testável, manutenível e com as responsabilidades devidamente separadas em camadas.

---

## 🚀 Tecnologias Utilizadas

Este projeto foi desenvolvido utilizando as seguintes tecnologias e padrões:

- **[.NET 10](https://dotnet.microsoft.com/)** - Framework principal.
- **[C#](https://learn.microsoft.com/pt-br/dotnet/csharp/)** - Linguagem de programação.
- **[ASP.NET Core Web API](https://learn.microsoft.com/pt-br/aspnet/core/web-api/)** - Para criação dos endpoints RESTful.
- **[OpenAPI / Swagger](https://swagger.io/)** - Para documentação automatizada e testes da API.
- **Princípios DDD (Domain-Driven Design)** - Foco em entidades ricas e regras de domínio isoladas.

---

## 🏗️ Arquitetura do Projeto

O sistema está dividido nas seguintes camadas lógicas:

1. 🌐 **`CP1-CursoTec.API` (Apresentação):** Controladores REST, injeção de dependências e configurações do Swagger.
2. ⚙️ **`CP1-CursoTec.Application` (Aplicação):** Casos de uso, orquestração e Data Transfer Objects (DTOs) para entrada e saída de dados.
3. 🧠 **`CP1-CursoTec.Domain` (Domínio):** O coração da aplicação. Contém as entidades (`Aluno`, `Professor`, `Curso`, `Turma`), validações e regras de negócio essenciais.

---

## ⚙️ Regras de Negócio e Funcionalidades

A camada de domínio garante que os dados obedeçam a regras estritas antes de serem processados:

### 🧑‍🎓 Alunos
- O e-mail fornecido deve ser válido (conter o caractere `@`).
- O aluno **deve ter no mínimo 17 anos** no momento do registo.
- O nome é de preenchimento obrigatório.

### 👨‍🏫 Professores
- O preenchimento da **especialidade** é obrigatório para o registo de um docente.
- O e-mail e nome seguem as mesmas regras de validação aplicadas aos alunos.

### 📚 Cursos
- O registo de um curso exige uma carga horária definida e uma **descrição válida**.
- Nomes de cursos não podem estar em branco.

---

## 🛠️ Como Executar na sua Máquina

### Pré-requisitos
Antes de começar, certifique-se de ter instalado em sua máquina:
- [SDK do .NET 10.0](https://dotnet.microsoft.com/download)
- O seu IDE favorito (recomendado: Visual Studio 2022, Rider ou VS Code)

### Passo a Passo

1. **Clone este repositório**
   ```bash
   git clone [https://github.com/seu-usuario/CP1_CursoTec.git](https://github.com/seu-usuario/CP1_CursoTec.git)
