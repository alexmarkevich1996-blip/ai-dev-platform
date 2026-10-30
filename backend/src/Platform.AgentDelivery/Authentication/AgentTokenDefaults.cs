namespace Platform.AgentDelivery.Authentication;

/// <summary>Names the Agent Token authentication scheme and the claim it issues.</summary>
public static class AgentTokenDefaults
{
    /// <summary>The authentication scheme an Employee Agent uses. Separate from the browser cookie.</summary>
    public const string AuthenticationScheme = "AgentToken";

    /// <summary>The authorization policy guarding the MCP route.</summary>
    public const string Policy = "AgentToken";

    /// <summary>The claim carrying the Employee the presented token belongs to.</summary>
    public const string EmployeeIdClaimType = "platform:employee_id";
}
