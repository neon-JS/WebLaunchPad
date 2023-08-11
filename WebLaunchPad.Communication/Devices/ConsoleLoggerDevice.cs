namespace WebLaunchPad.Communication.Devices;

/// <summary>
/// A fake device that just dumps all incoming data into the terminal.
/// Use for debugging purposes.
/// </summary>
public class ConsoleLoggerDevice
    : IConcurrentDevice
{
    private readonly SemaphoreSlim _lock;

    public ConsoleLoggerDevice()
    {
        _lock = new SemaphoreSlim(1);
    }

    public async Task WriteAsync(
        IEnumerable<byte> bytes,
        CancellationToken cancellationToken
    )
    {
        await _lock.WaitAsync(cancellationToken);

        foreach (var @byte in bytes)
        {
            Console.Write($"{@byte:X}, ");
        }

        _lock.Release();
    }
}