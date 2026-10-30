# AGENTS.md

## Language

Write documentation in English: Markdown, code comments, commit messages, pull request descriptions, and issue text.

## Vocabulary

`DOMAIN.md` is the glossary. Name domain concepts with its terms, in code and in prose. Each headword pairs an English term with the Russian one the team speaks; keep both. A concept missing from the glossary is a signal: either you are inventing language the project does not use, or you found a real gap.

## Scope

`docs/first-version.md` sets the boundary: the two scenarios the first version must close, and what we defer. Read it before scoping or building. When a request lands on the deferred list, say so rather than building it.

`docs/customer-requirements.md` is the customer's source text, for when the reason behind a requirement matters.

`docs/technical-foundation.md` sets the stack and structure: backend modules, data, auth, frontend, local environment, and the test seam. Read it before writing code.

`docs/adr/` records hard-to-reverse decisions. Read the ones touching your area; flag it when your work contradicts one.

## Commands

`docs/technical-foundation.md` describes the structure. These are the four commands it does not carry, each with the part that is not visible from a config file.

### Run the platform locally

Copy `.env.example` to `.env` first. Compose reads it, and the database container refuses to start without a password.

```bash
cp .env.example .env
docker compose up --build
```

The frontend answers on port 5173 and proxies `/api` and `/mcp` to the backend on 8080.

### Run the tests

```bash
cd backend && dotnet test
```

Start Docker first: the suite runs against a real PostgreSQL 17 through Testcontainers.

Every test goes through one seam, `PlatformApplication`. Put a new test class in the existing xUnit collection, so the suite starts one container. Call `ResetAsync` when the test writes data.

### Regenerate the frontend API types

Building the backend rewrites `backend/openapi/Platform.Host.json`. Generate the TypeScript from that file, and commit both.

```bash
cd backend && dotnet build
cd ../frontend && npm run generate:api
```

### Add a database migration

Each module keeps its own migration history, so name that module's project and its context.

```bash
cd backend
dotnet tool restore
dotnet dotnet-ef migrations add <Name> \
  --project src/Platform.Accounts.Infrastructure \
  --startup-project src/Platform.Accounts.Infrastructure \
  --context AccountsDbContext \
  --output-dir Persistence/Migrations
```

For the Projects module, use `src/Platform.Projects.Infrastructure` and `ProjectsDbContext`.

## Issues

Issues live in this repo's GitHub Issues, driven by `gh`. Read `docs/agents/issue-tracker.md` before creating, linking, labelling, or closing one: it carries the exact commands, including sub-issues and blocking edges. Triage labels live in `docs/agents/triage-labels.md` and are used as-is.
