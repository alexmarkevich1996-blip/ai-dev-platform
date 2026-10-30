import type { Brand } from '@/shared/lib/brand'

/** A named permission to create an account, valid for a limited time. */
export type InvitationId = Brand<'InvitationId'>

/**
 * The shape the Invitations screens will read.
 *
 * Invitation endpoints arrive with the onboarding spec; from then on this type
 * comes from `components['schemas']` rather than from here.
 */
export interface Invitation {
  readonly id: InvitationId
  readonly email: string
  readonly expiresAt: string
  readonly acceptedAt: string | null
}
