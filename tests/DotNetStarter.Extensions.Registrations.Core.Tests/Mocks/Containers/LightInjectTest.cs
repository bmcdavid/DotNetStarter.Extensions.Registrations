using DotNetStarter.Abstractions;
using LightInject;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations.Core.Tests.Mocks.Containers
{
    public class LightInjectTest : IContainerTest
    {
        private readonly IEnumerable<DependentRegistration> _registrations;
        private readonly ServiceContainer _container;

        public LightInjectTest(IEnumerable<DependentRegistration> registrations)
        {
            _registrations = registrations;
            _container = new ServiceContainer
            (
                new ContainerOptions
                {
                    EnableVariance = false,
                    EnablePropertyInjection = false, // for netcore support
                }
            );
        }

        public string ContainerName { get; } = "LightInject";

        public T Get<T>()
        {
            return _container.GetInstance<T>();
        }

        public IEnumerable<T> All<T>()
        {
            //return _container.GetAllInstances<T>();
            // hack: lightinject to get proper sorting, move unamed to last,
            // also this will not work for constructor injecting
            var serviceList = _container.GetAllInstances<T>().ToList();
            var first = serviceList[0];
            serviceList.RemoveAt(0);
            serviceList.Add(first);

            return serviceList;
        }

        public void Configure()
        {
            var groupd = _registrations.GroupBy
            (
                x => x.Registration.ServiceType,
                (k, g) => new { ServiceType = k, RegistrationList = g.ToList() }
            );

            foreach (var item in groupd)
            {
                _container.RegisterOrdered
                (
                    item.ServiceType,
                    item.RegistrationList.Select(x => x.Implementation).ToArray(),
                    t => ConvertLifetime
                    (
                        item.RegistrationList[0].CustomLifeCycle ??
                        item.RegistrationList[0].Registration.Lifecycle
                    ),
                    (i) => NameService(i, item.ServiceType, item.RegistrationList)
                );
            }
        }

        private static string NameService(int i, MemberInfo service, ICollection registrations)
        {
            var name = i == registrations.Count ? string.Empty : $"{service.Name}{i.ToString().PadLeft(3, '0')}";

            return name;
        }

        private static ILifetime ConvertLifetime(Lifecycle lifetime)
        {
            switch (lifetime)
            {
                case Lifecycle.Scoped:
                    return new PerScopeLifetime();

                case Lifecycle.Singleton:
                    return new PerContainerLifetime();

                case Lifecycle.Transient:
                    return null;
            }

            return null;
        }
    }
}