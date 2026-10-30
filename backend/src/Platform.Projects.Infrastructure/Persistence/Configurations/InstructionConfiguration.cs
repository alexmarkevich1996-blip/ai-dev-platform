using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Projects.Domain;

namespace Platform.Projects.Infrastructure.Persistence.Configurations;

internal sealed class InstructionConfiguration : IEntityTypeConfiguration<Instruction>
{
    public void Configure(EntityTypeBuilder<Instruction> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("instructions");

        // The Project identifier is the key: one Instruction per Project, enforced
        // by the primary key rather than by a check somewhere in code.
        builder.HasKey(instruction => instruction.ProjectId);

        builder.Property(instruction => instruction.ProjectId)
            .HasConversion(id => id.Value, value => new ProjectId(value))
            .ValueGeneratedNever();

        builder.Property(instruction => instruction.Markdown).IsRequired();

        builder.Property(instruction => instruction.UpdatedAt).IsRequired();
    }
}
