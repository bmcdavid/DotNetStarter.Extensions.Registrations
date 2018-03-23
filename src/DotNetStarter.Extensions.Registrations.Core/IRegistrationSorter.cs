using System.Collections.Generic;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations.Core
{
    /// <summary>
    /// Sorts RegistrationAttribute types from given assemblies
    /// </summary>
    public interface IRegistrationSorter
    {
        /// <summary>
        /// Sorts RegistrationAttribute types from given assemblies
        /// </summary>
        IList<DependentRegistration> Sort(IEnumerable<Assembly> assemblies);
    }
}