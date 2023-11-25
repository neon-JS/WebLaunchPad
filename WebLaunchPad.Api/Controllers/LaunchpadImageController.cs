namespace WebLaunchPad.Api.Controllers;

[ApiController]
[Route("launchpad/image")]
public class LaunchpadImageController(
    IGifConverter gifConverter,
    IConcurrencyService concurrencyService,
    IDeviceController deviceController
) : ControllerBase
{
    private const string _GIF_CONTENT_TYPE = "image/gif";
    private const string _GIF_FILE_EXTENSION = ".gif";

    private readonly byte[][] _gifMagicBytesSequences =
    {
        "GIF87a"u8.ToArray(),
        "GIF89a"u8.ToArray(),
    };

    [HttpPost]
    public async Task<IActionResult> ShowGifAsync(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken
    )
    {
        if (!file.ContentType.Equals(_GIF_CONTENT_TYPE, StringComparison.InvariantCulture))
        {
            return new UnsupportedMediaTypeResult();
        }

        if (!file.FileName.EndsWith(_GIF_FILE_EXTENSION, StringComparison.InvariantCulture))
        {
            return BadRequest();
        }

        var content = new MemoryStream();
        await file.CopyToAsync(content, cancellationToken);
        content.Position = 0;

        if (!_gifMagicBytesSequences.Any(content.StartsWith))
        {
            return BadRequest();
        }

        var frames = gifConverter.GetFrames(content);

        await concurrencyService.RunAsync(
            async taskCancellationToken =>
            {
                foreach (var frame in frames.ToList().RepeatForever())
                {
                    if (taskCancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    foreach (var (position, color) in frame.Mapping)
                    {
                        deviceController.SetColor(position.X, position.Y, color);
                    }

                    await deviceController.FlushAsync();

                    if (frames.Count == 1)
                    {
                        /* no need to be called again. */
                        return;
                    }

                    await Task.Delay(frame.DurationMs, taskCancellationToken);
                }
            },
            cancellationToken
        );

        return Ok();
    }
}