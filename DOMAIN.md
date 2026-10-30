# AI Dev Platform

The platform connects a company's employees, their projects, their agents, and its access rules. It gives a person and that person's agent the same permitted information.

Each headword carries the Russian term the team uses in conversation.

## Language

**Employee** (Сотрудник):
A person in the company who holds a platform account. Belonging to the company does not open every project to them.
_Avoid_: user, developer, account holder

**Invitation** (Приглашение):
A named permission to create an account, carrying the first set of project memberships. It is valid for a limited time.
_Avoid_: signup, registration, invite link, request

**Project** (Проект):
A platform unit standing for a product or a business task of the company. It groups repositories, members, their access, and an instruction.
_Avoid_: product, team, workspace, service

**Project Membership** (Участие в проекте):
The link between an employee and a project. It opens the records of every repository in the project, plus the project instruction.
_Avoid_: project access, enrolment, subscription

**Instruction** (Инструкция):
A project's Markdown text held in the platform. An employee agent follows it to prepare a working environment. A project has one instruction.
_Avoid_: documentation, rules, skill, prompt, guide

**Repository** (Репозиторий):
The source code store of one service in an external code host. A project groups one or more repositories.
_Avoid_: codebase, service, repo

**Repository Record** (Сведения о репозитории):
What the platform knows about a repository and hands out on project membership: address, purpose, and the link to the project. It grants no right to read code.
_Avoid_: metadata, repository description

**Repository Access** (Доступ к репозиторию):
An employee's live right to read code in the code host itself. An employee agent works within that right.
_Avoid_: repository permissions, code access

**Employee Agent** (Агент сотрудника):
A model-based development tool the employee already uses and connects to the platform. It works within its employee's rights.
_Avoid_: agent, bot, assistant, AI, LLM

**Company Agent** (Агент компании):
A shared platform agent serving the company's employees rather than one person. A direction for later; the first version has none.
_Avoid_: agent, shared agent, onboarding bot

**Agent Token** (Токен агента):
A secret the employee issues in the platform and hands to their agent. The platform uses it to tell whose request arrived.
_Avoid_: key, API key, agent password

**Company Administrator** (Администратор компании):
The employee role that may invite people, create projects, assign membership, and run offboarding.
_Avoid_: admin, owner, manager

**Integration** (Интеграция):
The platform's link to one external service through the integrations module. The first integration is GitHub.
_Avoid_: connector, plugin, provider

**Offboarding** (Офбординг):
Ending an employee's access to the platform. After it, that employee's agent stops receiving project information.
_Avoid_: termination, deactivation, removal
