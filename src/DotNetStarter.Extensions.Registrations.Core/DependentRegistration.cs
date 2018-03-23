using System;
using DotNetStarter.Abstractions;

namespace DotNetStarter.Extensions.Registrations.Core
{
    /// <summary>
    /// Stores registration and implementation for sorting and service registration
    /// </summary>
    public class DependentRegistration
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="implementation"></param>
        /// <param name="registration"></param>
        public DependentRegistration(Type implementation, RegistrationAttribute registration)
        {
            Implementation = implementation;
            Registration = registration;
        }

        /// <summary>
        /// Type with Registration Attribute
        /// </summary>
        public Type Implementation { get; }

        /// <summary>
        /// Attribute instance
        /// </summary>
        public RegistrationAttribute Registration { get; }
    }
}