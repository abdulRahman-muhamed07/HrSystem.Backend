using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HrSystem.Application.Tests.Architecture;

[TestClass]
public sealed class ArchitectureDependencyTests
{
    [TestMethod]
    public void Domain_does_not_reference_outer_layers()
    {
        var references = ReferencedAssemblyNames(typeof(global::HrSystem.Domain.Entities.Employee).Assembly);

        CollectionAssert.DoesNotContain(references.ToList(), "HrSystem.Application");
        CollectionAssert.DoesNotContain(references.ToList(), "HrSystem.Infrastructure");
        CollectionAssert.DoesNotContain(references.ToList(), "HrSystem.Api");
    }

    [TestMethod]
    public void Application_does_not_reference_infrastructure_or_api()
    {
        var references = ReferencedAssemblyNames(typeof(global::HrSystem.Application.Features.Attendance.IAttendanceService).Assembly);

        CollectionAssert.DoesNotContain(references.ToList(), "HrSystem.Infrastructure");
        CollectionAssert.DoesNotContain(references.ToList(), "HrSystem.Api");
    }

    [TestMethod]
    public void Infrastructure_does_not_reference_api()
    {
        var references = ReferencedAssemblyNames(typeof(global::HrSystem.Infrastructure.Security.CurrentUser).Assembly);

        CollectionAssert.DoesNotContain(references.ToList(), "HrSystem.Api");
    }

    private static HashSet<string> ReferencedAssemblyNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase)!;
}
