using DotNetStarter.Abstractions;

// this assembly attribute instructs the default assembly loaders to only get ones with this attribute
[assembly: DiscoverableAssembly]

// this assembly attribute instructs the type scanning to only look in exported types within the assembly instead of all types
// this will improve performance over ExportsType.All
[assembly: Exports(ExportsType.ExportsOnly)]