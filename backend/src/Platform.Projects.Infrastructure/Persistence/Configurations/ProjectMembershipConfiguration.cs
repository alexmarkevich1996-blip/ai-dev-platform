using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Projects.Domain;
using Platform.SharedKernel.Identifiers;

namespace Platform.Projects.Infrastructure.Persistence.Configurations;

internal sealed class ProjectMembershipConfiguration : IEntityTypeConfiguration<ProjectMembership>
{
    public void Configure(EntityTypeBuilder<ProjectMembership> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("project_memberships");

        builder.HasKey(membership => membership.Id);

        builder.Property(membership => membership.ProjectId)
            .HasConversion(id => id.Value, value => new ProjectId(value))
            .ValueGeneratedNever();

        // A plain uuid column. There is deliberately no foreign key into the
        // accounts schema: the Employee lives in another module.
        builder.Property(membership => membership.EmployeeId)
            .HasConversion(id => id.Value, value => new EmployeeId(value))
            .ValueGeneratedNever();

        builder.Property(membership => membership.StartedAt).IsRequired();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(membership => membership.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Every agent request filters by Employee, then by Project.
        builder.HasIndex(membership => new { membership.EmployeeId, membership.ProjectId });

        builder.Ignore(membership => membership.IsCurrent);
    }
}
