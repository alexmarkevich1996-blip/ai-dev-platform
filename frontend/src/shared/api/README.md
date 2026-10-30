# shared/api

`schema.ts` is generated from the backend's OpenAPI document and is not edited by
hand. Regenerate it with `npm run generate:api` after changing an endpoint or a
request or response type; see AGENTS.md, "Regenerate the frontend API types".

Everything else here is the thin fetch wrapper the entity slices use.
