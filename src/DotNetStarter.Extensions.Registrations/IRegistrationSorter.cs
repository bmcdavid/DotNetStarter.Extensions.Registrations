using System.Collections.Generic;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Sorts RegistrationAttribute types from given assemblies
    /// </summary>
    public interface IRegistrationSorter
    {
        /// <summary>
        /// Sorts given registration attributes
        /// </summary>
        void Sort(ICollection<DependentRegistration> dependentRegistrations);
    }
}