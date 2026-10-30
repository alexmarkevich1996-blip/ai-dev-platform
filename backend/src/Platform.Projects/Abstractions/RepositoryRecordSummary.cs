namespace Platform.Projects.Abstractions;

/// <summary>
/// What a reader is told about one Repository Record. It carries no credential and
/// grants no Repository Access.
/// </summary>
public sealed record RepositoryRecordSummary(Guid Id, Uri Address, string Purpose);
