namespace WebLaunchPad.Images.Services;

public interface IGifConverter
{
    ICollection<Frame> GetFrames(Stream fileStream);
}