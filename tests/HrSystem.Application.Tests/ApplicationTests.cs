using AutoMapper;
using HrSystem.Application.Mapping;
using HrSystem.Application.Models.Authentication;
using HrSystem.Application.Models.Common;
using HrSystem.Application.Models.Employees;
using HrSystem.Application.Validators.Authentication;
using HrSystem.Application.Validators.Employees;
using HrSystem.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HrSystem.Application.Tests;

[TestClass]
public sealed class ApplicationTests
{
    [TestMethod]
    public void PagedResult_ShouldCalculateTotalPages()
    {
        var result = new PagedResult<int>([1, 2], 2, 2, 5);
        Assert.AreEqual(3, result.TotalPages);
    }

    [TestMethod]
    public void MappingProfile_ShouldBeValid()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<HrSystemMappingProfile>(),
            NullLoggerFactory.Instance);
        configuration.AssertConfigurationIsValid();
    }

    [TestMethod]
    public void RegisterValidator_ShouldRejectWeakPassword()
    {
        var validator = new RegisterRequestValidator();
        var result = validator.Validate(new RegisterRequest("Test User", "user@example.com", "123"));
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(x => x.PropertyName == nameof(RegisterRequest.Password)));
    }

    [TestMethod]
    public void LoginValidator_ShouldRejectInvalidEmail()
    {
        var validator = new LoginRequestValidator();
        var result = validator.Validate(new LoginRequest("not-an-email", "password"));
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(x => x.PropertyName == nameof(LoginRequest.Email)));
    }

    [TestMethod]
    public void UpdateEmployeeValidator_ShouldRequireVersion()
    {
        var validator = new UpdateEmployeeRequestValidator();
        var request = new UpdateEmployeeRequest(Guid.Empty, "User", "user@example.com", "Developer", 1, 1000m,
            EmploymentType.FullTime, EmploymentStatus.Active, null, null, 0, 0, 0);
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(x => x.PropertyName == nameof(UpdateEmployeeRequest.Version)));
    }
}
