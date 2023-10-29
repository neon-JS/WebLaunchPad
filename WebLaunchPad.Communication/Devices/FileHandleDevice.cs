namespace WebLaunchPad.Communication.Devices;

/// <summary>
/// Represents a device that is controllable via writing to a file handle.
/// This works e.g. for Launchpads on linux systems (device is <i>/dev/midi3</i>
/// in my case).
/// </summary>
public class FileHandleDevice
    : IDevice
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock;

    public FileHandleDevice(string filePath)
    {
        _filePath = filePath;
        _lock = new SemaphoreSlim(1);
    }

    public async Task WriteAsync(
        ICollection<byte> bytes,
        CancellationToken cancellationToken
    )
    {
        if (!bytes.Any())
        {
            return;
        }
        
        /* Locking shouldn't really be necessary (as it's handled by the API)
         * but as we're writing to a real device, keep it for paranoia reasons */
        await _lock.WaitAsync(cancellationToken);

        await using var fileHandle = File.Open(
            _filePath,
            FileMode.Open,
            FileAccess.Write
        );

        await fileHandle.WriteAsync(
            new ReadOnlyMemory<byte>(bytes.ToArray()),
            cancellationToken
        );

        await fileHandle.FlushAsync(cancellationToken);

        _lock.Release();
    }
}