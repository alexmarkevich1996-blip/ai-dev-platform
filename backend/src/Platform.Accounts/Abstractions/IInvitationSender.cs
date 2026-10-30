using CSharpFunctionalExtensions;
using Platform.SharedKernel.Errors;

namespace Platform.Accounts.Abstractions;

/// <summary>
/// Delivers an Invitation to the person it names.
/// </summary>
/// <remarks>
/// The first version has no SMTP and no mail container. The development
/// implementation writes the link to the application log, so an Invitation can be
/// accepted without a mail server. The contract is here so that adding real mail
/// later changes one registration and nothing else.
/// </remarks>
public interface IInvitationSender
{
    Task<UnitResult<ErrorList>> SendAsync(InvitationDelivery delivery, CancellationToken cancellationToken);
}
