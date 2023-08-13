namespace WebLaunchPad.Api.Controllers;

[ApiController]
[Route("launchpad/image")]
public class LaunchpadImageController
    : ControllerBase
{
    private const string _GIF_CONTENT_TYPE = "image/gif";
    private const string _GIF_FILE_EXTENSION = ".gif";

    private readonly byte[][] _gifMagicBytesSequences =
    {
        "GIF87a"u8.ToArray(),
        "GIF89a"u8.ToArray(),
    };

    private readonly IGifConverter _gifConverter;
    private readonly IConcurrencyService _concurrencyService;
    private readonly IDeviceController _deviceController;

    public LaunchpadImageController(
        IGifConverter gifConverter,
        IConcurrencyService concurrencyService,
        IDeviceController deviceController
    )
    {
        _gifConverter = gifConverter;
        _concurrencyService = concurrencyService;
        _deviceController = deviceController;
    }

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

        var frames = await _gifConverter.GetFramesAsync(
            content,
            cancellationToken
        );

        await _concurrencyService.RunAsync(
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
                        _deviceController.SetColor(position.X,position.Y,color);
                    }

                    await _deviceController.FlushAsync(taskCancellationToken);

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