namespace FlightPlanner.Web.Handlers;

/// <summary>
/// HTTP handler that ensures a minimum delay for API calls to show loading indicator
/// </summary>
public class LoadingIndicatorHandler : DelegatingHandler
{
    private readonly ILogger<LoadingIndicatorHandler> _logger;
    private readonly int _minimumDelayMs;

    public LoadingIndicatorHandler(ILogger<LoadingIndicatorHandler> logger, int minimumDelayMs = 300)
    {
        _logger = logger;
        _minimumDelayMs = minimumDelayMs;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            // Make the actual API call
            var response = await base.SendAsync(request, cancellationToken);

            // Calculate elapsed time
            var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;

            // If the call was too fast, add a small delay to ensure loading indicator is visible
            if (elapsed < _minimumDelayMs)
            {
                var remainingDelay = _minimumDelayMs - (int)elapsed;
                await Task.Delay(remainingDelay, cancellationToken);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during API call to {RequestUri}", request.RequestUri);
            throw;
        }
    }
}

