namespace WebLaunchPad.Images.Services;

public interface IImageConverter
{
    Task<IDictionary<(uint, uint), Color>> GetPixelMappingAsync(
        Stream fileStream,
        CancellationToken cancellationToken
    );
}