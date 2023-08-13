namespace WebLaunchPad.Images.Services;

public interface IGifConverter
{
    Task<ICollection<Frame>> GetFramesAsync(
        Stream fileStream,
        CancellationToken cancellationToken
    );
}