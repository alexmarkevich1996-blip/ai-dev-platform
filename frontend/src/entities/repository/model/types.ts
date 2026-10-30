import type { Brand } from '@/shared/lib/brand'

/** What the platform knows about a Repository and hands out on Project Membership. */
export type RepositoryRecordId = Brand<'RepositoryRecordId'>

/**
 * A Repository Record grants no Repository Access: the right to read the code
 * lives in the code host, and per ADR-0001 the platform does not manage it.
 *
 * Repository Record endpoints arrive with the project specs.
 */
export interface RepositoryRecord {
  readonly id: RepositoryRecordId
  readonly projectId: string
  readonly address: string
  readonly purpose: string
}
