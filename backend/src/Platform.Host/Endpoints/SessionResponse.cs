namespace Platform.Host.Endpoints;

/// <summary>Who the browser is currently signed in as.</summary>
public sealed record SessionResponse(string EmployeeId, string Email, string FullName, IReadOnlyList<string> Roles);
