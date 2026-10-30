namespace Platform.Accounts.Domain;

/// <summary>
/// The two roles of the first version. Configurable roles are deferred — see
/// <c>docs/first-version.md</c>.
/// </summary>
public static class EmployeeRoles
{
    /// <summary>May invite people, create Projects, assign Membership, and run Offboarding.</summary>
    public const string CompanyAdministrator = "CompanyAdministrator";

    /// <summary>Holds an account and sees the Projects they are a member of.</summary>
    public const string Employee = "Employee";

    public static IReadOnlyList<string> All { get; } = [CompanyAdministrator, Employee];
}
