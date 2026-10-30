---
status: accepted
---

# Vite and React instead of Next.js

Next.js is the common default for a new TypeScript frontend, so a future reader will ask why it isn't here. We chose Vite with React instead, built as a static bundle that ASP.NET Core serves. The deciding constraint is an earlier decision: the backend runs as one process with two entry points, the HTTP API and the MCP route. Next.js in its normal mode brings its own Node server, which would make that a second backend and split session handling across two runtimes.

## Considered options

Next.js's server-rendering, server components, and server actions solve problems this interface doesn't have: it is a sign-in-gated set of 8-10 admin screens (forms and tables), with no SEO requirement and no public pages. Its static-export mode avoids the second server, but then none of those capabilities apply, leaving only the App Router's structure to learn without using. That structure also collides with Feature-Sliced Design's top layer, both of which claim an `app` directory at the project root, but for unrelated reasons.

Vite with React ships no server of its own. The dev server proxies API and MCP requests to the backend, and the production build is static files the backend serves directly — one origin in both environments, so no CORS configuration is needed either way.

## Consequences

- Routing, data fetching, and forms are assembled from separate libraries (React Router, TanStack Query, React Hook Form with Zod) rather than provided by the framework.
- There is exactly one server process carrying authentication and the governing access rule, matching the modular-monolith decision in ADR-0002.
- If a future direction needs server-rendering or a public marketing surface, that is a new decision, not a natural extension of this one.
