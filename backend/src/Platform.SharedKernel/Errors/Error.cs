namespace Platform.SharedKernel.Errors;

/// <summary>
/// One failure, carrying a stable code for callers and a message for people.
/// </summary>
public sealed record Error
{
    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    /// <summary>A stable identifier, for example <c>agent_token.not_found</c>.</summary>
    public string Code { get; }

    public string Message { get; }

    public ErrorType Type { get; }

    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);

    public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);

    public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);

    public ErrorList ToErrorList() => new(this);

    public static implicit operator ErrorList(Error error) => error.ToErrorList();
}
