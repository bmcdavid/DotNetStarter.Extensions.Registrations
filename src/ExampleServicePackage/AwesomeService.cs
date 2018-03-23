using DotNetStarter.Abstractions;

namespace ExampleServicePackage
{
    /// <summary>
    /// Actually provides the awesome work
    /// </summary>
    // this attribute marks AwsomeServiceImplementation as an implementation of IAwesomeService for dependency inject.
    [Registration(typeof(IAwesomeService), Lifecycle.Singleton)]
    public class AwsomeServiceImplementation : IAwesomeService
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