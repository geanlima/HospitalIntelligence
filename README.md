# Hospital Intelligence Platform

Hospital Intelligence Platform é uma plataforma modular para integração, consolidação e análise inteligente de dados hospitalares.

O projeto tem dois objetivos principais:

1. Construir uma plataforma hospitalar moderna e extensível.
2. Servir como projeto prático de estudo de arquitetura de software, integração de sistemas, inteligência artificial, DevOps e desenvolvimento profissional com .NET.

## Tecnologias iniciais

* .NET 10
* C#
* Entity Framework Core
* PostgreSQL
* Docker
* Angular
* Git
* xUnit

Tecnologias adicionais serão incorporadas conforme a evolução do projeto.

## Arquitetura

A solução utiliza inicialmente:

* Clean Architecture
* Domain-Driven Design
* Modular Monolith
* Dependency Inversion
* Anti-Corruption Layer
* Event Driven Architecture, quando necessário

Estrutura inicial:

```text
src/
├── BuildingBlocks/
├── Modules/
├── Integrations/
└── Hosts/

tests/

docs/
```

## Building Blocks

Contém elementos reutilizáveis e independentes de módulos específicos.

Exemplo:

```text
Hospital.SharedKernel
```

Responsabilidades planejadas:

* Entity
* AggregateRoot
* DomainEvent
* ValueObject
* Result
* Error

## Modules

Cada módulo representa um contexto de negócio.

Primeiro módulo:

```text
Patients
```

Estrutura:

```text
Hospital.Patients.Domain
Hospital.Patients.Application
Hospital.Patients.Contracts
Hospital.Patients.Infrastructure
```

## Regra de dependência

As dependências devem apontar para as camadas internas:

```text
Infrastructure
      ↓
Application
      ↓
Domain
      ↓
SharedKernel
```

O Domain não deve depender diretamente de:

* Entity Framework
* PostgreSQL
* HTTP
* Angular
* Salux
* FHIR
* OpenAI
* Redis

## Integração com Sistemas Hospitalares

A plataforma deverá integrar dados provenientes de diferentes sistemas.

Exemplos:

* Salux
* FHIR
* outros HIS/ERP
* APIs externas
* bancos de dados relacionais

Cada sistema deverá possuir um adaptador próprio.

Fluxo planejado:

```text
Sistema Hospitalar
        ↓
Adapter
        ↓
Anti-Corruption Layer
        ↓
Canonical Model
        ↓
Hospital Intelligence Platform
```

## Objetivos futuros

Entre os principais módulos planejados estão:

* Patient 360
* Busca Inteligente no Prontuário
* Alta Segura IA
* Deterioração do Paciente
* Reconciliação Medicamentosa
* Auditoria de Prontuário
* Auditoria Financeira
* Previsão de No-show
* Prontuário por Voz
* Hospital Command Center

## Documentação

A documentação técnica fica na pasta:

```text
docs/
```

Estrutura:

```text
docs/
├── architecture/
├── adr/
├── integration/
├── database/
└── ai/
```

Decisões arquiteturais importantes serão registradas utilizando ADRs — Architecture Decision Records.

## Desenvolvimento

Para restaurar dependências:

```bash
dotnet restore
```

Para compilar:

```bash
dotnet build
```

Para executar testes:

```bash
dotnet test
```

## SDK

O SDK utilizado pelo projeto é controlado pelo arquivo:

```text
global.json
```

## Status

Projeto em desenvolvimento.

A evolução será realizada incrementalmente, mantendo documentação, testes e decisões arquiteturais junto ao código.
