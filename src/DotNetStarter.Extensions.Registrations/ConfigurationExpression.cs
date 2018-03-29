using DotNetStarter.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Allows additional dependency configurations
    /// </summary>
    public class ConfigurationExpression
    {
        private List<DependentRegistration> _registrations;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assemblies"></param>
        public ConfigurationExpression(IEnumerable<Assembly> assemblies)
        {
            Assemblies = assemblies;
            _registrations = new List<DependentRegistration>();
        }

        /// <summary>
        /// Assemblies used in discovery
        /// </summary>
        public IEnumerable<Assembly> Assemblies { get; }

        /// <summary>
        /// Adds service registration for non abstract/interface types
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public ConfigurationExpression Add(Type serviceType, Lifecycle lifecycle = Lifecycle.Transient)
        {
            return Add(serviceType, serviceType, lifecycle);
        }

        /// <summary>
        /// Adds service and implementation
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="lifecycle"></param>
        /// <returns>Throws exceptions when implementation type is null, an abstract, or an interface.</returns>
        public ConfigurationExpression Add(Type serviceType, Type implementationType, Lifecycle lifecycle = Lifecycle.Transient)
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
        /// Removes service type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public ConfigurationExpression Remove(Type serviceType)
        {
            _registrations.RemoveAll(x => x.Registration.ServiceType == serviceType);

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
