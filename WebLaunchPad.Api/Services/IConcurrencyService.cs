namespace WebLaunchPad.Api.Services;

/// <summary>
/// This service makes sure that the Launchpad is controlled only by one request
/// at a time. This means that <i>any</i> call to the Launchpad must happen
/// inside the <see cref="RunAsync"/> method as it locks itself when being
/// called. 
/// The service also handles long living tasks by running them in the
/// background. If there's a task running while the next one is requested, the
/// original one will be notified by the given CancellationToken. When the first
/// one is finished, the new task will be started.
/// This way, we can ignore most concurrency inside Communication project and
/// inside the controllers.
/// </summary>
public interface IConcurrencyService
{
    /// <summary>
    /// Ensures that any given function is called after another.
    /// The task is run in the background, meaning that <see cref="RunAsync"/>
    /// will <b>not</b> wait for it to finish! The next time that this function
    /// is called, the task - if still running - will be requested to cancel.
    /// After that, the given function will be executed (in the background too).
    /// </summary>
    public Task RunAsync(
        Func<CancellationToken, Task> func,
        CancellationToken cancellationToken
    );
}