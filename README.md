# GraphHealthChecks

[![NuGet Version](https://img.shields.io/nuget/v/GraphHealthChecks)](https://www.nuget.org/packages/GraphHealthChecks)
[![NuGet Downloads](https://img.shields.io/nuget/dt/GraphHealthChecks)](https://www.nuget.org/packages/GraphHealthChecks)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/fiakkasa/GraphHealthChecks/blob/master/LICENSE)

Graph Health Checks for HotChocolate.

The purpose of this middleware is to provide feedback regarding the health of the schema.

[Nuget](https://www.nuget.org/packages/GraphHealthChecks/)

## Compatibility

### Current Releases

| HotChocolate Version | GraphHealthChecks Version | .NET Version |
|----------------------|---------------------------|--------------|
| 15.0.3 or higher     | 4.0.1                     | .NET 8, 9    |
| 15.0.3 or higher     | 4.0.0                     | .NET 8, 9    |
| 14.3.0 or higher     | 3.3.0                     | .NET 8, 9    |
| 14.2.0 or higher     | 3.2.0                     | .NET 8, 9    |
| 14.1.0 or higher     | 3.1.0                     | .NET 8, 9    |
| 14.0.0 or higher     | 3.0.0                     | .NET 8       |

### Past Releases

| HotChocolate Version | Last GraphHealthChecks Version | .NET Version |
|----------------------|--------------------------------|--------------|
| 13.7.0 or higher     | 2.0.1                          | .NET 6, 8    |
| 13.3.3 or higher     | 1.0.2                          | .NET 6, 7    |

📝For more information please visit https://www.nuget.org/packages/GraphHealthChecks/#versions-body-tab

## Note

There appears to be a compatibility issue for projects targeting .NET 7 and assembly
`Assembly Microsoft.Extensions.Hosting, Version=7.0.0.0` and more specifically
`OptionsBuilderExtensions.ValidateOnStart` resulting in error:

> The call is ambiguous between the following methods or properties:
> ```csharp
> Microsoft.Extensions.DependencyInjection.OptionsBuilderExtensions.ValidateOnStart<TOptions>(Microsoft.Extensions.Options.OptionsBuilder<TOptions>)
> ```

Until resolved please consider using a version of the package targeting .NET 7.

## Usage

Locate the services registration and append one of:

- `.AddGraphHealthWithNoLogger` - use when no logging is required
- `.AddGraphHealthWithILogger` - use when the ILogger provider is available
- `.AddGraphHealthWithILoggerFactory` - use when the ILoggerFactory provider is available

⚠️ Bear in mind that `IHealthChecksBuilder` needs to be appended before any of the aforementioned.

ex.

```csharp
// Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services
        .AddHealthChecks()
        .AddGraphHealthWithILogger();
    // ...
}
```

📝 If further customization is required, consider wiring up any of the factories manually or use the `GraphHealthCheck`
class itself as required.

If separate schemas are present, multiple registrations can be done for each schema.

ex.

```csharp
// Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services
        .AddHealthChecks()
        .AddGraphHealthWithILogger(healthName: "health1", schemaName: "schema1")
        .AddGraphHealthWithILogger(healthName: "health2", schemaName: "schema2");
    // ...
}
```
