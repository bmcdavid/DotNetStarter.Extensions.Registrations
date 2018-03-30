// todo: move to registrations package
// todo: add documentation about types that make good candidates, ie classes with no primitive arguments (string, int, bool, enum, etc)
//  at least within the greediest constructor
namespace DotNetStarter.Abstractions
{
    /// <summary>
    /// Add to classes that want to extend the registrations
    /// <para>IMPORTANT: Classes with this attribute require public static void modifier and accept one argument of type IDependencyConfigurationExpression</para>
    /// </summary>
    public class DependencyConfigurationAttribute : StartupDependencyBaseAttribute
    {
        /// <summary>
        /// public static void method in class to invoke, must have one argument of type IDependencyConfigurationExpression
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="methodName">If name isn't given, it defaults to 'Configure'</param>
        public DependencyConfigurationAttribute(string methodName = null)
        {
            MethodName = string.IsNullOrWhiteSpace(methodName) ? "Configure" : methodName;
        }
    }
}