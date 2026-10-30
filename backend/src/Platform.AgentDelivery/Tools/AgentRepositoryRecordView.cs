using System.ComponentModel;

namespace Platform.AgentDelivery.Tools;

/// <summary>
/// A Repository Record as an Employee Agent sees it. It carries no credential: the
/// right to read the code is Repository Access, which lives in the code host.
/// </summary>
public sealed record AgentRepositoryRecordView(
    [property: Description("Where the Repository lives in the code host.")] string Address,
    [property: Description("What this Repository is for.")] string Purpose);
