namespace WebLaunchPad.Api.Controllers;

[ApiController]
[Route("launchpad/fields")]
public class LaunchpadColorController
    : ControllerBase
{
    private readonly IDeviceController _deviceController;
    private readonly IConcurrencyService _concurrencyService;

    public LaunchpadColorController(
        IDeviceController deviceController,
        IConcurrencyService concurrencyService
    )
    {
        _deviceController = deviceController;
        _concurrencyService = concurrencyService;
    }

    [HttpPost("{xIndex}/{yIndex}")]
    public async Task<IActionResult> SetColorForFieldAsync(
        [FromRoute] uint xIndex,
        [FromRoute] uint yIndex,
        [FromBody] Color color,
        CancellationToken cancellationToken
    )
    {
        await _concurrencyService.RunAsync(
            async taskCancellationToken =>
            {
                _deviceController.SetColor(xIndex, yIndex, color);
                await _deviceController.FlushAsync(taskCancellationToken);
            },
            cancellationToken
        );

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SetColorAsync(
        [FromBody] Color color,
        CancellationToken cancellationToken
    )
    {
        await _concurrencyService.RunAsync(
            async taskCancellationToken =>
            {
                _deviceController.SetColor(color);
                await _deviceController.FlushAsync(taskCancellationToken);
            },
            cancellationToken
        );

        return Ok();
    }
}