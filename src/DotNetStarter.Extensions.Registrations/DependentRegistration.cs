using System;
using DotNetStarter.Abstractions;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Stores registration and implementation for sorting and service registration
    /// </summary>
    public class DependentRegistration: AttributeDependentBase<RegistrationAttribute>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="implementation"></param>
        /// <param name="registration"></param>
        public DependentRegistration(Type implementation, RegistrationAttribute registration) :
            base(implementation, registration)
        {
        }

        /// <summary>
        /// Attribute instance
        /// </summary>
        public RegistrationAttribute Registration => Attribute;

        /// <summary>
        /// Allows for custom lifecycle instead of attribute's selection
        /// </summary>
        public Lifecycle? CustomLifeCycle { get; set; }
    }
}