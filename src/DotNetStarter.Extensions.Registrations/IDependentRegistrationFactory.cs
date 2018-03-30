using System.Collections.Generic;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Creates DependentRegistrations from given discoveredAssemblies
    /// </summary>
    public interface IDependentRegistrationFactory
    {
        /// <summary>
        /// Creates DependentRegistrations from given discoveredAssemblies
        /// </summary>
        ICollection<DependentRegistration> CreateDependentRegistrations(IEnumerable<Assembly> assemblies);
    }
}