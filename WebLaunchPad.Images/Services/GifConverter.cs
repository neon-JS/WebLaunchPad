namespace WebLaunchPad.Images.Services;

public class GifConverter
    : IGifConverter
{
    public Task<ICollection<Frame>> GetFramesAsync(
        Stream fileStream,
        CancellationToken cancellationToken
    )
    {
        using var codec = SKCodec.Create(fileStream);
        var frames = new List<Frame>();

        var indexedFrames = codec
           .FrameInfo
           .Select((f, i) => (f, i));

        foreach (var (frameInfo, frameIndex) in indexedFrames)
        {
            var imageInfo = new SKImageInfo(codec.Info.Width, codec.Info.Height);

            using var bitmap = new SKBitmap(imageInfo);
            var bitmapPixelPointer = bitmap.GetPixels();

            codec.GetPixels(
                imageInfo,
                bitmapPixelPointer,
                new SKCodecOptions(frameIndex)
            );

            frames.Add(
                new Frame(
                    frameInfo.Duration,
                    ConvertBitmapPixelsToMapping(bitmap)
                )
            );
        }

        return Task.FromResult<ICollection<Frame>>(frames);
    }

    private static IDictionary<Frame.Coordinate, Color>
        ConvertBitmapPixelsToMapping(SKBitmap bitmap)
    {
        return bitmap.Pixels
           .Select(p => new Color(p.Red, p.Green, p.Blue))
           .Select((color, i) =>
            {
                var xIndex = i % bitmap.Width;
                var yIndex = i / bitmap.Width;

                return (
                    Coordinate: new Frame.Coordinate((uint)xIndex, (uint)yIndex),
                    Color: color
                );
            })
           .ToDictionary(
                kv => kv.Coordinate,
                kv => kv.Color
            );
    }
}