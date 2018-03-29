using DotNetStarter.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Creates dependent registrations from given assemblies
    /// </summary>
    public class DependentRegistrationFactory : IDependentRegistrationFactory
    {
        /// <summary>
        /// Defines the default behavior when getting an assemblies types, the default is ExportsOnly
        /// </summary>
        protected virtual ExportsType DefaultExportsType => ExportsType.ExportsOnly;

        /// <summary>
        /// Creates dependent registrations from given assemblies
        /// </summary>
        public virtual ICollection<DependentRegistration> CreateDependentRegistrations(IEnumerable<Assembly> assemblies)
        {
            var registrations = assemblies
                .SelectMany(GetExportedTypes)
                .SelectMany(ConvertToDependentRegistrations)
                .ToList();

            var configureExpression = new ConfigurationExpression(assemblies);
            var externals = BuildExternalRegistrations(registrations, configureExpression);

            return registrations
                .OfType<DependentRegistration>()
                .Union(externals)
                .ToList();
        }

        /// <summary>
        /// Builds a configurationexression to discover external dependencies
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="configurationExpression"></param>
        /// <returns></returns>
        protected virtual IEnumerable<DependentRegistration> BuildExternalRegistrations(ICollection<AttributeDependentBase> attributes,
            ConfigurationExpression configurationExpression)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

            foreach (var external in attributes.OfType<DependencyConfigurationRegistration>())
            {
                try
                {
                    MethodInfo info = external.Implementation.GetTypeInfo()
                        .GetMethod(external.Configurator.MethodName, flags);

                    info.Invoke(null, new object[] { configurationExpression });
                }
                catch (Exception e)
                {
                    ExceptionCollector.AddException(new Exception($"Failed to configure {external.Implementation.FullName}!", e));
                }
            }

            return configurationExpression.Build();
        }

        /// <summary>
        /// Extracts RegistrationAttribute from types and returns dependent registrations.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected virtual IEnumerable<AttributeDependentBase> ConvertToDependentRegistrations(Type t)
        {
            var type = t.GetTypeInfo();
            if (type.IsAbstract || type.IsInterface) return Enumerable.Empty<DependentRegistration>();
            var attrs = type.GetCustomAttributes<StartupDependencyBaseAttribute>(true);
            List<AttributeDependentBase> attrList = new List<AttributeDependentBase>();
            foreach (var attr in attrs)
            {
                //todo: refactor if another case is needed
                if (attr is RegistrationAttribute registration)
                {
                    attrList.Add(new DependentRegistration(t, registration));
                }
                else if (attr is DependencyConfigurationAttribute externalDependencyRegistration)
                {
                    attrList.Add(new DependencyConfigurationRegistration(t, externalDependencyRegistration));
                }
            }

            return attrList;
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
