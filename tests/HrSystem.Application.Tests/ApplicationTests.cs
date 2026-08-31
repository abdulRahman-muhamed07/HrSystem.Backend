using AutoMapper;
using HrSystem.Application.Mapping;
using HrSystem.Application.Models.Common;
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
}
