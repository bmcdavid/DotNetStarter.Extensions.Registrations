using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetStarter.Abstractions;

namespace DotNetStarter.Extensions.Registrations.Core
{
    /// <summary>
    /// Default dependency sorter
    /// </summary>
    public class RegistrationSorter : IRegistrationSorter
    {
        private readonly IComparer<DependentRegistration> _dependencyComparer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dependencyComparer"></param>
        public RegistrationSorter(IComparer<DependentRegistration> dependencyComparer = null)
        {
            _dependencyComparer = dependencyComparer ?? new DependentRegistrationComparer();
        }

        /// <summary>
        /// Defines the default behavior when getting an assemblies types, the default is ExportsOnly
        /// </summary>
        protected virtual ExportsType DefaultExportsType => ExportsType.ExportsOnly;

        /// <summary>
        /// Default sorter or Types or Assemblies
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public virtual IList<DependentRegistration> Sort(IEnumerable<Assembly> assemblies)
        {
            var registrations = assemblies
                .SelectMany(GetExportedTypes)
                .SelectMany(ConvertToDependentRegistrations);

            var unresolved = registrations.OrderBy(x => x, _dependencyComparer)
                .SkipWhile(x => x.Registration == null)
                .ToList();

            var resolved = new List<DependentRegistration>();
            var hashSet = new HashSet<object>();
            var count = unresolved.Count;
            var index = 0;

            while (unresolved.Count > 0)
            {
                var dependentRegistration = unresolved[index];

                if (hashSet.IsSupersetOf(dependentRegistration.Registration.Dependencies))
                {
                    resolved.Add(dependentRegistration);
                    hashSet.Add(dependentRegistration.Implementation);
                    unresolved.RemoveAt(index--);
                }

                if (++index < unresolved.Count) continue;

                if (count == unresolved.Count)
                {
                    var names = string.Join(Environment.NewLine, unresolved.Select(x => x.Implementation.FullName));
                    throw new InvalidOperationException($"Cannot resolve registrations for the following: {names}, please check their dependencies!");
                }

                index = 0;
                count = unresolved.Count;
            }

            return resolved;
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
    }
}