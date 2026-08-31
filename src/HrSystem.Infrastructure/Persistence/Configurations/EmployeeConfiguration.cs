using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrSystem.Infrastructure.Persistence.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.JobTitle).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NationalId).HasMaxLength(14);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.BankName).HasMaxLength(100);
        builder.Property(x => x.BankAccountNumber).HasMaxLength(50);
        builder.Property(x => x.EmploymentType).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.EmploymentStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Salary).HasPrecision(18, 2);
        builder.Property(x => x.HousingAllowance).HasPrecision(18, 2);
        builder.Property(x => x.TransportationAllowance).HasPrecision(18, 2);
        builder.Property(x => x.MealAllowance).HasPrecision(18, 2);
        builder.HasOne(x => x.Department).WithMany(x => x.Employees).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
    }
}
