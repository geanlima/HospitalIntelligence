# Hospital Intelligence Platform — Passo a Passo do Projeto

## Objetivo

Construir uma plataforma hospitalar inteligente, modular e desacoplada de sistemas externos como Salux, FHIR e outros HIS/ERP.

O projeto também será utilizado como trilha prática de estudo de:

- .NET 10
- C#
- Clean Architecture
- Domain-Driven Design
- Monólito Modular
- Entity Framework Core
- PostgreSQL
- Docker
- APIs REST
- Testes automatizados
- Integração de sistemas hospitalares
- Event-Driven Architecture
- Inteligência Artificial
- Machine Learning
- Angular
- Segurança e LGPD
- Observabilidade
- DevOps

---

# Status Geral

| Fase | Nome | Status |
|---|---|---|
| 0 | Fundação | ✅ Concluída |
| 1 | Arquitetura Base / SharedKernel | ✅ Concluída |
| 2 | Patient Domain | 🟡 Próxima fase |
| 3 | Patients Application | 🟡 Parcialmente iniciada |
| 4 | Patients Contracts | 🟡 Estrutura criada |
| 5 | Patients Infrastructure | 🟡 Parcialmente iniciada |
| 6 | Hospital.Api | ⬜ Não iniciada |
| 7 | PostgreSQL + Docker | ⬜ Não iniciada |
| 8 | Testes de domínio/aplicação/integração | 🟡 Parcialmente iniciada |
| 9 | Integration Core | ⬜ Não iniciada |
| 10 | Mock Hospital | ⬜ Não iniciada |
| 11 | Salux Connector | ⬜ Não iniciada |
| 12 | Patient 360 | ⬜ Não iniciada |
| 13 | Angular | ⬜ Não iniciada |
| 14 | Fundação de IA | ⬜ Não iniciada |
| 15 | Busca Inteligente | ⬜ Não iniciada |
| 16 | Auditoria Inteligente | ⬜ Não iniciada |
| 17 | Inteligência Clínica | ⬜ Não iniciada |
| 18 | Machine Learning | ⬜ Não iniciada |
| 19 | Hospital Command Center | ⬜ Não iniciada |
| 20 | Segurança e LGPD | ⬜ Não iniciada |
| 21 | Observabilidade | ⬜ Não iniciada |
| 22 | DevOps | ⬜ Não iniciada |

---

# Fase 0 — Fundação

## Status: ✅ 100% concluída

- [x] Instalar .NET 10 SDK
- [x] Fixar SDK 10.0.400 em `global.json`
- [x] Instalar Visual Studio compatível
- [x] Criar Solution
- [x] Criar estrutura `src`
- [x] Criar estrutura `tests`
- [x] Criar estrutura `docs`
- [x] Criar `src/BuildingBlocks`
- [x] Criar `src/Modules`
- [x] Criar `src/Integrations`
- [x] Criar `src/Hosts`
- [x] Inicializar Git
- [x] Configurar `.gitignore`
- [x] Usar branch `main`
- [x] Configurar remoto `origin`
- [x] Criar `README.md`
- [x] Adicionar projetos iniciais à Solution
- [x] Corrigir referências entre projetos
- [x] Conseguir `dotnet build` com sucesso
- [x] Revisar vulnerabilidades NuGet
- [x] Corrigir `System.Security.Cryptography.Xml`
- [x] Confirmar ausência de pacotes vulneráveis
- [x] Criar commit da fundação
- [x] Confirmar working tree limpo

---

# Fase 1 — Arquitetura Base / SharedKernel

## Status: ✅ 100% concluída

### Projeto

- [x] Criar `Hospital.SharedKernel`
- [x] Adicionar à Solution
- [x] Referenciar pelo `Patients.Domain`

### Domain Building Blocks

- [x] Criar `Entity<TId>`
- [x] Implementar igualdade por identidade
- [x] Criar `AggregateRoot<TId>`
- [x] Criar `IDomainEvent`
- [x] Criar coleção interna de Domain Events
- [x] Criar `RaiseDomainEvent`
- [x] Criar `ClearDomainEvents`

### Application Building Blocks

- [x] Criar `Error`
- [x] Criar `Result`
- [x] Criar `Result<T>`

### Primeiro Domain Event real

- [x] Criar `PatientCreatedDomainEvent`
- [x] Fazer `Patient.Create()` registrar o evento

### Testes

- [x] Criar `Hospital.SharedKernel.UnitTests`
- [x] Criar `ResultTests`
- [x] Criar `EntityTests`
- [x] Criar `AggregateRootTests`
- [x] Validar o projeto de testes na Solution
- [x] Referenciar `Hospital.SharedKernel`

### Decisões

- [x] Adotar Result Pattern para erros esperados
- [x] Adotar Domain Events no domínio
- [x] Adiar `ValueObject` genérico até existir caso real

### Observação sobre Value Objects

Não será criada uma abstração genérica apenas por padrão arquitetural.

O conceito será introduzido quando tivermos casos reais como:

- CPF
- E-mail
- ExternalPatientIdentifier
- Telefone

---

# Fase 2 — Patient Domain

## Status: 🟡 Próxima fase

### Já criado

- [x] `Hospital.Patients.Domain`
- [x] `Patient`
- [x] `PatientId`
- [x] `Gender`
- [x] `PatientDomainException`
- [x] Validação de nome
- [x] Validação de data de nascimento
- [x] `ExternalId`
- [x] `SourceSystem`
- [x] `CreatedAtUtc`
- [x] `UpdatedAtUtc`
- [x] `PatientCreatedDomainEvent`

### Próximos passos

- [ ] Revisar a modelagem atual de `Patient`
- [ ] Criar testes unitários de `Patient`
- [ ] Testar nome inválido
- [ ] Testar data futura
- [ ] Testar origem externa inválida
- [ ] Testar criação com origem externa válida
- [ ] Testar Domain Event
- [ ] Testar alteração de nome
- [ ] Avaliar CPF como Value Object
- [ ] Avaliar e-mail
- [ ] Avaliar telefone
- [ ] Modelar identificação externa de múltiplos HIS
- [ ] Documentar regras do domínio

---

# Fase 3 — Patients Application

## Status: 🟡 Parcialmente iniciada

### Já criado

- [x] `Hospital.Patients.Application`
- [x] `Abstractions`
- [x] `IPatientRepository`

### Pendente

- [ ] `CreatePatientCommand`
- [ ] `CreatePatientHandler`
- [ ] `CreatePatientResult`
- [ ] `GetPatientByIdQuery`
- [ ] `GetPatientByIdHandler`
- [ ] `SearchPatientsQuery`
- [ ] `UpdatePatientCommand`
- [ ] `SynchronizeExternalPatientCommand`
- [ ] Testes da Application

---

# Fase 4 — Patients Contracts

## Status: 🟡 Estrutura criada

- [x] Criar `Hospital.Patients.Contracts`
- [ ] Criar `PatientResponse`
- [ ] Criar contratos de criação
- [ ] Criar contratos de atualização
- [ ] Criar Integration Events públicos

---

# Fase 5 — Patients Infrastructure

## Status: 🟡 Parcialmente iniciada

### Já criado

- [x] `Hospital.Patients.Infrastructure`
- [x] Entity Framework Core
- [x] EF Core Design
- [x] Npgsql
- [x] `PatientsDbContext`
- [x] `PatientConfiguration`
- [x] `PatientRepository`
- [x] Implementação de `IPatientRepository`
- [x] Dependency Injection inicial

### Pendente

- [ ] Revisar `DependencyInjection.cs`
- [ ] Revisar mapeamento EF do `Patient`
- [ ] Configurar índices
- [ ] Criar constraint `(SourceSystem, ExternalId)`
- [ ] Criar migrations
- [ ] Conectar PostgreSQL real

---

# Fase 6 — Hospital.Api

## Status: ⬜ Não iniciada

- [ ] Criar `src/Hosts/Hospital.Api`
- [ ] Adicionar à Solution
- [ ] Configurar `Program.cs`
- [ ] Configurar Dependency Injection
- [ ] Registrar módulos
- [ ] Configurar OpenAPI
- [ ] Configurar Problem Details
- [ ] Criar Exception Handler global
- [ ] Criar Health Checks
- [ ] Criar `POST /api/patients`
- [ ] Criar `GET /api/patients/{id}`
- [ ] Criar `GET /api/patients`

---

# Fase 7 — PostgreSQL + Docker

## Status: ⬜ Não iniciada

- [ ] Criar `docker-compose.yml`
- [ ] Adicionar PostgreSQL
- [ ] Configurar volume persistente
- [ ] Configurar usuário e senha
- [ ] Criar banco `hospital_intelligence`
- [ ] Configurar Connection String
- [ ] Criar migration inicial
- [ ] Executar `database update`
- [ ] Validar tabela `patients`

---

# Fase 8 — Testes

## Domain

- [ ] `Hospital.Patients.UnitTests`
- [ ] Testes de criação
- [ ] Testes de invariantes
- [ ] Testes de eventos

## Application

- [ ] `Hospital.Patients.ApplicationTests`
- [ ] Fake Repository
- [ ] Testes dos Handlers

## Integration

- [ ] Testes com PostgreSQL
- [ ] Testes de Repository
- [ ] Testes da API

## Architecture Tests

- [ ] Criar testes que impeçam `Domain -> Infrastructure`
- [ ] Impedir `Domain -> EF Core`
- [ ] Impedir `Domain -> Salux`

---

# Fase 9 — Integration Core

## Status: ⬜ Não iniciada

- [ ] Criar `Hospital.Integration.Core`
- [ ] Criar contratos de integração
- [ ] Criar modelo canônico
- [ ] Criar abstrações de fontes externas
- [ ] Criar checkpoints
- [ ] Criar estratégia de idempotência
- [ ] Criar logs e retry

---

# Fase 10 — Mock Hospital

## Status: ⬜ Não iniciada

- [ ] Criar `Hospital.Integration.Mock`
- [ ] Gerar pacientes fictícios
- [ ] Gerar internações
- [ ] Gerar exames
- [ ] Gerar prescrições
- [ ] Gerar sinais vitais
- [ ] Gerar evoluções

---

# Fase 11 — Salux Connector

## Status: ⬜ Não iniciada

- [ ] Criar `Hospital.Integration.Salux`
- [ ] Configurar conexão read-only
- [ ] Mapear pacientes
- [ ] Mapear internações
- [ ] Mapear exames
- [ ] Mapear prescrições
- [ ] Mapear evoluções
- [ ] Implementar sincronização incremental
- [ ] Implementar checkpoint
- [ ] Implementar retry
- [ ] Implementar idempotência

Fluxo:

```text
SALUX
  ↓
Salux Adapter
  ↓
Anti-Corruption Layer
  ↓
Canonical Model
  ↓
Hospital Intelligence Platform
```

---

# Fase 12 — Patient 360

- [ ] Dados cadastrais
- [ ] Internações
- [ ] Exames
- [ ] Prescrições
- [ ] Sinais vitais
- [ ] Evoluções
- [ ] Alertas
- [ ] Timeline clínica
- [ ] Resumo clínico

---

# Fase 13 — Angular

- [ ] Criar projeto Angular
- [ ] Criar arquitetura front-end
- [ ] Criar login
- [ ] Criar layout
- [ ] Criar dashboard
- [ ] Criar listagem de pacientes
- [ ] Criar Patient 360
- [ ] Criar timeline
- [ ] Integrar com Hospital.Api

---

# Fase 14 — Fundação de IA

- [ ] Criar módulo `Hospital.AI`
- [ ] Abstração para provedor de LLM
- [ ] Embeddings
- [ ] pgvector
- [ ] RAG
- [ ] Prompt Management
- [ ] Guardrails
- [ ] Auditoria
- [ ] Rastreamento de fontes

---

# Fase 15 — Busca Inteligente

- [ ] Indexar prontuário
- [ ] Busca semântica
- [ ] Perguntas em linguagem natural
- [ ] Evidências
- [ ] Citações das fontes
- [ ] Controle de acesso

---

# Fase 16 — Auditoria Inteligente

- [ ] Auditoria de prontuário
- [ ] Documentação ausente
- [ ] Divergências
- [ ] Auditoria financeira
- [ ] Risco de glosa

---

# Fase 17 — Inteligência Clínica

- [ ] Alta Segura IA
- [ ] Reconciliação Medicamentosa
- [ ] Copiloto de Triagem
- [ ] Prontuário por Voz
- [ ] Deterioração do Paciente

---

# Fase 18 — Machine Learning

- [ ] Serviço/modelos Python
- [ ] Previsão de no-show
- [ ] Previsão de alta
- [ ] Deterioração
- [ ] Feature Engineering
- [ ] Treinamento
- [ ] Avaliação
- [ ] Versionamento de modelos
- [ ] Model Drift

---

# Fase 19 — Hospital Command Center

- [ ] Ocupação
- [ ] Leitos
- [ ] Altas previstas
- [ ] Alertas clínicos
- [ ] Emergência
- [ ] Auditoria
- [ ] Financeiro
- [ ] Indicadores operacionais

---

# Fase 20 — Segurança e LGPD

- [ ] Authentication
- [ ] Authorization
- [ ] Roles
- [ ] Policies
- [ ] Audit Trail
- [ ] Controle de acesso ao prontuário
- [ ] Criptografia
- [ ] Secrets
- [ ] Anonimização
- [ ] LGPD

---

# Fase 21 — Observabilidade

- [ ] Structured Logging
- [ ] OpenTelemetry
- [ ] Traces
- [ ] Metrics
- [ ] Correlation ID
- [ ] Health Checks
- [ ] Dashboards

---

# Fase 22 — DevOps

- [ ] Dockerizar API
- [ ] Dockerizar Angular
- [ ] Dockerizar Workers
- [ ] Pipeline CI
- [ ] Testes automáticos
- [ ] Análise de vulnerabilidades
- [ ] Versionamento
- [ ] CD
- [ ] Deploy

---

# Regra de evolução do projeto

Cada fase deve seguir a sequência:

```text
1. Entender o problema
2. Modelar
3. Implementar
4. Testar
5. Documentar
6. Build verde
7. Commit
8. Avançar
```

A ideia é evitar acumular funcionalidades parcialmente concluídas e manter o projeto utilizável durante toda a evolução.

---

# Próximo passo oficial

## Fase 2 — Patient Domain

Objetivo imediato:

1. revisar `Patient`;
2. criar `Hospital.Patients.UnitTests`;
3. criar testes de invariantes;
4. validar `PatientCreatedDomainEvent`;
5. revisar identificação externa;
6. documentar as regras do domínio;
7. build/test;
8. commit da Fase 2.
