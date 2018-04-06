using DotNetStarter.Abstractions;
using Grace.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace DotNetStarter.Extensions.Registrations.Core.Tests.Mocks.Containers
{
    public class GraceTest : IContainerTest
    {
        private readonly IEnumerable<DependentRegistration> _registrations;
        private readonly DependencyInjectionContainer _container;

        public GraceTest(IEnumerable<DependentRegistration> registrations)
        {
            _registrations = registrations;
            _container = new DependencyInjectionContainer();
        }

        public string ContainerName { get; } = "Grace";

        public T Get<T>() => _container.Locate<T>();

        public IEnumerable<T> All<T>() => _container.LocateAll(typeof(T)).OfType<T>();

        public void Configure() => _container.Configure(c => Register(c, _registrations));

        private static void Register(IExportRegistrationBlock c, IEnumerable<DependentRegistration> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                if (descriptor.Implementation != null)
                {
                    c.Export(descriptor.Implementation).
                      As(descriptor.Registration.ServiceType).
                      ConfigureLifetime(descriptor.Registration.Lifecycle);
                }
            }
        }
    }

    internal static class GraceExtensions
    {
        internal static IFluentExportStrategyConfiguration ConfigureLifetime(this IFluentExportStrategyConfiguration configuration, Lifecycle lifetime)
        {
            switch (lifetime)
            {
                case Lifecycle.Scoped:
                    return configuration.Lifestyle.SingletonPerScope();
                case Lifecycle.Singleton:
                    return configuration.Lifestyle.Singleton();
                default:
                    return configuration;
            }
        }
    }
}