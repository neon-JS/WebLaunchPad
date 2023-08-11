namespace WebLaunchPad.Communication.Devices;

/// <summary>
/// Represents a device that is controllable via writing to a file handle.
/// This works e.g. for Launchpads on linux systems (device is <i>/dev/midi3</i>
/// in my case).
/// </summary>
public class FileHandleDevice
    : IConcurrentDevice
{
    private readonly string _unixPath;
    private readonly SemaphoreSlim _lock;

    public FileHandleDevice(string unixPath)
    {
        _unixPath = unixPath;
        _lock = new SemaphoreSlim(1);
    }

    public async Task WriteAsync(
        IEnumerable<byte> bytes,
        CancellationToken cancellationToken
    )
    {
        await _lock.WaitAsync(cancellationToken);

        await using var fileHandle = File.Open(
            _unixPath,
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