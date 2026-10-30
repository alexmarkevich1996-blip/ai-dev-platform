import type { Brand } from '@/shared/lib/brand'

/**
 * What the platform knows about a Repository and hands out on Project Membership.
 *
 * Only the identifier is declared here. The Repository Record shape will come
 * from `components['schemas']` once the Project endpoints exist. Whatever it
 * holds, it grants no Repository Access: the right to read the code lives in the
 * code host, and per ADR-0001 the platform does not manage it.
 */
export type RepositoryRecordId = Brand<'RepositoryRecordId'>
