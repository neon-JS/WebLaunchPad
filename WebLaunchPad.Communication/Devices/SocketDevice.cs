namespace WebLaunchPad.Communication.Devices;

/// <summary>
/// Represents a device that is controllable via sending data over a unix socket.
/// This can be used e.g. when working on macOS, where MIDI devices are not
/// being represented as a file handle. (see fake_midi_socket project for this.)
/// </summary>
public class SocketDevice(string socketPath) : IDevice
{
    private readonly SemaphoreSlim _lock = new(1);

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

        using var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
        var endpoint = new UnixDomainSocketEndPoint(socketPath);
        await socket.ConnectAsync(endpoint, cancellationToken);

        await socket.SendAsync(bytes.ToArray(), cancellationToken);

        _lock.Release();
    }
}