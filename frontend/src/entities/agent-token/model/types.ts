import type { Brand } from '@/shared/lib/brand'

/**
 * A secret the Employee issues and hands to their Employee Agent.
 *
 * Only the identifier is declared here. The Agent Token shape will come from
 * `components['schemas']` once the onboarding spec adds the endpoints. The
 * secret itself will never be part of it: the backend stores only a hash and
 * returns the secret once, in the answer to issuing it.
 */
export type AgentTokenId = Brand<'AgentTokenId'>
