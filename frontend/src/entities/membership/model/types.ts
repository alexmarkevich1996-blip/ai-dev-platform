import type { Brand } from '@/shared/lib/brand'

/** The link between an Employee and a Project. */
export type ProjectMembershipId = Brand<'ProjectMembershipId'>

/**
 * Membership holds identifiers, not embedded Employee and Project objects.
 *
 * Entity slices may not import each other, which mirrors the backend decision
 * that Projects refers to an Employee by identifier only.
 */
export interface ProjectMembership {
  readonly id: ProjectMembershipId
  readonly employeeId: string
  readonly projectId: string
  readonly startedAt: string
  readonly endedAt: string | null
}
