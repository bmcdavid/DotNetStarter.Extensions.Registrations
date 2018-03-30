using DotNetStarter.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Allows additional dependency configurations
    /// </summary>
    public class DependencyConfigurationExpression
    {
        private readonly List<DependentRegistration> _registrations;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="discoveredAssemblies"></param>
        /// <param name="discoveredRegistrations"></param>
        public DependencyConfigurationExpression
        (
            IReadOnlyCollection<Assembly> discoveredAssemblies,
            IReadOnlyCollection<DependentRegistration> discoveredRegistrations
        )
        {
            DiscoveredAssemblies = discoveredAssemblies;
            Registrations = discoveredRegistrations;
            _registrations = new List<DependentRegistration>();
        }

        /// <summary>
        /// Discovered DiscoveredAssemblies
        /// </summary>
        public IReadOnlyCollection<Assembly> DiscoveredAssemblies { get; }

        /// <summary>
        /// Discovered Registrations
        /// </summary>
        public IReadOnlyCollection<DependentRegistration> Registrations { get; }

        /// <summary>
        /// Adds service registration for non abstract/interface types
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public DependencyConfigurationExpression Add(Type serviceType, Lifecycle lifecycle = Lifecycle.Transient)
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
        public DependencyConfigurationExpression Add(Type serviceType, Type implementationType, Lifecycle lifecycle = Lifecycle.Transient)
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
        /// Adds scoped service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DependencyConfigurationExpression AddScoped<T>() => Add(typeof(T), Lifecycle.Scoped);

        /// <summary>
        /// Adds scoped service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public DependencyConfigurationExpression AddScoped<TService, TImplementation>() where TImplementation : TService =>
            Add(typeof(TService), typeof(TImplementation), Lifecycle.Scoped);

        /// <summary>
        /// Adds singleton service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DependencyConfigurationExpression AddSingleton<T>() => Add(typeof(T), Lifecycle.Singleton);

        /// <summary>
        /// Adds singleton service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public DependencyConfigurationExpression AddSingleton<TService, TImplementation>() where TImplementation : TService =>
            Add(typeof(TService), typeof(TImplementation), Lifecycle.Singleton);

        /// <summary>
        /// Adds transient service 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DependencyConfigurationExpression AddTransient<T>() => Add(typeof(T));

        /// <summary>
        /// Adds transient service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public DependencyConfigurationExpression AddTransient<TService,TImplementation>() where TImplementation : TService =>
            Add(typeof(TService), typeof(TImplementation));
        
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