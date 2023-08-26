using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace GraphHealthChecks;

public record GraphHealthCheck : IHealthCheck
{
    public string? Schema { get; set; }

    private readonly IRequestExecutorResolver _requestExecutorResolver;
    private readonly ILogger<GraphHealthCheck>? _logger;

    private const string _message = $"[{nameof(GraphHealthChecks)}] An error has occurred with message: {{message}}";
    private const string _generalSchemaError = "The schema cannot be resolved.";
    private const string _generalError = "A general error has occurred.";

    public GraphHealthCheck(IRequestExecutorResolver requestExecutorResolver, ILogger<GraphHealthCheck> logger) =>
        (_requestExecutorResolver, _logger) = (requestExecutorResolver, logger);

    public GraphHealthCheck(IRequestExecutorResolver requestExecutorResolver) =>
        _requestExecutorResolver = requestExecutorResolver;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _ = (
                await _requestExecutorResolver.GetRequestExecutorAsync(
                    schemaName: Schema,
                    cancellationToken: cancellationToken
                )
            ).Schema.ToDocument();

            return new HealthCheckResult(HealthStatus.Healthy);
        }
        catch (SchemaException ex)
        {
            _logger?.LogError(ex, _message, ex.Message);

            return new HealthCheckResult(HealthStatus.Unhealthy, _generalSchemaError, ex);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, _message, ex.Message);

            return new HealthCheckResult(HealthStatus.Unhealthy, _generalError, ex);
        }
    }
}
