# ADR-0004 - Domain Events

## Status

Aceito.

## Contexto

Alguns acontecimentos do domínio precisam gerar efeitos em outros
componentes da aplicação.

Exemplo:

PatientCreatedDomainEvent

O domínio não deve depender diretamente de tecnologias como Kafka,
RabbitMQ ou Azure Service Bus.

## Decisão

Aggregate Roots poderão registrar Domain Events.

Os eventos serão armazenados no Aggregate Root e processados
posteriormente por camadas externas ao Domain.

## Consequências

Benefícios:

- domínio desacoplado;
- melhor extensibilidade;
- facilita comunicação entre módulos;
- prepara a solução para arquitetura orientada a eventos.

Riscos:

- necessidade futura de controle de consistência;
- possibilidade de duplicidade;
- provável uso de Outbox Pattern em integrações críticas.