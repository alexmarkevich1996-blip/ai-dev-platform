using System.ComponentModel;

namespace Platform.AgentDelivery.Tools;

/// <summary>A Project as an Employee Agent sees it.</summary>
public sealed record AgentProjectView(
    [property: Description("Identifier to pass to the other tools.")] string ProjectId,
    [property: Description("The Project's name.")] string Name,
    [property: Description("What the Project is for.")] string Description);
