# Integração Salux (read-only)

A plataforma **não escreve** no Salux. O adapter lê views/tabelas e sincroniza para o Postgres local.

## Fluxo

```text
Salux (SqlServer/Oracle)
  → SqlSaluxPatientReader
  → SaluxPatientMapper (ACL)
  → IntegrationMessage
  → SaluxPatientMessageHandler
  → SynchronizeExternalPatient (upsert por SALUX + PatientCode)
```

## Configuração local (sem Docker)

Em `appsettings.Development.json` ou User Secrets:

```json
"Salux": {
  "Enabled": true,
  "Provider": "SqlServer",
  "ConnectionString": "Server=SALUX-DB;Database=Salux;User Id=hi_readonly;Password=***;TrustServerCertificate=True",
  "PollIntervalSeconds": 60,
  "BatchSize": 200,
  "PatientQuery": "SELECT TOP (@BatchSize) ..."
}
```

### Oracle

```json
"Provider": "Oracle",
"ConnectionString": "User Id=hi_readonly;Password=***;Data Source=SALUX",
"PatientQuery": "SELECT ... FROM VW_HI_PACIENTES WHERE DAT_ALTERACAO > :Checkpoint FETCH FIRST :BatchSize ROWS ONLY"
```

## Docker (subir em outro local)

Arquivos:

| Arquivo | Função |
|---|---|
| `docker-compose.yml` | Stack completa (postgres, api+Salux worker, web, ml) |
| `.env.example` | Modelo de variáveis — copie para `.env` |
| `deploy/salux-patient-query.sql` | Query SQL montada no container |

### Passo a passo no servidor

```bash
cp .env.example .env
# edite .env: SALUX_ENABLED=true + SALUX_CONNECTION_STRING + JWT_SIGNING_KEY
# edite deploy/salux-patient-query.sql com a view real do hospital

docker compose up -d --build
```

Serviços:

- API (+ worker Salux): `http://HOST:8080`
- Web: `http://HOST:8088`
- Postgres: porta `5432` (interna `postgres`)
- Sync manual: `POST http://HOST:8080/integrations/salux/patients/sync`

### Rede até o Salux

O container `hospital-api` precisa alcançar o banco Salux:

- Salux no **mesmo host** do Docker: use `host.docker.internal` na connection string  
  (`extra_hosts` já está no compose)
- Salux na **rede do hospital**: IP/hostname resolvível a partir do servidor Docker + firewall liberando a API

Exemplo `.env`:

```env
SALUX_ENABLED=true
SALUX_PROVIDER=SqlServer
SALUX_CONNECTION_STRING=Server=host.docker.internal,1433;Database=Salux;User Id=hi_readonly;Password=***;TrustServerCertificate=True
PUBLIC_API_URL=http://IP-DO-SERVIDOR:8080
```

## Contrato mínimo da query

Colunas obrigatórias:

| Coluna | Tipo |
|---|---|
| `PatientCode` | string (ID Salux) |
| `PatientName` | string |
| `BirthDate` | date |
| `GenderCode` | int (`0` Unknown, `1` Male, `2` Female, `3` Other) |
| `UpdatedAtUtc` | datetime / datetimeoffset |

Parâmetros: `@Checkpoint` + `@BatchSize` (SqlServer) ou `:Checkpoint` + `:BatchSize` (Oracle).

> Ajuste nomes de view/colunas com a TI do hospital. O SQL em `deploy/salux-patient-query.sql` é um **template** (`VW_HI_PACIENTES`).

## Operação

1. Liberar rede/VPN + usuário **somente leitura**
2. Criar/liberar a view alinhada ao contrato
3. Preencher connection string (`.env` ou appsettings)
4. `SALUX_ENABLED=true` / `Salux:Enabled=true`
5. Subir stack — worker roda dentro do container `hospital-api`
6. Sync manual: `POST /integrations/salux/patients/sync` (JWT Admin/Clinician)

Checkpoint e idempotência ficam no Postgres da plataforma (`salux_sync_checkpoints`, `salux_idempotency`).
