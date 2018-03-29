using DotNetStarter.Abstractions;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Add to classes that want to extend the registrations
    /// <para>IMPORTANT: Classes with this attribute require public static void modifier and accept one argument of type ConfigureExpression</para>
    /// </summary>
    public class DependencyConfigurationAttribute : StartupDependencyBaseAttribute
    {
        /// <summary>
        /// public static void method in class to invoke, must have one argument of type ConfigureExpression
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="methodName">If name isn't given, it defaults to 'Configure'</param>
        public DependencyConfigurationAttribute(string methodName = null) : base()
        {
            MethodName = string.IsNullOrWhiteSpace(methodName) ? "Configure" : methodName;
        }
    }
}