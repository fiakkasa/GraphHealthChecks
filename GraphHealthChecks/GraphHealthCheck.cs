namespace GraphHealthChecks;

public record GraphHealthCheck : IHealthCheck
{
    private const string _message = $"[{nameof(GraphHealthChecks)}] An error has occurred with message: {{message}}";
    private const string _generalSchemaError = "The schema cannot be resolved.";
    private const string _generalError = "A general error has occurred.";
    private readonly ILogger<GraphHealthCheck>? _logger;

    private readonly IRequestExecutorResolver _requestExecutorResolver;

    public GraphHealthCheck(IRequestExecutorResolver requestExecutorResolver, ILogger<GraphHealthCheck> logger)
    {
        (_requestExecutorResolver, _logger) = (requestExecutorResolver, logger);
    }

    public GraphHealthCheck(IRequestExecutorResolver requestExecutorResolver)
    {
        _requestExecutorResolver = requestExecutorResolver;
    }

    public string? Schema { get; set; }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _ = (
                await _requestExecutorResolver.GetRequestExecutorAsync(
                    Schema,
                    cancellationToken
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
