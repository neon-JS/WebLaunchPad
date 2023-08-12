namespace WebLaunchPad.Api.Controllers;

[ApiController]
[Route("launchpad/fields")]
public class LaunchpadColorController
    : ControllerBase
{
    private readonly IConcurrentDeviceController _deviceController;

    public LaunchpadColorController(IConcurrentDeviceController deviceController)
    {
        _deviceController = deviceController;
    }

    [HttpPost("{xIndex}/{yIndex}")]
    public async Task<IActionResult> SetColorForFieldAsync(
        [FromRoute] uint xIndex,
        [FromRoute] uint yIndex,
        [FromBody] Color color,
        CancellationToken cancellationToken
    )
    {
        await _deviceController.SetColorAsync(
            xIndex,
            yIndex,
            color,
            cancellationToken
        );

        await _deviceController.FlushAsync(cancellationToken);

        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> SetColorAsync(
        [FromBody] Color color,
        CancellationToken cancellationToken
    )
    {
        await _deviceController.SetColorAsync(color, cancellationToken);
        await _deviceController.FlushAsync(cancellationToken);

        return Ok();
    }
}