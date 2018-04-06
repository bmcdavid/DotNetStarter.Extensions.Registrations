using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetStarter.Abstractions;
using DryIoc;

namespace DotNetStarter.Extensions.Registrations.Core.Tests.Mocks.Containers
{
    public class DryIocTest : IContainerTest
    {
        private readonly IEnumerable<DependentRegistration> _registrations;
        private readonly Container _container;

        public DryIocTest(IEnumerable<DependentRegistration> registrations)
        {
            _registrations = registrations;
            var rules = Rules.Default
                    .WithoutThrowIfDependencyHasShorterReuseLifespan()
                    .WithFactorySelector(Rules.SelectLastRegisteredFactory())
                    .WithTrackingDisposableTransients() //used in transient delegate cases
                    .WithImplicitRootOpenScope()
                ;

            _container = new Container(rules);
        }

        public string ContainerName { get; } = "DryIoc";

        public T Get<T>()
        {
            return _container.Resolve<T>();
        }

        public IEnumerable<T> All<T>()
        {
            return _container.ResolveMany<T>();
        }

        public void Configure()
        {
            foreach (var r in _registrations)
            {
                RegisterSimple(_container, r.Registration.ServiceType, r.Implementation, ConvertLifeTime(r.Registration.Lifecycle));
            }
        }

        private static IReuse ConvertLifeTime(Lifecycle lifetime)
        {
            switch (lifetime)
            {
                case Lifecycle.Singleton:
                    return Reuse.Singleton;
                case Lifecycle.Transient:
                    return Reuse.Transient;
                case Lifecycle.Scoped:
                    return Reuse.ScopedOrSingleton;
            }

            return Reuse.Transient;
        }

        private static Made GetConstructorFor(Type implementationType)
        {
            var allConstructors = implementationType.GetTypeInfo()
                .DeclaredConstructors
                .Where(x => x.IsConstructor && x.IsPublic)
                .OrderByDescending(x => x.GetParameters().Length);

            return Made.Of(allConstructors.FirstOrDefault());
        }

        private static void RegisterSimple(IRegistrator register, Type service, Type implementation, IReuse reuse = null, string key = null)
        {
            register.Register(service, implementation, reuse: reuse, made:  GetConstructorFor(implementation), serviceKey: key);
        }

    }
}