using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GraphHealthChecks;

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

    public static IHealthChecksBuilder AddGraphHealthWithLoggerFn(
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
                GraphHealthChecksFactories.GraphHealthCheckFactoryWithLoggerFn(schemaName),
                failureStatus,
                tags,
                timeout
            )
        );
}