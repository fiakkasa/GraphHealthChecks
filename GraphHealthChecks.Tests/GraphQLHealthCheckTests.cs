﻿using HotChocolate.Authorization;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Snapshooter.Xunit;

namespace GraphHealthChecks.Tests;

public class GraphQLHealthCheckTests
{
    private static object GetErrorResult(HealthCheckResult result) =>
        new
        {
            result.Status,
            result.Description,
            result.Data,
            Exception = new
            {
                result.Exception?.Message
            }
        };

    [Fact(DisplayName = "GraphQLHealthCheck - No Logger - Unhealthy Schema")]
    public async Task NoLoggerUnhealthySchema()
    {
        var mockRequestExecutorResolver = new Mock<IRequestExecutorResolver>();
        mockRequestExecutorResolver
            .Setup(m => m.GetRequestExecutorAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new SchemaException());

        var result = await new GraphHealthCheck(mockRequestExecutorResolver.Object)
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsAssignableFrom<SchemaException>(result.Exception);

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - No Logger - General Error")]
    public async Task NoLoggerGeneralError()
    {
        var mockRequestExecutorResolver = new Mock<IRequestExecutorResolver>();
        mockRequestExecutorResolver
            .Setup(m => m.GetRequestExecutorAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Splash!"));

        var result = await new GraphHealthCheck(mockRequestExecutorResolver.Object).CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsAssignableFrom<Exception>(result.Exception);

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - No Logger - Healthy")]
    public async Task NoLoggerHealthy()
    {
        var result = await new GraphHealthCheck(
                new ServiceCollection()
                    .AddGraphQLServer()
                    .AddQueryType<Query>()
                    .Services
                    .BuildServiceProvider()
                    .GetRequiredService<IRequestExecutorResolver>()
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Null(result.Description);

        result.MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - General Error")]
    public async Task ILoggerGeneralError()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();
        var mockResolver = new Mock<IRequestExecutorResolver>();
        mockResolver
            .Setup(m => m.GetRequestExecutorAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Splash!"));

        var result = await new GraphHealthCheck(
                mockResolver.Object,
                mockLogger.Object
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsAssignableFrom<Exception>(result.Exception);
        Assert.NotNull(result.Description);
        Assert.Contains("general", result.Description);

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<Exception?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Once);

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Schema")]
    public async Task ILoggerUnhealthySchema()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();

        var result = await new GraphHealthCheck(
                new ServiceCollection()
                    .AddGraphQLServer()
                    .AddQueryType()
                    .Services
                    .BuildServiceProvider()
                    .GetRequiredService<IRequestExecutorResolver>(),
                mockLogger.Object
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsAssignableFrom<SchemaException>(result.Exception);
        Assert.NotNull(result.Description);
        Assert.Contains("schema", result.Description);

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<SchemaException?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Once);

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Schema - No Auth")]
    public async Task ILoggerUnhealthySchemaNoAuth()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();

        var result = await new GraphHealthCheck(
                new ServiceCollection()
                    .AddGraphQLServer()
                    .AddQueryType<QueryAuth>()
                    .Services
                    .BuildServiceProvider()
                    .GetRequiredService<IRequestExecutorResolver>(),
                mockLogger.Object
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsAssignableFrom<SchemaException>(result.Exception);
        Assert.NotNull(result.Description);
        Assert.Contains("schema", result.Description);

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<SchemaException?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Once);

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Schema - No Auth - General Error")]
    public async Task ILoggerUnhealthySchemaNoAuthGeneralError()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();
        var mockRequestExecutorResolver = new Mock<IRequestExecutorResolver>();
        mockRequestExecutorResolver
            .Setup(m => m.GetRequestExecutorAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Splash!"));

        var result = await new GraphHealthCheck(mockRequestExecutorResolver.Object, mockLogger.Object)
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsAssignableFrom<Exception>(result.Exception);

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<Exception?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Once);

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Healthy - Auth")]
    public async Task ILoggerHealthyAuth()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();

        var result = await new GraphHealthCheck(
                new ServiceCollection()
                    .AddGraphQLServer()
                    .AddAuthorization()
                    .AddQueryType<QueryAuth>()
                    .Services
                    .BuildServiceProvider()
                    .GetRequiredService<IRequestExecutorResolver>(),
                mockLogger.Object
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Null(result.Description);

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<SchemaException?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Never);

        result.MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Healthy")]
    public async Task ILoggerHealthy()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();

        var result = await new GraphHealthCheck(
                new ServiceCollection()
                    .AddGraphQLServer()
                    .AddQueryType<Query>()
                    .Services
                    .BuildServiceProvider()
                    .GetRequiredService<IRequestExecutorResolver>(),
                mockLogger.Object
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Null(result.Description);

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<SchemaException?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Never);

        result.MatchSnapshot();
    }

    public class Query
    {
        public string Name => "Hello";
    }

    public class QueryAuth
    {
        [Authorize]
        public string Name => "Hello";
    }
}
