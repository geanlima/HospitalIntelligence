# ADR-0008 — Inteligência clínica de apoio (Fase 17)

## Status

Aceito.

## Contexto

A plataforma precisa apoiar decisões clínicas comuns
(alta segura, deterioração, medicação, triagem e nota por voz)
sem criar Domain acoplado a IA e sem ML Python (Fase 18).

## Decisão

1. Use case `AssessClinicalSafetyHandler` + motor `ClinicalSafetyEngine`:
   - Alta segura (checklist determinístico)
   - Deterioração NEWS2-lite a partir de vitais
   - Reconciliação medicamentosa textual
   - Copiloto de triagem (faixa de urgência)

2. `StructureVoiceNoteHandler` + prompt `voice-note-draft`:
   - “Prontuário por voz” sem hardware — cola transcrição / STT externo
   - gera rascunho para revisão humana

3. Endpoints:
   - `POST /ai/clinical-safety/patients/{patientId}`
   - `POST /ai/clinical-safety/voice-note`

4. UI Angular `/ai/clinical-safety`.

5. Domain clínico permanece livre de AI; leitura via `IClinicalRecordSource`.

## Consequências

- cobertura educativa dos 5 itens da Fase 17;
- NEWS2-lite e reconciliação são heurísticas, não produto clínico certificado;
- ML real de deterioração fica na Fase 18;
- STT/microfone nativo fica para evolução futura.
