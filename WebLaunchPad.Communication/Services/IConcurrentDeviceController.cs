namespace WebLaunchPad.Communication.Services;

/// <summary>
/// Controls an <see cref="IConcurrentDevice"/> (by setting colors and
/// flushing them). As the device will be controlled via WebApi, it must handle
/// concurrent access.
/// <remarks>
/// FIXME: This seems to be the wrong place, maybe concurrent stuff should happen at api level?
/// </remarks>
/// </summary>
public interface IConcurrentDeviceController
{
    /// <summary>
    /// Sets a given color for a given coordinate.
    /// </summary>
    /// <remarks>
    /// No changes will be made to the device until flushing.
    /// </remarks>
    public Task SetColorAsync(
        uint xIndex,
        uint yIndex,
        Color color,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Flushes all changes to the device.
    /// </summary>
    public Task FlushAsync(CancellationToken cancellationToken);
}