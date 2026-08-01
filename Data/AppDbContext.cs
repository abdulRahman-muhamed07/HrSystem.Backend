using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Models;

namespace HrSystem.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
        // ── User ───────────────────────────────────────
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.FullName).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(50).HasDefaultValue("Employee");
            entity.Property(u => u.IsActive).HasDefaultValue(true);
        });

        // ── Department ─────────────────────────────────
        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(d => d.Name).HasMaxLength(200).IsRequired();
            entity.Property(d => d.Description).HasMaxLength(500);
        });

        // ── Employee ───────────────────────────────────
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.JobTitle).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.HousingAllowance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TransportationAllowance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MealAllowance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NationalId).HasMaxLength(14);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.MaritalStatus).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.EmploymentType).HasMaxLength(20).HasDefaultValue("FullTime");
            entity.Property(e => e.EmploymentStatus).HasMaxLength(20).HasDefaultValue("Active");
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.BankAccountNumber).HasMaxLength(50);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Attendance ─────────────────────────────────
        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.HasIndex(a => new { a.EmployeeId, a.Date }).IsUnique();
            entity.Property(a => a.Status).HasMaxLength(50).HasDefaultValue("OnTime");

            entity.HasOne(a => a.Employee)
                .WithMany(e => e.AttendanceRecords)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Leave Request ──────────────────────────────
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.Property(l => l.Reason).HasMaxLength(1000).IsRequired();
            entity.Property(l => l.Status).HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(l => l.DurationDays).HasColumnType("decimal(5,1)");
            entity.Property(l => l.RejectionReason).HasMaxLength(1000);

            entity.HasOne(l => l.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.LeaveType)
                .WithMany(t => t.LeaveRequests)
                .HasForeignKey(l => l.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Leave Type ─────────────────────────────────
        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasIndex(t => t.Name).IsUnique();
            entity.Property(t => t.Name).HasMaxLength(100).IsRequired();
            entity.Property(t => t.NameAr).HasMaxLength(100);
            entity.Property(t => t.DaysPerYear).HasColumnType("decimal(5,1)");
            entity.Property(t => t.Description).HasMaxLength(500);
        });

        // ── Employee Leave Balance ─────────────────────
        modelBuilder.Entity<EmployeeLeaveBalance>(entity =>
        {
            entity.HasIndex(b => new { b.EmployeeId, b.LeaveTypeId, b.Year }).IsUnique();
            entity.Property(b => b.EntitledDays).HasColumnType("decimal(5,1)");
            entity.Property(b => b.UsedDays).HasColumnType("decimal(5,1)");
            entity.Property(b => b.AdjustedDays).HasColumnType("decimal(5,1)");

            entity.HasOne(b => b.Employee)
                .WithMany(e => e.LeaveBalances)
                .HasForeignKey(b => b.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.LeaveType)
                .WithMany(t => t.Balances)
                .HasForeignKey(b => b.LeaveTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Overtime ───────────────────────────────────
        modelBuilder.Entity<OvertimeRequest>(entity =>
        {
            entity.Property(o => o.Reason).HasMaxLength(500);
            entity.Property(o => o.Status).HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(o => o.Hours).HasColumnType("decimal(5,2)");
            entity.Property(o => o.RateMultiplier).HasColumnType("decimal(4,2)");

            entity.HasOne(o => o.Employee)
                .WithMany(e => e.OvertimeRequests)
                .HasForeignKey(o => o.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Employee Loan ──────────────────────────────
        modelBuilder.Entity<EmployeeLoan>(entity =>
        {
            entity.Property(l => l.Amount).HasColumnType("decimal(18,2)");
            entity.Property(l => l.MonthlyDeduction).HasColumnType("decimal(18,2)");
            entity.Property(l => l.RemainingAmount).HasColumnType("decimal(18,2)");
            entity.Property(l => l.Reason).HasMaxLength(500);
            entity.Property(l => l.Status).HasMaxLength(50).HasDefaultValue("Pending");

            entity.HasOne(l => l.Employee)
                .WithMany(e => e.Loans)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Payroll ────────────────────────────────────
        modelBuilder.Entity<PayrollRecord>(entity =>
        {
            entity.HasIndex(p => new { p.EmployeeId, p.Year, p.Month }).IsUnique();
            entity.Property(p => p.BasicSalary).HasColumnType("decimal(18,2)");
            entity.Property(p => p.HousingAllowance).HasColumnType("decimal(18,2)");
            entity.Property(p => p.TransportationAllowance).HasColumnType("decimal(18,2)");
            entity.Property(p => p.MealAllowance).HasColumnType("decimal(18,2)");
            entity.Property(p => p.OtherAllowances).HasColumnType("decimal(18,2)");
            entity.Property(p => p.OvertimePay).HasColumnType("decimal(18,2)");
            entity.Property(p => p.GrossSalary).HasColumnType("decimal(18,2)");
            entity.Property(p => p.GosiEmployee).HasColumnType("decimal(18,2)");
            entity.Property(p => p.GosiEmployer).HasColumnType("decimal(18,2)");
            entity.Property(p => p.IncomeTax).HasColumnType("decimal(18,2)");
            entity.Property(p => p.LoanDeduction).HasColumnType("decimal(18,2)");
            entity.Property(p => p.OtherDeductions).HasColumnType("decimal(18,2)");
            entity.Property(p => p.NetSalary).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Status).HasMaxLength(20).HasDefaultValue("Draft");

            entity.HasOne(p => p.Employee)
                .WithMany(e => e.PayrollRecords)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Audit Log ──────────────────────────────────
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(a => a.Action).HasMaxLength(50).IsRequired();
            entity.Property(a => a.EntityName).HasMaxLength(100).IsRequired();
            entity.Property(a => a.EntityId).HasMaxLength(50);
            entity.Property(a => a.UserName).HasMaxLength(200);
            entity.Property(a => a.IpAddress).HasMaxLength(50);
            entity.Property(a => a.Details).HasMaxLength(4000);
            entity.HasIndex(a => a.Timestamp);
        });

        base.OnModelCreating(modelBuilder);
    }
}