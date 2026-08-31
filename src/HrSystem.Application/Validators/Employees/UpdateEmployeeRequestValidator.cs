using FluentValidation;
using HrSystem.Application.Models.Employees;

namespace HrSystem.Application.Validators.Employees;

public sealed class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.Version).NotEqual(Guid.Empty);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.Salary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.HousingAllowance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TransportationAllowance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MealAllowance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Phone).MaximumLength(30).When(x => x.Phone is not null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
    }
}
