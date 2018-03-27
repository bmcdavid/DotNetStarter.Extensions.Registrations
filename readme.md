# DotNetStarter.Extensions.Registrations Read Me

[![Build status](https://ci.appveyor.com/api/projects/status/ub16hwapqm5q6mr0/branch/master?svg=true)](https://ci.appveyor.com/project/bmcdavid/dotnetstarter-extensions-registrations/branch/master)


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

The assembly loading and registration sorting can be customized by setting InitializeDotNetStarterRegistrations.ModuleEnabled equal to false in a PreApplicationStartMethod as show in the optional sample code below:

```cs
using DotNetStarter.Extensions.Registrations.EpiserverCms;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using System.Reflection;

// type and static void method 'Init' to assign GetAssembliesToScan or GetRegistrationSorter
[assembly: System.Web.PreApplicationStartMethod(typeof(Example.StartupClass), nameof(Example.StartupClass.Init))]

namespace Example
{
    /// <summary>
    /// Note: this is an optional example of how to scan custom assemblies or use a different IRegistrationSorter.
    /// </summary>
    public class StartupClass
    {
        /// <summary>
        /// Run pre start code
        /// </summary>
        public static void Init()
        {
            InitializeDotNetStarterRegistrations.ModuleEnabled = false;
        }

    }

    /// <summary>
    /// Example customization for advanced usages
    /// </summary>
    [InitializableModule]
    public class CustomDotNetStarterRegistration : IConfigurableModule
    {
        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            var customAssemblies = new Assembly[]
            {
                typeof(Example.StartupClass).Assembly // look for types in StartupClass assembly
                // can add many more assemblies in this manner
            };

            context.Services.AddDotNetStarterRegistrations
            (
                assembliesToScan: customAssemblies
            );
        }

        void IInitializableModule.Initialize(InitializationEngine context) { }

        void IInitializableModule.Uninitialize(InitializationEngine context) { }
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

The following assembly attributes must be added to the project code, normally in the Properties\AssemblyInfo.cs file where other assembly level information is store. These attributes are used during the scanning process to determine which DLL files to examine for discovering RegistrationAttribute usages.

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