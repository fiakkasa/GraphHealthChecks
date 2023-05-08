using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace GraphHealthChecks.Tests;

public class GraphHealthExtensionsTests
{
    [Fact(DisplayName = "AddGraphHealth - Multiple Instances - Separate Names and Schemas")]
    public void MultipleInstancesSeparateNamesAndSchemas()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddSingleton(new Mock<ILogFn>().Object)
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger(healthName: "health1", schemaName: "schema1")
                .AddGraphHealthWithNoLogger(healthName: "health2", schemaName: "schema2")
                .AddGraphHealthWithILogger(healthName: "health3", schemaName: "schema3")
                .AddGraphHealthWithILogger(healthName: "health4", schemaName: "schema4")
                .AddGraphHealthWithLoggerFn(healthName: "health5", schemaName: "schema5")
                .AddGraphHealthWithLoggerFn(healthName: "health6", schemaName: "schema6")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            new[]
            {
                "health1",
                "health2",
                "health3",
                "health4",
                "health5",
                "health6"
            }
            .Intersect(options.Registrations.Select(x => x.Name))
            .Distinct()
            .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealth - Multiple Instances - Separate Names and Same Schema")]
    public void MultipleInstancesSeparateNamesAndSameSchema()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddSingleton(new Mock<ILogFn>().Object)
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger(healthName: "health1", schemaName: "schema")
                .AddGraphHealthWithNoLogger(healthName: "health2", schemaName: "schema")
                .AddGraphHealthWithILogger(healthName: "health3", schemaName: "schema")
                .AddGraphHealthWithILogger(healthName: "health4", schemaName: "schema")
                .AddGraphHealthWithLoggerFn(healthName: "health5", schemaName: "schema")
                .AddGraphHealthWithLoggerFn(healthName: "health6", schemaName: "schema")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            new[]
            {
                "health1",
                "health2",
                "health3",
                "health4",
                "health5",
                "health6"
            }
            .Intersect(options.Registrations.Select(x => x.Name))
            .Distinct()
            .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealth - Multiple Instances - Separate Names and Same Schema (Default)")]
    public void MultipleInstancesSeparateNamesAndSameSchemaDefault()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddSingleton(new Mock<ILogFn>().Object)
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger(healthName: "health1")
                .AddGraphHealthWithNoLogger(healthName: "health2")
                .AddGraphHealthWithILogger(healthName: "health3")
                .AddGraphHealthWithILogger(healthName: "health4")
                .AddGraphHealthWithLoggerFn(healthName: "health5")
                .AddGraphHealthWithLoggerFn(healthName: "health6")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            new[]
            {
                "health1",
                "health2",
                "health3",
                "health4",
                "health5",
                "health6"
            }
            .Intersect(options.Registrations.Select(x => x.Name))
            .Distinct()
            .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealth - Multiple Instances - Same Name and Schema (Default)")]
    public void MultipleInstancesSameNameAndSchema()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddSingleton(new Mock<ILogFn>().Object)
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger(healthName: "health")
                .AddGraphHealthWithNoLogger(healthName: "health")
                .AddGraphHealthWithILogger(healthName: "health")
                .AddGraphHealthWithILogger(healthName: "health")
                .AddGraphHealthWithLoggerFn(healthName: "health")
                .AddGraphHealthWithLoggerFn(healthName: "health")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.All(
            options.Registrations.Select(x => x.Name),
            n =>
            {
                if (n != "health") throw new Exception("Does not match");
            }
        );
    }

    [Fact(DisplayName = "AddGraphHealth - Multiple Instances - Same Name (Default) and Schema (Default)")]
    public void MultipleInstancesSameNameDefaultAndSchemaDefault()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddSingleton(new Mock<ILogFn>().Object)
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger()
                .AddGraphHealthWithNoLogger()
                .AddGraphHealthWithILogger()
                .AddGraphHealthWithILogger()
                .AddGraphHealthWithLoggerFn()
                .AddGraphHealthWithLoggerFn()
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.All(
            options.Registrations.Select(x => x.Name),
            n =>
            {
                if (n != "Graph Health") throw new Exception("Does not match");
            }
        );
    }

    [Fact(DisplayName = "AddGraphHealthWithNoLogger - Default Health Name")]
    public void AddGraphHealthWithNoLoggerDefaultHealthName()
    {
        var options =
            new ServiceCollection()
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger()
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            new[] { "Graph Health" }
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthWithNoLogger - Custom Health Name")]
    public void AddGraphHealthWithNoLoggerCustomHealthName()
    {
        var options =
            new ServiceCollection()
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger(healthName: "Graph")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            new[] { "Graph" }
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthWithILogger - Default Health Name")]
    public void AddGraphHealthWithILoggerDefaultHealthName()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthWithILogger()
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            new[] { "Graph Health" }
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthWithILogger - Custom Health Name")]
    public void AddGraphHealthWithILoggerCustomHealthName()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthWithILogger(healthName: "Graph")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            new[] { "Graph" }
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthWithLoggerFn - Default Health Name")]
    public void AddGraphHealthWithLoggerFnDefaultHealthName()
    {
        var options =
            new ServiceCollection()
                .AddSingleton(new Mock<ILogFn>())
                .AddHealthChecks()
                .AddGraphHealthWithLoggerFn()
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            new[] { "Graph Health" }
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthWithLoggerFn - Custom Health Name")]
    public void AddGraphHealthWithLoggerFnCustomHealthName()
    {
        var options =
            new ServiceCollection()
                .AddSingleton(new Mock<ILogFn>())
                .AddHealthChecks()
                .AddGraphHealthWithLoggerFn(healthName: "Graph")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            new[] { "Graph" }
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }
}