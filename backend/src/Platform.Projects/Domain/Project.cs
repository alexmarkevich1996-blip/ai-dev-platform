namespace Platform.Projects.Domain;

/// <summary>
/// A platform unit standing for a product or a business task of the company.
/// </summary>
/// <remarks>
/// The Project is the aggregate root over its Repository Records and its one
/// Instruction: neither exists without a Project, so neither is created directly.
/// Project Membership is a separate root, because it names an Employee and the two
/// are managed on different occasions.
/// </remarks>
public sealed class Project
{
    private readonly List<RepositoryRecord> _repositoryRecords = [];

    private Project(ProjectId id, string name, string description, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
    }

    private Project()
    {
        // Entity Framework Core materialises through this constructor.
        Name = string.Empty;
        Description = string.Empty;
    }

    public ProjectId Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>A Project has one Instruction, or none until someone writes it.</summary>
    public Instruction? Instruction { get; private set; }

    public IReadOnlyList<RepositoryRecord> RepositoryRecords => _repositoryRecords;

    public static Project Create(string name, string description, DateTimeOffset createdAt) =>
        new(ProjectId.New(), name, description, createdAt);

    public RepositoryRecord AddRepositoryRecord(Uri address, string purpose, DateTimeOffset addedAt)
    {
        var record = RepositoryRecord.For(Id, address, purpose, addedAt);
        _repositoryRecords.Add(record);
        return record;
    }

    /// <summary>Writes the Project's Instruction, replacing the previous text.</summary>
    public void SetInstruction(string markdown, DateTimeOffset updatedAt)
    {
        if (Instruction is null)
        {
            Instruction = Instruction.For(Id, markdown, updatedAt);
            return;
        }

        Instruction.Rewrite(markdown, updatedAt);
    }
}
