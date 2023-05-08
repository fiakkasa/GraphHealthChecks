using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace GraphHealthChecks.Tests;

public class GraphQLHealthCheckTests
{
    public class Query
    {
        public string Name => "Hello";
    }

    [Fact(DisplayName = "GraphQLHealthCheck - No Logger - Unhealthy")]
    public async Task UnhealthyNoLogger() =>
        Assert.Equal(
            HealthStatus.Unhealthy,
            (
                await new GraphHealthCheck(
                    new ServiceCollection()
                        .AddGraphQLServer()
                        .AddQueryType()
                        .Services
                        .BuildServiceProvider()
                        .GetRequiredService<IRequestExecutorResolver>()
                )
                .CheckHealthAsync(null!)
            ).Status
        );

    [Fact(DisplayName = "GraphQLHealthCheck - No Logger - Healthy")]
    public async Task HealthyNoLogger() =>
        Assert.Equal(
            HealthStatus.Healthy,
            (
                await new GraphHealthCheck(
                    new ServiceCollection()
                        .AddGraphQLServer()
                        .AddQueryType<Query>()
                        .Services
                        .BuildServiceProvider()
                        .GetRequiredService<IRequestExecutorResolver>()
                )
                .CheckHealthAsync(null!)
            ).Status
        );

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy")]
    public async Task UnhealthyILogger()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();

        Assert.Equal(
            HealthStatus.Unhealthy,
            (
                await new GraphHealthCheck(
                    new ServiceCollection()
                        .AddGraphQLServer()
                        .AddQueryType()
                        .Services
                        .BuildServiceProvider()
                        .GetRequiredService<IRequestExecutorResolver>(),
                    mockLogger.Object
                )
                .CheckHealthAsync(null!)
            ).Status
        );

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<Exception?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Once);
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Healthy")]
    public async Task HealthyILogger()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();

        Assert.Equal(
            HealthStatus.Healthy,
            (
                await new GraphHealthCheck(
                    new ServiceCollection()
                        .AddGraphQLServer()
                        .AddQueryType<Query>()
                        .Services
                        .BuildServiceProvider()
                        .GetRequiredService<IRequestExecutorResolver>(),
                    mockLogger.Object
                )
                .CheckHealthAsync(null!)
            ).Status
        );

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<Exception?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Never);
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogFn - Unhealthy")]
    public async Task UnhealthyILogFn()
    {
        var mockLogger = new Mock<ILogFn>();

        Assert.Equal(
            HealthStatus.Unhealthy,
            (
                await new GraphHealthCheck(
                    new ServiceCollection()
                        .AddGraphQLServer()
                        .AddQueryType()
                        .Services
                        .BuildServiceProvider()
                        .GetRequiredService<IRequestExecutorResolver>(),
                    mockLogger.Object
                )
                .CheckHealthAsync(null!)
            ).Status
        );

        mockLogger.Verify(m => m.LogError(It.IsAny<string?>()), Times.Once);
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogFn - Healthy")]
    public async Task HealthyILogFn()
    {
        var mockLogger = new Mock<ILogFn>();

        Assert.Equal(
            HealthStatus.Healthy,
            (
                await new GraphHealthCheck(
                    new ServiceCollection()
                        .AddGraphQLServer()
                        .AddQueryType<Query>()
                        .Services
                        .BuildServiceProvider()
                        .GetRequiredService<IRequestExecutorResolver>(),
                    mockLogger.Object
                )
                .CheckHealthAsync(null!)
            ).Status
        );

        mockLogger.Verify(m => m.LogError(It.IsAny<string?>()), Times.Never);
    }
}
