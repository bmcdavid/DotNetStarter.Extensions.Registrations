using System.Text;
using DotNetStarter.Abstractions;

namespace DotNetStarter.Extensions.Registrations.Core.Tests.Mocks
{
    public interface IService { }

    [Registration(typeof(IService), Lifecycle.Transient, typeof(Service5))]
    public class Service1: IService { }

    [Registration(typeof(IService), Lifecycle.Transient, typeof(Service3))]
    public class Service2 : IService { }

    [Registration(typeof(IService), Lifecycle.Transient, typeof(Service5))]
    public class Service3 : IService { }

    [Registration(typeof(IService), Lifecycle.Transient, typeof(Service2), typeof(Service1))]
    public class Service4 : IService { }

    [Registration(typeof(IService))]
    public class Service5 : IService { }
    
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
        public static void Configure(IDependencyConfigurationExpression configure)
        {
            configure.AddTransient<StringBuilder>();
        }
    }
}