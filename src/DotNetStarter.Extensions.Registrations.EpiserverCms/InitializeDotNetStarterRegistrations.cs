using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetStarter.Abstractions;
using DotNetStarter.Extensions.Registrations.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace DotNetStarter.Extensions.Registrations.EpiserverCms
{
    /// <summary>
    /// Scans assemblies to find
    /// </summary>
    [InitializableModule]
    public class InitializeDotNetStarterRegistrations : IConfigurableModule
    {
        /// <summary>
        /// Allows for assemblies to be customized, default looks for assemblies with DiscoverableAssemblyAttribute attributes
        /// </summary>
        public static Func<IEnumerable<Assembly>> GetAssembliesToScan { get; set; }

        /// <summary>
        /// Allows for registration sorter customization
        /// </summary>
        public static Func<IRegistrationSorter> GetRegistrationSorter { get; set; }

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

        /// <summary>
        /// Gets project assemblies from the dependency context
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> AssemblyLoader()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            var assemblies = GetAssembliesToScan?.Invoke() ??
                             AssemblyLoader()
                                 .Where(a => a.GetCustomAttribute<DiscoverableAssemblyAttribute>() != null);

            var sorted = (GetRegistrationSorter?.Invoke() ?? new RegistrationSorter()).Sort(assemblies);

            foreach (var t in sorted)
            {
                context.Services.Add
                (
                    t.Registration.ServiceType,
                    t.Implementation,
                    ConvertToServiceLifetime(t.Registration.Lifecycle)
                );
            }
        }

        void IInitializableModule.Initialize(InitializationEngine context)
        {            
        }

        void IInitializableModule.Uninitialize(InitializationEngine context)
        {            
        }
    }
}