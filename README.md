<h1 align="center">🎓 CP1 - CursoTec API</h1>

<p align="center">
<img src="https://img.shields.io/badge/.NET-10-purple"/>
<img src="https://img.shields.io/badge/C%23-Language-green"/>
<img src="https://img.shields.io/badge/Swagger-API-green"/>
<img src="https://img.shields.io/badge/Architecture-Clean-black"/>
</p>

<p align="center">
Sistema backend de gestão acadêmica concebido para administrar alunos, professores e cursos técnicos.
</p>

---

# 👥 Integrantes

- **João Victor Caetano Alves da Silva** — RM: 562074  
- **João Victor Bueno Castelini da Silva** — RM: 564115  
- **Raul Rezende Iemini Aguiar** — RM: 564002  

---

# 📚 Domínio Escolhido

O domínio escolhido foi **Gestão Acadêmica de Cursos Técnicos**.

O sistema representa a estrutura básica de uma instituição de ensino, permitindo organizar **alunos, professores, cursos e turmas**, bem como o acompanhamento das **matrículas e avaliações** realizadas ao longo do curso.

---

# 🧩 Entidades Modeladas

As seguintes entidades foram definidas no modelo:

- **Aluno**
- **Professor**
- **Curso**
- **Turma**
- **Sala**
- **Matricula**
- **Avaliacao**

Cada entidade possui atributos próprios e uma **chave primária (PK)** responsável por identificar cada registro de forma única.

---

# 🔗 Resumo dos Relacionamentos

Os principais relacionamentos entre as entidades são:

- **Curso 1:N Turma**  
  Um curso pode possuir várias turmas.

- **Professor 1:N Turma**  
  Um professor pode ministrar várias turmas.

- **Sala 1:N Turma**  
  Uma sala pode ser utilizada por várias turmas ao longo do tempo.

- **Aluno N:N Turma**  
  Um aluno pode participar de várias turmas e uma turma pode ter vários alunos.  
  Esse relacionamento é realizado através da entidade **Matricula**.

- **Matricula 1:N Avaliacao**  
  Uma matrícula pode possuir várias avaliações associadas.

---

# 🗂 Estrutura do Repositório

```
CP1-CursoTec

/docs
   mer.png

/src
   CP1-CursoTec.Api
   CP1-CursoTec.Application
   CP1-CursoTec.Domain

README.md
```

- **/docs/mer.png** → Diagrama MER do sistema  
- **/src** → Código fonte organizado em camadas seguindo **Clean Architecture**

---

# 🧠 Regras de Domínio

### 👨‍🎓 Aluno
- O nome é obrigatório
- O e-mail deve conter **@**
- O aluno deve ter **no mínimo 17 anos**

### 👨‍🏫 Professor
- Nome obrigatório
- E-mail válido
- Especialidade obrigatória

### 📚 Curso
- Nome não pode ser vazio
- Carga horária deve ser definida

---

# 📌 Observação

Este projeto tem como objetivo **apenas a modelagem do domínio e do MER**, conforme solicitado na atividade.

Não foram implementados:

- CRUD
- Controllers
- Endpoints
- Integração com banco de dados

O foco está exclusivamente na **modelagem das entidades e seus relacionamentos**.

---

# 🏫 Instituição

Projeto acadêmico desenvolvido para a **FIAP – Faculdade de Informática e Administração Paulista**, no curso de **Análise e Desenvolvimento de Sistemas**.
