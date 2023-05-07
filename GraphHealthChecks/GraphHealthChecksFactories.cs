using System;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace GraphHealthChecks;

public static class GraphHealthChecksFactories
{
    public static Func<IServiceProvider, IHealthCheck> GraphHealthCheckFactoryWithILogger(string? schemaName = default) => (IServiceProvider sp) =>
        new GraphHealthCheck(
            sp.GetRequiredService<IRequestExecutorResolver>(),
            sp.GetRequiredService<ILogger<GraphHealthCheck>>()
        )
        {
            Schema = schemaName
        };

    public static Func<IServiceProvider, IHealthCheck> GraphHealthCheckFactoryWithLoggerFn(string? schemaName = default) => (IServiceProvider sp) =>
        new GraphHealthCheck(
            sp.GetRequiredService<IRequestExecutorResolver>(),
            sp.GetRequiredService<ILogFn>()
        )
        {
            Schema = schemaName
        };

    public static Func<IServiceProvider, IHealthCheck> GraphHealthCheckFactoryWithNoLogger(string? schemaName = default) => (IServiceProvider sp) =>
        new GraphHealthCheck(sp.GetRequiredService<IRequestExecutorResolver>())
        {
            Schema = schemaName
        };
}
