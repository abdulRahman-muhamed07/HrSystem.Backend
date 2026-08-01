using HrSystem.Backend.Models;

namespace HrSystem.Backend.Services;

/// <summary>
/// Computes Egyptian monthly payroll: GOSI social insurance (basic + variable)
/// and progressive personal income tax (annualized). Configurable via appsettings.
/// </summary>
public interface IPayrollEngine
{
    PayrollComputation Compute(Employee employee, decimal overtimePay = 0, decimal otherAllowances = 0,
        decimal otherDeductions = 0, decimal loanDeduction = 0);
}

public class PayrollEngine : IPayrollEngine
{
    private readonly PayrollSettings _settings;

    public PayrollEngine(PayrollSettings settings)
    {
        _settings = settings;
    }

    public PayrollComputation Compute(Employee employee, decimal overtimePay = 0, decimal otherAllowances = 0,
        decimal otherDeductions = 0, decimal loanDeduction = 0)
    {
        var basic = employee.Salary;
        var variable = employee.HousingAllowance + employee.TransportationAllowance + employee.MealAllowance
                       + otherAllowances;
        var gross = basic + variable + overtimePay;

        // GOSI on capped basic and capped variable portions
        var gosiEmpBasic = Math.Min(basic, _settings.BasicSalaryCap) * _settings.GosiEmployeeBasicRate;
        var gosiEmpVariable = Math.Min(variable, _settings.VariableSalaryCap) * _settings.GosiEmployeeVariableRate;
        var gosiEmployerBasic = Math.Min(basic, _settings.BasicSalaryCap) * _settings.GosiEmployerBasicRate;
        var gosiEmployerVariable = Math.Min(variable, _settings.VariableSalaryCap) * _settings.GosiEmployerVariableRate;

        var gosiEmployee = Round2(gosiEmpBasic + gosiEmpVariable);
        var gosiEmployer = Round2(gosiEmployerBasic + gosiEmployerVariable);

        // Taxable income = gross minus the employee's own social insurance share
        var taxableMonthly = gross - gosiEmployee;
        var annualTaxable = taxableMonthly * 12;
        var monthlyTax = Round2(ProgressiveTax(annualTaxable) / 12m);

        var net = gross - gosiEmployee - monthlyTax - loanDeduction - otherDeductions;

        return new PayrollComputation
        {
            BasicSalary = basic,
            HousingAllowance = employee.HousingAllowance,
            TransportationAllowance = employee.TransportationAllowance,
            MealAllowance = employee.MealAllowance,
            OtherAllowances = otherAllowances,
            OvertimePay = overtimePay,
            GrossSalary = gross,
            GosiEmployee = gosiEmployee,
            GosiEmployer = gosiEmployer,
            IncomeTax = monthlyTax,
            LoanDeduction = loanDeduction,
            OtherDeductions = otherDeductions,
            NetSalary = Round2(net)
        };
    }

    private decimal ProgressiveTax(decimal annualTaxable)
    {
        if (annualTaxable <= 0) return 0;

        decimal tax = 0;
        decimal previous = 0;
        var brackets = _settings.TaxBrackets.OrderBy(b => b.UpTo ?? decimal.MaxValue).ToList();

        foreach (var bracket in brackets)
        {
            var upper = bracket.UpTo ?? annualTaxable;
            if (annualTaxable > previous)
            {
                var taxableInBracket = Math.Min(annualTaxable, upper) - previous;
                if (taxableInBracket > 0)
                    tax += taxableInBracket * bracket.Rate;
            }
            previous = upper;
            if (annualTaxable <= upper) break;
        }

        return tax;
    }

    private static decimal Round2(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
