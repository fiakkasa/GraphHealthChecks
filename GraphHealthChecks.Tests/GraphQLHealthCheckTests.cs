using System.Net;
using System.Text;
using HotChocolate.Authorization;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq.Contrib.HttpClient;

namespace GraphHealthChecks.Tests;

public class GraphQLHealthCheckTests
{
    public class Query
    {
        public string Name => "Hello";
    }

    public class QueryAuth
    {
        [Authorize]
        public string Name => "Hello";
    }

    [Fact(DisplayName = "GraphQLHealthCheck - No Logger - Unhealthy Schema")]
    public async Task UnhealthySchemaNoLogger()
    {
        var result = await new GraphHealthCheck(
            new ServiceCollection()
                .AddGraphQLServer()
                .AddQueryType()
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IRequestExecutorResolver>()
        )
        .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsAssignableFrom<SchemaException>(result.Exception);
        Assert.NotNull(result.Description);
        Assert.Contains("schema", result.Description);
    }

    [Fact(DisplayName = "GraphQLHealthCheck - No Logger - Healthy")]
    public async Task HealthyNoLogger()
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
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - General Error")]
    public async Task GeneralErrorILogger()
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
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Schema")]
    public async Task UnhealthySchemaILogger()
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
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Stitched Schema - No Registered Http Client")]
    public async Task UnhealthyStitchedSchemaNoRegisteredHttpClientILogger()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();

        var result = await new GraphHealthCheck(
            new ServiceCollection()
                .AddGraphQLServer()
                .AddRemoteSchema("Remote")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IRequestExecutorResolver>(),
            mockLogger.Object
        )
        .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsAssignableFrom<InvalidOperationException>(result.Exception);
        Assert.NotNull(result.Description);
        Assert.Contains("general", result.Description);

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<InvalidOperationException?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Once);
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Stitched Schema - No Registered Http Client For Schema")]
    public async Task UnhealthyStitchedSchemaNoRegisteredHttpClientForSchemaILogger()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();

        var result = await new GraphHealthCheck(
            new ServiceCollection()
                .AddSingleton(new Mock<IHttpClientFactory>().Object)
                .AddGraphQLServer()
                .AddRemoteSchema("Remote")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IRequestExecutorResolver>(),
            mockLogger.Object
        )
        .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsAssignableFrom<ArgumentNullException>(result.Exception);
        Assert.NotNull(result.Description);
        Assert.Contains("general", result.Description);

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<ArgumentNullException?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Once);
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Stitched Schema - Http Client Returns Error For Schema")]
    public async Task UnhealthyStitchedSchemaHttpClientReturnsErrorForSchemaILogger()
    {
        var mockLogger = new Mock<ILogger<GraphHealthCheck>>();
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler
            .SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.InternalServerError);
        var httpClient = handler.CreateClient();
        httpClient.BaseAddress = new Uri("http://localhost/graphql");
        var mockHttpFactory = new Mock<IHttpClientFactory>();
        mockHttpFactory
            .Setup(m => m.CreateClient("Remote"))
            .Returns(httpClient);

        var result = await new GraphHealthCheck(
            new ServiceCollection()
                .AddSingleton(mockHttpFactory.Object)
                .AddGraphQLServer()
                .AddRemoteSchema("Remote")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IRequestExecutorResolver>(),
            mockLogger.Object
        )
        .CheckHealthAsync(null!);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.IsAssignableFrom<HttpRequestException>(result.Exception);
        Assert.NotNull(result.Description);
        Assert.Contains("general", result.Description);

        mockLogger.VerifyLog(m => m.LogError(It.IsAny<HttpRequestException?>(), It.IsAny<string?>(), It.IsAny<object>()), Times.Once);
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Unhealthy Schema - No Auth")]
    public async Task UnhealthySchemaNoAuthILogger()
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
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Healthy - Auth")]
    public async Task HealthyAuthILogger()
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
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogger - Healthy")]
    public async Task HealthyILogger()
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
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogFn - Unhealthy Schema")]
    public async Task UnhealthyILogFn()
    {
        var mockLogger = new Mock<ILogFn>();

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

        mockLogger.Verify(m => m.LogError(It.IsAny<string?>()), Times.Once);
    }

    [Fact(DisplayName = "GraphQLHealthCheck - ILogFn - Healthy")]
    public async Task HealthyILogFn()
    {
        var mockLogger = new Mock<ILogFn>();

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

        mockLogger.Verify(m => m.LogError(It.IsAny<string?>()), Times.Never);
    }
}
