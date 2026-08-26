# ADR-0007 — Auditoria inteligente do prontuário (Fase 16)

## Status

Aceito.

## Contexto

Após a busca inteligente (Fase 15), o hospital precisa detectar
lacunas documentais, inconsistências clínicas e sinais de risco
de glosa — sem poluir o Domain clínico com regras de IA/auditoria.

Não existe ainda módulo de faturamento/contas; a “auditoria
financeira” nesta fase é **heurística de risco de glosa** baseada
em documentação clínica.

## Decisão

1. Use case `AuditPatientChartHandler` no módulo AI.
2. Motor determinístico `ChartAuditEngine` (sem LLM obrigatório):
   - `MissingDocumentation`
   - `Divergence`
   - `FinancialGlosaRisk`
3. Leitura via `IClinicalRecordSource` (Host), com `Status`/`SubType`.
4. Endpoint `POST /ai/audit/patients/{patientId}`.
5. UI Angular em `/ai/audit`.
6. Distinção: `IAiAuditStore` continua sendo trilha de interações LLM,
   não achados de prontuário.

## Consequências

Benefícios:

- achados reproduzíveis e testáveis;
- Domain clínico intacto;
- base para evolução com LLM narrativo e faturamento real.

Limitações:

- sem integração com contas/convênios ainda;
- regras iniciais são educacionais/operacionais, não normativas.
