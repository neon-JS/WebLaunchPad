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

    private readonly IImageConverter _imageConverter;
    private readonly IConcurrencyService _concurrencyService;
    private readonly IDeviceController _deviceController;

    public LaunchpadImageController(
        IImageConverter imageConverter,
        IConcurrencyService concurrencyService,
        IDeviceController deviceController
    )
    {
        _imageConverter = imageConverter;
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

        var pixels = await _imageConverter.GetPixelMappingAsync(
            content,
            cancellationToken
        );

        await _concurrencyService.RunAsync(
            async () =>
            {
                foreach (var ((xIndex, yIndex), color) in pixels)
                {
                    _deviceController.SetColor(xIndex, yIndex, color);
                }

                await _deviceController.FlushAsync(cancellationToken);
            },
            cancellationToken
        );

        return Ok();
    }
}