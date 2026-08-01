namespace HrSystem.Backend.Models;

public class PayrollRecord
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }

    // Earnings
    public decimal BasicSalary { get; set; }        // الأجر الأساسي
    public decimal HousingAllowance { get; set; }   // بدل سكن
    public decimal TransportationAllowance { get; set; } // بدل انتقال
    public decimal MealAllowance { get; set; }      // بدل وجبات
    public decimal OtherAllowances { get; set; }
    public decimal OvertimePay { get; set; }        // أجر الأوفر تايم
    public decimal GrossSalary { get; set; }        // إجمالي الأجر

    // Deductions
    public decimal GosiEmployee { get; set; }       // حصة الموظف تأمينات
    public decimal GosiEmployer { get; set; }       // حصة الشركة تأمينات
    public decimal IncomeTax { get; set; }          // ضريبة الدخل
    public decimal LoanDeduction { get; set; }      // خصم السلف
    public decimal OtherDeductions { get; set; }
    public decimal NetSalary { get; set; }          // صافي الأجر

    public string Status { get; set; } = "Draft";   // Draft, Paid
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Employee? Employee { get; set; }
}
