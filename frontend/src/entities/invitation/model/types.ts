import type { Brand } from '@/shared/lib/brand'

/**
 * A named permission to create an account, valid for a limited time.
 *
 * Only the identifier is declared here. The Invitation shape will come from
 * `components['schemas']` once the onboarding spec adds the endpoints.
 */
export type InvitationId = Brand<'InvitationId'>
