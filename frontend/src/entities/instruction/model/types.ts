/**
 * A Project's Markdown text. An Employee Agent follows it to prepare a working
 * environment. A Project has one Instruction, so the Instruction is identified
 * by its Project rather than by an identifier of its own.
 *
 * Instruction endpoints arrive with the project specs.
 */
export interface Instruction {
  readonly projectId: string
  readonly markdown: string
  readonly updatedAt: string
}
