using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Projects.Domain;

namespace Platform.Projects.Infrastructure.Persistence.Configurations;

internal sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Id)
            .HasConversion(id => id.Value, value => new ProjectId(value))
            .ValueGeneratedNever();

        builder.Property(project => project.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(project => project.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(project => project.CreatedAt).IsRequired();

        // A Project has one Instruction and owns its Repository Records: both are
        // reached through the root and removed with it.
        builder.HasOne(project => project.Instruction)
            .WithOne()
            .HasForeignKey<Instruction>(instruction => instruction.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(project => project.RepositoryRecords)
            .WithOne()
            .HasForeignKey(record => record.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(project => project.RepositoryRecords)
            .HasField("_repositoryRecords")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
