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
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger("health1", "schema1")
                .AddGraphHealthWithNoLogger("health2", "schema2")
                .AddGraphHealthWithILogger("health3", "schema3")
                .AddGraphHealthWithILogger("health4", "schema4")
                .AddGraphHealthWithILoggerFactory("health5", "schema5")
                .AddGraphHealthWithILoggerFactory("health6", "schema6")
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
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger("health1", "schema")
                .AddGraphHealthWithNoLogger("health2", "schema")
                .AddGraphHealthWithILogger("health3", "schema")
                .AddGraphHealthWithILogger("health4", "schema")
                .AddGraphHealthWithILoggerFactory("health5", "schema")
                .AddGraphHealthWithILoggerFactory("health6", "schema")
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
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger("health1")
                .AddGraphHealthWithNoLogger("health2")
                .AddGraphHealthWithILogger("health3")
                .AddGraphHealthWithILogger("health4")
                .AddGraphHealthWithILoggerFactory("health5")
                .AddGraphHealthWithILoggerFactory("health6")
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
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger("health")
                .AddGraphHealthWithNoLogger("health")
                .AddGraphHealthWithILogger("health")
                .AddGraphHealthWithILogger("health")
                .AddGraphHealthWithILoggerFactory("health")
                .AddGraphHealthWithILoggerFactory("health")
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
                .AddHealthChecks()
                .AddGraphHealthWithNoLogger()
                .AddGraphHealthWithNoLogger()
                .AddGraphHealthWithILogger()
                .AddGraphHealthWithILogger()
                .AddGraphHealthWithILoggerFactory()
                .AddGraphHealthWithILoggerFactory()
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
                .AddGraphHealthWithNoLogger("Graph")
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
                .AddGraphHealthWithILogger("Graph")
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

    [Fact(DisplayName = "AddGraphHealthWithILoggerFactory - Default Health Name")]
    public void AddGraphHealthWithILoggerFactoryDefaultHealthName()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthWithILoggerFactory()
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

    [Fact(DisplayName = "AddGraphHealthWithILoggerFactory - Custom Health Name")]
    public void AddGraphHealthWithILoggerFactoryCustomHealthName()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthWithILoggerFactory("Graph")
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
