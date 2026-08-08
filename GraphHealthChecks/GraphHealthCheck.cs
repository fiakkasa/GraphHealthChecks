namespace GraphHealthChecks;

public record GraphHealthCheck : IHealthCheck
{
    private const string _message = $"[{nameof(GraphHealthChecks)}] An error has occurred with message: {{message}}";
    private const string _generalSchemaError = "The schema cannot be resolved.";
    private const string _generalError = "A general error has occurred.";
    private readonly ILogger<GraphHealthCheck>? _logger;

    private readonly IRequestExecutorProvider _requestExecutorProvider;

    public GraphHealthCheck(IRequestExecutorProvider requestExecutorProvider, ILogger<GraphHealthCheck> logger)
    {
        _requestExecutorProvider = requestExecutorProvider;
        _logger = logger;
    }

    public GraphHealthCheck(IRequestExecutorProvider requestExecutorProvider)
    {
        _requestExecutorProvider = requestExecutorProvider;
    }

    public string? Schema { get; set; }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var executor = await _requestExecutorProvider.GetExecutorAsync(
                Schema,
                cancellationToken
            );
            _ = executor.Schema.ToString();

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