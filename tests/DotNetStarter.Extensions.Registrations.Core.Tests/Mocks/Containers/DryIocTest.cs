using DotNetStarter.Abstractions;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetStarter.Extensions.Registrations.Core.Tests.Mocks.Containers
{
    public class DryIocTest : IContainerTest
    {
        private readonly IEnumerable<DependentRegistration> _registrations;
        private readonly Container _container;

        public DryIocTest(IEnumerable<DependentRegistration> registrations)
        {
            _registrations = registrations;
            _container = new Container
            (
                Rules.Default
                .WithoutThrowIfDependencyHasShorterReuseLifespan()
                .WithFactorySelector(Rules.SelectLastRegisteredFactory())
                .WithTrackingDisposableTransients() //used in transient delegate cases
                .WithImplicitRootOpenScope()
            );
        }

        public string ContainerName { get; } = "DryIoc";

        public T Get<T>() => _container.Resolve<T>();

        public IEnumerable<T> All<T>() => _container.ResolveMany<T>();

        public void Configure()
        {
            foreach (var r in _registrations)
                _container.Register(r.Registration.ServiceType, r.Implementation, ConvertLifeTime(r.Registration.Lifecycle), GetConstructorFor(r.Implementation));
        }

        private static IReuse ConvertLifeTime(Lifecycle lifetime)
        {
            switch (lifetime)
            {
                case Lifecycle.Singleton:
                    return Reuse.Singleton;
                case Lifecycle.Scoped:
                    return Reuse.ScopedOrSingleton;
                default:
                    return Reuse.Transient;
            }
        }

        private static Made GetConstructorFor(Type implementationType)
        {
            return Made.Of(implementationType.GetConstructors()
                .OrderByDescending(x => x.GetParameters().Length).FirstOrDefault());
        }
    }
}