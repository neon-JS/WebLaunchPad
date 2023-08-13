namespace WebLaunchPad.Api.Services;

public class ConcurrencyService
    : IConcurrencyService
{
    private readonly SemaphoreSlim _lock = new(1);
    private CancellationTokenSource? _cancellationTokenSource;

    public async Task RunAsync(
        Func<CancellationToken, Task> func,
        CancellationToken cancellationToken
    )
    {
        _cancellationTokenSource?.Cancel();
        await _lock.WaitAsync(cancellationToken);
        _cancellationTokenSource = new CancellationTokenSource();

#pragma warning disable CS4014
        Task.Run(
            async () =>
            {
                try
                {
                    await func(_cancellationTokenSource.Token);
                }
                finally
                {
                    _lock.Release();
                }
            },
            CancellationToken.None
        );
#pragma warning restore CS4014
    }
}