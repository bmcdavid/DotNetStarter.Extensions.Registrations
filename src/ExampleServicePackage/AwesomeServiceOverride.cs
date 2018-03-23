using DotNetStarter.Abstractions;

namespace ExampleServicePackage
{
    /// <summary>
    /// The below attribute demonstrates how service implementations can be overriden.
    /// Note: services may only override each if they derive both use the RegistrationAttribute.
    /// </summary>
    [Registration(typeof(IAwesomeService), Lifecycle.Singleton, typeof(AwsomeServiceImplementation))]
    public class AwsomeServiceImplementationOverride : IAwesomeService
    {
        /// <summary>
        /// Actually does the awesome work
        /// </summary>
        public void DoAwesomeThing()
        {
            // where awesome things happen
        }
    }
}