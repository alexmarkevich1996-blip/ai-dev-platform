namespace Platform.Accounts.Abstractions;

/// <summary>What an <see cref="IInvitationSender"/> needs to deliver one Invitation.</summary>
public sealed record InvitationDelivery(string Email, Uri AcceptUrl, DateTimeOffset ExpiresAt);
