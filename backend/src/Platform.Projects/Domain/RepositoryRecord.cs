namespace Platform.Projects.Domain;

/// <summary>
/// What the platform knows about a Repository and hands out on Project Membership:
/// address, purpose, and the link to the Project.
/// </summary>
/// <remarks>
/// A Repository Record grants no right to read code. Repository Access lives in the
/// code host, and per ADR-0001 the platform neither grants nor revokes it.
/// </remarks>
public sealed class RepositoryRecord
{
    private RepositoryRecord(Guid id, ProjectId projectId, Uri address, string purpose, DateTimeOffset addedAt)
    {
        Id = id;
        ProjectId = projectId;
        Address = address;
        Purpose = purpose;
        AddedAt = addedAt;
    }

    private RepositoryRecord()
    {
        // Entity Framework Core materialises through this constructor.
        Address = new Uri("about:blank");
        Purpose = string.Empty;
    }

    public Guid Id { get; private set; }

    public ProjectId ProjectId { get; private set; }

    /// <summary>Where the Repository lives in the external code host.</summary>
    public Uri Address { get; private set; }

    public string Purpose { get; private set; }

    public DateTimeOffset AddedAt { get; private set; }

    internal static RepositoryRecord For(ProjectId projectId, Uri address, string purpose, DateTimeOffset addedAt) =>
        new(Guid.CreateVersion7(), projectId, address, purpose, addedAt);
}
