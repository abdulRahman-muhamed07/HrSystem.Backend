using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Backend.Services;

public interface ILeaveBalanceService
{
    /// <summary>Working days between two dates (inclusive), excluding the Egyptian weekend (Friday & Saturday).</summary>
    decimal WorkingDays(DateTime start, DateTime end);

    /// <summary>Ensures a balance row exists for the employee/leave type/year and returns it.</summary>
    Task<EmployeeLeaveBalance> EnsureBalanceAsync(AppDbContext db, int employeeId, int leaveTypeId, int year);

    /// <summary>Remaining balance for an employee/leave type/year (entitled - used + adjusted).</summary>
    Task<decimal> RemainingAsync(AppDbContext db, int employeeId, int leaveTypeId, int year);
}

public class LeaveBalanceService : ILeaveBalanceService
{
    public decimal WorkingDays(DateTime start, DateTime end)
    {
        if (end < start) return 0;
        int count = 0;
        for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
        {
            if (d.DayOfWeek != DayOfWeek.Friday && d.DayOfWeek != DayOfWeek.Saturday)
                count++;
        }
        return count;
    }

    public async Task<EmployeeLeaveBalance> EnsureBalanceAsync(AppDbContext db, int employeeId, int leaveTypeId, int year)
    {
        var balance = await db.EmployeeLeaveBalances
            .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.LeaveTypeId == leaveTypeId && b.Year == year);

        if (balance != null) return balance;

        var leaveType = await db.LeaveTypes.FindAsync(leaveTypeId);
        var employee = await db.Employees.FindAsync(employeeId);

        var entitled = leaveType?.DaysPerYear ?? 21;
        // Prorate entitlement for employees hired mid-year
        if (employee != null && employee.HireDate.Year == year)
        {
            var monthsWorked = Math.Max(1, 12 - employee.HireDate.Month + 1);
            entitled = Math.Round(entitled * monthsWorked / 12m, 1);
        }

        balance = new EmployeeLeaveBalance
        {
            EmployeeId = employeeId,
            LeaveTypeId = leaveTypeId,
            Year = year,
            EntitledDays = entitled,
            UsedDays = 0,
            AdjustedDays = 0
        };
        db.EmployeeLeaveBalances.Add(balance);
        return balance;
    }

    public async Task<decimal> RemainingAsync(AppDbContext db, int employeeId, int leaveTypeId, int year)
    {
        var balance = await db.EmployeeLeaveBalances
            .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.LeaveTypeId == leaveTypeId && b.Year == year);

        if (balance != null)
            return balance.EntitledDays - balance.UsedDays + balance.AdjustedDays;

        // No balance row yet — fall back to the leave type's entitled days (prorated for mid-year hires).
        var leaveType = await db.LeaveTypes.FindAsync(leaveTypeId);
        if (leaveType == null) return 0;

        var entitled = leaveType.DaysPerYear;
        var employee = await db.Employees.FindAsync(employeeId);
        if (employee != null && employee.HireDate.Year == year)
        {
            var monthsWorked = Math.Max(1, 12 - employee.HireDate.Month + 1);
            entitled = Math.Round(entitled * monthsWorked / 12m, 1);
        }

        return entitled;
    }
}
