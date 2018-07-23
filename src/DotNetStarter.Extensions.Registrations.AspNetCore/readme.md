# DotNetStarter.Extensions.Registrations.AspNetCore

This package removes the startup framework within DotNetStarter but allows all types that utilize the RegistrationAttribute to be added to the AspNetCore IServiceCollection.

Usage in the Startup.ConfigureServices

```cs
// using DotNetStarter.AspNetCore.Registrations;

public void ConfigureServices(IServiceCollection services)
{	
    services.AddDotNetStarterRegistrations();
}
```