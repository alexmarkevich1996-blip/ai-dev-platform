# Customer requirements

> Translated from the customer's Russian original. The Russian text is in the git history,
> at commit c44e9c3.

## The problem

We are an IT company with several teams and services. We keep our repositories on GitHub. Our developers already work with agents, but each one assembles their own instructions and configures their own tools. Useful practices stay inside individual teams.

A new hire asks colleagues which repositories they need, waits for access, and prepares a working environment. When someone leaves, we may forget to close some of their access. We find it hard to see what data the agents receive and how much the teams spend on models.

## Why we want a platform

We want to organise development around an AI-first approach. Under that approach, agents take part in the work from task definition through to release. People define the outcome, make the decisions, and check the work.

We need a platform that connects employees, projects, agents, and company rules. It should help give a person and their agent the information and access they need. We will start by connecting a developer to a project. Later we want to apply the platform to other stages of development and to the work of other employees.

## Where we want to start

For the first version we need employee accounts and invitation-based sign-in. We want to create projects, link them to repositories on GitHub, and store instructions for working. One project may group the repositories of several services.

An employee should get access to the whole project they are permitted, including all of its repositories through GitHub. They connect their own agent to the platform. The agent receives only the projects, repositories, and instructions available to that employee. Following those instructions, the agent prepares a working environment and checks access to the repositories it needs.

When an employee leaves, we must close their access to the platform and to the repositories. Their agent must stop receiving company data. An employee's rights must constrain their own access and their agent's access equally.

To connect external services we need a separate integrations module. The first integration will be GitHub. Later we can connect other services through that module, for example an issue tracker or a documentation store.

To begin with, supporting one developer agent of the team's choosing is enough. We leave the specific connection method, the interface, and the technical decisions to the team. It matters to us that the boundaries between parts of the application stay clear, so we can keep developing the platform. We will settle the details of the initial scenarios as the work goes on.

## Where we might take the platform

We are considering several directions, but we are not yet fixing their order or their full set.

**Teams and access.** We want to group employees into teams, account for different roles, and manage access to projects and tools.

**Shared tooling.** We are interested in shared skills, meaning reusable instructions for agents, in company tools, and in controlling model spend.

**A company agent.** Such an agent could run onboarding, meaning it would help a new hire find their way around projects and start working. It could answer from the documentation with links to sources, and say when it has no information. It could also help find suitable tasks. The answers and tasks it offers must respect the employee's rights.

**Tasks and cloud execution.** We could connect an external tracker such as Jira, or build our own. We want to try assigning tasks to agents, running them in the cloud, and handing work to several agents in parallel. For running code we are considering a sandbox, an isolated environment with restricted access.

**Quality, releases, and operations.** The platform could help review changes, evaluate how agents perform, prepare releases, and support services. Employee feedback on agent results could help improve how the agents work. We are also interested in error monitoring and recovery from failures.

These directions are open for discussion. We will pick the specific capabilities and priorities from our experience with the first version.
