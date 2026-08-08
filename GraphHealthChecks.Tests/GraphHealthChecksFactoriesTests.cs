using Microsoft.Extensions.DependencyInjection;

namespace GraphHealthChecks.Tests;

public class GraphHealthChecksFactoriesTests
{
    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryWithNoLogger - Custom Schema")]
    public void GraphHealthCheckFactoryWithNoLoggerCustomSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryWithNoLogger("schema")(
            new ServiceCollection()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Equal("schema", (result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryWithNoLogger - Default Schema")]
    public void GraphHealthCheckFactoryWithNoLoggerDefaultSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryWithNoLogger()(
            new ServiceCollection()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Null((result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryWithILogger - Custom Schema")]
    public void GraphHealthCheckFactoryWithILoggerCustomSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryWithILogger("schema")(
            new ServiceCollection()
                .AddLogging()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Equal("schema", (result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryWithILogger - Default Schema")]
    public void GraphHealthCheckFactoryWithILoggerDefaultSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryWithILogger()(
            new ServiceCollection()
                .AddLogging()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Null((result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryWithLoggerFn - Custom Schema")]
    public void GraphHealthCheckFactoryWithLoggerFnCustomSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryWithILoggerFactory("schema")(
            new ServiceCollection()
                .AddLogging()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Equal("schema", (result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryWithLoggerFn - Default Schema")]
    public void GraphHealthCheckFactoryWithLoggerFnDefaultSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryWithILoggerFactory()(
            new ServiceCollection()
                .AddLogging()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Null((result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryUsingProviderWithNoLogger - Custom Schema")]
    public void GraphHealthCheckFactoryUsingProviderWithNoLoggerCustomSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryUsingProviderWithNoLogger("schema")(
            new ServiceCollection()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Equal("schema", (result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryUsingProviderWithNoLogger - Default Schema")]
    public void GraphHealthCheckFactoryUsingProviderWithNoLoggerDefaultSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryUsingProviderWithNoLogger()(
            new ServiceCollection()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Null((result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryUsingProviderWithILogger - Custom Schema")]
    public void GraphHealthCheckFactoryUsingProviderWithILoggerCustomSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryUsingProviderWithILogger("schema")(
            new ServiceCollection()
                .AddLogging()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Equal("schema", (result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryUsingProviderWithILogger - Default Schema")]
    public void GraphHealthCheckFactoryUsingProviderWithILoggerDefaultSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryUsingProviderWithILogger()(
            new ServiceCollection()
                .AddLogging()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Null((result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryUsingProviderWithLoggerFn - Custom Schema")]
    public void GraphHealthCheckFactoryUsingProviderWithLoggerFnCustomSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryUsingProviderWithILoggerFactory("schema")(
            new ServiceCollection()
                .AddLogging()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Equal("schema", (result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryUsingProviderWithLoggerFn - Default Schema")]
    public void GraphHealthCheckFactoryUsingProviderWithLoggerFnDefaultSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryUsingProviderWithILoggerFactory()(
            new ServiceCollection()
                .AddLogging()
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsType<GraphHealthCheck>(result, exactMatch: false);
        Assert.Null((result as GraphHealthCheck)?.Schema);
    }

    public class Query
    {
        public string Name => "Hello";
    }
}