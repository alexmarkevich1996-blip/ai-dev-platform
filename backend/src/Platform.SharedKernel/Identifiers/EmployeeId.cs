namespace Platform.SharedKernel.Identifiers;

/// <summary>
/// Identifies an Employee across module boundaries.
/// </summary>
/// <remarks>
/// This type lives in the shared kernel, not in Accounts, because Projects stores an
/// Employee identifier and must not reference Accounts. Keeping the identifier here
/// is what lets the two modules stay apart while still speaking about the same person.
/// </remarks>
public readonly record struct EmployeeId(Guid Value)
{
    public static EmployeeId New() => new(Guid.CreateVersion7());

    public static EmployeeId Parse(string value) => new(Guid.Parse(value));

    public static bool TryParse(string? value, out EmployeeId employeeId)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            employeeId = new EmployeeId(parsed);
            return true;
        }

        employeeId = default;
        return false;
    }

    public override string ToString() => Value.ToString();
}
