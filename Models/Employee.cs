namespace HrSystem.Backend.Models;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // ── Egyptian HR profile fields ────────────────────
    public string? NationalId { get; set; }              // الرقم القومي
    public string? Phone { get; set; }
    public string? Gender { get; set; }                  // Male / Female
    public DateTime? BirthDate { get; set; }
    public string? MaritalStatus { get; set; }           // Single / Married / Divorced / Widowed
    public string? Address { get; set; }
    public string? ProfilePhotoPath { get; set; }

    public string EmploymentType { get; set; } = "FullTime"; // FullTime / PartTime / Contract / Probation
    public string EmploymentStatus { get; set; } = "Active"; // Active / Resigned / Terminated / OnLeave
    public DateTime? ResignationDate { get; set; }

    // ── Employment / financial data ───────────────────
    public string JobTitle { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public decimal Salary { get; set; }                  // Basic salary (الأساسي)
    public decimal HousingAllowance { get; set; }        // بدل سكن
    public decimal TransportationAllowance { get; set; } // بدل انتقال
    public decimal MealAllowance { get; set; }           // بدل وجبات
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public DateTime? ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }

    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }

    // Navigation
    public virtual Department? Department { get; set; }
    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public virtual ICollection<EmployeeLeaveBalance> LeaveBalances { get; set; } = new List<EmployeeLeaveBalance>();
    public virtual ICollection<OvertimeRequest> OvertimeRequests { get; set; } = new List<OvertimeRequest>();
    public virtual ICollection<EmployeeLoan> Loans { get; set; } = new List<EmployeeLoan>();
    public virtual ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();
}
