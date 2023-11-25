namespace WebLaunchPad.Api.Controllers;

[ApiController]
[Route("launchpad/fields")]
public class LaunchpadColorController(
    IDeviceController deviceController,
    IConcurrencyService concurrencyService
) : ControllerBase
{
    [HttpPost("{xIndex}/{yIndex}")]
    public async Task<IActionResult> SetColorForFieldAsync(
        [FromRoute] uint xIndex,
        [FromRoute] uint yIndex,
        [FromBody] Color color,
        CancellationToken cancellationToken
    )
    {
        await concurrencyService.RunAsync(
            async taskCancellationToken =>
            {
                deviceController.SetColor(xIndex, yIndex, color);
                await deviceController.FlushAsync();
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
        await concurrencyService.RunAsync(
            async taskCancellationToken =>
            {
                deviceController.SetColor(color);
                await deviceController.FlushAsync();
            },
            cancellationToken
        );

        return Ok();
    }
}