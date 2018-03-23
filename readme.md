# DotNetStarter.Extensions.Registrations Read Me

This project provides extensions for including packages utilizing DotNetStarter.RegistrationAbstractions into AspNetCore or Episerver Cms projects. This package removes the need for the full DotNetStarter framework, only focusing on adding registrations for dependency injection.

## Creating services

To create an injectable service in a project or NuGet package, add a reference to DotNetStarter.RegistrationAbstractions. Then create a service abstraction, example below:

```cs
using DotNetStarter.Abstractions;

// this assembly attribute instructs the default assembly loaders to only get ones with this attribute
[assembly: DiscoverableAssembly]

// this assembly attribute instructs the type scanning to only look in exported types within the assembly instead of all types
// this will improve performance over ExportsType.All
[assembly: Exports(ExportsType.ExportsOnly)]

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

## AspNetCore Service Wireup

Usage in the Startup.ConfigureServices

```cs
// using DotNetStarter.Extensions.Registrations.AspNetCore;

public void ConfigureServices(IServiceCollection services)
{	
    services.AddDotNetStarterRegistrations();
}
```

## Episerver Cms Wireup

Done automatically during Episerver initialization using the InitializeDotNetStarterRegistrations class. The assembly loading and registration sorting can be customized by setting InitializeDotNetStarterRegistrations.GetAssembliesToScan or InitializeDotNetStarterRegistrations.GetRegistrationSorter in a PreApplicationStartMethod.

```cs
// type and static void method 'Init' to assign GetAssembliesToScan or GetRegistrationSorter
[assembly: System.Web.PreApplicationStartMethod(typeof(StartupClass), "Init")] 
```