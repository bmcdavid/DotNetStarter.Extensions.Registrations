using DotNetStarter.Abstractions;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Advanced usage for adding types to extend registrations to external dependencies.
    /// <para>IMPORTANT: Classes with this attribute require a method with 'public static void' modifiers and accept one argument of type 'DotNetStarter.Extensions.Registrations.IDependencyConfigurationExpression'</para>
    /// </summary>
    /// <remarks>
    /// When registering dependencies be sure the types include a public constructor (many DI containers select the greediest public constructor by default), avoid injecting types such as strings, numbers (int|long|float|decimal), bools, enums. In those cases, introduce a factory to create the dependency and inject the factory to create the object instance. Also be sure to register any additional dependencies the registering type needs.
    /// </remarks>
    public class DependencyConfigurationAttribute : StartupDependencyBaseAttribute
    {
        /// <summary>
        /// public static void method in class to invoke, must have one argument of type IDependencyConfigurationExpression
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="methodName">Method name to invoke, default value is "Configure"</param>
        public DependencyConfigurationAttribute(string methodName = null)
        {
            MethodName = string.IsNullOrWhiteSpace(methodName) ? "Configure" : methodName;
        }
    }
}