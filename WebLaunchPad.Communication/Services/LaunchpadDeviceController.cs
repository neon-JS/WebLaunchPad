namespace WebLaunchPad.Communication.Services;

/// <summary>
/// Implementation of a <see cref="IDeviceController"/> for a Launchpad MK2
/// </summary>
public class LaunchpadDeviceController
    : IDeviceController
{
    private readonly IDevice _launchpad;
    private readonly List<byte> _queue;

    public LaunchpadDeviceController(IDevice launchpad)
    {
        _launchpad = launchpad;
        _queue = new List<byte>();
    }

    public void SetColor(uint xIndex, uint yIndex, Color color)
    {
        if (xIndex > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(xIndex));
        }

        if (yIndex > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(yIndex));
        }

        if (xIndex == 8 && yIndex == 8)
        {
            /* This pixel should exist but doesn't. Ignore it as otherwise you
             * couldn't upload a gif to a Launchpad. */
            return;
        }

        /* The launchpad index starts on the bottom left */
        var invertedYIndex = 8 - yIndex;

        var launchpadIndex = invertedYIndex < 8
            ? (invertedYIndex + 1) * 10 + xIndex + 1
            : 104 + xIndex;

        _queue.AddRange(new byte[]
        {
            0xF0,
            0x00,
            0x20,
            0x29,
            0x02,
            0x18,
            0x0B,
            (byte)launchpadIndex,
            (byte)(color.Red / 4),
            (byte)(color.Green / 4),
            (byte)(color.Blue / 4),
            0xF7
        });
    }

    public void SetColor(Color color)
    {
        /* Launchpad does not support setting all colors in RGB mode. Therefore
         * we must set each field manually.  */
        for (uint x = 0; x < 9; x++)
        {
            for (uint y = 0; y < 9; y++)
            {
                SetColor(x, y, color);
            }
        }
    }

    public async Task FlushAsync(CancellationToken cancellationToken)
    {
        await _launchpad.WriteAsync(_queue, CancellationToken.None);
        _queue.Clear();
    }
}