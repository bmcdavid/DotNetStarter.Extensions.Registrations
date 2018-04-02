using DotNetStarter.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Allows additional dependency configurations
    /// </summary>
    public interface IDependencyConfigurationExpression
    {
        /// <summary>
        /// Discovered Assemblies
        /// </summary>
        IEnumerable<Assembly> DiscoveredAssemblies { get; }

        /// <summary>
        /// Adds a dependency for registration, generics registration are available via extensions in the DotNetStarter.Extensions.Registrations namespace.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        IDependencyConfigurationExpression Add(Type serviceType, Type implementationType, Lifecycle lifecycle = Lifecycle.Transient);
    }
}