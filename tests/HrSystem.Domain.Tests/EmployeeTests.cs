using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HrSystem.Domain.Tests;

[TestClass]
public sealed class EmployeeTests
{
    [TestMethod]
    public void UpdateProfile_ShouldChangeEmployeeData()
    {
        var employee = new Employee("Ahmed", "ahmed@example.com", "Developer", 1, 10000m, new DateTime(2026, 1, 1));

        employee.UpdateProfile(
            "Mostafa",
            "mostafa@example.com",
            "Senior Developer",
            2,
            15000m,
            EmploymentType.FullTime,
            EmploymentStatus.Active,
            "01000000000",
            "Cairo");

        Assert.AreEqual("Mostafa", employee.FullName);
        Assert.AreEqual("mostafa@example.com", employee.Email);
        Assert.AreEqual("Senior Developer", employee.JobTitle);
        Assert.AreEqual(2, employee.DepartmentId);
        Assert.AreEqual(15000m, employee.Salary);
        Assert.AreEqual(EmploymentStatus.Active, employee.EmploymentStatus);
    }

    [TestMethod]
    public void UpdateAllowances_ShouldStoreAllowanceValues()
    {
        var employee = new Employee("Ahmed", "ahmed@example.com", "Developer", 1, 10000m, DateTime.UtcNow);

        employee.UpdateAllowances(1000m, 500m, 250m);

        Assert.AreEqual(1000m, employee.HousingAllowance);
        Assert.AreEqual(500m, employee.TransportationAllowance);
        Assert.AreEqual(250m, employee.MealAllowance);
    }
}
