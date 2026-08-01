using HrSystem.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Backend.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Only seed if database is empty
        if (db.Departments.Any()) return;

        // ── Departments ────────────────────────────────
        var departments = new List<Department>
        {
            new() { Name = "تطوير البرمجيات", Description = "مسؤول عن بناء وتطوير الأنظمة والتطبيقات" },
            new() { Name = "الموارد البشرية HR", Description = "إدارة الشؤون والتوظيف والأجور" },
            new() { Name = "التسويق والمبيعات", Description = "التسويق الرقمي وجلب العملاء" }
        };
        db.Departments.AddRange(departments);
        db.SaveChanges();

        // ── Employees ──────────────────────────────────
        var employees = new List<Employee>
        {
            new() { FullName = "أحمد محمود العلي", Email = "ahmed@company.com", DepartmentId = 1, JobTitle = "Senior .NET Developer", Salary = 28000m, HireDate = new DateTime(2022, 3, 15) },
            new() { FullName = "سارة عبد الله", Email = "sara@company.com", DepartmentId = 2, JobTitle = "HR Specialist", Salary = 18000m, HireDate = new DateTime(2023, 1, 10) },
            new() { FullName = "محمود الملاح", Email = "mahmoud@company.com", DepartmentId = 3, JobTitle = "Marketing Manager", Salary = 22000m, HireDate = new DateTime(2021, 8, 1) },
            new() { FullName = "فاطمة أحمد", Email = "fatma@company.com", DepartmentId = 1, JobTitle = "Frontend Developer", Salary = 20000m, HireDate = new DateTime(2023, 5, 20) },
            new() { FullName = "عمر حسين", Email = "omar@company.com", DepartmentId = 1, JobTitle = "Backend Developer", Salary = 25000m, HireDate = new DateTime(2022, 11, 1) },
            new() { FullName = "نور الدين", Email = "nour@company.com", DepartmentId = 2, JobTitle = "Recruiter", Salary = 15000m, HireDate = new DateTime(2023, 9, 15) },
            new() { FullName = "ريم محمد", Email = "reem@company.com", DepartmentId = 3, JobTitle = "Social Media Specialist", Salary = 14000m, HireDate = new DateTime(2024, 2, 1) },
            new() { FullName = "خالد إبراهيم", Email = "khaled@company.com", DepartmentId = 1, JobTitle = "DevOps Engineer", Salary = 30000m, HireDate = new DateTime(2021, 6, 10) },
            new() { FullName = "هند علي", Email = "hend@company.com", DepartmentId = 2, JobTitle = "Payroll Specialist", Salary = 16000m, HireDate = new DateTime(2023, 7, 1) },
            new() { FullName = "يوسف أحمد", Email = "youssef@company.com", DepartmentId = 3, JobTitle = "Sales Representative", Salary = 13000m, HireDate = new DateTime(2024, 1, 15) },
            new() { FullName = "مريم حسن", Email = "mariam@company.com", DepartmentId = 1, JobTitle = "QA Engineer", Salary = 19000m, HireDate = new DateTime(2022, 9, 1) },
            new() { FullName = "عبد الرحمن", Email = "abdulrahman@company.com", DepartmentId = 1, JobTitle = "Junior Developer", Salary = 12000m, HireDate = new DateTime(2024, 6, 1) }
        };
        db.Employees.AddRange(employees);
        db.SaveChanges();

        // ── Leave Types ────────────────────────────────
        var leaveTypes = new List<LeaveType>
        {
            new() { Name = "Annual", NameAr = "إجازة سنوية", DaysPerYear = 21, IsPaid = true, Description = "إجازة سنوية مدفوعة الأجر" },
            new() { Name = "Sick", NameAr = "إجازة مرضية", DaysPerYear = 30, IsPaid = true, Description = "إجازة مرضية وفق القانون" },
            new() { Name = "Unpaid", NameAr = "إجازة بدون أجر", DaysPerYear = 15, IsPaid = false, Description = "إجازة بدون أجر" },
            new() { Name = "Maternity", NameAr = "إجازة وضع", DaysPerYear = 90, IsPaid = true, Description = "إجازة وضع مدفوعة الأجر" },
            new() { Name = "Emergency", NameAr = "إجازة اضطرارية", DaysPerYear = 5, IsPaid = true, Description = "إجازة اضطرارية مدفوعة" }
        };
        db.LeaveTypes.AddRange(leaveTypes);
        db.SaveChanges();

        // ── Leave Balances (Annual leave for the current year) ──
        var year = DateTime.Today.Year;
        var annualType = leaveTypes[0];
        foreach (var emp in employees)
        {
            db.EmployeeLeaveBalances.Add(new EmployeeLeaveBalance
            {
                EmployeeId = emp.Id,
                LeaveTypeId = annualType.Id,
                Year = year,
                EntitledDays = annualType.DaysPerYear,
                UsedDays = 0
            });
        }
        db.SaveChanges();

        // ── Users (passwords: Admin123! and Emp123!) ──
        var users = new List<User>
        {
            new() { Email = "admin@company.com", PasswordHash = HashPassword("Admin123!"), FullName = "مدير النظام", Role = "Admin" },
            new() { Email = "emp@company.com", PasswordHash = HashPassword("Emp123!"), FullName = "الموظف التجريبي", Role = "Employee", EmployeeId = 1 }
        };
        db.Users.AddRange(users);
        db.SaveChanges();

        // ── Sample Attendance ───────────────────────────
        var today = DateTime.Today;
        var sampleAttendance = new List<AttendanceRecord>
        {
            new() { EmployeeId = 1, Date = today, CheckIn = new TimeOnly(8, 55), CheckOut = new TimeOnly(17, 0), Status = "منتظم" },
            new() { EmployeeId = 2, Date = today, CheckIn = new TimeOnly(9, 12), CheckOut = new TimeOnly(17, 5), Status = "متأخر" },
            new() { EmployeeId = 3, Date = today, CheckIn = new TimeOnly(8, 45), CheckOut = new TimeOnly(16, 50), Status = "منتظم" },
            new() { EmployeeId = 4, Date = today, CheckIn = new TimeOnly(9, 30), CheckOut = new TimeOnly(17, 15), Status = "متأخر" },
            new() { EmployeeId = 5, Date = today, CheckIn = new TimeOnly(8, 50), CheckOut = new TimeOnly(17, 0), Status = "منتظم" }
        };
        db.AttendanceRecords.AddRange(sampleAttendance);
        db.SaveChanges();

        // ── Sample Leave Requests ───────────────────────
        var sampleLeaves = new List<LeaveRequest>
        {
            new() { EmployeeId = 3, LeaveTypeId = leaveTypes[0].Id, StartDate = new DateTime(2026, 8, 1), EndDate = new DateTime(2026, 8, 5), DurationDays = 5, Reason = "إجازة سنوية شخصية", Status = "Pending" },
            new() { EmployeeId = 1, LeaveTypeId = leaveTypes[1].Id, StartDate = new DateTime(2026, 7, 20), EndDate = new DateTime(2026, 7, 22), DurationDays = 3, Reason = "مرضي", Status = "Approved" }
        };
        db.LeaveRequests.AddRange(sampleLeaves);
        db.SaveChanges();
    }

    // Simple hash function for seed data.
    // In production, use BCrypt or ASP.NET Core Identity.
    // This uses HMAC-SHA256 with a static salt for reproducibility.
    private static string HashPassword(string password)
    {
        var salt = "HrSystem_2024_Salt"u8.ToArray();
        using var hmac = new System.Security.Cryptography.HMACSHA256(salt);
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }

    // Verify password with the same hash function
    public static bool VerifyPassword(string password, string storedHash)
    {
        var computed = HashPassword(password);
        return computed == storedHash;
    }
}