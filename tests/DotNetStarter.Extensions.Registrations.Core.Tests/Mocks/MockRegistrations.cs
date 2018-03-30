using System.Text;
using DotNetStarter.Abstractions;

namespace DotNetStarter.Extensions.Registrations.Core.Tests.Mocks
{
    /// <summary>
    /// Shows how to oeverride, and first class for unsorted test
    /// </summary>
    [Registration(typeof(ZTest1), Lifecycle.Transient, typeof(ATest2))]
    public class ZTest1 { }

    [Registration(typeof(ZTest1))]
    public class ATest2 : ZTest1 { }

    [DependencyConfiguration]
    public class ExternalConfiguration
    {
        public static void Configure(DependencyConfigurationExpression configure)
        {
            configure.AddTransient<StringBuilder>();
        }
    }
}