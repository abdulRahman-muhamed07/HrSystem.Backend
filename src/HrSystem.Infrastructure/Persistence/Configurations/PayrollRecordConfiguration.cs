using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrSystem.Infrastructure.Persistence.Configurations;

public sealed class PayrollRecordConfiguration : IEntityTypeConfiguration<PayrollRecord>
{
    public void Configure(EntityTypeBuilder<PayrollRecord> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.EmployeeId, x.Year, x.Month }).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasOne(x => x.Employee).WithMany(x => x.PayrollRecords).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.BasicSalary).HasPrecision(18, 2);
        builder.Property(x => x.HousingAllowance).HasPrecision(18, 2);
        builder.Property(x => x.TransportationAllowance).HasPrecision(18, 2);
        builder.Property(x => x.MealAllowance).HasPrecision(18, 2);
        builder.Property(x => x.OtherAllowances).HasPrecision(18, 2);
        builder.Property(x => x.OvertimePay).HasPrecision(18, 2);
        builder.Property(x => x.GrossSalary).HasPrecision(18, 2);
        builder.Property(x => x.GosiEmployee).HasPrecision(18, 2);
        builder.Property(x => x.GosiEmployer).HasPrecision(18, 2);
        builder.Property(x => x.IncomeTax).HasPrecision(18, 2);
        builder.Property(x => x.LoanDeduction).HasPrecision(18, 2);
        builder.Property(x => x.OtherDeductions).HasPrecision(18, 2);
        builder.Property(x => x.NetSalary).HasPrecision(18, 2);
    }
}
