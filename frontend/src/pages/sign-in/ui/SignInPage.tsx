import { Alert, Button, Card, Center, PasswordInput, Stack, TextInput, Title } from '@mantine/core'
import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'

import { useSignIn } from '@/entities/employee'

const credentialsSchema = z.object({
  email: z.email('Enter the email address the Invitation was sent to.'),
  password: z.string().min(1, 'Enter your password.'),
})

type Credentials = z.infer<typeof credentialsSchema>

export function SignInPage() {
  const signIn = useSignIn()

  const form = useForm<Credentials>({
    resolver: zodResolver(credentialsSchema),
    defaultValues: { email: '', password: '' },
  })

  const submit = form.handleSubmit((credentials) => {
    signIn.mutate(credentials)
  })

  return (
    <Center h="100%" p="md">
      <Card withBorder shadow="sm" w={380} padding="lg">
        <form onSubmit={submit}>
          <Stack>
            <Title order={2}>Sign in</Title>

            {signIn.isError && (
              <Alert color="red">That email address and password did not match.</Alert>
            )}

            <TextInput
              label="Email"
              autoComplete="username"
              error={form.formState.errors.email?.message}
              {...form.register('email')}
            />

            <PasswordInput
              label="Password"
              autoComplete="current-password"
              error={form.formState.errors.password?.message}
              {...form.register('password')}
            />

            <Button type="submit" loading={signIn.isPending}>
              Sign in
            </Button>
          </Stack>
        </form>
      </Card>
    </Center>
  )
}
