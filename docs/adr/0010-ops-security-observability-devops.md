# ADR-0010 — Fechamento operacional (Fases 19–22)

## Status

Aceito.

## Decisão

### Fase 19 — Command Center
`GET /command-center/summary` agrega Dashboard + previsões ML.
UI Angular em `/command-center`.

### Fase 20 — Segurança e LGPD
JWT de estudo (`/auth/login`), roles Admin/Clinician/Auditor,
policies, audit trail em memória, anonimização LGPD.
`Security:RequireAuth` permanece `false` por padrão para não
quebrar o front de estudo.

### Fase 21 — Observabilidade
Serilog estruturado, Correlation ID (`X-Correlation-ID`),
`ActivitySource`, Health Checks (`/health` + Postgres).

### Fase 22 — DevOps
Dockerfiles (API/Angular/ML), `docker-compose` completo,
GitHub Actions CI (build/test/.NET + Angular + scan NuGet).
