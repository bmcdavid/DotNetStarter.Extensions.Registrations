using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetStarter.Abstractions;
using DotNetStarter.Extensions.Registrations.Core;
using EPiServer.ServiceLocation;

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
        /// <param name="registrationSorter">Optional custom registration sorter.</param>
        /// <param name="addServiceAccessor">Optional function to determine if a service accessor is needed for Inject&lt;T> usages</param>
        public static void AddDotNetStarterRegistrations(this IServiceConfigurationProvider services,
            IEnumerable<Assembly> assembliesToScan = null,
            IRegistrationSorter registrationSorter = null,
            Func<IRegisteredService, bool> addServiceAccessor = null)
        {
            var assemblies = assembliesToScan ??
                             AssemblyLoader()
                                 .Where(a => a.GetCustomAttribute<DiscoverableAssemblyAttribute>() != null);

            var sorted = (registrationSorter ?? new RegistrationSorter()).Sort(assemblies);

            foreach (var t in sorted)
            {
                var service = services.Add
                (
                    t.Registration.ServiceType,
                    t.Implementation,
                    ConvertToServiceLifetime(t.Registration.Lifecycle)
                );

                if (addServiceAccessor?.Invoke(service) == true)
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
        public static ServiceInstanceScope ConvertToServiceLifetime(Lifecycle lifecycle)
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