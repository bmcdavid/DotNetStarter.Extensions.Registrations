using DotNetStarter.Abstractions;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations.EpiserverCms
{
    /// <summary>
    /// Registration exgtensions
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Add classes with [Registration] attributes to Episerver Service Locator.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembliesToScan">Assemblies to scan for types with RegistrationAttribute,
        ///      if null Assemblies with DiscoverableAssemblyAttribute are used to filter.</param>
        /// <param name="dependentRegistrationFactory">Optional custom IDependentRegistrationFacory</param>
        /// <param name="registrationSorter">Optional custom registration sorter.</param>
        /// <param name="addServiceAccessor">Optional function to determine if a service accessor is needed for Inject&lt;T> usages</param>
        /// <param name="registrationFilter">Optional action to modify regististrations, add/remove or changelifestyle.</param>
        public static void AddDotNetStarterRegistrations
        (
            this IServiceConfigurationProvider services,
            IEnumerable<Assembly> assembliesToScan = null,
            IDependentRegistrationFactory dependentRegistrationFactory = null,
            IRegistrationSorter registrationSorter = null,
            Func<DependentRegistration, bool> addServiceAccessor = null,
            Action<ICollection<DependentRegistration>> registrationFilter = null
        )
        {
            var assemblies = assembliesToScan ??
                             AssemblyLoader()
                                 .Where(a => a.GetCustomAttribute<DiscoverableAssemblyAttribute>() != null);
            var registrations = 
                (dependentRegistrationFactory ?? new DependentRegistrationFactory()).
                    CreateDependentRegistrations(assemblies);

            registrationFilter?.Invoke(registrations);
            (registrationSorter ?? new RegistrationSorter()).Sort(registrations);

            foreach (var t in registrations)
            {
                var service = services.Add
                (
                    t.Registration.ServiceType,
                    t.Implementation,
                    (t.CustomLifeCycle ?? t.Registration.Lifecycle).ConvertToServiceLifetime()
                );

                if (addServiceAccessor?.Invoke(t) == true)
                {
                    service.AddServiceAccessor();
                }
            }
        }

        /// <summary>
        /// Gets project assemblies from the dependency context
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> AssemblyLoader()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// Converts DotNetStarter.Lifecycle to ServiceLifetime
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public static ServiceInstanceScope ConvertToServiceLifetime(this Lifecycle lifecycle)
        {
            switch (lifecycle)
            {
                case Lifecycle.Singleton: return ServiceInstanceScope.Singleton;
                case Lifecycle.Scoped: return ServiceInstanceScope.HttpContext;
                case Lifecycle.Transient: return ServiceInstanceScope.Transient;
                default: throw new Exception($"Unable to convert {lifecycle} to {typeof(ServiceInstanceScope).FullName}!");
            }
        }
    }
}