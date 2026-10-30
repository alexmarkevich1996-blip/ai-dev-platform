using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Accounts.Domain;

namespace Platform.Accounts.Infrastructure.Persistence.Configurations;

internal sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("invitations");

        builder.HasKey(invitation => invitation.Id);

        builder.Property(invitation => invitation.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(invitation => invitation.TokenHash)
            .HasConversion(hash => hash.Value, value => SecretHash.FromStoredValue(value))
            .HasMaxLength(AgentTokenConfiguration.HashLength)
            .IsRequired();

        builder.Property(invitation => invitation.IssuedAt).IsRequired();
        builder.Property(invitation => invitation.ExpiresAt).IsRequired();

        builder.HasIndex(invitation => invitation.TokenHash).IsUnique();

        builder.Ignore(invitation => invitation.IsAccepted);
    }
}
