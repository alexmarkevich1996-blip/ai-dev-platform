using Platform.Projects.Domain;

namespace Platform.Projects.Abstractions;

/// <summary>What a reader is told about a Project it may see.</summary>
public sealed record ProjectSummary(ProjectId Id, string Name, string Description);
