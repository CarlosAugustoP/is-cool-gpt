#  IsCool API

A **IsCool API** é um backend moderno e escalável desenvolvido em **ASP.NET Core 8.0**, projetado para fornecer funcionalidades educacionais dinâmicas. Ela integra recursos avançados de IA usando a **OpenAI API**, permitindo geração de conteúdo personalizado, resumos e prompts educacionais sob demanda.

A arquitetura foi construída para alta disponibilidade, utilizando **Azure Container Apps**, pipelines automatizados e um banco de dados gerenciado via **PostgreSQL Flexible Server**.

---

##  Visão Geral do Projeto

- Backend em **ASP.NET Core 8**
- Deploy automatizado em **Azure Container Apps**
- Banco de dados **PostgreSQL Flexible Server**
- Geração de conteúdo via **OpenAI API**
- Autenticação via **JWT**
- Testes automatizados com **XUnit** e **Moq**
- Pipeline CI/CD via **GitHub Actions**

---

## Arquitetura e Funcionalidades Principais

| Componente | Detalhes |
|-----------|----------|
| **Ambiente de Hospedagem** | Azure Container Apps (ACA) para escalabilidade e gestão de containers |
| **Banco de Dados** | PostgreSQL Flexible Server — seguro, gerenciado e confiável |
| **Geração de Conteúdo** | Integração com OpenAI API para NLP e conteúdo educacional |
| **Segurança** | Autenticação via JWT (Bearer) |
| **Qualidade de Código** | Testes unitários com XUnit + Moq |
| **Workflow de Desenvolvimento** | CI/CD automatizado contendo build, testes, criação de imagem Docker e deploy |

---
