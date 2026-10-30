# First version of the platform

This document sets the boundary of the first version. It answers two questions: what we build now, and what we defer. The source requirements are in [customer-requirements.md](./customer-requirements.md). Terms are defined in [DOMAIN.md](../DOMAIN.md).

## Definition of done

The first version is done when two end-to-end scenarios run: **onboarding a developer into a project** and **offboarding an employee**. There are no other mandatory scenarios.

## The governing rule

An employee agent's rights equal its employee's rights. If a project is not open to the employee, the agent receives nothing about it.

## What the platform does not do

The platform **neither grants nor revokes GitHub permissions**. A person configures repository access in GitHub, outside the platform. The platform stores repository records and hands them to the employee and their agent.

This split matters. Project membership opens the records of every repository in the project. To read code, the employee needs live access in GitHub itself.

Managing GitHub permissions is moved into an extension, described in its own section below. See [ADR-0001](./adr/0001-platform-does-not-manage-github-access.md).

## Scenario 1: onboarding

1. The company administrator creates a project, adds repositories to it, and writes the instruction.
2. The administrator creates an email invitation and names the first set of projects in it.
3. The employee accepts the invitation and sets a password. Their account and memberships come into being at that moment.
4. The employee signs in and sees their projects.
5. The employee issues an agent token and connects their agent to the platform.
6. The agent requests its projects, their repository records, and their instructions.
7. The agent prepares the working environment from the instruction, then checks for itself that the code reads.

### Invitation rules

- An invitation is valid for 7 days, then expires on its own.
- The project list in an invitation may be empty.
- Before acceptance, the employee does not exist in the platform.
- The administrator changes project membership at any time after onboarding.

### Preparing the environment

The platform does not describe the steps for preparing an environment. The project instruction defines them. The platform only delivers the instruction to the agent.

The agent performs the code access check. It reaches the repository with the employee's credentials and reports the result to them. The platform does not run this check, because it does not manage GitHub permissions.

## Scenario 2: offboarding

The company administrator marks an employee as departed. Immediately after that:

- the employee can no longer sign in;
- all of their agent tokens stop working;
- their project memberships end;
- their agent no longer receives project information.

The account and its history are kept. GitHub permissions are left untouched: the extension closes those.

## Decisions for the first version

| Decision | Choice |
|---|---|
| Employee sign-in | Email and password, by invitation only |
| Roles | Two fixed: company administrator and employee |
| Configurable permissions | None |
| Companies per installation | One; "company" is not a model entity |
| Instruction | Markdown text in the platform; one per project |
| What the agent does | Reads only; changes nothing |
| Agent sign-in | An agent token issued by the employee |
| How the agent connects | An MCP server exposing read-only tools |
| Human interface | A minimal web interface |
| Code host | GitHub only, behind the integrations boundary |

### MCP server tools

The server returns only what the token's employee is permitted to see:

- the employee's projects;
- the repository records of a project;
- the instruction of a project.

### Web interface

The company administrator manages invitations, projects, repositories, instructions, membership, and offboarding. The employee sees their projects and manages their agent tokens. Visual design and extra screens are out of scope for the first version.

## Parts of the application

The customer asked us to keep the boundaries between parts clear. We define four parts:

| Part | Responsible for |
|---|---|
| **Accounts and access** | Invitations, sign-in, roles, agent tokens, offboarding |
| **Projects** | Projects, repositories, instructions, membership |
| **Integrations** | External services; an empty shell in the first version |
| **Agent data delivery** | The MCP server |

"Agent data delivery" stores nothing and decides nothing. It asks the first two parts and returns the answer to the agent. This makes the governing rule checkable in one place.

"Integrations" stays an empty shell in the first version. This is deliberate: the boundary is set in advance, so the extension does not reshape the application.

## Extension: managing GitHub access

This is separate work, after the first version. It adds:

- linking a GitHub account to a platform account;
- granting access to the repositories of permitted projects;
- revoking that access on offboarding.

The extension does not move the boundaries between parts. It fills the "Integrations" part.

## What we defer

None of the following is part of the first version:

- employee teams, and roles with configurable permissions;
- shared company skills, meaning reusable instructions for agents;
- a company agent;
- model spend tracking;
- issue trackers and documentation stores;
- running agents in the cloud, and sandboxing;
- change review, agent evaluation, releases, and operations;
- multiple companies in one installation;
- any write to platform data by an agent.
