using System;

// todo: move to registrations package

namespace DotNetStarter.Abstractions
{
    /// <summary>
    /// Configuration extensions for generic types
    /// </summary>
    public static class DependencyConfigurationExpressionExtensions
    {
        /// <summary>
        /// Adds service registration for non abstract/interface types
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="serviceType"></param>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public static IDependencyConfigurationExpression Add(this IDependencyConfigurationExpression expression, Type serviceType, Lifecycle lifecycle = Lifecycle.Transient)
        {
            return expression.Add(serviceType, serviceType, lifecycle);
        }

        /// <summary>
        /// Adds scoped service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDependencyConfigurationExpression AddScoped<T>(this IDependencyConfigurationExpression expression) => Add(expression, typeof(T), Lifecycle.Scoped);

        /// <summary>
        /// Adds scoped service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public static IDependencyConfigurationExpression AddScoped<TService, TImplementation>(this IDependencyConfigurationExpression expression) where TImplementation : TService
        {
            return expression.Add(typeof(TService), typeof(TImplementation), Lifecycle.Scoped);
        }

        /// <summary>
        /// Adds singleton service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDependencyConfigurationExpression AddSingleton<T>(this IDependencyConfigurationExpression expression) => Add(expression, typeof(T), Lifecycle.Singleton);

        /// <summary>
        /// Adds singleton service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public static IDependencyConfigurationExpression AddSingleton<TService, TImplementation>(this IDependencyConfigurationExpression expression) where TImplementation : TService
        {
            return expression.Add(typeof(TService), typeof(TImplementation), Lifecycle.Singleton);
        }

        /// <summary>
        /// Adds transient service 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDependencyConfigurationExpression AddTransient<T>(this IDependencyConfigurationExpression expression) => Add(expression, typeof(T));

        /// <summary>
        /// Adds transient service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public static IDependencyConfigurationExpression AddTransient<TService, TImplementation>(this IDependencyConfigurationExpression expression) where TImplementation : TService
        {
            return expression.Add(typeof(TService), typeof(TImplementation));
        }
    }
}