using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Accounts.Domain;
using Platform.SharedKernel.Identifiers;

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
            .HasMaxLength(SecretHash.Length)
            .IsRequired();

        // A plain uuid array column. There is deliberately no foreign key into the
        // projects schema: the Project lives in another module. See ProjectMembership
        // for the same rule applied to the Employee identifier, the other direction.
        builder.Property(invitation => invitation.ProjectIds)
            .HasConversion(
                ids => ids.Select(id => id.Value).ToArray(),
                values => values.Select(value => new ProjectId(value)).ToList())
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyList<ProjectId>>(
                (left, right) => left!.SequenceEqual(right!),
                ids => ids.Aggregate(0, (hash, id) => HashCode.Combine(hash, id.Value)),
                ids => ids.ToList()));

        builder.Property(invitation => invitation.IssuedAt).IsRequired();
        builder.Property(invitation => invitation.ExpiresAt).IsRequired();

        builder.HasIndex(invitation => invitation.TokenHash).IsUnique();
    }
}
