﻿using System.Collections.Generic;
using DotNetStarter.Abstractions;
using StructureMap;
using StructureMap.Pipeline;

namespace DotNetStarter.Extensions.Registrations.Core.Tests.Mocks.Containers
{
    public class StructureMapTest : IContainerTest
    {
        private readonly IEnumerable<DependentRegistration> _registrations;
        private readonly Container _container;

        public StructureMapTest(IEnumerable<DependentRegistration> registrations)
        {
            _registrations = registrations;
            _container = new Container();
        }

        public string ContainerName { get; } = "Structuremap";

        public T Get<T>()
        {
            return _container.GetInstance<T>();
        }

        public IEnumerable<T> All<T>()
        {
            return _container.GetAllInstances<T>();
        }

        public void Configure()
        {
            _container.Configure(x =>
            {
                foreach (var registration in _registrations)
                {
                    x.For(registration.Registration.ServiceType)
                        .LifecycleIs(ConvertLifeTime(registration.CustomLifeCycle ?? registration.Registration.Lifecycle))
                        .Use(registration.Implementation);
                }
            });
        }

        private static ILifecycle ConvertLifeTime(Lifecycle lifetime)
        {
            switch (lifetime)
            {
                case Lifecycle.Transient:
                    return Lifecycles.Transient;

                case Lifecycle.Singleton:
                    return Lifecycles.Singleton;

                case Lifecycle.Scoped:
                    return Lifecycles.Container;
            }

            return Lifecycles.Transient;
        }
    }
}