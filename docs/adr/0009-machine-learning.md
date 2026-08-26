# ADR-0009 — Machine Learning de estudo (Fase 18)

## Status

Aceito.

## Decisão

1. Módulo `Hospital.ML` (.NET) com feature engineering no Host
   e previsões versionadas (no-show, alta, deterioração).
2. Serviço Python FastAPI em `services/ml` (Docker) espelhando
   as mesmas heurísticas para trilha de estudo.
3. Registry com métricas, versão e notas de drift.
4. ML real/treinamento contínuo pode evoluir depois sem quebrar
   a porta `IMlPredictionService`.
