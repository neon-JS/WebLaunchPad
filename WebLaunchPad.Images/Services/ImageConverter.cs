namespace WebLaunchPad.Images.Services;

public class ImageConverter
    : IImageConverter
{
    public Task<IDictionary<(uint, uint), Color>> GetPixelMappingAsync(
        Stream fileStream,
        CancellationToken cancellationToken
    )
    {
        using var bitmap = SKBitmap.Decode(fileStream);
        var width = bitmap.Width;

        var colors = bitmap.Pixels
           .Select(p => new Color(p.Red, p.Green, p.Blue))
           .Select((color, i) =>
            {
                var xIndex = i % width;
                var yIndex = i / width;

                return (
                    Coordinate: (xIndex, yIndex),
                    Color: color
                );
            })
           .ToDictionary(
                kv => ((uint, uint))kv.Coordinate,
                kv => kv.Color
            );

        return Task.FromResult<IDictionary<(uint, uint), Color>>(colors);
    }
}