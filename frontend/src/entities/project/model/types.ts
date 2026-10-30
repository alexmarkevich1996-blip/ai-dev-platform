import type { Brand } from '@/shared/lib/brand'

/**
 * A platform unit standing for a product or a business task of the company.
 *
 * Only the identifier is declared here. The Project shape itself will come from
 * `components['schemas']` in `@/shared/api` once the Project endpoints exist;
 * writing it out now would be the hand-written type the generated ones replace.
 */
export type ProjectId = Brand<'ProjectId'>
