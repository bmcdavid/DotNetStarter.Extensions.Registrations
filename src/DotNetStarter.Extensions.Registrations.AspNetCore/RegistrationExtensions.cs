﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetStarter.Abstractions;
using DotNetStarter.Extensions.Registrations.Core;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace DotNetStarter.Extensions.Registrations.AspNetCore
{
    /// <summary>
    /// Usage in Startup class
    /// Add using namespace.of.this
    /// Insert line Startup.ConfigureServices: services.AddDotNetStarterRegistrations();
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Adds classes that contain RegistrationAttribute in given assemblies or assemblies with DiscoverableAssemblyAttribute assembly attribute
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembliesToScan">Assemblies to scan for types with RegistrationAttribute,
        ///      if null Assemblies with DiscoverableAssemblyAttribute are used to filter.</param>
        /// <param name="registrationSorter">Optional custom registration sorter.</param>
        /// <returns></returns>
        public static IServiceCollection AddDotNetStarterRegistrations
        (
            this IServiceCollection services,
            IEnumerable<Assembly> assembliesToScan = null,
            IRegistrationSorter registrationSorter = null
        )
        {
            var assemblies = assembliesToScan ??
                              AssemblyLoader()
                                  .Where(a => a.GetCustomAttribute<DiscoverableAssemblyAttribute>() != null);

            var sorted = (registrationSorter ?? new RegistrationSorter()).Sort(assemblies);
            
            foreach (var t in sorted)
            {
                services.Add
                (
                    new ServiceDescriptor
                    (
                        t.Registration.ServiceType,
                        t.Implementation,
                        ConvertToServiceLifetime(t.Registration.Lifecycle)
                    )
                );
            }

            return services;
        }
        
        /// <summary>
        /// Gets project assemblies from the dependency context
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> AssemblyLoader()
        {
            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var libraries = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId);

            return libraries.Select(x => Assembly.Load(new AssemblyName(x.Name)));
        }

        /// <summary>
        /// Converts DotNetStarter.Lifecycle to ServiceLifetime
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public static ServiceLifetime ConvertToServiceLifetime(Lifecycle lifecycle)
        {
            switch (lifecycle)
            {
                case Lifecycle.Singleton: return ServiceLifetime.Singleton;
                case Lifecycle.Scoped: return ServiceLifetime.Scoped;
                case Lifecycle.Transient: return ServiceLifetime.Transient;
                default: throw new Exception($"Unable to convert {lifecycle} to {typeof(ServiceLifetime).FullName}!");
            }
        }
    }
}