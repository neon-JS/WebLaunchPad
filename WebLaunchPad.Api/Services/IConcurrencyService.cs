namespace WebLaunchPad.Api.Services;

/// <summary>
/// This service makes sure that the Launchpad is controlled only by one request
/// at a time. This means that <i>any</i> call to the Launchpad must happen
/// inside the <see cref="RunAsync"/> method as it locks itself when being
/// called. This way, we can ignore concurrency inside Communication project and
/// inside the controllers.
/// </summary>
public interface IConcurrencyService
{
    /// <summary>
    /// Ensures that any given function is called after another.
    /// </summary>
    public Task RunAsync(
        Func<Task> func,
        CancellationToken cancellationToken
    );
}