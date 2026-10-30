import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import type { SignedInEmployee, SignInCredentials } from '@/entities/employee/model/types'
import { get, HttpError, post, remove } from '@/shared/api'

const sessionPath = '/api/session'
const sessionKey = ['session'] as const

/** The signed-in Employee, or null when there is no session. */
export function useSession() {
  return useQuery({
    queryKey: sessionKey,
    queryFn: async (): Promise<SignedInEmployee | null> => {
      try {
        return await get<SignedInEmployee>(sessionPath)
      } catch (error) {
        if (error instanceof HttpError && error.status === 401) {
          return null
        }
        throw error
      }
    },
  })
}

export function useSignIn() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (credentials: SignInCredentials) => post(sessionPath, { ...credentials }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: sessionKey }),
  })
}

export function useSignOut() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: () => remove(sessionPath),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: sessionKey }),
  })
}
