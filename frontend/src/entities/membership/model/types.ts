import type { Brand } from '@/shared/lib/brand'

/**
 * The link between an Employee and a Project.
 *
 * Only the identifier is declared here. The Membership shape will come from
 * `components['schemas']` once the Membership endpoints exist. When it does, it
 * will hold an Employee identifier and a Project identifier rather than embedded
 * objects: entity slices may not import each other, which mirrors the backend
 * decision that Projects refers to an Employee by identifier only.
 */
export type ProjectMembershipId = Brand<'ProjectMembershipId'>
