using System.ComponentModel;

namespace Platform.AgentDelivery.Tools;

/// <summary>A Project's Instruction as an Employee Agent sees it.</summary>
public sealed record AgentInstructionView(
    [property: Description("The Instruction, in Markdown.")] string Markdown,
    [property: Description("When the Instruction was last rewritten.")] DateTimeOffset UpdatedAt);
