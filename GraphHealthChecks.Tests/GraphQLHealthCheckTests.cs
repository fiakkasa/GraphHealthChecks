using HotChocolate.Authorization;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

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
        var mockRequestExecutorResolver = Substitute.For<IRequestExecutorManager>();
        mockRequestExecutorResolver
            .GetExecutorAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Throws(new SchemaException());

        var result = await new GraphHealthCheck(mockRequestExecutorResolver)
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsType<SchemaException>(result.Exception, exactMatch: false);

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - No Logger - General Error")]
    public async Task NoLoggerGeneralError()
    {
        var mockRequestExecutorResolver = Substitute.For<IRequestExecutorManager>();
        mockRequestExecutorResolver
            .GetExecutorAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Splash!"));

        var result = await new GraphHealthCheck(mockRequestExecutorResolver).CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsType<Exception>(result.Exception, exactMatch: false);

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
                    .GetRequiredService<IRequestExecutorManager>()
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Null(result.Description);

        result.MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - General Error")]
    public async Task ILoggerGeneralError()
    {
        var mockLogger = Substitute.For<ILogger<GraphHealthCheck>>();
        var mockResolver = Substitute.For<IRequestExecutorManager>();
        mockResolver
            .GetExecutorAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Splash!"));

        var result = await new GraphHealthCheck(
                mockResolver,
                mockLogger
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsType<Exception>(result.Exception, exactMatch: false);
        Assert.NotNull(result.Description);
        Assert.Contains("general", result.Description);

        mockLogger
            .Received(1)
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Is<Exception>(x => x.Message == "Splash!"),
                Arg.Any<Func<object, Exception?, string>>()
            );

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Schema")]
    public async Task ILoggerUnhealthySchema()
    {
        var mockLogger = Substitute.For<ILogger<GraphHealthCheck>>();

        var result = await new GraphHealthCheck(
                new ServiceCollection()
                    .AddGraphQLServer()
                    .AddQueryType()
                    .Services
                    .BuildServiceProvider()
                    .GetRequiredService<IRequestExecutorManager>(),
                mockLogger
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsType<SchemaException>(result.Exception, exactMatch: false);
        Assert.NotNull(result.Description);
        Assert.Contains("schema", result.Description);

        mockLogger
            .Received(1)
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<SchemaException>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Schema - No Auth")]
    public async Task ILoggerUnhealthySchemaNoAuth()
    {
        var mockLogger = Substitute.For<ILogger<GraphHealthCheck>>();

        var result = await new GraphHealthCheck(
                new ServiceCollection()
                    .AddGraphQLServer()
                    .AddQueryType<QueryAuth>()
                    .Services
                    .BuildServiceProvider()
                    .GetRequiredService<IRequestExecutorManager>(),
                mockLogger
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsType<SchemaException>(result.Exception, exactMatch: false);
        Assert.NotNull(result.Description);
        Assert.Contains("schema", result.Description);

        mockLogger
            .Received(1)
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<SchemaException>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Schema - No Auth - General Error")]
    public async Task ILoggerUnhealthySchemaNoAuthGeneralError()
    {
        var mockLogger = Substitute.For<ILogger<GraphHealthCheck>>();
        var mockRequestExecutorResolver = Substitute.For<IRequestExecutorManager>();
        mockRequestExecutorResolver
            .GetExecutorAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Splash!"));

        var result = await new GraphHealthCheck(mockRequestExecutorResolver, mockLogger)
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsType<Exception>(result.Exception, exactMatch: false);

        mockLogger
            .Received(1)
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Is<Exception>(x => x.Message == "Splash!"),
                Arg.Any<Func<object, Exception?, string>>()
            );

        GetErrorResult(result).MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Healthy - Auth")]
    public async Task ILoggerHealthyAuth()
    {
        var mockLogger = Substitute.For<ILogger<GraphHealthCheck>>();

        var result = await new GraphHealthCheck(
                new ServiceCollection()
                    .AddGraphQLServer()
                    .AddAuthorization()
                    .AddQueryType<QueryAuth>()
                    .Services
                    .BuildServiceProvider()
                    .GetRequiredService<IRequestExecutorManager>(),
                mockLogger
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Null(result.Description);

        mockLogger
            .Received(0)
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<SchemaException>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

        result.MatchSnapshot();
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Healthy")]
    public async Task ILoggerHealthy()
    {
        var mockLogger = Substitute.For<ILogger<GraphHealthCheck>>();

        var result = await new GraphHealthCheck(
                new ServiceCollection()
                    .AddGraphQLServer()
                    .AddQueryType<Query>()
                    .Services
                    .BuildServiceProvider()
                    .GetRequiredService<IRequestExecutorManager>(),
                mockLogger
            )
            .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Null(result.Description);

        mockLogger
            .Received(0)
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<SchemaException>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

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