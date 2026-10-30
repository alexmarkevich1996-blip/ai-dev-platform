import { MantineProvider } from '@mantine/core'
import { Notifications } from '@mantine/notifications'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import type { ReactNode } from 'react'
import { BrowserRouter } from 'react-router'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      // The session and the lists it guards change on another person's action,
      // not on this tab's, so refetching on focus is the honest default.
      retry: false,
      staleTime: 30_000,
    },
  },
})

/** Everything the whole application needs in scope, in one place. */
export function AppProviders({ children }: { children: ReactNode }) {
  return (
    <MantineProvider defaultColorScheme="auto">
      <Notifications />
      <QueryClientProvider client={queryClient}>
        <BrowserRouter>{children}</BrowserRouter>
      </QueryClientProvider>
    </MantineProvider>
  )
}
