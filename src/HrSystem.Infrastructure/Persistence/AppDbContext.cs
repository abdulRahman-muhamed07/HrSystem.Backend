using HrSystem.Application;
using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances => Set<EmployeeLeaveBalance>();
    public DbSet<OvertimeRequest> OvertimeRequests => Set<OvertimeRequest>();
    public DbSet<EmployeeLoan> EmployeeLoans => Set<EmployeeLoan>();
    public DbSet<PayrollRecord> PayrollRecords => Set<PayrollRecord>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e => { e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedOnAdd(); e.Property(x => x.Email).HasMaxLength(256).IsRequired(); e.HasIndex(x => x.Email).IsUnique(); e.Property(x => x.PasswordHash).IsRequired(); e.Property(x => x.FullName).HasMaxLength(200).IsRequired(); e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20); e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.SetNull); });
        modelBuilder.Entity<Department>(e => { e.HasKey(x => x.Id); e.Property(x => x.Name).HasMaxLength(200).IsRequired(); e.HasIndex(x => x.Name).IsUnique(); e.Property(x => x.Description).HasMaxLength(500); });
        modelBuilder.Entity<Employee>(e => { e.HasKey(x => x.Id); e.Property(x => x.FullName).HasMaxLength(200).IsRequired(); e.Property(x => x.Email).HasMaxLength(256).IsRequired(); e.HasIndex(x => x.Email).IsUnique(); e.Property(x => x.JobTitle).HasMaxLength(200).IsRequired(); e.Property(x => x.NationalId).HasMaxLength(14); e.Property(x => x.Phone).HasMaxLength(30); e.Property(x => x.Address).HasMaxLength(500); e.Property(x => x.BankName).HasMaxLength(100); e.Property(x => x.BankAccountNumber).HasMaxLength(50); e.Property(x => x.EmploymentType).HasConversion<string>().HasMaxLength(20); e.Property(x => x.EmploymentStatus).HasConversion<string>().HasMaxLength(20); e.Property(x => x.Salary).HasPrecision(18,2); e.Property(x => x.HousingAllowance).HasPrecision(18,2); e.Property(x => x.TransportationAllowance).HasPrecision(18,2); e.Property(x => x.MealAllowance).HasPrecision(18,2); e.HasOne(x => x.Department).WithMany(x => x.Employees).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<AttendanceRecord>(e => { e.HasKey(x => x.Id); e.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique(); e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20); e.HasOne(x => x.Employee).WithMany(x => x.AttendanceRecords).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade); });
        modelBuilder.Entity<LeaveType>(e => { e.HasKey(x => x.Id); e.HasIndex(x => x.Name).IsUnique(); e.Property(x => x.Name).HasMaxLength(100).IsRequired(); e.Property(x => x.NameAr).HasMaxLength(100); e.Property(x => x.DaysPerYear).HasPrecision(5,1); e.Property(x => x.Description).HasMaxLength(500); });
        modelBuilder.Entity<LeaveRequest>(e => { e.HasKey(x => x.Id); e.Property(x => x.Reason).HasMaxLength(1000).IsRequired(); e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20); e.Property(x => x.DurationDays).HasPrecision(5,1); e.Property(x => x.RejectionReason).HasMaxLength(1000); e.HasOne(x => x.Employee).WithMany(x => x.LeaveRequests).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade); e.HasOne(x => x.LeaveType).WithMany(x => x.LeaveRequests).HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<EmployeeLeaveBalance>(e => { e.HasKey(x => x.Id); e.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique(); e.Property(x => x.EntitledDays).HasPrecision(5,1); e.Property(x => x.UsedDays).HasPrecision(5,1); e.Property(x => x.AdjustedDays).HasPrecision(5,1); e.HasOne(x => x.Employee).WithMany(x => x.LeaveBalances).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade); e.HasOne(x => x.LeaveType).WithMany(x => x.Balances).HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.Cascade); });
        modelBuilder.Entity<OvertimeRequest>(e => { e.HasKey(x => x.Id); e.Property(x => x.Hours).HasPrecision(5,2); e.Property(x => x.RateMultiplier).HasPrecision(4,2); e.Property(x => x.Reason).HasMaxLength(500); e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20); e.HasOne(x => x.Employee).WithMany(x => x.OvertimeRequests).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade); });
        modelBuilder.Entity<EmployeeLoan>(e => { e.HasKey(x => x.Id); e.Property(x => x.Amount).HasPrecision(18,2); e.Property(x => x.MonthlyDeduction).HasPrecision(18,2); e.Property(x => x.RemainingAmount).HasPrecision(18,2); e.Property(x => x.Reason).HasMaxLength(500); e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20); e.HasOne(x => x.Employee).WithMany(x => x.Loans).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade); });
        modelBuilder.Entity<PayrollRecord>(e => { e.HasKey(x => x.Id); e.HasIndex(x => new { x.EmployeeId, x.Year, x.Month }).IsUnique(); e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20); e.HasOne(x => x.Employee).WithMany(x => x.PayrollRecords).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade); foreach (var p in new[] { "BasicSalary", "HousingAllowance", "TransportationAllowance", "MealAllowance", "OtherAllowances", "OvertimePay", "GrossSalary", "GosiEmployee", "GosiEmployer", "IncomeTax", "LoanDeduction", "OtherDeductions", "NetSalary" }) e.Property<decimal>(p).HasPrecision(18,2); });
        modelBuilder.Entity<AuditLog>(e => { e.HasKey(x => x.Id); e.Property(x => x.Action).HasMaxLength(50).IsRequired(); e.Property(x => x.EntityName).HasMaxLength(100).IsRequired(); e.Property(x => x.EntityId).HasMaxLength(50); e.Property(x => x.UserName).HasMaxLength(200); e.Property(x => x.IpAddress).HasMaxLength(50); e.Property(x => x.Details).HasMaxLength(4000); e.HasIndex(x => x.Timestamp); });
        base.OnModelCreating(modelBuilder);
    }
}
