using DotNetStarter.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DotNetStarter.Extensions.Registrations.Core.Tests
{
    [TestClass]
    public class DependentRegistrationTests
    {
        [TestMethod]
        public void ShouldChangeLifecycle()
        {
            // local function
            bool GetZTest(DependentRegistration r)
            {
                return r.Implementation == typeof(ZTest1);
            }

            var registrations = GetRegistrationsFromMocks();
            var registration = registrations.FirstOrDefault(GetZTest);
            var preChange = registration.CustomLifeCycle;
            registration.CustomLifeCycle = Lifecycle.Singleton;

            Assert.IsNull(preChange);
            Assert.IsTrue(registration.Registration.Lifecycle == Lifecycle.Transient);
            Assert.IsTrue(registration.CustomLifeCycle == Lifecycle.Singleton);
        }

        [TestMethod]
        public void ShouldCreateRegistrationsFromAssembly()
        {
            var registrations = GetRegistrationsFromMocks();

            Assert.IsTrue(registrations.Count > 0);
            Assert.IsTrue(registrations.Any(r => r.Implementation == typeof(ZTest1)));
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
}
