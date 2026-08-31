using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrSystem.Infrastructure.Persistence.Configurations;

public sealed class EmployeeLeaveBalanceConfiguration : IEntityTypeConfiguration<EmployeeLeaveBalance>
{
    public void Configure(EntityTypeBuilder<EmployeeLeaveBalance> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique();
        builder.Property(x => x.EntitledDays).HasPrecision(5, 1);
        builder.Property(x => x.UsedDays).HasPrecision(5, 1);
        builder.Property(x => x.AdjustedDays).HasPrecision(5, 1);
        builder.HasOne(x => x.Employee).WithMany(x => x.LeaveBalances).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.LeaveType).WithMany(x => x.Balances).HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.Cascade);
    }
}
