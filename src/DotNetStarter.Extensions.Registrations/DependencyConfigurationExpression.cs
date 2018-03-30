using DotNetStarter.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Allows additional dependency configurations
    /// </summary>
    public class DependencyConfigurationExpression : IDependencyConfigurationExpression
    {
        private readonly List<DependentRegistration> _registrations;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="discoveredAssemblies"></param>
        public DependencyConfigurationExpression
        (
            IReadOnlyCollection<Assembly> discoveredAssemblies
        )
        {
            DiscoveredAssemblies = discoveredAssemblies;
            _registrations = new List<DependentRegistration>();
        }

        /// <summary>
        /// Discovered DiscoveredAssemblies
        /// </summary>
        public IEnumerable<Assembly> DiscoveredAssemblies { get; }

        /// <summary>
        /// Adds service and implementation
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="lifecycle"></param>
        /// <returns>Throws exceptions when implementation type is null, an abstract, or an interface.</returns>
        public IDependencyConfigurationExpression Add(Type serviceType, Type implementationType, Lifecycle lifecycle = Lifecycle.Transient)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));
            var impleInfo = implementationType.GetTypeInfo();

            if (impleInfo.IsAbstract || impleInfo.IsInterface)
            {
                throw new ArgumentException($"{nameof(implementationType)} cannot be abstraction or interface type!");
            }

            _registrations
                .Add(new DependentRegistration(implementationType, new RegistrationAttribute(serviceType, lifecycle)));

            return this;
        }

        /// <summary>
        /// Returns gathered dependencies
        /// </summary>
        /// <returns></returns>
        public ICollection<DependentRegistration> Build()
        {
            return _registrations;
        }
    }
}