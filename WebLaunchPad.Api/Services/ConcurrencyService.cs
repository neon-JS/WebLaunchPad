namespace WebLaunchPad.Api.Services;

public class ConcurrencyService
    : IConcurrencyService
{
    private readonly SemaphoreSlim _lock;

    public ConcurrencyService()
    {
        _lock = new SemaphoreSlim(1);
    }

    public async Task RunAsync(
        Func<Task> func,
        CancellationToken cancellationToken
    )
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            await func();
        }
        finally
        {
            _lock.Release();
        }
    }
}