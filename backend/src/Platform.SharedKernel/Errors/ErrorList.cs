using System.Collections;

namespace Platform.SharedKernel.Errors;

/// <summary>
/// The failure side of every <c>Result&lt;T, ErrorList&gt;</c> in the platform.
/// A list rather than a single error, because validation reports all problems at once.
/// </summary>
public sealed class ErrorList : IReadOnlyList<Error>
{
    private readonly Error[] _errors;

    public ErrorList(params Error[] errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Length == 0)
        {
            throw new ArgumentException("An error list holds at least one error.", nameof(errors));
        }

        _errors = [.. errors];
    }

    public ErrorList(IEnumerable<Error> errors)
        : this(errors?.ToArray() ?? throw new ArgumentNullException(nameof(errors)))
    {
    }

    public int Count => _errors.Length;

    public Error this[int index] => _errors[index];

    /// <summary>The first error, which callers use when they report only one.</summary>
    public Error First => _errors[0];

    public IEnumerator<Error> GetEnumerator() => ((IEnumerable<Error>)_errors).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _errors.GetEnumerator();

    public override string ToString() => string.Join("; ", _errors.Select(error => $"{error.Code}: {error.Message}"));
}
