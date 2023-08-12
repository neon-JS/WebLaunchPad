namespace WebLaunchPad.Communication.Services;

/// <summary>
/// Controls an <see cref="IDevice"/> (by setting colors and flushing them).
/// </summary>
public interface IDeviceController
{
    /// <summary>
    /// Sets a given color for a given coordinate.
    /// </summary>
    /// <remarks>
    /// No changes will be made to the device until flushing.
    /// </remarks>
    public void SetColor(uint xIndex, uint yIndex, Color color);

    /// <summary>
    /// Sets all fields of a device to given color
    /// </summary>
    /// <remarks>
    /// No changes will be made to the device until flushing.
    /// </remarks>
    public void SetColor(Color color);

    /// <summary>
    /// Flushes all changes to the device.
    /// </summary>
    public Task FlushAsync(CancellationToken cancellationToken);
}