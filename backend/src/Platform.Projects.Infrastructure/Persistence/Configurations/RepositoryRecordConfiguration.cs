using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Projects.Domain;

namespace Platform.Projects.Infrastructure.Persistence.Configurations;

internal sealed class RepositoryRecordConfiguration : IEntityTypeConfiguration<RepositoryRecord>
{
    public void Configure(EntityTypeBuilder<RepositoryRecord> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("repository_records");

        builder.HasKey(record => record.Id);

        builder.Property(record => record.ProjectId)
            .HasConversion(id => id.Value, value => new ProjectId(value))
            .ValueGeneratedNever();

        builder.Property(record => record.Address)
            .HasConversion(address => address.AbsoluteUri, value => new Uri(value))
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(record => record.Purpose)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(record => record.AddedAt).IsRequired();

        builder.HasIndex(record => record.ProjectId);
    }
}
