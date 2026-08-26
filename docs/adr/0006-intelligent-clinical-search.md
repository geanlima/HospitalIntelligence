# ADR-0006 — Busca inteligente no prontuário (Fase 15)

## Status

Aceito.

## Contexto

A Fase 14 entregou a fundação de IA (RAG, pgvector, LLM plugável)
com knowledge mock. A Fase 15 precisa usar **dados clínicos reais**
do monólito, com evidências, citações e controle de acesso básico.

## Decisão

1. Indexação sob demanda por paciente:
   - `POST /ai/index/patients/{patientId}`
   - Handler `IndexPatientClinicalRecordsHandler`
   - Porta `IClinicalRecordSource` (implementada no **Host**)

2. Busca semântica (sem LLM):
   - `POST /ai/search`
   - Retorna hits com `SourceId`, título, excerpt e score

3. Q&A em linguagem natural com evidências:
   - Reusa `POST /ai/ask` com `PromptKey = clinical-chart-qa`
   - Exige `PatientId` + política de acesso
   - Citações já existentes em `AskAiResponse.Sources`

4. Controle de acesso (Fase 15):
   - `IAiAccessPolicy` / `PatientScopedAiAccessPolicy`
   - Exige paciente existente; filtra vector search por `PatientId`
   - AuthN/AuthZ real permanece na Fase 20

5. Domain clínico **não** referencia o módulo AI.
   Composição de repositórios fica no Host.

## Consequências

Benefícios:

- RAG sobre prontuário real sem acoplar Domain à IA;
- evidências auditáveis;
- escopo por paciente desde o início.

Limitações:

- embedding ainda determinístico;
- indexação sob demanda (UI/API); reindexação event-driven fica para evolução futura;
- sem papéis/perfis clínicos ainda (Fase 20).

UI: `src/Frontend/hospital-web` rota `/ai/search`.
