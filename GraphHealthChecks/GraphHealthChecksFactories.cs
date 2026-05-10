namespace GraphHealthChecks;

public static class GraphHealthChecksFactories
{
    public static Func<IServiceProvider, IHealthCheck> GraphHealthCheckFactoryWithNoLogger(string? schemaName = default) => sp =>
        new GraphHealthCheck(sp.GetRequiredService<IRequestExecutorManager>())
        {
            Schema = schemaName
        };

    public static Func<IServiceProvider, IHealthCheck> GraphHealthCheckFactoryWithILogger(string? schemaName = default) => sp =>
        new GraphHealthCheck(
            sp.GetRequiredService<IRequestExecutorManager>(),
            sp.GetRequiredService<ILogger<GraphHealthCheck>>()
        )
        {
            Schema = schemaName
        };

    public static Func<IServiceProvider, IHealthCheck> GraphHealthCheckFactoryWithILoggerFactory(string? schemaName = default) => sp =>
        new GraphHealthCheck(
            sp.GetRequiredService<IRequestExecutorManager>(),
            sp.GetRequiredService<ILoggerFactory>().CreateLogger<GraphHealthCheck>()
        )
        {
            Schema = schemaName
        };
}