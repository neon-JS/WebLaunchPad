namespace WebLaunchPad.Communication.Services;

/// <summary>
/// Implementation of a <see cref="IConcurrentDeviceController"/> for a
/// Launchpad MK2
/// </summary>
public class LaunchpadDeviceController
    : IConcurrentDeviceController
{
    private readonly IConcurrentDevice _launchpad;
    private readonly List<byte> _queue;
    private readonly SemaphoreSlim _lock;

    public LaunchpadDeviceController(IConcurrentDevice launchpad)
    {
        _launchpad = launchpad;
        _queue = new List<byte>();
        _lock = new SemaphoreSlim(1);
    }

    public async Task SetColorAsync(
        uint xIndex,
        uint yIndex,
        Color color,
        CancellationToken cancellationToken
    )
    {
        if (xIndex > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(xIndex));
        }

        if (yIndex > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(yIndex));
        }

        var launchpadIndex = yIndex < 8
            ? (yIndex + 1) * 10 + xIndex + 1
            : 104 + xIndex;

        /* As the controller must handle concurrency, we have to make sure that
         * two calls at the same time won't mix up any data in the queue,
         * therefore we must lock it. */

        await _lock.WaitAsync(cancellationToken);

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

        _lock.Release();
    }

    public async Task FlushAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        /* As the queue is locked on all read/write operations, we can safely
         * write & clear it afterwards. */
        await _launchpad.WriteAsync(_queue, cancellationToken);
        _queue.Clear();

        _lock.Release();
    }
}