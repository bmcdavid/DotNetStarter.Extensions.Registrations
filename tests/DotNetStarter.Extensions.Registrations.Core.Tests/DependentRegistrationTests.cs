using DotNetStarter.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNetStarter.Extensions.Registrations.Core.Tests
{
    [TestClass]
    public class DependentRegistrationTests
    {
        [TestMethod]
        public void ShouldChangeLifecycle()
        {
            var registrations = GetRegistrationsFromMocks();
            var registration = registrations.FirstOrDefault(r => r.Implementation == typeof(ZTest1));
            var preChange = registration.CustomLifeCycle;
            registration.CustomLifeCycle = Lifecycle.Singleton;

            Assert.IsNull(preChange);
            Assert.IsNotNull(registration.CustomLifeCycle);
            Assert.IsTrue(registration.Registration.Lifecycle == Lifecycle.Transient);            
            Assert.IsTrue(registrations.First(r => r.Implementation == typeof(ZTest1)).CustomLifeCycle != registration.Registration.Lifecycle);
        }

        [TestMethod]
        public void ShouldConfigureExpression()
        {
            string methodName = "Configure";
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;// BindingFlags.NonPublic | BindingFlags.FlattenHierarchy                
            List<Type> configurationTypes = new List<Type>
            {
                typeof(TestClassConfigure1),
                typeof(TestClassConfigure2)
            };
            var sut = new ConfigurationExpression(new Assembly[] { configurationTypes[0].Assembly });

            foreach (var type in configurationTypes)
            {
                MethodInfo info = type.GetMethod(methodName, flags);
                info.Invoke(null, new object[] { sut });
            }

            Assert.IsNotNull(sut);
            Assert.IsTrue(sut.Build().Count == 3);
        }

        [TestMethod]
        public void ShouldCreateRegistrationsFromAssembly()
        {
            var registrations = GetRegistrationsFromMocks();

            Assert.IsTrue(registrations.Count > 0);
            Assert.IsTrue(registrations.Any(r => r.Implementation == typeof(ZTest1)));
        }

        [TestMethod]
        public void ShouldDiscoverStringBuilder()
        {
            var registrations = GetRegistrationsFromMocks();

            Assert.IsTrue(registrations.Any(x => x.Implementation == typeof(StringBuilder)));
        }

        [TestMethod]
        public void ShouldSortRegistrations()
        {
            var registrations = GetRegistrationsFromMocks();
            var sut = new RegistrationSorter();
            var test1Index = 0;
            var test2Index = 0;
            var index = 0;

            // local function
            void CheckIndexes()
            {
                foreach (var r in registrations)
                {
                    if (r.Implementation == typeof(ZTest1)) test1Index = index;
                    if (r.Implementation == typeof(ATest2)) test2Index = index;

                    index++;
                }
            }

            CheckIndexes();
            Assert.IsFalse(test1Index > test2Index, "Failed unsorted");

            sut.Sort(registrations);
            CheckIndexes();

            Assert.IsTrue(test1Index > test2Index, "Failed sorted");
        }

        private ICollection<DependentRegistration> GetRegistrationsFromMocks()
        {
            var factory = new DependentRegistrationFactory();
            var registrations = factory.CreateDependentRegistrations(new[] { typeof(DependentRegistrationTests).Assembly });

            return registrations;
        }
    }

    public class TestClassConfigure1
    {
        public static void Configure(ConfigurationExpression expression)
        {
            expression
                .Add(typeof(StringBuilder), typeof(StringBuilder), Lifecycle.Transient)
                .Add(typeof(DependentRegistration));
        }
    }

    public class TestClassConfigure2
    {
        public static void Configure(ConfigurationExpression expression)
        {
            expression
                .Add(typeof(StringBuilder), typeof(StringBuilder), Lifecycle.Transient)
                .Add(typeof(RegistrationAttribute))
                .Remove(typeof(DependentRegistration));
        }
    }
}
