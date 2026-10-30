using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Platform.Accounts.Abstractions;
using Platform.SharedKernel.Errors;

namespace Platform.Accounts.Infrastructure.Invitations;

/// <summary>
/// Writes the Invitation link to the application log instead of sending mail.
/// </summary>
/// <remarks>
/// The first version runs without SMTP and without a mail container, so this is how
/// an Invitation is accepted during development: read the link from the console.
/// Replacing this registration with a real sender is the only change real mail needs.
/// </remarks>
/// <remarks>
/// Registered only in Development, by
/// <see cref="AccountsInfrastructureModule.AddDevelopmentInvitationSender"/>. Were it
/// registered everywhere, a production log would carry working accept links.
/// </remarks>
internal sealed class LoggingInvitationSender(ILogger<LoggingInvitationSender> logger) : IInvitationSender
{
    public Task<UnitResult<ErrorList>> SendAsync(InvitationDelivery delivery, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(delivery);

        logger.LogInformation(
            "Invitation for {Email} is valid until {ExpiresAt:O}. Accept it at {AcceptUrl}",
            delivery.Email,
            delivery.ExpiresAt,
            delivery.AcceptUrl);

        return Task.FromResult(UnitResult.Success<ErrorList>());
    }
}
