# Frontend structure

Feature-Sliced Design with four layers. `widgets` and `features` are deliberately
absent until something earns them.

| Layer | Holds | May import from |
| --- | --- | --- |
| `app` | Providers, routing, the composition of the whole application | every layer below |
| `pages` | One screen each | `entities`, `shared` |
| `entities` | One slice per `DOMAIN.md` term | `shared` only |
| `shared` | The API client, generated types, small helpers | — |

**Entity slices may not import each other.** A slice that refers to another entity
holds a plain identifier, never an embedded object — `membership` carries
`employeeId` and `projectId`, not an Employee and a Project. This mirrors the
backend decision that Projects stores an Employee identifier and no foreign key
into the `accounts` schema.

Slices are named after `DOMAIN.md`: `employee`, `project`, `invitation`,
`agent-token`, `repository`, `instruction`, `membership`. Most hold only their
types so far; their API calls and components arrive with the feature specs.
