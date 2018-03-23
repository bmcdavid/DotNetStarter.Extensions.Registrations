# DotNetStarter.Extensions.Registrations Read Me

The extensions provided in this project focus only on the dependency injection component of [DotNetStarter](https://bmcdavid.github.io/DotNetStarter/). If the full DotNetStarter package is installed, these extensions should not be used. These extensions provide service registration to AspNetCore's dependency injection or Episerver's service configuration utilizing the DotNetStarter.RegistrationAttribute attribute on class types that implement abstract services.

## AspNetCore Service Wireup

To add registrations to AspNetCore applications, simply use the provided AddDotNetStarterRegistrations() extension to the service collection in the application startup class.

```cs
// using DotNetStarter.Extensions.Registrations.AspNetCore;

public void ConfigureServices(IServiceCollection services)
{	
    services.AddDotNetStarterRegistrations();
}
```

## Episerver Cms Wireup

By default, the Episerver extension requires no additional work other than installing its Nuget Package of DotNetStarter.Extensions.Registrations.EpiserverCms

The assembly loading and registration sorting can be customized by setting InitializeDotNetStarterRegistrations.GetAssembliesToScan or InitializeDotNetStarterRegistrations.GetRegistrationSorter in a PreApplicationStartMethod as show in the optional sample code below:

```cs
using System.Collections.Generic;
using System.Reflection;
using DotNetStarter.Abstractions;
using DotNetStarter.Extensions.Registrations.Core;
using DotNetStarter.Extensions.Registrations.EpiserverCms;

// type and static void method 'Init' to assign GetAssembliesToScan or GetRegistrationSorter
[assembly: System.Web.PreApplicationStartMethod(typeof(Example.StartupClass), "Init")]

namespace Example
{
    /// <summary>
    /// Note: this is an optional example of how to scan custom assemblies or use a different IRegistrationSorter.
    /// </summary>
    public class StartupClass
    {

        public static void Init()
        {
            // area to perform customization before Episerver initialization

            // use manual assembly list
            InitializeDotNetStarterRegistrations.GetAssembliesToScan = CustomizedAssemblies;

            // changes default behavior to export all types
            InitializeDotNetStarterRegistrations.GetRegistrationSorter = () => new CustomRegistrationSorter();
        }

        private static IEnumerable<Assembly> CustomizedAssemblies()
        {
            // typeof(Namespace.Class).Assembly is only needed once per assembly, do not add one for every class in the assembly.
            return new []
            {
                typeof(Example.StartupClass).Assembly, // look for classes using RegistrationAttribute in this dll
                // can add many more assemblies
            };
        }
    }

    public class CustomRegistrationSorter : RegistrationSorter
    {
        public CustomRegistrationSorter(): base(new DependentRegistrationComparer()){ }

        /// <summary>
        /// Changes default behavior to export all assembly types when Exports assembly attribute isn't included
        /// </summary>
        protected override ExportsType DefaultExportsType { get; } = ExportsType.All;
    }
}
```

## Example Service

To create an injectable service in a project or NuGet package, add a reference to DotNetStarter.RegistrationAbstractions. Then create a service abstraction, example below:

```cs
using DotNetStarter.Abstractions;

namespace ExampleNamespace
{

    public interface IAwesomeService
    {
        void DoAwesomeThing();
    }

    // this attribute marks AwsomeServiceImplementation as an implementation of IAwesomeService for dependency inject.
    [Registration(typeof(IAwesomeService), Lifecycle.Singleton)]
    public class AwsomeServiceImplementation : IAwesomeService
    {
        public void DoAwesomeThing()
        {
            // where awesome things happen
        }
    }
}
```

To opt into the default registration extensions discovery process, the following attributes must be added to the project code, normally in the Properties\AssemblyInfo.cs file where other assembly level information is store.

```cs
using DotNetStarter.Abstractions;

// this assembly attribute instructs the default assembly loaders to only get ones with this attribute
[assembly: DiscoverableAssembly]

// this assembly attribute instructs the type scanning to only look in exported types within the assembly instead of all types
// this will improve performance over using ExportsType.All
[assembly: Exports(ExportsType.ExportsOnly)]
```

## Example Project

A short [example project](https://github.com/bmcdavid/DotNetStarter.Extensions.Registrations/tree/master/src/ExampleServicePackage) is also provided in the source code which targets multiple frameworks, demonstrated in the csproj file.