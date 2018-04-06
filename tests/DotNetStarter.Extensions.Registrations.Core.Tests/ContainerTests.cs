using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetStarter.Abstractions;
using DotNetStarter.Extensions.Registrations.Core.Tests.Mocks;
using DotNetStarter.Extensions.Registrations.Core.Tests.Mocks.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetStarter.Extensions.Registrations.Core.Tests
{
    [TestClass]
    public class ContainerTests
    {
        private IList<IContainerTest> _containers;

        [TestInitialize]
        public void Init()
        {
            // create and sort dependent registrations with DiscoverableAssembly attribute
            var registrationFactory = new DependentRegistrationFactory();
            var sorter = new RegistrationSorter();
            var registrations = registrationFactory.CreateDependentRegistrations
            (
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.GetCustomAttribute<DiscoverableAssemblyAttribute>() != null)
            );

            sorter.Sort(registrations);

            // assign the registrations to container test instances
            _containers = new List<IContainerTest>
            {
                new StructureMapTest(registrations),
                new DryIocTest(registrations),
                new GraceTest(registrations),
                new LightInjectTest(registrations),
                new SimpleInjectorTests(registrations)
            };

            // configure the registrations for the container test
            foreach (var c in _containers) c.Configure();
        }

        [TestMethod]
        public void ShouldResolveLast()
        {
            foreach (var c in _containers)
            {
                var sut = c.Get<ZTest1>();
                Assert.IsInstanceOfType(sut, typeof(ZTest1), c.ContainerName + " failed last registration");

                var sut2 = c.Get<IService>();
                Assert.IsInstanceOfType(sut2, typeof(Service4), c.ContainerName + " failed last registration");
            }
        }

        [TestMethod]
        public void ShouldResolveDeterministicOrder()
        {
            foreach (var c in _containers)
            {
                var sut = c.All<IService>().ToList();

                Assert.IsTrue(sut.First() is Service5, c.ContainerName + " failed deterministic");
                Assert.IsTrue(sut[1] is Service1, c.ContainerName + " failed deterministic");
                Assert.IsTrue(sut[2] is Service3, c.ContainerName + " failed deterministic");
                Assert.IsTrue(sut[3] is Service2, c.ContainerName + " failed deterministic");
                Assert.IsTrue(sut.Last() is Service4, c.ContainerName + " failed deterministic");
            }
        }

        [TestMethod]
        public void ShouldResolveSimpleFunc()
        {
            foreach (var c in _containers)
            {
                var sut = c.Get<Func<ZTest1>>();
                var instance = sut?.Invoke();
                Assert.IsNotNull(instance, c.ContainerName + " failed func test");
                Assert.IsTrue(instance.Services.Count == 5, c.ContainerName + " failed injecting services");
            }
        }
    }
}