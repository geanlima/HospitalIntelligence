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
| 3 | Patients Application | ✅ Concluída |
| 4 | Patients Contracts | ✅ Concluída |
| 5 | Patients Infrastructure | ✅ Concluída |
| 6 | Hospital.Api | 🟡 95% — implementação validada; commit/push pendente |
| 7 | PostgreSQL + Docker | 🟡 95% — implementação e testes concluídos; fechamento pendente |
| 8 | Testes de domínio/aplicação/integração | 🟡 Domínio, Application e Repository integration concluídos; API tests pendentes |
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

## Status: ✅ 100% concluída

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

- [x] Confirmar execução final de `dotnet test`
- [x] Confirmar execução final de `dotnet build`
- [x] Atualizar `PROJECT_ROADMAP.md`
- [x] Fase 3 preparada para commit/push final


---

# Fase 4 — Patients Contracts

## Status: ✅ 100% concluída

### Projeto e dependências

- [x] Criar `Hospital.Patients.Contracts`
- [x] Adicionar à Solution
- [x] Referenciar `Hospital.Patients.Contracts` pela Application
- [x] Manter `Hospital.Patients.Contracts` sem dependência do Domain
- [x] Manter `Hospital.Patients.Contracts` sem dependência da Application
- [x] Manter `Hospital.Patients.Contracts` sem dependência da Infrastructure

### Contratos de saída

- [x] Criar `PatientResponse`
- [x] Expor tipos simples (`Guid`, `string`, `DateOnly`, `DateTimeOffset`)
- [x] Não expor `Patient`, `PatientId`, `Gender` ou `ExternalPatientIdentifier`
- [x] Criar mapeamento `Patient -> PatientResponse`
- [x] Alterar `GetPatientByIdHandler` para retornar `PatientResponse`
- [x] Alterar `SearchPatientsHandler` para retornar coleção de `PatientResponse`
- [x] Ajustar testes afetados pelo novo contrato de saída

### Contratos de entrada

- [x] Criar `CreatePatientRequest`
- [x] Criar `UpdatePatientRequest`
- [x] Criar `SearchPatientsRequest`
- [x] Criar `SynchronizeExternalPatientRequest`
- [x] Criar `PatientRequestMappings`
- [x] Mapear `CreatePatientRequest -> CreatePatientCommand`
- [x] Mapear `UpdatePatientRequest -> UpdatePatientCommand`
- [x] Mapear `SearchPatientsRequest -> SearchPatientsQuery`
- [x] Mapear `SynchronizeExternalPatientRequest -> SynchronizeExternalPatientCommand`
- [x] Validar conversão do valor de `Gender`

### Integration Events públicos

- [x] Criar `PatientCreatedIntegrationEvent`
- [x] Criar `PatientUpdatedIntegrationEvent`
- [x] Manter os eventos com tipos públicos simples
- [x] Definir contratos agora e adiar publicação para a infraestrutura de eventos
- [x] Diferenciar Domain Events internos de Integration Events públicos

### Testes e validação

- [x] Criar testes de mappings dos Requests
- [x] Validar `GetPatientById` com `PatientResponse`
- [x] Validar `SearchPatients` com `PatientResponse`
- [x] Executar `dotnet build`
- [x] Executar `dotnet test`

### Resultado arquitetural

O módulo de Patients agora possui uma fronteira pública clara:

```text
Domain
  ↓
Application
  ↓
Contracts
  ↓
API / outros módulos / integrações
```

Entidades e Value Objects do Domain permanecem internos ao módulo.


---

# Fase 5 — Patients Infrastructure

## Status: ✅ 100% concluída

### Projeto e persistência

- [x] `Hospital.Patients.Infrastructure`
- [x] Entity Framework Core
- [x] EF Core Design
- [x] Npgsql
- [x] `PatientsDbContext`
- [x] `DbSet<Patient>`
- [x] `ApplyConfigurationsFromAssembly`

### Mapeamento EF Core

- [x] `PatientConfiguration`
- [x] Mapear `Patient` para tabela `patients`
- [x] Mapear `PatientId` para `Guid/uuid`
- [x] Usar `ValueGeneratedNever()` para o ID
- [x] Mapear `Name`
- [x] Mapear `BirthDate`
- [x] Mapear `Gender` como inteiro
- [x] Mapear `CreatedAtUtc`
- [x] Mapear `UpdatedAtUtc`
- [x] Mapear `ExternalPatientIdentifier` com `OwnsOne`
- [x] Mapear `SourceSystem`
- [x] Mapear `ExternalId`
- [x] Criar índice único `(SourceSystem, ExternalId)`
- [x] Nomear índice `ux_patients_external_identifier`
- [x] Permitir `ExternalIdentifier` nulo

### Repository

- [x] Implementar `IPatientRepository`
- [x] `GetByIdAsync`
- [x] `GetByExternalIdAsync`
- [x] `SearchAsync`
- [x] `AddAsync`
- [x] `UpdateAsync`
- [x] Utilizar tracking para comandos de atualização
- [x] Utilizar `AsNoTracking()` nas consultas de busca
- [x] Utilizar `EF.Functions.ILike` para busca por nome
- [x] Normalizar parâmetros externos com `Trim`

### Dependency Injection

- [x] Revisar `DependencyInjection.cs`
- [x] Registrar `PatientsDbContext`
- [x] Configurar `UseNpgsql`
- [x] Registrar `IPatientRepository -> PatientRepository`
- [x] Ler `ConnectionStrings:PatientsDatabase`

### Migrations

- [x] Criar `PatientsDbContextFactory`
- [x] Usar variável de ambiente `HOSPITAL_PATIENTS_CONNECTION_STRING` em design-time
- [x] Criar migration inicial `InitialPatients`
- [x] Gerar `PatientsDbContextModelSnapshot`

### Decisão de escopo

A Fase 5 encerra a implementação da camada de Infrastructure e a preparação do EF Core.

Os itens que dependem de um PostgreSQL real foram movidos para a Fase 7:

- [x] subir PostgreSQL com Docker;
- [x] aplicar `database update`;
- [x] validar tabela `patients`;
- [x] validar `__EFMigrationsHistory`;
- [x] executar testes de integração com PostgreSQL real.

Essa separação mantém a Fase 5 focada em código, persistência e configuração, enquanto a Fase 7 fica responsável pelo ambiente de banco e Docker.


---

# Fase 6 — Hospital.Api

## Status: 🟡 95% concluída — implementação validada; commit/push pendente

### Projeto e referências

- [x] Criar `Hospital.Api`
- [x] Adicionar à Solution
- [x] Referenciar `Hospital.Patients.Application`
- [x] Referenciar `Hospital.Patients.Contracts`
- [x] Referenciar `Hospital.Patients.Infrastructure`

### Program.cs / Composition Root

- [x] Configurar `Program.cs`
- [x] Registrar `AddPatientsInfrastructure`
- [x] Registrar handlers da Application
- [x] Configurar Swagger/OpenAPI
- [x] Configurar `AddProblemDetails`
- [x] Configurar `GlobalExceptionHandler`
- [x] Configurar `UseExceptionHandler`

### Endpoints de Patients

- [x] `POST /patients`
- [x] `GET /patients/{id}`
- [x] `GET /patients`
- [x] `PUT /patients/{id}`
- [x] `POST /patients/synchronize`

### Integração com Application / Contracts

- [x] Usar `CreatePatientRequest`
- [x] Usar `UpdatePatientRequest`
- [x] Usar `SynchronizeExternalPatientRequest`
- [x] Usar mappings `Request -> Command/Query`
- [x] Retornar `PatientResponse` em consultas
- [x] Manter entidades do Domain fora dos contratos HTTP

### Tratamento de erros HTTP

- [x] Criar `ResultExtensions`
- [x] Mapear `Patient.NotFound` para HTTP 404
- [x] Mapear `Patient.ExternalIdentifier.Invalid` para HTTP 400
- [x] Mapear `Patient.ExternalIdentifier.AlreadyExists` para HTTP 409
- [x] Criar tratamento global de exceções
- [x] Mapear `PatientDomainException` para HTTP 400
- [x] Mapear `ArgumentOutOfRangeException` para HTTP 400
- [x] Manter HTTP 500 para exceções inesperadas

### Swagger / documentação

- [x] Adicionar tags `Patients`
- [x] Adicionar nomes aos endpoints
- [x] Adicionar summaries
- [x] Adicionar descriptions
- [x] Documentar status codes principais com `Produces`

### Validação final

- [x] Executar a API localmente
- [x] Confirmar abertura do Swagger UI
- [x] Confirmar os 5 endpoints no Swagger
- [x] Executar `dotnet build`
- [x] Executar `dotnet test`
- [x] Revisar `git status`
- [x] Commit da Fase 6
- [x] Push para `origin/main`

### Observação de escopo

A API pode ser estruturada e documentada antes de o PostgreSQL real estar disponível.

A validação ponta a ponta com persistência real será realizada na Fase 7 — PostgreSQL + Docker.


---

# Fase 7 — PostgreSQL + Docker

## Status: 🟡 95% concluída — implementação e testes concluídos; fechamento pendente

### Docker / PostgreSQL

- [x] Criar `docker-compose.yml`
- [x] Adicionar serviço PostgreSQL
- [x] Usar imagem `postgres:17`
- [x] Configurar volume persistente
- [x] Configurar usuário e senha de desenvolvimento
- [x] Criar banco `hospital_intelligence`
- [x] Expor porta `5432`
- [x] Adicionar healthcheck
- [x] Subir container `hospital-postgres`
- [x] Confirmar container como `healthy`

### Integração com EF Core

- [x] Configurar `HOSPITAL_PATIENTS_CONNECTION_STRING`
- [x] Executar `dotnet ef database update`
- [x] Aplicar migration `20260823154226_InitialPatients`
- [x] Validar `__EFMigrationsHistory`
- [x] Validar tabela `patients`
- [x] Validar `PK_patients`
- [x] Validar índice único `ux_patients_external_identifier`

### Validação ponta a ponta da API

- [x] Executar `Hospital.Api` conectada ao PostgreSQL
- [x] Criar paciente via `POST /patients`
- [x] Consultar paciente via `GET /patients/{id}`
- [x] Pesquisar paciente via `GET /patients?name=...`
- [x] Atualizar paciente via `PUT /patients/{id}`
- [x] Sincronizar paciente via `POST /patients/synchronize`
- [x] Confirmar persistência real no PostgreSQL

### Testes de integração da Infrastructure

- [x] Criar `Hospital.Patients.IntegrationTests`
- [x] Adicionar projeto à Solution
- [x] Referenciar Infrastructure e Domain
- [x] Testar `AddAsync`
- [x] Testar `GetByIdAsync`
- [x] Testar `GetByExternalIdAsync`
- [x] Testar `SearchAsync`
- [x] Testar `UpdateAsync`
- [x] Validar constraint de identificação externa duplicada
- [x] Executar os testes contra PostgreSQL real

### Fechamento

- [x] Executar `dotnet build`
- [x] Executar `dotnet test`
- [x] Revisar `git status`
- [x] Commit da Fase 7
- [x] Push para `origin/main`

### Resultado da fase

Ao final da Fase 7, o módulo Patients possui persistência real validada de ponta a ponta:

```text
Swagger / HTTP
      ↓
Hospital.Api
      ↓
Patients.Application
      ↓
Patients.Infrastructure
      ↓
Entity Framework Core / Npgsql
      ↓
PostgreSQL em Docker
```


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

- [x] Criar `Hospital.Patients.IntegrationTests`
- [x] Testes com PostgreSQL real
- [x] Testes de `PatientRepository`
- [x] Persistência, busca, atualização e unicidade
- [x] Testes automatizados dos endpoints da API

## Architecture Tests

- [x] Criar testes que impeçam `Domain -> Infrastructure`
- [x] Impedir `Domain -> EF Core`
- [x] Impedir `Domain -> Salux`

---

# Fase 9 — Integration Core

## Status: ⬜ Não iniciada

- [x] Criar `Hospital.Integration.Core`
- [x] Criar contratos de integração
- [x] Criar modelo canônico
- [x] Criar abstrações de fontes externas
- [x] Criar checkpoints
- [x] Criar estratégia de idempotência
- [x] Criar logs e retry

---

# Fase 10 — Mock Hospital

## Status: ⬜ Não iniciada

- [x] Criar `Hospital.Integration.Mock`
- [x] Gerar pacientes fictícios
- [x] Gerar internações
- [x] Gerar exames
- [x] Gerar prescrições
- [x] Gerar sinais vitais
- [x] Gerar evoluções

---

# Fase 11 — Salux Connector

## Status: ⬜ Não iniciada

- [x] Criar `Hospital.Integration.Salux`
- [x] Configurar conexão read-only
- [x] Mapear pacientes
- [x] Mapear internações
- [x] Mapear exames
- [x] Mapear prescrições
- [x] Mapear evoluções
- [x] Implementar sincronização incremental
- [x] Implementar checkpoint
- [x] Implementar retry
- [x] Implementar idempotência

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

- [x] Dados cadastrais
- [x] Internações
- [x] Exames
- [x] Prescrições
- [x] Sinais vitais
- [x] Evoluções
- [x] Alertas
- [x] Timeline clínica
- [x] Resumo clínico

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

## Fechamento da Fase 7 — PostgreSQL + Docker

A implementação e os testes principais da Fase 7 estão concluídos.

Antes de avançar:

1. executar `dotnet build`;
2. executar `dotnet test`;
3. revisar `git status`;
4. confirmar que os arquivos pertencem às Fases 6 e 7;
5. fazer os commits pendentes;
6. fazer push para `origin/main`;
7. atualizar este roadmap para marcar a Fase 7 como ✅ 100%.

Após o fechamento, o próximo passo oficial será:

## Fase 8 — Testes

O foco pendente da Fase 8 será completar os testes automatizados da API e, em seguida, criar testes de arquitetura para proteger as dependências da Clean Architecture.
