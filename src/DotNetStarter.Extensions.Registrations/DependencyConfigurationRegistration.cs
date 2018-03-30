using System;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Stores external configuration
    /// </summary>
    public class DependencyConfigurationRegistration : AttributeDependentBase<DependencyConfigurationAttribute>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="implementation"></param>
        /// <param name="configurator"></param>
        public DependencyConfigurationRegistration(Type implementation, DependencyConfigurationAttribute configurator) :
            base(implementation, configurator)
        {
        }

        /// <summary>
        /// Attribute with method to invoke
        /// </summary>
        public DependencyConfigurationAttribute Configurator => Attribute;
    }
}