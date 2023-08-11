namespace WebLaunchPad.Communication.Devices;

/// <summary>
/// A fake device that mimics a Launchpad in an ANSI compatible terminal.
/// Use for debugging purposes.
/// </summary>
public class FakeLaunchpadConsoleDevice
    : IConcurrentDevice
{
    private readonly SemaphoreSlim _lock;

    public FakeLaunchpadConsoleDevice()
    {
        _lock = new SemaphoreSlim(1);
    }

    public async Task WriteAsync(
        IEnumerable<byte> bytes,
        CancellationToken cancellationToken
    )
    {
        await _lock.WaitAsync(cancellationToken);

        foreach (var colorCommand in bytes.Chunk(12))
        {
            if (colorCommand.Length != 12)
            {
                continue;
            }

            var index = colorCommand[7];
            var xIndex = index < 104 ? index % 10 - 1 : index - 104;
            var yIndex = 9 - (index < 104 ? index / 10 - 1 : 8);

            var red = colorCommand[8] * 4;
            var green = colorCommand[9] * 4;
            var blue = colorCommand[10] * 4;

            /* \x1B[H                          - reset cursor to top left
             * \x1B[<ROWS>B                    - go <ROWS> down
             * \x1B[<COLUMNS>C                 - go columns right
             * \x1B[38;2;<RED>;<GREEN>;<BLUE>m - set text color */
            Console.Write($"\x1B[H\x1B[{yIndex}B\x1B[{3 * (xIndex + 1)}C\x1B[38;2;{red};{green};{blue}m██ ");
        }

        _lock.Release();
    }
}