namespace Platform.Projects.Domain;

/// <summary>
/// A Project's Markdown text. An Employee Agent follows it to prepare a working
/// environment. A Project has one Instruction.
/// </summary>
public sealed class Instruction
{
    private Instruction(ProjectId projectId, string markdown, DateTimeOffset updatedAt)
    {
        ProjectId = projectId;
        Markdown = markdown;
        UpdatedAt = updatedAt;
    }

    private Instruction()
    {
        // Entity Framework Core materialises through this constructor.
        Markdown = string.Empty;
    }

    /// <summary>The Instruction shares its Project's identifier: there is exactly one per Project.</summary>
    public ProjectId ProjectId { get; private set; }

    public string Markdown { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    internal static Instruction For(ProjectId projectId, string markdown, DateTimeOffset updatedAt) =>
        new(projectId, markdown, updatedAt);

    internal void Rewrite(string markdown, DateTimeOffset updatedAt)
    {
        Markdown = markdown;
        UpdatedAt = updatedAt;
    }
}
