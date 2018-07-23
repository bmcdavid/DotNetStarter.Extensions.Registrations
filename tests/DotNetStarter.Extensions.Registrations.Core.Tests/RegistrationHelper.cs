using DotNetStarter.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations.Core.Tests
{
    public class RegistrationHelper
    {
        private readonly IDependentRegistrationFactory _dependentRegistrationFactory;
        private readonly IRegistrationSorter _registrationSorter;

        public RegistrationHelper(IDependentRegistrationFactory dependentRegistrationFactory = null, IRegistrationSorter registrationSorter = null)
        {
            _dependentRegistrationFactory = dependentRegistrationFactory ?? new DependentRegistrationFactory();
            _registrationSorter = registrationSorter ?? new RegistrationSorter();
        }

        public ICollection<DependentRegistration> GetRegistrations(IEnumerable<Assembly> assemblies = null)
        {
            assemblies = assemblies ?? GetAssemblies()
                             .Where(a => a.GetCustomAttribute<DiscoverableAssemblyAttribute>() != null);

            var registrations = _dependentRegistrationFactory.CreateDependentRegistrations
            (
                assemblies
            );

            _registrationSorter.Sort(registrations);

            return registrations;
        }

        private IEnumerable<Assembly> GetAssemblies()
        {
#if NETCOREAPP1_0
            var libraries = Microsoft.Extensions.DependencyModel.DependencyContextExtensions.GetRuntimeAssemblyNames
            (
                Microsoft.Extensions.DependencyModel.DependencyContext.Default,
                Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.GetRuntimeIdentifier()
            );
            return libraries.Select(x => Assembly.Load(new AssemblyName(x.Name)));
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }
    }
}