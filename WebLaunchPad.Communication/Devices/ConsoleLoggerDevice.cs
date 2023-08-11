namespace WebLaunchPad.Communication.Devices;

/// <summary>
/// A fake device that just dumps all incoming data into the terminal.
/// Use for debugging purposes.
/// </summary>
public class ConsoleLoggerDevice
    : IConcurrentDevice
{
    public Task WriteAsync(
        IEnumerable<byte> bytes,
        CancellationToken cancellationToken
    )
    {
        foreach (var @byte in bytes)
        {
            Console.Write($"{@byte:X}, ");
        }

        return Task.CompletedTask;
    }
}