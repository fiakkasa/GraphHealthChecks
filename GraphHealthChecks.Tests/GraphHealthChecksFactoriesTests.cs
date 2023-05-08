using Microsoft.Extensions.DependencyInjection;

namespace GraphHealthChecks.Tests;

public class GraphHealthChecksFactoriesTests
{
    public class Query
    {
        public string Name => "Hello";
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

        Assert.IsAssignableFrom<GraphHealthCheck>(result);
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

        Assert.IsAssignableFrom<GraphHealthCheck>(result);
        Assert.Null((result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryWithLoggerFn - Custom Schema")]
    public void GraphHealthCheckFactoryWithLoggerFnCustomSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryWithLoggerFn("schema")(
            new ServiceCollection()
                .AddSingleton(new Mock<ILogFn>().Object)
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsAssignableFrom<GraphHealthCheck>(result);
        Assert.Equal("schema", (result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryWithLoggerFn - Default Schema")]
    public void GraphHealthCheckFactoryWithLoggerFnDefaultSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryWithLoggerFn()(
            new ServiceCollection()
                .AddSingleton(new Mock<ILogFn>().Object)
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsAssignableFrom<GraphHealthCheck>(result);
        Assert.Null((result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryWithNoLogger - Custom Schema")]
    public void GraphHealthCheckFactoryWithNoLoggerCustomSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryWithNoLogger("schema")(
            new ServiceCollection()
                .AddSingleton(new Mock<ILogFn>().Object)
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsAssignableFrom<GraphHealthCheck>(result);
        Assert.Equal("schema", (result as GraphHealthCheck)?.Schema);
    }

    [Fact(DisplayName = "GraphHealthChecksFactories - GraphHealthCheckFactoryWithNoLogger - Default Schema")]
    public void GraphHealthCheckFactoryWithNoLoggerDefaultSchema()
    {
        var result = GraphHealthChecksFactories.GraphHealthCheckFactoryWithNoLogger()(
            new ServiceCollection()
                .AddSingleton(new Mock<ILogFn>().Object)
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .Services
                .BuildServiceProvider()
        );

        Assert.IsAssignableFrom<GraphHealthCheck>(result);
        Assert.Null((result as GraphHealthCheck)?.Schema);
    }
}
