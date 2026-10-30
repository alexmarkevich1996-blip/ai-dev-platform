---
status: accepted
---

# Modular monolith with one project and one schema per module

The author's prior projects split a solution by technical layer (Domain, Infrastructure, Web). We decided differently here: one project per domain module (`Accounts`, `Projects`, `Integrations`, `AgentDelivery`), with layering expressed inside each module as vertical slices, and one database schema per module. This makes the governing rule — an Employee Agent's rights equal its Employee's rights — enforceable by the compiler and by the schema boundary, not just by convention.

## Considered options

We considered keeping the familiar layer-per-project split (Domain / Core / Infrastructure / Web), since that is the author's established working pattern and already produces clean, analyzer-checked code. Under that split, the four product modules named in `docs/first-version.md` would exist only as folders inside a shared Domain project. A module reaching into another module's internals would be a convention violation, not a build failure.

The chosen option inverts the axis: modules are projects, layers are folders within them. `Platform.AgentDelivery` can reference `Platform.Accounts` and `Platform.Projects` only through their public surface; it cannot reference each other's `Infrastructure` project or read another module's schema directly, because project references and schema ownership both forbid it.

## Consequences

- A forbidden module dependency (e.g. `Accounts` referencing `AgentDelivery`) fails the build instead of surfacing in review.
- Each module owns its own `DbContext` and migration history. Cross-module joins are impossible; `Projects` stores an Employee identifier rather than a foreign key into `accounts`.
- `Integrations` and `AgentDelivery` get no `Infrastructure` project, since neither owns storage. Adding one for symmetry was rejected as dead weight.
- This departs from the author's default layering habit. Vertical slices and the Clean Architecture dependency rule still apply, but inside the module boundary rather than across top-level projects.
