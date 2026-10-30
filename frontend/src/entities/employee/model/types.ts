import type { components } from '@/shared/api'
import type { Brand } from '@/shared/lib/brand'

/** A person in the company who holds a platform account. */
export type EmployeeId = Brand<'EmployeeId'>

/** Who the browser is currently signed in as. Generated from the backend's contract. */
export type SignedInEmployee = components['schemas']['SessionResponse']

/** What the sign-in endpoint accepts. */
export type SignInCredentials = components['schemas']['SignInRequest']
