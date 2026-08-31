using HrSystem.Application.Models.Authentication;
using HrSystem.Application.Models.Employees;
using HrSystem.Application.Validators.Authentication;
using HrSystem.Application.Validators.Employees;
using HrSystem.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HrSystem.Application.Tests;

[TestClass]
public sealed class ValidationTests
{
    [TestMethod]
    public void RegisterValidator_ShouldRejectWeakPassword()
    {
        var result = new RegisterRequestValidator().Validate(new RegisterRequest("User", "user@example.com", "123"));
        Assert.IsFalse(result.IsValid);
    }

    [TestMethod]
    public void LoginValidator_ShouldRejectInvalidEmail()
    {
        var result = new LoginRequestValidator().Validate(new LoginRequest("bad", "password"));
        Assert.IsFalse(result.IsValid);
    }

    [TestMethod]
    public void UpdateEmployeeValidator_ShouldRequireVersion()
    {
        var request = new UpdateEmployeeRequest(Guid.Empty, "User", "user@example.com", "Developer", 1, 1000m,
            EmploymentType.FullTime, EmploymentStatus.Active, null, null, 0, 0, 0);
        var result = new UpdateEmployeeRequestValidator().Validate(request);
        Assert.IsFalse(result.IsValid);
    }
}
