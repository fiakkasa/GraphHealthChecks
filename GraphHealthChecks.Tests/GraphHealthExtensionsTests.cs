using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace GraphHealthChecks.Tests;

public class GraphHealthExtensionsTests
{
    private static readonly string[] _schemas =
    [
        "health1",
        "health2",
        "health3",
        "health4",
        "health5",
        "health6"
    ];
    private static readonly string[] _defaultHealthNames = ["Graph Health"];
    private static readonly string[] _customHealthNames = ["Graph"];

    [Fact(DisplayName = "AddGraphHealth - Multiple Instances - Separate Names and Schemas")]
    public void AddGraphHealthMultipleInstancesSeparateNamesAndSchemas()
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
            _schemas
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealth - Multiple Instances - Separate Names and Same Schema")]
    public void AddGraphHealthMultipleInstancesSeparateNamesAndSameSchema()
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
            _schemas
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealth - Multiple Instances - Separate Names and Same Schema (Default)")]
    public void AddGraphHealthMultipleInstancesSeparateNamesAndSameSchemaDefault()
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
            _schemas
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealth - Multiple Instances - Same Name and Schema (Default)")]
    public void AddGraphHealthMultipleInstancesSameNameAndSchema()
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
    public void AddGraphHealthMultipleInstancesSameNameDefaultAndSchemaDefault()
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
                if (n != _defaultHealthNames[0]) throw new Exception("Does not match");
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
            _defaultHealthNames
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
                .AddGraphHealthWithNoLogger(_customHealthNames[0])
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _customHealthNames
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
            _defaultHealthNames
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
                .AddGraphHealthWithILogger(_customHealthNames[0])
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _customHealthNames
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
            _defaultHealthNames
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
                .AddGraphHealthWithILoggerFactory(_customHealthNames[0])
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _customHealthNames
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthUsingProvider - Multiple Instances - Separate Names and Schemas")]
    public void AddGraphHealthUsingProviderMultipleInstancesSeparateNamesAndSchemas()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithNoLogger("health1", "schema1")
                .AddGraphHealthUsingProviderWithNoLogger("health2", "schema2")
                .AddGraphHealthUsingProviderWithILogger("health3", "schema3")
                .AddGraphHealthUsingProviderWithILogger("health4", "schema4")
                .AddGraphHealthUsingProviderWithILoggerFactory("health5", "schema5")
                .AddGraphHealthUsingProviderWithILoggerFactory("health6", "schema6")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _schemas
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthUsingProvider - Multiple Instances - Separate Names and Same Schema")]
    public void AddGraphHealthUsingProviderMultipleInstancesSeparateNamesAndSameSchema()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithNoLogger("health1", "schema")
                .AddGraphHealthUsingProviderWithNoLogger("health2", "schema")
                .AddGraphHealthUsingProviderWithILogger("health3", "schema")
                .AddGraphHealthUsingProviderWithILogger("health4", "schema")
                .AddGraphHealthUsingProviderWithILoggerFactory("health5", "schema")
                .AddGraphHealthUsingProviderWithILoggerFactory("health6", "schema")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _schemas
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthUsingProvider - Multiple Instances - Separate Names and Same Schema (Default)")]
    public void AddGraphHealthUsingProviderMultipleInstancesSeparateNamesAndSameSchemaDefault()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithNoLogger("health1")
                .AddGraphHealthUsingProviderWithNoLogger("health2")
                .AddGraphHealthUsingProviderWithILogger("health3")
                .AddGraphHealthUsingProviderWithILogger("health4")
                .AddGraphHealthUsingProviderWithILoggerFactory("health5")
                .AddGraphHealthUsingProviderWithILoggerFactory("health6")
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _schemas
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthUsingProvider - Multiple Instances - Same Name and Schema (Default)")]
    public void AddGraphHealthUsingProviderMultipleInstancesSameNameAndSchema()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithNoLogger("health")
                .AddGraphHealthUsingProviderWithNoLogger("health")
                .AddGraphHealthUsingProviderWithILogger("health")
                .AddGraphHealthUsingProviderWithILogger("health")
                .AddGraphHealthUsingProviderWithILoggerFactory("health")
                .AddGraphHealthUsingProviderWithILoggerFactory("health")
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

    [Fact(DisplayName = "AddGraphHealthUsingProvider - Multiple Instances - Same Name (Default) and Schema (Default)")]
    public void AddGraphHealthUsingProviderMultipleInstancesSameNameDefaultAndSchemaDefault()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithNoLogger()
                .AddGraphHealthUsingProviderWithNoLogger()
                .AddGraphHealthUsingProviderWithILogger()
                .AddGraphHealthUsingProviderWithILogger()
                .AddGraphHealthUsingProviderWithILoggerFactory()
                .AddGraphHealthUsingProviderWithILoggerFactory()
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.All(
            options.Registrations.Select(x => x.Name),
            n =>
            {
                if (n != _defaultHealthNames[0]) throw new Exception("Does not match");
            }
        );
    }

    [Fact(DisplayName = "AddGraphHealthUsingProviderWithNoLogger - Default Health Name")]
    public void AddGraphHealthUsingProviderWithNoLoggerDefaultHealthName()
    {
        var options =
            new ServiceCollection()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithNoLogger()
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _defaultHealthNames
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthUsingProviderWithNoLogger - Custom Health Name")]
    public void AddGraphHealthUsingProviderWithNoLoggerCustomHealthName()
    {
        var options =
            new ServiceCollection()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithNoLogger(_customHealthNames[0])
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _customHealthNames
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthUsingProviderWithILogger - Default Health Name")]
    public void AddGraphHealthUsingProviderWithILoggerDefaultHealthName()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithILogger()
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _defaultHealthNames
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthUsingProviderWithILogger - Custom Health Name")]
    public void AddGraphHealthUsingProviderWithILoggerCustomHealthName()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithILogger(_customHealthNames[0])
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _customHealthNames
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthUsingProviderWithILoggerFactory - Default Health Name")]
    public void AddGraphHealthUsingProviderWithILoggerFactoryDefaultHealthName()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithILoggerFactory()
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _defaultHealthNames
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }

    [Fact(DisplayName = "AddGraphHealthUsingProviderWithILoggerFactory - Custom Health Name")]
    public void AddGraphHealthUsingProviderWithILoggerFactoryCustomHealthName()
    {
        var options =
            new ServiceCollection()
                .AddLogging()
                .AddHealthChecks()
                .AddGraphHealthUsingProviderWithILoggerFactory(_customHealthNames[0])
                .Services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<HealthCheckServiceOptions>>()
                .Value;

        Assert.Equal(
            options.Registrations.Count,
            _customHealthNames
                .Intersect(options.Registrations.Select(x => x.Name))
                .Distinct()
                .Count()
        );
    }
}