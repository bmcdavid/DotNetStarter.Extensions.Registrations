using System;
using System.Collections.Generic;
using System.Reflection;

//todo: move to registrations package

namespace DotNetStarter.Abstractions
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
        /// Adds a dependency for registration
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        IDependencyConfigurationExpression Add(Type serviceType, Type implementationType, Lifecycle lifecycle = Lifecycle.Transient);
    }
}