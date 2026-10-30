import { Button, Container, Group, Stack, Text, Title } from '@mantine/core'

import { useSession, useSignOut } from '@/entities/employee'

/**
 * The first screen after sign-in. It lists nothing yet: creating Projects and
 * assigning Project Membership are separate specs built on this foundation.
 */
export function ProjectsPage() {
  const session = useSession()
  const signOut = useSignOut()

  return (
    <Container size="md" py="xl">
      <Stack>
        <Group justify="space-between">
          <Title order={2}>Projects</Title>
          <Button variant="subtle" onClick={() => signOut.mutate()} loading={signOut.isPending}>
            Sign out
          </Button>
        </Group>

        <Text c="dimmed">
          Signed in as {session.data?.fullName ?? session.data?.email ?? 'an Employee'}.
        </Text>

        <Text>
          No Projects yet. Creating Projects, writing Instructions, and assigning Project
          Membership arrive with the project specs.
        </Text>
      </Stack>
    </Container>
  )
}
