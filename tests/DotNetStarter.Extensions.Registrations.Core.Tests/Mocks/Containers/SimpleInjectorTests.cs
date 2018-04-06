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

        public IEnumerable<T> All<T>()
        {
            return _container.GetAllInstances(typeof(T)).OfType<T>();
        }

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
        }

        public T Get<T>()
        {
            return (T)_container.GetInstance(typeof(T));
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

        private static Lifestyle ConvertLifeTime(Lifecycle lifetime)
        {
            switch (lifetime)
            {
                case Lifecycle.Singleton:
                    return Lifestyle.Singleton;

                case Lifecycle.Transient:
                    return Lifestyle.Transient;

                case Lifecycle.Scoped:
                    return Lifestyle.Scoped;
            }

            return Lifestyle.Transient;
        }

        // Custom constructor resolution behavior
        private class GreediestConstructorBehavior : IConstructorResolutionBehavior
        {
            public ConstructorInfo GetConstructor(Type implementationType)
            {
                // hack: only for existing test configuration test, shouldn't really be used
                if (implementationType == typeof(StringBuilder))
                {
                    return (from ctor in implementationType.GetConstructors()
                            orderby ctor.GetParameters().Length descending
                            select ctor)
                        .Last();
                }

                return (from ctor in implementationType.GetConstructors()
                        orderby ctor.GetParameters().Length descending
                        select ctor)
                    .First();
            }
        }
    }
}