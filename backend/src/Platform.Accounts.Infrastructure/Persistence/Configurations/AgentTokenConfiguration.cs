using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Accounts.Domain;
using Platform.SharedKernel.Identifiers;

namespace Platform.Accounts.Infrastructure.Persistence.Configurations;

internal sealed class AgentTokenConfiguration : IEntityTypeConfiguration<AgentToken>
{
    public void Configure(EntityTypeBuilder<AgentToken> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("agent_tokens");

        builder.HasKey(token => token.Id);

        builder.Property(token => token.EmployeeId)
            .HasConversion(id => id.Value, value => new EmployeeId(value))
            .IsRequired();

        builder.Property(token => token.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Only the hash is stored. The secret itself is shown once, when issued.
        builder.Property(token => token.TokenHash)
            .HasConversion(hash => hash.Value, value => SecretHash.FromStoredValue(value))
            .HasMaxLength(SecretHash.Length)
            .IsRequired();

        builder.Property(token => token.IssuedAt).IsRequired();

        // Resolving a presented token is a lookup by hash on every agent request.
        builder.HasIndex(token => token.TokenHash).IsUnique();

        // Offboarding revokes every token of one Employee in one statement.
        builder.HasIndex(token => token.EmployeeId);
    }
}
