# GraphHealthChecks

Graph Health Checks for HotChocolate

The purpose of this middleware is to provide feedback in regards to the health of the schema.

## Usage

Locate the services registration and append one of:

- `.AddGraphHealthWithNoLogger` - use when no logging is required
- `.AddGraphHealthWithILogger` - use when the ILogger provider is available
- `.AddGraphHealthWithLoggerFn` - use when a custom logger is required, in which case the `ILogFn` interface will need to be implemented and present in the DI.

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

📝 If further customization is required, consider wiring up any of the factories manually or use the `GraphHealthCheck` class itself as required.

If separate schemas are present, multiple registrations can be done for each schema.

ex.

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
