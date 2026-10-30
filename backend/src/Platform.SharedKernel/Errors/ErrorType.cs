namespace Platform.SharedKernel.Errors;

/// <summary>
/// The kind of failure an <see cref="Error"/> reports. The HTTP layer maps each
/// kind to a status code, so a module never names a status code itself.
/// </summary>
public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Forbidden,
    Unauthorized,
    Failure,
}
