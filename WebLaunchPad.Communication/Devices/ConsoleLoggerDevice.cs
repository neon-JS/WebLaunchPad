namespace WebLaunchPad.Communication.Devices;

/// <summary>
/// A fake device that just dumps all incoming data into the terminal.
/// Use for debugging purposes.
/// </summary>
public class ConsoleLoggerDevice
    : IDevice
{
    public Task WriteAsync(
        ICollection<byte> bytes,
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