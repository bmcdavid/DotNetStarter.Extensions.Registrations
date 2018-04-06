using DotNetStarter.Abstractions;
using SimpleInjector;
using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNetStarter.Extensions.Registrations.Core.Tests.Mocks.Containers
{
    public class SimpleInjectorTests : IContainerTest
    {
        private readonly Container _container;

        private readonly IEnumerable<DependentRegistration> _registrations;

        public SimpleInjectorTests(IEnumerable<DependentRegistration> registrations)
        {
            _registrations = registrations;
            _container = new Container();
            _container.Options.AllowOverridingRegistrations = true;
            _container.Options.ConstructorResolutionBehavior = new GreediestConstructorBehavior();
        }

        public string ContainerName { get; } = "SimpleInjector";

        public IEnumerable<T> All<T>() => _container.GetAllInstances(typeof(T)).OfType<T>();

        public void Configure()
        {
            var groupd = _registrations.GroupBy
            (
                x => x.Registration.ServiceType,
                (k, g) => new { ServiceType = k, RegistrationList = g.ToList() }
            );

            foreach (var r in groupd)
            {
                _container.RegisterCollection
                (
                    r.ServiceType,
                    r.RegistrationList.Select(x => ConvertToRegistration(x, _container))
                );

                var lastReg = r.RegistrationList.Last();
                _container.Register
                (
                    lastReg.Registration.ServiceType,
                    lastReg.Implementation,
                    ConvertLifeTime(lastReg.CustomLifeCycle ?? lastReg.Registration.Lifecycle)
                );
            }

            // hack: simpleinjector resolving func
            _container.RegisterInstance<Func<ZTest1>>(() => _container.GetInstance<ZTest1>());
            //_container.Register<Func<T>>(() => _container.GetInstance<T>)
            //_container.Register<Func<Type, object>>(() => { return (x) => _container.GetInstance(x); });

        }

        public T Get<T>() => (T)_container.GetInstance(typeof(T));

        private static Lifestyle ConvertLifeTime(Lifecycle lifetime)
        {
            switch (lifetime)
            {
                case Lifecycle.Singleton:
                    return Lifestyle.Singleton;
                case Lifecycle.Scoped:
                    return Lifestyle.Scoped;
                default:
                    return Lifestyle.Transient;
            }
        }

        private static Registration ConvertToRegistration(DependentRegistration r, Container c)
        {
            var lifecycle = r.CustomLifeCycle ?? r.Registration.Lifecycle;

            switch (lifecycle)
            {
                case Lifecycle.Scoped:
                    return Lifestyle.Scoped.CreateRegistration(r.Implementation, c);

                case Lifecycle.Singleton:
                    return Lifestyle.Singleton.CreateRegistration(r.Implementation, c);

                default:
                    return Lifestyle.Transient.CreateRegistration(r.Implementation, c);
            }
        }
        private class GreediestConstructorBehavior : IConstructorResolutionBehavior
        {
            public ConstructorInfo GetConstructor(Type implementationType)
            {
                var constructors = (from ctor in implementationType.GetConstructors()
                    orderby ctor.GetParameters().Length descending
                    select ctor);

                // hack: simpleinjector only for existing test configuration test, shouldn't really be used
                return (implementationType == typeof(StringBuilder)) ? constructors.Last() : constructors.First();
            }
        }
    }
}