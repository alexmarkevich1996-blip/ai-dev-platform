using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Accounts.Domain;

namespace Platform.Accounts.Infrastructure.Persistence.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("employees");

        builder.Property(employee => employee.FullName)
            .HasMaxLength(200)
            .IsRequired();

        // EmployeeId is derived from the Identity key and is not a second column.
        builder.Ignore(employee => employee.EmployeeId);
    }
}
