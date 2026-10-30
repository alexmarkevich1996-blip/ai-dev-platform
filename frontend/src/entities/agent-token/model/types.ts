import type { Brand } from '@/shared/lib/brand'

/** A secret the Employee issues and hands to their Employee Agent. */
export type AgentTokenId = Brand<'AgentTokenId'>

/**
 * What the Agent Tokens screen will list. The secret itself is never part of
 * this shape: the backend stores only a hash and shows the secret once, in the
 * response to issuing it.
 *
 * Agent Token endpoints arrive with the onboarding spec.
 */
export interface AgentToken {
  readonly id: AgentTokenId
  readonly name: string
  readonly issuedAt: string
  readonly revokedAt: string | null
}
