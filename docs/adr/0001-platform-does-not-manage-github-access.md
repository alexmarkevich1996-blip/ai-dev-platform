---
status: accepted
---

# The first version does not manage GitHub permissions

The customer named forgotten access as one of the founding problems, so the expected move was to let the platform grant and revoke GitHub permissions. We decided otherwise: in the first version the platform stores and serves **repository records**, while a person configures **repository access** in GitHub, outside the platform. This lets the first version close the end-to-end agent onboarding scenario without depending on a finished GitHub integration.

## Considered options

We considered putting permission granting into the first version. That makes the GitHub integration mandatory: without it the employee never gets the code, and onboarding does not complete. It ties the first version's delivery date to the integration's delivery date.

The chosen option breaks that tie. Permission management moves into a separate extension that fills the integrations module.

## Consequences

- Offboarding in the first version closes sign-in, agent tokens, and project memberships. It does not touch GitHub permissions. The original forgotten-access problem is only solved once the extension ships.
- The employee agent performs the code access check, not the platform. The platform does not manage the permissions and therefore cannot verify them.
- The integrations module stays an empty shell in the first version. This is deliberate: the boundary is set in advance so the extension does not reshape the application.
- `DOMAIN.md` separates "repository record" from "repository access". Conflating the two breaks this boundary immediately.
