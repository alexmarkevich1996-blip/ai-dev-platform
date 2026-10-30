import type { Brand } from '@/shared/lib/brand'

/**
 * A Project's Markdown text. An Employee Agent follows it to prepare a working
 * environment. A Project has one Instruction, so the Instruction is identified by
 * its Project rather than by an identifier of its own.
 *
 * The Instruction shape will come from `components['schemas']` once the Project
 * endpoints exist.
 */
export type InstructionProjectId = Brand<'ProjectId'>
