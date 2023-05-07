using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Execution;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace GraphHealthChecks;

public record GraphHealthCheck : IHealthCheck
{
    public string? Schema { get; set; }

    private readonly IRequestExecutorResolver _requestExecutorResolver;
    private readonly ILogger<GraphHealthCheck>? _logger;
    private readonly ILogFn? _loggerFn;

    private const string _message = $"[{nameof(GraphHealthChecks)}] An error has occurred with message: {{message}}";

    public GraphHealthCheck(IRequestExecutorResolver requestExecutorResolver, ILogger<GraphHealthCheck> logger) =>
        (_requestExecutorResolver, _logger) = (requestExecutorResolver, logger);

    public GraphHealthCheck(IRequestExecutorResolver requestExecutorResolver, ILogFn loggerFn) =>
        (_requestExecutorResolver, _loggerFn) = (requestExecutorResolver, loggerFn);

    public GraphHealthCheck(IRequestExecutorResolver requestExecutorResolver) =>
        _requestExecutorResolver = requestExecutorResolver;

    private void Log(Exception ex)
    {
        if (_logger is { } logger)
            logger.LogError(ex, _message, ex.Message);
        else if (_loggerFn is { } loggerFn)
            _loggerFn.LogError(_message.Replace("{message}", ex.Message));
    }

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
        catch (Exception ex)
        {
            Log(ex);
            return new HealthCheckResult(HealthStatus.Unhealthy);
        }
    }
}
