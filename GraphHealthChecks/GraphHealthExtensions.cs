﻿namespace GraphHealthChecks;

public static class GraphHealthExtensions
{
    private const string _defaultHealthName = "Graph Health";

    public static IHealthChecksBuilder AddGraphHealthWithNoLogger(
        this IHealthChecksBuilder builder,
        string healthName = _defaultHealthName,
        string? schemaName = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default
    ) =>
        builder.Add(
            new(
                healthName,
                GraphHealthChecksFactories.GraphHealthCheckFactoryWithNoLogger(schemaName),
                failureStatus,
                tags,
                timeout
            )
        );

    public static IHealthChecksBuilder AddGraphHealthWithILogger(
        this IHealthChecksBuilder builder,
        string healthName = _defaultHealthName,
        string? schemaName = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default
    ) =>
        builder.Add(
            new(
                healthName,
                GraphHealthChecksFactories.GraphHealthCheckFactoryWithILogger(schemaName),
                failureStatus,
                tags,
                timeout
            )
        );

    public static IHealthChecksBuilder AddGraphHealthWithILoggerFactory(
        this IHealthChecksBuilder builder,
        string healthName = _defaultHealthName,
        string? schemaName = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default
    ) =>
        builder.Add(
            new(
                healthName,
                GraphHealthChecksFactories.GraphHealthCheckFactoryWithILoggerFactory(schemaName),
                failureStatus,
                tags,
                timeout
            )
        );
}
