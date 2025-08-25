namespace ITC.Authorization.Background;

public abstract class BackgroundServiceBase : BackgroundService
{
    protected readonly IServiceScopeFactory _factory;
    protected readonly ILogger<BackgroundServiceBase> _logger;

    protected BackgroundServiceBase(IServiceScopeFactory serviceScopeFactory, ILogger<BackgroundServiceBase> logger)
    {
        _factory = serviceScopeFactory;
        _logger = logger;
    }

    protected abstract TimeSpan Period { get; }

    protected abstract Task Work(CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Period);
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await Work(stoppingToken);
            }
            catch (System.OperationCanceledException)
            { }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Failed to execute {this.GetType().Name}");
            }
        }
    }
}