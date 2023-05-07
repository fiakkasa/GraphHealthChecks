using Microsoft.Extensions.DependencyInjection;

namespace GraphHealthChecks;

public static class GraphHealthExtensions
{
    private const string _defaultHealthName = "Graph Health";

    public static IHealthChecksBuilder AddGraphHealth(this IHealthChecksBuilder builder, string healthName = _defaultHealthName) =>
        builder.Add(
            new(
                healthName,
                GraphHealthChecksFactories.GraphHealthCheckFactory,
                default,
                default,
                default
            )
        );

    public static IHealthChecksBuilder AddGraphHealthWithILogger(this IHealthChecksBuilder builder, string healthName = _defaultHealthName) =>
        builder.Add(
            new(
                healthName,
                GraphHealthChecksFactories.GraphHealthCheckFactoryWithILogger,
                default,
                default,
                default
            )
        );

    public static IHealthChecksBuilder AddGraphHealthWithLoggerFn(this IHealthChecksBuilder builder, string healthName = _defaultHealthName) =>
        builder.Add(
            new(
                healthName,
                GraphHealthChecksFactories.GraphHealthCheckFactoryWithLoggerFn,
                default,
                default,
                default
            )
        );
}