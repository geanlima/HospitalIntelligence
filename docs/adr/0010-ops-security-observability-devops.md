# ADR-0010 — Fechamento operacional (Fases 19–22)

## Status

Aceito.

## Decisão

### Fase 19 — Command Center
`GET /command-center/summary` agrega Dashboard + previsões ML.
UI Angular em `/command-center`.

### Fase 20 — Segurança e LGPD
JWT (`/auth/login`), roles Admin/Clinician/Auditor,
policies com `Security:RequireAuth=true` (fallback autenticado),
audit trail **persistido** em Postgres (`security_audit_entries`),
anonimização LGPD, UI Angular de login + Bearer interceptor.
Testes de integração desligam `RequireAuth` via factory.

### Fase 21 — Observabilidade
Serilog estruturado, Correlation ID (`X-Correlation-ID`),
`ActivitySource`, Health Checks (`/health` + Postgres).

### Fase 22 — DevOps
Dockerfiles (API/Angular/ML), `docker-compose` completo,
GitHub Actions CI (build/test/.NET + Angular + scan NuGet).
