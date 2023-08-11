namespace WebLaunchPad.Communication.Devices;

/// <summary>
/// Representation of a device (e.g. a Launchpad) that we can send any data to.
/// As the device will be controlled via WebApi, it must handle concurrent
/// access.
/// </summary>
/// <remarks>
/// FIXME: This seems to be the wrong place, maybe concurrent stuff should happen at api level?
/// </remarks>
public interface IConcurrentDevice
{
    public Task WriteAsync(
        IEnumerable<byte> bytes,
        CancellationToken cancellationToken
    );
}