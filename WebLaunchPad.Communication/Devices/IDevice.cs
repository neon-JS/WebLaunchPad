namespace WebLaunchPad.Communication.Devices;

/// <summary>
/// Representation of a device (e.g. a Launchpad) that we can send any data to.
/// </summary>
public interface IDevice
{
    public Task WriteAsync(
        IEnumerable<byte> bytes,
        CancellationToken cancellationToken
    );
}