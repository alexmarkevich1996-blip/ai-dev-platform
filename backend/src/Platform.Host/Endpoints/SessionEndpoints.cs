using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Platform.Accounts.Domain;

namespace Platform.Host.Endpoints;

/// <summary>
/// Starting, reading, and ending a browser session.
/// </summary>
/// <remarks>
/// There is no registration endpoint, by decision: an account comes into being only
/// when an Invitation is accepted. Accepting an Invitation is the onboarding spec's
/// work and lands beside these endpoints.
/// </remarks>
internal static class SessionEndpoints
{
    public static IEndpointRouteBuilder MapSessionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var session = endpoints.MapGroup("/api/session").WithTags("Session");

        session.MapGet(string.Empty, GetCurrentSessionAsync)
            .WithName("GetCurrentSession")
            .WithSummary("Returns the signed-in Employee, or 401 when there is no session.");

        session.MapPost(string.Empty, SignInAsync)
            .WithName("SignIn")
            .WithSummary("Starts a session and sets the session cookie.")
            .AllowAnonymous();

        session.MapDelete(string.Empty, SignOutAsync)
            .WithName("SignOut")
            .WithSummary("Ends the session and clears the cookie.");

        return endpoints;
    }

    private static async Task<Results<Ok<SessionResponse>, UnauthorizedHttpResult>> GetCurrentSessionAsync(
        ClaimsPrincipal principal,
        UserManager<Employee> employees)
    {
        var employee = await employees.GetUserAsync(principal);

        if (employee is null)
        {
            return TypedResults.Unauthorized();
        }

        var roles = await employees.GetRolesAsync(employee);

        return TypedResults.Ok(new SessionResponse(
            employee.EmployeeId.ToString(),
            employee.Email ?? string.Empty,
            employee.FullName,
            [.. roles]));
    }

    private static async Task<Results<NoContent, ValidationProblem, UnauthorizedHttpResult>> SignInAsync(
        SignInRequest request,
        IValidator<SignInRequest> validator,
        SignInManager<Employee> signIn,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var outcome = await signIn.PasswordSignInAsync(
            request.Email,
            request.Password,
            isPersistent: true,
            lockoutOnFailure: true);

        // One answer for a wrong password, an unknown address, and a locked account:
        // the caller learns only that the attempt failed.
        return outcome.Succeeded ? TypedResults.NoContent() : TypedResults.Unauthorized();
    }

    private static async Task<NoContent> SignOutAsync(SignInManager<Employee> signIn)
    {
        await signIn.SignOutAsync();
        return TypedResults.NoContent();
    }
}
