# Technical foundation

This document records the technical stack and structure for the first version described in [first-version.md](./first-version.md). It is the implementation counterpart to that document: first-version.md sets what must work, this sets what it runs on. Terms follow [DOMAIN.md](../DOMAIN.md). See [ADR-0002](./adr/0002-modular-monolith-one-project-and-schema-per-module.md) and [ADR-0003](./adr/0003-vite-react-over-nextjs.md) for the two decisions here that are hard to reverse.

The runnable skeleton this document describes is tracked as [issue #3](https://github.com/alexmarkevich1996-blip/ai-dev-platform/issues/3).

## Stack

| Layer | Choice |
|---|---|
| Backend | C#, ASP.NET Core |
| Frontend | TypeScript, React, Vite |
| Database | PostgreSQL 17 |
| Repository | Monorepo |
| Backend architecture | Modular monolith, one process, two entry points (HTTP API, MCP route) |

## Backend modules

Four product modules, mapped one-to-one onto the four parts named in `docs/first-version.md`, plus a composition root and a shared kernel:

| Project | Responsibility | May reference |
|---|---|---|
| `Platform.Host` | Composition root, HTTP API, MCP route, health endpoints | All modules |
| `Platform.SharedKernel` | Result type, base value objects | — |
| `Platform.Accounts` | Invitations, sign-in, roles, Agent Tokens, Offboarding | `SharedKernel` |
| `Platform.Accounts.Infrastructure` | EF Core mapping and repositories for Accounts | `Accounts`, `SharedKernel` |
| `Platform.Projects` | Projects, Repository Records, Instructions, Project Membership | `SharedKernel` |
| `Platform.Projects.Infrastructure` | EF Core mapping and repositories for Projects | `Projects`, `SharedKernel` |
| `Platform.Integrations` | External services; an empty shell in the first version | `SharedKernel` |
| `Platform.AgentDelivery` | The MCP server; holds no storage of its own | `Accounts`, `Projects`, `SharedKernel` |

`Integrations` and `AgentDelivery` have no `Infrastructure` project: the first has no storage yet, the second never will.

Inside each module, code is organised as vertical slices — one folder per use case holding its command, handler, and validator — following the Clean Architecture dependency rule between a module and its `Infrastructure` project.

`AgentDelivery` stores nothing and decides nothing. It resolves an Agent Token to an Employee, asks `Accounts` and `Projects` what that Employee may see, and returns the answer. This is the one place the governing rule from `docs/first-version.md` — an Employee Agent's rights equal its Employee's rights — is checked.

## Data

One PostgreSQL database, one schema per module (`accounts`, `projects`), one `DbContext` and migration history per module. Cross-module joins are impossible by construction; `Projects` references an Employee by identifier only, never by foreign key into `accounts`.

Migrations apply automatically at startup in the Development environment only. A separate migrator service is deferred as operations work.

## Authentication

Two separate mechanisms:

- **Browser**: ASP.NET Core Identity with cookie authentication. Self-registration is disabled — an account exists only after an Invitation is accepted.
- **Employee Agent**: an opaque Agent Token, stored only as a hash. An Employee issues and revokes several tokens; Offboarding kills all of them in one operation.

## MCP contract

Exposed over HTTP on a route of the same host (not a separate process, not stdio). Tools are read-only and scoped to the token's Employee:

- the Employee's Projects
- the Repository Records of a Project
- the Instruction of a Project

## Invitations

An invitation-sender interface is defined; the development implementation writes the Invitation link to the application log instead of sending email. An Invitation is valid for 7 days.

## Frontend

Feature-Sliced Design with four layers — `app`, `pages`, `entities`, `shared` — omitting `widgets` and `features` until something earns them. Entity slices are named after `DOMAIN.md` terms (employee, project, invitation, agent token, repository, instruction, membership).

Libraries: React Router, TanStack Query, React Hook Form with Zod, Mantine for components. TypeScript types are generated from the backend's OpenAPI document rather than hand-written.

The dev server proxies API and MCP requests to the backend; in production the backend serves the built static files. Both environments run on a single origin, so no CORS configuration is needed.

## Conventions carried over from prior work

`Result<T, ErrorList>` (CSharpFunctionalExtensions), Scrutor for DI registration, FluentValidation, central package management, `TreatWarningsAsErrors` with the Roslynator, Meziantou, Sonar, and AsyncFixer analyzers, and Serilog writing to the console. Seq is deferred as observability work.

## Local environment

Docker Compose, three services:

| Service | Readiness |
|---|---|
| `db` | `pg_isready`, with a data volume |
| `backend` | `/health/ready`, starts after `db` is healthy |
| `frontend` | dev server port, starts after `backend` |

Two health endpoints: `/health/live` reports the process is running; `/health/ready` reports the database connection answers (Npgsql check).

Configuration is environment variables. `.env` is ignored by Git; `.env.example` is committed and lists every variable: database user, password, name, the connection string, and the ASP.NET Core environment.

## Testing

One seam for the whole codebase: the composed application over HTTP, backed by a real PostgreSQL via Testcontainers (`WebApplicationFactory` around `Platform.Host`). No module is tested in isolation and no module is mocked — the governing rule only holds in composition, so a per-module seam could pass in isolation and fail in composition.

## What this foundation does not include

- Any first-version feature behaviour (Invitations, Projects, Instructions, Membership, Agent Tokens, Offboarding) — those are separate specs built on this foundation.
- Managing Repository Access in GitHub (ADR-0001); `Integrations` stays an empty shell.
- CI, production container images, deployment manifests, monitoring, log aggregation, rate limiting, backups.
- Real mail (SMTP, a mail-catcher container).
- Visual design beyond Mantine's defaults.
- Everything on the deferred list in `docs/first-version.md`.
