using Microsoft.VisualStudio.TestTools.UnitTesting;
using HrSystem.Application;

namespace HrSystem.Application.Tests;

[TestClass]
public sealed class AuthorizationIdentityTests
{
    [TestMethod]
    public void CurrentUserContract_ShouldExposeEmployeeIdentity()
    {
        var properties = typeof(ICurrentUser).GetProperties().Select(x => x.Name).ToHashSet();

        Assert.IsTrue(properties.Contains(nameof(ICurrentUser.UserId)));
        Assert.IsTrue(properties.Contains(nameof(ICurrentUser.EmployeeId)));
        Assert.IsTrue(properties.Contains(nameof(ICurrentUser.Role)));
    }
}
