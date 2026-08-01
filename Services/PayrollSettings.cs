namespace HrSystem.Backend.Services;

/// <summary>
/// Configurable Egyptian payroll parameters (GOSI rates/caps + income tax brackets).
/// Values are loaded from the "PayrollSettings" section of appsettings.json and can be
/// updated as legislation changes.
/// </summary>
public class PayrollSettings
{
    // GOSI (Law 148/2019) employee & employer rates
    public decimal GosiEmployeeBasicRate { get; set; } = 0.11m;
    public decimal GosiEmployeeVariableRate { get; set; } = 0.11m;
    public decimal GosiEmployerBasicRate { get; set; } = 0.1875m;
    public decimal GosiEmployerVariableRate { get; set; } = 0.1875m;

    // Insurance contribution caps (monthly, EGP)
    public decimal BasicSalaryCap { get; set; } = 12400m;
    public decimal VariableSalaryCap { get; set; } = 24800m;

    // Income tax — annual brackets (Law 91/2005 as amended). Progressive.
    public List<TaxBracket> TaxBrackets { get; set; } = new()
    {
        new TaxBracket { UpTo = 45000m, Rate = 0m },
        new TaxBracket { UpTo = 60000m, Rate = 0.10m },
        new TaxBracket { UpTo = 200000m, Rate = 0.15m },
        new TaxBracket { UpTo = 400000m, Rate = 0.20m },
        new TaxBracket { UpTo = 600000m, Rate = 0.225m },
        new TaxBracket { UpTo = 1000000m, Rate = 0.25m },
        new TaxBracket { UpTo = null, Rate = 0.275m }
    };
}

public class TaxBracket
{
    public decimal? UpTo { get; set; }   // null = last (unlimited) bracket
    public decimal Rate { get; set; }
}

/// <summary>
/// Result of computing a single employee's monthly payroll.
/// </summary>
public class PayrollComputation
{
    public decimal BasicSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportationAllowance { get; set; }
    public decimal MealAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal GosiEmployee { get; set; }
    public decimal GosiEmployer { get; set; }
    public decimal IncomeTax { get; set; }
    public decimal LoanDeduction { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal NetSalary { get; set; }
}
