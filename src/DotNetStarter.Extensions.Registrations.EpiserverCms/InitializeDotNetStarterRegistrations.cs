using System;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace DotNetStarter.Extensions.Registrations.EpiserverCms
{
    /// <summary>
    /// Scans ass
    /// </summary>
    [InitializableModule]
    public class InitializeDotNetStarterRegistrations : IConfigurableModule
    {
        /// <summary>
        /// Determines if service needs a service accessor registration for Inject&lt;T> types.
        /// </summary>
        public static Func<IRegisteredService, bool> AddServiceAccessor { get; set; }

        /// <summary>
        /// Global switch to disable default module for advanced configurations
        /// </summary>
        public static bool ModuleEnabled { get; set; } = true;

        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            if (ModuleEnabled)
            {
                context.Services.AddDotNetStarterRegistrations(addServiceAccessor: AddServiceAccessor);
            }
        }

        void IInitializableModule.Initialize(InitializationEngine context)
        {
        }

        void IInitializableModule.Uninitialize(InitializationEngine context)
        {
        }
    }
}