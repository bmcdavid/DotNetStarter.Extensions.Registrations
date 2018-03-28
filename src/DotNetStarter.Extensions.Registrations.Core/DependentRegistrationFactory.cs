using DotNetStarter.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations.Core
{
    /// <summary>
    /// Creates dependent registrations from given assemblies
    /// </summary>
    public class DependentRegistrationFactory : IDependentRegistrationFactory
    {
        /// <summary>
        /// Creates dependent registrations from given assemblies
        /// </summary>
        public virtual ICollection<DependentRegistration> CreateDependentRegistrations(IEnumerable<Assembly> assemblies)
        {
            var registrations = assemblies
                .SelectMany(GetExportedTypes)
                .SelectMany(ConvertToDependentRegistrations);

            return registrations.ToList();
        }

        /// <summary>
        /// Defines the default behavior when getting an assemblies types, the default is ExportsOnly
        /// </summary>
        protected virtual ExportsType DefaultExportsType => ExportsType.ExportsOnly;

        /// <summary>
        /// Gets types from an assembly, which defaults to all types unless the assembly instructs otherwise with the ExportsAttribute
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        protected virtual IEnumerable<Type> GetExportedTypes(Assembly assembly)
        {
            var exportsType = DefaultExportsType;
            var exportAttribute = assembly.GetCustomAttribute<ExportsAttribute>();

            if (exportAttribute != null)
            {
                exportsType = exportAttribute.ExportsType;
            }

            switch (exportsType)
            {
                case ExportsType.All:
                    return assembly.GetTypes();
                case ExportsType.ExportsOnly:
                    return assembly.ExportedTypes;
                case ExportsType.Specfic:
                    return exportAttribute?.Exports ?? Enumerable.Empty<Type>();
                default:
                    throw new NotSupportedException("Unknown ExportsType of " + exportsType);
            }
        }

        /// <summary>
        /// Extracts RegistrationAttribute from types and returns dependent registrations.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected virtual IEnumerable<DependentRegistration> ConvertToDependentRegistrations(Type t)
        {
            var type = t.GetTypeInfo();
            if (type.IsAbstract || type.IsInterface) return Enumerable.Empty<DependentRegistration>();
            var attrs = type.GetCustomAttributes<RegistrationAttribute>();

            return attrs.Select(x => new DependentRegistration(t, x));
        }
    }
}
