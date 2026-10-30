# AGENTS.md

## Language

Write documentation in English: Markdown, code comments, commit messages, pull request descriptions, and issue text.

## Vocabulary

`DOMAIN.md` is the glossary. Name domain concepts with its terms, in code and in prose. Each headword pairs an English term with the Russian one the team speaks; keep both. A concept missing from the glossary is a signal: either you are inventing language the project does not use, or you found a real gap.

## Scope

`docs/first-version.md` sets the boundary: the two scenarios the first version must close, and what we defer. Read it before scoping or building. When a request lands on the deferred list, say so rather than building it.

`docs/customer-requirements.md` is the customer's source text, for when the reason behind a requirement matters.

`docs/adr/` records hard-to-reverse decisions. Read the ones touching your area; flag it when your work contradicts one.

## Issues

Issues live in this repo's GitHub Issues, driven by `gh`. Read `docs/agents/issue-tracker.md` before creating, linking, labelling, or closing one: it carries the exact commands, including sub-issues and blocking edges. Triage labels live in `docs/agents/triage-labels.md` and are used as-is.
