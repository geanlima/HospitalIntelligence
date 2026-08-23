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
| 2 | Patient Domain | ✅ Concluída |
| 3 | Patients Application | 🟡 95% — aguardando validação final |
| 4 | Patients Contracts | 🟡 Estrutura criada |
| 5 | Patients Infrastructure | 🟡 Parcialmente iniciada |
| 6 | Hospital.Api | ⬜ Não iniciada |
| 7 | PostgreSQL + Docker | ⬜ Não iniciada |
| 8 | Testes de domínio/aplicação/integração | 🟡 Domínio e Application concluídos |
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

## Status: ✅ 100% concluída

### Domínio

- [x] `Hospital.Patients.Domain`
- [x] `Patient`
- [x] `PatientId`
- [x] `Gender`
- [x] `PatientDomainException`
- [x] Validação de nome
- [x] Validação de data de nascimento
- [x] `CreatedAtUtc`
- [x] `UpdatedAtUtc`
- [x] `PatientCreatedDomainEvent`
- [x] `Patient.Create()` registra Domain Event

### Value Object — identificação externa

- [x] Criar `ExternalPatientIdentifier`
- [x] Encapsular `SourceSystem`
- [x] Encapsular `ExternalId`
- [x] Validar valores obrigatórios
- [x] Validar tamanho máximo de `SourceSystem`
- [x] Validar tamanho máximo de `ExternalId`
- [x] Normalizar valores com `Trim`
- [x] Garantir igualdade por valor usando `record`
- [x] Refatorar `Patient` para usar `ExternalIdentifier`

### Comportamentos do Patient

- [x] `ChangeName`
- [x] `ChangeBirthDate`
- [x] `ChangeGender`
- [x] `UpdateExternalIdentifier`
- [x] Atualizar `UpdatedAtUtc` quando há mudança real
- [x] Não alterar `UpdatedAtUtc` quando o valor não muda

### Testes de domínio

- [x] Criar `Hospital.Patients.UnitTests`
- [x] Testar criação válida
- [x] Testar nome vazio
- [x] Testar nome em branco
- [x] Testar nome curto
- [x] Testar nome acima do limite
- [x] Testar data de nascimento futura
- [x] Testar criação com identificação externa
- [x] Testar `PatientCreatedDomainEvent`
- [x] Testar timestamp do Domain Event
- [x] Testar `ChangeName`
- [x] Testar `ChangeBirthDate`
- [x] Testar `ChangeGender`
- [x] Testar `UpdateExternalIdentifier`
- [x] Testar `CreatedAtUtc`
- [x] Testar `UpdatedAtUtc`
- [x] Criar `ExternalPatientIdentifierTests`
- [x] Testar validações e igualdade do Value Object

### Impactos da refatoração

- [x] Atualizar `PatientRepository` para consultar por `ExternalIdentifier`
- [x] Manter `IPatientRepository.GetByExternalIdAsync(sourceSystem, externalId)` como contrato de consulta
- [x] Validar domínio e testes após a refatoração
- [x] Build/test do projeto após os ajustes

### Decisões da fase

- [x] Introduzir Value Object apenas quando surgiu um caso real
- [x] Manter `Patient` responsável apenas pelas regras do paciente
- [x] Mover as regras de identificação externa para `ExternalPatientIdentifier`
- [x] Adiar CPF, e-mail e telefone até existir requisito real do produto


---

# Fase 3 — Patients Application

## Status: 🟡 95% concluída — aguardando validação final

### Estrutura base

- [x] `Hospital.Patients.Application`
- [x] `Abstractions`
- [x] `IPatientRepository`
- [x] `GetByIdAsync`
- [x] `GetByExternalIdAsync`
- [x] `SearchAsync`
- [x] `AddAsync`
- [x] `UpdateAsync`

### Caso de uso — CreatePatient

- [x] `CreatePatientCommand`
- [x] `CreatePatientHandler`
- [x] Criar paciente sem identificação externa
- [x] Criar paciente com identificação externa
- [x] Validar `SourceSystem + ExternalId`
- [x] Verificar duplicidade por identificador externo
- [x] Retornar `Result<PatientId>`
- [x] Padronizar erro `Patient.ExternalIdentifier.Invalid`
- [x] Padronizar erro `Patient.ExternalIdentifier.AlreadyExists`
- [x] Testes do `CreatePatientHandler`

### Caso de uso — GetPatientById

- [x] `GetPatientByIdQuery`
- [x] `GetPatientByIdHandler`
- [x] Consultar paciente por `PatientId`
- [x] Retornar `Patient.NotFound` quando não encontrado
- [x] Testes de paciente encontrado
- [x] Testes de paciente não encontrado

### Caso de uso — SearchPatients

- [x] `SearchPatientsQuery`
- [x] `SearchPatientsHandler`
- [x] Busca sem filtro
- [x] Busca parcial por nome
- [x] Retorno de coleção vazia quando não há resultados
- [x] `SearchAsync` no `FakePatientRepository`
- [x] `SearchAsync` no `PatientRepository`
- [x] Testes de busca

### Caso de uso — UpdatePatient

- [x] `UpdatePatientCommand`
- [x] `UpdatePatientHandler`
- [x] Buscar paciente antes da atualização
- [x] Retornar `Patient.NotFound`
- [x] Atualizar nome via `Patient.ChangeName`
- [x] Atualizar nascimento via `Patient.ChangeBirthDate`
- [x] Atualizar gênero via `Patient.ChangeGender`
- [x] `UpdateAsync` no `IPatientRepository`
- [x] `UpdateAsync` no `FakePatientRepository`
- [x] `UpdateAsync` no `PatientRepository`
- [x] Testes de atualização válida
- [x] Testes de paciente inexistente
- [x] Testes das invariantes do Domain durante atualização

### Caso de uso — SynchronizeExternalPatient

- [x] `SynchronizeExternalPatientCommand`
- [x] `SynchronizeExternalPatientHandler`
- [x] Criar paciente quando a identificação externa não existe
- [x] Atualizar paciente quando a identificação externa já existe
- [x] Manter o mesmo `PatientId` em sincronizações
- [x] Utilizar `ExternalPatientIdentifier`
- [x] Testar origem externa inválida
- [x] Criar `SynchronizeExternalPatientHandlerTests`

### Testes da Application

- [x] Criar `Hospital.Patients.ApplicationTests`
- [x] Criar `FakePatientRepository`
- [x] Separar testes de Domain e Application
- [x] Testes de `CreatePatient`
- [x] Testes de `GetPatientById`
- [x] Testes de `SearchPatients`
- [x] Testes de `UpdatePatient`
- [x] Testes de `SynchronizeExternalPatient`

### Padronização de erros

- [x] `Patient.NotFound`
- [x] `Patient.ExternalIdentifier.Invalid`
- [x] `Patient.ExternalIdentifier.AlreadyExists`

### Validação final da fase

- [ ] Confirmar execução final de `dotnet test`
- [ ] Confirmar execução final de `dotnet build`
- [ ] Atualizar `PROJECT_ROADMAP.md`
- [ ] Commit da Fase 3
- [ ] Push para `origin/main`


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

- [x] `Hospital.Patients.UnitTests`
- [x] Testes de criação
- [x] Testes de invariantes
- [x] Testes de eventos
- [x] Testes de `ExternalPatientIdentifier`

## Application

- [x] `Hospital.Patients.ApplicationTests`
- [x] `FakePatientRepository`
- [x] Testes de `CreatePatientHandler`
- [x] Testes de `GetPatientByIdHandler`
- [x] Testes de `SearchPatientsHandler`
- [x] Testes de `UpdatePatientHandler`
- [x] Testes de `SynchronizeExternalPatientHandler`

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

## Fechamento da Fase 3 — Patients Application

Antes de avançar:

1. executar `dotnet test`;
2. executar `dotnet build`;
3. confirmar todos os testes verdes;
4. revisar `git status`;
5. criar commit da Fase 3;
6. executar push para `origin/main`.

Após essa validação:

## Fase 4 — Patients Contracts

Objetivo inicial:

1. revisar a responsabilidade do projeto `Hospital.Patients.Contracts`;
2. criar `PatientResponse`;
3. criar contratos de criação e atualização;
4. separar contratos públicos dos tipos internos do Domain;
5. preparar Integration Events públicos do módulo.
