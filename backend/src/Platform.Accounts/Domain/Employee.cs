using Microsoft.AspNetCore.Identity;
using Platform.SharedKernel.Identifiers;

namespace Platform.Accounts.Domain;

/// <summary>
/// A person in the company who holds a platform account.
/// </summary>
/// <remarks>
/// The Employee is the ASP.NET Core Identity user: sign-in, password hashing, and
/// roles all belong to Accounts, so there is no second user record to keep in step.
/// An Employee comes into being only by accepting an Invitation; self-registration
/// is switched off in the composition root.
/// </remarks>
public sealed class Employee : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;

    /// <summary>The identifier other modules use to refer to this Employee.</summary>
    public EmployeeId EmployeeId => new(Id);
}
