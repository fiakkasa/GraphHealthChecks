using System;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace GraphHealthChecks;

public static class GraphHealthChecksFactories
{
    public static IHealthCheck GraphHealthCheckFactoryWithILogger(IServiceProvider sp) =>
        new GraphHealthCheck(sp.GetRequiredService<IRequestExecutorResolver>(), sp.GetRequiredService<ILogger<GraphHealthCheck>>());

    public static IHealthCheck GraphHealthCheckFactoryWithLoggerFn(IServiceProvider sp) =>
        new GraphHealthCheck(sp.GetRequiredService<IRequestExecutorResolver>(), sp.GetRequiredService<Action<string>>());

    public static IHealthCheck GraphHealthCheckFactory(IServiceProvider sp) =>
        new GraphHealthCheck(sp.GetRequiredService<IRequestExecutorResolver>());
}
