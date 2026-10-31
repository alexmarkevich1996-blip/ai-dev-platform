# Clean local startup verification — 2026-10-31

The development stack started successfully from a clean checkout and an empty PostgreSQL volume. All three services became healthy, and ten HTTP checks passed. A database interruption exposed a recovery limitation: the first sign-in request after readiness recovered returned HTTP 500; repeating the checks passed without restarting the backend. This run verifies the runnable foundation, not the two completed product scenarios in [first-version.md](./first-version.md).

## Revision and environment

- Tested commit: `daa1b063f466954595522cb9d481d74034960010`, the head of merged [PR #4](https://github.com/alexmarkevich1996-blip/ai-dev-platform/pull/4), branch `3-technical-foundation`. The remote branch was checked with `git ls-remote` and matched this SHA.
- The report's working branch, `sardine`, started at `c4e57de4e96c18e444314f01d52aa2b94a535222`. That revision contains the startup instructions but no application source, Compose file, or `.env.example`; it cannot itself be launched. The implementation revision above was therefore checked out separately. No branch was merged or switched in the original workspace.
- Host: macOS, ARM64; Docker Desktop `4.90.0`, Docker Engine `29.7.2`, Docker Compose `v5.5.1`, context `desktop-linux`.
- Container versions observed: .NET SDK `10.0.403`, Node.js `v24.22.2`, Vite `8.3.5`, PostgreSQL `17.11` on ARM64.
- Fresh independent local clone: `/tmp/platform-startup-20261031.0TrEOB/source`, detached at the tested SHA, initially clean. No existing `.env`, dependencies, build output, or database contents were copied. `git status --porcelain --untracked-files=all` was also empty after the checks; `.env` is ignored.
- Dedicated Compose project: `platform-clean-20261031-0treob`; new network `platform-clean-20261031-0treob_default`; new volume `platform-clean-20261031-0treob_db-data`. Neither network nor volume existed before startup. Application image layers were rebuilt with `--no-cache`; the Docker daemon, downloaded base images, and host kernel were shared. This was not a fresh VM or Docker installation.
- Repository instructions followed: the tested revision's `AGENTS.md` and `docs/technical-foundation.md`. `.env.example` was copied unchanged to `.env`; its example password was used only in this isolated development stack.

## Commands

The existing `ai-dev-platform-db-1` already occupied port 5432. An override outside the checkout changed only host port bindings and bound them to loopback. Internal ports, DNS names (`db`, `backend`), connection strings, dependency ordering, and healthchecks remained those of the repository. Existing containers were left running.

```bash
mktemp -d /tmp/platform-startup-20261031.XXXXXX
# Returned /tmp/platform-startup-20261031.0TrEOB
git clone --no-hardlinks /Users/aleksandr/orca/workspaces/ai-dev-platform/sardine /tmp/platform-startup-20261031.0TrEOB/source
git -C /tmp/platform-startup-20261031.0TrEOB/source checkout --detach daa1b063f466954595522cb9d481d74034960010
git -C /tmp/platform-startup-20261031.0TrEOB/source status --short
cd /tmp/platform-startup-20261031.0TrEOB/source
cp .env.example .env
```

`../ports.yml` was written with Python `Path.write_text` and contained:

```yaml
services:
  db:
    ports: !override
      - "127.0.0.1:15432:5432"
  backend:
    ports: !override
      - "127.0.0.1:18080:8080"
  frontend:
    ports: !override
      - "127.0.0.1:15173:5173"
```

Build and startup commands (both exited 0):

```bash
docker compose -p platform-clean-20261031-0treob -f docker-compose.yml -f ../ports.yml build --no-cache > ../build.log 2>&1
docker compose -p platform-clean-20261031-0treob -f docker-compose.yml -f ../ports.yml up --build -d --wait --wait-timeout 240 > ../up.log 2>&1
python3 ../probe.py > ../http-checks.log 2>&1
docker compose -p platform-clean-20261031-0treob -f docker-compose.yml -f ../ports.yml ps
docker compose -p platform-clean-20261031-0treob -f docker-compose.yml -f ../ports.yml exec -T db psql -U platform -d platform -c "SELECT 'accounts' AS schema, migration_id, product_version FROM accounts.__migrations_history UNION ALL SELECT 'projects', migration_id, product_version FROM projects.__migrations_history;"
docker compose -p platform-clean-20261031-0treob -f docker-compose.yml -f ../ports.yml logs --no-color > ../services.log
```

The HTTP probe source is preserved below. It asserts status codes, the `Healthy` response, the frontend root element, and the served JavaScript entry module. Requests use a 45-second timeout. The invalid credentials and token in it are synthetic values, not real credentials.

## Observations

At `2026-10-31T16:39:02Z`, all ten initial HTTP checks passed:

| Check | Observed result |
|---|---|
| Backend `GET /health/live`, port 18080 | 200, `Healthy` |
| Backend `GET /health/ready`, port 18080 | 200, `Healthy`; readiness executes `SELECT 1` through Npgsql |
| Frontend `GET /`, port 15173 | 200, HTML containing the React root |
| Frontend `GET /src/main.tsx` | 200, transformed JavaScript containing `createRoot` |
| Backend `GET /api/session`, no cookie | 401 |
| Frontend `GET /api/session`, no cookie | 401 through the Vite API proxy |
| Frontend `POST /api/session`, well-formed credentials for an absent Employee (Сотрудник) | 401; backend logs show a query against `accounts.employees` |
| Frontend `POST /api/session`, invalid email and empty password | 400, `application/problem+json`, validation errors for both fields |
| Backend `POST /mcp`, initialize without an Agent Token (Токен агента) | 401, `WWW-Authenticate: Bearer` |
| Frontend `POST /mcp`, initialize with an unknown Agent Token | 401, `WWW-Authenticate: Bearer`; backend logs show a query against `accounts.agent_tokens` |

The proxy results and corresponding backend SQL logs establish the frontend → backend → database path. A 401 here proves rejection and routing, not a successful authenticated MCP handshake.

The new database contained both module migration histories:

```text
schema    migration_id                    product_version
accounts  20261030111240_InitialAccounts   10.0.14
projects  20261030105703_InitialProjects   10.0.14
```

The database log showed initial probes for missing `accounts.__migrations_history` and `projects.__migrations_history` tables before they were created. Startup completed, both histories were subsequently readable, and the backend reported that both schemas were up to date.

Other build/startup observations:

- `npm ci` completed and reported **3 moderate severity vulnerabilities**. No dependency changes or vulnerability investigation were performed.
- ASP.NET Core warned that Data Protection keys inside the container may be lost when the container is destroyed, and that `ASPNETCORE_URLS` overrides `HTTP_PORTS`. Neither prevented startup. Cookie survival across container replacement was not checked.

## Database interruption and recovery

Only this run's isolated database was stopped:

```bash
docker compose -p platform-clean-20261031-0treob -f docker-compose.yml -f ../ports.yml stop db > ../outage.log 2>&1
```

A Python `urllib.request.urlopen` probe of the two backend health URLs, catching `HTTPError` and asserting the statuses, observed at `2026-10-31T16:39:16Z`:

```text
live  200 Healthy
ready 503 Unhealthy
```

Recovery used:

```bash
docker compose -p platform-clean-20261031-0treob -f docker-compose.yml -f ../ports.yml up -d --wait --wait-timeout 120 > ../recovery.log 2>&1
python3 ../probe.py > ../http-checks-after-recovery.log 2>&1
python3 ../probe.py > ../http-checks-retry.log 2>&1
docker compose -p platform-clean-20261031-0treob -f docker-compose.yml -f ../ports.yml logs --no-color > ../services-after-recovery.log
```

Compose recovery exited 0 and all services became healthy. The first HTTP probe at `16:39:26Z` nevertheless exited 1: after successful health, frontend, and session GET checks, the well-formed sign-in POST returned **500** instead of 401. Its response and backend logs contained:

```text
System.InvalidOperationException: An exception has been raised that is likely due to a transient failure.
Npgsql.PostgresException: 57P01: terminating connection due to administrator command
```

The next complete probe at `16:39:39Z` passed all ten checks without restarting the backend. A stale EF Core pooled database connection after the stop is a plausible explanation, but was not independently proven. Readiness uses a separate Npgsql data source, so a recovered readiness check alone did not establish that the first application query would succeed. Seamless database recovery is therefore **not verified**; one transient application failure was observed.

At the final check, `16:39:57Z`, `db`, `backend`, and `frontend` were all healthy. Container inspection after recovery reported zero automatic restarts for each container. The database contained zero Employees and zero Agent Tokens.

## Not verified

- Launching the original `sardine` SHA: runnable files are absent there. The report applies only to the implementation SHA named above.
- Exact default host bindings 5432/8080/5173: loopback ports 15432/18080/15173 were used to keep this run separate from existing services.
- Browser rendering, interactive forms, and cookie authentication with a real Employee; only HTML/JavaScript delivery and HTTP behavior were checked. No Employee or valid Agent Token was seeded.
- Authenticated MCP initialization, tool discovery/calls, successful sign-in, onboarding, or Offboarding (Офбординг). The clean database has no credentials, and the tested revision is the technical foundation; complete first-version feature behavior is separate work.
- The automated `dotnet test` suite, production builds/deployment, non-Development migration behavior, and long-running stability. This was a development Compose startup and connectivity check.
- A host with no downloaded images, a different CPU architecture, or complete recovery guarantees after database outages.

## Environment retained for inspection

The isolated stack was left running. Frontend: <http://127.0.0.1:15173/>. Backend readiness: <http://127.0.0.1:18080/health/ready>. The checkout, override, probe, and raw logs are under `/tmp/platform-startup-20261031.0TrEOB/`; these local temporary files are not a durable artifact. This document preserves the commands, probe, observations, and limits in the project.

To remove only this run's containers, network, and test database volume when finished:

```bash
cd /tmp/platform-startup-20261031.0TrEOB/source
docker compose -p platform-clean-20261031-0treob -f docker-compose.yml -f ../ports.yml down --volumes
```

## HTTP probe source

```python
import json
import urllib.request
import urllib.error
from datetime import datetime, timezone

cases = [
    ('live', 'http://127.0.0.1:18080/health/live', 'GET', None, {}, 200),
    ('ready', 'http://127.0.0.1:18080/health/ready', 'GET', None, {}, 200),
    ('frontend', 'http://127.0.0.1:15173/', 'GET', None, {}, 200),
    ('vite module', 'http://127.0.0.1:15173/src/main.tsx', 'GET', None, {}, 200),
    ('direct session', 'http://127.0.0.1:18080/api/session', 'GET', None, {}, 401),
    ('proxied session', 'http://127.0.0.1:15173/api/session', 'GET', None, {}, 401),
    ('proxied sign-in, absent employee', 'http://127.0.0.1:15173/api/session', 'POST', {'email':'startup-check@example.invalid','password':'Startup-check-123!'}, {}, 401),
    ('proxied validation', 'http://127.0.0.1:15173/api/session', 'POST', {'email':'bad','password':''}, {}, 400),
    ('direct MCP', 'http://127.0.0.1:18080/mcp', 'POST', {'jsonrpc':'2.0','id':1,'method':'initialize','params':{'protocolVersion':'2025-03-26','capabilities':{},'clientInfo':{'name':'startup-check','version':'1.0'}}}, {'Accept':'application/json, text/event-stream'}, 401),
    ('proxied MCP, unknown token', 'http://127.0.0.1:15173/mcp', 'POST', {'jsonrpc':'2.0','id':1,'method':'initialize','params':{'protocolVersion':'2025-03-26','capabilities':{},'clientInfo':{'name':'startup-check','version':'1.0'}}}, {'Accept':'application/json, text/event-stream','Authorization':'Bearer startup-check-invalid-token'}, 401),
]
print(datetime.now(timezone.utc).isoformat())
for label, url, method, data, headers, expected in cases:
    if data is not None:
        headers['Content-Type'] = 'application/json'
    request = urllib.request.Request(url, data=None if data is None else json.dumps(data).encode(), headers=headers, method=method)
    try:
        response = urllib.request.urlopen(request, timeout=45)
    except urllib.error.HTTPError as error:
        response = error
    body = response.read().decode()
    print(json.dumps({'check': label, 'url':url, 'status':response.status, 'content_type':response.headers.get('Content-Type'), 'www_authenticate':response.headers.get('WWW-Authenticate'), 'body':body[:260]}))
    assert response.status == expected, (label, response.status, expected)
    if label in ('live', 'ready'):
        assert body == 'Healthy'
    if label == 'frontend':
        assert '<div id="root"></div>' in body
    if label == 'vite module':
        assert 'createRoot' in body
print('All 10 HTTP checks passed.')
```
