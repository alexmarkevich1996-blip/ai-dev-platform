import type { Brand } from '@/shared/lib/brand'

/** A platform unit standing for a product or a business task of the company. */
export type ProjectId = Brand<'ProjectId'>

/**
 * The shape the Projects screens will read.
 *
 * Project endpoints arrive with the project specs; from then on this type comes
 * from `components['schemas']` rather than from here.
 */
export interface Project {
  readonly id: ProjectId
  readonly name: string
  readonly description: string
}
