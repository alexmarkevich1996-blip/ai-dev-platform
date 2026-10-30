import { Center, Loader } from '@mantine/core'
import { Navigate, Route, Routes } from 'react-router'

import { useSession } from '@/entities/employee'
import { ProjectsPage } from '@/pages/projects'
import { SignInPage } from '@/pages/sign-in'

/**
 * Every screen is behind sign-in. There is no registration route, because an
 * account comes into being only when an Invitation is accepted.
 */
export function AppRoutes() {
  const session = useSession()

  if (session.isPending) {
    return (
      <Center h="100%">
        <Loader />
      </Center>
    )
  }

  const signedIn = session.data != null

  return (
    <Routes>
      <Route path="/sign-in" element={signedIn ? <Navigate to="/" replace /> : <SignInPage />} />
      <Route
        path="/"
        element={signedIn ? <ProjectsPage /> : <Navigate to="/sign-in" replace />}
      />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
