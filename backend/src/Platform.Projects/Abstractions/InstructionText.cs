using Platform.Projects.Domain;

namespace Platform.Projects.Abstractions;

/// <summary>A Project's Instruction as a reader receives it.</summary>
public sealed record InstructionText(ProjectId ProjectId, string Markdown, DateTimeOffset UpdatedAt);
